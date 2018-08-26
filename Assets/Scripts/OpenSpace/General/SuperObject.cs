using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class SuperObject {
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
        public Pointer off_child_first;
        public Pointer off_child_last;
        public uint num_children;
        public List<SuperObject> children;
        public Pointer off_brother_next;
        public Pointer off_brother_prev;
        public Pointer off_parent;
        public SuperObject parent;
        public Pointer off_matrix;
        public Matrix matrix;
        public IEngineObject data;
        public int superObjectFlags;
        public BoundingVolume boundingVolume;

        public GameObject Gao {
            get {
                if (data != null) {
                    return data.Gao;
                } else return null;
            }
        }

        public SuperObject(Pointer offset) {
            this.offset = offset;
            children = new List<SuperObject>();
        }

        public static List<SuperObject> Read(Reader reader, Pointer off_so, bool parseSiblings = true, bool parseChildren = true, SuperObject parent = null) {
            MapLoader l = MapLoader.Loader;
            List<SuperObject> superObjects = new List<SuperObject>();
            if (IsParsed(off_so)) {
                return null;
            }
            bool isFirstNode = true;
            bool hasNextBrother = false;
            bool isValidNode = true;
            while (isFirstNode || (hasNextBrother && parseSiblings)) {
                SuperObject so = new SuperObject(off_so);
                superObjects.Add(so); // Local list of superobjects (only this & siblings)
                l.superObjects.Add(so); // Global list of superobjects (all)
                if (parent != null) {
                    parent.children.Add(so);
                    so.parent = parent;
                }
                hasNextBrother = false;
                so.typeCode = reader.ReadUInt32(); // 0 - 4
                so.off_data = Pointer.Read(reader); // 4 - 8
                so.off_child_first = Pointer.Read(reader); // 8 - C
                so.off_child_last = Pointer.Read(reader); // C - 10
                so.num_children = reader.ReadUInt32(); // 10 - 14
                so.off_brother_next = Pointer.Read(reader); // 14 - 18
                so.off_brother_prev = Pointer.Read(reader); // 18 - 1C
                so.off_parent = Pointer.Read(reader); // 1C - 20
                so.off_matrix = Pointer.Read(reader); // 0x20->0x24
                Pointer.Read(reader); // other matrix
                reader.ReadInt32(); // 0x28 -> 0x2C
                reader.ReadInt32(); // 0x2C -> 0x30
                so.superObjectFlags = reader.ReadInt32(); // 0x30->0x34
                if (Settings.s.engineVersion == Settings.EngineVersion.R3) reader.ReadUInt32();
                Pointer off_boundingVolume = Pointer.Read(reader);

                //R3Pointer.Read(reader); // a copy of the matrix right after, at least in R3GC
                Vector3 pos = Vector3.zero;
                Vector3 scale = Vector3.one;
                Quaternion rot = Quaternion.identity;
                if (so.off_matrix != null) {
                    Pointer curPos = Pointer.Goto(ref reader, so.off_matrix);
                    so.matrix = Matrix.Read(reader, so.off_matrix);
                    pos = so.matrix.GetPosition(convertAxes: true);
                    rot = so.matrix.GetRotation(convertAxes: true);
                    scale = so.matrix.GetScale(convertAxes: true);
                    Pointer.Goto(ref reader, curPos);
                }
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

                SuperObjectFlags soFlags = null;
                if (so.Gao != null) {
                    soFlags = so.Gao.AddComponent<SuperObjectFlags>();
                    soFlags.SetRawFlags(so.superObjectFlags);
                }

                if (off_boundingVolume != null && soFlags != null) {
                    Pointer original = Pointer.Goto(ref reader, off_boundingVolume);
                    so.boundingVolume = BoundingVolume.Read(reader, off_boundingVolume, soFlags.BoundingBoxInsteadOfSphere ?
                        BoundingVolume.Type.Box : BoundingVolume.Type.Sphere);
                    Pointer.Goto(ref reader, original);
                }

                if (so.Gao != null) {
                    if (parent != null && parent.Gao != null) so.Gao.transform.parent = parent.Gao.transform;
                    so.Gao.transform.localPosition = pos;
                    so.Gao.transform.localRotation = rot;
                    so.Gao.transform.localScale = scale;

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
                isFirstNode = false;
                if (isValidNode) {
                    if (parseChildren && so.num_children > 0 && so.off_child_first != null) {
                        //if (type == 0x01) print("parsing children now");
                        Pointer off_current = Pointer.Goto(ref reader, so.off_child_first);
                        SuperObject.Read(reader, so.off_child_first, true, true, so);
                        //R3Pointer.Goto(ref reader, off_current);
                    }
                    if (so.off_brother_next != null && !IsParsed(so.off_brother_next)) {
                        hasNextBrother = true;
                        Pointer.Goto(ref reader, so.off_brother_next);
                        off_so = so.off_brother_next;
                    }
                }
            }
            return superObjects;
        }

        public static SuperObject FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.superObjects.FirstOrDefault(so => so.offset == offset);
        }

        public static bool IsParsed(Pointer offset) {
            return FromOffset(offset) != null;
        }

        public static Type GetSOType(uint typeCode) {
            Type type = Type.Unknown;
            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
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
