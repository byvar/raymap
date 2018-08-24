using OpenSpace.Collide;
using OpenSpace.Visual;
using OpenSpace.Visual.Deform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class PhysicalObject : IEquatable<PhysicalObject> {
        public Pointer offset;
        public Pointer off_visualSet;
        public Pointer off_collideSet;
        public Pointer off_visualBoundingVolume;
        public Pointer off_collideBoundingVolume;
        public VisualSetLOD[] visualSet;
        public CollideMeshObject collideMesh;
        public Vector3? scaleMultiplier = null;
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("[PO]");
                }
                return gao;
            }
        }
        public DeformSet Bones {
            get {
                for (int i = 0; i < visualSet.Length; i++) {
                    if (visualSet[i].obj != null && visualSet[i].obj is MeshObject && ((MeshObject)visualSet[i].obj).bones != null) {
                        return ((MeshObject)visualSet[i].obj).bones;
                    }
                }
                return null;
            }
        }

        public PhysicalObject(Pointer offset) {
            this.offset = offset;
            visualSet = new VisualSetLOD[0];
        }
        public override bool Equals(System.Object obj) {
            return obj is PhysicalObject && this == (PhysicalObject)obj;
        }
        public override int GetHashCode() {
            return offset.GetHashCode();
        }

        public bool Equals(PhysicalObject other) {
            return this == (PhysicalObject)other;
        }

        public static bool operator ==(PhysicalObject x, PhysicalObject y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.offset == y.offset;
        }
        public static bool operator !=(PhysicalObject x, PhysicalObject y) {
            return !(x == y);
        }

        public static PhysicalObject Read(Reader reader, Pointer offset) {
            PhysicalObject po = new PhysicalObject(offset);

            // Header
            po.off_visualSet = Pointer.Read(reader);
            po.off_collideSet = Pointer.Read(reader);
            po.off_visualBoundingVolume = Pointer.Read(reader);
            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    po.off_collideBoundingVolume = po.off_visualBoundingVolume;
                    reader.ReadUInt32();
                } else {
                    po.off_collideBoundingVolume = Pointer.Read(reader);
                }
            }

            // Parse visual set
            if (po.off_visualSet != null) {
                Pointer.Goto(ref reader, po.off_visualSet);
                reader.ReadUInt32(); // 0
                ushort numberOfLOD = reader.ReadUInt16();
                if (numberOfLOD > 1) MapLoader.Loader.print("Found a PO with " + numberOfLOD + " levels of detail @ " + offset);
                ushort type = reader.ReadUInt16();
                if (numberOfLOD > 0) {
                    Pointer off_LODDistances = Pointer.Read(reader);
                    Pointer off_LODDataOffsets = Pointer.Read(reader);
                    reader.ReadUInt32(); // always 0?
                    reader.ReadUInt32(); // always 0?
                    po.visualSet = new VisualSetLOD[numberOfLOD];
                    for (uint i = 0; i < numberOfLOD; i++) {
                        po.visualSet[i] = new VisualSetLOD();
                    }
                    Pointer.DoAt(ref reader, off_LODDistances, () => {
                        for (uint i = 0; i < numberOfLOD; i++) {
                            // if distance > the float at this offset, game engine uses next LOD if there is one
                            po.visualSet[i].LODdistance = reader.ReadSingle();
                        }
                    });
                    Pointer.DoAt(ref reader, off_LODDataOffsets, () => {
                        for (uint i = 0; i < numberOfLOD; i++) {
                            po.visualSet[i].off_data = Pointer.Read(reader);
                        }
                    });
                    for (uint i = 0; i < numberOfLOD; i++) {
                        if (po.visualSet[i].off_data != null) {
                            Pointer off_current = Pointer.Goto(ref reader, po.visualSet[i].off_data);
                            switch (type) {
                                case 0:
                                    po.visualSet[i].obj = MeshObject.Read(reader, po, po.visualSet[i].off_data);
                                    MeshObject m = ((MeshObject)po.visualSet[i].obj);
                                    if (m.name != "Mesh") po.Gao.name = "[PO] " + m.name;
                                    m.gao.transform.parent = po.Gao.transform;
                                    break;
                                case 1:
                                    po.visualSet[i].obj = UnknownGeometricObject.Read(reader, po, po.visualSet[i].off_data);
                                    break;
                                default:
                                    MapLoader.Loader.print("unknown type " + type + " at offset: " + offset);
                                    break;
                            }
                            Pointer.Goto(ref reader, off_current);
                        }
                    }
                }
            }

            // Parse collide set
            if (po.off_collideSet != null) {
                Pointer.Goto(ref reader, po.off_collideSet);
                uint u1 = reader.ReadUInt32(); // 0
                uint u2 = reader.ReadUInt32(); // 0
                uint u3 = reader.ReadUInt32(); // 0
                //MapLoader.Loader.print(po.off_collideSet);
                /*MapLoader.Loader.print(u1);
                MapLoader.Loader.print(u2);
                MapLoader.Loader.print(u3);*/
                Pointer off_zdr = Pointer.Read(reader);
                if (off_zdr != null) {
                    //R3Loader.Loader.print("Collide mesh offset: " + off_mesh);
                    Pointer.Goto(ref reader, off_zdr);
                    po.collideMesh = CollideMeshObject.Read(reader, off_zdr);
                    po.collideMesh.gao.transform.parent = po.Gao.transform;
                }
                //R3Loader.Loader.print("Collide set: " + po.off_collideSet + " - vol: " + po.off_visualBoundingVolume);
            }
            MapLoader.Loader.physicalObjects.Add(po);
            return po;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public PhysicalObject Clone() {
            PhysicalObject po = (PhysicalObject)MemberwiseClone();
            po.visualSet = new VisualSetLOD[visualSet.Length];
            po.Reset();
            for (int i = 0; i < visualSet.Length; i++) {
                po.visualSet[i].LODdistance = visualSet[i].LODdistance;
                po.visualSet[i].off_data = visualSet[i].off_data;
                po.visualSet[i].obj = visualSet[i].obj.Clone();
                if (po.visualSet[i].obj is MeshObject) {
                    MeshObject m = ((MeshObject)po.visualSet[i].obj);
                    if (m.name != "Mesh") po.Gao.name = "[PO] " + m.name;
                    m.gao.transform.parent = po.Gao.transform;
                }
            }
            if (collideMesh != null) {
                po.collideMesh = collideMesh.Clone();
                po.collideMesh.gao.transform.parent = po.Gao.transform;
            }
            MapLoader.Loader.physicalObjects.Add(po);
            return po;
        }

        public void Destroy() {
            if (gao != null) GameObject.Destroy(gao);
        }
    }
}
