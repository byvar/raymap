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
    [DllImport("__Internal")]
    private static extern void UnityJSMessage(string jsonString);

    public Controller controller;
    public ObjectSelector selector;
    private PersoBehaviour highlightedPerso_;
    private PersoBehaviour selectedPerso_;
    bool sentHierarchy = false;
    string allJSON = null;
    public void Start() {
    }

    public void Update() {
        if (controller.LoadState == Controller.State.Finished && !sentHierarchy) {
            SendHierarchy();
            sentHierarchy = true;
        }
        if (controller.LoadState == Controller.State.Finished) {
            if (highlightedPerso_ != selector.highlightedPerso) {
                highlightedPerso_ = selector.highlightedPerso;
                Send(GetHighlightJSON());
            }
            if (selectedPerso_ != selector.selectedPerso) {
                selectedPerso_ = selector.selectedPerso;
                Send(GetSelectionJSON());
            }
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
    public void Send(JSONObject obj) {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            if (controller.LoadState == Controller.State.Finished) {
                UnityJSMessage(obj.ToString());
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
        soJSON["offset"] = so.offset.ToString();
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
        PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
        persoJSON["offset"] = perso.offset.ToString();
        persoJSON["nameFamily"] = perso.nameFamily;
        persoJSON["nameModel"] = perso.nameModel;
        persoJSON["nameInstance"] = perso.namePerso;
        if (perso.p3dData.family != null) persoJSON["family"] = perso.p3dData.family.family_index;
        if (pb != null) {
            persoJSON["state"] = pb.stateIndex;
            if (perso.p3dData.objectList != null) persoJSON["objectList"] = pb.poListIndex;
        }
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
    private JSONObject GetSelectionJSON() {
        MapLoader l = MapLoader.Loader;
        JSONObject selectionJSON = new JSONObject();
        selectionJSON["type"] = "selection";
        bool selectionIsAlways = l.globals.spawnablePersos.Contains(selector.selectedPerso.perso);
        selectionJSON["selectionType"] = selectionIsAlways ? "always" : "superobject";
        if (selectionIsAlways) {
            selectionJSON["selection"] = GetPersoJSON(selector.selectedPerso.perso);
        } else {
            selectionJSON["selection"] = GetSuperObjectJSON(selector.selectedPerso.perso.SuperObject);
        }
        return selectionJSON;
    }
    private JSONObject GetHighlightJSON() {
        MapLoader l = MapLoader.Loader;
        JSONObject selectionJSON = new JSONObject();
        selectionJSON["type"] = "highlight";
        if (highlightedPerso_ != null) {
            selectionJSON["perso"] = GetPersoJSON(highlightedPerso_.perso);
        }
        return selectionJSON;
    }

    public void ParseMessage(string msgString) {
        JSONNode msg = JSON.Parse(msgString);
        if (msg["perso"] != null) {
            ParsePersoJSON(msg["perso"]);
        }
        if (msg["settings"] != null) {
            ParseSettingsJSON(msg["settings"]);
        }
        if (msg["selection"] != null) {
            ParseSelectionJSON(msg["selection"]);
        }
    }
    public void ParseSelectionJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        Perso perso = null;
        if (msg["offset"] != null && msg["offset"] != "null") {
            perso = l.persos.Where(p => p.offset.ToString() == msg["offset"]).FirstOrDefault();
            if(perso != null) {
                PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
                selector.Select(pb, view: msg["view"] != null);
                //Send(GetSelectionJSON());
            }
        } else {
            selector.Deselect();
        }
    }
    public void ParsePersoJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        Perso perso = null;
        if (msg["offset"] != null) {
            perso = l.persos.Where(p => p.offset.ToString() == msg["offset"]).FirstOrDefault();
        }
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
