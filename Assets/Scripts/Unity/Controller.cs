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

public class Controller : MonoBehaviour {
    public Settings.Mode mode = Settings.Mode.Rayman3PC;
    public string gameDataBinFolder;
    public string lvlName;

    public Material baseMaterial;
    public Material baseTransparentMaterial;
    public Material baseLightMaterial;
    public Material collideMaterial;
    public Material collideTransparentMaterial;
    public Material billboardMaterial;
    public Material billboardAdditiveMaterial;
    public SectorManager sectorManager;
    public LightManager lightManager;
    public LoadingScreen loadingScreen;
    public bool allowDeadPointers = false;
    public bool forceDisplayBackfaces = false;
    public bool blockyMode = false;
    MapLoader loader = null;
    bool viewCollision_ = false;
    public bool viewCollision = false;

    public enum State {
        None,
        Downloading,
        Loading,
        Initializing,
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
        if (Application.platform == RuntimePlatform.WebGLPlayer) {
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
            case "dd_pc":
                mode = Settings.Mode.DonaldDuckPC; break;
            case "tt_pc":
                mode = Settings.Mode.TonicTroublePC; break;
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
        loader.billboardMaterial = billboardMaterial;
        loader.billboardAdditiveMaterial = billboardAdditiveMaterial;

        /*if (Application.platform == RuntimePlatform.WebGLPlayer) { // || (Application.isEditor && UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)) {
            StartCoroutine(DownloadInit());
        } else {*/
        StartCoroutine(Init());
        //}
    }

    /*IEnumerator DownloadInit() {
        state = State.Downloading;
        string[] requiredFiles = {
            "FIX.DAT",
            "FIX.TEX",
            lvlName + "/" + lvlName + ".DAT",
            lvlName + "/" + lvlName + ".TEX",
            "TEXTS/ENGLISH.LNG",
            "TEXTS/FRENCH.LNG",
            "TEXTS/GERMAN.LNG",
            "TEXTS/ITALIAN.LNG",
            "TEXTS/SPANISH.LNG",
        };
        foreach (string file in requiredFiles) {
            detailedState = "Downloading file: " + file;
            string path = loader.gameDataBinFolder + "/" + file;
            yield return FileSystem.DownloadFile(path);
        }
        yield return StartCoroutine(Init());
    }*/

    IEnumerator Init() {
        state = State.Loading;
        yield return StartCoroutine(loader.Load());
        state = State.Initializing;
        detailedState = "Initializing sectors";
        sectorManager.Init();
        detailedState = "Initializing lights";
        lightManager.Init();
        detailedState = "Initializing persos";
        yield return StartCoroutine(InitPersos());
        detailedState = "Initializing camera";
        InitCamera();
        /*if (viewCollision)*/ UpdateViewCollision();
        detailedState = "Finished";
        state = State.Finished;
        loadingScreen.Active = false;
    }

	// Update is called once per frame
	void Update () {
        if (loadingScreen.Active) {
            if (state == State.Loading) {
                loadingScreen.LoadingText = loader.loadingState;
            } else {
                loadingScreen.LoadingText = detailedState;
            }
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            viewCollision = !viewCollision;
        }
        if (loader != null && viewCollision != viewCollision_) {
            UpdateViewCollision();
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
                if (p.sectInfo != null && p.sectInfo.off_sector != null) {
                    unityBehaviour.sector = Sector.FromSuperObjectOffset(p.sectInfo.off_sector);
                } else {
                    unityBehaviour.sector = sectorManager.GetActiveSectorsAtPoint(p.Gao.transform.position).FirstOrDefault();
                }
                Moddable mod = null;
                if (p.SuperObject!=null && p.SuperObject.Gao!=null) {
                    mod = p.SuperObject.Gao.GetComponent<Moddable>();
                    if (mod!=null) {
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
                                string name = behavior.name;
                                if (name.Contains("^CreateComport:")) {
                                    name = name.Substring(name.IndexOf("^CreateComport") + 15); 
                                }
                                GameObject behaviorGao = new GameObject(name);
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
                                string name = behavior.name;
                                if (name.Contains("^CreateComport:")) {
                                    name = name.Substring(name.IndexOf("^CreateComport:") + 15);
                                }
                                GameObject behaviorGao = new GameObject(name);
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
                                GameObject behaviorGao = new GameObject(macro.name);
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
            Perso camera = loader.persos.Where(p => p != null && p.namePerso.Equals("StdCamer")).FirstOrDefault();
            if (camera != null) {
                Camera.main.transform.position = camera.Gao.transform.position;
                Camera.main.transform.rotation = camera.Gao.transform.rotation * Quaternion.Euler(0,180,0);
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
                if (po.collideMesh != null) po.collideMesh.gao.SetActive(viewCollision);
            }
            foreach (Perso perso in loader.persos) {
                if (perso.collset != null) {
                    CollSet c = perso.collset;
                    if (c.zdd != null) foreach (CollideMeshObject col in c.zdd) {
                            if (col == null) continue;
                            col.gao.SetActive(viewCollision);
                        }
                    if (c.zde != null) foreach (CollideMeshObject col in c.zde) {
                            if (col == null) continue;
                            col.gao.SetActive(viewCollision);
                        }
                    if (c.zdm != null) foreach (CollideMeshObject col in c.zdm) {
                            if (col == null) continue;
                            col.gao.SetActive(viewCollision);
                        }
                    if (c.zdr != null) foreach (CollideMeshObject col in c.zdr) {
                            if (col == null) continue;
                            col.gao.SetActive(viewCollision);
                        }
                }
            }
            foreach (SuperObject so in loader.superObjects) {
                if (so.Gao != null) {
                    if (so.flags.HasFlag(OpenSpace.Object.Properties.SuperObjectFlags.Flags.Invisible)) {
                        so.Gao.SetActive(viewCollision);
                    }
                }
            }
        }
    }

    public void SaveChanges() {
        if(loader != null) loader.Save();
    }
}
