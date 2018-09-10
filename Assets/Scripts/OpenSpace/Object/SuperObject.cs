using OpenSpace.Object.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Object {
    public class SuperObject : ILinkedListEntry {
        public enum Type {
            Unknown,
            World,
            IPO,
            IPO_2,
            Perso,
            Sector
        }

        public Pointer offset;
        public uint typeCode;
        public Type type;
        public Pointer off_data;
        public LinkedList<SuperObject> children;
        public Pointer off_brother_next;
        public Pointer off_brother_prev;
        public Pointer off_parent;
        public SuperObject parent;
        public Pointer off_matrix;
        public Matrix matrix;
        public IEngineObject data;
        public SuperObjectFlags flags;
        public BoundingVolume boundingVolume;

        public GameObject Gao {
            get {
                if (data != null) {
                    return data.Gao;
                } else return null;
            }
        }

        public Pointer NextEntry {
            get {
                return off_brother_next;
            }
        }

        public Pointer PreviousEntry {
            get {
                return off_brother_prev;
            }
        }

        public SuperObject(Pointer offset) {
            this.offset = offset;
        }

        public static SuperObject Read(Reader reader, Pointer off_so, SuperObject parent = null) {
            MapLoader l = MapLoader.Loader;
            if (IsParsed(off_so)) return null;
            bool isValidNode = true;
            SuperObject so = new SuperObject(off_so);
            l.superObjects.Add(so); // Global list of superobjects (all)
            if (parent != null) {
                so.parent = parent;
            }
            so.typeCode = reader.ReadUInt32(); // 0 - 4
            so.off_data = Pointer.Read(reader); // 4 - 8
            so.children = LinkedList<SuperObject>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double); // 8 - 14
            so.off_brother_next = Pointer.Read(reader); // 14 - 18
            so.off_brother_prev = Pointer.Read(reader); // 18 - 1C
            so.off_parent = Pointer.Read(reader); // 1C - 20
            so.off_matrix = Pointer.Read(reader); // 0x20->0x24
            Pointer.Read(reader); // other matrix
            reader.ReadInt32(); // 0x28 -> 0x2C
            reader.ReadInt32(); // 0x2C -> 0x30
            so.flags = SuperObjectFlags.Read(reader); // 0x30->0x34
            if (Settings.s.engineVersion == Settings.EngineVersion.R3) reader.ReadUInt32();
            Pointer off_boundingVolume = Pointer.Read(reader);
            //R3Pointer.Read(reader); // a copy of the matrix right after, at least in R3GC
            Vector3 pos = Vector3.zero;
            Vector3 scale = Vector3.one;
            Quaternion rot = Quaternion.identity;
            Pointer.DoAt(ref reader, so.off_matrix, () => {
                so.matrix = Matrix.Read(reader, so.off_matrix);
                pos = so.matrix.GetPosition(convertAxes: true);
                rot = so.matrix.GetRotation(convertAxes: true);
                scale = so.matrix.GetScale(convertAxes: true);
            });
            so.type = GetSOType(so.typeCode);
            switch (so.type) {
                case Type.IPO:
                    Pointer.Goto(ref reader, so.off_data);
                    so.data = IPO.Read(reader, so.off_data, so);
                    break;
                case Type.IPO_2:
                    l.print("IPO with code 0x40 at offset " + String.Format("0x{0:X}", so.offset.offset));
                    Pointer.Goto(ref reader, so.off_data);
                    so.data = IPO.Read(reader, so.off_data, so);
                    break;
                case Type.Perso:
                    Pointer.Goto(ref reader, so.off_data);
                    so.data = Perso.Read(reader, so.off_data, so);
                    break;
                case Type.World:
                    so.data = World.New(so);
                    //print("parsing world superobject with " + num_children + " children");
                    break;
                case Type.Sector:
                    Pointer.Goto(ref reader, so.off_data);
                    so.data = Sector.Read(reader, so.off_data, so);
                    break;
                default:
                    l.print("Unknown SO type " + so.typeCode + " at offset " + String.Format("0x{0:X}", so.offset.offset));
                    //isValidNode = false;
                    break;
            }

            Pointer.DoAt(ref reader, off_boundingVolume, () => {
                so.boundingVolume = BoundingVolume.Read(reader, off_boundingVolume, so.flags.HasFlag(SuperObjectFlags.Flags.BoundingBoxInsteadOfSphere) ?
                    BoundingVolume.Type.Box : BoundingVolume.Type.Sphere);
            });

            if (so.Gao != null) {
                if (parent != null && parent.Gao != null) so.Gao.transform.parent = parent.Gao.transform;
                so.Gao.transform.localPosition = pos;
                so.Gao.transform.localRotation = rot;
                so.Gao.transform.localScale = scale;

                SuperObjectComponent soc = so.Gao.AddComponent<SuperObjectComponent>();
                soc.so = so;

                if (so.boundingVolume != null) {
                    if (so.boundingVolume.type == BoundingVolume.Type.Box) {
                        BoxCollider collider = so.Gao.AddComponent<BoxCollider>();

                        collider.center = so.boundingVolume.boxCenter;
                        collider.center -= so.Gao.transform.position;
                        collider.size = so.boundingVolume.boxSize;
                    } else {
                        SphereCollider collider = so.Gao.AddComponent<SphereCollider>();

                        collider.center = so.boundingVolume.sphereCenter;
                        collider.radius = so.boundingVolume.sphereRadius;
                    }
                }
            }
            if (isValidNode) {
                so.children.ReadEntries(ref reader, (off_child) => {
                    SuperObject child = SuperObject.Read(reader, off_child, so);
                    child.parent = so;
                    return child;
                }, LinkedList.Flags.HasHeaderPointers);
            }
            return so;
        }

        public static SuperObject FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.superObjects.FirstOrDefault(so => so.offset == offset);
        }

        public static SuperObject FromOffsetOrRead(Pointer offset, Reader reader, SuperObject parent = null) {
            if (offset == null) return null;
            SuperObject so = FromOffset(offset);
            if (so == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    so = SuperObject.Read(reader, offset, parent: parent);
                });
            }
            return so;
        }

        public static bool IsParsed(Pointer offset) {
            return FromOffset(offset) != null;
        }

        public static Type GetSOType(uint typeCode) {
            Type type = Type.Unknown;
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                switch (typeCode) {
                    case 0x1: type = Type.World; break;
                    case 0x2: type = Type.Perso; break;
                    case 0x4: type = Type.Sector; break;
                    case 0x20: type = Type.IPO; break;
                    case 0x40: type = Type.IPO_2; break;
                }
            } else {
                switch (typeCode) {
                    case 0x0: type = Type.World; break;
                    case 0x4: type = Type.Perso; break;
                    case 0x8: type = Type.Sector; break;
                    case 0xD: type = Type.IPO; break;
                    case 0x15: type = Type.IPO_2; break;
                }
            }
            return type;
        }
    }
}
