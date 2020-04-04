using OpenSpace;
using OpenSpace.Object;
using OpenSpace.Visual;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorManager : MonoBehaviour {
	public Controller controller;
    bool loaded = false;
    public bool displayInactiveSectors = true; bool _displayInactiveSectors = true;
    public List<SectorComponent> sectors;
    public Camera mainCamera;
    //public List<Sector> activeSectors = new List<Sector>();
    public SectorComponent activeSector = null;
    Vector3 camPosPrevious;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (loaded) {
            Vector3 camPos = Camera.main.transform.localPosition;
			UpdateSectorLoading(camPos);

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
				SectorComponent sc = sectors[i];
				if (sc.Loaded) {
					sc.SetGameObjectActive(true);
				} else {
					sc.SetGameObjectActive(false);
				}
			}
		} else {
			for (int i = 0; i < sectors.Count; i++) {
				sectors[i].SetGameObjectActive(true);
			}
		}
	}

	public void UpdateSectorLoading(Vector3 camPos) {
		if (Settings.s.engineVersion < Settings.EngineVersion.R2 || Settings.s.game == Settings.Game.LargoWinch) {
			activeSector = GetActiveSectorOld(camPos, activeSector);
		} else {
			activeSector = GetActiveSector(camPos, blockVirtual: true);
		}
		for (int i = 0; i < sectors.Count; i++) {
			SectorComponent s = sectors[i];
			s.Loaded = false;
			s.Active = false;
		}
		if (activeSector == null) {
			for (int i = 0; i < sectors.Count; i++) {
				sectors[i].Loaded = true;
			}
		} else {
			activeSector.Loaded = true;
			for (int j = 0; j < activeSector.neighbors.Length; j++) {
				activeSector.neighbors[j].Loaded = true;
			}
		}
	}

	public SectorComponent GetActiveSectorWrapper(Vector3 point) {
		if (Settings.s.engineVersion < Settings.EngineVersion.R2 || Settings.s.game == Settings.Game.LargoWinch) {
			return GetActiveSectorOld(point);
		} else {
			return GetActiveSector(point);
		}
	}

	private SectorComponent GetActiveSectorOld(Vector3 point, SectorComponent currentActiveSector = null, bool allowVirtual = false) {
        if (currentActiveSector!= null && currentActiveSector.sectorTransitions != null && currentActiveSector.sectorTransitions.Length > 0) {
			// We shouldn't really test the sector transitions here, but a lot of "absorbing" sectors have no transitions
			if (currentActiveSector.SectorBorder != null ? currentActiveSector.SectorBorder.ContainsPoint(point) : false) {
				return currentActiveSector;
			}
        }
		SectorComponent activeSector = null;
		for (int i = 0; i < sectors.Count; i++) {
            SectorComponent s = sectors[i];
            s.Loaded = false;
			s.Active = false;
        }
		if(currentActiveSector != null && currentActiveSector.sectorTransitions != null) {
			for (int i = 0; i < currentActiveSector.sectorTransitions.Length; i++) {
				SectorComponent s = currentActiveSector.sectorTransitions[i];
				bool active = (allowVirtual || !s.IsSectorVirtual) && (s.SectorBorder != null ? s.SectorBorder.ContainsPoint(point) : false);
				if (active) {
					activeSector = s;
					break;
				}
			}
		}
		if (activeSector == null) {
			for (int i = 0; i < sectors.Count; i++) {
				SectorComponent s = sectors[i];
				bool active = (allowVirtual || !s.IsSectorVirtual) && (s.SectorBorder != null ? s.SectorBorder.ContainsPoint(point) : false);
				if (active) {
					activeSector = s;
					break;
				}
			}
		}

        return activeSector;
    }

	private SectorComponent GetActiveSector(Vector3 point, bool blockVirtual = false) {
		float smallestDistanceToSector = float.MaxValue;
		float smallestDistanceToSectorVirtual = float.MaxValue;
		byte activeSectorPriority = 0;
		byte activeSectorPriorityVirtual = 0;
		SectorComponent activeSectorVirtual = null;
		SectorComponent activeSector = null;
		foreach(SectorComponent s in sectors) {
			if (s.SectorBorder != null ? s.SectorBorder.ContainsPoint(point) : false) {
				float dist = Vector3.Distance(s.SectorBorder.Center, point);
				if (s.IsSectorVirtual) {
					if (s.SectorPriority > activeSectorPriorityVirtual || (s.SectorPriority == activeSectorPriorityVirtual && dist < smallestDistanceToSectorVirtual)) {
						smallestDistanceToSectorVirtual = dist;
						activeSectorVirtual = s;
						activeSectorPriorityVirtual = s.SectorPriority;
					}
				} else {
					if (s.SectorPriority > activeSectorPriority || (s.SectorPriority == activeSectorPriority && dist < smallestDistanceToSector)) {
						smallestDistanceToSector = dist;
						activeSector = s;
						activeSectorPriority = s.SectorPriority;
					}
				}
			}
		}
		if (activeSector != null) {
			return activeSector;
		} else if (activeSectorVirtual != null && !blockVirtual) {
			return activeSectorVirtual;
		} else {
			return sectors.LastOrDefault(); // Univers
		}
	}

	public void AddSector(SectorComponent sc) {
		sectors.Add(sc);
	}

    public void Init() {
        /*sectors = MapLoader.Loader.sectors;
        sectorComponents = sectors.Select(s => s.Gao.AddComponent<SectorComponent>()).ToList();
		if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
			SectorsROM = new List<SectorComponent>();
			OpenSpace.Loader.R2ROMLoader l = (OpenSpace.Loader.R2ROMLoader)MapLoader.Loader;
			OpenSpace.ROM.LevelHeader lh = l.level;
		}*/
		for (int i = 0; i < sectors.Count; i++) {
            SectorComponent sc = sectors[i];
            sc.Init();
			/*if (sc.sector != null) {
				ApplySectorLighting(sc, sc.Gao, LightInfo.ObjectLightedFlag.Environment);
			}*/
        }
    }

	public void InitLights() {
		/*for (int i = 0; i < sectors.Count; i++) {
			SectorComponent sc = sectors[i];
			ApplySectorLighting(sc, sc.Gao, LightInfo.ObjectLightedFlag.Environment);
		}*/
		loaded = true;
		RecalculateSectorLighting();
		//loaded = true;
	}

    public void RecalculateSectorLighting() {
        foreach (SectorComponent sc in sectors) {
			ApplySectorLighting(sc, sc.Gao, LightInfo.ObjectLightedFlag.Environment);
			/*foreach (Perso p in sc.sector.persos) {
				if (p.Gao) {
					PersoBehaviour pb = p.Gao.GetComponent<PersoBehaviour>();
					if (pb != null) {
						pb.sector = sc;
					}
				}
			}*/
        }
        if (loaded) {
            foreach (Perso p in MapLoader.Loader.persos) {
                PersoBehaviour pb = p.Gao.GetComponent<PersoBehaviour>();
                ApplySectorLighting(pb.sector, p.Gao, LightInfo.ObjectLightedFlag.Perso);
            }
			foreach (ROMPersoBehaviour p in controller.romPersos) {
				ApplySectorLighting(p.sector, p.gameObject, LightInfo.ObjectLightedFlag.Perso);
			}
        }
    }

    public void ApplySectorLighting(SectorComponent sector, GameObject gao, LightInfo.ObjectLightedFlag objectType) {
		if (objectType == LightInfo.ObjectLightedFlag.None) {
			if (gao) {
				List<Renderer> rs = gao.GetComponents<Renderer>().ToList();
				foreach (Renderer r in rs) {
					if (r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend")) {
						r.material.SetFloat("_DisableLightingLocal", 1);
					}
				}
				rs = gao.GetComponentsInChildren<Renderer>(true).ToList();
				foreach (Renderer r in rs) {
					if (r.sharedMaterial.shader.name.Contains("Gouraud") || r.sharedMaterial.shader.name.Contains("Texture Blend")) {
						r.material.SetFloat("_DisableLightingLocal", 1);
					}
				}
			}
		} else {
			if (sector == null) return;
			/*Sector s = sector.sector;
			if (s == null) return;*/
			if (sector.lights != null) {
				Vector4? fogColor = null;
				Vector4? fogParams = null;
				Vector4 ambientLight = Vector4.zero;
				List<Vector4> staticLightPos = new List<Vector4>();
				List<Vector4> staticLightDir = new List<Vector4>();
				List<Vector4> staticLightCol = new List<Vector4>();
				List<Vector4> staticLightParams = new List<Vector4>();
				for (int i = 0; i < sector.lights.Length; i++) {
					LightBehaviour lb = sector.lights[i];
					if (!lb.IsActive) continue;
					if (!lb.IsObjectLighted(objectType)) continue;
					switch (lb.Type) {
						case 4:
							ambientLight += lb.Color;
							staticLightPos.Add(new Vector4(lb.transform.position.x, lb.transform.position.y, lb.transform.position.z, lb.Type));
							staticLightDir.Add(lb.transform.TransformVector(Vector3.back));
							staticLightCol.Add(lb.Color);
							staticLightParams.Add(new Vector4(lb.Near, lb.Far, lb.PaintingLightFlag, lb.AlphaLightFlag));
							break;
						case 6:
							if (!fogColor.HasValue) {
								fogColor = lb.Color;
								fogParams = new Vector4(lb.FogBlendNear / 255f, lb.FogBlendFar / 255f, lb.Near, lb.Far);
							}
							break;
						default:
							staticLightPos.Add(new Vector4(lb.transform.position.x, lb.transform.position.y, lb.transform.position.z, lb.Type));
							staticLightDir.Add(lb.transform.TransformVector(Vector3.back));
							staticLightCol.Add(lb.Color);
							Vector3 scale = lb.transform.localScale;
							float maxScale = Mathf.Max(scale.x, scale.y, scale.z);
							staticLightParams.Add(new Vector4(lb.Near * maxScale, lb.Far * maxScale, lb.PaintingLightFlag, lb.AlphaLightFlag));
							break;
					}
				}
				Vector4[] staticLightPosArray = staticLightPos.ToArray();
				Vector4[] staticLightDirArray = staticLightDir.ToArray();
				Vector4[] staticLightColArray = staticLightCol.ToArray();
				Vector4[] staticLightParamsArray = staticLightParams.ToArray();
				//print(staticLightPosArray.Length + " - " + objectType);
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
