using OpenSpace;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorComponent : MonoBehaviour {
    public SectorManager sectorManager;
    public Sector sector;
    public LightBehaviour[] staticLights;
    public SectorComponent[] neighbors;
    public SectorComponent[] sectors_unk1;
    public SectorComponent[] sectors_unk2;
    //public LightBehaviour[] dynamicLights;

    // Use this for initialization
    void Start() {
    }

    public void Init() {
        staticLights = sector.staticLights.Select(l => l.Light).ToArray();
        neighbors = sector.neighbors.Select(s => s.sector.Gao.GetComponent<SectorComponent>()).ToArray();
        sectors_unk1 = sector.sectors_unk1.Select(s => s.sector.Gao.GetComponent<SectorComponent>()).ToArray();
        sectors_unk2 = sector.sectors_unk2.Select(s => s.Gao.GetComponent<SectorComponent>()).ToArray();
        //dynamicLights = sector.dynamicLights.Select(l => l.Light).ToArray();
    }
    
}
