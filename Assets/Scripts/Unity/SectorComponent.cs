using OpenSpace;
using OpenSpace.Collide;
using OpenSpace.Loader;
using OpenSpace.Object;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SectorComponent : MonoBehaviour, IReferenceable {
    public SectorManager sectorManager;
    public Sector sector;
	public OpenSpace.ROM.Sector sectorROM;
	public OpenSpace.PS1.Sector sectorPS1;
    //public LightBehaviour[] staticLights;
    public SectorComponent[] graphicSectors;
    public SectorComponent[] collisionSectors;
    public SectorComponent[] activitySectors;
	public LightBehaviour[] lights;
	public GameObject[] persos;
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

	public bool ContainsPoint(Vector3 point) {
		if (SectorBorder != null) {
			return SectorBorder.ContainsPoint(point);
		}
		/*if (sector?.SuperObject?.boundingVolumeTT != null) {
			GeometricObjectCollide col = sector?.SuperObject?.boundingVolumeTT;
			return col.ContainsPoint(point);
		}*/
		return false;
	}
	public Vector3 CenterPoint {
		get {
			if (SectorBorder != null) {
				return SectorBorder.Center;
			}
			/*if (sector?.SuperObject?.boundingVolumeTT != null) {
				if (convertedBoundsTT == null) {
					GeometricObjectCollide col = sector?.SuperObject?.boundingVolumeTT;
					convertedBoundsTT = col.BoundingBox;
				}
				return convertedBoundsTT.Center;
			}*/
			return Vector3.zero;
		}
	}
	//private BoundingVolume convertedBoundsTT;

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

                if (UnitySettings.VisualizeSectorBorders) {
                    if (bounds != null) {
                        if (bounds.Center.IsValid()) {
                            if (bounds.type == BoundingVolume.Type.Box) {
                                if (bounds.Size.IsValid()) {
                                    BoxCollider collider = gameObject.AddComponent<BoxCollider>();

                                    collider.center = bounds.Center;
                                    collider.center -= transform.position;
                                    collider.size = bounds.Size;
                                } else {
									Debug.LogWarning($"Invalid Sector Bounds Size for sector {sector.Gao}");
                                }
                            } else if (bounds.type == BoundingVolume.Type.Sphere) {
                                SphereCollider collider = gameObject.AddComponent<SphereCollider>();

                                collider.center = bounds.Center;
                                collider.center -= transform.position;
                                collider.radius = bounds.sphereRadius;
                            }
                        } else {
							Debug.LogWarning($"Invalid Sector Bounds Center for sector {sector.Gao}");
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

    public ReferenceFields References { get => ((IReferenceable)sector).References; set => ((IReferenceable)sector).References = value; }

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
		//gameObject.name = SectorPriority + " - " + gameObject.name;
		//gameObject.name += " - " + IsSectorVirtual;
		if (sector != null) {
			//staticLights = sector.staticLights.Select(l => l.Light).ToArray();
			graphicSectors = sector.graphicSectors.Select(s => sectorManager.sectors.First(ns => ns.sector == s.sector)).ToArray();
			collisionSectors = sector.collisionSectors.Select(s => sectorManager.sectors.First(ns => ns.sector == s.sector)).ToArray();
			activitySectors = sector.activitySectors.Select(s => sectorManager.sectors.First(ns => ns.sector == s)).ToArray();
			persos = sector.persos.Select(p => p.Gao).ToArray();
			//dynamicLights = sector.dynamicLights.Select(l => l.Light).ToArray();
		} else if (sectorROM != null) {
			if (sectorROM.graphicSectors.Value != null) {
				graphicSectors = sectorROM.graphicSectors.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				graphicSectors = new SectorComponent[0];
			}
			if (sectorROM.activitySectors.Value != null) {
				activitySectors = sectorROM.activitySectors.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				activitySectors = new SectorComponent[0];
			}
			if (sectorROM.collisionSectors.Value != null) {
				collisionSectors = sectorROM.collisionSectors.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			} else {
				collisionSectors = new SectorComponent[0];
			}
			//name += " - " + sectorROM.byte1E + " " + sectorROM.byte1F;
			//neighbors = sectorROM.sectors2.Value.superObjects.Select(s => sectorManager.sectors.First(ns => ns.sectorROM == (s.Value.data.Value as OpenSpace.ROM.Sector))).ToArray();
			//if (sectorROM.sectors4.Value != null) debugList4 = sectorROM.sectors4.Value.superObjects.Select(s => s.Value.Offset.ToString()).ToArray();
			//if (sectorROM.sectors5.Value != null) debugList5 = sectorROM.sectors5.Value.superObjects.Select(s => s.Value.Offset.ToString()).ToArray();
			//BoundingVolume vol = SectorBorder;
		} else if (sectorPS1 != null) {
			OpenSpace.PS1.LevelHeader h = (MapLoader.Loader as R2PS1Loader).levelHeader;
			//gameObject.name = sectorPS1.short_46 + " - " + gameObject.name;
			if (sectorPS1.graphicSectors != null) {
				graphicSectors = sectorPS1.graphicSectors.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				graphicSectors = new SectorComponent[0];
			}
			if (sectorPS1.collisionSectors != null) {
				collisionSectors = sectorPS1.collisionSectors.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				collisionSectors = new SectorComponent[0];
			}
			if (sectorPS1.activitySectors != null) {
				activitySectors = sectorPS1.activitySectors.Select(s => sectorManager.sectors.First(ns => ns.sectorPS1 == s.Sector)).ToArray();
			} else {
				activitySectors = new SectorComponent[0];
			}
		}
    }
    
}
