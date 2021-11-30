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
using OpenSpace.Exporter;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenSpace.Loader;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity;

public class Controller : MonoBehaviour {
	public Material baseMaterial;
	public Material baseTransparentMaterial;
	public Material baseLightMaterial;
	public Material collideMaterial;
	public Material collideTransparentMaterial;
	public SectorManager sectorManager;
	public LightManager lightManager;
	public GraphManager graphManager;
	public PortalManager portalManager;
	public LoadingScreen loadingScreen;
	public ObjectSelector selector;
	public WebCommunicator communicator;
	public RecordingTool recordingToolPrefab;
	public GameObject SpawnableParent { get; set; }
	public GameObject PersoPartsParent { get; set; }
	public TMP_Text ScreenshotHighlightTextPrefab;

	public MapLoader loader = null;
	bool viewCollision_ = false; public bool viewCollision = false;
	bool viewInvisible_ = false; public bool viewInvisible = false;
	bool viewGraphs_ = false; public bool viewGraphs = false;
	bool playAnimations_ = true; public bool playAnimations = true;
	bool playTextureAnimations_ = true; public bool playTextureAnimations = true;
	bool showPersos_ = true; public bool showPersos = true;
	bool livePreview_ = false; public bool livePreview = false;

	public Dictionary<WebJSON.CameraPos, WebJSON.CameraSettings> CameraSettings = new Dictionary<WebJSON.CameraPos, WebJSON.CameraSettings>();
	
	float livePreviewUpdateCounter = 0;

	public CinematicSwitcher CinematicSwitcher { get; private set; } = null;
	
	private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

	private bool ExportAfterLoad { get; set; }
	private bool ExportText { get; set; }
    private UnitySettings.ScreenshotAfterLoadSetting ScreenshotAfterLoad { get; set; }
	private float ScreenshotScale { get; set; }
    private string HighlightObjectsFilter { get; set; }
    private string HighlightObjectsTextFormat { get; set; }
    public MapExporter.ExportFlags ExportFlags { get; set; }
    public string ExportPath { get; set; }


	// Lists for easy access
	public List<PersoBehaviour> persos { get; set; } = new List<PersoBehaviour>();
	public List<ROMPersoBehaviour> romPersos { get; set; } = new List<ROMPersoBehaviour>();
	public List<PS1PersoBehaviour> ps1Persos { get; set; } = new List<PS1PersoBehaviour>();
	public List<SuperObjectComponent> superObjects { get; set; } = new List<SuperObjectComponent>();

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
	public async UniTaskVoid Start() {
		// Read command line arguments
		string[] args = System.Environment.GetCommandLineArgs();
		string gameDataBinFolder = "";
		string lvlName = "";
		string actor1Name = "";
		string actor2Name = "";
		Settings.Mode mode = UnitySettings.GameMode;
		gameDataBinFolder = UnitySettings.CurrentDirectory;
		lvlName = UnitySettings.MapName;
		actor1Name = UnitySettings.Actor1Name;
		actor2Name = UnitySettings.Actor2Name;

		ExportPath = UnitySettings.ExportPath;
		ExportAfterLoad = UnitySettings.ExportAfterLoad;
        ExportFlags = UnitySettings.ExportFlags;
        ScreenshotAfterLoad = UnitySettings.ScreenshotAfterLoad;
        ScreenshotScale = UnitySettings.ScreenshotScale;
        HighlightObjectsFilter = UnitySettings.HighlightObjectsFilter;
        HighlightObjectsTextFormat = UnitySettings.HighlightObjectsTextFormat;
		if (FileSystem.mode != FileSystem.Mode.Web) {
			if (UnitySettings.LoadFromMemory) {
				lvlName = UnitySettings.ProcessName + ".exe";
			}
		}
		Application.logMessageReceived += Log;

		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			//Application.logMessageReceived += communicator.WebLog;
			//UnityEngine.Debug.unityLogger.logEnabled = false; // We don't need prints here
			UnityEngine.Debug.unityLogger.filterLogType = LogType.Assert;
		}
		loadingScreen.Active = true;
		Settings.Init(mode);
		loader = MapLoader.Loader;
		loader.controller = this;
		loader.gameDataBinFolder = gameDataBinFolder;
		loader.lvlName = lvlName;
		loader.baseMaterial = baseMaterial;
		loader.baseTransparentMaterial = baseTransparentMaterial;
		loader.collideMaterial = collideMaterial;
		loader.collideTransparentMaterial = collideTransparentMaterial;
		loader.baseLightMaterial = baseLightMaterial;
		
		loader.allowDeadPointers = UnitySettings.AllowDeadPointers;
		loader.forceDisplayBackfaces = UnitySettings.ForceDisplayBackfaces;
		loader.blockyMode = UnitySettings.BlockyMode;
		loader.exportTextures = UnitySettings.SaveTextures;
		ExportText = UnitySettings.ExportText;

		if (loader is R2PS1Loader) {
			(loader as R2PS1Loader).actor1Name = actor1Name;
			(loader as R2PS1Loader).actor2Name = actor2Name;
		}

		await Init();
	}

    async UniTask Init() {
        state = State.Loading;
        await loader.LoadWrapper();
        if (state == State.Error) return;
        stopwatch.Start();
        state = State.Initializing;
        detailedState = "Initializing sectors";
        await WaitIfNecessary();
        sectorManager.Init();
        detailedState = "Initializing graphs";
        await WaitIfNecessary();
        graphManager.Init();
        detailedState = "Initializing lights";
        await WaitIfNecessary();
        lightManager.Init();
        detailedState = "Initializing persos";
        await InitPersos();
        sectorManager.InitLights();
        detailedState = "Initializing camera";
        await WaitIfNecessary();
        InitCamera();
        detailedState = "Initializing portals";
        await WaitIfNecessary();
        portalManager.Init();

        /*if (viewCollision)*/
        UpdateViewCollision();
        if (loader.cinematicsManager != null || loader is R2PS1Loader) {
            detailedState = "Initializing cinematics";
            await UniTask.WaitForEndOfFrame();
            InitCinematics();
        }
        detailedState = "Finished";
        stopwatch.Stop();
        state = State.Finished;
        loadingScreen.Active = false;

		if (ExportText) {
			MapExporter.ExportText();
		}

        if (ExportAfterLoad) {
            MapExporter e = new MapExporter(this.loader, ExportPath, ExportFlags);
            e.Export();

            Application.Quit();
        }

        if (!string.IsNullOrWhiteSpace(HighlightObjectsFilter)) {

            int count = 0;

            foreach (var p in persos) {

                int itemCount = 1;
                bool passesFilter = string.Equals(HighlightObjectsFilter, "*");

                if (string.Equals(HighlightObjectsFilter, "$redlums")) {

                    DsgMem dsgMem = p.perso?.brain?.mind?.dsgMem;

                    // Red lums:
					// CHR_lums_basic (dsgVar_0, 0 = red lums, 1 = yellow lums (including big lusm), 2 = ?, 3+4 = blue lums, 
					// Alw_Lums_Model (dsgVar_0, 0 = red lums, 1 = yellow lums (including big lusm), 2 = ?, 3+4 = blue lums, 
					// ARG_SerieRouge
					// Red lums spawned by:
					// MIC_SbireNinja (5 lums)
					// SUN_Clarky (1 lum per hit, maximum of 5 lums (6th hit doesnt give a lum)
					// SUN_fayagwod (7 lums)
					// ARG_Araignee -> 1 lum
					// OLD_SBIRE_alarme (depends on custombits (excludes cb30) dsgvar_1: 0->3, 1->2, 2->4)
					// ARG_SimpleSbire -> 1 lum, depending on CB31/CB29 and actor hitpoints
					// XAP_TonoGear_light -> 3 red lums
					// STN_tonogear -> 3 red lums
					// FRH_Sbire_Gnak -> CB31 = invincible -> depending on CB30 + DsgVar_2: 0->2 lums, !0->3 lums 
					// ARG_CageLums -> doesnt spawn red lums, lums activated by a different object

                    bool invincible = (p.perso.stdGame.customBits & 1 << 30) != 0; // 31 is 1-based, 30 = 0-based

					if (p.NameModel.ToLower() == "chr_lums_basic") {

                        if (dsgMem != null) {
                            int value_0 = dsgMem.values[0].valueInt;
                            if (value_0 == 0) {
								passesFilter = true;
                                itemCount = 1;

                                p.stateIndex = 2;
							}
                        }


                    } else if (p.NameModel.ToLower() == "alw_lums_model") {

                        if (dsgMem != null) {
                            byte value_0 = dsgMem.values[0].valueUByte;
                            if (value_0 == 0) {
                                passesFilter = true;
								itemCount = 1;

                                p.stateIndex = 2;
							}
                        }


					} else if (p.NameModel.ToLower() == "arg_serierouge") {
                        passesFilter = true;

                        p.stateIndex = 2;
					} else if (p.NameModel.ToLower() == "mic_sbireninja") {
                        passesFilter = true;
                        itemCount = 5;
					} else if (p.NameModel.ToLower() == "sun_clarky") {
                        passesFilter = true;
                        itemCount = 5;
					} else if (p.NameModel.ToLower() == "sun_fayagwod") {
                        passesFilter = true;
                        itemCount = 7;
					} else if (p.NameModel.ToLower() == "xap_tonogear_light" || p.NameModel.ToLower() == "stn_tonogear") {
                        passesFilter = true;
                        itemCount = 3;
					} else if (p.NameModel.ToLower() == "arg_araignee") {
                        passesFilter = true;
                        itemCount = 1;
					} else if (p.NameModel.ToLower() == "arg_simplesbire" || p.NameModel.ToLower() == "old_sbire_shoot_sol") {
                        passesFilter = !invincible;
                        itemCount = 1;
					} else if (p.NameModel.ToLower() == "old_sbire_alarme") {
                        // OLD_SBIRE_alarme (depends on custombits (excludes cb30) dsgvar_1: 0->3, 1->2, 2->4)
						passesFilter = !invincible;

                        if (dsgMem != null) {
                            byte value_1 = dsgMem.values[1].valueUByte;
                            switch (value_1) {
                                case 0:
                                    itemCount = 3;
                                    break;
                                case 1:
                                    itemCount = 2;
                                    break;
                                case 2:
                                    itemCount = 4;
                                    break;
                            }
                        }

					} else if (p.NameModel.ToLower() == "frh_sbire_gnak") {
                        // FRH_Sbire_Gnak -> CB31 = invincible -> depending on CB30 + DsgVar_2: 0->2 lums, !0->3 lums 
						passesFilter = !invincible;

                        if (dsgMem != null) {
                            int value_2 = dsgMem.values[2].valueInt;
							itemCount = value_2 == 0 ? 2 : 3;
						}
                    }

                    if (passesFilter && p.NameModel.ToLower()!="chr_lums_basic"&& p.NameModel.ToLower() != "arg_serierouge" && p.NameModel.ToLower() != "alw_lums_model") {

                        p.stateIndex = 0;
					}

				} else if (string.Equals(HighlightObjectsFilter, "$yellowlums")) {

					DsgMem dsgMem = p.perso?.brain?.mind?.dsgMem;

					if (p.NameModel.ToLower() == "chr_lums_basic") {

						if (dsgMem != null) {
							int lumType = dsgMem.values[0].valueInt;
                            bool isBigLum = dsgMem.values[4].valueBool;

							if (lumType == 1) {
								passesFilter = true;
								itemCount = 1;

                                p.stateIndex = 4;

                                if (isBigLum) {
                                    p.stateIndex = 13;
                                    itemCount = 5;
                                }
							}
						}


					} else if (p.NameModel.ToLower() == "alw_lums_model") {

						if (dsgMem != null) {
							byte lumType = dsgMem.values[0].valueUByte;
                            bool isBigLum = dsgMem.values[4].valueBool;
							if (lumType == 1) {
								passesFilter = true;
								itemCount = 1;

								p.stateIndex = 4;

                                if (isBigLum) {
                                    p.stateIndex = 13;
									itemCount = 5;
                                }
							}
						}


					} else if (p.NameModel.ToLower() == "arg_cagelums") {

                        if (dsgMem != null) {
                            passesFilter = true;
                            itemCount = dsgMem.values[4].valueInt;
						}
					}

                } else {

					foreach (var fi in HighlightObjectsFilter.Split(',')) {
                        if (string.Equals(p.NameFamily, fi, StringComparison.CurrentCultureIgnoreCase) ||
                            string.Equals(p.NameModel, fi, StringComparison.CurrentCultureIgnoreCase)) {
                            passesFilter = true;
                        }
                    }

                }

                if (passesFilter) {

                    count+=itemCount;

                    var highlightTextParentGao = new GameObject("HighlightTextParent");
                    highlightTextParentGao.transform.SetParent(p.gameObject.transform, false);
					var textComponent = Instantiate(ScreenshotHighlightTextPrefab);
                    textComponent.gameObject.transform.SetParent(highlightTextParentGao.transform, false);

                    string text = HighlightObjectsTextFormat;
                    text = text.Replace("\\n", Environment.NewLine);
                    text = text.Replace("$f", p.NameFamily);
					text = text.Replace("$m", p.NameModel);
					text = text.Replace("$i", p.NameInstance);
					text = text.Replace("$c", count.ToString());
					text = text.Replace("$_c", itemCount.ToString());

                    textComponent.text = text;

                    BillboardBehaviour b = highlightTextParentGao.AddComponent<BillboardBehaviour>();
                    b.mode = BillboardBehaviour.LookAtMode.ViewRotation;

                    textComponent.transform.localRotation = Quaternion.Euler(0, -90, 0);

                    BillboardBehaviour billboardPerso = p.gameObject.AddComponent<BillboardBehaviour>();
                    billboardPerso.mode = BillboardBehaviour.LookAtMode.ViewRotation;
                    billboardPerso.scaleMode = BillboardBehaviour.ScaleMode.KeepSize;
                    billboardPerso.RotationOffset = Quaternion.Euler(0, -90, 0);
					billboardPerso.ScaleMultiplier = 1.0f / 15.0f;

                    if (ScreenshotAfterLoad == UnitySettings.ScreenshotAfterLoadSetting.None) {
                        LayerUtil.SetLayerRecursive(p.gameObject, LayerMask.NameToLayer("HighlightAlwaysOnTop"));
                    } else {
                        LayerUtil.SetLayerRecursive(p.gameObject, LayerMask.NameToLayer("Default"));
                        LayerUtil.SetLayerRecursive(highlightTextParentGao, LayerMask.NameToLayer("Default"));
						billboardPerso.ForceCloseToOrthoCamera = true;
                    }
                }
            }
        }

        if (ScreenshotAfterLoad!=UnitySettings.ScreenshotAfterLoadSetting.None) {

            CreateScreenshotAfterLoad(false);

			IEnumerator ScreenshotCoroutine()
            {
                yield return new WaitForSeconds(1);
                CreateScreenshotAfterLoad(true);
			}

            StartCoroutine(ScreenshotCoroutine());
        }

		// Collect searchable strings
		if (Application.platform != RuntimePlatform.WebGLPlayer) {
			loader.searchableStrings.AddRange(loader.persos.SelectMany(p => p.GetSearchableStrings()));
		}

    }

    private async Task CreateScreenshotAfterLoad(bool save)
    {
        Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
        System.DateTime dateTime = System.DateTime.Now;
        TransparencyCaptureBehaviour pb = new GameObject("Dummy").AddComponent<TransparencyCaptureBehaviour>();
        lightManager.enableFog = false;
        Camera.main.orthographic = true;

        var filledSectors = sectorManager.sectors.Where(s => s.sector?.SuperObject?.children?.Count > 0 ? true : false);

        float minX = filledSectors.Min(v => v.SectorBorder.boxMin.x);
        float minY = filledSectors.Min(v => v.SectorBorder.boxMin.y);
        float minZ = filledSectors.Min(v => v.SectorBorder.boxMin.z);

        float maxX = filledSectors.Max(v => v.SectorBorder.boxMax.x);
        float maxY = filledSectors.Max(v => v.SectorBorder.boxMax.y);
        float maxZ = filledSectors.Max(v => v.SectorBorder.boxMax.z);

        Vector3 worldMin = new Vector3(minX, minY, minZ);
        Vector3 worldMax = new Vector3(maxX, maxY, maxZ);

        Vector3 worldSize = (worldMax - worldMin);
        Vector3 center = worldMin + worldSize * 0.5f;

        sectorManager.displayInactiveSectors = true;
        lightManager.luminosity = Settings.s.luminosity * 2.0f;
        SpawnableParent?.SetActive(false);

        byte[] screenshotBytes;

        if (ScreenshotAfterLoad == UnitySettings.ScreenshotAfterLoadSetting.TopDownAndOrthographic ||
            ScreenshotAfterLoad == UnitySettings.ScreenshotAfterLoadSetting.TopDownOnly) {
            Camera.main.transform.position = center + new Vector3(0, Camera.main.farClipPlane * 0.5f, 0);
            Camera.main.transform.rotation = Quaternion.Euler(90, worldSize.z <= worldSize.x ? 90 : 0, 0);

            Camera.main.orthographicSize = (worldSize.z > worldSize.x ? worldSize.z : worldSize.x) * 0.5f;

            if (save) {
                screenshotBytes =
                    await pb.Capture((int) (res.width * ScreenshotScale), (int) (res.height * ScreenshotScale), true);
                OpenSpace.Util.ByteArrayToFile(
                    UnitySettings.ScreenshotPath + "/" + loader.lvlName + "_top_" +
                    dateTime.ToString("yyyy_MM_dd HH_mm_ss") +
                    ".png", screenshotBytes);
            }
        }

        if (ScreenshotAfterLoad == UnitySettings.ScreenshotAfterLoadSetting.TopDownAndOrthographic ||
            ScreenshotAfterLoad == UnitySettings.ScreenshotAfterLoadSetting.OrthographicOnly) {
            var pitch = Mathf.Rad2Deg * Mathf.Atan(Mathf.Sin(Mathf.Deg2Rad * 45));
            for (int i = 0; i < 360; i += 45) {
                Camera.main.transform.rotation = Quaternion.Euler(pitch, i, 0);
                Camera.main.transform.position =
                    center - Camera.main.transform.rotation * Vector3.forward * Camera.main.farClipPlane * 0.5f;

                Camera.main.orthographicSize = (worldSize.x + worldSize.y + worldSize.z) / 6.0f;

                if (save) {
                    screenshotBytes = await pb.Capture((int) (res.width * ScreenshotScale),
                        (int) (res.height * ScreenshotScale), true);
                    OpenSpace.Util.ByteArrayToFile(
                        UnitySettings.ScreenshotPath + "/" + loader.lvlName + "_iso_" + i +
                        dateTime.ToString("yyyy_MM_dd HH_mm_ss") + ".png", screenshotBytes);
                }
            }
        }

        Destroy(pb.gameObject);

        if (save) {
            Application.Quit();
        }
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
		if (Camera.main.orthographic) {
			if(SpawnableParent != null) SpawnableParent.SetActive(false);
			if(PersoPartsParent != null) PersoPartsParent.SetActive(false);
		} else {
			if (SpawnableParent != null) SpawnableParent.SetActive(true);
			if (PersoPartsParent != null) PersoPartsParent.SetActive(true);
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

	async UniTask InitPersos() {
		if (loader != null) {
			for (int i = 0; i < loader.persos.Count; i++) {
				detailedState = "Initializing persos: " + i + "/" + loader.persos.Count;
				await WaitIfNecessary();
				Perso p = loader.persos[i];
				PersoBehaviour unityBehaviour = p.Gao.AddComponent<PersoBehaviour>();
				persos.Add(unityBehaviour);
				unityBehaviour.controller = this;
				if (loader.globals != null && loader.globals.spawnablePersos != null) {
					if (loader.globals.spawnablePersos.IndexOf(p) > -1) {
						unityBehaviour.IsAlways = true;
						unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
					}
				}
				if (!unityBehaviour.IsAlways) {
					if (p.sectInfo != null && p.sectInfo.off_sector != null) {
						unityBehaviour.sector = sectorManager.sectors.FirstOrDefault(s => s.sector != null && s.sector.SuperObject.offset == p.sectInfo.off_sector);
					} else {
						SectorComponent sc = sectorManager.GetActiveSectorWrapper(p.Gao.transform.position);
						unityBehaviour.sector = sc;
					}
				} else unityBehaviour.sector = null;
				unityBehaviour.perso = p;
				unityBehaviour.Init();
			}
			// Initialize AI stuff (brain, scripts, DSGVars) after all persos have their perso behaviours
			for (int i = 0; i < persos.Count; i++) {
				PersoBehaviour unityBehaviour = persos[i];
				Perso p = unityBehaviour.perso;
				Moddable mod = null;
				if (p.SuperObject != null && p.SuperObject.Gao != null) {
					mod = p.SuperObject.Gao.GetComponent<Moddable>();
					if (mod != null) {
						mod.persoBehaviour = p.Gao.GetComponent<PersoBehaviour>();
					}
				}
				if (p.Gao && p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null) {
					// Brain
					BrainComponent brain = unityBehaviour.gameObject.AddComponent<BrainComponent>();
					unityBehaviour.brain = brain;
					brain.perso = unityBehaviour;

					// Scripts
					if (p.brain.mind.AI_model.behaviors_normal != null) {
						GameObject intelParent = new GameObject("Rule behaviours");
						intelParent.transform.parent = p.Gao.transform;
						Behavior[] normalBehaviors = p.brain.mind.AI_model.behaviors_normal;
						int iter = 0;
						foreach (Behavior behavior in normalBehaviors) {
							BrainComponent.Comport c = new BrainComponent.Comport();
							string shortName = behavior.GetShortName(p.brain.mind.AI_model, Behavior.BehaviorType.Intelligence, iter);
							GameObject behaviorGao = new GameObject(shortName);
							behaviorGao.transform.parent = intelParent.transform;
							foreach (Script script in behavior.scripts) {
								GameObject scriptGao = new GameObject("Script");
								scriptGao.transform.parent = behaviorGao.transform;
								ScriptComponent scriptComponent = scriptGao.AddComponent<ScriptComponent>();
								scriptComponent.SetScript(script, p);
								c.Scripts.Add(scriptComponent);
							}
							if (behavior.scheduleScript != null) {
								ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
								scriptComponent.SetScript(behavior.scheduleScript, p);
								c.FirstScript = scriptComponent;
							}
							if (iter == 0) {
								behaviorGao.name += " (Init)";
							}
							if ((behavior.scripts == null || behavior.scripts.Length == 0) && behavior.scheduleScript == null) {
								behaviorGao.name += " (Empty)";
							}
							c.Offset = behavior.Offset;
							c.GaoName = behaviorGao.name;
							c.Name = behavior.NameSubstring;
							brain.Intelligence.Add(c);
							iter++;
						}
					}
					if (p.brain.mind.AI_model.behaviors_reflex != null) {
						GameObject reflexParent = new GameObject("Reflex behaviours");
						reflexParent.transform.parent = p.Gao.transform;
						Behavior[] reflexBehaviors = p.brain.mind.AI_model.behaviors_reflex;
						int iter = 0;
						foreach (Behavior behavior in reflexBehaviors) {
							BrainComponent.Comport c = new BrainComponent.Comport();
							string shortName = behavior.GetShortName(p.brain.mind.AI_model, Behavior.BehaviorType.Reflex, iter);
							GameObject behaviorGao = new GameObject(shortName);
							behaviorGao.transform.parent = reflexParent.transform;
							foreach (Script script in behavior.scripts) {
								GameObject scriptGao = new GameObject("Script");
								scriptGao.transform.parent = behaviorGao.transform;
								ScriptComponent scriptComponent = scriptGao.AddComponent<ScriptComponent>();
								scriptComponent.SetScript(script, p);
								c.Scripts.Add(scriptComponent);
							}
							if (behavior.scheduleScript != null) {
								ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
								scriptComponent.SetScript(behavior.scheduleScript, p);
								c.FirstScript = scriptComponent;
							}
							if ((behavior.scripts == null || behavior.scripts.Length == 0) && behavior.scheduleScript == null) {
								behaviorGao.name += " (Empty)";
							}
							c.Offset = behavior.Offset;
							c.GaoName = behaviorGao.name;
							c.Name = behavior.NameSubstring;
							brain.Reflex.Add(c);
							iter++;
						}
					}
					if (p.brain.mind.AI_model.macros != null) {
						GameObject macroParent = new GameObject("Macros");
						macroParent.transform.parent = p.Gao.transform;
						Macro[] macros = p.brain.mind.AI_model.macros;
						int iter = 0;

						foreach (Macro macro in macros) {
							GameObject behaviorGao = new GameObject(macro.GetShortName(p.brain.mind.AI_model, iter));
							behaviorGao.transform.parent = macroParent.transform;
							ScriptComponent scriptComponent = behaviorGao.AddComponent<ScriptComponent>();
							scriptComponent.SetScript(macro.script, p);
							brain.Macros.Add(new BrainComponent.Macro() {
								Offset = macro.Offset,
								GaoName = behaviorGao.name,
								Name = macro.NameSubstring,
								Script = scriptComponent
							});
							iter++;
						}
					}

					// DsgVars
					if (p.brain.mind.dsgMem != null || p.brain.mind.AI_model.dsgVar != null) {
						DsgVarComponent dsgVarComponent = p.Gao.AddComponent<DsgVarComponent>();
						brain.dsgVars = dsgVarComponent;
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
						brain.mind = mindComponent;
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
		if (loader is OpenSpace.Loader.R2ROMLoader) {
			OpenSpace.Loader.R2ROMLoader romLoader = loader as OpenSpace.Loader.R2ROMLoader;
			if (romPersos.Count > 0) {
				for (int i = 0; i < romPersos.Count; i++) {
					detailedState = "Initializing persos: " + i + "/" + romPersos.Count;
					await WaitIfNecessary();
					ROMPersoBehaviour unityBehaviour = romPersos[i];
					unityBehaviour.controller = this;
					/*if (loader.globals != null && loader.globals.spawnablePersos != null) {
						if (loader.globals.spawnablePersos.IndexOf(p) > -1) {
							unityBehaviour.IsAlways = true;
							unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
						}
					}*/
					if (!unityBehaviour.IsAlways) {
						SectorComponent sc = sectorManager.GetActiveSectorWrapper(unityBehaviour.transform.position);
						unityBehaviour.sector = sc;
					} else unityBehaviour.sector = null;
					/*Moddable mod = null;
					if (p.SuperObject != null && p.SuperObject.Gao != null) {
						mod = p.SuperObject.Gao.GetComponent<Moddable>();
						if (mod != null) {
							mod.persoBehaviour = unityBehaviour;
						}
					}*/
					unityBehaviour.Init();

					var iteratorPerso = unityBehaviour.perso;

					// Of sound brain and AI model?
					if (iteratorPerso.brain?.Value?.aiModel?.Value != null) {
						var aiModel = iteratorPerso.brain.Value.aiModel.Value;

						// Brain
						BrainComponent brain = unityBehaviour.gameObject.AddComponent<BrainComponent>();
						unityBehaviour.brain = brain;
						brain.perso = unityBehaviour;

						// DsgVars
						if (iteratorPerso.brain?.Value?.dsgMem?.Value != null || aiModel.dsgVar?.Value != null) {
							DsgVarComponent dsgVarComponent = unityBehaviour.gameObject.AddComponent<DsgVarComponent>();
							brain.dsgVars = dsgVarComponent;
							dsgVarComponent.SetPerso(iteratorPerso);
						}

						// Comports
						if (aiModel.comportsIntelligence.Value != null) {
							aiModel.comportsIntelligence.Value.CreateGameObjects(Behavior.BehaviorType.Intelligence, brain, iteratorPerso);
						}
						if (aiModel.comportsReflex.Value != null) {
							aiModel.comportsReflex.Value.CreateGameObjects(Behavior.BehaviorType.Reflex, brain, iteratorPerso);
						}
					}
				}
			}
			if (romLoader.level != null && romLoader.level.spawnablePersos.Value != null && romLoader.level.num_spawnablepersos > 0) {
				SpawnableParent = new GameObject("Spawnable persos");
				for (int i = 0; i < romLoader.level.num_spawnablepersos; i++) {
					detailedState = "Initializing spawnable persos: " + i + "/" + romLoader.level.num_spawnablepersos;
					await WaitIfNecessary();
					OpenSpace.ROM.SuperObjectDynamic sod = romLoader.level.spawnablePersos.Value.superObjects[i];
					GameObject sodGao = sod.GetGameObject();
					if (sodGao != null) {
						ROMPersoBehaviour unityBehaviour = sodGao.GetComponent<ROMPersoBehaviour>();
						unityBehaviour.controller = this;
						unityBehaviour.IsAlways = true;
						unityBehaviour.transform.SetParent(SpawnableParent.transform);
						unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
						unityBehaviour.transform.rotation = Quaternion.identity;
						unityBehaviour.transform.localScale = Vector3.one;
						if (!unityBehaviour.IsAlways) {
							SectorComponent sc = sectorManager.GetActiveSectorWrapper(unityBehaviour.transform.position);
							unityBehaviour.sector = sc;
						} else unityBehaviour.sector = null;
						unityBehaviour.Init();

						var iteratorPerso = unityBehaviour.perso;

						// Of sound brain and AI model?
						if (iteratorPerso.brain?.Value?.aiModel?.Value != null) {
							var aiModel = iteratorPerso.brain.Value.aiModel.Value;

							// Brain
							BrainComponent brain = unityBehaviour.gameObject.AddComponent<BrainComponent>();
							unityBehaviour.brain = brain;
							brain.perso = unityBehaviour;

							// DsgVars
							if (iteratorPerso.brain?.Value?.dsgMem?.Value != null || aiModel.dsgVar?.Value != null) {
								DsgVarComponent dsgVarComponent = unityBehaviour.gameObject.AddComponent<DsgVarComponent>();
								brain.dsgVars = dsgVarComponent;
								dsgVarComponent.SetPerso(iteratorPerso);
							}

							// Comports
							if (aiModel.comportsIntelligence.Value != null) {
								aiModel.comportsIntelligence.Value.CreateGameObjects(Behavior.BehaviorType.Intelligence, brain, iteratorPerso);
							}
							if (aiModel.comportsReflex.Value != null) {
								aiModel.comportsReflex.Value.CreateGameObjects(Behavior.BehaviorType.Reflex, brain, iteratorPerso);
							}
						}
					}
				}
			}
		}
		if (loader is OpenSpace.Loader.R2PS1Loader) {
			OpenSpace.Loader.R2PS1Loader romLoader = loader as OpenSpace.Loader.R2PS1Loader;
			if (ps1Persos.Count > 0) {
				for (int i = 0; i < ps1Persos.Count; i++) {
					detailedState = "Initializing persos: " + i + "/" + ps1Persos.Count;
					await WaitIfNecessary();
					PS1PersoBehaviour unityBehaviour = ps1Persos[i];
					unityBehaviour.controller = this;
					if (romLoader.levelHeader?.always != null) {
						foreach(var list in romLoader.levelHeader.always) {
							foreach (var item in list.items) {
								if (item.superObject == unityBehaviour.superObject) {
									unityBehaviour.IsAlways = true;
									break;
								}
							}
							if (unityBehaviour.IsAlways) break;
						}
					}
					/*if (loader.globals != null && loader.globals.spawnablePersos != null) {
						if (loader.globals.spawnablePersos.IndexOf(p) > -1) {
							unityBehaviour.IsAlways = true;
							unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
						}
					}*/
					if (!unityBehaviour.IsAlways) {
						SectorComponent sc = sectorManager.GetActiveSectorWrapper(unityBehaviour.transform.position);
						unityBehaviour.sector = sc;
					} else unityBehaviour.sector = null;
					/*Moddable mod = null;
					if (p.SuperObject != null && p.SuperObject.Gao != null) {
						mod = p.SuperObject.Gao.GetComponent<Moddable>();
						if (mod != null) {
							mod.persoBehaviour = unityBehaviour;
						}
					}*/
					unityBehaviour.Init();

					var iteratorPerso = unityBehaviour.perso;

					// Of sound brain and AI model?
					/*if (iteratorPerso.brain?.Value?.aiModel?.Value != null) {
						var aiModel = iteratorPerso.brain.Value.aiModel.Value;

						// DsgVars
						if (iteratorPerso.brain?.Value?.dsgMem?.Value != null || aiModel.dsgVar?.Value != null) {
							DsgVarComponent dsgVarComponent = unityBehaviour.gameObject.AddComponent<DsgVarComponent>();
							dsgVarComponent.SetPerso(iteratorPerso);
						}

						// Comports
						if (aiModel.comportsIntelligence.Value != null) {
							aiModel.comportsIntelligence.Value.CreateGameObjects("Rule", unityBehaviour.gameObject, iteratorPerso);
						}
						if (aiModel.comportsReflex.Value != null) {
							aiModel.comportsReflex.Value.CreateGameObjects("Reflex", unityBehaviour.gameObject, iteratorPerso);
						}
					}*/
				}
			}
			/*if (romLoader.level != null && romLoader.level.spawnablePersos.Value != null && romLoader.level.num_spawnablepersos > 0) {
				spawnableParent = new GameObject("Spawnable persos");
				for (int i = 0; i < romLoader.level.num_spawnablepersos; i++) {
					detailedState = "Initializing spawnable persos: " + i + "/" + romLoader.level.num_spawnablepersos;
					await WaitIfNecessary();
					OpenSpace.ROM.SuperObjectDynamic sod = romLoader.level.spawnablePersos.Value.superObjects[i];
					GameObject sodGao = sod.GetGameObject();
					if (sodGao != null) {
						ROMPersoBehaviour unityBehaviour = sodGao.GetComponent<ROMPersoBehaviour>();
						unityBehaviour.controller = this;
						unityBehaviour.IsAlways = true;
						unityBehaviour.transform.SetParent(spawnableParent.transform);
						unityBehaviour.transform.position = new Vector3(i * 10, -1000, 0);
						unityBehaviour.transform.rotation = Quaternion.identity;
						unityBehaviour.transform.localScale = Vector3.one;
						if (!unityBehaviour.IsAlways) {
							SectorComponent sc = sectorManager.GetActiveSectorWrapper(unityBehaviour.transform.position);
							unityBehaviour.sector = sc;
						} else unityBehaviour.sector = null;
						unityBehaviour.Init();

						var iteratorPerso = unityBehaviour.perso;

						// Of sound brain and AI model?
						if (iteratorPerso.brain?.Value?.aiModel?.Value != null) {
							var aiModel = iteratorPerso.brain.Value.aiModel.Value;

							// DsgVars
							if (iteratorPerso.brain?.Value?.dsgMem?.Value != null || aiModel.dsgVar?.Value != null) {
								DsgVarComponent dsgVarComponent = unityBehaviour.gameObject.AddComponent<DsgVarComponent>();
								dsgVarComponent.SetPerso(iteratorPerso);
							}

							// Comports
							if (aiModel.comportsIntelligence.Value != null) {
								aiModel.comportsIntelligence.Value.CreateGameObjects("Rule", unityBehaviour.gameObject, iteratorPerso);
							}
							if (aiModel.comportsReflex.Value != null) {
								aiModel.comportsReflex.Value.CreateGameObjects("Reflex", unityBehaviour.gameObject, iteratorPerso);
							}
						}
					}
				}
			}*/
		}
	}

	public void InitCamera() {
		if (loader != null) {
			if (loader is OpenSpace.Loader.R2ROMLoader) {
				ROMPersoBehaviour camera = romPersos.FirstOrDefault(p => p.perso.camera != null);
				if (camera != null) {
					Camera.main.transform.position = camera.transform.position;
					Camera.main.transform.rotation = camera.transform.rotation * Quaternion.Euler(0, 180, 0);
				}
			} else {
				Perso camera = loader.persos.FirstOrDefault(p => p != null && p.namePerso.Equals("StdCamer"));
				if (camera == null && loader.globals != null && loader.globals.off_camera != null) {
					camera = loader.persos.FirstOrDefault(p => p != null && p.stdGame != null && p.stdGame.off_superobject == loader.globals.off_camera);
				}
				if (camera != null) {
					Camera.main.transform.position = camera.Gao.transform.position;
					Camera.main.transform.rotation = camera.Gao.transform.rotation * Quaternion.Euler(0, 180, 0);
				}
			}
		}
		CameraSettings[WebJSON.CameraPos.Initial] = communicator.GetCameraJSON(includeTransform: true);

		// Calculate world size
		IEnumerable<SectorComponent> filledSectors;
		switch (loader) {
			case R2PS1Loader pl:
				filledSectors = sectorManager.sectors.TakeAllButLast();
				break;
			case R2ROMLoader rl:
				filledSectors = sectorManager.sectors.TakeAllButLast().Where(s => s.SectorBorder != null);
				break;
			default:
				filledSectors = sectorManager.sectors.Where(s => s.sector?.SuperObject?.children?.Count > 0 && s.SectorBorder != null);
				break;
		}
		Vector3 center;
		float orthoSize;
		if (filledSectors.Count() == 0) {
			center = Vector3.zero;
			orthoSize = 100f / 6.0f;
		} else {
			float minX = filledSectors.Min(v => v.SectorBorder.boxMin.x);
			float minY = filledSectors.Min(v => v.SectorBorder.boxMin.y);
			float minZ = filledSectors.Min(v => v.SectorBorder.boxMin.z);

			float maxX = filledSectors.Max(v => v.SectorBorder.boxMax.x);
			float maxY = filledSectors.Max(v => v.SectorBorder.boxMax.y);
			float maxZ = filledSectors.Max(v => v.SectorBorder.boxMax.z);

			Vector3 worldMin = new Vector3(minX, minY, minZ);
			Vector3 worldMax = new Vector3(maxX, maxY, maxZ);

			Vector3 worldSize = (worldMax - worldMin);
			center = worldMin + worldSize * 0.5f;
			orthoSize = (worldSize.x + worldSize.y + worldSize.z) / 6.0f;
		}

		for (int i = 0; i < 4; i++) {
			Quaternion rot = Quaternion.Euler(0, (90 * i), 0);
			CameraSettings[WebJSON.CameraPos.Front + i] = new WebJSON.CameraSettings() {
				IsOrthographic = true,
				OrthographicSize = orthoSize,
				Position = center - rot * Vector3.forward * Camera.main.farClipPlane * 0.5f,
				Rotation = rot.eulerAngles
			};
		}

		var pitch = Mathf.Rad2Deg * Mathf.Atan(Mathf.Sin(Mathf.Deg2Rad * 45));
		for (int i = 0; i < 4; i++) {
			Vector3 rot = new Vector3(pitch, 45 + (90 * i), 0);
			CameraSettings[WebJSON.CameraPos.IsometricFront + i] = new WebJSON.CameraSettings() {
				IsOrthographic = true,
				OrthographicSize = orthoSize,
				Position = center - Quaternion.Euler(rot) * Vector3.forward * Camera.main.farClipPlane * 0.5f,
				Rotation = rot
			};
		}

		for (int i = 0; i < 2; i++) {
			Vector3 rot = new Vector3(90 - 180 * i, 0, 0); ;
			CameraSettings[WebJSON.CameraPos.Top + i] = new WebJSON.CameraSettings() {
				IsOrthographic = true,
				OrthographicSize = orthoSize,
				Position = center - Quaternion.Euler(rot) * Vector3.forward * Camera.main.farClipPlane * 0.5f,
				Rotation = rot
			};
		}
	}

	public void SetCameraPosition(WebJSON.CameraPos cameraPos) {
		if (CameraSettings.ContainsKey(cameraPos)) {
			ApplyCameraSettings(CameraSettings[cameraPos], applyCameraPos: false);
			CameraComponent cc = Camera.main.GetComponent<CameraComponent>();
			if (cc != null) cc.UpdateDebugCameraPos(cameraPos);
			communicator.SendChangedCameraMode(cameraPos);
		}
	}

	public void ApplyCameraSettings(WebJSON.CameraSettings camSettings, bool applyCameraPos = true, bool applyTransform = true) {
		if (camSettings == null) return;
		Camera c = Camera.main;
		if (camSettings.ClipFar.HasValue) c.farClipPlane = camSettings.ClipFar.Value;
		if (camSettings.ClipNear.HasValue) c.nearClipPlane = camSettings.ClipNear.Value;
		if (camSettings.FieldOfView.HasValue) c.fieldOfView = camSettings.FieldOfView.Value;
		if (camSettings.IsOrthographic.HasValue) c.orthographic = camSettings.IsOrthographic.Value;
		if (camSettings.OrthographicSize.HasValue) c.orthographicSize = camSettings.OrthographicSize.Value;
		if (applyCameraPos) {
			if (camSettings.CameraPos.HasValue) {
				SetCameraPosition(camSettings.CameraPos.Value);
			}
		}
		if (applyTransform) {
			if (camSettings.Position.HasValue) c.transform.localPosition = camSettings.Position.Value;
			if (camSettings.Rotation.HasValue) c.transform.localEulerAngles = camSettings.Rotation.Value;
		}
		CameraComponent cc = c.GetComponent<CameraComponent>();
		if(cc != null) cc.StopLerp();
	}

	public void UpdateViewGraphs() {
		if (loader != null) {
			viewGraphs_ = viewGraphs;
			graphManager.UpdateViewGraphs();
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
			foreach (PS1PersoBehaviour perso in ps1Persos) {
				UpdatePersoActive(perso);
			}
			foreach (ROMPersoBehaviour perso in romPersos) {
				UpdatePersoActive(perso);
			}
		}
	}

	public void UpdatePersoActive(Perso perso) {
		if (perso != null && perso.Gao != null) {
			PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
			UpdatePersoActive(pb);
		}
	}

	public void UpdatePersoActive(BasePersoBehaviour perso) {
		if (perso != null) {
			bool isVisible = true;
			switch (perso) {
				case PersoBehaviour pb:
					if (pb.perso?.SuperObject != null) {
						isVisible = !pb.perso.SuperObject.flags.HasFlag(SuperObjectFlags.Flags.Invisible);
					}
					break;
				case ROMPersoBehaviour rpb:
					break;
				case PS1PersoBehaviour ps1pb:
					break;
			}
			perso.gameObject.SetActive(showPersos && perso.IsEnabled && (isVisible || (viewCollision || viewInvisible)));
		}
	}

	public void UpdateViewCollision() {
		if (loader != null) {
			viewCollision_ = viewCollision;
			foreach (PhysicalObject po in loader.physicalObjects) {
				if (po != null) po.UpdateViewCollision(viewCollision);
			}
			foreach (Perso perso in loader.persos) {
				if (perso != null && perso.Gao != null) {
					PersoBehaviour pb = perso.Gao.GetComponent<PersoBehaviour>();
                    if (pb != null) {
                        pb.UpdateViewCollision(viewCollision);
                    }
                }
			}
			foreach (ROMPersoBehaviour perso in romPersos) {
				if (perso != null) { perso.UpdateViewCollision(viewCollision); }
			}
			foreach (PS1PersoBehaviour perso in ps1Persos) {
				if (perso != null) { perso.UpdateViewCollision(viewCollision); }
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
			if (loader is OpenSpace.Loader.R2ROMLoader) {
				PhysicalObjectComponent[] pos = FindObjectsOfType<PhysicalObjectComponent>();
				foreach (PhysicalObjectComponent po in pos) {
					po.UpdateViewCollision(viewCollision);
				}
			}
		}
	}

	public void InitCinematics() {
		if (loader.cinematicsManager != null && loader.cinematicsManager.cinematics.Count > 0) {
			CinematicSwitcher = new GameObject("Cinematic Switcher").AddComponent<CinematicSwitcher>();
			CinematicSwitcher.Init();
		}
		if (loader is R2PS1Loader && (loader as R2PS1Loader).streams.Length > 0) {
			CinematicSwitcher = new GameObject("Cinematic Switcher").AddComponent<CinematicSwitcher>();
			CinematicSwitcher.Init();
		}
	}

	public void UpdateLivePreview() {
		if (loader == null || LoadState != State.Finished) return;
		Reader reader = MapLoader.Loader.livePreviewReader;

		foreach (SuperObject so in MapLoader.Loader.superObjects) {

			if (!(so.data is Perso)) {
				continue;
			}

			if (so.off_matrix == null) {
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
						Pointer.DoAt(ref reader, mind.Offset, () => {
							mind.UpdateCurrentBehaviors(reader);
						});
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

					DynamicsMechanicsComponent dnComponent = perso.Gao.GetComponent<DynamicsMechanicsComponent>();
					if (dnComponent != null) {
						Pointer.DoAt(ref reader, perso.off_dynam, () => {
							perso.dynam = Dynam.Read(reader, perso.off_dynam);
						});

						dnComponent.SetDynamics(perso.dynam.dynamics);
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
					if (condition.Contains("desc.isValid() failed!")) break;

					// Go to error state
					state = State.Error;
					if (loadingScreen.Active) {
						detailedState = condition;
					}
				}
				break;
		}
	}
	public async UniTask WaitFrame() {
		await UniTask.WaitForEndOfFrame();
		if (stopwatch.IsRunning) {
			stopwatch.Restart();
		}
	}
	public async UniTask WaitIfNecessary() {
		if (stopwatch.ElapsedMilliseconds > 16) {
			await WaitFrame();
		}
	}
}
