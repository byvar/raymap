using Assets.Scripts;
using OpenSpace;
using OpenSpace.Loader;
using OpenSpace.PS1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CollideType = OpenSpace.Collide.CollideType;

public class PS1PersoBehaviour : MonoBehaviour {
    public bool IsLoaded { get; private set; } = false;
    public Perso perso;
    public SectorComponent sector;
    public Controller controller;

    // States
    bool hasStates = false;
	public State[] states;
    public State state { get; private set; } = null;
    public string[] stateNames = { "Placeholder" };
	public int currentState { get; private set; } = 0; // Follows "stateIndex", sometimes with a small delay.
	public int stateIndex = 0; // Set this variable
	public bool autoNextState = false;

	// Animation
	LevelHeader h;
	PS1Animation anim;
	int animIndex = -1;
    bool forceAnimUpdate = false;
    public uint currentFrame = 0;
    public bool playAnimation = true;
    public float animationSpeed = 15f;
    private float updateCounter = 0f;
    public GameObject[][] subObjects { get; private set; } = null; // [channel][ntto]
    public GameObject[] channelObjects { get; private set; }
	private int[] currentActivePO = null;
	private bool[] channelParents = null;
	private ushort[][] channelNTTO;
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
		R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
		h = l.levelHeader;
		if (perso != null && perso.p3dData != null && perso.p3dData.family != null) {
			Family fam = perso.p3dData.family;
			if (fam.animations != null) {
				states = h.states.pointers
					.Where(s => s.Value != null && (s.Value.anim?.index == null || s.Value.anim.index < fam.animations.Length))
					.Select(s => s.Value).ToArray();
				stateNames = states.Select(s => ((s.anim == null) ? "Null" : fam.animations[s.anim.index].name)).ToArray();
				hasStates = true;
				stateIndex = Array.IndexOf(states, h.states.pointers[perso.p3dData.stateIndex].Value);
				currentState = stateIndex;
				SetState(stateIndex);
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
		/*if (perso.collset.Value != null) {
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
		}*/
	}

	private void SetState(State state)
    {
		this.state = state;
        UpdateViewCollision(controller.viewCollision);
        // Set animation
        MapLoader l = MapLoader.Loader;
		//print(name + " - " + state.Index + " - " + perso.p3dData.Value.currentState.index);
		if (state.anim != null) {
			animationSpeed = state.speed;
			LoadNewAnimation((int)state.anim.index);
			UpdateAnimation();
		} else {
			LoadNewAnimation(-1);
		}
    }

	public void SetState(int index) {
		Family fam = perso.p3dData.family;
		//stateNames = states.Select(s => (s.Value == null ? "Null" : "State " + s.Value.Index)).ToArray();
		if (index < 0 || index >= states.Length) return;
		stateIndex = index;
		currentState = index;
		state = states[index];
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
                if (anim != null && currentFrame >= anim.num_frames) {
                    if (autoNextState) {
                        PS1Animation prevAnim = anim;
                        GotoAutoNextState();
                        if (anim == prevAnim) {
                            currentFrame = currentFrame % anim.num_frames;
                            UpdateAnimation();
                        }
                    } else {
                        currentFrame = currentFrame % anim.num_frames;
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

	void InitAnimation() {
		if (forceAnimUpdate) {
			forceAnimUpdate = false;
			DeinitAnimation();
			// Init animation
			if (anim != null) {
				//animationSpeed = a3d.speed;
				// Init channels & subobjects
				subObjects = new GameObject[anim.num_channels][];
				channelObjects = new GameObject[anim.num_channels];
				//if (anim.a3d.num_morphData > 0) fullMorphPOs = new Dictionary<ushort, GameObject>[anim.a3d.num_channels];
				currentActivePO = new int[anim.num_channels];
				channelParents = new bool[anim.num_channels];
				channelNTTO = new ushort[anim.num_channels][];
				for (int i = 0; i < anim.num_channels; i++) {
					PS1AnimationChannel ch = anim.channels[i];
					short id = ch.id;
					channelObjects[i] = new GameObject("Channel " + id);
					channelObjects[i].transform.SetParent(transform);
					channelObjects[i].transform.localPosition = Vector3.zero;

					currentActivePO[i] = -1;
					AddChannelID(id, i);
					channelNTTO[i] = ch.frames?.SelectMany(f => f.ntto != null ? new ushort[] { f.ntto.Value } : new ushort[0]).Distinct().ToArray();
					subObjects[i] = new GameObject[channelNTTO[i] != null ? channelNTTO[i].Length : 0];
					for (int k = 0; k < subObjects[i].Length; k++) {
						int j = (int)channelNTTO[i][k] - 1;
						if (j == 0xFFFF || j < 0 || j > h.geometricObjectsDynamic.length) {
							subObjects[i][k] = new GameObject();
							subObjects[i][k].transform.parent = channelObjects[i].transform;
							subObjects[i][k].name = "[" + j + "] Invisible NTTO";
							subObjects[i][k].SetActive(false);
						} else {
							GameObject c = h.geometricObjectsDynamic.GetGameObject(j);
							subObjects[i][k] = c;
							/*if (entry.scale.HasValue) {
								objectIndexScales[ntto.object_index] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
								subObjects[i][j].transform.localScale = objectIndexScales[ntto.object_index];
							}*/
							c.transform.parent = channelObjects[i].transform;
							c.name = "[" + j + "] " + c.name;
							//c.SetActive(false);
						}
						subObjects[i][k].transform.localPosition = Vector3.zero;
					}
				}


				// Keep lighting last so that it is applied to all new sub objects
				/*if (!isAlways) {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, OpenSpace.Visual.LightInfo.ObjectLightedFlag.None);
				}*/
			}
			IsLoaded = true;
		}
	}

	void LoadNewAnimation(int animIndex) {
		if (animIndex != this.animIndex || forceAnimUpdate) {
			forceAnimUpdate = false;
			this.animIndex = animIndex;
			DeinitAnimation();
			currentFrame = 0;
			anim = null;
			if (animIndex >= 0 && animIndex < perso.p3dData.family.animations.Length) {
				anim = perso.p3dData.family.animations[animIndex];
				//animationSpeed = shAnim.speed;
				//print(animCuts.Length);
				forceAnimUpdate = true;
				InitAnimation();
			}
		}
	}

	public void UpdateAnimation() {
	}
	/*public void UpdateAnimation() {
		if (IsLoaded && anim != null && channelObjects != null && subObjects != null) {
			if (currentFrame >= anim.num_frames) currentFrame %= anim.num_frames;
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
			}
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
    }*/

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
        if (state != null && state.state_auto != null) {
			State state_auto = state.state_auto;
            if (state_auto != null) {
				//int indexOfStateAuto = Array.IndexOf(stateNames, state_auto.ToString());
				int indexOfStateAuto = Array.IndexOf(states, state_auto);
				if (indexOfStateAuto > -1) SetState(indexOfStateAuto);
            }
        }
    }
}