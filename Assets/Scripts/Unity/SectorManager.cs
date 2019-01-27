using OpenSpace;
using OpenSpace.Object;
using OpenSpace.Visual;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorManager : MonoBehaviour {
    bool loaded = false;
    public bool displayInactiveSectors = true; bool _displayInactiveSectors = true;
    public List<Sector> sectors;
    private List<SectorComponent> sectorComponents;
    public Camera mainCamera;
    public List<Sector> activeSectors = new List<Sector>();
	Vector3 camPosPrevious;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (loaded) {
            Vector3 camPos = Camera.main.transform.localPosition;
            activeSectors = GetActiveSectorsAtPoint(camPos);


			if (Input.GetKeyDown(KeyCode.Y)) {
				displayInactiveSectors = !displayInactiveSectors;
			}

			if (_displayInactiveSectors != displayInactiveSectors) {
				_displayInactiveSectors = displayInactiveSectors;
				UpdateSectors();
			} else if (camPos != camPosPrevious) {
				camPosPrevious = camPos;
				UpdateSectors();
			}
        }
    }

	private void UpdateSectors() {
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

    public List<Sector> GetActiveSectorsAtPoint(Vector3 point, bool allowVirtual = false) {
        List<Sector> activeSectors = new List<Sector>();
        for (int i = 0; i < sectors.Count; i++) {
            Sector s = sectors[i];
            s.Loaded = false;
            s.Active = (allowVirtual || s.isSectorVirtual == 0) && (s.sectorBorder != null ? s.sectorBorder.ContainsPoint(point) : true);
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
        return activeSectors;
    }

    public void Init() {
        sectors = MapLoader.Loader.sectors;
        sectorComponents = sectors.Select(s => s.Gao.AddComponent<SectorComponent>()).ToList();
        for (int i = 0; i < sectors.Count; i++) {
            Sector s = sectors[i];
            sectorComponents[i].sector = s;
            sectorComponents[i].Init();
            ApplySectorLighting(s, s.Gao, LightInfo.ObjectLightedFlag.Environment);
        }
        loaded = true;
    }

    public void RecalculateSectorLighting() {
        foreach (Sector s in sectors) {
            ApplySectorLighting(s, s.Gao, LightInfo.ObjectLightedFlag.Environment);
            /*foreach (Perso p in s.persos) {
                if (p.Gao) {
                    PersoBehaviour pb = p.Gao.GetComponent<PersoBehaviour>();
                    if (pb != null) {
                        pb.sector = s;
                    }
                }
            }*/
        }
        if (loaded) {
            foreach (Perso p in MapLoader.Loader.persos) {
                PersoBehaviour pb = p.Gao.GetComponent<PersoBehaviour>();
                ApplySectorLighting(pb.sector, p.Gao, LightInfo.ObjectLightedFlag.Perso);
            }
        }
    }

    public void ApplySectorLighting(Sector s, GameObject gao, LightInfo.ObjectLightedFlag objectType) {
		if (objectType == LightInfo.ObjectLightedFlag.None) {
			if (gao) {
				List<Renderer> rs = gao.GetComponents<Renderer>().ToList();
				foreach (Renderer r in rs) {
					if (r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend")) {
						r.sharedMaterial.SetFloat("_DisableLightingLocal", 1);
					}
				}
				rs = gao.GetComponentsInChildren<Renderer>(true).ToList();
				foreach (Renderer r in rs) {
					if (r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend")) {
						r.sharedMaterial.SetFloat("_DisableLightingLocal", 1);
					}
				}
			}
		} else {
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
					LightInfo li = s.staticLights[i];
					//if (!li.IsObjectLighted(objectType)) continue;
					//if (li.turnedOn == 0x0) continue;
					switch (li.type) {
						case 4:
							ambientLight += li.color;
							staticLightPos.Add(new Vector4(li.Light.transform.position.x, li.Light.transform.position.y, li.Light.transform.position.z, li.type));
							staticLightDir.Add(li.Light.transform.TransformVector(Vector3.back));
							staticLightCol.Add(li.color);
							staticLightParams.Add(new Vector4(li.near, li.far, li.paintingLightFlag, li.alphaLightFlag));
							break;
						case 6:
							if (!fogColor.HasValue) {
								fogColor = li.color;
								fogParams = new Vector4(li.bigAlpha_fogBlendNear / 255f, li.intensityMin_fogBlendFar / 255f, li.near, li.far);
							}
							break;
						default:
							staticLightPos.Add(new Vector4(li.Light.transform.position.x, li.Light.transform.position.y, li.Light.transform.position.z, li.type));
							staticLightDir.Add(li.Light.transform.TransformVector(Vector3.back));
							staticLightCol.Add(li.color);
							Vector3 scale = li.transMatrix.GetScale(true);
							float maxScale = Mathf.Max(scale.x, scale.y, scale.z);
							staticLightParams.Add(new Vector4(li.near * maxScale, li.far * maxScale, li.paintingLightFlag, li.alphaLightFlag));
							break;
					}
				}
				Vector4[] staticLightPosArray = staticLightPos.ToArray();
				Vector4[] staticLightDirArray = staticLightDir.ToArray();
				Vector4[] staticLightColArray = staticLightCol.ToArray();
				Vector4[] staticLightParamsArray = staticLightParams.ToArray();
				if (gao) {
					List<Renderer> rs = gao.GetComponents<Renderer>().ToList();
					foreach (Renderer r in rs) {
						if (r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend")) {
							MaterialPropertyBlock props = new MaterialPropertyBlock();
							r.GetPropertyBlock(props);
							if (fogColor.HasValue) props.SetVector("_SectorFog", fogColor.Value);
							if (fogParams.HasValue) props.SetVector("_SectorFogParams", fogParams.Value);
							//r.material.SetVector("_SectorAmbient", ambientLight);
							props.SetFloat("_StaticLightCount", staticLightPosArray.Length);
							if (staticLightPosArray.Length > 0) {
								props.SetVectorArray("_StaticLightPos", staticLightPosArray);
								props.SetVectorArray("_StaticLightDir", staticLightDirArray);
								props.SetVectorArray("_StaticLightCol", staticLightColArray);
								props.SetVectorArray("_StaticLightParams", staticLightParamsArray);
							}
							r.SetPropertyBlock(props);
						}
					}
					rs = gao.GetComponentsInChildren<Renderer>(true).ToList();
					foreach (Renderer r in rs) {
						if (r.sharedMaterial != null &&
							(r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend"))) {
							MaterialPropertyBlock props = new MaterialPropertyBlock();
							r.GetPropertyBlock(props);
							if (fogColor.HasValue) props.SetVector("_SectorFog", fogColor.Value);
							if (fogParams.HasValue) props.SetVector("_SectorFogParams", fogParams.Value);
							//r.material.SetVector("_SectorAmbient", ambientLight);
							props.SetFloat("_StaticLightCount", staticLightPosArray.Length);
							if (staticLightPosArray.Length > 0) {
								props.SetVectorArray("_StaticLightPos", staticLightPosArray);
								props.SetVectorArray("_StaticLightDir", staticLightDirArray);
								props.SetVectorArray("_StaticLightCol", staticLightColArray);
								props.SetVectorArray("_StaticLightParams", staticLightParamsArray);
							}
							r.SetPropertyBlock(props);
						}
					}
				}
			}
		}
    }
}
