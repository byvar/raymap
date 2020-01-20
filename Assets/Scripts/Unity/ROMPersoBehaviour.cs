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

public class ROMPersoBehaviour : MonoBehaviour {
    bool loaded = false;
    public Perso perso;
    public SectorComponent sector;
    public Controller controller;

    // States
    bool hasStates = false;
    public State state = null;
    public string[] stateNames = { "Placeholder" };
    int currentState = 0;
    public int stateIndex = 0;
    public bool autoNextState = false;

    // Physical object lists
    public string[] poListNames = { "Null" };
    int currentPOList = 0;
    public int poListIndex = 0;

	// Animation
	ushort currentShAnim = 0xFFFF;
	ROMShAnimation shAnim;
	ROMAnimationCutTable.AnimationCut[] animCuts;
	int currentAnimCut;
    bool forceAnimUpdate = false;
    public uint currentFrame = 0;
    public bool playAnimation = true;
    public float animationSpeed = 15f;
    private float updateCounter = 0f;
    public GameObject[][] subObjects { get; private set; } = null; // [channel][ntto]
    public GameObject[] channelObjects { get; private set; }
	private int[] currentActivePO = null;
	private bool[] channelParents = null;
    public AnimMorphData[,] morphDataArray;
    private Dictionary<short, List<int>> channelIDDictionary = new Dictionary<short, List<int>>();
	private Dictionary<ushort, GameObject>[] fullMorphPOs = null;
	private Dictionary<CollideType, GameObject[]> collSetObjects = null;
	public Dictionary<byte, Vector3> objectIndexScales = new Dictionary<byte, Vector3>();
	private bool isAlways = false;
	public bool IsAlways {
		get { return isAlways; }
		set { isAlways = value; }
	}
	private bool isEnabled = true;
	public bool IsEnabled {
		get { return isEnabled; }
		set {
			isEnabled = value;
			//controller.UpdatePersoActive(perso);
		}
	}
	

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
				stateNames = states.Select(s => (s.Value == null ? "Null" : "State " + s.Value.Index)).ToArray();
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
		loaded = true;
    }

    #region Print debug info
    public void PrintAnimationDebugInfo() {
		if (loaded && hasStates) { }
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
        if (loaded) {
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
		if (sector == null || isAlways || sector.Loaded) sectorActive = true;
		if (sector == null || isAlways || controller.sectorManager.activeSector != null) insideSectors = true;
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
        loaded = false;
        // Destroy currently loaded subobjects
        if (subObjects != null) {
            for (int i = 0; i < subObjects.Length; i++) {
                if (subObjects[i] == null) continue;
                for (int j = 0; j < subObjects[i].Length; j++) {
					if (subObjects[i][j] != null) {
						GameObject.Destroy(subObjects[i][j]);
					}
                }
            }
            subObjects = null;
        }
        if (channelObjects != null) {
            for (int i = 0; i < channelObjects.Length; i++) {
				if (channelObjects[i] != null) {
					Destroy(channelObjects[i]);
				}
            }
            channelObjects = null;
        }
		if (fullMorphPOs != null) {
			for (int i = 0; i < fullMorphPOs.Length; i++) {
				if (fullMorphPOs[i] != null) {
					foreach (GameObject po in fullMorphPOs[i].Values) {
						GameObject.Destroy(po);
					}
					fullMorphPOs[i].Clear();
				}
			}
			fullMorphPOs = null;
		}
        channelIDDictionary.Clear();
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
			DeinitAnimation();
			// Init animation
			this.currentAnimCut = newAnimCut;

			ROMAnimation anim = ROMStruct.Loader.romAnims[animCuts[currentAnimCut].index];
			if (anim != null) {
				//animationSpeed = a3d.speed;
				// Init channels & subobjects
				subObjects = new GameObject[anim.a3d.num_channels][];
				channelObjects = new GameObject[anim.a3d.num_channels];
				if (anim.a3d.num_morphData > 0) fullMorphPOs = new Dictionary<ushort, GameObject>[anim.a3d.num_channels];
				currentActivePO = new int[anim.a3d.num_channels];
				channelParents = new bool[anim.a3d.num_channels];
				for (int i = 0; i < anim.a3d.num_channels; i++) {
					short id = anim.channels[i].id;
					channelObjects[i] = new GameObject("Channel " + id);
					channelObjects[i].transform.SetParent(transform);
					currentActivePO[i] = -1;
					AddChannelID(id, i);
					subObjects[i] = new GameObject[anim.a3d.num_NTTO];
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
					for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
						int j = listOfNTTOforChannel[k];
						AnimNTTO ntto = anim.ntto[j];
						if (ntto.IsInvisibleNTTO) {
							subObjects[i][j] = new GameObject();
							subObjects[i][j].transform.parent = channelObjects[i].transform;
							subObjects[i][j].name = "[" + j + "] Invisible NTTO";
							subObjects[i][j].SetActive(false);
						} else {
							if (perso.p3dData.Value.objectsTable.Value != null
								&& perso.p3dData.Value.objectsTable.Value.length > ntto.object_index) {
								ObjectsTableData.Entry entry = perso.p3dData.Value.objectsTable.Value.data.Value.entries[ntto.object_index];
								PhysicalObject o = entry.obj.Value;
								if (o != null) {
									//if (o.visualSetType == 1) print(name);
									GameObject c = o.GetGameObject().gameObject;
									subObjects[i][j] = c;
									if (entry.scale.HasValue) {
										objectIndexScales[ntto.object_index] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
										subObjects[i][j].transform.localScale = objectIndexScales[ntto.object_index];
									}
									c.transform.parent = channelObjects[i].transform;
									c.name = "[" + j + "] " + c.name;
									c.SetActive(false);
								}
							}
						}
					}
				}


				if (anim.a3d.num_morphData > 0 && anim.morphData != null) {
					morphDataArray = new AnimMorphData[anim.channels.Length, anim.onlyFrames.Length];
					// Iterate over morph data to find the correct channel and keyframe
					for (int i = 0; i < anim.a3d.num_morphData; i++) {
						AnimMorphData m = anim.morphData[i];
						if (m != null) {
							/*print("F:" + a3d.num_onlyFrames + " - C:" + a3d.num_channels + " - CF" + (a3d.num_onlyFrames * a3d.num_channels) + " - " +
								m.channel + " - " + m.frame + " - " + m.morphProgress + " - " + m.objectIndexTo + " - " + m.byte5 + " - " + m.byte6 + " - " + m.byte7);*/
							int channelIndex = this.GetChannelByID(m.channel)[0];
							if (channelIndex < morphDataArray.GetLength(0) && m.frame < morphDataArray.GetLength(1)) {
								morphDataArray[channelIndex, m.frame] = m;
								if (m.morphProgress == 100 && perso.p3dData.Value.objectsTable.Value != null
								&& perso.p3dData.Value.objectsTable.Value.length > m.objectIndexTo) {

									if (fullMorphPOs[channelIndex] == null) fullMorphPOs[channelIndex] = new Dictionary<ushort, GameObject>();
									if (!fullMorphPOs[channelIndex].ContainsKey(m.objectIndexTo)) {
										ObjectsTableData.Entry entry = perso.p3dData.Value.objectsTable.Value.data.Value.entries[m.objectIndexTo];
										PhysicalObject o = entry.obj.Value;
										if (o != null) {
											//if (o.visualSetType == 1) print(name);
											GameObject c = o.GetGameObject().gameObject;
											fullMorphPOs[channelIndex][m.objectIndexTo] = c;
											if (entry.scale.HasValue) {
												objectIndexScales[m.objectIndexTo] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
												c.transform.localScale = objectIndexScales[m.objectIndexTo];
											}
											/*subObjects[i][j].transform.localScale =
												subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;*/
											c.transform.parent = channelObjects[channelIndex].transform;
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
				if (!isAlways) {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.None);
				}
			}
			loaded = true;
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
				SwitchAnimationCut(0);
			}
		}
	}

	public void UpdateAnimation() {
		if (loaded && shAnim != null && animCuts != null && animCuts.Length > 0 && channelObjects != null & subObjects != null) {
			if (currentFrame >= shAnim.num_frames) currentFrame %= shAnim.num_frames;
			UpdateAnimationCut();
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
			}
			ROMAnimation anim = ROMStruct.Loader.romAnims[animCuts[currentAnimCut].index];
			uint currentRelativeFrame = currentFrame - anim.a3d.this_start_onlyFrames;
			AnimOnlyFrame of = anim.onlyFrames[currentRelativeFrame];
			// Create hierarchy for this frame
			for (int i = of.start_hierarchies_for_frame;
				i < of.start_hierarchies_for_frame + of.num_hierarchies_for_frame; i++) {
				AnimHierarchy h = anim.hierarchies[i];

				if (!channelIDDictionary.ContainsKey(h.childChannelID) || !channelIDDictionary.ContainsKey(h.parentChannelID)) {
					continue;
				}
				List<int> ch_child_list = GetChannelByID(h.childChannelID);
				List<int> ch_parent_list = GetChannelByID(h.parentChannelID);
				foreach (int ch_child in ch_child_list) {
					foreach (int ch_parent in ch_parent_list) {
						channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
						channelParents[ch_child] = true;
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
					physicalObject = subObjects[i][poNum];
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

				if (poNum != currentActivePO[i]) {
					if (currentActivePO[i] == -2 && fullMorphPOs != null && fullMorphPOs[i] != null) {
						foreach (GameObject morphPO in fullMorphPOs[i].Values) {
							if (morphPO.activeSelf) morphPO.SetActive(false);
						}
					}
					if (currentActivePO[i] >= 0 && subObjects[i][currentActivePO[i]] != null) {
						subObjects[i][currentActivePO[i]].SetActive(false);
					}
					currentActivePO[i] = poNum;
					if (physicalObject != null) physicalObject.SetActive(true);
				}
				if (!channelParents[i]) channelObjects[i].transform.SetParent(transform);
				channelObjects[i].transform.localPosition = vector;// * positionMultiplier;
				channelObjects[i].transform.localRotation = quaternion;
				channelObjects[i].transform.localScale = scale;

				if (physicalObject != null && anim.a3d.num_morphData > 0 && morphDataArray != null && i < morphDataArray.GetLength(0) && currentRelativeFrame < morphDataArray.GetLength(1)) {
					AnimMorphData morphData = morphDataArray[i, currentRelativeFrame];
					GeometricObject ogPO = perso.p3dData.Value.objectsTable.Value.data.Value.entries[anim.ntto[poNum].object_index].obj.Value.visual.Value;
					if (morphData != null && morphData.morphProgress != 0 && morphData.morphProgress != 100) {
						//print("morphing " + physicalObject.name);
						GeometricObject morphToPO = perso.p3dData.Value.objectsTable.Value.data.Value.entries[morphData.objectIndexTo].obj.Value.visual.Value;
						ogPO.MorphVertices(physicalObject, morphToPO, morphData.morphProgress / 100f);
					} else if (morphData != null && morphData.morphProgress == 100) {
						physicalObject.SetActive(false);
						GameObject c = fullMorphPOs[i][morphData.objectIndexTo];
						c.transform.localScale = objectIndexScales.ContainsKey(morphData.objectIndexTo) ? objectIndexScales[morphData.objectIndexTo] : Vector3.one;
						c.transform.localPosition = Vector3.zero;
						c.transform.localRotation = Quaternion.identity;
						c.SetActive(true);
						currentActivePO[i] = -2;
					} else {
						ogPO.ResetMorph(physicalObject);
					}
				}
			}
		}
    }

	List<int> GetChannelByID(short id) {
        if (channelIDDictionary.ContainsKey(id)) {
            return channelIDDictionary[id];
        } else return new List<int>();
    }

    void AddChannelID(short id, int index) {
        // Apparently there can be multiple channels with the ID -1, so this requires a list
        if (!channelIDDictionary.ContainsKey(id) || channelIDDictionary[id] == null) {
            channelIDDictionary[id] = new List<int>();
        }
        channelIDDictionary[id].Add(index);
    }

    void GotoAutoNextState() {
        /*if (state != null && state.off_state_auto != null) {
            State state_auto = State.FromOffset(perso.p3dData.family, state.off_state_auto);
            if (state_auto != null) {
                int indexOfStateAuto = perso.p3dData.family.states.IndexOf(state_auto);
                if (indexOfStateAuto > -1) SetState(indexOfStateAuto);
            }
        }*/
    }

    public void PrintTranslatedScripts()
    {
        if (loaded && hasStates) {
            if (perso.brain.Value != null
                && perso.brain.Value.mind.Value != null) {
                ComportList intelligenceNormal = perso.brain.Value.mind.Value.comportsIntelligence.Value;
                ComportList intelligenceReflex = perso.brain.Value.mind.Value.comportsReflex.Value;

                List<string> ruleStatesInitializer = new List<string>();
                List<string> reflexStatesInitializer = new List<string>();

                var rules = intelligenceNormal.comports?.Value?.comports;
                var reflexes = intelligenceReflex.comports?.Value?.comports;

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