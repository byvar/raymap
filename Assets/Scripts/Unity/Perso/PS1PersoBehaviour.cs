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

public class PS1PersoBehaviour : BasePersoBehaviour {
    public Perso perso;
	public SuperObject superObject;

    // States
	public State[] states;
    public State state { get; private set; } = null;

	// Globals
	LevelHeader h;
	ActorFileHeader a1h;
	ActorFileHeader a2h;

	// Animation
	PS1Animation anim;
	int animIndex = -1;
    public GameObject[][] subObjects { get; private set; } = null; // [channel][ntto]
	public GameObject[][][] subObjectBones { get; private set; } = null;
	public GameObject[][] subObjectMorph { get; private set; } = null;
	private short[][] channelNTTO;
	private Dictionary<CollideType, GameObject[]> collSetObjects = null;
	private Dictionary<byte, Vector3> objectIndexScales = new Dictionary<byte, Vector3>();

	// Abstract properties
	public override LegacyPointer Offset => perso?.Offset;
	public override string NameFamily => perso?.p3dData?.family?.name;
	public override string NameModel => null;
	public override string NameInstance => perso?.name;


	// Use this for initialization
	void Start() {
    }

    public void Init() {
		R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
		h = l.levelHeader;
		a1h = l.actor1Header;
		a2h = l.actor2Header;
		if (perso != null && perso.p3dData != null && perso.p3dData.family != null) {
			Family fam = perso.p3dData.family;
			if (fam.animations != null) {
				PointerList<State> statePtrs = l.GetStates(perso.p3dData);
				State[] tempStates = statePtrs.pointers.Select(s => s.Value).ToArray();
				int ind = (int)perso.p3dData.stateIndex;
				if (ind >= tempStates.Length) {
					ind = 0;
				}
				int stateInd = ind;

				states = fam.states;
				stateNames = new string[states.Length + 1 + fam.animations.Length];
				stateNames[0] = "Null";
				for (int i = 0; i < states.Length; i++) {
					State s = states[i];
					stateNames[i + 1] = (s.anim == null || s.anim.index >= fam.animations.Length) ? "Null" : $"State {Array.IndexOf(tempStates, s)}: {fam.animations[s.anim.index].name}";
				}
				for (int i = 0; i < fam.animations.Length; i++) {
					stateNames[i + 1 + states.Length] = $"(Animation {i}) {fam.animations[i].name}";
				}
				hasStates = true;
				stateIndex = stateInd - (int)fam.startState;
				stateIndex++; // After null
				currentState = stateIndex;
				SetState(stateIndex);
			}
		}
		IsLoaded = true;
    }

	public string GetStateName(State state) {
		int ind = GetStateIndex(state);
		if (ind >= 1) {
			return stateNames[ind];
		} else {
			return null;
		}
	}

	public int GetStateIndex(State state) {
		int ind = Array.IndexOf(states, state);
		return ind + 1;
	}

	#region Print debug info
	public void PrintAnimationDebugInfo() {
		if (IsLoaded && hasStates) {
			if (anim != null) print("Animation offset: " + anim.Offset);
		}
    }
	#endregion

	public void UpdateViewCollision(bool viewCollision) {
		if (perso.collSet != null) {
			CollSet c = perso.collSet;
			if (collSetObjects == null) {
				collSetObjects = new Dictionary<CollideType, GameObject[]>();
				foreach (KeyValuePair<CollideType, ZdxList> zdxListKV in c.zdxList) {
					CollideType type = zdxListKV.Key;
					ZdxList zdxList = zdxListKV.Value;
					if (zdxList == null) continue;
					collSetObjects[type] = new GameObject[zdxList.entries?.Length ?? 0];
					for (int i = 0; i < zdxList.entries?.Length; i++) {
						ZdxEntry zdx = zdxList.entries[i];
						collSetObjects[type][i] = zdx.GetGameObject(type);
						collSetObjects[type][i].transform.SetParent(transform);
						collSetObjects[type][i].transform.localPosition = Vector3.zero;
						collSetObjects[type][i].transform.localRotation = Quaternion.identity;
						collSetObjects[type][i].transform.localScale = Vector3.one;
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
				if (state != null && c.activationList != null) {
					foreach (KeyValuePair<CollideType, short> activationZone in state.zoneZdx) {
						CollideType type = activationZone.Key;
						short cur_activation = activationZone.Value;
						if (cur_activation == -1) continue;
						if (!collSetObjects.ContainsKey(type) || collSetObjects[type] == null) continue;
						if (cur_activation < 0 || cur_activation >= c.activationList.num_activationZones) continue;
						if (c.activationList.activationZones[cur_activation].num_activations == 0) continue;
						for (int i = 0; i < c.activationList.activationZones[cur_activation].activations.Length; i++) {
							uint ind_zdx = c.activationList.activationZones[cur_activation].activations[i];
							if (collSetObjects[type].Length <= ind_zdx) continue;
							GameObject gao = collSetObjects[type][ind_zdx];
							if (gao == null) continue;
							gao.SetActive(true);
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
		if (state.anim != null) {
			//animationSpeed = state.speed;
			LoadNewAnimation((int)state.anim.index);
			UpdateAnimation();
		} else {
			LoadNewAnimation(-1);
		}
    }

	public void SetState(int index) {
		Family fam = perso.p3dData.family;
		//stateNames = states.Select(s => (s.Value == null ? "Null" : "State " + s.Value.Index)).ToArray();
		if (index < 0 || index >= stateNames.Length) return;
		stateIndex = index;
		currentState = index;
		if (index < 1) {
			state = null;
			UpdateViewCollision(controller.viewCollision);
			LoadNewAnimation(-1);
		} else if (index < 1 + states.Length) {
			state = states[index - 1];
			SetState(state);
		} else {
			state = null;
			UpdateViewCollision(controller.viewCollision);
			LoadNewAnimation(index - 1 - states.Length);
		}
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
		if (!IsLoaded || !(controller.LoadState == Controller.State.Finished || controller.LoadState == Controller.State.Error)) return;
		bool sectorActive = false, insideSectors = false;
		if (sector == null || IsAlways || AlwaysPlayAnimation || sector.Loaded) sectorActive = true;
		if (sector == null || IsAlways || AlwaysPlayAnimation || controller.sectorManager.activeSector != null) insideSectors = true;
        if (controller.playAnimations && playAnimation && sectorActive) {
            updateCounter += Time.deltaTime * animationSpeed;
            // If the camera is not inside a sector, animations will only update 1 out of 2 times (w/ frameskip) to avoid lag
            if ((!insideSectors && updateCounter >= 2f) || (insideSectors && updateCounter >= 1f)) {
                uint passedFrames = (uint)Mathf.FloorToInt(updateCounter);
                updateCounter %= 1;
                currentFrame += passedFrames;
                if (anim != null && currentFrame >= anim.num_frames && anim.num_frames != 0) {
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
		if (subObjectBones != null) {
			for (int i = 0; i < subObjectBones.Length; i++) {
				if (subObjectBones[i] == null) continue;
				for (int j = 0; j < subObjectBones[i].Length; j++) {
					if (subObjectBones[i][j] == null) continue;
					for (int k = 0; k < subObjectBones[i][j].Length; k++) {
						if (subObjectBones[i][j][k] != null) {
							GameObject.Destroy(subObjectBones[i][j][k]);
						}
					}
				}
			}
			subObjectBones = null;
		}
		if (subObjectMorph != null) {
			for (int i = 0; i < subObjectMorph.Length; i++) {
				if (subObjectMorph[i] == null) continue;
				for (int j = 0; j < subObjectMorph[i].Length; j++) {
					if (subObjectMorph[i][j] == null) continue;
					GameObject.Destroy(subObjectMorph[i][j]);
				}
			}
			subObjectMorph = null;
		}
		if (channelObjects != null) {
            for (int i = 0; i < channelObjects.Length; i++) {
				if (channelObjects[i] != null) {
					Destroy(channelObjects[i]);
				}
            }
            channelObjects = null;
        }
        channelIDDictionary.Clear();
		objectIndexScales.Clear();
		channelNTTO = null;
		hasBones = false;
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
				subObjectBones = new GameObject[anim.num_channels][][];
				subObjectMorph = new GameObject[anim.num_channels][];
				//if (anim.a3d.num_morphData > 0) fullMorphPOs = new Dictionary<ushort, GameObject>[anim.a3d.num_channels];
				currentActivePO = new int[anim.num_channels];
				channelParents = new bool[anim.num_channels];
				channelNTTO = new short[anim.num_channels][];
				for (int i = 0; i < anim.num_channels; i++) {
					PS1AnimationChannel ch = anim.channels[i];
					short id = ch.id;
					channelObjects[i] = new GameObject("Channel " + id + " - " + ch.Offset);
					channelObjects[i].transform.SetParent(transform);
					channelObjects[i].transform.localPosition = Vector3.zero;

					currentActivePO[i] = -1;
					AddChannelID(id, i);
					channelNTTO[i] = ch.frames?.SelectMany(f => (f.ntto != null && f.ntto >= 1) ? new short[] { f.ntto.Value } : new short[0]).Distinct().ToArray();
					subObjects[i] = new GameObject[channelNTTO[i] != null ? channelNTTO[i].Length : 0];
					subObjectBones[i] = new GameObject[subObjects[i].Length][];
					subObjectMorph[i] = new GameObject[anim.num_frames];
					ObjectsTable geometricObjectsDynamic = h.geometricObjectsDynamic;
					switch (anim.file_index) {
						case 1: geometricObjectsDynamic = a1h?.geometricObjectsDynamic; break;
						case 2: geometricObjectsDynamic = a2h?.geometricObjectsDynamic; break; 
					}
					for (int k = 0; k < subObjects[i].Length; k++) {
						int j = (int)channelNTTO[i][k] - 1;
						if (j == 0xFFFF || j < 0 || j > geometricObjectsDynamic.length.Value) {
							subObjects[i][k] = new GameObject();
							subObjects[i][k].transform.parent = channelObjects[i].transform;
							subObjects[i][k].name = "[" + j + "] Invisible NTTO";
							subObjects[i][k].SetActive(false);
						} else {
							GameObject[] bones = null;
							GameObject c = geometricObjectsDynamic.GetGameObject(j, perso.p3dData?.collisionMapping, out bones);
							if (c != null) {
								//GeometricObject geo = geometricObjectsDynamic.entries[j].geo;
								if (CPA_Settings.s.hasDeformations && bones != null) {
									subObjectBones[i][k] = bones;
									hasBones = true;
								}
							}
							subObjects[i][k] = c;
							/*if (entry.scale.HasValue) {
								objectIndexScales[ntto.object_index] = new Vector3(entry.scale.Value.x, entry.scale.Value.z, entry.scale.Value.y);
								subObjects[i][j].transform.localScale = objectIndexScales[ntto.object_index];
							}*/
							c.transform.parent = channelObjects[i].transform;
							c.name = "[" + j + "] " + c.name;
							c.SetActive(false);
						}
						subObjects[i][k].transform.localPosition = Vector3.zero;
						subObjects[i][k].transform.localRotation = Quaternion.identity;
						subObjects[i][k].transform.localScale = Vector3.one;
					}

					// Calculate morph objects
					int nextChannelFrame = 0;
					int curChannelFrame = 0;
					for (int f = 0; f < ch.frames.Length; f++) {
						if (!ch.frames[f].ntto.HasValue || ch.frames[f].ntto >= 0 || ch.frames[f].ntto == -20) {
							PS1AnimationKeyframe frame = ch.frames[f];
							curChannelFrame = nextChannelFrame;
							nextChannelFrame++;
							if (frame.extraDuration.HasValue && frame.extraDuration.Value > 0) {
								nextChannelFrame += frame.extraDuration.Value;
							}
							int curFrameIndex = f;
							PS1AnimationKeyframe nextKF = null;
							if (frame.extraDuration.HasValue && frame.extraDuration.Value > 0 && curFrameIndex + 1 < ch.frames.Length) {
								nextKF = ch.frames[curFrameIndex + 1];
								//print(ch.frames[0].Offset);
								if (nextKF.ntto.HasValue && (nextKF.ntto.Value < 0 && nextKF.ntto.Value != -20)) nextKF = ch.frames[curFrameIndex + 2];
							}
							if (ch.frames[f].HasFlag(PS1AnimationKeyframe.AnimationFlags.Morph) && ch.frames[f].HasFlag(PS1AnimationKeyframe.AnimationFlags.NTTO)) {
								int j = ch.frames[f].ntto.Value - 1;
								int morphJ = ch.frames[f].morphNTTO.Value - 1;
								if (j < 0 || j > geometricObjectsDynamic.length.Value || morphJ < 0 || morphJ > geometricObjectsDynamic.length.Value) {
								} else {
									int duration = nextChannelFrame - curChannelFrame;
									if (duration < 2 || nextKF == null || nextKF.ntto != frame.ntto || nextKF.morphNTTO != frame.morphNTTO || nextKF.morphProgress == frame.morphProgress) {
										if (frame.morphProgress.Value == 0) continue;
										float morphProgress = frame.morphProgress.Value / 100f;
										GameObject c = geometricObjectsDynamic.GetGameObject(j, perso.p3dData?.collisionMapping, out _, morphI: morphJ, morphProgress: morphProgress);
										for (int d = 0; d < duration; d++) {
											subObjectMorph[i][curChannelFrame + d] = c;
										}
										c.transform.parent = channelObjects[i].transform;
										c.name = "[" + j + "] " + c.name + " - " + morphJ;
										c.SetActive(false);
										c.transform.localPosition = Vector3.zero;
										c.transform.localRotation = Quaternion.identity;
										c.transform.localScale = Vector3.one;
									} else {
										for (int d = 0; d < duration; d++) {
											float interpolation = d / (float)duration;
											float morphProgress = Mathf.Lerp(frame.morphProgress.Value / 100f, nextKF.morphProgress.Value / 100f, interpolation);
											GameObject c = geometricObjectsDynamic.GetGameObject(j, perso.p3dData?.collisionMapping, out _, morphI: morphJ, morphProgress: morphProgress);
											subObjectMorph[i][curChannelFrame + d] = c;
											c.transform.parent = channelObjects[i].transform;
											c.SetActive(false);
											c.transform.localPosition = Vector3.zero;
											c.transform.localRotation = Quaternion.identity;
											c.transform.localScale = Vector3.one;
										}
									}
								}
							}
						}
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
				animationSpeed = anim.speed;
				//print(animCuts.Length);
				forceAnimUpdate = true;
				InitAnimation();
			}
		}
	}

	private int GetNextGoodFrameIndex(int channel, int curFrame) {
		PS1AnimationChannel ch = anim.channels[channel];
		PS1AnimationKeyframe frame = ch.frames[curFrame];
		int skipFrames = 0;
		while (frame.ntto < 0) {
			skipFrames++;
			if (skipFrames >= ch.frames.Length) break;
			frame = ch.frames[(curFrame + skipFrames) % ch.frames.Length];
		}
		if (skipFrames == ch.frames.Length) return -1;
		return skipFrames;
	}

	public void UpdateAnimation() {
		if (IsLoaded && anim != null && channelObjects != null && subObjects != null && anim.num_frames != 0) {
			if (currentFrame >= anim.num_frames) {
				currentFrame %= anim.num_frames;
			}
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
			}
			// Create hierarchy for this frame
			for (int i = 0; i < anim.hierarchies.Length; i++) {
				PS1AnimationHierarchy h = anim.hierarchies[i];

				if (h.child > anim.channels.Length || h.parent > anim.channels.Length
					|| h.child < 1 || h.parent < 1) {
					continue;
				}
				int ch_child = h.child - 1;
				int ch_parent = h.parent - 1;
				channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
				channelObjects[ch_child].transform.localPosition = Vector3.zero;
				channelObjects[ch_child].transform.localRotation = Quaternion.identity;
				channelObjects[ch_child].transform.localScale = Vector3.one;
				channelParents[ch_child] = true;
			}
			PS1AnimationVector[] animPositions = h.animPositions;
			PS1AnimationQuaternion[] animRotations = h.animRotations;
			PS1AnimationVector[] animScales = h.animScales;
			switch (anim.file_index) {
				case 1:
					animPositions = a1h?.animPositions;
					animRotations = a1h?.animRotations;
					animScales = a1h?.animScales;
					break;
				case 2:
					animPositions = a2h?.animPositions;
					animRotations = a2h?.animRotations;
					animScales = a2h?.animScales;
					break;
			}
			int[] channelNttoChanges = new int[anim.channels.Length];
			for (int i = 0; i < anim.channels.Length; i++) {
				PS1AnimationChannel ch = anim.channels[i];
				if (ch.frames.Length < 1) continue;
				PS1AnimationKeyframe frame = ch.frames[0];
				PS1AnimationKeyframe nextKF = null;

				int? posInd = null;
				int? rotInd = null;
				int? sclInd = null;
				int curChannelFrame = 0;
				int nextChannelFrame = 0;
				int curFrameIndex = 0;

				channelNttoChanges[i] = -1;
				int? previousNtto = null;
				for (int f = 0; f < ch.frames.Length; f++) {
					if (!ch.frames[f].ntto.HasValue || ch.frames[f].ntto >= 0 || ch.frames[f].ntto == -20) {
						if (!previousNtto.HasValue || previousNtto.Value != ch.frames[f].ntto) {
							channelNttoChanges[i]++;
							previousNtto = ch.frames[f].ntto;
						}
						frame = ch.frames[f];
						curChannelFrame = nextChannelFrame;
						curFrameIndex = f;
						if (frame.pos.HasValue) posInd = frame.pos.Value;
						if (frame.rot.HasValue) rotInd = frame.rot.Value;
						if (frame.scl.HasValue) sclInd = frame.scl.Value;
						nextChannelFrame++;
						if (frame.extraDuration.HasValue && frame.extraDuration.Value > 0) {
							nextChannelFrame += frame.extraDuration.Value;
						}
						if (currentFrame < nextChannelFrame) {
							break;
						}
					}
				}
				if (frame.extraDuration.HasValue && frame.extraDuration.Value > 0 && curFrameIndex + 1 < ch.frames.Length) {
					nextKF = ch.frames[curFrameIndex + 1];
					//print(ch.frames[0].Offset);
					if (nextKF.ntto.HasValue && (nextKF.ntto.Value < 0 && nextKF.ntto.Value != -20)) nextKF = ch.frames[curFrameIndex + 2];
				}
				float interpolation = 0f;

				if (nextKF != null) {
					interpolation = (currentFrame - curChannelFrame) / (float)(nextChannelFrame - curChannelFrame);
				}

				if (currentActivePO[i] == -2) {
					for (int j = 0; j < subObjectMorph[i].Length; j++) {
						GameObject morphGao = subObjectMorph[i][j];
						if (morphGao != null && morphGao.activeSelf) morphGao.SetActive(false);
					}
				}

				if (!frame.ntto.HasValue || frame.ntto.Value >= 0 || frame.ntto.Value == -20) {
					GameObject physicalObject = null;
					short poNum;
					if (subObjectMorph?[i]?[currentFrame] != null) {
						physicalObject = subObjectMorph[i][currentFrame];
						poNum = -2;
						physicalObject.SetActive(true);
					} else {
						poNum = (frame.ntto.HasValue && frame.ntto.Value != -20) ? frame.ntto.Value : (short)-1;
						poNum = (short)Array.IndexOf(channelNTTO[i], poNum);

						physicalObject = poNum >= 0 ? subObjects[i][poNum] : null;
					}
					if (poNum != currentActivePO[i]) {
						if (currentActivePO[i] >= 0 && subObjects[i][currentActivePO[i]] != null) {
							subObjects[i][currentActivePO[i]].SetActive(false);
						}
						currentActivePO[i] = poNum;
						if (physicalObject != null) physicalObject.SetActive(true);
					}
					if (physicalObject != null) {
						if (frame.HasFlag(PS1AnimationKeyframe.AnimationFlags.FlipX)) {
							physicalObject.transform.localScale = new Vector3(-1, 1, 1);
						} else {
							physicalObject.transform.localScale = Vector3.one;
						}
					}
					if (!channelParents[i]) channelObjects[i].transform.SetParent(transform);


					if (posInd.HasValue) {
						Vector3 pos = animPositions[posInd.Value].vector;
						if (interpolation > 0f && nextKF.pos.HasValue) {
							pos = Vector3.Lerp(pos, animPositions[nextKF.pos.Value].vector, interpolation);
						}
						channelObjects[i].transform.localPosition = pos;
					}
					if (rotInd.HasValue) {
						Quaternion rot = animRotations[rotInd.Value].quaternion;
						if (interpolation > 0f && nextKF.rot.HasValue) {
							rot = Quaternion.Lerp(rot, animRotations[nextKF.rot.Value].quaternion, interpolation);
						}
						channelObjects[i].transform.localRotation = rot;
					}
					if (sclInd.HasValue) {
						Vector3 scl = animScales[sclInd.Value].GetVector(factor: 256f * 16f);
						if (interpolation > 0f && nextKF.scl.HasValue) {
							scl = Vector3.Lerp(scl, animScales[nextKF.scl.Value].GetVector(factor: 256f * 16f), interpolation);
						}

							
						/*scl = new Vector3(
							1f / (scl.x != 0f ? scl.x : 256f),
							1f / (scl.y != 0f ? scl.y : 256f),
							1f / (scl.z != 0f ? scl.z : 256f));*/
						channelObjects[i].transform.localScale = scl;
					}
				} else {
					switch (frame.ntto.Value) {
						case -13:
							// morph
							break;
						case -9:
							// 6
							break;
						case -8:
							break;
						case -7:
							// 5
							break;
						case -6:
							// 4
							break;
						case -5:
							// 3
							break;
						case -4:
							// 2
							break;
						case -3:
							// sound event related. extraDuration is sound bank
							break;
						case -2:
							// 1
							break;
						case -1:
							break;
					}
				}
				/*if (frame.pos.HasValue) {
					channelObjects[i].transform.localPosition = h.animPositions[frame.pos.Value].vector;
				}
				if (frame.rot.HasValue) {
					channelObjects[i].transform.localRotation = h.quaternions[frame.rot.Value].quaternion;
				}
				if (frame.scl.HasValue) {
					Vector3 v = h.animScales[frame.scl.Value].vector;
					v = new Vector3(
						(1 / 256f) * (v.x != 0f ? v.x : 256f),
						(1 / 256f) * (v.y != 0f ? v.y : 256f),
						(1 / 256f) * (v.z != 0f ? v.z : 256f));
					channelObjects[i].transform.localScale = v;
				}*/
			}

			if (hasBones && anim.bones?.Length > 0) {
				for (int i = 0; i < anim.channels.Length; i++) {
					if (currentActivePO[i] >= 0 && subObjectBones[i][currentActivePO[i]] != null) {
						GameObject[] bones = subObjectBones[i][currentActivePO[i]];
						int bonesIndex = Array.FindIndex(anim.bones, b => b.ind_ntto_channel == i);
						if (bonesIndex != -1) {
							int curFrameIndex = bonesIndex + (channelNttoChanges[i] >= 0 ? channelNttoChanges[i] : 0);
							PS1AnimationBoneChannelLinks b = anim.bones[curFrameIndex];
							if (b.ind_ntto_channel != i) print("Channel did not match!");
							ushort[] indices = b.indices;
							for (int j = 1; j < bones.Length; j++) {
								bones[j].transform.SetParent(channelObjects[indices[j-1]].transform);
								bones[j].transform.localPosition = Vector3.zero;
								bones[j].transform.localRotation = Quaternion.identity;
								bones[j].transform.localScale = Vector3.one;
							}
						}
					}
				}
			}
		}
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
				if (indexOfStateAuto > -1) SetState(1 + indexOfStateAuto);
            }
        }
    }


	public override StateTransition[] GetStateTransitions(int index) {
		if (index < 1 || index >= 1 + states.Length) return null;
		State state = states[index - 1];
		if (state != null && state.transitions != null && state.transitions.Length > 0) {
			R2PS1Loader l = MapLoader.Loader as R2PS1Loader;
			PointerList<State> statePtrs = l.GetStates(perso.p3dData);
			State[] states = statePtrs.pointers.Select(s => s.Value).ToArray();
			List<StateTransition> tr = new List<StateTransition>();
			foreach (OpenSpace.PS1.StateTransition t in state.transitions) {
				State stateToGo = t.stateToGo.Value;
				State targetState = t.targetState.Value;
				if (stateToGo == null || targetState == null) continue;
				string targetStateName = GetStateName(targetState);
				string stateToGoName = GetStateName(stateToGo);
				if (targetStateName != null && stateToGoName != null) {
					tr.Add(new StateTransition() {
						StateToGoName = stateToGoName,
						StateToGoIndex = GetStateIndex(stateToGo),
						TargetStateName = targetStateName,
						TargetStateIndex = GetStateIndex(targetState),
						LinkingType = 0
					});
				}
			}
			return tr.ToArray();
		}
		return null;
	}
}