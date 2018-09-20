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

public class Controller : MonoBehaviour {
    public MapLoader.Mode mode = MapLoader.Mode.Rayman3PC;
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
    public bool allowDeadPointers = false;
    public bool forceDisplayBackfaces = false;
    public bool blockyMode = false;
    MapLoader loader = null;
    bool viewCollision_ = false;
    public bool viewCollision = false;

    // Use this for initialization
    void Start() {
        // Read command line arguments
        string[] args = System.Environment.GetCommandLineArgs();
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
                    switch (args[i + 1]) {
                        case "r3_gc":
                            mode = MapLoader.Mode.Rayman3GC; break;
                        case "ra_gc":
                            mode = MapLoader.Mode.RaymanArenaGC; break;
                        case "r3_pc":
                            mode = MapLoader.Mode.Rayman3PC; break;
                        case "ra_pc":
                            mode = MapLoader.Mode.RaymanArenaPC; break;
                        case "r2_pc":
                            mode = MapLoader.Mode.Rayman2PC; break;
                        case "r2_ios":
                            mode = MapLoader.Mode.Rayman2IOS; break;
                        case "dd_pc":
                            mode = MapLoader.Mode.DonaldDuckPC; break;
                        case "tt_pc":
                            mode = MapLoader.Mode.TonicTroublePC; break;
                        case "r2_demo1_pc":
                            mode = MapLoader.Mode.Rayman2PCDemo1; break;
                        case "r2_demo2_pc":
                            mode = MapLoader.Mode.Rayman2PCDemo2; break;
                        case "playmobil_hype_pc":
                            mode = MapLoader.Mode.PlaymobilHypePC; break;
                        case "playmobil_alex_pc":
                            mode = MapLoader.Mode.PlaymobilAlexPC; break;
                        case "playmobil_laura_pc":
                            mode = MapLoader.Mode.PlaymobilLauraPC; break;
                    }
                    break;
            }
        }

        loader = MapLoader.Loader;
        loader.mode = mode;
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

        loader.Load();
        sectorManager.Init();
        lightManager.Init();
        InitPersos();
        InitSectors();
        InitCamera();
        if (viewCollision) UpdateViewCollision();
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C)) {
            viewCollision = !viewCollision;
        }
        if (loader != null && viewCollision != viewCollision_) {
            UpdateViewCollision();
        }
    }

    public void InitPersos() {
        if (loader != null) {
            for (int i = 0; i < loader.persos.Count; i++) {
                Perso p = loader.persos[i];
                PersoBehaviour unityBehaviour = p.Gao.AddComponent<PersoBehaviour>();
                unityBehaviour.controller = this;
                if (p.sectInfo != null && p.sectInfo.off_sector != null) unityBehaviour.sector = Sector.FromSuperObjectOffset(p.sectInfo.off_sector);
                if (p.SuperObject!=null && p.SuperObject.Gao!=null)
                {
                    Moddable mod = p.SuperObject.Gao.GetComponent<Moddable>();
                    if (mod!=null)
                    {
                        mod.persoBehaviour = unityBehaviour;
                    }
                }
                unityBehaviour.perso = p;
                unityBehaviour.Init();

                if (p.Gao) {
                    if (p.brain != null && p.brain.mind != null && p.brain.mind.AI_model != null) {
                        if (p.brain.mind.AI_model.behaviors_normal != null) {
                            GameObject intelParent = new GameObject("Rule behaviours");
                            intelParent.transform.parent = p.Gao.transform;
                            Behavior[] normalBehaviors = p.brain.mind.AI_model.behaviors_normal;
                            int iter = 0;
                            foreach (Behavior behavior in normalBehaviors) {
                                GameObject behaviorGao = new GameObject(behavior.name);
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
                                GameObject behaviorGao = new GameObject(behavior.name);
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

                        // DsgVars
                        if (p.brain.mind.dsgMem != null) {
                            DsgVarComponent dsgVarComponent = p.Gao.AddComponent<DsgVarComponent>();
                            dsgVarComponent.SetPerso(p);

                            if (p.SuperObject != null && p.SuperObject.Gao != null) {
                                Moddable mod = p.SuperObject.Gao.GetComponent<Moddable>();
                                if (mod != null) {
                                    mod.dsgVarComponent = dsgVarComponent;
                                }
                            }
                        }
                        
                        // Dynam
                        if (p.dynam!=null && p.dynam.dynamics!=null) {
                            DynamicsMechanicsComponent dynamicsBehaviour = p.Gao.AddComponent<DynamicsMechanicsComponent>();
                            dynamicsBehaviour.SetDynamics(p.dynam.dynamics);
                        }

                        if (p.brain!=null && p.brain.mind!=null) {
                            MindComponent mindComponent = p.Gao.AddComponent<MindComponent>();
                            mindComponent.Init(p, p.brain.mind);

                            if (p.SuperObject != null && p.SuperObject.Gao != null) {
                                Moddable mod = p.SuperObject.Gao.GetComponent<Moddable>();
                                if (mod != null) {
                                    mod.mindComponent = mindComponent;
                                }
                            }
                        }

                    }
                }
                if (p.Gao != null) {
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

    public void InitSectors() {
        if (loader != null) {
            foreach (Sector s in loader.sectors) {
                sectorManager.ApplySectorLighting(s, s.Gao);
                /*foreach (Perso p in s.persos) {
                    if (p.Gao) {
                        PersoBehaviour pb = p.Gao.GetComponent<PersoBehaviour>();
                        if (pb != null) {
                            pb.sector = s;
                        }
                    }
                }*/
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
        }
    }

    public void SaveChanges() {
        if(loader != null) loader.Save();
    }
}
