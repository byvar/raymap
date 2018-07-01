using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using OpenSpace;
using OpenSpace.Visual;
using OpenSpace.EngineObject;

public class Controller : MonoBehaviour {
    public MapLoader.Mode mode = MapLoader.Mode.Rayman3PC;
    public string gameDataBinFolder;
    public string lvlName;

    public Material baseMaterial;
    public Material baseLightMaterial;
    public Material baseTransparentMaterial;
    public Material baseBlendMaterial;
    public Material baseBlendTransparentMaterial;
    public Material negativeLightProjectorMaterial;
    public SectorManager sectorManager;
    public LightManager lightManager;
    public bool allowDeadPointers = false;
    public bool forceDisplayBackfaces = false;
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
        loader.baseMaterial = baseMaterial;
        loader.baseTransparentMaterial = baseTransparentMaterial;
        loader.baseBlendMaterial = baseBlendMaterial;
        loader.baseBlendTransparentMaterial = baseBlendTransparentMaterial;
        loader.negativeLightProjectorMaterial = negativeLightProjectorMaterial;
        loader.baseLightMaterial = baseLightMaterial;

        loader.Load();
        sectorManager.Init();
        lightManager.Init();
        InitPersos();
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
                PersoBehaviour unityBehaviour = loader.persos[i].Gao.AddComponent<PersoBehaviour>();
                unityBehaviour.perso = loader.persos[i];
                unityBehaviour.Init();
            }
        }
    }

    public void InitCamera() {
        if (loader != null) {
            Perso camera = loader.persos.Where(p => p != null && p.name2.Equals("StdCamer")).FirstOrDefault();
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
                    if (l.obj != null && l.obj is MeshObject) {
                        GameObject gao = ((MeshObject)l.obj).gao;
                        if (gao != null) gao.SetActive(!viewCollision);
                    }
                }
                if (po.collideMesh != null) po.collideMesh.gao.SetActive(viewCollision);
            }
        }
    }

    public void SaveChanges() {
        if(loader != null) loader.Save();
    }
}
