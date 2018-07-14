using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenSpace;
using System.Linq;

public class LightManager : MonoBehaviour {
    bool loaded = false;
    public bool simulateSectorLoading = true;
    public bool neighborSectorLights = true;
    public bool useTestFog = false;
    public bool useAmbientColor = false;
    public SectorManager sectorManager;
    List<LightInfo> lights;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (loaded) {
            if (useTestFog && Camera.main.renderingPath == RenderingPath.DeferredShading) {
                // Fog doesn't work for deferred
                Camera.main.renderingPath = RenderingPath.Forward;
            }
            bool fogSet = false;
            bool ambientSet = false;
            if (simulateSectorLoading) {
                for (int i = 0; i < lights.Count; i++) {
                    LightInfo l = lights[i];
                    if (l.containingSectors.FirstOrDefault(s => (neighborSectorLights ? s.Loaded : s.Active)) != null) {
                        l.Light.Activate();
                        if (useTestFog && l.type == 6 && !fogSet) {
                            RenderSettings.fog = true;
                            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, l.Light.color, 0.5f * Time.deltaTime);
                            RenderSettings.fogMode = FogMode.Linear;
                            RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, l.near, 0.5f * Time.deltaTime);
                            RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, l.far * 5f, 0.5f * Time.deltaTime);
                            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, l.Light.backgroundColor, 0.5f * Time.deltaTime);
                            fogSet = true;
                        }
                        if (useAmbientColor && l.type == 4 && !ambientSet) {
                            RenderSettings.ambientLight = l.color;
                        }
                    } else {
                        l.Light.Deactivate();
                    }
                }

            } else {
                for (int i = 0; i < lights.Count; i++) {
                    LightInfo l = lights[i];
                    l.Light.Activate();
                }
            }
            if (useTestFog && !fogSet) {
                Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.black, 0.5f * Time.deltaTime);
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.black, 0.5f * Time.deltaTime);
                RenderSettings.fogStartDistance += 50 * Time.deltaTime;
                RenderSettings.fogEndDistance += 50 * Time.deltaTime;
                if (RenderSettings.fogStartDistance > 500) {
                    RenderSettings.fog = false;
                }
            }
        }
	}

    public void Init() {
        lights = MapLoader.Loader.lights;
        for (int i = 0; i < lights.Count; i++) {
            Register(lights[i]);
        }
        loaded = true;
    }

    public void Register(LightInfo light) {
        LightBehaviour l = light.Light;
        l.Init();
        l.transform.parent = transform;
    }
}
