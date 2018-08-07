using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Collide {
    public class CollideSpheresElement : ICollideGeometricElement {
        public class IndexedSphere {
            public float radius;
            public Pointer off_material;
            public ushort centerPoint;

            public GameMaterial gameMaterial;
        }


        public CollideMeshObject mesh;
        public Pointer offset;

        public Pointer off_spheres; // called IndexedSprites in the game code
        public ushort num_spheres;
        public IndexedSphere[] spheres;

        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("Collide Spheres @ " + offset);// Create object and read triangle data
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public CollideSpheresElement(Pointer offset, CollideMeshObject mesh) {
            this.mesh = mesh;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            for (uint i = 0; i < num_spheres; i++) {
                GameObject sphere_gao = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere_gao.name = "Collide Spheres @ " + offset + " - " + i;
                sphere_gao.transform.SetParent(gao.transform);
                MeshFilter mf = sphere_gao.GetComponent<MeshFilter>();
                MeshRenderer mr = sphere_gao.GetComponent<MeshRenderer>();
                sphere_gao.transform.localPosition = mesh.vertices[spheres[i].centerPoint];
                sphere_gao.transform.localScale = Vector3.one * spheres[i].radius;

                mr.material = MapLoader.Loader.collideMaterial;
                if (spheres[i].gameMaterial != null && spheres[i].gameMaterial.collideMaterial != null) {
                    spheres[i].gameMaterial.collideMaterial.SetMaterial(mr);
                }
            }
        }

        public static CollideSpheresElement Read(Reader reader, Pointer offset, CollideMeshObject m) {
            MapLoader l = MapLoader.Loader;
            CollideSpheresElement s = new CollideSpheresElement(offset, m);
            s.off_spheres = Pointer.Read(reader);
            s.num_spheres = reader.ReadUInt16();
            reader.ReadInt16(); // -1
            reader.ReadInt16();

            if (s.off_spheres != null) {
                Pointer off_current = Pointer.Goto(ref reader, s.off_spheres);
                s.spheres = new IndexedSphere[s.num_spheres];
                for (uint i = 0; i < s.num_spheres; i++) {
                    s.spheres[i] = new IndexedSphere();
                    s.spheres[i].radius = reader.ReadSingle();
                    s.spheres[i].off_material = Pointer.Read(reader);
                    s.spheres[i].centerPoint = reader.ReadUInt16();
                    reader.ReadUInt16();
                    s.spheres[i].gameMaterial = GameMaterial.FromOffsetOrRead(s.spheres[i].off_material, reader);
                }
                Pointer.Goto(ref reader, off_current);
            }
            return s;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public ICollideGeometricElement Clone(CollideMeshObject mesh) {
            CollideSpheresElement sm = (CollideSpheresElement)MemberwiseClone();
            sm.mesh = mesh;
            sm.Reset();
            return sm;
        }
    }
}
