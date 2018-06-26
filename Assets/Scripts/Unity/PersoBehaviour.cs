using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.Animation.Component;
using OpenSpace.EngineObject;
using OpenSpace.Visual;
using OpenSpace.Visual.Deform;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersoBehaviour : MonoBehaviour {
    bool loaded = false;
    bool hasStates = false;
    public Perso perso;
    public State state = null;
    public string[] stateNames = { "Placeholder" };
    int currentState = 0;
    public int stateIndex = 0;

    // Animation
    public AnimA3DGeneral a3d = null;
    public uint currentFrame = 0;
    public bool playAnimation = true;
    public float animationSpeed = 15f;
    private float updateCounter = 0f;
    private PhysicalObject[][] subObjects = null; // [channel][ntto]
    private GameObject[] channelObjects = null;
    private Dictionary<short, List<int>> channelIDDictionary = new Dictionary<short, List<int>>();

    // Use this for initialization
    void Start() {
    }

    public void Init() {
        if (perso != null) {
            Family fam = perso.family;
            if (fam != null && fam.states != null && fam.states.Length > 0) {
                stateNames = fam.states.Select(s => (s == null ? "Null" : s.ToString())).ToArray();
                hasStates = true;
                state = perso.initialState;
                for (int i = 0; i < fam.states.Length; i++) {
                    if (state == fam.states[i]) {
                        currentState = i;
                        stateIndex = i;
                        SetState(i);
                        break;
                    }
                }
                if (state == null && fam.states.Length > 0) {
                    currentState = 0;
                    stateIndex = 0;
                    SetState(0);
                }
            }
        }
        loaded = true;
    }

    public void PrintDsgVar()
    {
        if (loaded && hasStates) {
            if (perso.brain != null && perso.brain.mind != null) {

                DsgVar dsgVar = perso.brain.mind.AI_model.dsgVar;
                if (dsgVar != null) {
                    MapLoader l = MapLoader.Loader;
                    l.print("DsgVar.offset: " + dsgVar.offset);
                    l.print("DsgVarFromModel.amountOfInfos: " + dsgVar.amountOfInfos);
                    l.print("DsgVarFromModel.dsgMemBufferLength: " + dsgVar.dsgMemBufferLength);

                    int c = 0;

                    foreach (DsgVarInfoEntry entry in dsgVar.dsgVarInfos) {
                        l.print("Info Entry " + c + ", type " + entry.type + ", offset " + entry.offsetInBuffer + " , value " + entry.value + ", initType " + entry.initType + ", saveType " + entry.saveType);
                        c++;
                    }
                }

            }
        }
    }

    public void PrintDsgVarFromMindMem()
    {
        MapLoader l = MapLoader.Loader;

        if (loaded && hasStates) {
            if (perso.brain != null && perso.brain.mind != null) {

                DsgMem dsgMem = perso.brain.mind.dsgMem;
                if (dsgMem != null) {
                    l.print("DsgMem.offset: " + dsgMem.offset);
                    DsgVar dsgVar = perso.brain.mind.dsgMem.dsgVar;
                    if (dsgVar != null) {
                        l.print("DsgVar.offset: " + dsgVar.offset);
                        l.print("DsgVarFromMem.amountOfInfos: " + dsgVar.amountOfInfos);
                        l.print("DsgVarFromMem.dsgMemBufferLength: " + dsgVar.dsgMemBufferLength);

                        int c = 0;

                        foreach (DsgVarInfoEntry entry in dsgVar.dsgVarInfos) {
                            l.print("Info Entry " + c + ", type " + entry.type + ", offset " + entry.offsetInBuffer + " , value " + entry.value + ", initType " + entry.initType + ", saveType " + entry.saveType);
                            c++;
                        }
                    }
                }



            }
        }
    }

    public void PrintScripts()
    {
        if (loaded && hasStates) {
            if (perso.brain != null
                && perso.brain.mind != null
                && perso.brain.mind.AI_model != null) {
                AIModel ai = perso.brain.mind.AI_model;
                if (ai.behaviors_normal != null) {
                    for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                        if (ai.behaviors_normal[i].scripts != null) {
                            for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                                perso.brain.mind.AI_model.behaviors_normal[i].scripts[j].print(perso);
                            }
                        }
                    }
                }
                if (ai.behaviors_reflex != null) {
                    for (int i = 0; i < ai.behaviors_reflex.Length; i++) {
                        if (ai.behaviors_reflex[i].scripts != null) {
                            for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++) {
                                perso.brain.mind.AI_model.behaviors_reflex[i].scripts[j].print(perso);
                            }
                        }
                    }
                }
                if (ai.macros != null) {
                    for (int i = 0; i < ai.macros.Length; i++) {
                        if (ai.macros[i].script != null) {
                            perso.brain.mind.AI_model.macros[i].script.print(perso);
                        }
                    }
                }
            }
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
                    AnimChannel c = a3d.channels[i + a3d.start_channels];
                    print("Channel " + i + ": ID:" + c.id + " - U0:" + c.unk0 + " - V:" + c.vector + " - ");
                }
                print("Objects offset: " + perso.family.off_physical_list_first);
            } else if (state != null) {

                MapLoader l = MapLoader.Loader;
                ushort anim_index = 0;
                byte bank_index = 0;
                if (state.anim_ref != null) {
                    anim_index = state.anim_ref.anim_index;
                    bank_index = perso.family.animBank;
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

    public void SetState(int index) {
        if (index < 0 || index >= perso.family.states.Length) return;
        stateIndex = index;
        currentState = index;
        state = perso.family.states[index];

        // Set animation
        MapLoader l = MapLoader.Loader;
        ushort anim_index = 0;
        byte bank_index = 0;
        if (state.anim_ref != null) {
            anim_index = state.anim_ref.anim_index;
            bank_index = perso.family.animBank;
        }
        if (state.anim_ref != null
            && l.animationBanks != null
            && l.animationBanks.Length > bank_index
            && l.animationBanks[bank_index] != null
            && l.animationBanks[bank_index].animations != null
            && l.animationBanks[bank_index].animations.Length > anim_index
            && l.animationBanks[bank_index].animations[anim_index] != null) {
            InitAnimation(l.animationBanks[bank_index].animations[anim_index]);
            UpdateFrame(currentFrame);
        } else {
            a3d = null;
        }
    }

    // Update is called once per frame
    void Update() {
        if (loaded && hasStates) {
            if (stateIndex != currentState) {
                currentState = stateIndex;
                SetState(currentState);
            }
        }
        if (playAnimation) {
            updateCounter += Time.deltaTime * animationSpeed;
            if (updateCounter >= 1f) {
                updateCounter %= 1f;
                UpdateFrame(currentFrame);
            }
        }
    }

    void InitAnimation(AnimA3DGeneral a3d) {
        if (a3d != this.a3d) {
            loaded = false;
            // Destroy currently loaded subobjects
            if (subObjects != null) {
                for (int i = 0; i < subObjects.Length; i++) {
                    if (subObjects[i] == null) continue;
                    for (int j = 0; j < subObjects[i].Length; j++) {
                        if (subObjects[i][j] != null) subObjects[i][j].Destroy();
                    }
                }
                subObjects = null;
            }
            if (channelObjects != null) {
                for (int i = 0; i < channelObjects.Length; i++) {
                    if (channelObjects[i] != null) Destroy(channelObjects[i]);
                }
                channelObjects = null;
            }
            channelIDDictionary.Clear();
            // Init animation
            this.a3d = a3d;
            currentFrame = 0;
            if (a3d != null) {
                // Init channels & subobjects
                subObjects = new PhysicalObject[a3d.num_channels][];
                channelObjects = new GameObject[a3d.num_channels];
                for (int i = 0; i < a3d.num_channels; i++) {
                    short id = a3d.channels[a3d.start_channels + i].id;
                    channelObjects[i] = new GameObject("Channel " + id);
                    channelObjects[i].transform.SetParent(perso.Gao.transform);
                    AddChannelID(id, i);
                    subObjects[i] = new PhysicalObject[a3d.num_NTTO];
                    AnimChannel ch = a3d.channels[a3d.start_channels + i];
                    List<ushort> listOfNTTOforChannel = new List<ushort>();
                    for (int j = 0; j < a3d.num_onlyFrames; j++) {
                        AnimOnlyFrame of = a3d.onlyFrames[a3d.start_onlyFrames + j];
                        AnimNumOfNTTO numOfNTTO = a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
                        if (!listOfNTTOforChannel.Contains(numOfNTTO.numOfNTTO)) {
                            listOfNTTOforChannel.Add(numOfNTTO.numOfNTTO);
                        }
                    }
                    for (int k = 0; k < listOfNTTOforChannel.Count; k++) {
                        int j = listOfNTTOforChannel[k] - a3d.start_NTTO;
                        AnimNTTO ntto = a3d.ntto[a3d.start_NTTO + j];
                        if (ntto.IsBoneNTTO) {
                            subObjects[i][j] = new PhysicalObject(null);
                            subObjects[i][j].Gao.transform.parent = channelObjects[i].transform;
                            subObjects[i][j].Gao.name = "[" + j + "] Bone PO";
                            subObjects[i][j].Gao.SetActive(false);
                            /*GameObject boneVisualisation = new GameObject("Bone vis");
                            boneVisualisation.transform.SetParent(subObjects[i][j].Gao.transform);
                            MeshRenderer mr = boneVisualisation.AddComponent<MeshRenderer>();
                            MeshFilter mf = boneVisualisation.AddComponent<MeshFilter>();
                            Mesh mesh = Util.CreateBox(0.1f);
                            mf.mesh = mesh;
                            boneVisualisation.transform.localScale = Vector3.one / 4f;*/
                        } else {
                            if (perso.physical_objects != null && perso.physical_objects.Length > ntto.object_index) {
                                PhysicalObject o = perso.physical_objects[ntto.object_index];
                                if (o != null) {
                                    PhysicalObject c = o.Clone();
                                    subObjects[i][j] = c;
                                    c.Gao.transform.parent = channelObjects[i].transform;
                                    c.Gao.name = "[" + j + "] " + c.Gao.name;
                                    c.Gao.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
            loaded = true;
        }
    }

    void UpdateFrame(uint currentFrame) {
        if (loaded && a3d != null && channelObjects != null & subObjects != null) {
            // First pass: reset TRS for all sub objects
            for (int i = 0; i < channelObjects.Length; i++) {
                GameObject c = channelObjects[i];
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
                }
            }
            AnimOnlyFrame of = a3d.onlyFrames[a3d.start_onlyFrames + currentFrame];
            // Create hierarchy for this frame
            for (int i = of.start_hierarchies_for_frame;
                i < of.start_hierarchies_for_frame + of.num_hierarchies_for_frame; i++) {
                AnimHierarchy h = a3d.hierarchies[i];

                if (!channelIDDictionary.ContainsKey(h.childChannelID)
                    || !channelIDDictionary.ContainsKey(h.parentChannelID)) {
                    continue;
                }
                List<int> ch_child_list = GetChannelByID(h.childChannelID);
                List<int> ch_parent_list = GetChannelByID(h.parentChannelID);
                foreach (int ch_child in ch_child_list) {
                    foreach (int ch_parent in ch_parent_list) {
                        channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
                    }
                }

                //channelObjects[ch_child].transform.SetParent(channelObjects[ch_parent].transform);
            }
            // Final pass
            for (int i = 0; i < a3d.num_channels; i++) {
                AnimChannel ch = a3d.channels[a3d.start_channels + i];
                AnimFramesKFIndex kfi = a3d.framesKFIndex[currentFrame + ch.framesKF];
                AnimKeyframe kf = a3d.keyframes[kfi.kf];
                AnimVector pos = a3d.vectors[kf.positionVector];
                AnimQuaternion qua = a3d.quaternions[kf.quaternion];
                AnimVector scl = a3d.vectors[kf.scaleVector];
                AnimNumOfNTTO numOfNTTO = a3d.numOfNTTO[ch.numOfNTTO + of.numOfNTTO];
                AnimNTTO ntto = a3d.ntto[numOfNTTO.numOfNTTO];
                //if (ntto.IsBoneNTTO) continue;
                PhysicalObject physicalObject = subObjects[i][numOfNTTO.numOfNTTO - a3d.start_NTTO];
                Vector3 vector = pos.vector;
                Quaternion quaternion = qua.quaternion;
                Vector3 scaleMultiplier = a3d.vectors[ch.vector].vector;
                //scaleMultiplier = new Vector3(Mathf.Sign(scaleMultiplier.x), Mathf.Sign(scaleMultiplier.y), Mathf.Sign(scaleMultiplier.z));
                scaleMultiplier = Vector3.one;
                Vector3 scale = scl.vector;
                scale = new Vector3(scale.x * scaleMultiplier.x, scale.y * scaleMultiplier.y, scale.z * scaleMultiplier.z);
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
                AnimVector pos2 = a3d.vectors[nextKF.positionVector];
                AnimQuaternion qua2 = a3d.quaternions[nextKF.quaternion];
                AnimVector scl2 = a3d.vectors[nextKF.scaleVector];
                vector = Vector3.Lerp(pos.vector, pos2.vector, interpolation);
                quaternion = Quaternion.Lerp(qua.quaternion, qua2.quaternion, interpolation);
                scale = Vector3.Lerp(scl.vector, scl2.vector, interpolation);
                scale = new Vector3(scale.x * scaleMultiplier.x, scale.y * scaleMultiplier.y, scale.z * scaleMultiplier.z);
                float positionMultiplier = Mathf.Lerp(kf.positionMultiplier, nextKF.positionMultiplier, interpolation);

                if (physicalObject != null) physicalObject.Gao.SetActive(true);
                channelObjects[i].transform.localPosition = vector * positionMultiplier;
                channelObjects[i].transform.localRotation = quaternion;
                channelObjects[i].transform.localScale = scale;
            }
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
            this.currentFrame = (currentFrame + 1) % a3d.num_onlyFrames;
        } /*else if (loaded && (a3d == null || !playAnimation) && perso.physical_objects != null) {
            for (int i = 0; i < perso.physical_objects.Length; i++) {
                if (perso.physical_objects[i] != null) {
                    GameObject poGao = perso.physical_objects[i].Gao;
                    if (poGao != null && !poGao.activeSelf) {
                        poGao.transform.SetParent(perso.Gao.transform);
                        poGao.transform.localPosition = Vector3.zero;
                        poGao.transform.localEulerAngles = Vector3.zero;
                        poGao.SetActive(true);
                    }
                }
            }
        }*/
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
}