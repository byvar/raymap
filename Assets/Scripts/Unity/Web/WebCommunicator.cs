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
using OpenSpace.Object.Properties;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

public class WebCommunicator : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void SetAllJSON(string jsonString);
    [DllImport("__Internal")]
    private static extern void UnityJSMessage(string jsonString);
	[DllImport("__Internal")]
	private static extern void SaveFile(byte[] array, int size, string filename);

	public Controller controller;
    public ObjectSelector selector;
	public MeshCollider preventMeshColliderStrip;
    private BasePersoBehaviour highlightedPerso_;
	private CollideComponent highlightedCollision_;
	private WayPointBehaviour highlightedWayPoint_;
    private BasePersoBehaviour selectedPerso_;
	private int selectedPersoStateIndex_;
	private int cinematicIndex_;
    bool sentHierarchy = false;
    string allJSON = null;

	private Newtonsoft.Json.JsonSerializerSettings _settings;
	public Newtonsoft.Json.JsonSerializerSettings Settings {
		get {
			if (_settings == null) {
				_settings = new Newtonsoft.Json.JsonSerializerSettings() {
					Formatting = Newtonsoft.Json.Formatting.None,
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
					
				};
				_settings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector3Converter());
				_settings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector2Converter());
				_settings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector4Converter());
				_settings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.QuaternionConverter());
				_settings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.ColorConverter());
			}
			return _settings;
		}
	}

	public void Start() {
    }

    public void Update() {
        if (controller.LoadState == Controller.State.Finished && !sentHierarchy) {
            SendHierarchy();
            sentHierarchy = true;
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer && controller.LoadState == Controller.State.Finished) {
            if (highlightedPerso_ != selector.highlightedPerso ||
				highlightedCollision_ != selector.highlightedCollision ||
				highlightedWayPoint_ != selector.highlightedWayPoint) {
                highlightedPerso_ = selector.highlightedPerso;
				highlightedCollision_ = selector.highlightedCollision;
				highlightedWayPoint_ = selector.highlightedWayPoint;
                Send(GetHighlightMessageJSON());
            }
            if (selectedPerso_ != selector.selectedPerso) {
                selectedPerso_ = selector.selectedPerso;
				if (selectedPerso_ != null) {
					selectedPersoStateIndex_ = selectedPerso_.currentState;
					Send(GetSelectionMessageJSON(true, true));
				}
            }
			if (selectedPerso_ != null && selectedPersoStateIndex_ != selectedPerso_.currentState) {
				selectedPersoStateIndex_ = selectedPerso_.currentState;
				Send(GetSelectionMessageJSON(false, false));
			}
			if (controller.CinematicSwitcher != null && cinematicIndex_ != controller.CinematicSwitcher.currentCinematic) {
				cinematicIndex_ = controller.CinematicSwitcher.currentCinematic;
				Send(GetCineDataMessageJSON());
			}
        }
    }

    public void SendHierarchy() {
        if (Application.platform == RuntimePlatform.WebGLPlayer && controller.LoadState == Controller.State.Finished) {
			allJSON = SerializeMessage(GetHierarchyMessageJSON());
            SetAllJSON(allJSON);
        }
    }
	public void SendSettings() {
		if (Application.platform == RuntimePlatform.WebGLPlayer && controller.LoadState == Controller.State.Finished) {
			Send(GetSettingsMessageJSON());
		}
	}
	public void SendChangedCameraMode(WebJSON.CameraPos cameraPos) {
		if (Application.platform == RuntimePlatform.WebGLPlayer && controller.LoadState == Controller.State.Finished) {
			Send(new WebJSON.Message() {
				Type = WebJSON.MessageType.Camera,
				Camera = new WebJSON.CameraSettings() { CameraPos = cameraPos }
			});
		}
	}
    public void Send(WebJSON.Message obj) {
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
            if (controller.LoadState == Controller.State.Finished) {
				string json = SerializeMessage(obj);
                UnityJSMessage(json);
            }
        }
    }
	public string SerializeMessage(WebJSON.Message obj) {
		string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Settings);
		return json;
	}
	public WebJSON.CameraSettings GetCameraJSON(bool includeTransform = false) {
		Camera c = Camera.main;
		return new WebJSON.CameraSettings() {
			ClipFar = c.farClipPlane,
			ClipNear = c.nearClipPlane,
			FieldOfView = c.fieldOfView,
			IsOrthographic = c.orthographic,
			OrthographicSize = c.orthographicSize,
			Position = includeTransform ? (Vector3?)c.transform.localPosition : null,
			Rotation = includeTransform ? (Vector3?)c.transform.localEulerAngles : null
		};
	}

	private WebJSON.Message GetHierarchyMessageJSON() {
        MapLoader l = MapLoader.Loader;
		WebJSON.Message message = new WebJSON.Message() {
			Type = WebJSON.MessageType.Hierarchy,
			Settings = GetSettingsJSON(),
			Localization = GetLocalizationJSON(),
			Input = GetInputJSON(),
			CineData = GetCineDataJSON(),
			Hierarchy = new WebJSON.Hierarchy() {
				Always = GetAlwaysJSON()
			},
			Camera = GetCameraJSON(),
			GameSettings = GetGameSettingsJSON()
		};
		switch (l) {
			case OpenSpace.Loader.R2ROMLoader rl:
				message.Hierarchy.DynamicSuperObjects = controller.romPersos.Where(p => !p.IsAlways)
					.Select(p => GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.soROMDynamic == p.superObject))).ToArray();
				message.Hierarchy.FatherSector = GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.Offset == rl.level?.hierarchyRoot?.Value?.fatherSector?.Value?.Offset));
				break;
			case OpenSpace.Loader.R2PS1Loader pl:
				message.Hierarchy.DynamicWorld = GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.Offset == pl.levelHeader?.dynamicWorld.Offset));
				message.Hierarchy.FatherSector = GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.Offset == pl.levelHeader?.fatherSector.Offset));
				break;
			default:
				message.Hierarchy.ActualWorld = GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.Offset == l.actualWorld.offset));
				if (l.transitDynamicWorld != null) {
					message.Hierarchy.TransitDynamicWorld = GetSuperObjectJSON(controller.superObjects.FirstOrDefault(s => s.Offset == l.transitDynamicWorld.offset));
				}
				break;
		}
		return message;
    }
	public WebJSON.GameSettings GetGameSettingsJSON() {
		MapLoader l = controller.loader;
		WebJSON.EngineMode mode = WebJSON.EngineMode.Main;
		if (l is OpenSpace.Loader.R2PS1Loader) {
			mode = WebJSON.EngineMode.PS1;
		} else if (l is OpenSpace.Loader.R2ROMLoader) {
			mode = WebJSON.EngineMode.ROM;
		}
		return new WebJSON.GameSettings() {
			EngineMode = mode,
			EngineVersion = OpenSpace.Settings.s.engineVersion,
			Game = OpenSpace.Settings.s.game,
			Mode = OpenSpace.Settings.s.mode
		};
	}
	public WebJSON.Message GetCineDataMessageJSON() {
		return new WebJSON.Message() {
			Type = WebJSON.MessageType.CineData,
			CineData = GetCineDataJSON(),
		};
	}
	public WebJSON.CineData GetCineDataJSON() {
		if (controller.CinematicSwitcher != null && controller.CinematicSwitcher.CinematicNames.Length > 1) {
			return new WebJSON.CineData() {
				CinematicNames = controller.CinematicSwitcher.CinematicNames,
				CinematicIndex = controller.CinematicSwitcher.CinematicIndex,
				Actors = controller.CinematicSwitcher.actors == null || controller.CinematicSwitcher.actors.Length == 0 ? null : controller.CinematicSwitcher.actors.Select(a => GetPersoJSON(a, includeDetails: false)).ToArray(),
				AnimationSpeed = OpenSpace.Settings.s.platform == OpenSpace.Settings.Platform.PS1 ? (float?)controller.CinematicSwitcher.animationSpeed : null
			};
		}
		return null;
	}
	public WebJSON.Localization GetLocalizationJSON() {
		MapLoader l = MapLoader.Loader;
		if (l is OpenSpace.Loader.R2ROMLoader) {
			OpenSpace.Loader.R2ROMLoader rl = l as OpenSpace.Loader.R2ROMLoader;
			OpenSpace.ROM.Localization rloc = rl.localizationROM;
			if (rloc != null) {
				int commonStart = rloc.languageTables[1].num_txtTable + rloc.languageTables[1].num_binaryTable;
				WebJSON.Localization.Language GetLanguage(int langIndex) {
					OpenSpace.ROM.LanguageTable lang = rloc.languageTables[langIndex];
					string[] entries = new string[lang.num_txtTable + lang.num_binaryTable];
					if (langIndex == 0) {
						for (int i = 0; i < entries.Length; i++) {
							entries[i] = rloc.Lookup(commonStart + i);
						}
					} else {
						for (int i = 0; i < entries.Length; i++) {
							entries[i] = rloc.Lookup(i, languageIndex: langIndex);
						}
					}
					return new WebJSON.Localization.Language() {
						Name = lang.name.Split(':')[0],
						NameLocalized = lang.name.Split(':')[1],
						Entries = entries
					};
				}
				return new WebJSON.Localization() {
					LanguageStart = 0,
					CommonStart = commonStart,
					Common = GetLanguage(0),
					Languages = Enumerable.Range(1, rloc.languageTables.Length-1).Select(ind => GetLanguage(ind)).ToArray()
				};
			}
		} else if (l is OpenSpace.Loader.R2PS1Loader) {
		} else {
			OpenSpace.Text.LocalizationStructure loc = l.localization;
			if (loc != null) {
				return new WebJSON.Localization() {
					LanguageStart = 0,
					CommonStart = 20000,
					Common = new WebJSON.Localization.Language() {
						Name = "Common",
						NameLocalized = "Common",
						Entries = loc.misc.entries
					},
					Languages = Enumerable.Range(0, loc.num_languages).Select(ind => new WebJSON.Localization.Language() {
						Name = l.languages?[ind] ?? ("Language " + ind),
						NameLocalized = l.languages_loc?[ind] ?? ("Language " + ind),
						Entries = loc.languages[ind].entries
					}).ToArray()
				};
			}
		}
		return null;
	}
	public WebJSON.InputStruct GetInputJSON() {
		switch (MapLoader.Loader) {
			case OpenSpace.Loader.R2ROMLoader roml:
				OpenSpace.ROM.EngineStruct es = roml.Get<OpenSpace.ROM.EngineStruct>(0 | (ushort)OpenSpace.ROM.FATEntry.Flag.Fix);
				OpenSpace.ROM.EntryActionArray eaa = es?.inputStruct?.Value?.entryActions?.Value;
				if (eaa != null && eaa.length > 0) {
					return new WebJSON.InputStruct() {
						EntryActions = eaa.entryActions.Select(ea => ea?.Value != null ? new WebJSON.EntryAction() {
							Name = ea?.Value.GetNameString(),
							Input = ea?.Value.GetValueOnlyString()
						} : null).ToArray()
					};
				}
				break;
			case OpenSpace.Loader.R2PS1Loader ps1l:
				return null;
			default:
				MapLoader l = MapLoader.Loader;
				if (l.entryActions?.Count > 0) {
					return new WebJSON.InputStruct() {
						EntryActions = l.entryActions.Select(ea => ea != null ? new WebJSON.EntryAction() {
							Name = ea.ExportName,
							Input = ea?.GetValueOnlyString()
						} : null).ToArray()
					};
				}
				break;
		}
		return null;
	}
	private WebJSON.Message GetSettingsMessageJSON() {
		WebJSON.Message message = new WebJSON.Message() {
			Type = WebJSON.MessageType.Settings,
			Settings = GetSettingsJSON()
		};
		return message;
    }

	private WebJSON.Settings GetSettingsJSON() {
		return new WebJSON.Settings() {
			ViewCollision = controller.viewCollision,
			ViewGraphs = controller.viewGraphs,
			ViewInvisible = controller.viewInvisible,
			EnableLighting = controller.lightManager.enableLighting,
			EnableFog = controller.lightManager.enableFog,
			Luminosity = controller.lightManager.luminosity,
			Saturate = controller.lightManager.saturate,
			DisplayInactive = controller.sectorManager.displayInactiveSectors,
			PlayAnimations = controller.playAnimations,
			PlayTextureAnimations = controller.playTextureAnimations,
			ShowPersos = controller.showPersos,
		};
	}
    private WebJSON.SuperObject GetSuperObjectJSON(SuperObjectComponent so, bool includeChildren = true) {
		if (so == null) return null;
		WebJSON.SuperObject soJSON = new WebJSON.SuperObject() {
			Type = so.Type,
			Offset = so.Offset
		};
		soJSON.Name = so.gameObject.name;
		soJSON.Position = so.gameObject.transform.localPosition;
		soJSON.Rotation = so.gameObject.transform.localEulerAngles;
		soJSON.Scale = so.gameObject.transform.localScale;

        if (soJSON.Type == SuperObject.Type.Perso) {
			BasePersoBehaviour pb = GetPersoFromSuperObjectOffset(soJSON.Offset);
			if (pb != null) soJSON.Perso = GetPersoJSON(pb);
        }
		if (includeChildren) {
			soJSON.Children = so.Children.Select(s => GetSuperObjectJSON(s, includeChildren: includeChildren)).ToArray();
		}
        return soJSON;
	}
	private WebJSON.Perso GetPersoJSON(BasePersoBehaviour pb, bool includeDetails = true, bool includeLists = false, bool includeBrain = false) {
		if (pb == null) return null;
		WebJSON.Perso persoJSON = new WebJSON.Perso();
		persoJSON.Offset = pb.Offset;
		persoJSON.NameFamily = pb.NameFamily;
		persoJSON.NameModel = pb.NameModel;
		persoJSON.NameInstance = pb.NameInstance;
		if (includeDetails) {
			persoJSON.IsEnabled = pb.IsEnabled;
			persoJSON.State = pb.currentState;
			persoJSON.ObjectList = pb.poListIndex;
			persoJSON.PlayAnimation = pb.playAnimation;
			persoJSON.AnimationSpeed = pb.animationSpeed;
			persoJSON.AutoNextState = pb.autoNextState;
		}
		persoJSON.Type = pb.IsAlways ? WebJSON.PersoType.Always : WebJSON.PersoType.Instance;

		persoJSON.Name = pb.gameObject.name;
		persoJSON.Position = pb.transform.localPosition;
		persoJSON.Rotation = pb.transform.localEulerAngles;
		persoJSON.Scale = pb.transform.localScale;

		if (includeLists) {
			if (pb.stateNames != null) {
				persoJSON.States = Enumerable.Range(0, pb.stateNames.Length).Select(i => new WebJSON.State() {
					Name = pb.stateNames[i],
					Transitions = pb.GetStateTransitions(i)?.Select(t => new WebJSON.State.Transition() {
						LinkingType = t.LinkingType,
						StateToGo = t.StateToGoIndex,
						TargetState = t.TargetStateIndex
					}).ToArray(),
				}).ToArray();
			}
			persoJSON.ObjectLists = pb.poListNames;
		}
		if (includeBrain) {
			persoJSON.Brain = GetBrainJSON(selector.selectedPerso);
			persoJSON.BehaviorTransitionExportAvailable =
				selector.selectedPerso is PersoBehaviour
				&& (persoJSON.Brain?.Intelligence?.Length > 0 || persoJSON.Brain?.Reflex?.Length > 0 || persoJSON.Brain?.Macros?.Length > 0);
		}
		return persoJSON;
	}
	private WebJSON.Brain GetBrainJSON(BasePersoBehaviour perso, bool includeScriptContents = false) {
		WebJSON.Brain brainJSON = new WebJSON.Brain();
		if (perso.brain != null) {
			brainJSON.Intelligence = perso.brain.Intelligence.Select(i => GetComportJSON(i, includeScriptContents: includeScriptContents)).ToArray();
			brainJSON.Reflex = perso.brain.Reflex.Select(i => GetComportJSON(i, includeScriptContents: includeScriptContents)).ToArray();
			brainJSON.Macros = perso.brain.Macros.Select(i => GetMacroJSON(i, includeScriptContents: includeScriptContents)).ToArray();
			brainJSON.DsgVars = GetDsgVarsJSON(perso);
		}
		
		return brainJSON;
	}
	private WebJSON.Comport GetComportJSON(BrainComponent.Comport c, bool includeScriptContents = false) {
		return new WebJSON.Comport() {
			Offset = c.Offset,
			Name = c.Name,
			Scripts = c.Scripts.Select(s => GetScriptJSON(s, includeScriptContents: includeScriptContents)).ToArray(),
			FirstScript = GetScriptJSON(c.FirstScript, includeScriptContents: includeScriptContents)
		};
	}
	private WebJSON.Macro GetMacroJSON(BrainComponent.Macro m, bool includeScriptContents = false) {
		return new WebJSON.Macro() {
			Offset = m.Offset,
			Name = m.Name,
			Script = GetScriptJSON(m.Script, includeScriptContents: includeScriptContents)
		};
	}
	private WebJSON.DsgVar[] GetDsgVarsJSON(BasePersoBehaviour perso) {
		DsgVarComponent dsgComponent = perso?.brain?.dsgVars;
		if (dsgComponent != null && dsgComponent.editableEntries != null && dsgComponent.editableEntries.Length > 0) {
			return dsgComponent.editableEntries.Select(e => GetDsgVarJSON(e)).ToArray();
		}
		return null;
	}

	private WebJSON.DsgVar GetDsgVarJSON(DsgVarComponent.DsgVarEditableEntry dsg) {
		bool isArray = dsg.IsArray;
		WebJSON.DsgVar dsgObj = new WebJSON.DsgVar() {
			Name = dsg.Name,
			Type = dsg.Type,
			IsArray = isArray,
			ArrayType = isArray ? (DsgVarInfoEntry.DsgVarType?)DsgVarInfoEntry.GetDsgVarTypeFromArrayType(dsg.Type) : null,
			ArrayLength = isArray ? (int?)dsg.ArrayLength : null,
			ValueCurrent = GetDsgVarValueJSON(dsg.valueCurrent, isArray),
			ValueInitial = GetDsgVarValueJSON(dsg.valueInitial, isArray),
			ValueModel = GetDsgVarValueJSON(dsg.valueModel, isArray)
		};
		return dsgObj;
	}
	private WebJSON.DsgVarValue GetDsgVarValueJSON(DsgVarComponent.DsgVarEditableEntry.Value value, bool isArray) {
		if (value == null) return null;
		WebJSON.DsgVarValue dsgObj = new WebJSON.DsgVarValue() {
			Type = value.type
		};
		if (isArray) {
			dsgObj.AsArray = value.AsArray.Select(a => GetDsgVarValueJSON(a, false)).ToArray();
		} else {
			switch (value.type) {
				case DsgVarInfoEntry.DsgVarType.Boolean: dsgObj.AsBoolean = value.AsBoolean; break;
				case DsgVarInfoEntry.DsgVarType.Byte: dsgObj.AsByte = value.AsByte; break;
				case DsgVarInfoEntry.DsgVarType.UByte: dsgObj.AsUByte = value.AsUByte; break;
				case DsgVarInfoEntry.DsgVarType.Short: dsgObj.AsShort = value.AsShort; break;
				case DsgVarInfoEntry.DsgVarType.UShort: dsgObj.AsUShort = value.AsUShort; break;
				case DsgVarInfoEntry.DsgVarType.Int: dsgObj.AsInt = value.AsInt; break;
				case DsgVarInfoEntry.DsgVarType.UInt: dsgObj.AsUInt = value.AsUInt; break;
				case DsgVarInfoEntry.DsgVarType.Float: dsgObj.AsFloat = value.AsFloat; break;
				case DsgVarInfoEntry.DsgVarType.Caps: dsgObj.AsCaps = value.AsCaps; break;
				case DsgVarInfoEntry.DsgVarType.Text: dsgObj.AsText = value.AsText; break;
				case DsgVarInfoEntry.DsgVarType.Vector: dsgObj.AsVector = value.AsVector; break;
				case DsgVarInfoEntry.DsgVarType.Perso:
					if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
						dsgObj.AsPerso = GetPersoJSON(value.AsPersoROM, includeDetails: false);
					} else {
						dsgObj.AsPerso = GetPersoJSON(value.AsPerso, includeDetails: false);
					}
					break;
				case DsgVarInfoEntry.DsgVarType.SuperObject:
					dsgObj.AsSuperObject = GetSuperObjectJSON(value.AsSuperObject, includeChildren: false);
					break;
				case DsgVarInfoEntry.DsgVarType.WayPoint:
					dsgObj.AsWayPoint = GetWayPointJSON(value.AsWayPoint, false);
					break;
				case DsgVarInfoEntry.DsgVarType.Graph:
					dsgObj.AsGraph = GetGraphJSON(value.AsGraph);
					break;
				case DsgVarInfoEntry.DsgVarType.Action:
					dsgObj.AsAction = new WebJSON.DsgState() { Name = value.AsAction?.ToString() };
					break;
			}
		}
		return dsgObj;
	}
	private WebJSON.Script GetScriptJSON(BaseScriptComponent script, bool includeScriptContents) {
		if (script == null) return null;
		WebJSON.Script scriptJSON = new WebJSON.Script();
		scriptJSON.Offset = script.Offset;

		if (includeScriptContents) {
			scriptJSON.Translation = script.TranslatedScript;
		}
		return scriptJSON;
	}
    private WebJSON.Always GetAlwaysJSON() {
        MapLoader l = MapLoader.Loader;
		WebJSON.Always alwaysJSON = new WebJSON.Always();
		switch (l) {
			case OpenSpace.Loader.R2ROMLoader roml:
				if (roml.level?.spawnablePersos?.Value != null && roml.level?.spawnablePersos?.Value.superObjects.Length > 0) {
					alwaysJSON.SpawnablePersos = controller.romPersos.Where(p => p.IsAlways).Select(p => GetPersoJSON(p)).ToArray();
				}
				break;
			case OpenSpace.Loader.R2PS1Loader ps1l:
				if (ps1l.levelHeader?.always != null && ps1l.levelHeader.always.Length > 0) {
					alwaysJSON.SpawnablePersos = controller.ps1Persos.Where(p => p.IsAlways).Select(p => GetPersoJSON(p)).ToArray();
				}
				break;
			default:
				if (l.globals.spawnablePersos != null) {
					alwaysJSON.SpawnablePersos = l.globals.spawnablePersos.Select(p => GetPersoJSON(p?.Gao?.GetComponent<BasePersoBehaviour>())).ToArray();
				}
				break;
		}
        return alwaysJSON;
    }
    private WebJSON.Message GetSelectionMessageJSON(bool includeLists, bool includeBrain) {
        MapLoader l = MapLoader.Loader;
		WebJSON.Message selectionJSON = new WebJSON.Message() {
			Type = WebJSON.MessageType.Selection,
			Selection = new WebJSON.Selection() {
				Perso = GetPersoJSON(selector.selectedPerso, includeLists: includeLists, includeBrain: includeBrain)
			}
		};
        return selectionJSON;
    }
    private WebJSON.Message GetHighlightMessageJSON() {
		WebJSON.Message selectionJSON = new WebJSON.Message() {
			Type = WebJSON.MessageType.Highlight,
			Highlight = new WebJSON.Highlight() {
				Perso = GetPersoJSON(highlightedPerso_),
				WayPoint = GetWayPointJSON(highlightedWayPoint_, true),
				Collider = GetColliderJSON(highlightedCollision_)
			}
		};
        return selectionJSON;
    }

	private WebJSON.WayPoint GetWayPointJSON(WayPointBehaviour wp, bool includeGraphs) {
		if (wp == null) return null;
		return new WebJSON.WayPoint() {
			Name = wp.gameObject.name,
			Graphs = includeGraphs ? wp.graphs.Select(g => GetGraphJSON(g)).ToArray() : null
		};
	}
	private WebJSON.Graph GetGraphJSON(GraphBehaviour gb) {
		if (gb == null) return null;
		return new WebJSON.Graph() { Name = gb.gameObject.name };
	}

	public WebJSON.Collider GetColliderJSON(CollideComponent cc) {
		if (cc == null) return null;
		var flags = cc.CollisionFlagsR2;
		List<string> flagsList = new List<string>();

		foreach (CollideMaterial.CollisionFlags_R2 r in Enum.GetValues(typeof(CollideMaterial.CollisionFlags_R2))) {
			if (r == CollideMaterial.CollisionFlags_R2.None) continue;
			if ((flags & r) == r) flagsList.Add(r.ToString());
		}

		return new WebJSON.Collider() {
			CollideTypes = flagsList.ToArray()
		};
	}

    public void ParseMessage(string msgString) {
		WebJSON.Message msg = Newtonsoft.Json.JsonConvert.DeserializeObject<WebJSON.Message>(msgString, Settings);
        if (msg.SuperObject != null) {
            ParseSuperObjectJSON(msg.SuperObject);
		}
		if (msg.Perso != null) {
			ParsePersoJSON(msg.Perso);
		}
		if (msg.Settings != null) {
            ParseSettingsJSON(msg.Settings);
        }
		if (msg.Camera != null) {
			ParseCameraJSON(msg.Camera);
		}
        if (msg.Selection != null) {
            ParseSelectionJSON(msg.Selection);
        }
		if (msg.Request != null) {
			ParseRequestJSON(msg.Request);
		}
		if (msg.CineData != null) {
			ParseCineDataJSON(msg.CineData);
		}
		if (msg.Outline != null) {
			ParseOutlineJSON(msg.Outline);
		}
    }
    private void ParseSelectionJSON(WebJSON.Selection msg) {
        MapLoader l = MapLoader.Loader;
		if (msg.Perso != null && msg.Perso.Offset != null) {
			BasePersoBehaviour bpb = GetPersoFromOffset(msg.Perso.Offset);
			if (bpb != null) {
				selector.Select(bpb, view: msg.View);
			}
		} else if (msg.SuperObject != null && msg.SuperObject != null && msg.SuperObject.Offset != null) {
			SuperObjectComponent so = controller.superObjects.FirstOrDefault(s => s.Offset == msg.SuperObject.Offset);
			if (so != null) {
				selector.Select(so);
			}
		} else {
            selector.Deselect();
        }
	}
	private void ParseOutlineJSON(WebJSON.OutlineColors msg) {
		if (msg.HighlightColor.HasValue) selector.outline.highlightColor = msg.HighlightColor.Value;
		if (msg.SelectColor.HasValue) selector.outline.selectColor = msg.SelectColor.Value;
	}
	private void ParseSuperObjectJSON(WebJSON.SuperObject msg) {
		MapLoader l = MapLoader.Loader;
		SuperObjectComponent so = null;
		if (msg.Offset != null) {
			so = controller.superObjects.FirstOrDefault(s => s.Offset == msg.Offset);
		}
		if (so != null) {
			if (msg.Position.HasValue) so.transform.localPosition = msg.Position.Value;
			if (msg.Rotation.HasValue) so.transform.localEulerAngles = msg.Rotation.Value;
			if (msg.Scale.HasValue) so.transform.localScale = msg.Scale.Value;
		}
	}
	private void ParsePersoJSON(WebJSON.Perso msg) {
        MapLoader l = MapLoader.Loader;
        BasePersoBehaviour perso = null;
        if (msg.Offset != null) {
			perso = GetPersoFromOffset(msg.Offset);
        }
        if (perso != null) {
			BasePersoBehaviour pb = perso;
			if (msg.IsEnabled.HasValue) pb.IsEnabled = msg.IsEnabled.Value;
			if (msg.State.HasValue) {
				pb.stateIndex = msg.State.Value;
				/*if (pb == selectedPerso_) {
					selectedPersoStateIndex_ = msg.State.Value;
				}*/
			}
			if (msg.ObjectList.HasValue) pb.poListIndex = msg.ObjectList.Value;
			if (msg.PlayAnimation.HasValue) pb.playAnimation = msg.PlayAnimation.Value;
			if (msg.AnimationSpeed.HasValue) pb.animationSpeed = msg.AnimationSpeed.Value;
			if (msg.AutoNextState.HasValue) pb.autoNextState = msg.AutoNextState.Value;


			if (msg.Position.HasValue) pb.transform.localPosition = msg.Position.Value;
			if (msg.Rotation.HasValue) pb.transform.localEulerAngles = msg.Rotation.Value;
			if (msg.Scale.HasValue) pb.transform.localScale = msg.Scale.Value;
		}
    }
	private void ParseCameraJSON(WebJSON.CameraSettings msg) {
		if (msg == null) return;
		controller.ApplyCameraSettings(msg, applyCameraPos: true, applyTransform: true);
	}
    private void ParseSettingsJSON(WebJSON.Settings msg) {
		if (msg.ViewCollision.HasValue) controller.viewCollision = msg.ViewCollision.Value;
		if (msg.Luminosity.HasValue) controller.lightManager.luminosity = msg.Luminosity.Value;
		if (msg.Saturate.HasValue) controller.lightManager.saturate = msg.Saturate.Value;
		if (msg.ViewGraphs.HasValue) controller.viewGraphs = msg.ViewGraphs.Value;
		if (msg.EnableLighting.HasValue) controller.lightManager.enableLighting = msg.EnableLighting.Value;
		if (msg.EnableFog.HasValue) controller.lightManager.enableFog = msg.EnableFog.Value;
		if (msg.ViewInvisible.HasValue) controller.viewInvisible = msg.ViewInvisible.Value;
		if (msg.DisplayInactive.HasValue) controller.sectorManager.displayInactiveSectors = msg.DisplayInactive.Value;
		if (msg.PlayAnimations.HasValue) controller.playAnimations = msg.PlayAnimations.Value;
		if (msg.PlayTextureAnimations.HasValue) controller.playTextureAnimations = msg.PlayTextureAnimations.Value;
		if (msg.ShowPersos.HasValue) controller.showPersos = msg.ShowPersos.Value;
	}
	private void ParseCineDataJSON(WebJSON.CineData msg) {
		if (msg != null && controller.CinematicSwitcher != null) {
			if (msg.CinematicIndex.HasValue) controller.CinematicSwitcher.CinematicIndex = msg.CinematicIndex.Value;
			if (msg.AnimationSpeed.HasValue) controller.CinematicSwitcher.animationSpeed = msg.AnimationSpeed.Value;
		}
	}
	private void ParseRequestJSON(WebJSON.Request msg) {
		switch (msg.Type) {
			case WebJSON.RequestType.Script:
				BaseScriptComponent s = GetScriptFromRequest(msg);
				if (s != null) Send(new WebJSON.Message() {
					Type = WebJSON.MessageType.Script,
					Script = GetScriptJSON(s, true)
				});
				break;
			case WebJSON.RequestType.Comport:
				BrainComponent.Comport c = GetComportFromRequest(msg);
				if (c != null) Send(new WebJSON.Message() {
					Type = WebJSON.MessageType.Comport,
					Comport = GetComportJSON(c, includeScriptContents: true)
				});
				break;
			case WebJSON.RequestType.Macro:
				BrainComponent.Macro m = GetMacroFromRequest(msg);
				if (m != null) Send(new WebJSON.Message() {
					Type = WebJSON.MessageType.Macro,
					Macro = GetMacroJSON(m, includeScriptContents: true)
				});
				break;
			case WebJSON.RequestType.TransitionExport:
				if (selectedPerso_ != null && selectedPerso_ is PersoBehaviour) {
					MindComponent mc = selectedPerso_.GetComponent<MindComponent>();
					if (mc != null) {
						try {
							var export = mc.TransitionExport;
							if (export != null) {
								Send(new WebJSON.Message() {
									Type = WebJSON.MessageType.TransitionExport,
									TransitionExport = export
								});
							}
						} catch (Exception) { }
					}
				}
				break;
			case WebJSON.RequestType.Screenshot:
				TakeScreenshot(msg.Screenshot).Forget(); // Start the async task for taking a screenshot
				break;
		}
	}
	async UniTaskVoid TakeScreenshot(WebJSON.Screenshot msg) {
		if (msg != null) {
			TransparencyCaptureBehaviour tcb = Camera.main.GetComponent<TransparencyCaptureBehaviour>();
			if (tcb != null) {
				try {
					Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
					int height = msg.Height ?? Mathf.RoundToInt(res.height * (msg.SizeFactor ?? 1));
					int width = msg.Width ?? Mathf.RoundToInt(res.width * (msg.SizeFactor ?? 1));
					if (width > 0 && height > 0) {
						System.DateTime dateTime = System.DateTime.Now;
						byte[] screenshotBytes = await tcb.Capture(width, height, msg.IsTransparent ?? true);
						SaveFile(screenshotBytes, screenshotBytes.Length, $"Screenshot_{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}.png");
					}
				} catch (Exception) {
					Debug.Log("Screenshot failed");
				}
			}
		}
	}
	private BaseScriptComponent GetScriptFromRequest(WebJSON.Request msg) {
		if (selector.selectedPerso == null || selector.selectedPerso.brain == null) return null;
		BrainComponent brain = selector.selectedPerso.brain;
		Pointer offset = msg.Offset;
		switch (msg.BehaviorType) {
			case WebJSON.BehaviorType.Intelligence:
				if (brain.Intelligence == null) return null;
				foreach (var be in brain.Intelligence) {
					if (be.FirstScript != null && be.FirstScript.Offset == offset) return be.FirstScript;
					foreach (var s in be.Scripts) {
						if (s.Offset == offset) return s;
					}
				}
				break;
			case WebJSON.BehaviorType.Reflex:
				if (brain.Reflex == null) return null;
				foreach (var be in brain.Reflex) {
					if (be.FirstScript != null && be.FirstScript.Offset == offset) return be.FirstScript;
					foreach (var s in be.Scripts) {
						if (s.Offset == offset) return s;
					}
				}
				break;
			case WebJSON.BehaviorType.Macro:
				if (brain.Macros == null) return null;
				foreach (var m in brain.Macros) {
					if (m.Script != null && m.Script.Offset == offset) return m.Script;
				}
				break;
		}
		return null;
	}
	private BrainComponent.Comport GetComportFromRequest(WebJSON.Request msg) {
		if (selector.selectedPerso == null || selector.selectedPerso.brain == null) return null;
		BrainComponent brain = selector.selectedPerso.brain;
		Pointer offset = msg.Offset;
		switch (msg.BehaviorType) {
			case WebJSON.BehaviorType.Intelligence:
				if (brain.Intelligence == null) return null;
				foreach (var be in brain.Intelligence) {
					if (be.Offset == offset) return be;
				}
				break;
			case WebJSON.BehaviorType.Reflex:
				if (brain.Reflex == null) return null;
				foreach (var be in brain.Reflex) {
					if (be.Offset == offset) return be;
				}
				break;
		}
		return null;
	}
	private BrainComponent.Macro GetMacroFromRequest(WebJSON.Request msg) {
		if (selector.selectedPerso == null || selector.selectedPerso.brain == null) return null;
		BrainComponent brain = selector.selectedPerso.brain;
		Pointer offset = msg.Offset;
		switch (msg.BehaviorType) {
			case WebJSON.BehaviorType.Macro:
				if (brain.Macros == null) return null;
				foreach (var m in brain.Macros) {
					if (m.Offset == offset) return m;
				}
				break;
		}
		return null;
	}
	private BasePersoBehaviour GetPersoFromOffset(Pointer offset) {
		if (offset == selectedPerso_?.Offset) {
			return selectedPerso_;
		} else {
			MapLoader l = MapLoader.Loader;
			switch (l) {
				case OpenSpace.Loader.R2ROMLoader rl:
					return controller.romPersos.FirstOrDefault(p => p.Offset == offset);
				case OpenSpace.Loader.R2PS1Loader pl:
					return controller.ps1Persos.FirstOrDefault(p => p.Offset == offset);
				default:
					return controller.persos.FirstOrDefault(p => p.Offset == offset);
			}
		}
	}
	private BasePersoBehaviour GetPersoFromSuperObjectOffset(Pointer offset) {
		if (offset == selectedPerso_?.Offset) {
			return selectedPerso_;
		} else {
			MapLoader l = MapLoader.Loader;
			switch (l) {
				case OpenSpace.Loader.R2ROMLoader rl:
					return controller.romPersos.FirstOrDefault(p => p.superObject.Offset == offset);
				case OpenSpace.Loader.R2PS1Loader pl:
					return controller.ps1Persos.FirstOrDefault(p => p.superObject.Offset == offset);
				default:
					return controller.persos.FirstOrDefault(p => p.perso.SuperObject?.offset == offset);
			}
		}
	}
}
