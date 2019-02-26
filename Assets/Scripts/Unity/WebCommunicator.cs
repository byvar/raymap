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
	private int selectedPersoStateIndex_;
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
				if (selectedPerso_ != null) {
					selectedPersoStateIndex_ = selectedPerso_.stateIndex;
					Send(GetSelectionJSON());
				}
            }
			if (selectedPerso_ != null && selectedPersoStateIndex_ != selectedPerso_.stateIndex) {
				selectedPersoStateIndex_ = selectedPerso_.stateIndex;
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
	public void SendSettings() {
		if (Application.platform == RuntimePlatform.WebGLPlayer && controller.LoadState == Controller.State.Finished) {
			Send(GetSettingsJSON());
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
		if (l.globals != null) {
			allJSON["always"] = GetAlwaysJSON();
		}
        allJSON["settings"] = GetSettingsJSON();
        return allJSON;
    }
    private JSONObject GetSettingsJSON() {
		JSONObject settingsJSONcontainer = new JSONObject();
        JSONObject settingsJSON = new JSONObject();
		settingsJSON["viewCollision"] = controller.viewCollision;
		settingsJSON["viewGraphs"] = controller.viewGraphs;
		settingsJSON["viewInvisible"] = controller.viewInvisible;
		settingsJSON["enableLighting"] = controller.lightManager.enableLighting;
		settingsJSON["enableFog"] = controller.lightManager.enableFog;
		settingsJSON["luminosity"] = controller.lightManager.luminosity;
        settingsJSON["saturate"] = controller.lightManager.saturate;
        settingsJSON["displayInactive"] = controller.sectorManager.displayInactiveSectors;
		settingsJSON["playAnimations"] = controller.playAnimations;
		settingsJSON["playTextureAnimations"] = controller.playTextureAnimations;
		settingsJSON["showPersos"] = controller.showPersos;
		settingsJSONcontainer["type"] = "settings";
		settingsJSONcontainer["settings"] = settingsJSON;
        return settingsJSONcontainer;
    }
    private JSONObject GetSuperObjectJSON(SuperObject so) {
        JSONObject soJSON = new JSONObject();
        if (so.Gao != null) {
            soJSON["name"] = so.Gao.name;
        }
        soJSON["type"] = so.type.ToString();
        soJSON["offset"] = so.offset.ToString();
        if (so.Gao != null) {
            soJSON["position"] = so.Gao.transform.localPosition;
            soJSON["rotation"] = so.Gao.transform.localEulerAngles;
            soJSON["scale"] = so.Gao.transform.localScale;
        }
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
        familyJSON["name"] = f.name != null ? f.name : "Family";
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
			persoJSON["enabled"] = pb.IsEnabled;
			persoJSON["state"] = pb.stateIndex;
            if (perso.p3dData.objectList != null) persoJSON["objectList"] = pb.poListIndex;
			persoJSON["playAnimation"] = pb.playAnimation;
			persoJSON["animationSpeed"] = pb.animationSpeed;
			persoJSON["autoNextState"] = pb.autoNextState;
        }
        return persoJSON;
    }
	private JSONObject GetBrainJSON(Perso perso, bool includeScriptContents = false) {
		JSONObject brainJSON = new JSONObject();
		PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
		if (perso.brain != null && perso.brain.mind != null && perso.brain.mind.AI_model != null) {
			if (perso.brain.mind.AI_model.behaviors_normal != null && perso.brain.mind.AI_model.behaviors_normal.Length > 0) {
				JSONArray ruleBehaviorsJSON = new JSONArray();
				Behavior[] ruleBehaviors = perso.brain.mind.AI_model.behaviors_normal;
				foreach (Behavior behavior in ruleBehaviors) {
					ruleBehaviorsJSON.Add(GetBehaviorJSON(perso, behavior, includeScriptContents));
				}
				brainJSON["ruleBehaviors"] = ruleBehaviorsJSON;
			}
			if (perso.brain.mind.AI_model.behaviors_reflex != null && perso.brain.mind.AI_model.behaviors_reflex.Length > 0) {
				JSONArray reflexBehaviorsJSON = new JSONArray();
				Behavior[] reflexBehaviors = perso.brain.mind.AI_model.behaviors_reflex;
				foreach (Behavior behavior in reflexBehaviors) {
					reflexBehaviorsJSON.Add(GetBehaviorJSON(perso, behavior, includeScriptContents));
				}
				brainJSON["reflexBehaviors"] = reflexBehaviorsJSON;
			}
			if (perso.brain.mind.AI_model.macros != null && perso.brain.mind.AI_model.macros.Length > 0) {
				JSONArray macrosJSON = new JSONArray();
				Macro[] macros = perso.brain.mind.AI_model.macros;
				foreach (Macro macro in macros) {
					macrosJSON.Add(GetBehaviorJSON(perso, macro, includeScriptContents));
				}
				brainJSON["macros"] = macrosJSON;
			}
		}
		JSONArray dsgVars = GetDsgVarsJSON(perso);
		if (dsgVars != null && dsgVars.Count > 0) {
			brainJSON["dsgVars"] = dsgVars;
		}

		return brainJSON;
	}
	private JSONArray GetDsgVarsJSON(Perso perso) {
		JSONArray dsgVarsJSON = new JSONArray();
		DsgVarComponent dsgComponent = perso.Gao.GetComponent<DsgVarComponent>();
		if (dsgComponent != null && dsgComponent.dsgVarEntries != null && dsgComponent.dsgVarEntries.Length > 0) {
			foreach (DsgVarComponent.DsgVarEditableEntry dsg in dsgComponent.editableEntries) {
				dsgVarsJSON.Add(GetDsgVarJSON(perso, dsg));
			}
		}
		return dsgVarsJSON;
	}

	private JSONObject GetDsgVarJSON(Perso perso, DsgVarComponent.DsgVarEditableEntry dsg) {
		JSONObject dsgObj = new JSONObject();
		dsgObj["name"] = dsg.entry.NiceVariableName;
		dsgObj["type"] = dsg.entry.type.ToString();
		switch (dsg.entry.type) {
			case DsgVarInfoEntry.DsgVarType.Boolean: dsgObj["value"] = dsg.valueAsBool; break;
			case DsgVarInfoEntry.DsgVarType.Int: dsgObj["value"] = dsg.valueAsInt; break;
			case DsgVarInfoEntry.DsgVarType.UInt: dsgObj["value"] = dsg.valueAsUInt; break;
			case DsgVarInfoEntry.DsgVarType.Short: dsgObj["value"] = dsg.valueAsShort; break;
			case DsgVarInfoEntry.DsgVarType.UShort: dsgObj["value"] = dsg.valueAsUShort; break;
			case DsgVarInfoEntry.DsgVarType.Byte: dsgObj["value"] = dsg.valueAsByte; break;
			case DsgVarInfoEntry.DsgVarType.UByte: dsgObj["value"] = dsg.valueAsByte; break;
			case DsgVarInfoEntry.DsgVarType.Float: dsgObj["value"] = dsg.valueAsFloat; break;
			case DsgVarInfoEntry.DsgVarType.Text: dsgObj["value"] = dsg.valueAsString; break;
			case DsgVarInfoEntry.DsgVarType.Vector: dsgObj["value"] = dsg.valueAsVector; break;
			case DsgVarInfoEntry.DsgVarType.Perso:
				if (dsg.entry.value != null) {
					SuperObject so = SuperObject.FromOffset(dsg.entry.value as Pointer);
					if (so != null && so.type == SuperObject.Type.Perso && so.data != null) {
						Perso p = so.data as Perso;
						if (p != null) {
							JSONObject persoJSON = new JSONObject();
							persoJSON["offset"] = p.offset.ToString();
							persoJSON["nameFamily"] = p.nameFamily;
							persoJSON["nameModel"] = p.nameModel;
							persoJSON["nameInstance"] = p.namePerso;
							dsgObj["value"] = persoJSON;
						}
					}
				}
				break;
			case DsgVarInfoEntry.DsgVarType.SuperObject:
				SuperObject spo = SuperObject.FromOffset(dsg.entry.value as Pointer);
				if (spo != null) {
					JSONObject soJSON = new JSONObject();
					soJSON["offset"] = spo.offset.ToString();
					soJSON["type"] = spo.type.ToString();
					soJSON["offset"] = spo.offset.ToString();
					if (spo.type == SuperObject.Type.Perso && spo.data != null) {
						Perso p = spo.data as Perso;
						if (p != null) {
							JSONObject persoJSON = new JSONObject();
							persoJSON["offset"] = p.offset.ToString();
							persoJSON["nameFamily"] = p.nameFamily;
							persoJSON["nameModel"] = p.nameModel;
							persoJSON["nameInstance"] = p.namePerso;
							soJSON["perso"] = persoJSON;
						}
					}
					dsgObj["value"] = soJSON;
				}
				break;
		}
		return dsgObj;
	}
	private JSONObject GetBehaviorJSON(Perso perso, BehaviorOrMacro behavior, bool includeScriptContents) {
		JSONObject behaviorJSON = new JSONObject();
		string name;
		if (behavior is Macro) {
			Macro m = behavior as Macro;
			name = m.ShortName;
			if (m.script == null) name += " (null)";
			if (m.script != null) {
				behaviorJSON["script"] = GetScriptJSON(perso, m.script, includeScriptContents);
			}
			behaviorJSON["type"] = "Macro";
		} else {
			Behavior b = behavior as Behavior;
			name = b.ShortName;
			JSONArray scripts = new JSONArray();
			foreach (Script script in b.scripts) {
				if (script != null) {
					scripts.Add(GetScriptJSON(perso, script, includeScriptContents));
				}
			}
			behaviorJSON["scripts"] = scripts;
			behaviorJSON["type"] = b.type.ToString();
			if (b.firstScript != null) {
				behaviorJSON["firstScript"] = GetScriptJSON(perso, b.firstScript, includeScriptContents);
			}
		}

		behaviorJSON["name"] = name;
		return behaviorJSON;
	}
	private JSONObject GetScriptJSON(Perso perso, Script script, bool includeScriptContents) {
		JSONObject scriptJSON = new JSONObject();
		scriptJSON["offset"] = script.offset.ToString();
		scriptJSON["type"] = "script";

		if (includeScriptContents) {
			TranslatedScript ts = new TranslatedScript(script, perso);
			scriptJSON["translation"] = ts.ToString();
		}
		return scriptJSON;
	}
    private JSONObject GetAlwaysJSON() {
        MapLoader l = MapLoader.Loader;
        JSONObject alwaysJSON = new JSONObject();
        JSONArray spawnables = new JSONArray();
        if (l.globals.spawnablePersos != null) {
            for (int i = 0; i < l.globals.spawnablePersos.Count; i++) {
				JSONObject alwaysPersoJSON = GetPersoJSON(l.globals.spawnablePersos[i]);
				alwaysPersoJSON["name"] = l.globals.spawnablePersos[i].Gao.name;
				alwaysPersoJSON["type"] = "Always";
				alwaysPersoJSON["position"] = l.globals.spawnablePersos[i].Gao.transform.localPosition;
				alwaysPersoJSON["rotation"] = l.globals.spawnablePersos[i].Gao.transform.localEulerAngles;
				alwaysPersoJSON["scale"] = l.globals.spawnablePersos[i].Gao.transform.localScale;
				spawnables.Add(alwaysPersoJSON);
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
			selectionJSON["selection"]["brain"] = GetBrainJSON(selector.selectedPerso.perso);
			selectionJSON["selection"]["name"] = selector.selectedPerso.name;
			selectionJSON["selection"]["type"] = "Always";
			selectionJSON["selection"]["position"] = selector.selectedPerso.transform.localPosition;
			selectionJSON["selection"]["rotation"] = selector.selectedPerso.transform.localEulerAngles;
			selectionJSON["selection"]["scale"] = selector.selectedPerso.transform.localScale;
		} else {
            selectionJSON["selection"] = GetSuperObjectJSON(selector.selectedPerso.perso.SuperObject);
			selectionJSON["selection"]["perso"]["brain"] = GetBrainJSON(selector.selectedPerso.perso);
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
        if (msg["superobject"] != null) {
            ParseSuperObjectJSON(msg["superobject"]);
		}
		if (msg["perso"] != null) {
			ParsePersoJSON(msg["perso"]);
		}
		if (msg["settings"] != null) {
            ParseSettingsJSON(msg["settings"]);
        }
        if (msg["selection"] != null) {
            ParseSelectionJSON(msg["selection"]);
        }
		if (msg["request"] != null) {
			ParseRequestJSON(msg["request"]);
		}
    }
    private void ParseSelectionJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        Perso perso = null;
		SuperObject so = null;
        if (msg["offset"] != null && msg["offset"] != "null") {
			if (msg["type"] != null && msg["type"] == SuperObject.Type.Perso.ToString()) {
				perso = l.persos.FirstOrDefault(p => p.offset.ToString() == msg["offset"]);
				if (perso != null) {
					PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
					selector.Select(pb, view: msg["view"] != null);
				}
			} else {
				so = l.superObjects.FirstOrDefault(s => s.offset.ToString() == msg["offset"]);
				if (so != null) {
					selector.Select(so);
				}
			}
        } else {
            selector.Deselect();
        }
	}
	private void ParseSuperObjectJSON(JSONNode msg) {
		MapLoader l = MapLoader.Loader;
		SuperObject so = null;
		if (msg["type"] != null && msg["type"] == "Always") {
			// Fake superobject
			Perso alwaysPerso = l.globals.spawnablePersos.FirstOrDefault(s => s.offset.ToString() == msg["offset"]);
			if (alwaysPerso != null) {
				if (msg["position"] != null) alwaysPerso.Gao.transform.localPosition = msg["position"].ReadVector3();
				if (msg["rotation"] != null) alwaysPerso.Gao.transform.localEulerAngles = msg["rotation"].ReadVector3();
				if (msg["scale"] != null) alwaysPerso.Gao.transform.localScale = msg["scale"].ReadVector3();
			}
		} else {
			if (msg["offset"] != null) {
				so = l.superObjects.FirstOrDefault(s => s.offset.ToString() == msg["offset"]);
			}
			if (so != null) {
				if (msg["position"] != null) so.Gao.transform.localPosition = msg["position"].ReadVector3();
				if (msg["rotation"] != null) so.Gao.transform.localEulerAngles = msg["rotation"].ReadVector3();
				if (msg["scale"] != null) so.Gao.transform.localScale = msg["scale"].ReadVector3();
			}
		}
	}
	private void ParsePersoJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        Perso perso = null;
        if (msg["offset"] != null) {
			perso = l.persos.FirstOrDefault(p => p.offset.ToString() == msg["offset"]);
        }
        if (perso != null) {
            PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
			if (msg["enabled"] != null) pb.IsEnabled = (msg["enabled"].AsBool);
			if (msg["state"] != null) {
				pb.SetState(msg["state"].AsInt);
				if (pb == selectedPerso_) {
					selectedPersoStateIndex_ = msg["state"].AsInt;
				}
			}
            if (msg["objectList"] != null) pb.poListIndex = msg["objectList"].AsInt;
			if (msg["playAnimation"] != null) pb.playAnimation = msg["playAnimation"].AsBool;
			if (msg["animationSpeed"] != null) pb.animationSpeed = msg["animationSpeed"].AsFloat;
			if (msg["autoNextState"] != null) pb.autoNextState = msg["autoNextState"].AsBool;
        }
    }
    private void ParseSettingsJSON(JSONNode msg) {
        MapLoader l = MapLoader.Loader;
        if (msg["viewCollision"] != null) controller.viewCollision = msg["viewCollision"].AsBool;
        if (msg["luminosity"] != null) controller.lightManager.luminosity = msg["luminosity"].AsFloat;
        if (msg["saturate"] != null) controller.lightManager.saturate = msg["saturate"].AsBool;
        if (msg["viewGraphs"] != null) controller.viewGraphs = msg["viewGraphs"].AsBool;
        if (msg["enableLighting"] != null) controller.lightManager.enableLighting = msg["enableLighting"].AsBool;
		if (msg["enableFog"] != null) controller.lightManager.enableFog = msg["enableFog"].AsBool;
		if (msg["viewInvisible"] != null) controller.viewInvisible = msg["viewInvisible"].AsBool;
        if (msg["displayInactive"] != null) controller.sectorManager.displayInactiveSectors = msg["displayInactive"].AsBool;
		if (msg["playAnimations"] != null) controller.playAnimations = msg["playAnimations"].AsBool;
		if (msg["playTextureAnimations"] != null) controller.playTextureAnimations = msg["playTextureAnimations"].AsBool;
		if (msg["showPersos"] != null) controller.showPersos = msg["showPersos"].AsBool;
	}
	private void ParseRequestJSON(JSONNode msg) {
		if (msg["type"] != null) {
			switch ((string)msg["type"]) {
				case "script":
					Script s = GetScriptFromRequest(msg);
					if (s != null) Send(GetScriptJSON(selector.selectedPerso.perso, s, true));
					break;
			}
		}
	}
	private Script GetScriptFromRequest(JSONNode msg) {
		if (selector.selectedPerso == null
			|| selector.selectedPerso.perso.brain == null
			|| selector.selectedPerso.perso.brain.mind == null
			|| selector.selectedPerso.perso.brain.mind.AI_model == null) return null;
		AIModel ai = selector.selectedPerso.perso.brain.mind.AI_model;
		string offset = msg["scriptOffset"];
		switch ((string)msg["behaviorType"]) {
			case "Rule":
				if (ai.behaviors_normal == null) return null;
				foreach (Behavior be in ai.behaviors_normal) {
					if (be.firstScript != null && be.firstScript.offset.ToString() == offset) return be.firstScript;
					foreach (Script s in be.scripts) {
						if (s.offset.ToString() == offset) return s;
					}
				}
				break;
			case "Reflex":
				if (ai.behaviors_reflex == null) return null;
				foreach (Behavior be in ai.behaviors_reflex) {
					if (be.firstScript != null && be.firstScript.offset.ToString() == offset) return be.firstScript;
					foreach (Script s in be.scripts) {
						if (s.offset.ToString() == offset) return s;
					}
				}
				break;
			case "Macro":
				if (ai.macros == null) return null;
				foreach (Macro m in ai.macros) {
					if (m.script != null && m.script.offset.ToString() == offset) return m.script;
				}
				break;
		}
		return null;
	}
}
