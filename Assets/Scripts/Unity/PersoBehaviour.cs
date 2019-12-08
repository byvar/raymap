using Assets.Scripts;
using Assets.Scripts.Unity;
using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.Animation.Component;
using OpenSpace.Animation.ComponentLargo;
using OpenSpace.Animation.ComponentMontreal;
using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Visual;
using OpenSpace.Visual.Deform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersoBehaviour : MonoBehaviour {
    public bool isLoaded { get; private set; } = false;
    public Perso perso;
    public SectorComponent sector;
    public Controller controller;

    // States
    bool hasStates = false;
    public State state { get; private set; } = null;
    public string[] stateNames = { "Placeholder" };
    public int currentState { get; private set; } = 0; // Follows "stateIndex", sometimes with a small delay.
    public int stateIndex = 0; // Set this variable
    public bool autoNextState = false;

    // Physical object lists
    public string[] poListNames = { "Null" };
    int currentPOList = 0;
    public int poListIndex = 0;

    // Animation
    public AnimA3DGeneral a3d = null;
    public AnimationMontreal animMontreal = null;
	public AnimA3DLargo animLargo = null;
    bool forceAnimUpdate = false;
    public uint currentFrame = 0;
    public bool playAnimation = true;
    public bool playAnimationFramesAutomatically = true;
    public float animationSpeed = 15f;
    private float updateCounter = 0f;
    public PhysicalObject[][] subObjects { get; private set; } = null; // [channel][ntto]
    public GameObject[] channelObjects { get; private set; }
	private int[] currentActivePO = null;
	private bool[] channelParents = null;
    public AnimMorphData[,] morphDataArray;
    private Dictionary<short, List<int>> channelIDDictionary = new Dictionary<short, List<int>>();
	private Dictionary<ushort, PhysicalObject>[] fullMorphPOs = null;
	bool hasBones = false; // We can optimize a tiny bit if this object doesn't have bones
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
			controller.UpdatePersoActive(perso);
		}
	}

    // Brain clearance
    public bool clearTheBrain = false;
    private PersoAnimationsDataExporter persoAnimationsDataExporter;


    // Use this for initialization
    void Start() {
    }

    public void Init() {
        MapLoader l = MapLoader.Loader;
        if (perso != null && perso.p3dData != null) {
            Family fam = perso.p3dData.family;
            if (fam != null && fam.objectLists != null && fam.objectLists.Count > 0) {
                Array.Resize(ref poListNames, fam.objectLists.Count + 1 + l.uncategorizedObjectLists.Count);
                Array.Copy(fam.objectLists.Select(o => (o == null ? "Null" : o.ToString())).ToArray(), 0, poListNames, 1, fam.objectLists.Count);
                Array.Copy(l.uncategorizedObjectLists.Select(o => (o == null ? "Null" : o.ToString())).ToArray(), 0, poListNames, 1 + fam.objectLists.Count, l.uncategorizedObjectLists.Count);
                currentPOList = perso.p3dData.off_objectList == null ? 0 : fam.GetIndexOfPhysicalList(perso.p3dData.off_objectList) + 1;
                if (currentPOList == -1) currentPOList = 0;
                poListIndex = currentPOList;
            }
            if (fam != null && fam.states != null && fam.states.Count > 0) {
                stateNames = fam.states.Select(s => (s == null ? "Null" : s.ToString())).ToArray();
                hasStates = true;
                state = perso.p3dData.stateCurrent;
                for (int i = 0; i < fam.states.Count; i++) {
                    if (state == fam.states[i]) {
                        currentState = i;
                        stateIndex = i;
                        SetState(i);
                        break;
                    }
                }
                if (state == null && fam.states.Count > 0) {
                    currentState = 0;
                    stateIndex = 0;
                    SetState(0);
                }
            }
            persoAnimationsDataExporter = new Assets.Scripts.Unity.PersoAnimationsDataExporter(this);
        }
        isLoaded = true;
    }

    #region Print debug info
    public void PrintDsgVar() {
        if (isLoaded && hasStates) {
            if (perso.brain != null && perso.brain.mind != null) {

                DsgVar dsgVar = perso.brain.mind.AI_model.dsgVar;
                if (dsgVar != null) {
                    MapLoader l = MapLoader.Loader;
                    l.print("DsgVar.offset: " + dsgVar.Offset);
                    l.print("DsgVarFromModel.amountOfInfos: " + dsgVar.amountOfInfos);
                    l.print("DsgVarFromModel.dsgMemBufferLength: " + dsgVar.dsgMemBufferLength);

					for(int c = 0; c < dsgVar.dsgVarInfos.Length; c++) {
						DsgVarInfoEntry entry = dsgVar.dsgVarInfos[c];
						l.print("Info Entry " + c + ", type " + entry.type + ", offset " + entry.offsetInBuffer + " , value " + dsgVar.defaultValues[c].ToString() + ", initType " + entry.initType + ", saveType " + entry.saveType);
                    }
                }

            }
        }
    }

    public void PrintDsgVarFromMindMem() {
        MapLoader l = MapLoader.Loader;

        if (isLoaded && hasStates) {
            if (perso.brain != null && perso.brain.mind != null) {

                DsgMem dsgMem = perso.brain.mind.dsgMem;
                if (dsgMem != null) {
                    l.print("DsgMem.offset: " + dsgMem.Offset);
                    DsgVar dsgVar = perso.brain.mind.dsgMem.dsgVar;
                    if (dsgVar != null) {
                        l.print("DsgVar.offset: " + dsgVar.Offset);
                        l.print("DsgVarFromMem.amountOfInfos: " + dsgVar.amountOfInfos);
                        l.print("DsgVarFromMem.dsgMemBufferLength: " + dsgVar.dsgMemBufferLength);


						for (int c = 0; c < dsgVar.dsgVarInfos.Length; c++) {
							DsgVarInfoEntry entry = dsgVar.dsgVarInfos[c];
							l.print("Info Entry " + c + ", type " + entry.type + ", offset " + entry.offsetInBuffer + " , value " + (dsgMem.values != null ? dsgMem.values[c] : dsgVar.defaultValues[c]).ToString() + ", initType " + entry.initType + ", saveType " + entry.saveType);
						}
                    }
                }



            }
        }
    }

    public void PrintScripts() {
        if (isLoaded && hasStates) {
            if (perso.brain != null
                && perso.brain.mind != null
                && perso.brain.mind.AI_model != null) {
                AIModel ai = perso.brain.mind.AI_model;
               
                if (ai.behaviors_normal != null) {
                    MapLoader.Loader.print("Normal behaviours");
                    for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                        if (ai.behaviors_normal[i].scripts != null) {
                            for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                                perso.brain.mind.AI_model.behaviors_normal[i].scripts[j].print(perso);
                            }
                        }
                    }
                }
                if (ai.behaviors_reflex != null) {
                    MapLoader.Loader.print("Reflex behaviours");
                    for (int i = 0; i < ai.behaviors_reflex.Length; i++) {
                        if (ai.behaviors_reflex[i].scripts != null) {
                            for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++) {
                                perso.brain.mind.AI_model.behaviors_reflex[i].scripts[j].print(perso);
                            }
                        }
                    }
                }
                if (ai.macros != null) {
                    MapLoader.Loader.print("Macros");
                    for (int i = 0; i < ai.macros.Length; i++) {
                        if (ai.macros[i].script != null) {
                            perso.brain.mind.AI_model.macros[i].script.print(perso);
                        }
                    }
                }
            }
        }
    }

    public void PrintTranslatedScripts()
    {
        if (isLoaded && hasStates)
        {
            if (perso.brain != null
                && perso.brain.mind != null
                && perso.brain.mind.AI_model != null)
            {
                AIModel ai = perso.brain.mind.AI_model;

                List<string> ruleStatesInitializer = new List<string>();
                List<string> reflexStatesInitializer = new List<string>();

                if (ai.behaviors_normal != null)
                {
                    MapLoader.Loader.print("Normal behaviours");
                    for (int i = 0; i < ai.behaviors_normal.Length; i++)
                    {
                        if (ai.behaviors_normal[i].scripts != null)
                        {
                            string combinedScript = "private IEnumerator Rule_"+i+"_"+ai.behaviors_normal[i].name+"() {"+Environment.NewLine;
                            ruleStatesInitializer.Add("Rule_" + i + "_" + ai.behaviors_normal[i].name);
                            for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++)
                            {
                                TranslatedScript ts = new TranslatedScript(perso.brain.mind.AI_model.behaviors_normal[i].scripts[j], perso);
                                combinedScript += "// Script "+j+Environment.NewLine+ts.ToString()+Environment.NewLine;
                            }
                            combinedScript += "yield return null;"+Environment.NewLine+"}";
                            MapLoader.Loader.print(combinedScript);
                        }
                    }
                }
                if (ai.behaviors_reflex != null)
                {
                    MapLoader.Loader.print("Reflex behaviours");
                    for (int i = 0; i < ai.behaviors_reflex.Length; i++)
                    {
                        if (ai.behaviors_reflex[i].scripts != null)
                        {
                            string combinedScript = "private IEnumerator Reflex_" + i + "_" + ai.behaviors_reflex[i].name + "() {" + Environment.NewLine;
                            for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++)
                            {
                                reflexStatesInitializer.Add("Reflex_" + i + "_" + ai.behaviors_reflex[i].name);
                                TranslatedScript ts = new TranslatedScript(perso.brain.mind.AI_model.behaviors_reflex[i].scripts[j], perso);
                                combinedScript += "// Script " + j + Environment.NewLine + ts.ToString() + Environment.NewLine;
                            }
                            combinedScript += "yield return null;" + Environment.NewLine + "}";
                            MapLoader.Loader.print(combinedScript);
                        }
                    }
                }
                if (ai.macros != null)
                {
                    MapLoader.Loader.print("Macros");
                    for (int i = 0; i < ai.macros.Length; i++)
                    {
                        if (ai.macros[i].script != null)
                        {
                            string combinedScript = "private IEnumerator Macro_" + i + "() {" + Environment.NewLine;
                            TranslatedScript ts = new TranslatedScript(perso.brain.mind.AI_model.macros[i].script, perso);
                            combinedScript += ts.ToString() + Environment.NewLine;
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
                startString += Environment.NewLine+"sm.ChangeActiveRuleState(0);" + Environment.NewLine + "sm.ChangeActiveReflexState(0);" + Environment.NewLine + "}";

                MapLoader.Loader.print(startString);
            }
        }
    }

    public void PrintAnimationDebugInfo() {
        if (isLoaded && hasStates) {
			if (a3d != null) {
				for (int i = 0; i < a3d.num_NTTO; i++) {
					AnimNTTO ntto = a3d.ntto[i + a3d.start_NTTO];
					print("NTTO " + i + ": FLAGS:" + ntto.flags + " - OBJIND:" + ntto.object_index +
						" - UNK4:" + ntto.unk4 + " - UNK5:" + ntto.unk5);
				}
				for (int i = 0; i < a3d.num_deformations; i++) {
					AnimDeformation d = a3d.deformations[i + a3d.start_deformations];
					print("Deform " + i + ": CH1:" + d.channel + " - BI1:" + d.bone +
						" - CH2:" + d.linkChannel + " - BI2:" + d.linkBone);
				}
				for (int i = 0; i < a3d.num_hierarchies; i++) {
					AnimHierarchy h = a3d.hierarchies[i + a3d.start_hierarchies];
					print("Hierarchy " + i + ": Child:" + h.childChannelID + " - Parent:" + h.parentChannelID);
				}
				for (int i = 0; i < a3d.num_channels; i++) {
					AnimChannel ch = a3d.channels[i + a3d.start_channels];
					AnimFramesKFIndex kfi = a3d.framesKFIndex[currentFrame + ch.framesKF];
					AnimKeyframe kf = a3d.keyframes[kfi.kf];
					print("Channel " + i + ": ID:" + ch.id + " - U0:" + ch.unk0 + " - V:" + ch.vector + " - ");
					print("Current keyframe: Flags:" + kf.flags + " - Q2:" + kf.quaternion2 + " - Frame:" + kf.frame);
				}

				//AnimOnlyFrame of = a3d.onlyFrames[a3d.start_onlyFrames + currentFrame];
				print("POs offset: " + perso.p3dData.family.objectLists.off_head);
			} else if (animMontreal != null) {
				print("POs offset: " + perso.p3dData.family.objectLists.off_head + " - Length: " + perso.p3dData.family.objectLists[0].Count);
				for (int i = 0; i < animMontreal.num_channels; i++) {
					for (int j = 0; j < animMontreal.num_frames; j++) {
						print("Frame " + j + " ch " + i + ": "
							+ animMontreal.frames[j].channels[i].objectIndex + " - "
							+ animMontreal.frames[j].channels[i].unk2 + " - "
							+ animMontreal.frames[j].channels[i].unk3 + " - "
							+ animMontreal.frames[j].channels[i].unkByte1 + " - "
							+ animMontreal.frames[j].channels[i].unkByte2 + " - "
							+ animMontreal.frames[j].channels[i].unkUint);
					}
				}
			} else if(animLargo != null) {
				print("Animation offset: " + animLargo.Offset);
				print("Num Deformations: " + animLargo.num_deformations);
            } else if (state != null) {
                MapLoader l = MapLoader.Loader;
                ushort anim_index = 0;
                byte bank_index = 0;
                if (state.anim_ref != null) {
                    anim_index = state.anim_ref.anim_index;
                    bank_index = perso.p3dData.family.animBank;
                }
                print(state.anim_ref != null);
                print(l.animationBanks != null);
                print(l.animationBanks.Length > bank_index);
                print(l.animationBanks[bank_index] != null);
                print(l.animationBanks[bank_index].animations != null);
                print(l.animationBanks[bank_index].animations.Length > anim_index);
                print(l.animationBanks[bank_index].animations[anim_index] != null);
            }
        }
    }

    public void NextFrameOfAnimation()
    {
        currentFrame += 1;
    }

    public void ExportAnimationsData()
    {
        persoAnimationsDataExporter.InitExportProcess();
    }
    #endregion

    public void UpdateViewCollision(bool viewCollision) {
		if (perso.collset != null) {
			CollSet c = perso.collset;
			foreach (KeyValuePair<CollideType, OpenSpace.LinkedList<GeometricObjectCollide>> entry in c.zdxList) {
				if (entry.Value != null) {
					for (int i = 0; i < entry.Value.Count; i++) {
						GeometricObjectCollide col = entry.Value[i];
						if (col == null) continue;
						if (viewCollision && c.GetPrivilegedActionZoneStatus(entry.Key, i) == CollSet.PrivilegedActivationStatus.ForceActive) {
							col.SetVisualsActive(true);
						} else {
							col.SetVisualsActive(false);
						}
					}
				}
			}
			if (viewCollision) {
				foreach (KeyValuePair<CollideType, OpenSpace.LinkedList<CollideActivation>> entry in c.activationList) {
					if (entry.Value != null && entry.Value.Count == perso.p3dData.family.states.Count) {
						CollideActivation ca = entry.Value[stateIndex];
						if (ca.activationZone != null) {
							foreach (CollideActivationZone caz in ca.activationZone) {
								int index = caz.zdxIndex;
								if (index >= c.zdxList[entry.Key].Count) index = c.zdxList[entry.Key].Count - 1;
								if (index < 0) continue;
								if (c.zdxList[entry.Key][index] == null) continue;
								if (c.GetPrivilegedActionZoneStatus(entry.Key, index) != CollSet.PrivilegedActivationStatus.ForceInactive) {
									c.zdxList[entry.Key][index].SetVisualsActive(true);
								}
							}
						}
					}
				}
			}
		}
		if (subObjects != null) {
			for (int i = 0; i < subObjects.Length; i++) {
				if (subObjects[i] == null) continue;
				for (int j = 0; j < subObjects[i].Length; j++) {
					if (subObjects[i][j] != null) subObjects[i][j].UpdateViewCollision(viewCollision);
				}
			}
		}
		if (fullMorphPOs != null) {
			for (int i = 0; i < fullMorphPOs.Length; i++) {
				if (fullMorphPOs[i] != null) {
					foreach (PhysicalObject po in fullMorphPOs[i].Values) {
						if(po != null) po.UpdateViewCollision(viewCollision);
					}
				}
			}
		}
	}

    public void SetState(State s)
    {
        this.state = s;
        stateIndex = s.index;
        currentState = s.index;
        UpdateViewCollision(controller.viewCollision);

        // Set animation
        MapLoader l = MapLoader.Loader;
        ushort anim_index = 0;
        byte bank_index = 0;
        if (state.anim_ref != null) {
            anim_index = state.anim_ref.anim_index;
            bank_index = perso.p3dData.family.animBank;
        }
		if (state.anim_refMontreal != null) {
			a3d = null;
			animLargo = null;
			animationSpeed = state.speed;
			//animationSpeed = state.speed;
			InitAnimationMontreal(state.anim_refMontreal);
			UpdateAnimation();
		} else if (state.anim_ref != null
			&& l.animationBanks != null
			&& l.animationBanks.Length > bank_index
			&& l.animationBanks[bank_index] != null
			&& l.animationBanks[bank_index].animations != null
			&& l.animationBanks[bank_index].animations.Length > anim_index
			&& l.animationBanks[bank_index].animations[anim_index] != null) {
			animMontreal = null;
			animLargo = null;
			animationSpeed = state.speed;
			//animationSpeed = state.speed;
			InitAnimation(l.animationBanks[bank_index].animations[anim_index]);
			UpdateAnimation();
		} else if (state.anim_ref != null && state.anim_ref.a3d != null) {
			animMontreal = null;
			animLargo = null;
			animationSpeed = state.speed;
			InitAnimation(state.anim_ref.a3d);
			UpdateAnimation();
		} else if (state.anim_ref != null && state.anim_ref.a3dLargo != null) {
			animMontreal = null;
			a3d = null;
			animationSpeed = state.speed;
			InitAnimationLargo(state.anim_ref.a3dLargo);
			UpdateAnimation();
		} else {
			a3d = null;
		}
    }

	public void SetState(int index) {
		if (index < 0 || index >= perso.p3dData.family.states.Count) return;
		stateIndex = index;
		currentState = index;
		state = perso.p3dData.family.states[index];
        SetState(state);
    }

    // Update is called once per frame
    void Update() {
        if (isLoaded) {
            if (hasStates) {
                if (stateIndex != currentState) {
                    currentState = stateIndex;
                    SetState(currentState);
                }
            }
            MapLoader l = MapLoader.Loader;
            if (poListIndex != currentPOList && perso.p3dData != null) {
                if (poListIndex > 0 && poListIndex < perso.p3dData.family.objectLists.Count + 1) {
                    currentPOList = poListIndex;
                    perso.p3dData.objectList = perso.p3dData.family.objectLists[currentPOList - 1];
                } else if (poListIndex >= perso.p3dData.family.objectLists.Count + 1 && poListIndex < perso.p3dData.family.objectLists.Count + 1 + l.uncategorizedObjectLists.Count) {
                    currentPOList = poListIndex;
                    perso.p3dData.objectList = l.uncategorizedObjectLists[currentPOList - perso.p3dData.family.objectLists.Count - 1];
                } else {
                    poListIndex = 0;
                    currentPOList = 0;
                    perso.p3dData.objectList = null;
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
                if (playAnimationFramesAutomatically)
                {
                    currentFrame += passedFrames;
                }                
                if (a3d != null && currentFrame >= a3d.num_onlyFrames) {
                    if (autoNextState) {
                        AnimA3DGeneral prevAnim = a3d;
                        GotoAutoNextState();
                        if (a3d == prevAnim) {
                            currentFrame = currentFrame % a3d.num_onlyFrames;
                            UpdateAnimation();
                        }
                    } else {
                        currentFrame = currentFrame % a3d.num_onlyFrames;
                        UpdateAnimation();
                    }
                } else if (animMontreal != null && currentFrame >= animMontreal.num_frames) {
                    if (autoNextState) {
                        AnimationMontreal prevAnim = animMontreal;
                        GotoAutoNextState();
                        if (animMontreal == prevAnim) {
                            currentFrame = currentFrame % animMontreal.num_frames;
                            UpdateAnimation();
                        }
                    } else {
                        currentFrame = currentFrame % animMontreal.num_frames;
                        UpdateAnimation();
                    }
                } else {
                    UpdateAnimation();
                }
            }
            if (persoAnimationsDataExporter.isAnimationsExportInProgress)
            {
                persoAnimationsDataExporter.UpdateExportProcess();
            }
        }
    }

    void DeinitAnimation() {
        isLoaded = false;
        // Destroy currently loaded subobjects
        if (subObjects != null) {
            for (int i = 0; i < subObjects.Length; i++) {
                if (subObjects[i] == null) continue;
                for (int j = 0; j < subObjects[i].Length; j++) {
					if (subObjects[i][j] != null) {
						subObjects[i][j].Destroy();
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
					foreach (PhysicalObject po in fullMorphPOs[i].Values) {
						po.Destroy();
					}
					fullMorphPOs[i].Clear();
				}
			}
			fullMorphPOs = null;
		}
        channelIDDictionary.Clear();
		hasBones = false;
    }

    void InitAnimation(AnimA3DGeneral a3d) {
        if (a3d != this.a3d || forceAnimUpdate) {
            forceAnimUpdate = false;
            DeinitAnimation();
            // Init animation
            this.a3d = a3d;
            currentFrame = 0;
            if (a3d != null) {
                //animationSpeed = a3d.speed;
                // Init channels & subobjects
                subObjects = new PhysicalObject[a3d.num_channels][];
                channelObjects = new GameObject[a3d.num_channels];
				if (a3d.num_morphData > 0) fullMorphPOs = new Dictionary<ushort, PhysicalObject>[a3d.num_channels];
				currentActivePO = new int[a3d.num_channels];
				channelParents = new bool[a3d.num_channels];
                for (int i = 0; i < a3d.num_channels; i++) {
                    short id = a3d.channels[a3d.start_channels + i].id;
                    channelObjects[i] = new GameObject("Channel " + id);
                    channelObjects[i].transform.SetParent(perso.Gao.transform);
					currentActivePO[i] = -1;
					AddChannelID(id, i);
                    subObjects[i] = new PhysicalObject[a3d.num_NTTO];
                    AnimChannel ch = a3d.channels[a3d.start_channels + i];
                    List<ushort> listOfNTTOforChannel = new List<ushort>();
                    for (int j = 0; j < a3d.num_onlyFrames; j++) {
                        AnimOnlyFrame of = a3d.onlyFrames[a3d.start_onlyFrames + j];
                        //print(ch.numOfNTTO + " - " + of.numOfNTTO + " - " + a3d.numOfNTTO.Length);
                        AnimNumOfNTTO numOfNTTO = a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
                        if (!listOfNTTOforChannel.Contains(numOfNTTO.numOfNTTO)) {
                            listOfNTTOforChannel.Add(numOfNTTO.numOfNTTO);
                        }
                    }
                    for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
                        int j = listOfNTTOforChannel[k] - a3d.start_NTTO;
                        AnimNTTO ntto = a3d.ntto[a3d.start_NTTO + j];
                        if (ntto.IsInvisibleNTTO) {
                            subObjects[i][j] = new PhysicalObject(null);
                            subObjects[i][j].Gao.transform.parent = channelObjects[i].transform;
                            subObjects[i][j].Gao.name = "[" + j + "] Invisible PO";
                            subObjects[i][j].Gao.SetActive(false);
                            /*GameObject boneVisualisation = new GameObject("Bone vis");
                            boneVisualisation.transform.SetParent(subObjects[i][j].Gao.transform);
                            MeshRenderer mr = boneVisualisation.AddComponent<MeshRenderer>();
                            MeshFilter mf = boneVisualisation.AddComponent<MeshFilter>();
                            Mesh mesh = Util.CreateBox(0.1f);
                            mf.mesh = mesh;
                            boneVisualisation.transform.localScale = Vector3.one / 4f;*/
                        } else {
                            if (perso.p3dData.objectList != null && perso.p3dData.objectList.Count > ntto.object_index) {
                                PhysicalObject o = perso.p3dData.objectList[ntto.object_index].po;
                                if (o != null) {
                                    //if (o.visualSetType == 1) print(name);
                                    PhysicalObject c = o.Clone();
                                    subObjects[i][j] = c;
									subObjects[i][j].Gao.transform.localScale =
										subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;
									c.Gao.transform.parent = channelObjects[i].transform;
                                    c.Gao.name = "[" + j + "] " + c.Gao.name;
									if (Settings.s.hasDeformations && c.Bones != null) hasBones = true;
									foreach (VisualSetLOD l in c.visualSet) {
										if (l.obj != null) {
											GameObject gao = l.obj.Gao;
											if (gao != null) gao.SetActive(!controller.viewCollision);
										}
									}
									if (c.collideMesh != null) c.collideMesh.SetVisualsActive(controller.viewCollision);
									c.Gao.SetActive(false);
                                }
                            }
                        }
                    }
                }

				if (a3d.num_morphData > 0 && a3d.morphData != null && Settings.s.engineVersion < Settings.EngineVersion.R3) {
					morphDataArray = new AnimMorphData[a3d.num_channels, a3d.num_onlyFrames];
					// Iterate over morph data to find the correct channel and keyframe
					for (int i = 0; i < a3d.num_morphData; i++) {
						AnimMorphData m = a3d.morphData[a3d.start_morphData + i];
						if (m != null) {
							//print(m.Offset);
							/*print("F:" + a3d.num_onlyFrames + " - C:" + a3d.num_channels + " - CF" + (a3d.num_onlyFrames * a3d.num_channels) + " - " +
								m.channel + " - " + m.frame + " - " + m.morphProgress + " - " + m.objectIndexTo + " - " + m.byte5 + " - " + m.byte6 + " - " + m.byte7);*/
							int channelIndex = this.GetChannelByID(m.channel)[0];
							if (channelIndex < morphDataArray.GetLength(0) && m.frame < morphDataArray.GetLength(1)) {
								morphDataArray[channelIndex, m.frame] = m;
								if (m.morphProgress == 100 && perso.p3dData.objectList != null && perso.p3dData.objectList.Count > m.objectIndexTo) {
									if (fullMorphPOs[channelIndex] == null) fullMorphPOs[channelIndex] = new Dictionary<ushort, PhysicalObject>();
									if (!fullMorphPOs[channelIndex].ContainsKey(m.objectIndexTo)) {
										PhysicalObject o = perso.p3dData.objectList[m.objectIndexTo].po;
										if (o != null) {
											PhysicalObject c = o.Clone();
											c.Gao.transform.localScale = c.scaleMultiplier.HasValue ? c.scaleMultiplier.Value : Vector3.one;
											c.Gao.transform.parent = channelObjects[channelIndex].transform;
											c.Gao.name = "[Morph] " + c.Gao.name;
											if (Settings.s.hasDeformations && c.Bones != null) hasBones = true;
											foreach (VisualSetLOD l in c.visualSet) {
												if (l.obj != null) {
													GameObject gao = l.obj.Gao;
													if (gao != null) gao.SetActive(!controller.viewCollision);
												}
											}
											if (c.collideMesh != null) c.collideMesh.SetVisualsActive(controller.viewCollision);
											c.Gao.SetActive(false);
											fullMorphPOs[channelIndex][m.objectIndexTo] = c;
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
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.None);
				}
			}
            isLoaded = true;
        }
    }

    void InitAnimationMontreal(AnimationMontreal animMontreal) {
        if (animMontreal != this.animMontreal || forceAnimUpdate) {
            forceAnimUpdate = false;
            DeinitAnimation();
            // Init animation
            this.animMontreal = animMontreal;
            currentFrame = 0;
            if (animMontreal != null) {
                //animationSpeed = a3d.speed;
                // Init channels & subobjects
                subObjects = new PhysicalObject[animMontreal.num_channels][];
                channelObjects = new GameObject[animMontreal.num_channels];
				currentActivePO = new int[animMontreal.num_channels];
				channelParents = new bool[animMontreal.num_channels];
				for (int i = 0; i < animMontreal.num_channels; i++) {
                    channelObjects[i] = new GameObject("Channel " + i);
                    channelObjects[i].transform.SetParent(perso.Gao.transform);
					currentActivePO[i] = -1;
                    subObjects[i] = new PhysicalObject[animMontreal.num_frames];
                    List<short> listOfNTTOforChannel = new List<short>();
                    for (int j = 0; j < animMontreal.num_frames; j++) {
                        AnimFrameMontreal of = animMontreal.frames[j];
                        if (!listOfNTTOforChannel.Contains(of.channels[i].objectIndex)) {
                            listOfNTTOforChannel.Add(of.channels[i].objectIndex);
                        }
                    }
                    for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
                        PhysicalObject subObj = null;
                        short object_index = listOfNTTOforChannel[k];
                        if (object_index == -1) {
                            subObj = new PhysicalObject(null);
                            subObj.Gao.transform.parent = channelObjects[i].transform;
                            subObj.Gao.name = "[" + i + "] Invisible PO";
                            subObj.Gao.SetActive(false);
                        } else {
                            if (perso.p3dData.objectList != null && perso.p3dData.objectList.Count > object_index) {
                                PhysicalObject o = perso.p3dData.objectList[object_index].po;
                                if (o != null) {
                                    subObj = o.Clone();
                                    subObj.Gao.transform.parent = channelObjects[i].transform;
                                    subObj.Gao.name = "[" + i + "] " + subObj.Gao.name;
									subObj.Gao.transform.localScale = subObj.scaleMultiplier.HasValue ? subObj.scaleMultiplier.Value : Vector3.one;
									foreach (VisualSetLOD l in subObj.visualSet) {
										if (l.obj != null) {
											GameObject gao = l.obj.Gao;
											if (gao != null) gao.SetActive(!controller.viewCollision);
										}
									}
									if (subObj.collideMesh != null) subObj.collideMesh.SetVisualsActive(controller.viewCollision);
									subObj.Gao.SetActive(false);
								}
                            }
                        }
                        for (int j = 0; j < animMontreal.num_frames; j++) {
                            AnimFrameMontreal of = animMontreal.frames[j];
                            if (of.channels[i].objectIndex == object_index) {
                                subObjects[i][j] = subObj;
                            }
                        }
                    }
				}
				if (!isAlways) {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.None);
				}
			}
            isLoaded = true;
        }
    }

	void InitAnimationLargo(AnimA3DLargo animLargo) {
		if (animLargo != this.animLargo || forceAnimUpdate) {
			forceAnimUpdate = false;
			DeinitAnimation();
			// Init animation
			this.animLargo = animLargo;
			currentFrame = 0;
			if (animLargo != null) {
				//animationSpeed = a3d.speed;
				// Init channels & subobjects
				subObjects = new PhysicalObject[animLargo.num_channels][];
				channelObjects = new GameObject[animLargo.num_channels];
				currentActivePO = new int[animLargo.num_channels];
				channelParents = new bool[animLargo.num_channels];
				for (int i = 0; i < animLargo.num_channels; i++) {
					short id = animLargo.channels[i].id;
					channelObjects[i] = new GameObject("Channel " + id);
					channelObjects[i].transform.SetParent(perso.Gao.transform);
					currentActivePO[i] = -1;
					AddChannelID(id, i);
					subObjects[i] = new PhysicalObject[animLargo.num_ntto];
					AnimChannelLargo ch = animLargo.channels[i];
					List<ushort> listOfNTTOforChannel = new List<ushort>();
					for (int j = 0; j < animLargo.num_onlyFrames; j++) {
						AnimOnlyFrame of = animLargo.onlyFrames[j];
						//print(ch.numOfNTTO + " - " + of.numOfNTTO + " - " + a3d.numOfNTTO.Length);
						AnimNumOfNTTO numOfNTTO = animLargo.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
						if (!listOfNTTOforChannel.Contains(numOfNTTO.numOfNTTO)) {
							listOfNTTOforChannel.Add(numOfNTTO.numOfNTTO);
						}
					}
					for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
						int j = listOfNTTOforChannel[k]; ;
						AnimNTTO ntto = animLargo.ntto[j];
						if (ntto.IsInvisibleNTTO) {
							subObjects[i][j] = new PhysicalObject(null);
							subObjects[i][j].Gao.transform.parent = channelObjects[i].transform;
							subObjects[i][j].Gao.name = "[" + j + "] Invisible PO";
							subObjects[i][j].Gao.SetActive(false);
							/*GameObject boneVisualisation = new GameObject("Bone vis");
                            boneVisualisation.transform.SetParent(subObjects[i][j].Gao.transform);
                            MeshRenderer mr = boneVisualisation.AddComponent<MeshRenderer>();
                            MeshFilter mf = boneVisualisation.AddComponent<MeshFilter>();
                            Mesh mesh = Util.CreateBox(0.1f);
                            mf.mesh = mesh;
                            boneVisualisation.transform.localScale = Vector3.one / 4f;*/
						} else {
							if (perso.p3dData.objectList != null && perso.p3dData.objectList.Count > ntto.object_index) {
								PhysicalObject o = perso.p3dData.objectList[ntto.object_index].po;
								if (o != null) {
									//if (o.visualSetType == 1) print(name);
									PhysicalObject c = o.Clone();
									subObjects[i][j] = c;
									subObjects[i][j].Gao.transform.localScale =
										subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;
									c.Gao.transform.parent = channelObjects[i].transform;
									c.Gao.name = "[" + j + "] " + c.Gao.name;
									if (Settings.s.hasDeformations && c.Bones != null) hasBones = true;
									foreach (VisualSetLOD l in c.visualSet) {
										if (l.obj != null) {
											GameObject gao = l.obj.Gao;
											if (gao != null) gao.SetActive(!controller.viewCollision);
										}
									}
									if (c.collideMesh != null) c.collideMesh.SetVisualsActive(controller.viewCollision);
									c.Gao.SetActive(false);
								}
							}
						}
					}
				}
	
				morphDataArray = null;

				// Keep lighting last so that it is applied to all new sub objects
				if (!isAlways) {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.Perso);
				} else {
					controller.sectorManager.ApplySectorLighting(sector, gameObject, LightInfo.ObjectLightedFlag.None);
				}
			}
			isLoaded = true;
		}
	}

	public void UpdateAnimation() {
		if (isLoaded && a3d != null && channelObjects != null & subObjects != null) {
			if (currentFrame >= a3d.num_onlyFrames) currentFrame %= a3d.num_onlyFrames;
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
			}
			AnimOnlyFrame of = a3d.onlyFrames[a3d.start_onlyFrames + currentFrame];
			// Create hierarchy for this frame
			for (int i = of.start_hierarchies_for_frame;
				i < of.start_hierarchies_for_frame + of.num_hierarchies_for_frame; i++) {
				AnimHierarchy h = a3d.hierarchies[i];

				if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
					channelObjects[h.childChannelID].transform.SetParent(channelObjects[h.parentChannelID].transform);
					channelParents[h.childChannelID] = true;
				} else {
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
				}

				//channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
			}
			// Final pass
			for (int i = 0; i < a3d.num_channels; i++) {
				AnimChannel ch = a3d.channels[a3d.start_channels + i];
				AnimFramesKFIndex kfi = a3d.framesKFIndex[currentFrame + ch.framesKF];
				AnimKeyframe kf = a3d.keyframes[kfi.kf];
				//print(perso.p3dData.family.animBank);
				AnimVector pos = a3d.vectors[kf.positionVector];
				AnimQuaternion qua = a3d.quaternions[kf.quaternion];
				AnimVector scl = a3d.vectors[kf.scaleVector];
				AnimNumOfNTTO numOfNTTO = a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
				AnimNTTO ntto = a3d.ntto[numOfNTTO.numOfNTTO];
				//if (ntto.IsBoneNTTO) continue;
				int poNum = numOfNTTO.numOfNTTO - a3d.start_NTTO;
				PhysicalObject physicalObject = subObjects[i][poNum];
				Vector3 vector = pos.vector;
				Quaternion quaternion = qua.quaternion;
				Vector3 scale = scl.vector;
				int framesSinceKF = (int)currentFrame - (int)kf.frame;
				AnimKeyframe nextKF = null;
				int framesDifference;
				float interpolation;
				if (kf.IsEndKeyframe) {
					AnimFramesKFIndex next_kfi = a3d.framesKFIndex[0 + ch.framesKF];
					nextKF = a3d.keyframes[next_kfi.kf];
					framesDifference = a3d.num_onlyFrames - 1 + (int)nextKF.frame - (int)kf.frame;
					if (framesDifference == 0) {
						interpolation = 0;
					} else {
						//interpolation = (float)(nextKF.interpolationFactor * (framesSinceKF / (float)framesDifference) + 1.0 * nextKF.interpolationFactor);
						interpolation = framesSinceKF / (float)framesDifference;
					}
				} else {
					nextKF = a3d.keyframes[kfi.kf + 1];
					framesDifference = (int)nextKF.frame - (int)kf.frame;
					//interpolation = (float)(nextKF.interpolationFactor * (framesSinceKF / (float)framesDifference) + 1.0 * nextKF.interpolationFactor);
					interpolation = framesSinceKF / (float)framesDifference;
				}
				//print(interpolation);
				//print(a3d.vectors.Length + " - " + nextKF.positionVector);
				//print(perso.p3dData.family.animBank);
				AnimVector pos2 = a3d.vectors[nextKF.positionVector];
				AnimQuaternion qua2 = a3d.quaternions[nextKF.quaternion];
				AnimVector scl2 = a3d.vectors[nextKF.scaleVector];
				vector = Vector3.Lerp(pos.vector, pos2.vector, interpolation);
				quaternion = Quaternion.Lerp(qua.quaternion, qua2.quaternion, interpolation);
				scale = Vector3.Lerp(scl.vector, scl2.vector, interpolation);
				float positionMultiplier = Mathf.Lerp(kf.positionMultiplier, nextKF.positionMultiplier, interpolation);

				if (poNum != currentActivePO[i]) {
					if (currentActivePO[i] == -2 && fullMorphPOs != null && fullMorphPOs[i] != null) {
						foreach (PhysicalObject morphPO in fullMorphPOs[i].Values) {
							if (morphPO.Gao.activeSelf) morphPO.Gao.SetActive(false);
						}
					}
					if (currentActivePO[i] >= 0 && subObjects[i][currentActivePO[i]] != null) {
						subObjects[i][currentActivePO[i]].Gao.SetActive(false);
					}
					currentActivePO[i] = poNum;
					if (physicalObject != null) physicalObject.Gao.SetActive(true);
				}
				if (!channelParents[i]) channelObjects[i].transform.SetParent(perso.Gao.transform);
				channelObjects[i].transform.localPosition = vector * positionMultiplier;
				channelObjects[i].transform.localRotation = quaternion;
				channelObjects[i].transform.localScale = scale;

				if (physicalObject != null && a3d.num_morphData > 0 && morphDataArray != null && i < morphDataArray.GetLength(0) && currentFrame < morphDataArray.GetLength(1)) {
					AnimMorphData morphData = morphDataArray[i, currentFrame];

					if (morphData != null && morphData.morphProgress != 0 && morphData.morphProgress != 100) {
						PhysicalObject morphToPO = perso.p3dData.objectList[morphData.objectIndexTo].po;
						Vector3[] morphVerts = null;

						for (int j = 0; j < physicalObject.visualSet.Length; j++) {
							IGeometricObject obj = physicalObject.visualSet[j].obj;
							if (obj == null || obj as GeometricObject == null) continue;
							GeometricObject fromM = obj as GeometricObject;
							GeometricObject toM = morphToPO.visualSet[j].obj as GeometricObject;
							if (toM == null) continue;
							if (fromM.vertices.Length != toM.vertices.Length) {
								// For those special cases like the mistake in the Clark cinematic
								continue;
							}
							int numVertices = fromM.vertices.Length;
							morphVerts = new Vector3[numVertices];
							for (int vi = 0; vi < numVertices; vi++) {
								morphVerts[vi] = Vector3.Lerp(fromM.vertices[vi], toM.vertices[vi], morphData.morphProgressFloat);
							}
							for (int k = 0; k < fromM.num_elements; k++) {
								if (fromM.elements[k] == null || fromM.element_types[k] != 1) continue;
								GeometricObjectElementTriangles el = (GeometricObjectElementTriangles)fromM.elements[k];
								if (el != null) el.UpdateMeshVertices(morphVerts);
							}
						}
					} else if (morphData != null && morphData.morphProgress == 100) {
						physicalObject.Gao.SetActive(false);
						PhysicalObject c = fullMorphPOs[i][morphData.objectIndexTo];
						c.Gao.transform.localScale = c.scaleMultiplier.HasValue ? c.scaleMultiplier.Value : Vector3.one;
						c.Gao.transform.localPosition = Vector3.zero;
						c.Gao.transform.localRotation = Quaternion.identity;
						c.Gao.SetActive(true);
						currentActivePO[i] = -2;
					} else {
						for (int j = 0; j < physicalObject.visualSet.Length; j++) {
							IGeometricObject obj = physicalObject.visualSet[j].obj;
							if (obj == null || obj as GeometricObject == null) continue;
							GeometricObject fromM = obj as GeometricObject;
							for (int k = 0; k < fromM.num_elements; k++) {
								if (fromM.elements[k] == null || fromM.element_types[k] != 1) continue;
								GeometricObjectElementTriangles el = (GeometricObjectElementTriangles)fromM.elements[k];
								if (el != null) el.ResetVertices();
							}
						}
					}
				}
			}
            /* */
			if (hasBones) {
				for (int i = 0; i < a3d.num_channels; i++) {
					AnimChannel ch = a3d.channels[a3d.start_channels + i];
					Transform baseChannelTransform = channelObjects[i].transform;
					Vector3 invertedScale = new Vector3(1f / baseChannelTransform.localScale.x, 1f / baseChannelTransform.localScale.y, 1f / baseChannelTransform.localScale.z);
					AnimNumOfNTTO numOfNTTO = a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
					AnimNTTO ntto = a3d.ntto[numOfNTTO.numOfNTTO];
					PhysicalObject physicalObject = subObjects[i][numOfNTTO.numOfNTTO - a3d.start_NTTO];
					if (physicalObject == null) continue;
					DeformSet bones = physicalObject.Bones;
					// Deformations
					if (bones != null) {
						for (int j = 0; j < a3d.num_deformations; j++) {
							AnimDeformation d = a3d.deformations[a3d.start_deformations + j];
							if (d.channel < ch.id) continue;
							if (d.channel > ch.id) break;
							if (!channelIDDictionary.ContainsKey(d.linkChannel)) continue;
							List<int> ind_linkChannel_list = GetChannelByID(d.linkChannel);
							foreach (int ind_linkChannel in ind_linkChannel_list) {
								AnimChannel ch_link = a3d.channels[a3d.start_channels + ind_linkChannel];
								AnimNumOfNTTO numOfNTTO_link = a3d.numOfNTTO[ch_link.numOfNTTO + of.numOfNTTO];
								AnimNTTO ntto_link = a3d.ntto[numOfNTTO_link.numOfNTTO];
								PhysicalObject physicalObject_link = subObjects[ind_linkChannel][numOfNTTO_link.numOfNTTO - a3d.start_NTTO];
								if (physicalObject_link == null) continue;
								if (bones == null || bones.bones.Length <= d.bone + 1) continue;
								DeformBone bone = bones.r3bones[d.bone + 1];
								if (bone != null) {
									Transform channelTransform = channelObjects[ind_linkChannel].transform;
                                    bone.UnityBone.transform.SetParent(channelTransform);

                                    //bone.UnityBone.position = new Vector3(channelTransform.position.x, channelTransform.position.y, channelTransform.position.z);

                                    bone.UnityBone.localPosition = Vector3.zero;
									bone.UnityBone.localRotation = Quaternion.identity;
									bone.UnityBone.localScale = Vector3.one;
								}
							}
						}
					}
				}
			}
            /* */
			//this.currentFrame = (currentFrame + 1) % a3d.num_onlyFrames;
		} else if (isLoaded && animMontreal != null && channelObjects != null & subObjects != null) {
			UpdateFrameMontreal();
		} else if (isLoaded && animLargo != null && channelObjects != null && subObjects != null) {
			UpdateFrameLargo();
		}
    }

    void UpdateFrameMontreal() {
        if (isLoaded && animMontreal != null && channelObjects != null & subObjects != null) {
            if (currentFrame >= animMontreal.num_frames) currentFrame %= animMontreal.num_frames;
            // First pass: reset TRS for all sub objects
            for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
                /*GameObject c = channelObjects[i];
                if (c != null) {
                    c.transform.SetParent(perso.Gao.transform);
                    c.transform.localPosition = Vector3.zero;
                    c.transform.localEulerAngles = Vector3.zero;
                    c.transform.localScale = Vector3.one; // prevent float precision errors after a long time, lol
                }
                for (int j = 0; j < subObjects[i].Length; j++) {
                    if (subObjects[i][j] == null) continue;
                    subObjects[i][j].Gao.transform.parent = c.transform;
                    subObjects[i][j].Gao.transform.localPosition = Vector3.zero;
                    subObjects[i][j].Gao.transform.localEulerAngles = Vector3.zero;
                    subObjects[i][j].Gao.transform.localScale =
                        subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;
                    subObjects[i][j].Gao.SetActive(false);
                }*/
            }
            AnimFrameMontreal of = animMontreal.frames[currentFrame];
			// Create hierarchy for this frame
			//bool[] channelParents = new bool[channelObjects.Length];
			if (of.hierarchies != null) {
                for (int i = 0; i < of.hierarchies.Length; i++) {
                    AnimHierarchy h = of.hierarchies[i];
                    channelObjects[h.childChannelID].transform.SetParent(channelObjects[h.parentChannelID].transform);
					channelParents[h.childChannelID] = true;
                }
            }
            // Final pass
            for (int i = 0; i < animMontreal.num_channels; i++) {
                AnimChannelMontreal ch = of.channels[i];
                PhysicalObject physicalObject = subObjects[i][currentFrame];
                Vector3 vector, scale;
                Quaternion quaternion;
                if (ch.matrix != null) {
                    vector = ch.matrix.GetPosition(convertAxes: true);
                    quaternion = ch.matrix.GetRotation(convertAxes: true);
                    scale = ch.matrix.GetScale(convertAxes: true);
                } else {
                    vector = Vector3.zero;
                    quaternion = Quaternion.identity;
                    scale = Vector3.one;
                }
				//float positionMultiplier = Mathf.Lerp(kf.positionMultiplier, nextKF.positionMultiplier, interpolation);


				if (currentFrame != currentActivePO[i]) {
					if (currentActivePO[i] >= 0 && subObjects[i][currentActivePO[i]] != null) {
						subObjects[i][currentActivePO[i]].Gao.SetActive(false);
					}
					currentActivePO[i] = (int)currentFrame;
					if (physicalObject != null) physicalObject.Gao.SetActive(true);
				}
				if (!channelParents[i]) channelObjects[i].transform.SetParent(perso.Gao.transform);
				channelObjects[i].transform.localPosition = vector;// * positionMultiplier;
                channelObjects[i].transform.localRotation = quaternion;
                channelObjects[i].transform.localScale = scale;
            }
            //this.currentFrame = (currentFrame + 1) % animMontreal.num_frames;
        }
    }


	public void UpdateFrameLargo() {
		if (isLoaded && animLargo != null && channelObjects != null & subObjects != null) {
			if (currentFrame >= animLargo.num_onlyFrames) currentFrame %= animLargo.num_onlyFrames;
			// First pass: reset TRS for all sub objects
			for (int i = 0; i < channelParents.Length; i++) {
				channelParents[i] = false;
				/*GameObject c = channelObjects[i];
                if (c != null) {
                    c.transform.SetParent(perso.Gao.transform);
                    c.transform.localPosition = Vector3.zero;
                    c.transform.localEulerAngles = Vector3.zero;
                    c.transform.localScale = Vector3.one; // prevent float precision errors after a long time, lol
                }
               for (int j = 0; j < subObjects[i].Length; j++) {
                    if (subObjects[i][j] == null) continue;
                    subObjects[i][j].Gao.transform.parent = c.transform;
                    subObjects[i][j].Gao.transform.localPosition = Vector3.zero;
                    subObjects[i][j].Gao.transform.localEulerAngles = Vector3.zero;
                    subObjects[i][j].Gao.transform.localScale =
                        subObjects[i][j].scaleMultiplier.HasValue ? subObjects[i][j].scaleMultiplier.Value : Vector3.one;
                    //subObjects[i][j].Gao.SetActive(false);
                }*/
			}
			AnimOnlyFrame of = animLargo.onlyFrames[currentFrame];
			// Create hierarchy for this frame
			for (int i = of.start_hierarchies_for_frame;
				i < of.start_hierarchies_for_frame + of.num_hierarchies_for_frame; i++) {
				AnimHierarchy h = animLargo.hierarchies[i];

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
			int startFrameVectors = 0;
			int startFrameQuaternions = 0;
			for (int i = 0; i < animLargo.num_channels; i++) {
				AnimChannelLargo ch = animLargo.channels[i];
				AnimFrameVector fv = null;
				AnimFrameQuaternion fq = null;
				int? curFV = null, curFQ = null, nextFV = null, nextFQ = null;
				if (animLargo.channels[i].numFrameVectors > 0) {
					curFV = startFrameVectors + animLargo.channels[i].numFrameVectors - 1;
					nextFV = startFrameVectors;
					for (int j = 0; j < animLargo.channels[i].numFrameVectors; j++) {
						if (animLargo.framevector[startFrameVectors + j].frame > currentFrame) {
							nextFV = startFrameVectors + j;
							break;
						}
						curFV = startFrameVectors + j;
					}
					fv = animLargo.framevector[curFV.Value];
					startFrameVectors += animLargo.channels[i].numFrameVectors;
				}
				if (animLargo.channels[i].numFrameQuaternions > 0) {
					curFQ = startFrameQuaternions + animLargo.channels[i].numFrameQuaternions - 1;
					nextFQ = startFrameQuaternions;
					for (int j = 0; j < animLargo.channels[i].numFrameQuaternions; j++) {
						if (animLargo.framequaternion[startFrameQuaternions + j].frame > currentFrame) {
							nextFQ = startFrameQuaternions + j;
							break;
						}
						curFQ = startFrameQuaternions + j;
					}
					fq = animLargo.framequaternion[curFQ.Value];
					startFrameQuaternions += animLargo.channels[i].numFrameQuaternions;
				}
				AnimVector pos = fv != null ? animLargo.vectors[fv.vector] : null;
				AnimQuaternion qua = fq != null ? animLargo.quaternions[fq.quaternion] : null;
				AnimVector scl = null; // animLargo.vectors[0];
				AnimNumOfNTTO numOfNTTO = animLargo.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
				AnimNTTO ntto = animLargo.ntto[numOfNTTO.numOfNTTO];
				//if (ntto.IsBoneNTTO) continue;
				int poNum = numOfNTTO.numOfNTTO;
				PhysicalObject physicalObject = subObjects[i][poNum];
				Vector3 vector = pos != null ? pos.vector : Vector3.zero;
				Quaternion quaternion = qua != null ? qua.quaternion : Quaternion.identity;
				Vector3 scale = scl != null ? scl.vector : Vector3.one;

				// Interpolation
				if (nextFV.HasValue) {
					AnimFrameVector fv2 = animLargo.framevector[nextFV.Value];
					AnimVector pos2 = animLargo.vectors[fv2.vector];
					int curFrame = fv.frame;
					int nextFrame = fv2.frame;
					if (curFrame != nextFrame) {
						float interpolation = ((((int)currentFrame + animLargo.num_onlyFrames) - curFrame) % animLargo.num_onlyFrames) /
							(((nextFrame + animLargo.num_onlyFrames) - curFrame) % animLargo.num_onlyFrames);
						vector = Vector3.Lerp(pos.vector, pos2.vector, interpolation);
					}
				}
				if (nextFQ.HasValue) {
					AnimFrameQuaternion fq2 = animLargo.framequaternion[nextFQ.Value];
					AnimQuaternion qua2 = animLargo.quaternions[fq2.quaternion];
					int curFrame = fq.frame;
					int nextFrame = fq2.frame;
					if (curFrame != nextFrame) {
						float interpolation = ((((int)currentFrame + animLargo.num_onlyFrames) - curFrame) % animLargo.num_onlyFrames) /
							(((nextFrame + animLargo.num_onlyFrames) - curFrame) % animLargo.num_onlyFrames);
						quaternion = Quaternion.Lerp(qua.quaternion, qua2.quaternion, interpolation);
					}
				}


				//print(interpolation);
				//print(a3d.vectors.Length + " - " + nextKF.positionVector);
				//print(perso.p3dData.family.animBank);
				//AnimVector scl2 = a3d.vectors[nextKF.scaleVector];
				//scale = Vector3.Lerp(scl.vector, scl2.vector, interpolation);
				//float positionMultiplier = Mathf.Lerp(kf.positionMultiplier, nextKF.positionMultiplier, interpolation);

				if (poNum != currentActivePO[i]) {
					if (currentActivePO[i] == -2 && fullMorphPOs != null && fullMorphPOs[i] != null) {
						foreach (PhysicalObject morphPO in fullMorphPOs[i].Values) {
							if (morphPO.Gao.activeSelf) morphPO.Gao.SetActive(false);
						}
					}
					if (currentActivePO[i] >= 0 && subObjects[i][currentActivePO[i]] != null) {
						subObjects[i][currentActivePO[i]].Gao.SetActive(false);
					}
					currentActivePO[i] = poNum;
					if (physicalObject != null) physicalObject.Gao.SetActive(true);
				}
				if (!channelParents[i]) channelObjects[i].transform.SetParent(perso.Gao.transform);
				channelObjects[i].transform.localPosition = vector; // * positionMultiplier;
				channelObjects[i].transform.localRotation = quaternion;
				channelObjects[i].transform.localScale = scale;
			}
			if (hasBones) {
				for (int i = 0; i < animLargo.num_channels; i++) {
					AnimChannelLargo ch = animLargo.channels[i];
					Transform baseChannelTransform = channelObjects[i].transform;
					Vector3 invertedScale = new Vector3(1f / baseChannelTransform.localScale.x, 1f / baseChannelTransform.localScale.y, 1f / baseChannelTransform.localScale.z);
					AnimNumOfNTTO numOfNTTO = animLargo.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
					AnimNTTO ntto = animLargo.ntto[numOfNTTO.numOfNTTO];
					PhysicalObject physicalObject = subObjects[i][numOfNTTO.numOfNTTO];
					if (physicalObject == null) continue;
					DeformSet bones = physicalObject.Bones;
					// Deformations
					if (bones != null) {
						for (int j = 0; j < animLargo.num_deformations; j++) {
							AnimDeformation d = animLargo.deformations[j];
							if (d.channel < ch.id) continue;
							if (d.channel > ch.id) break;
							if (!channelIDDictionary.ContainsKey(d.linkChannel)) continue;
							List<int> ind_linkChannel_list = GetChannelByID(d.linkChannel);
							foreach (int ind_linkChannel in ind_linkChannel_list) {
								AnimChannelLargo ch_link = animLargo.channels[ind_linkChannel];
								AnimNumOfNTTO numOfNTTO_link = animLargo.numOfNTTO[ch_link.numOfNTTO + of.numOfNTTO];
								AnimNTTO ntto_link = animLargo.ntto[numOfNTTO_link.numOfNTTO];
								//PhysicalObject physicalObject_link = subObjects[ind_linkChannel][numOfNTTO_link.numOfNTTO];
								//if (physicalObject_link == null) continue;
								if (bones == null || bones.bones.Length <= d.bone + 1) continue;
								//print(d.bone + 1);
								DeformBone bone = bones.r3bones[d.bone + 1];
								if (bone != null) {
									Transform channelTransform = channelObjects[ind_linkChannel].transform;
									bone.UnityBone.transform.SetParent(channelTransform);
									bone.UnityBone.localPosition = Vector3.zero;
									bone.UnityBone.localRotation = Quaternion.identity;
									bone.UnityBone.localScale = Vector3.one;
									/*bone.UnityBone.position = channelTransform.position;
									bone.UnityBone.rotation = channelTransform.rotation;
									//bone.UnityBone.localScale = Vector3.one;
									bone.UnityBone.localScale = channelTransform.localScale;*/
								}
							}
						}
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
        if (state != null && state.off_nextState != null) {
            State state_auto = State.FromOffset(perso.p3dData.family, state.off_nextState);
            if (state_auto != null) {
                int indexOfStateAuto = perso.p3dData.family.states.IndexOf(state_auto);
                if (indexOfStateAuto > -1) SetState(indexOfStateAuto);
            }
        }
    }
}