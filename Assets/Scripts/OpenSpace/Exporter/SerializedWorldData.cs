using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class SerializedWorldData {

        public struct ESector {
            public VisualMaterial SkyMaterial;
            public List<String> Neighbours;
            public Dictionary<string, EGeometry> Geometry;
            public BoundingVolume SectorBorder;
            public List<LightInfo> LightInfo;
            public bool Virtual;
        }

        public struct EGeometry { // IPO
            public MeshObject Visuals;
            public CollideMeshObject Collision;
        }

        public Dictionary<string, ESector> Sectors;

        public ESector CreateESector(Sector sector)
        {
            ESector eSector = new ESector()
            {
                SkyMaterial = sector.skyMaterial,
                SectorBorder = sector.sectorBorder,
                LightInfo = sector.staticLights.ToList()
            };

            eSector.Neighbours = new List<String>();
            foreach (Sector.NeighborSector sn in sector.neighbors) {
                eSector.Neighbours.Add(sn.sector.offset.ToString());
            }

            eSector.Geometry = new Dictionary<string, EGeometry>();
            foreach(SuperObject spo in sector.SuperObject.children) {

                IPO ipo = spo.data as IPO;
                if (ipo != null) { // If IPO

                    EGeometry eg = new EGeometry();

                    PhysicalObject po = ipo.data;
                    if (po.visualSet.Length > 0) {
                        eg.Visuals = po.visualSet[0].obj as MeshObject;
                        eg.Collision = po.collideMesh;
                    }

                    eSector.Geometry.Add(ipo.offset.ToString(), eg);
                }
            }

            return eSector;
        }

        public SerializedWorldData(List<Sector> sectors)
        {
            Sectors = new Dictionary<string, ESector>();

            foreach (Sector sector in sectors) {
                Sectors.Add(sector.offset.ToString(), CreateESector(sector));
            }
        }
    }
}
