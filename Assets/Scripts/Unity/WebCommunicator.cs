using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using OpenSpace;
using OpenSpace.Visual;
using OpenSpace.Object;
using OpenSpace.AI;
using OpenSpace.Collide;
using System.Collections;
using SimpleJSON;
using OpenSpace.Object.Properties;
using System.Runtime.InteropServices;

public class WebCommunicator : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void SetAllJSON(string jsonString);

    public Controller controller;
    bool sentHierarchy = false;
    string allJSON = null;
    public void Start() {
    }

    public void Update() {
        if (controller.LoadState == Controller.State.Finished && !sentHierarchy) {
            SendHierarchy();
            sentHierarchy = true;
        }
    }

    public void SendHierarchy() {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            if (controller.LoadState == Controller.State.Finished) {
                allJSON = GetHierarchyJSON().ToString();
                SetAllJSON(allJSON);
            }
        }
        
    }

    private JSONObject GetHierarchyJSON() {
        MapLoader l = MapLoader.Loader;
        JSONObject allJSON = new JSONObject();
        if (l.families != null) {
            JSONArray families = new JSONArray();
            for (int i = 0; i < l.families.Count; i++) {
                families.Add(GetFamilyJSON(l.families[i]));
            }
            allJSON["families"] = families;
        }
        if (l.uncategorizedObjectLists != null) {
            JSONArray uncategorizedObjectLists = new JSONArray();
            for (int i = 0; i < l.uncategorizedObjectLists.Count; i++) {
                uncategorizedObjectLists.Add(l.uncategorizedObjectLists[i].ToString());
            }
            allJSON["uncategorizedObjectLists"] = uncategorizedObjectLists;
        }
        if (l.actualWorld != null) {
            allJSON["actualWorld"] = GetSuperObjectJSON(l.actualWorld);
        }
        if (l.transitDynamicWorld != null) {
            allJSON["transitDynamicWorld"] = GetSuperObjectJSON(l.transitDynamicWorld);
        }
        allJSON["settings"] = GetSettingsJSON();
        return allJSON;
    }
    private JSONObject GetSettingsJSON() {
        JSONObject settingsJSON = new JSONObject();
        settingsJSON["luminosity"] = controller.lightManager.luminosity;
        settingsJSON["saturate"] = controller.lightManager.saturate;
        settingsJSON["displayInactiveSectors"] = controller.sectorManager.displayInactiveSectors;
        return settingsJSON;
    }
    private JSONObject GetSuperObjectJSON(SuperObject so) {
        JSONObject soJSON = new JSONObject();
        soJSON["name"] = so.Gao.name;
        soJSON["type"] = so.type.ToString();
        soJSON["hash"] = so.offset.GetHashCode();
        soJSON["position"] = so.Gao.transform.localPosition;
        soJSON["rotation"] = so.Gao.transform.localEulerAngles;
        soJSON["scale"] = so.Gao.transform.localScale;
        if (so.type == SuperObject.Type.Perso) {
            soJSON["perso"] = GetPersoJSON((Perso)so.data);
        }
        JSONArray children = new JSONArray();
        for (int i = 0; i < so.children.Count; i++) {
            children.Add(GetSuperObjectJSON(so.children[i]));
        }
        soJSON["children"] = children;
        return soJSON;
    }
    private JSONObject GetFamilyJSON(Family f) {
        JSONObject familyJSON = new JSONObject();
        familyJSON["name"] = f.name;
        familyJSON["index"] = f.family_index;
        JSONArray states = new JSONArray();
        for (int i = 0; i < f.states.Count; i++) {
            states.Add(f.states[i].ToString());
        }
        familyJSON["states"] = states;
        JSONArray objectLists = new JSONArray();
        for (int i = 0; i < f.objectLists.Count; i++) {
            objectLists.Add(f.objectLists[i].ToString());
        }
        familyJSON["objectLists"] = objectLists;
        return familyJSON;
    }
    private JSONObject GetPersoJSON(Perso perso) {
        JSONObject persoJSON = new JSONObject();
        persoJSON["offset"] = perso.offset.ToString();
        persoJSON["nameFamily"] = perso.nameFamily;
        persoJSON["nameModel"] = perso.nameModel;
        persoJSON["nameInstance"] = perso.namePerso;
        if (perso.p3dData.family != null) persoJSON["family"] = perso.p3dData.family.family_index;
        if (perso.p3dData.stateCurrent != null && perso.p3dData.family != null) {
            persoJSON["state"] = perso.p3dData.family.states.IndexOf(perso.p3dData.stateCurrent);
        }
        if (perso.p3dData.objectList != null) persoJSON["objectList"] = perso.Gao.GetComponent<PersoBehaviour>().poListIndex;
        return persoJSON;
    }
    private JSONObject GetAlwaysJSON() {
        MapLoader l = MapLoader.Loader;
        JSONObject alwaysJSON = new JSONObject();
        JSONArray spawnables = new JSONArray();
        if (l.globals.spawnablePersos != null) {
            for (int i = 0; i < l.globals.spawnablePersos.Count; i++) {
                spawnables.Add(GetPersoJSON(l.globals.spawnablePersos[i]));
            }
        }
        alwaysJSON["spawnablePersos"] = spawnables;
        return alwaysJSON;
    }

    
    public void ParseMessage(string msgString) {
        JSONNode msg = JSON.Parse(msgString);
        if (msg["perso"] != null) {
            ParsePersoJSON(msg["perso"]);
        }
        if (msg["settings"] != null) {
            ParseSettingsJSON(msg["settings"]);
        }
    }
    public void ParsePersoJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        Perso perso = l.persos.Where(p => p.offset.ToString() == msg["offset"]).FirstOrDefault();
        if (perso != null) {
            PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
            if (msg["state"] != null) pb.SetState(msg["state"].AsInt);
            if (msg["objectList"] != null) pb.poListIndex = msg["objectList"].AsInt;
        }
    }
    public void ParseSettingsJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        if (msg["viewCollision"] != null) controller.viewCollision = msg["viewCollision"].AsBool;
        if (msg["luminosity"] != null) controller.lightManager.luminosity = msg["luminosity"].AsFloat;
        if (msg["saturate"] != null) controller.lightManager.saturate = msg["saturate"].AsBool;
        if (msg["viewGraphs"] != null) controller.viewGraphs = msg["viewGraphs"].AsBool;
        if (msg["enableLighting"] != null) controller.lightManager.enableLighting = msg["enableLighting"].AsBool;
        if (msg["viewInvisible"] != null) controller.viewInvisible = msg["viewInvisible"].AsBool;
        if (msg["displayInactive"] != null) controller.sectorManager.displayInactiveSectors = msg["displayInactive"].AsBool;
    }
}
