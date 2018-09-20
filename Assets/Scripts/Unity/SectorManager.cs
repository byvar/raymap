using OpenSpace;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorManager : MonoBehaviour {
    bool loaded = false;
    public bool useMultiCameras = true;
    public bool displayInactiveSectors = true;
    public List<Sector> sectors;
    public List<SectorCamera> cameras = new List<SectorCamera>();
    public SectorCamera sectorCameraPrefab;
    public Camera mainCamera;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(useMultiCameras) return;
        if (loaded) {
            List<Sector> activeSectors = new List<Sector>();
            Vector3 camPos = Camera.main.transform.localPosition;
            for (int i = 0; i < sectors.Count; i++) {
                Sector s = sectors[i];
                s.Loaded = false;
                s.Active = (s.isSectorVirtual == 0) && (s.sectorBorder != null ? s.sectorBorder.ContainsPoint(camPos) : true);
                if (s.Active) activeSectors.Add(s);
            }
            for (int i = 0; i < activeSectors.Count; i++) {
                Sector s = activeSectors[i];
                for (int j = 0; j < s.neighbors.Count; j++) {
                    s.neighbors[j].sector.Loaded = true;
                }
            }
            if (activeSectors.Count == 0) {
                for (int i = 0; i < sectors.Count; i++) {
                    sectors[i].Loaded = true;
                }
            }
            /*for (int i = 0; i < sectors.Count; i++) {
                R3Sector s = sectors[i];
                if (!s.Active) {
                    // We want to display all sectors if none are active at the moment.
                    s.Loaded = activeSectorsCount == 0 || (s.neighbors.FirstOrDefault(n => n.Active) != null);
                }
            }*/

            // Old method: load everything that's in view. Not really a good idea
            /*Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            for (int i = 0; i < sectors.Count; i++) {
                R3Sector s = sectors[i];
                Vector3 center = new Vector3(s.minBorder.x + s.maxBorder.x / 2f,
                    s.minBorder.y + s.maxBorder.y / 2f,
                    s.minBorder.z + s.maxBorder.z / 2f);
                Vector3 size = new Vector3(s.maxBorder.x - s.minBorder.x,
                    s.maxBorder.y - s.minBorder.y,
                    s.maxBorder.z - s.minBorder.z);
                Bounds bounds = new Bounds(center, size);
                s.Loaded = false;
                s.Active = GeometryUtility.TestPlanesAABB(planes, bounds);
            }*/

            if (!displayInactiveSectors) {
                for (int i = 0; i < sectors.Count; i++) {
                    Sector s = sectors[i];
                    if (s.Loaded) {
                        s.Gao.SetActive(true);
                    } else {
                        s.Gao.SetActive(false);
                    }
                }
            } else {
                for (int i = 0; i < sectors.Count; i++) {
                    Sector s = sectors[i];
                    s.Gao.SetActive(true);
                }
            }
        }
    }

    public void Init() {
        sectors = MapLoader.Loader.sectors;
        for (int i = 0; i < sectors.Count; i++) {
            Sector s = sectors[i];
            if (useMultiCameras) {
                s.Gao.SetActive(false);
                SectorCamera sc = Instantiate(sectorCameraPrefab, Vector3.zero, Quaternion.identity, mainCamera.transform);
                cameras.Add(sc);
                sc.transform.localPosition = Vector3.zero;
                sc.transform.localRotation = Quaternion.identity;
                sc.transform.localScale = Vector3.one;
                sc.sectorIndex = i;
                sc.sectorManager = this;
            } else {
                mainCamera.cullingMask = -1;
            }
        }
        loaded = true;
    }

    public void RecalculateSectorLighting() {
        foreach (Sector s in sectors) {
            ApplySectorLighting(s, s.Gao);
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

    public void ApplySectorLighting(Sector s, GameObject gao) {
        if (s == null) return;
        if (s.staticLights != null) {
            Vector4? fogColor = null;
            Vector4? fogParams = null;
            Vector4 ambientLight = Vector4.zero;
            List<Vector4> staticLightPos = new List<Vector4>();
            List<Vector4> staticLightDir = new List<Vector4>();
            List<Vector4> staticLightCol = new List<Vector4>();
            List<Vector4> staticLightParams = new List<Vector4>();
            for (int i = 0; i < s.staticLights.Count; i++) {
                if (s.staticLights[i].turnedOn == 0x0) continue;
                switch (s.staticLights[i].type) {
                    case 4:
                        ambientLight += s.staticLights[i].color * 0.5f;
                        break;
                    case 6:
                        if (!fogColor.HasValue) {
                            fogColor = s.staticLights[i].color;
                            fogParams = new Vector4(s.staticLights[i].littleAlpha, s.staticLights[i].bigAlpha, s.staticLights[i].near, s.staticLights[i].far);
                        }
                        break;
                    default:
                        staticLightPos.Add(new Vector4(s.staticLights[i].Light.transform.position.x, s.staticLights[i].Light.transform.position.y, s.staticLights[i].Light.transform.position.z, s.staticLights[i].type));
                        staticLightDir.Add(s.staticLights[i].Light.transform.TransformVector(Vector3.back));
                        staticLightCol.Add(s.staticLights[i].color);
                        staticLightParams.Add(new Vector4(s.staticLights[i].near, s.staticLights[i].far, s.staticLights[i].littleAlpha, s.staticLights[i].alphaLightFlag));
                        break;
                }
            }
            Vector4[] staticLightPosArray = staticLightPos.ToArray();
            Vector4[] staticLightDirArray = staticLightDir.ToArray();
            Vector4[] staticLightColArray = staticLightCol.ToArray();
            Vector4[] staticLightParamsArray = staticLightParams.ToArray();
            if (gao) {
                List<Renderer> rs = gao.GetComponentsInChildren<Renderer>(true).ToList();
                foreach (Renderer r in rs) {
                    if (r.material.shader.name.Contains("Gouraud") || r.material.shader.name.Contains("Texture Blend")) {
                        if (fogColor.HasValue) r.material.SetVector("_SectorFog", fogColor.Value);
                        if (fogParams.HasValue) r.material.SetVector("_SectorFogParams", fogParams.Value);
                        r.material.SetVector("_SectorAmbient", ambientLight);
                        r.material.SetFloat("_StaticLightCount", staticLightPosArray.Length);
                        if (staticLightPosArray.Length > 0) {
                            r.material.SetVectorArray("_StaticLightPos", staticLightPosArray);
                            r.material.SetVectorArray("_StaticLightDir", staticLightDirArray);
                            r.material.SetVectorArray("_StaticLightCol", staticLightColArray);
                            r.material.SetVectorArray("_StaticLightParams", staticLightParamsArray);
                        }
                    }
                }
            }
        }
    }
}
