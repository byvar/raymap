using OpenSpace;
using OpenSpace.EngineObject;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorManager : MonoBehaviour {
    bool loaded = false;
    public bool displayInactiveSectors = true;
    public List<Sector> sectors;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (loaded) {
            List<Sector> activeSectors = new List<Sector>();
            Vector3 camPos = Camera.main.transform.localPosition;
            for (int i = 0; i < sectors.Count; i++) {
                Sector s = sectors[i];
                s.Loaded = false;
                s.Active = (camPos.x >= s.minBorder.x && camPos.x <= s.maxBorder.x
                && camPos.y >= s.minBorder.y && camPos.y <= s.maxBorder.y
                && camPos.z >= s.minBorder.z && camPos.z <= s.maxBorder.z);
                if (s.Active) activeSectors.Add(s);
            }
            for (int i = 0; i < activeSectors.Count; i++) {
                Sector s = activeSectors[i];
                for (int j = 0; j < s.neighbors.Count; j++) {
                    s.neighbors[j].Loaded = true;
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
            s.ProcessNeighbors();
        }
        loaded = true;
    }
}
