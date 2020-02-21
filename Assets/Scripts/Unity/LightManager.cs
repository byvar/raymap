using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenSpace;
using System.Linq;
using OpenSpace.Visual;
using OpenSpace.Object;
using UnityEngine.UI;

public class LightManager : MonoBehaviour {
    bool loaded = false;
    public Controller controller;
    public SectorManager sectorManager;
    public MeshRenderer backgroundPanel;
	public MeshRenderer[] backgroundPanelsROM;
    private VisualMaterial backgroundMaterial;
	private OpenSpace.ROM.VisualMaterial backgroundMaterialROM;
	private OpenSpace.ROM.VisualMaterial[] backgroundMaterialsDDROM;
	private SectorComponent previousActiveBackgroundSector = null;
	List<LightBehaviour> lights;

    [Range(0.0f, 1.0f)]
    public float luminosity = 0.5f;
    private float _luminosity = 0.5f;

    public bool saturate = true;
    private bool _saturate = true;
    public bool useDefaultSettings = true;

    public bool enableLighting = true; private bool _enableLighting = true;
	public bool enableFog = true; private bool _enableFog = true;

	public bool IsLoaded {
        get { return loaded; }
    }

    // Use this for initialization
    void Start () {
        Shader.SetGlobalFloat("_Luminosity", luminosity);
        Shader.SetGlobalFloat("_Saturate", saturate ? 1f : 0f);

        // Set background UVs
        Mesh mesh = backgroundPanel.GetComponent<MeshFilter>().sharedMesh;
        List<Vector2> uvs = new List<Vector2>();
        mesh.GetUVs(0, uvs);
        mesh.SetUVs(0, uvs.Select(u => new Vector3(u.x, u.y, 1f)).ToList());
        /*backgroundScroll = backgroundPanel.gameObject.AddComponent<ScrollingTexture>();
        backgroundScroll.visMat = null;
        backgroundScroll.mat = null;*/
    }

	private void CheckKeys() {
		if (Input.GetKeyDown(KeyCode.L)) {
			enableLighting = !enableLighting;
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			enableFog = !enableFog;
		}
		if (_enableLighting != enableLighting) {
			_enableLighting = enableLighting;
			Shader.SetGlobalFloat("_DisableLighting", enableLighting ? 0f : 1f);
			controller.communicator.SendSettings();
		}
		if (_enableFog != enableFog) {
			_enableFog = enableFog;
			Shader.SetGlobalFloat("_DisableFog", enableFog ? 0f : 1f);
			controller.communicator.SendSettings();
		}
		if (_luminosity != luminosity) {
			_luminosity = luminosity;
			Shader.SetGlobalFloat("_Luminosity", luminosity);
		}
		if (_saturate != saturate) {
			_saturate = saturate;
			Shader.SetGlobalFloat("_Saturate", saturate ? 1f : 0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
        if (loaded) {
			CheckKeys();
			if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
				UpdateBackgroundROM();
			} else {
				UpdateBackground();
			}
            /*if (useFog && Camera.main.renderingPath == RenderingPath.DeferredShading) {
                // Fog doesn't work for deferred
                Camera.main.renderingPath = RenderingPath.Forward;
            }
            bool fogSet = false;
            bool ambientSet = false;
            if (simulateSectorLoading) {
                List<LightInfo> ambientLights = new List<LightInfo>();
                List<LightInfo> fogLights = new List<LightInfo>();
                for (int i = 0; i < lights.Count; i++) {
                    LightInfo l = lights[i];
                    if (l.containingSectors.FirstOrDefault(s => (neighborSectorLights ? s.Loaded : s.Active)) != null) {
                        l.Light.Activate();
                        if (l.type == 6) fogLights.Add(l);
                        if (l.type == 4) ambientLights.Add(l);
                    } else {
                        l.Light.Deactivate();
                    }
                }
                if (useAmbientColor) {
                    Color ambientLight = Color.black;
                    if (ambientLights.Count > 0) {
                        LightInfo l = ambientLights[0];
                        ambientLight = l.color;
                    }
                    RenderSettings.ambientLight = ambientLight;
                }

                if (useFog) {
                    if (fogLights.Count > 0) {
                        LightInfo l = fogLights[0];
                        float minFogDist = Vector3.Distance(fogLights[0].Light.transform.position, Camera.main.transform.position);
                        for (int i = 1; i < fogLights.Count; i++) {
                            float fogDist = Vector3.Distance(fogLights[i].Light.transform.position, Camera.main.transform.position);
                            if (fogDist < l.far && fogDist < minFogDist) {
                                l = fogLights[i];
                            }
                        }
                        RenderSettings.fog = true;
                        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, l.Light.color, 0.5f * Time.deltaTime);
                        RenderSettings.fogMode = FogMode.Linear;
                        RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, l.near, 0.5f * Time.deltaTime);
                        RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, l.far, 0.5f * Time.deltaTime);
                        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, l.Light.backgroundColor, 0.5f * Time.deltaTime);
                        fogSet = true;
                    } else {
                        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, 0.5f * Time.deltaTime);
                        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, 0.5f * Time.deltaTime);
                        RenderSettings.fogStartDistance += 50 * Time.deltaTime;
                        RenderSettings.fogEndDistance += 50 * Time.deltaTime;
                        if (RenderSettings.fogStartDistance > 500) {
                            RenderSettings.fog = false;
                        }
                    }
                }
            } else {
                for (int i = 0; i < lights.Count; i++) {
                    LightInfo l = lights[i];
                    l.Light.Activate();
                }
            }*/
        }
	}

	private void UpdateBackground() {
		// Update background color or material
		Color? backgroundColor = null;
		VisualMaterial skyMaterial = null;
		SectorComponent activeBackgroundSector = null;
		if (MapLoader.Loader.globals != null && MapLoader.Loader.globals.backgroundGameMaterial != null && MapLoader.Loader.globals.backgroundGameMaterial.visualMaterial != null) {
			skyMaterial = MapLoader.Loader.globals.backgroundGameMaterial.visualMaterial;
		} else {
			if (sectorManager != null && sectorManager.sectors != null && sectorManager.sectors.Count > 0) {
				foreach (SectorComponent s in sectorManager.sectors) {
					if (!s.Loaded) continue;
					if (s.sector == null) continue;
					if (s.sector.skyMaterial != null && s.sector.skyMaterial.textures.Count > 0 && s.sector.skyMaterial.textures.Where(t => t.texture != null).Count() > 0) {
						skyMaterial = s.sector.skyMaterial;
						activeBackgroundSector = s;
						break;
					} else {
						if (!s.Active) continue;
						foreach (LightInfo li in s.sector.staticLights) {
							if (li.type == 6) {
								backgroundColor = li.background_color;
								break;
							}
						}
					}
				}
			}
		}
		if (skyMaterial != null && !controller.viewCollision) {
			backgroundPanel.gameObject.SetActive(true);
			if (backgroundMaterial != skyMaterial) {
				backgroundMaterial = skyMaterial;
				Material skyboxMat = skyMaterial.GetMaterial();
				backgroundPanel.sharedMaterial = skyboxMat;
			}
			//skyboxMat.SetFloat("_DisableLighting", 1f);
			backgroundPanel.sharedMaterial.SetFloat("_DisableLightingLocal", 1f);
			if (activeBackgroundSector != null) {
				if (activeBackgroundSector != previousActiveBackgroundSector) {
					//backgroundPanel.material.SetFloat("_DisableLightingLocal", 0f);
					sectorManager.ApplySectorLighting(activeBackgroundSector, backgroundPanel.gameObject, LightInfo.ObjectLightedFlag.Environment);
					previousActiveBackgroundSector = activeBackgroundSector;
				}
			} else {
				//backgroundPanel.material.SetFloat("_DisableLighting", 1f);
			}
			//RenderSettings.skybox = skyboxMat;
			//Camera.main.clearFlags = CameraClearFlags.Skybox;
		} else {
			backgroundPanel.gameObject.SetActive(false);
			//RenderSettings.skybox = null;
			//Camera.main.clearFlags = CameraClearFlags.SolidColor;
		}
		if (backgroundColor.HasValue && !controller.viewCollision) {
			Camera.main.backgroundColor = backgroundColor.Value;
			//Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, backgroundColor.Value, 0.5f * Time.deltaTime);
		} else {
			Camera.main.backgroundColor = Color.black;
			//Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, 0.5f * Time.deltaTime);
		}
	}


	private void UpdateBackgroundROM() {
		// Update background color or material
		Color? backgroundColor = null;
		OpenSpace.ROM.VisualMaterial skyMaterial = null;
		OpenSpace.ROM.VisualMaterial[] skyMaterialsDD = null;
		SectorComponent activeBackgroundSector = null;
		OpenSpace.Loader.R2ROMLoader l = MapLoader.Loader as OpenSpace.Loader.R2ROMLoader;
		OpenSpace.ROM.LevelHeader lh = l.level;
		if (lh != null
			&& lh.backgroundUpLeft != null && lh.backgroundUpLeft.Value != null
			&& lh.backgroundUpRight != null && lh.backgroundUpRight.Value != null
			&& lh.backgroundDownLeft != null && lh.backgroundDownLeft.Value != null 
			&& lh.backgroundDownRight != null && lh.backgroundDownRight.Value != null) {
			skyMaterial = null;
			skyMaterialsDD = new OpenSpace.ROM.VisualMaterial[4];
			skyMaterialsDD[0] = lh.backgroundUpLeft.Value;
			skyMaterialsDD[1] = lh.backgroundUpRight.Value;
			skyMaterialsDD[2] = lh.backgroundDownLeft.Value;
			skyMaterialsDD[3] = lh.backgroundDownRight.Value;
		} else {
			if (sectorManager != null && sectorManager.sectors != null && sectorManager.sectors.Count > 0) {
				foreach (SectorComponent s in sectorManager.sectors) {
					if (!s.Loaded) continue;
					if (s.sectorROM == null) continue;
					if (s.sectorROM.background != null
						&& s.sectorROM.background.Value != null
						&& s.sectorROM.background.Value.num_textures > 0
						&& s.sectorROM.background.Value.textures.Value != null
						&& s.sectorROM.background.Value.textures.Value.vmTex[0].texRef.Value != null
						&& s.sectorROM.background.Value.textures.Value.vmTex[0].texRef.Value.texInfo.Value != null) {
						skyMaterial = s.sectorROM.background;
						//print(skyMaterial.Offset);
						activeBackgroundSector = s;
						break;
					} else {
						/*foreach (LightInfo li in s.sector.staticLights) {
							if (li.type == 6) {
								backgroundColor = li.background_color;
								break;
							}
						}*/
					}
				}
			}
		}
		if(!controller.viewCollision) {
			if (skyMaterial != null) {
				backgroundPanel.gameObject.SetActive(true);
				if (backgroundMaterialROM != skyMaterial) {
					backgroundMaterialROM = skyMaterial;
					Material skyboxMat = skyMaterial.GetMaterial(OpenSpace.ROM.VisualMaterial.Hint.None);
					/*Texture tex = skyboxMat.GetTexture("_Tex0");
					tex.filterMode = FilterMode.Point;
					tex.wrapMode = TextureWrapMode.Clamp;
					skyboxMat.SetTexture("_Tex0", tex);*/
					backgroundPanel.sharedMaterial = skyboxMat;
				}
				//skyboxMat.SetFloat("_DisableLighting", 1f);
				backgroundPanel.sharedMaterial.SetFloat("_DisableLightingLocal", 1f);
				if (activeBackgroundSector != null) {
					if (activeBackgroundSector != previousActiveBackgroundSector) {
						//backgroundPanel.material.SetFloat("_DisableLightingLocal", 0f);
						sectorManager.ApplySectorLighting(activeBackgroundSector, backgroundPanel.gameObject, LightInfo.ObjectLightedFlag.Environment);
						previousActiveBackgroundSector = activeBackgroundSector;
					}
				} else {
					//backgroundPanel.material.SetFloat("_DisableLighting", 1f);
				}
				//RenderSettings.skybox = skyboxMat;
				//Camera.main.clearFlags = CameraClearFlags.Skybox;
			} else {
				backgroundPanel.gameObject.SetActive(false);
			}
			if (skyMaterialsDD != null) {
				for (int i = 0; i < 4; i++) {
					backgroundPanelsROM[i].gameObject.SetActive(true);
				}
				if (backgroundMaterialsDDROM == null) {
					backgroundMaterialsDDROM = skyMaterialsDD;
					for (int i = 0; i < 4; i++) {
						Material skyboxMat = skyMaterialsDD[i].GetMaterial(OpenSpace.ROM.VisualMaterial.Hint.None);
						backgroundPanelsROM[i].sharedMaterial = skyboxMat;
						backgroundPanelsROM[i].sharedMaterial.SetFloat("_DisableLightingLocal", 1f);
					}
				}
				if (activeBackgroundSector != null) {
					if (activeBackgroundSector != previousActiveBackgroundSector) {
						//backgroundPanel.material.SetFloat("_DisableLightingLocal", 0f);
						//sectorManager.ApplySectorLighting(activeBackgroundSector, backgroundPanel.gameObject, LightInfo.ObjectLightedFlag.Environment);
						previousActiveBackgroundSector = activeBackgroundSector;
					}
				} else {
					//backgroundPanel.material.SetFloat("_DisableLighting", 1f);
				}
				//RenderSettings.skybox = skyboxMat;
				//Camera.main.clearFlags = CameraClearFlags.Skybox;
			} else {
				for (int i = 0; i < 4; i++) {
					backgroundPanelsROM[i].gameObject.SetActive(false);
				}
			}
		} else {
			backgroundPanel.gameObject.SetActive(false);
			for (int i = 0; i < 4; i++) {
				backgroundPanelsROM[i].gameObject.SetActive(false);
			}
			//RenderSettings.skybox = null;
			//Camera.main.clearFlags = CameraClearFlags.SolidColor;
		}
		if (backgroundColor.HasValue && !controller.viewCollision) {
			Camera.main.backgroundColor = backgroundColor.Value;
			//Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, backgroundColor.Value, 0.5f * Time.deltaTime);
		} else {
			Camera.main.backgroundColor = Color.black;
			//Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, 0.5f * Time.deltaTime);
		}
	}

	public void Init() {
		lights = new List<LightBehaviour>();
		if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
			for (int i = 0; i < sectorManager.sectors.Count; i++) {
				SectorComponent sc = sectorManager.sectors[i];
				if (sc.sectorROM.lights.Value != null) {
					sc.lights = new LightBehaviour[sc.sectorROM.lights.Value.lights.Length];
					for (int j = 0; j < sc.lights.Length; j++) {
						sc.lights[j] = Register(sc.sectorROM.lights.Value.lights[j]);
					}
				}
			}
		} else {
			for (int i = 0; i < sectorManager.sectors.Count; i++) {
				SectorComponent sc = sectorManager.sectors[i];
				if (sc.sector.staticLights != null) {
					sc.lights = new LightBehaviour[sc.sector.staticLights.Count];
					for (int j = 0; j < sc.lights.Length; j++) {
						sc.lights[j] = Register(sc.sector.staticLights[j]);
					}
				}
			}
		}
        if (useDefaultSettings) {
            luminosity = Settings.s.luminosity;
            saturate = Settings.s.saturate;
        }
        loaded = true;
    }

    public LightBehaviour Register(LightInfo light) {
		LightBehaviour l = lights.FirstOrDefault(li => li.li == light);
		if (l == null) {
			GameObject gao = new GameObject("LightInfo @ " + light.Offset);
			l = gao.AddComponent<LightBehaviour>();
			l.Init(this, light);
			l.transform.parent = transform;
			lights.Add(l);
		}
		return l;
	}
	public LightBehaviour Register(OpenSpace.ROM.LightInfo light) {
		LightBehaviour l = lights.FirstOrDefault(li => li.liROM == light);
		if (l == null) {
			GameObject gao = new GameObject("LightInfo @ " + light.Offset);
			l = gao.AddComponent<LightBehaviour>();
			l.Init(this, light);
			l.transform.parent = transform;
			lights.Add(l);
		}
		return l;
	}

	public void RecalculateSectorLighting() {
        if (sectorManager != null) sectorManager.RecalculateSectorLighting();
    }
}
