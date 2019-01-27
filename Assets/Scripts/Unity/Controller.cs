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
using OpenSpace.Waypoints;
using OpenSpace.Object.Properties;

public class Controller : MonoBehaviour {
	public Settings.Mode mode = Settings.Mode.Rayman3PC;
	public string gameDataBinFolder;
	public string lvlName;

	public Material baseMaterial;
	public Material baseTransparentMaterial;
	public Material baseLightMaterial;
	public Material collideMaterial;
	public Material collideTransparentMaterial;
	public SectorManager sectorManager;
	public LightManager lightManager;
	public LoadingScreen loadingScreen;
	public WebCommunicator communicator;
	public bool allowDeadPointers = false;
	public bool forceDisplayBackfaces = false;
	public bool blockyMode = false;
	MapLoader loader = null;
	bool viewCollision_ = false; public bool viewCollision = false;
	bool viewInvisible_ = false; public bool viewInvisible = false;
	bool viewGraphs_ = false; public bool viewGraphs = false;
	bool playAnimations_ = true; public bool playAnimations = true;
	bool playTextureAnimations_ = true; public bool playTextureAnimations = true;
	bool showPersos_ = true; public bool showPersos = true;
    bool livePreview_ = false; public bool livePreview = false;
    float livePreviewUpdateCounter = 0;

	private GameObject graphRoot = null;
	private GameObject isolateWaypointRoot = null;
	private CinematicSwitcher cinematicSwitcher = null;

	public enum State {
		None,
		Downloading,
		Loading,
		Initializing,
		Error,
		Finished
	}
	private State state = State.None;
	private string detailedState = "None";
	public State LoadState {
		get { return state; }
	}

	// Use this for initialization
	void Start() {
		// Read command line arguments
		string[] args = System.Environment.GetCommandLineArgs();
		string modeString = "";
		for (int i = 0; i < args.Length; i++) {
			switch (args[i]) {
				case "--lvl":
				case "-l":
					lvlName = args[i + 1];
					i++;
					break;
				case "--folder":
				case "--directory":
				case "-d":
				case "-f":
					gameDataBinFolder = args[i + 1];
					i++;
					break;
				case "--mode":
				case "-m":
					modeString = args[i + 1];
					i++;
					break;
			}
		}
		Application.logMessageReceived += Log;

		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			//Application.logMessageReceived += communicator.WebLog;
			Debug.unityLogger.logEnabled = false; // We don't need prints here
			string url = Application.absoluteURL;
			if (url.IndexOf('?') > 0) {
				string urlArgsStr = url.Split('?')[1].Split('#')[0];
				if (urlArgsStr.Length > 0) {
					string[] urlArgs = urlArgsStr.Split('&');
					foreach (string arg in urlArgs) {
						string[] argKeyVal = arg.Split('=');
						if (argKeyVal.Length > 1) {
							switch (argKeyVal[0]) {
								case "lvl":
									lvlName = argKeyVal[1]; break;
								case "mode":
									modeString = argKeyVal[1]; break;
								case "folder":
								case "directory":
								case "dir":
									gameDataBinFolder = argKeyVal[1]; break;
							}
						}
					}
				}
			}
		}
		switch (modeString) {
			case "r3_gc":
				mode = Settings.Mode.Rayman3GC; break;
			case "ra_gc":
				mode = Settings.Mode.RaymanArenaGC; break;
			case "r3_pc":
				mode = Settings.Mode.Rayman3PC; break;
			case "ra_pc":
				mode = Settings.Mode.RaymanArenaPC; break;
			case "r2_pc":
				mode = Settings.Mode.Rayman2PC; break;
			case "r2_dc":
				mode = Settings.Mode.Rayman2DC; break;
			case "r2_ios":
				mode = Settings.Mode.Rayman2IOS; break;
			case "r2_ps1":
				mode = Settings.Mode.Rayman2PS1; break;
			case "r2_ps2":
				mode = Settings.Mode.Rayman2PS2; break;
			case "dd_pc":
				mode = Settings.Mode.DonaldDuckPC; break;
			case "dd_dc":
				mode = Settings.Mode.DonaldDuckDC; break;
			case "tt_pc":
				mode = Settings.Mode.TonicTroublePC; break;
			case "ttse_pc":
				mode = Settings.Mode.TonicTroubleSEPC; break;
			case "r2_demo1_pc":
				mode = Settings.Mode.Rayman2PCDemo1; break;
			case "r2_demo2_pc":
				mode = Settings.Mode.Rayman2PCDemo2; break;
			case "playmobil_hype_pc":
				mode = Settings.Mode.PlaymobilHypePC; break;
			case "playmobil_alex_pc":
				mode = Settings.Mode.PlaymobilAlexPC; break;
			case "playmobil_laura_pc":
				mode = Settings.Mode.PlaymobilLauraPC; break;
		}
		loadingScreen.Active = true;
		Settings.Init(mode);
		loader = MapLoader.Loader;
		loader.controller = this;
		loader.gameDataBinFolder = gameDataBinFolder;
		loader.lvlName = lvlName;
		loader.allowDeadPointers = allowDeadPointers;
		loader.forceDisplayBackfaces = forceDisplayBackfaces;
		loader.blockyMode = blockyMode;
		loader.baseMaterial = baseMaterial;
		loader.baseTransparentMaterial = baseTransparentMaterial;
		loader.collideMaterial = collideMaterial;
		loader.collideTransparentMaterial = collideTransparentMaterial;
		loader.baseLightMaterial = baseLightMaterial;

		StartCoroutine(Init());
	}

	IEnumerator Init() {
#if UNITY_EDITOR
		if (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL) {
			FileSystem.mode = FileSystem.Mode.Web;
		}
#endif
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			FileSystem.mode = FileSystem.Mode.Web;
		}
		state = State.Loading;
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine(loader.Load());
		yield return new WaitForEndOfFrame();
		if (state == State.Error) yield break;
		state = State.Initializing;
		detailedState = "Initializing sectors";
		yield return null;
		sectorManager.Init();
		detailedState = "Initializing graphs";
		yield return null;
		CreateGraphs();
		detailedState = "Initializing persos";
		yield return StartCoroutine(InitPersos());
		detailedState = "Initializing lights";
		yield return null;
		lightManager.Init();
		detailedState = "Initializing camera";
		yield return null;
		InitCamera();
		/*if (viewCollision)*/
		UpdateViewCollision();
		if (loader.cinematicsManager != null) {
			detailedState = "Initializing cinematics";
			yield return null;
			InitCinematics();
		}
		detailedState = "Finished";
		state = State.Finished;
		loadingScreen.Active = false;
	}

	// Update is called once per frame
	void Update() {
		if (loadingScreen.Active) {
			if (state == State.Error) {
				loadingScreen.LoadingText = detailedState;
				loadingScreen.LoadingtextColor = Color.red;
			} else {
				if (state == State.Loading) {
					loadingScreen.LoadingText = loader.loadingState;
				} else {
					loadingScreen.LoadingText = detailedState;
				}
				loadingScreen.LoadingtextColor = Color.white;
			}
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			viewCollision = !viewCollision;
		}
		if (Input.GetKeyDown(KeyCode.I)) {
			viewInvisible = !viewInvisible;
		}
		if (Input.GetKeyDown(KeyCode.G)) {
			viewGraphs = !viewGraphs;
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			playAnimations = !playAnimations;
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			playTextureAnimations = !playTextureAnimations;
		}
		if (Input.GetKeyDown(KeyCode.U)) {
			showPersos = !showPersos;
		}
		bool updatedSettings = false;
		if (loader != null) {
			if (viewInvisible != viewInvisible_) {
				updatedSettings = true;
				UpdateViewInvisible();
			}
			if (viewCollision != viewCollision_) {
				updatedSettings = true;
				UpdateViewCollision();
			}
			if (viewGraphs != viewGraphs_) {
				updatedSettings = true;
				UpdateViewGraphs();
			}
			if (showPersos != showPersos_) {
				updatedSettings = true;
				UpdateShowPersos();
			}
            if (livePreview != livePreview_) {
                livePreview_ = livePreview;
                //updatedSettings = true;
            }
            if (playAnimations != playAnimations_ || playTextureAnimations != playTextureAnimations_) {
				playTextureAnimations_ = playTextureAnimations;
				playAnimations_ = playAnimations;
				updatedSettings = true;
			}


		}
		if (updatedSettings) {
			communicator.SendSettings();
		}

        if (livePreview) {
            livePreviewUpdateCounter += Time.deltaTime;
            if (livePreviewUpdateCounter > 1.0f / 60.0f) {
                UpdateLivePreview();
                livePreviewUpdateCounter = 0.0f;
            }
        }
    }

    public IEnumerator InitPersos() {
		if (loader != null) {
			for (int i = 0; i < loader.persos.Count; i++) {
				detailedState = "Initializing persos: " + i + "/" + loader.persos.Count;
				yield return null;
				Perso p = loader.persos[i];
				PersoBehaviour unityBehaviour = p.Gao.AddComponent<PersoBehaviour>();
				unityBehaviour.controller = this;
				if (loader.globals != null && loader.globals.spawnablePersos != null) {
					if (loader.globals.spawnablePersos.IndexOf(p) > -1) {
						unityBehaviour.IsAlways = true;
						unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
					}
				}
				if (!unityBehaviour.IsAlways) {
					if (p.sectInfo != null && p.sectInfo.off_sector != null) {
						unityBehaviour.sector = Sector.FromSuperObjectOffset(p.sectInfo.off_sector);
					} else {
						unityBehaviour.sector = sectorManager.GetActiveSectorsAtPoint(p.Gao.transform.position).FirstOrDefault();
					}
				} else unityBehaviour.sector = null;
				Moddable mod = null;
				if (p.SuperObject != null && p.SuperObject.Gao != null) {
					mod = p.SuperObject.Gao.GetComponent<Moddable>();
					if (mod != null) {
						mod.persoBehaviour = unityBehaviour;
					}
				}
				unityBehaviour.perso = p;
				unityBehaviour.Init();

				// Scripts
				if (p.Gao) {
					if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null) {
						if (p.brain.mind.AI_model.behaviors_normal != null) {
							GameObject intelParent = new GameObject("Rule behaviours");
							intelParent.transform.parent = p.Gao.transform;
							Behavior[] normalBehaviors = p.brain.mind.AI_model.behaviors_normal;
							int iter = 0;
							foreach (Behavior behavior in normalBehaviors) {
								GameObject behaviorGao = new GameObject(behavior.ShortName);
								behaviorGao.transform.parent = intelParent.transform;
								foreach (Script script in behavior.scripts) {
									GameObject scriptGao = new GameObject("Script");
									scriptGao.transform.parent = behaviorGao.transform;
									ScriptComponent scriptComponent = scriptGao.AddComponent<ScriptComponent>();
									scriptComponent.SetScript(script, p);
								}
								if (behavior.firstScript != null) {
									ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
									scriptComponent.SetScript(behavior.firstScript, p);
								}
								if (iter == 0) {
									behaviorGao.name += " (Init)";
								}
								if ((behavior.scripts == null || behavior.scripts.Length == 0) && behavior.firstScript == null) {
									behaviorGao.name += " (Empty)";
								}
								iter++;
							}
						}
						if (p.brain.mind.AI_model.behaviors_reflex != null) {
							GameObject reflexParent = new GameObject("Reflex behaviours");
							reflexParent.transform.parent = p.Gao.transform;
							Behavior[] reflexBehaviors = p.brain.mind.AI_model.behaviors_reflex;
							int iter = 0;
							foreach (Behavior behavior in reflexBehaviors) {
								GameObject behaviorGao = new GameObject(behavior.ShortName);
								behaviorGao.transform.parent = reflexParent.transform;
								foreach (Script script in behavior.scripts) {
									GameObject scriptGao = new GameObject("Script");
									scriptGao.transform.parent = behaviorGao.transform;
									ScriptComponent scriptComponent = scriptGao.AddComponent<ScriptComponent>();
									scriptComponent.SetScript(script, p);
								}
								if (behavior.firstScript != null) {
									ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
									scriptComponent.SetScript(behavior.firstScript, p);
								}
								if ((behavior.scripts == null || behavior.scripts.Length == 0) && behavior.firstScript == null) {
									behaviorGao.name += " (Empty)";
								}
								iter++;
							}
						}
						if (p.brain.mind.AI_model.macros != null) {
							GameObject macroParent = new GameObject("Macros");
							macroParent.transform.parent = p.Gao.transform;
							Macro[] macros = p.brain.mind.AI_model.macros;
							int iter = 0;

							foreach (Macro macro in macros) {
								GameObject behaviorGao = new GameObject(macro.ShortName);
								behaviorGao.transform.parent = macroParent.transform;
								ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
								scriptComponent.SetScript(macro.script, p);
								iter++;
							}
						}
					}
					// DsgVars
					if (p.brain != null && p.brain.mind != null && p.brain.mind.dsgMem != null) {
						DsgVarComponent dsgVarComponent = p.Gao.AddComponent<DsgVarComponent>();
						dsgVarComponent.SetPerso(p);
						if (mod != null) mod.dsgVarComponent = dsgVarComponent;
					}
					// Dynam
					if (p.dynam != null && p.dynam.dynamics != null) {
						DynamicsMechanicsComponent dynamicsBehaviour = p.Gao.AddComponent<DynamicsMechanicsComponent>();
						dynamicsBehaviour.SetDynamics(p.dynam.dynamics);
					}
					// Mind
					if (p.brain != null && p.brain.mind != null) {
						MindComponent mindComponent = p.Gao.AddComponent<MindComponent>();
						mindComponent.Init(p, p.brain.mind);
						if (mod != null) mod.mindComponent = mindComponent;
					}
					// Custom Bits
					if (p.stdGame != null) {
						CustomBitsComponent c = p.Gao.AddComponent<CustomBitsComponent>();
						c.stdGame = p.stdGame;
						if (Settings.s.engineVersion == Settings.EngineVersion.R3) c.hasAiCustomBits = true;
						c.Init();
					}
				}
			}
		}
	}

	public void InitCamera() {
		if (loader != null) {
			Perso camera = loader.persos.FirstOrDefault(p => p != null && p.namePerso.Equals("StdCamer"));
			if (camera == null && loader.globals != null) {
				camera = loader.persos.FirstOrDefault(p => p != null && p.stdGame != null && p.stdGame.off_superobject == loader.globals.off_camera);
			}
			if (camera != null) {
				Camera.main.transform.position = camera.Gao.transform.position;
				Camera.main.transform.rotation = camera.Gao.transform.rotation * Quaternion.Euler(0, 180, 0);
			}
		}
	}

	public void UpdateViewGraphs() {
		if (loader != null) {
			viewGraphs_ = viewGraphs;
			if (graphRoot != null) graphRoot.SetActive(viewGraphs);
			if (isolateWaypointRoot != null) isolateWaypointRoot.SetActive(viewGraphs);
		}
	}

	public void UpdateViewInvisible() {
		if (loader != null) {
			viewInvisible_ = viewInvisible;
			foreach (SuperObject so in loader.superObjects) {
				if (so.type == SuperObject.Type.Perso) {
					UpdatePersoActive(so.data as Perso);
				} else {
					if (so.Gao != null) {
						if (so.flags.HasFlag(OpenSpace.Object.Properties.SuperObjectFlags.Flags.Invisible)) {
							so.Gao.SetActive(viewCollision || viewInvisible);
						}
					}
				}
			}
		}
	}

	public void UpdateShowPersos() {
		if (loader != null) {
			showPersos_ = showPersos;
			foreach (Perso perso in loader.persos) {
				UpdatePersoActive(perso);
			}
		}
	}

	public void UpdatePersoActive(Perso perso) {
		if (perso != null && perso.Gao != null) {
			PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
			if (pb != null) {
				bool isVisible = true;
				if (perso.SuperObject != null) {
					isVisible = !perso.SuperObject.flags.HasFlag(OpenSpace.Object.Properties.SuperObjectFlags.Flags.Invisible);
				}
				perso.Gao.SetActive(showPersos && pb.IsEnabled && (isVisible || (viewCollision || viewInvisible)));
			}
		}
	}

	public void UpdateViewCollision() {
		if (loader != null) {
			viewCollision_ = viewCollision;
			foreach (PhysicalObject po in loader.physicalObjects) {
				foreach (VisualSetLOD l in po.visualSet) {
					if (l.obj != null) {
						GameObject gao = l.obj.Gao;
						if (gao != null) gao.SetActive(!viewCollision);
					}
				}
				if (po.collideMesh != null) po.collideMesh.SetVisualsActive(viewCollision);
				//if (po.collideMesh != null) po.collideMesh.gao.SetActive(viewCollision);
			}
			foreach (Perso perso in loader.persos) {
				if (perso.collset != null) {
					CollSet c = perso.collset;
					if (c.zdd != null) foreach (CollideMeshObject col in c.zdd) {
							if (col == null) continue;
							col.SetVisualsActive(viewCollision);
							//col.gao.SetActive(viewCollision);
						}
					if (c.zde != null) foreach (CollideMeshObject col in c.zde) {
							if (col == null) continue;
							col.SetVisualsActive(viewCollision);
							//col.gao.SetActive(viewCollision);
						}
					if (c.zdm != null) foreach (CollideMeshObject col in c.zdm) {
							if (col == null) continue;
							col.SetVisualsActive(viewCollision);
							//col.gao.SetActive(viewCollision);
						}
					if (c.zdr != null) foreach (CollideMeshObject col in c.zdr) {
							if (col == null) continue;
							col.SetVisualsActive(viewCollision);
							//col.gao.SetActive(viewCollision);
						}
				}

			}
			foreach (SuperObject so in loader.superObjects) {
				if (so.Gao != null) {
					if (so.flags.HasFlag(OpenSpace.Object.Properties.SuperObjectFlags.Flags.Invisible)) {
						if (so.type == SuperObject.Type.Perso) {
							UpdatePersoActive(so.data as Perso);
						} else {
							so.Gao.SetActive(viewCollision || viewInvisible);
						}
					}
				}
			}
		}
	}

	public void CreateGraphs() {
		MapLoader l = MapLoader.Loader;
		if (graphRoot == null && l.graphs.Count > 0) {
			graphRoot = new GameObject("Graphs");
			graphRoot.SetActive(false);
		}
		foreach (Graph graph in l.graphs) {
			GameObject go_graph = new GameObject(graph.name ?? "Graph " + graph.offset.ToString());
			go_graph.transform.SetParent(graphRoot.transform);

			for (int i = 0; i < graph.nodes.Count; i++) {
				GraphNode node = graph.nodes[i];
				node.Gao.name = "GraphNode[" + i + "].WayPoint ("+node.wayPoint.offset+")";
				if (i == 0) {
					go_graph.transform.position = graph.nodes[i].Gao.transform.position;
				}
				node.Gao.transform.SetParent(go_graph.transform);
			}
		}

		List<WayPoint> isolateWaypoints = l.waypoints.Where(w => w.containingGraphNodes.Count == 0).ToList();
		if (isolateWaypointRoot == null && isolateWaypoints.Count > 0) {
			isolateWaypointRoot = new GameObject("Isolate WayPoints");
			isolateWaypointRoot.SetActive(false);
		}
		foreach (WayPoint wayPoint in isolateWaypoints) {
			wayPoint.Gao.name = "Isolate WayPoint @"+wayPoint.offset;
			wayPoint.Gao.transform.SetParent(isolateWaypointRoot.transform);
		}
	}

	public void InitCinematics() {
		if (loader.cinematicsManager != null && loader.cinematicsManager.cinematics.Count > 0) {
			cinematicSwitcher = new GameObject("Cinematic Switcher").AddComponent<CinematicSwitcher>();
			cinematicSwitcher.Init();
		}
	}

    public void UpdateLivePreview() {
        Reader reader = MapLoader.Loader.livePreviewReader;

        foreach (SuperObject so in MapLoader.Loader.superObjects) {

            if (!(so.data is Perso)) {
                continue;
            }

            if (so.off_matrix==null) {
                continue;
            }
            Pointer.Goto(ref reader, so.off_matrix);
            so.matrix = Matrix.Read(MapLoader.Loader.livePreviewReader, so.off_matrix);
            if (so.data != null && so.data.Gao != null) {
                so.data.Gao.transform.localPosition = so.matrix.GetPosition(convertAxes: true);
                so.data.Gao.transform.localRotation = so.matrix.GetRotation(convertAxes: true);
                so.data.Gao.transform.localScale = so.matrix.GetScale(convertAxes: true);

                if (so.data is Perso) {
                    Perso perso = (Perso)so.data;

                    PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
                    if (pb != null) {

                        Pointer.Goto(ref reader, perso.p3dData.offset);
                        perso.p3dData.UpdateCurrentState(reader);

						// State offset changed?
						if (perso.p3dData.stateCurrent != null) {
							pb.SetState(perso.p3dData.stateCurrent.index);
							pb.autoNextState = true;
						}
                    }


                    MindComponent mc = perso.Gao.GetComponent<MindComponent>();
                    if (mc != null) {
                        Mind mind = mc.mind;
                        Pointer.Goto(ref reader, mind.offset);
                        mind.UpdateCurrentBehaviors(reader);
                    }

                    DsgVarComponent dsgVarComponent = perso.Gao.GetComponent<DsgVarComponent>();
                    if (dsgVarComponent != null) {
                        dsgVarComponent.SetPerso(perso);
                    }

                    CustomBitsComponent customBitsComponent = perso.Gao.GetComponent<CustomBitsComponent>();
                    if (customBitsComponent != null) {
                        Pointer.Goto(ref reader, perso.off_stdGame);
                        perso.stdGame = StandardGame.Read(reader, perso.off_stdGame);
                        customBitsComponent.stdGame = perso.stdGame;
                        customBitsComponent.Init();
                    }
                }
            }
        }

        Perso camera = loader.persos.FirstOrDefault(p => p != null && p.namePerso.Equals("StdCamer"));
        if (camera != null) {

            SuperObject cameraSO = camera.SuperObject;
            Pointer.Goto(ref reader, cameraSO.off_matrix);
            cameraSO.matrix = Matrix.Read(reader, cameraSO.off_matrix);
            camera.Gao.transform.localPosition = cameraSO.matrix.GetPosition(convertAxes: true);
            camera.Gao.transform.localRotation = cameraSO.matrix.GetRotation(convertAxes: true);
            camera.Gao.transform.localScale = cameraSO.matrix.GetScale(convertAxes: true);

            Camera.main.transform.position = camera.Gao.transform.position;
            Camera.main.transform.rotation = camera.Gao.transform.rotation * Quaternion.Euler(0, 180, 0);
        }
    }

    public void SaveChanges() {
		if (loader != null) loader.Save();
	}

	public void Log(string condition, string stacktrace, LogType type) {
		switch (type) {
			case LogType.Exception:
			case LogType.Error:
				if (state != State.Finished) {
					// Allowed exceptions
					if (condition.Contains("cleaning the mesh failed")) break;

					// Go to error state
					state = State.Error;
					if (loadingScreen.Active) {
						detailedState = condition;
					}
				}
				break;
		}
	}
}
