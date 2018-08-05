using OpenSpace;
using OpenSpace.EngineObject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorCamera : MonoBehaviour {
    public SectorManager sectorManager;
    public int sectorIndex;
    bool ambientSet = false;
    bool fogSet = false;

    // Use this for initialization
    void Start() {
        GetComponent<Camera>().depth = 0;
    }

    void OnPreCull() {
        if (sectorManager != null && sectorManager.sectors != null && sectorIndex >= 0 && sectorIndex < sectorManager.sectors.Count) {
            bool first = this.Equals(sectorManager.cameras.First());
            if(first) {
                foreach (Sector sect in sectorManager.sectors) sect.Gao.SetActive(false);
            }
            Sector s = sectorManager.sectors[sectorIndex];
            s.Gao.SetActive(true);
            if (s.sectorLights != null) {
                Color ambientLight = Color.black;
                for (int i = 0; i < s.sectorLights.Count; i++) {
                    s.sectorLights[i].Light.gameObject.SetActive(true);
                    if (s.sectorLights[i].type == 4) {
                        ambientLight = new Color(
                            ambientLight.r + s.sectorLights[i].Light.color.r,
                            ambientLight.g + s.sectorLights[i].Light.color.g,
                            ambientLight.b + s.sectorLights[i].Light.color.b);
                    }
                    if (!fogSet && s.sectorLights[i].type == 6) {
                        fogSet = true;
                        RenderSettings.fog = true;
                        RenderSettings.fogColor = s.sectorLights[i].color;
                        RenderSettings.fogMode = FogMode.Linear;
                        RenderSettings.fogStartDistance = s.sectorLights[i].near;
                        RenderSettings.fogEndDistance = s.sectorLights[i].far;
                        //Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, l.Light.backgroundColor, 0.5f * Time.deltaTime);
                    }
                }
                RenderSettings.ambientLight = ambientLight;
            }
            if (s.persos != null) {
                if (first) {
                    for (int i = 0; i < MapLoader.Loader.persos.Count; i++) {
                        MapLoader.Loader.persos[i].Gao.SetActive(false);
                    }
                }
                for (int i = 0; i < s.persos.Count; i++) {
                    s.persos[i].Gao.SetActive(true);
                }
            }
        }
    }

    void OnPostRender() {
        if (sectorManager != null && sectorManager.sectors != null && sectorIndex >= 0 && sectorIndex < sectorManager.sectors.Count) {
            bool last = this.Equals(sectorManager.cameras.Last());
            Sector s = sectorManager.sectors[sectorIndex];
            if (last) {
                foreach (Sector sect in sectorManager.sectors) sect.Gao.SetActive(true);
            } else {
                s.Gao.SetActive(false);
            }
            //s.Gao.SetActive(false);
            if (s.sectorLights != null) {
                for (int i = 0; i < s.sectorLights.Count; i++) {
                    s.sectorLights[i].Light.gameObject.SetActive(false);
                }
                if (fogSet) {
                    RenderSettings.fog = false;
                    fogSet = false;
                }
                RenderSettings.ambientLight = Color.white;
                /*if (ambientSet) {
                    RenderSettings.ambientLight = Color.black;
                    ambientSet = false;
                }*/
            }
            if (s.persos != null) {
                if (last) {
                    for (int i = 0; i < MapLoader.Loader.persos.Count; i++) {
                        MapLoader.Loader.persos[i].Gao.SetActive(true);
                    }
                } else {
                    for (int i = 0; i < s.persos.Count; i++) {
                        s.persos[i].Gao.SetActive(false);
                    }
                }
            }
        }
    }
}
