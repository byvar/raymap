using OpenSpace.EngineObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    public class SuperObject {
        public Pointer offset;
        public uint type;
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

        public static List<SuperObject> Read(EndianBinaryReader reader, Pointer off_so, bool parseSiblings = true, bool parseChildren = true, SuperObject parent = null) {
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
                so.type = reader.ReadUInt32();
                so.off_data = Pointer.Read(reader);
                so.off_child_first = Pointer.Read(reader);
                so.off_child_last = Pointer.Read(reader);
                so.num_children = reader.ReadUInt32();
                so.off_brother_next = Pointer.Read(reader);
                so.off_brother_prev = Pointer.Read(reader);
                so.off_parent = Pointer.Read(reader);
                so.off_matrix = Pointer.Read(reader); // 0x20->0x24
                reader.ReadInt32();
                so.superObjectFlags = reader.ReadInt32();

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
                switch (so.type) {
                    case 0x20: // IPO
                        Pointer.Goto(ref reader, so.off_data);
                        so.data = IPO.Read(reader, so.off_data, so);
                        break;
                    case 0x40: // IPO
                        l.print("IPO with code 0x40 at offset " + String.Format("0x{0:X}", so.offset.offset));
                        Pointer.Goto(ref reader, so.off_data);
                        so.data = IPO.Read(reader, so.off_data, so);
                        break;
                    case 0x02: // e.o.
                        Pointer.Goto(ref reader, so.off_data);
                        so.data = Perso.Read(reader, so.off_data, so);
                        break;
                    case 0x01: // world superobject
                        so.data = World.New(so);
                        //print("parsing world superobject with " + num_children + " children");
                        break;
                    case 0x04: // sector
                        Pointer.Goto(ref reader, so.off_data);
                        so.data = Sector.Read(reader, so.off_data, so);
                        break;
                    default:
                        l.print("Unknown SO type " + so.type + " at offset " + String.Format("0x{0:X}", so.offset.offset));
                        isValidNode = false;
                        break;
                }
                if (so.Gao != null) {
                    if (parent != null) so.Gao.transform.parent = parent.Gao.transform;
                    so.Gao.transform.localPosition = pos;
                    so.Gao.transform.localRotation = rot;
                    so.Gao.transform.localScale = scale;

                    SuperObjectFlags soFlags = so.Gao.AddComponent<SuperObjectFlags>();
                    soFlags.SetRawFlags(so.superObjectFlags);
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
    }
}
