using OpenSpace;
using OpenSpace.Collide;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorComponent : MonoBehaviour {
    public SectorManager sectorManager;
    public Sector sector;
	public OpenSpace.ROM.Sector sectorROM;
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

						if (bounds != null) {
							if (bounds.type == BoundingVolume.Type.Box) {
								BoxCollider collider = gameObject.AddComponent<BoxCollider>();

								collider.center = bounds.Center;
								collider.center -= transform.position;
								collider.size = bounds.Size;
							}
							/*GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
							go.name = name + " - Mesh";
							go.transform.position = bounds.Center;
							go.transform.localScale = bounds.Size;*/
						}
					}
				}
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
		}
    }
    
}
