using OpenSpace;
using OpenSpace.Collide;
using OpenSpace.Loader;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorComponent : MonoBehaviour {
    public SectorManager sectorManager;
    public Sector sector;
	public OpenSpace.ROM.Sector sectorROM;
	public OpenSpace.PS1.Sector sectorPS1;
    //public LightBehaviour[] staticLights;
    public SectorComponent[] neighbors;
    public SectorComponent[] sectorTransitions;
    public SectorComponent[] sectors_unk2;
	public LightBehaviour[] lights;
	//public string[] debugList4;
	//public string[] debugList5;
	//public LightBehaviour[] dynamicLights;

	private bool active = true;
	private bool neighborActive = true;
	public bool Active {
		get { return active; }
		set { active = value; }
	}
	public bool Loaded {
		get { return active || neighborActive; }
		set { neighborActive = value; }
	}

	public bool IsSectorVirtual {
		get {
			if (sector != null) {
				return sector.isSectorVirtual != 0;
			} else if (sectorROM != null) {
				return sectorROM.isSectorVirtual != 0;
			}
			return false;
		}
	}
	public byte SectorPriority {
		get {
			if (sector != null) {
				return sector.sectorPriority;
			} else if (sectorROM != null) {
				return sectorROM.sectorPriority;
			}
			return 0;
		}
	}

	private BoundingVolume bounds;
	public BoundingVolume SectorBorder {
		get {
			if (bounds == null) {
				if (sector != null) {
					bounds = sector.sectorBorder;
				} else if (sectorROM != null) {
					if (sectorROM.boundingVolume.Value != null) {
						Vector3[] vectors = sectorROM.boundingVolume.Value.GetVectors(32f, switchAxes: true);
						/*vectors[0] = new Vector3(vectors[0].y, vectors[0].x, vectors[0].z);
						vectors[1] = new Vector3(vectors[1].y, vectors[1].x, vectors[1].z);*/
						//print(vectors[0] + " - " + vectors[1] + " - " + sectorROM.float14 + " - " + sectorROM.float18);
						bounds = new BoundingVolume(null) {
							type = BoundingVolume.Type.Box,
							boxMin = vectors[0],
							boxMax = vectors[1],
							//boxCenter = Vector3.Lerp(vectors[0], vectors[1], 0.5f),
							boxSize = vectors[1] - vectors[0],
							boxCenter = vectors[0] + (vectors[1] - vectors[0]) * 0.5f
						};
					}
				} else if (sectorPS1 != null) {
					bounds = sectorPS1.BoundingVolume;
				}
				/*if (bounds != null) {
					if (bounds.type == BoundingVolume.Type.Box) {
						BoxCollider collider = gameObject.AddComponent<BoxCollider>();

						collider.center = bounds.Center;
						collider.center -= transform.position;
						collider.size = bounds.Size;
					}
				}*/
			}
			return bounds;
		}
	}

	public GameObject Gao {
		get {
			if (sectorROM != null) {
				//return transform.parent.gameObject;
				return gameObject;
			} else {
				return gameObject;
			}
		}
	}

	public void SetGameObjectActive(bool active) {
		Gao.SetActive(active);
	}

	// Use this for initialization
	void Start() {
    }

    public void Init() {
		if (gameObject.name.Contains("^Sector:")) {
			gameObject.name = name.Substring(name.IndexOf("^Sector:") + 8);
		}
		gameObject.name = SectorPriority + " - " + gameObject.name;
		//gameObject.name += " - " + IsSectorVirtual;
		if (sector != null) {
			//staticLights = sector.staticLights.Select(l => l.Light).ToArray();
			neighbors = sector.neighbors.Select(s => sectorManager.sectors.First(ns => ns.sector == s.sector)).ToArray();
			sectorTransitions = sector.sectors_unk1.Select(s => sectorManager.sectors.First(ns => ns.sector == s.sector)).ToArray();
			sectors_unk2 = sector.sectors_unk2.Select(s => sectorManager.sectors.First(ns => ns.sector == s)).ToArray();
			//dynamicLights = sector.dynamicLights.Select(l => l.Light).ToArray();
		} else if (sectorROM != null) {
			if (sectorROM.neighbors.Value != null) {
				neighbors = sectorROM.neighbors.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				neighbors = new SectorComponent[0];
			}
			if (sectorROM.sectors2.Value != null) {
				sectors_unk2 = sectorROM.sectors2.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				sectors_unk2 = new SectorComponent[0];
			}
			if (sectorROM.sectors3.Value != null) {
				sectorTransitions = sectorROM.sectors3.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				sectorTransitions = new SectorComponent[0];
			}
			//name += " - " + sectorROM.byte1E + " " + sectorROM.byte1F;
			//neighbors = sectorROM.sectors2.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			//if (sectorROM.sectors4.Value != null) debugList4 = sectorROM.sectors4.Value.superObjects.Select(s => s.Value.Offset.ToString()).ToArray();
			//if (sectorROM.sectors5.Value != null) debugList5 = sectorROM.sectors5.Value.superObjects.Select(s => s.Value.Offset.ToString()).ToArray();
			//BoundingVolume vol = SectorBorder;
		} else if (sectorPS1 != null) {
			OpenSpace.PS1.LevelHeader h = (MapLoader.Loader as R2PS1Loader).levelHeader;
			gameObject.name = sectorPS1.short_46 + " - " + gameObject.name;
			if (sectorPS1.neighbors != null) {
				neighbors = sectorPS1.neighbors.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				neighbors = new SectorComponent[0];
			}
			if (sectorPS1.sectors_unk1 != null) {
				sectorTransitions = sectorPS1.sectors_unk1.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				sectorTransitions = new SectorComponent[0];
			}
			if (sectorPS1.sectors_unk2 != null) {
				sectors_unk2 = sectorPS1.sectors_unk2.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				sectors_unk2 = new SectorComponent[0];
			}
		}
    }
    
}
