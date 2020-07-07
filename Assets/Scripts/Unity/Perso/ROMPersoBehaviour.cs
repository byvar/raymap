using Assets.Scripts;
using OpenSpace;
using OpenSpace.ROM;
using OpenSpace.ROM.ANIM.Component;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

public class ROMPersoBehaviour : BasePersoBehaviour {
    public Perso perso;
	public SuperObjectDynamic superObject;

    // States
    public State state { get; private set; } = null;

	// Animation
	ushort currentShAnim = 0xFFFF;
	ROMShAnimation shAnim;
	ROMAnimationCutTable.AnimationCut[] animCuts;
	int currentAnimCut;

	// Arrays
    public GameObject[][][] subObjects { get; private set; } = null; // [cut][channel][ntto]
	public AnimMorphData[][,] morphDataArray;
	private Dictionary<ushort, GameObject>[][] fullMorphPOs = null;
	private Dictionary<CollideType, GameObject[]> collSetObjects = null;
	private Dictionary<byte, Vector3> objectIndexScales = new Dictionary<byte, Vector3>();
	public new GameObject[][] channelObjects { get; protected set; }
	protected new int[][] currentActivePO = null;
	protected new bool[][] channelParents = null;
	protected new Dictionary<short, List<int>>[] channelIDDictionary;

	// Abstract properties
	public override Pointer Offset => perso?.Offset;
	public override string NameFamily => _nameFamily ?? $"Family_{perso.stdGame.Value.family.Value.IndexString}";
	public override string NameModel => _nameModel ?? $"Model_{(perso.brain.Value?.aiModel.Value?.IndexString ?? "null")}";
	public override string NameInstance => _nameInstance ?? $"Instance_{perso.stdGame.Value.family.Value.IndexString}";
	public string _nameFamily { get; set; }
	public string _nameModel { get; set; }
	public string _nameInstance { get; set; }


	// Use this for initialization
	void Start() {
    }

    public void Init() {
        MapLoader l = MapLoader.Loader;
		if (perso != null && perso.p3dData.Value != null && perso.stdGame.Value != null) {
			// N64/DS/3DS
			Family fam = perso.stdGame.Value.family;
			List<ObjectsTable> objectsTables = ROMStruct.Loader.objectsTables;
			Array.Resize(ref poListNames, objectsTables.Count + 1);
			Array.Copy(objectsTables.Select(o => "Objects Table " + o.Index + " - count: " + o.length).ToArray(), 0, poListNames, 1, poListNames.Length - 1);
			currentPOList = perso.p3dData.Value.objectsTable.Value == null ? 0 : objectsTables.IndexOf(perso.p3dData.Value.objectsTable.Value) + 1;
			if (currentPOList == -1) currentPOList = 0;
			poListIndex = currentPOList;

			if (fam != null && fam.states.Value != null && fam.states.Value.states.Value != null && fam.states.Value.states.Value.states.Length > 0) {
				OpenSpace.ROM.Reference<OpenSpace.ROM.State>[] states = fam.states.Value.states.Value.states;
				stateNames = states.Select(s => (s.Value == null ? "Null" : s.Value.ToString())).ToArray();
				hasStates = true;
				state = perso.p3dData.Value.currentState.Value;
				for (int i = 0; i < states.Length; i++) {
					if (state == states[i].Value) {
						currentState = i;
						stateIndex = i;
						SetState(i);
						break;
					}
				}
				if (state == null && states.Length > 0) {
					currentState = 0;
					stateIndex = 0;
					SetState(0);
				}
			}
		}
		IsLoaded = true;
    }

    #region Print debug info
    public void PrintAnimationDebugInfo() {
		if (IsLoaded && hasStates) { }
    }
	#endregion

	public void UpdateViewCollision(bool viewCollision) {
		if (perso.collset.Value != null) {
			CollSet c = perso.collset.Value;
			if (collSetObjects == null) {
				collSetObjects = new Dictionary<CollideType, GameObject[]>();
				foreach (KeyValuePair<CollideType, Reference<ZdxList>> entry in c.zdxList) {
					if (entry.Value.Value != null) {
						ZdxList zdx = entry.Value.Value;
						collSetObjects[entry.Key] = new GameObject[zdx.num_objects];
						if (zdx.num_objects > 0 && zdx.objects.Value != null) {
							for (int i = 0; i < zdx.objects.Value.objects.Length; i++) {
								GeometricObject geo = zdx.objects.Value.objects[i].Value;
								if (geo == null) continue;
								collSetObjects[entry.Key][i] = geo.GetGameObject(GeometricObject.Type.Collide, entry.Key);
								collSetObjects[entry.Key][i].transform.SetParent(transform);
								collSetObjects[entry.Key][i].transform.localPosition = Vector3.zero;
								collSetObjects[entry.Key][i].transform.localRotation = Quaternion.identity;
								collSetObjects[entry.Key][i].transform.localScale = Vector3.one;
								/*if(viewCollision && 
								if (viewCollision && c.GetPrivilegedActionZoneStatus(entry.Key, i) == CollSet.PrivilegedActivationStatus.ForceActive) {
									col.SetVisualsActive(true);
								} else {
									col.SetVisualsActive(false);
								}*/
							}
						}
					}
				}
			}
			foreach (KeyValuePair<CollideType, GameObject[]> entry in collSetObjects) {
				if (entry.Value != null && entry.Value.Length > 0) {
					foreach (GameObject gao in entry.Value) {
						if (gao != null) gao.SetActive(false);
					}
				}
			}
			if (viewCollision) {
				foreach (KeyValuePair<CollideType, Reference<ActivationList>> entry in c.activationList) {
					if (!collSetObjects.ContainsKey(entry.Key)) continue;
					if (entry.Value.Value != null && entry.Value.Value.num_objects > 0 && entry.Value.Value.objects.Value != null) {
						ActivationZoneArray azr = entry.Value.Value.objects.Value;
						if (azr.elements.Length == 0) continue;
						for (int i = 0; i < azr.elements.Length; i++) {
							if (azr.elements[i].state.Value == state) {
								ActivationZone zone = azr.elements[i].activationList.Value;
								if (zone != null && zone.num_objects > 0 && zone.objects.Value != null) {
									foreach (ushort act in zone.objects.Value.objects) {
										if (collSetObjects[entry.Key].Length > act
											&& collSetObjects[entry.Key][act] != null) {
											collSetObjects[entry.Key][act].SetActive(true);
										}
									}
								}
								break;
							}
						}
					}
				}
			}
		}
	}

	private void SetState(State state)
    {
		this.state = state;
        UpdateViewCollision(controller.viewCollision);
        // Set animation
        MapLoader l = MapLoader.Loader;
		//print(name + " - " + state.Index + " - " + perso.p3dData.Value.currentState.index);
		if (state.anim.Value != null && state.anim.Value.animIndex != 0xFFFF) {
			animationSpeed = state.speed;
			LoadNewAnimation(state.anim.Value.animIndex);
			UpdateAnimation();
		} else {
			LoadNewAnimation(0xFFFF);
		}
    }

	public void SetState(int index) {
		Family fam = perso.stdGame.Value.family;
		Reference<OpenSpace.ROM.State>[] states = fam.states.Value.states.Value.states;
		//stateNames = states.Select(s => (s.Value == null ? "Null" : "State " + s.Value.Index)).ToArray();
		if (index < 0 || index >= states.Length) return;
		stateIndex = index;
		currentState = index;
		state = states[index].Value;
        SetState(state);
    }

    // Update is called once per frame
    void Update() {
        if (IsLoaded) {
            if (hasStates) {
                if (stateIndex != currentState) {
                    currentState = stateIndex;
                    SetState(currentState);
                }
            }
            MapLoader l = MapLoader.Loader;
            if (poListIndex != currentPOList && perso.p3dData != null) {
                if (poListIndex > 0 && poListIndex < poListNames.Length + 1) {
                    currentPOList = poListIndex;
					ObjectsTable newOT = ROMStruct.Loader.objectsTables[currentPOList - 1];
					perso.p3dData.Value.objectsTable = new Reference<ObjectsTable>(newOT.Index, newOT);
                } else {
                    poListIndex = 0;
                    currentPOList = 0;
					perso.p3dData.Value.objectsTable = new Reference<ObjectsTable>();
				}
                forceAnimUpdate = true;
                SetState(currentState);
			}
		}
        bool sectorActive = false, insideSectors = false;
		if (sector == null || IsAlways || sector.Loaded) sectorActive = true;
		if (sector == null || IsAlways || controller.sectorManager.activeSector != null) insideSectors = true;
        if (controller.playAnimations && playAnimation && sectorActive) {
            updateCounter += Time.deltaTime * animationSpeed;
            // If the camera is not inside a sector, animations will only update 1 out of 2 times (w/ frameskip) to avoid lag
            if ((!insideSectors && updateCounter >= 2f) || (insideSectors && updateCounter >= 1f)) {
                uint passedFrames = (uint)Mathf.FloorToInt(updateCounter);
                updateCounter %= 1;
                currentFrame += passedFrames;
                if (shAnim != null && currentFrame >= shAnim.num_frames) {
                    if (autoNextState) {
                        ROMShAnimation prevshAnim = shAnim;
                        GotoAutoNextState();
                        if (shAnim == prevshAnim) {
                            currentFrame = currentFrame % shAnim.num_frames;
                            UpdateAnimation();
                        }
                    } else {
                        currentFrame = currentFrame % shAnim.num_frames;
                        UpdateAnimation();
                    }
                } else {
                    UpdateAnimation();
                }
            }
        }
    }

    void DeinitAnimation() {
        IsLoaded = false;
        // Destroy currently loaded subobjects
        if (subObjects != null) {
            for (int i = 0; i < subObjects.Length; i++) {
                if (subObjects[i] == null) continue;
                for (int j = 0; j < subObjects[i].Length; j++) {
					if (subObjects[i][j] == null) continue;
					for (int k = 0; k < subObjects[i][j].Length; k++) {
						if (subObjects[i][j][k] != null) {
							GameObject.Destroy(subObjects[i][j][k]);
						}
					}
                }
            }
            subObjects = null;
        }
        if (channelObjects != null) {
            for (int i = 0; i < channelObjects.Length; i++) {
				if (channelObjects[i] == null) continue;
				for (int j = 0; j < channelObjects[i].Length; j++) {
					if (channelObjects[i][j] != null) {
						Destroy(channelObjects[i][j]);
					}
				}
            }
            channelObjects = null;
        }
		if (fullMorphPOs != null) {
			for (int i = 0; i < fullMorphPOs.Length; i++) {
				if (fullMorphPOs[i] != null) {
					for (int j = 0; j < fullMorphPOs[i].Length; j++) {
						if (fullMorphPOs[i][j] != null) {
							foreach (GameObject po in fullMorphPOs[i][j].Values) {
								GameObject.Destroy(po);
							}
							fullMorphPOs[i][j].Clear();
						}
					}
				}
			}
			fullMorphPOs = null;
		}
		if (channelIDDictionary != null) {
			for (int i = 0; i < channelIDDictionary.Length; i++) {
				if (channelIDDictionary[i] == null) continue;
				channelIDDictionary[i].Clear();
			}
			channelIDDictionary = null;
		}
		objectIndexScales.Clear();
    }

	void UpdateAnimationCut() {
		if (shAnim != null && animCuts != null && animCuts.Length > 0) {
			for (int i = 0; i < animCuts.Length; i++) {
				if (currentFrame < animCuts[i].currentTotalFrames) {
					if(i != currentAnimCut) SwitchAnimationCut(i);
					break;
				}
			}
			if (currentAnimCut >= animCuts.Length) {
				SwitchAnimationCut(0);
			}
		}
	}

	void SwitchAnimationCut(int newAnimCut) {
		if (currentAnimCut != newAnimCut || forceAnimUpdate) {
			forceAnimUpdate = false;
			// Init animation
			this.currentAnimCut = newAnimCut;

			ROMAnimation anim = ROMStruct.Loader.romAnims[animCuts[currentAnimCut].index];
			bool newCut = subObjects[currentAnimCut] == null;
			if (anim != null && newCut) {
				//animationSpeed = a3d.speed;
				// Init channels & subobjects
				channelObjects[currentAnimCut] = new GameObject[anim.a3d.num_channels];
				currentActivePO[currentAnimCut] = new int[anim.a3d.num_channels];
				channelParents[currentAnimCut] = new bool[anim.a3d.num_channels];
				subObjects[currentAnimCut] = new GameObject[anim.a3d.num_channels][];
				if (anim.a3d.num_morphData > 0) fullMorphPOs[currentAnimCut] = new Dictionary<ushort, GameObject>[anim.a3d.num_channels];
				for (int i = 0; i < anim.a3d.num_channels; i++) {
					short id = anim.channels[i].id;
					channelObjects[currentAnimCut][i] = new GameObject($"[Cut {currentAnimCut}] Channel {id}");
					channelObjects[currentAnimCut][i].transform.SetParent(transform);
					currentActivePO[currentAnimCut][i] = -1;
					AddChannelID(id, i);
					if (newCut) {
						subObjects[currentAnimCut][i] = new GameObject[anim.a3d.num_NTTO];
					}
					AnimChannel ch = anim.channels[i];
					List<ushort> listOfNTTOforChannel = new List<ushort>();
					for (int j = 0; j < anim.onlyFrames.Length; j++) {
						AnimOnlyFrame of = anim.onlyFrames[j];
						//print(ch.numOfNTTO + " - " + of.numOfNTTO + " - " + a3d.numOfNTTO.Length);
						AnimNumOfNTTO numOfNTTO = anim.numOfNTTO[(i *anim.a3d.num_numNTTO) + of.numOfNTTO];
						if (!listOfNTTOforChannel.Contains(numOfNTTO.numOfNTTO) && numOfNTTO.numOfNTTO != 0xFFFF) {
							listOfNTTOforChannel.Add(numOfNTTO.numOfNTTO);
						}
					}
					if (newCut) {
						for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
							int j = listOfNTTOforChannel[k];
							AnimNTTO ntto = anim.ntto[j];
							if (ntto.IsInvisibleNTTO) {
								subObjects[currentAnimCut][i][j] = new GameObject();
								subObjects[currentAnimCut][i][j].transform.parent = channelObjects[currentAnimCut][i].transform;
								subObjects[currentAnimCut][i][j].name = "[" + j + "] Invisible NTTO";
								subObjects[currentAnimCut][i][j].SetActive(false);
							} else {
								if (perso.p3dData.Value.objectsTable.Value != null
									&& perso.p3dData.Value.objectsTable.Value.length > ntto.object_index) {
									ObjectsTableData.Entry entry = perso.p3dData.Value.objectsTable.Value.data.Value.entries[ntto.object_index];
									PhysicalObject o = entry.obj.Value;
									if (o != null) {
										//if (o.visualSetType == 1) print(name);
										GameObject c = o.GetGameObject().gameObject;
										subObjects[currentAnimCut][i][j] = c;
										if (entry.scale.HasValue) {
											objectIndexScales[ntto.object_index] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
											subObjects[currentAnimCut][i][j].transform.localScale = objectIndexScales[ntto.object_index];
										}
										c.transform.parent = channelObjects[currentAnimCut][i].transform;
										c.name = "[" + j + "] " + c.name;
										c.SetActive(false);
									}
								}
							}
						}
					}
				}


				if (anim.a3d.num_morphData > 0 && anim.morphData != null) {
					morphDataArray[currentAnimCut] = new AnimMorphData[anim.channels.Length, anim.onlyFrames.Length];
					// Iterate over morph data to find the correct channel and keyframe
					for (int i = 0; i < anim.a3d.num_morphData; i++) {
						AnimMorphData m = anim.morphData[i];
						if (m != null) {
							/*print("F:" + a3d.num_onlyFrames + " - C:" + a3d.num_channels + " - CF" + (a3d.num_onlyFrames * a3d.num_channels) + " - " +
								m.channel + " - " + m.frame + " - " + m.morphProgress + " - " + m.objectIndexTo + " - " + m.byte5 + " - " + m.byte6 + " - " + m.byte7);*/
							int channelIndex = this.GetChannelByID(m.channel)[0];
							if (channelIndex < morphDataArray[currentAnimCut].GetLength(0) && m.frame < morphDataArray[currentAnimCut].GetLength(1)) {
								morphDataArray[currentAnimCut][channelIndex, m.frame] = m;
								if (m.morphProgress == 100 && perso.p3dData.Value.objectsTable.Value != null
								&& perso.p3dData.Value.objectsTable.Value.length > m.objectIndexTo) {

									if (fullMorphPOs[currentAnimCut][channelIndex] == null) fullMorphPOs[currentAnimCut][channelIndex] = new Dictionary<ushort, GameObject>();
									if (!fullMorphPOs[currentAnimCut][channelIndex].ContainsKey(m.objectIndexTo)) {
										ObjectsTableData.Entry entry = perso.p3dData.Value.objectsTable.Value.data.Value.entries[m.objectIndexTo];
										PhysicalObject o = entry.obj.Value;
										if (o != null) {
											//if (o.visualSetType == 1) print(name);
											GameObject c = o.GetGameObject().gameObject;
											fullMorphPOs[currentAnimCut][channelIndex][m.objectIndexTo] = c;
											if (entry.scale.HasValue) {
												objectIndexScales[m.objectIndexTo] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
												c.transform.localScale = objectIndexScales[m.objectIndexTo];
											}
											/*subObjects[i][j].transform.localScale =
												subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;*/
											c.transform.parent = channelObjects[currentAnimCut][channelIndex].transform;
											c.name = "[Morph] " + c.name;
											c.SetActive(false);
										}
									}
								}
							}
						}
					}
				} else {
					morphDataArray = null;
				}

				// Keep lighting last so that it is applied to all new sub objects
				if (!IsAlways) {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.None);
				}
			}
			if (channelObjects != null) {
				for (int i = 0; i < animCuts.Length; i++) {
					if (channelObjects[i] != null) {
						if (currentAnimCut == i) {
							foreach (GameObject g in channelObjects[i]) {
								g.SetActive(true);
							}
						} else {
							foreach (GameObject g in channelObjects[i]) {
								g.SetActive(false);
							}
						}
					}
				}
			}
			IsLoaded = true;
		}
	}

	void LoadNewAnimation(ushort shAnimIndex) {
		if (shAnimIndex != currentShAnim || forceAnimUpdate) {
			forceAnimUpdate = false;
			currentShAnim = shAnimIndex;
			DeinitAnimation();
			currentFrame = 0;
			shAnim = null;
			animCuts = null;
			if (shAnimIndex != 0xFFFF) {
				shAnim = ROMStruct.Loader.shAnims[shAnimIndex];
				animationSpeed = shAnim.speed;
				animCuts = ROMStruct.Loader.cutTable.GetAnimationChain(shAnimIndex);
				//print(animCuts.Length);
				forceAnimUpdate = true;
				InitAnimationCuts();
				SwitchAnimationCut(0);
			}
		}
	}

	private void InitAnimationCuts() {
		int cuts = animCuts.Length;
		channelObjects = new GameObject[cuts][];
		subObjects = new GameObject[cuts][][];
		fullMorphPOs = new Dictionary<ushort, GameObject>[cuts][];
		morphDataArray = new AnimMorphData[cuts][,];
		channelParents = new bool[cuts][];
		channelIDDictionary = new Dictionary<short, List<int>>[cuts];
		currentActivePO = new int[cuts][];
		for (int i = 0; i < cuts; i++) {
			channelIDDictionary[i] = new Dictionary<short, List<int>>();
		}
}

	public void UpdateAnimation() {
		if (IsLoaded && shAnim != null && animCuts != null && animCuts.Length > 0 && channelObjects != null && subObjects != null) {
			if (currentFrame >= shAnim.num_frames) currentFrame %= shAnim.num_frames;
			UpdateAnimationCut();
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents[currentAnimCut].Length; i++) {
				channelParents[currentAnimCut][i] = false;
			}
			ROMAnimation anim = ROMStruct.Loader.romAnims[animCuts[currentAnimCut].index];
			uint currentRelativeFrame = currentFrame - anim.a3d.this_start_onlyFrames;
			AnimOnlyFrame of = anim.onlyFrames[currentRelativeFrame];
			// Create hierarchy for this frame
			for (int i = of.start_hierarchies_for_frame;
				i < of.start_hierarchies_for_frame + of.num_hierarchies_for_frame; i++) {
				AnimHierarchy h = anim.hierarchies[i];

				if (!channelIDDictionary[currentAnimCut].ContainsKey(h.childChannelID) || !channelIDDictionary[currentAnimCut].ContainsKey(h.parentChannelID)) {
					continue;
				}
				List<int> ch_child_list = GetChannelByID(h.childChannelID);
				List<int> ch_parent_list = GetChannelByID(h.parentChannelID);
				foreach (int ch_child in ch_child_list) {
					foreach (int ch_parent in ch_parent_list) {
						channelObjects[currentAnimCut][ch_child].transform.SetParent(channelObjects[currentAnimCut][ch_parent].transform);
						channelParents[currentAnimCut][ch_child] = true;
					}
				}

				//channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
			}

			// Final pass
			int currentKFIndex = 0;
			for (int i = 0; i < anim.channels.Length; i++) {
				AnimChannel ch = anim.channels[i];

				// Select keyframe
				AnimKeyframe kf = null;
				int selectedKFindex = 0;
				for (int k = 0; k < ch.num_keyframes; k++) {
					if (anim.keyframes[currentKFIndex + k].frame <= currentFrame) {
						kf = anim.keyframes[currentKFIndex + k];
						selectedKFindex = k;
					} else {
						break;
					}
				}
				AnimVector pos = anim.vectors[kf.positionVector];
				AnimQuaternion qua = anim.quaternions[kf.quaternion];
				AnimVector scl = anim.vectors[kf.scaleVector];
				AnimNumOfNTTO numOfNTTO = anim.numOfNTTO[(i * anim.a3d.num_numNTTO) + of.numOfNTTO];
				AnimNTTO ntto = null;
				int poNum = -1;
				GameObject physicalObject = null;
				if (numOfNTTO.numOfNTTO != 0xFFFF) {
					poNum = numOfNTTO.numOfNTTO;
					ntto = anim.ntto[poNum];
					physicalObject = subObjects[currentAnimCut][i][poNum];
				}
				Vector3 vector = pos.vector;
				Quaternion quaternion = qua.quaternion;
				Vector3 scale = scl.vector;
				int framesSinceKF = (int)currentFrame - (int)kf.frame;
				AnimKeyframe nextKF = null;
				int framesDifference;
				float interpolation;
				if (selectedKFindex == ch.num_keyframes-1) {
					nextKF = anim.keyframes[currentKFIndex];
					framesDifference = anim.a3d.total_num_onlyFrames - 1 + (int)nextKF.frame - (int)kf.frame;
					if (framesDifference == 0) {
						interpolation = 0;
					} else {
						//interpolation = (float)(nextKF.interpolationFactor * (framesSinceKF / (float)framesDifference) + 1.0 * nextKF.interpolationFactor);
						interpolation = framesSinceKF / (float)framesDifference;
					}
				} else {
					nextKF = anim.keyframes[currentKFIndex + selectedKFindex + 1];
					framesDifference = (int)nextKF.frame - (int)kf.frame;
					//interpolation = (float)(nextKF.interpolationFactor * (framesSinceKF / (float)framesDifference) + 1.0 * nextKF.interpolationFactor);
					interpolation = framesSinceKF / (float)framesDifference;
				}

				currentKFIndex += ch.num_keyframes;


				//print(interpolation);
				//print(a3d.vectors.Length + " - " + nextKF.positionVector);
				//print(perso.p3dData.family.animBank);
				AnimVector pos2 = anim.vectors[nextKF.positionVector];
				AnimQuaternion qua2 = anim.quaternions[nextKF.quaternion];
				AnimVector scl2 = anim.vectors[nextKF.scaleVector];
				vector = Vector3.Lerp(pos.vector, pos2.vector, interpolation);
				quaternion = Quaternion.Lerp(qua.quaternion, qua2.quaternion, interpolation);
				scale = Vector3.Lerp(scl.vector, scl2.vector, interpolation);
				//float positionMultiplier = Mathf.Lerp(kf.positionMultiplier, nextKF.positionMultiplier, interpolation);

				if (poNum != currentActivePO[currentAnimCut][i]) {
					if (currentActivePO[currentAnimCut][i] == -2 && fullMorphPOs[currentAnimCut] != null && fullMorphPOs[currentAnimCut][i] != null) {
						foreach (GameObject morphPO in fullMorphPOs[currentAnimCut][i].Values) {
							if (morphPO.activeSelf) morphPO.SetActive(false);
						}
					}
					if (currentActivePO[currentAnimCut][i] >= 0 && subObjects[currentAnimCut][i][currentActivePO[currentAnimCut][i]] != null) {
						subObjects[currentAnimCut][i][currentActivePO[currentAnimCut][i]].SetActive(false);
					}
					currentActivePO[currentAnimCut][i] = poNum;
					if (physicalObject != null) physicalObject.SetActive(true);
				}
				if (!channelParents[currentAnimCut][i]) channelObjects[currentAnimCut][i].transform.SetParent(transform);
				channelObjects[currentAnimCut][i].transform.localPosition = vector;// * positionMultiplier;
				channelObjects[currentAnimCut][i].transform.localRotation = quaternion;
				channelObjects[currentAnimCut][i].transform.localScale = scale;

				if (physicalObject != null
					&& anim.a3d.num_morphData > 0 
					&& morphDataArray[currentAnimCut] != null 
					&& i < morphDataArray[currentAnimCut].GetLength(0) 
					&& currentRelativeFrame < morphDataArray[currentAnimCut].GetLength(1)) {
					AnimMorphData morphData = morphDataArray[currentAnimCut][i, currentRelativeFrame];
					GeometricObject ogPO = perso.p3dData.Value.objectsTable.Value.data.Value.entries[anim.ntto[poNum].object_index].obj.Value.visual.Value;
					if (morphData != null && morphData.morphProgress != 0 && morphData.morphProgress != 100) {
						//print("morphing " + physicalObject.name);
						GeometricObject morphToPO = perso.p3dData.Value.objectsTable.Value.data.Value.entries[morphData.objectIndexTo].obj.Value.visual.Value;
						ogPO.MorphVertices(physicalObject, morphToPO, morphData.morphProgress / 100f);
					} else if (morphData != null && morphData.morphProgress == 100) {
						physicalObject.SetActive(false);
						GameObject c = fullMorphPOs[currentAnimCut][i][morphData.objectIndexTo];
						c.transform.localScale = objectIndexScales.ContainsKey(morphData.objectIndexTo) ? objectIndexScales[morphData.objectIndexTo] : Vector3.one;
						c.transform.localPosition = Vector3.zero;
						c.transform.localRotation = Quaternion.identity;
						c.SetActive(true);
						currentActivePO[currentAnimCut][i] = -2;
					} else {
						ogPO.ResetMorph(physicalObject);
					}
				}
			}
		}
    }

	List<int> GetChannelByID(short id) {
        if (channelIDDictionary[currentAnimCut].ContainsKey(id)) {
            return channelIDDictionary[currentAnimCut][id];
        } else return new List<int>();
    }

    void AddChannelID(short id, int index) {
        // Apparently there can be multiple channels with the ID -1, so this requires a list
        if (!channelIDDictionary[currentAnimCut].ContainsKey(id) || channelIDDictionary[currentAnimCut][id] == null) {
            channelIDDictionary[currentAnimCut][id] = new List<int>();
        }
        channelIDDictionary[currentAnimCut][id].Add(index);
    }

    void GotoAutoNextState() {
        if (state != null && state.state_auto.Value != null) {
			State state_auto = state.state_auto;
            if (state_auto != null) {
				State[] states = perso.stdGame.Value.family.Value.states.Value.states.Value.states.Select(s => s.Value).ToArray();
				//int indexOfStateAuto = Array.IndexOf(stateNames, state_auto.ToString());
				int indexOfStateAuto = Array.IndexOf(states, state_auto);
                if (indexOfStateAuto > -1) SetState(indexOfStateAuto);
            }
        }
    }

    public void PrintTranslatedScripts()
    {
        if (IsLoaded && hasStates) {
            if (perso.brain.Value != null
                && perso.brain.Value.aiModel.Value != null) {
                ComportList comportsIntelligence = perso.brain.Value.aiModel.Value.comportsIntelligence.Value;
                ComportList comportsReflex = perso.brain.Value.aiModel.Value.comportsReflex.Value;

                List<string> ruleStatesInitializer = new List<string>();
                List<string> reflexStatesInitializer = new List<string>();

                var rules = comportsIntelligence.comports?.Value?.comports;
                var reflexes = comportsReflex.comports?.Value?.comports;

                if (rules != null) {
                    MapLoader.Loader.print("Normal behaviours");
                    for (int i = 0; i < rules.Length; i++) {
                        var scripts = rules[i].Value?.scripts?.Value?.scripts;
                        if (scripts != null) {
                            string combinedScript = "private IEnumerator Rule_" + i + "() {" + Environment.NewLine;
                            ruleStatesInitializer.Add("Rule_" + i);
                            for (int j = 0; j < scripts.Length; j++) {
                                TranslatedROMScript ts = new TranslatedROMScript(scripts[j], perso);
                                combinedScript += "// Script " + j + Environment.NewLine + ts.ToString() + Environment.NewLine;
                            }
                            combinedScript += "yield return null;" + Environment.NewLine + "}";
                            MapLoader.Loader.print(combinedScript);
                        }
                    }
                }
                if (reflexes != null) {
                    MapLoader.Loader.print("Reflex behaviours");
                    for (int i = 0; i < reflexes.Length; i++) {
                        var scripts = rules[i].Value?.scripts?.Value?.scripts;
                        if (scripts != null) {
                            string combinedScript = "private IEnumerator Reflex_" + i + "() {" + Environment.NewLine;
                            for (int j = 0; j < scripts.Length; j++) {
                                reflexStatesInitializer.Add("Reflex_" + i);
                                TranslatedROMScript ts = new TranslatedROMScript(scripts[j], perso);
                                combinedScript += "// Script " + j + Environment.NewLine + ts.ToString() + Environment.NewLine;
                            }
                            combinedScript += "yield return null;" + Environment.NewLine + "}";
                            MapLoader.Loader.print(combinedScript);
                        }
                    }
                }
                

                // TODO: replace evalMacro calls by replacing regex "evalMacro\([a-zA-Z0-9_]*\.Macro\[([0-9]+)\]\)" to "yield return Macro_$1()"
                // TODO: replace Proc_ChangeMyComport\([a-zA-Z0-9_]+\.Rule\[[0-9]+\]\[\"([^"]+)\"\]\)     with     sm.ChangeActiveRuleState("$1")
                // TODO: replace Cond_IsValidObject\(([^\)]+)\)    with $1 != null

                string startString = "protected override void Start() {" + Environment.NewLine + "base.Start();" + Environment.NewLine + Environment.NewLine;
                startString += "// Rules" + Environment.NewLine;
                ruleStatesInitializer.ForEach((s) => { startString += "sm.AddRuleState(State.Create(\"" + s + "\", " + s + "));" + Environment.NewLine; });
                startString += Environment.NewLine + "// Reflexes " + Environment.NewLine;
                reflexStatesInitializer.ForEach((s) => { startString += "sm.AddReflexState(State.Create(\"" + s + "\", " + s + "));" + Environment.NewLine; });
                startString += Environment.NewLine + "sm.ChangeActiveRuleState(0);" + Environment.NewLine + "sm.ChangeActiveReflexState(0);" + Environment.NewLine + "}";

                MapLoader.Loader.print(startString);
                
            }
        }
    }

}