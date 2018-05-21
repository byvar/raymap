using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3SuperObject {
        public R3Pointer off_superObject;
        public uint type;
        public R3Pointer off_data;
        public R3Pointer off_child_first;
        public R3Pointer off_child_last;
        public uint num_children;
        public List<R3SuperObject> children;
        public R3Pointer off_brother_next;
        public R3Pointer off_brother_prev;
        public R3Pointer off_parent;
        public R3SuperObject parent;
        public R3Pointer off_matrix;
        public R3Matrix matrix;
        public IR3Data data;
        public GameObject Gao {
            get {
                if (data != null) {
                    return data.Gao;
                } else return null;
            }
        }

        public R3SuperObject(R3Pointer off_superObject) {
            this.off_superObject = off_superObject;
            children = new List<R3SuperObject>();
        }

        public static List<R3SuperObject> Read(EndianBinaryReader reader, R3Pointer off_so, bool parseSiblings = true, bool parseChildren = true, R3SuperObject parent = null) {
            R3Loader l = R3Loader.Loader;
            List<R3SuperObject> superObjects = new List<R3SuperObject>();
            if (IsParsed(off_so)) {
                return null;
            }
            bool isFirstNode = true;
            bool hasNextBrother = false;
            bool isValidNode = true;
            while (isFirstNode || (hasNextBrother && parseSiblings)) {
                R3SuperObject so = new R3SuperObject(off_so);
                superObjects.Add(so); // Local list of superobjects (only this & siblings)
                l.superObjects.Add(so); // Global list of superobjects (all)
                if (parent != null) {
                    parent.children.Add(so);
                    so.parent = parent;
                }
                hasNextBrother = false;
                so.type = reader.ReadUInt32();
                so.off_data = R3Pointer.Read(reader);
                so.off_child_first = R3Pointer.Read(reader);
                so.off_child_last = R3Pointer.Read(reader);
                so.num_children = reader.ReadUInt32();
                so.off_brother_next = R3Pointer.Read(reader);
                so.off_brother_prev = R3Pointer.Read(reader);
                so.off_parent = R3Pointer.Read(reader);
                so.off_matrix = R3Pointer.Read(reader);
                //R3Pointer.Read(reader); // a copy of the matrix right after, at least in R3GC
                Vector3 pos = Vector3.zero;
                Vector3 scale = Vector3.one;
                Quaternion rot = Quaternion.identity;
                if (so.off_matrix != null) {
                    R3Pointer curPos = R3Pointer.Goto(ref reader, so.off_matrix);
                    so.matrix = R3Matrix.Read(reader, so.off_matrix);
                    pos = so.matrix.GetPosition(convertAxes: true);
                    rot = so.matrix.GetRotation(convertAxes: true);
                    scale = so.matrix.GetScale(convertAxes: true);
                    R3Pointer.Goto(ref reader, curPos);
                }
                switch (so.type) {
                    case 0x20: // IPO
                        R3Pointer.Goto(ref reader, so.off_data);
                        so.data = R3IPO.Read(reader, so.off_data, so);
                        break;
                    case 0x40: // IPO
                        l.print("IPO with code 0x40 at offset " + String.Format("0x{0:X}", so.off_superObject.offset));
                        R3Pointer.Goto(ref reader, so.off_data);
                        so.data = R3IPO.Read(reader, so.off_data, so);
                        break;
                    case 0x02: // e.o.
                        R3Pointer.Goto(ref reader, so.off_data);
                        so.data = R3Perso.Read(reader, so.off_data, so);
                        break;
                    case 0x01: // world superobject
                        so.data = R3World.New(so);
                        //print("parsing world superobject with " + num_children + " children");
                        break;
                    case 0x04: // sector
                        R3Pointer.Goto(ref reader, so.off_data);
                        so.data = R3Sector.Read(reader, so.off_data, so);
                        break;
                    default:
                        l.print("Unknown SO type " + so.type + " at offset " + String.Format("0x{0:X}", so.off_superObject.offset));
                        isValidNode = false;
                        break;
                }
                if (so.Gao != null) {
                    if (parent != null) so.Gao.transform.parent = parent.Gao.transform;
                    so.Gao.transform.localPosition = pos;
                    so.Gao.transform.localRotation = rot;
                    so.Gao.transform.localScale = scale;
                }
                isFirstNode = false;
                if (isValidNode) {
                    if (parseChildren && so.num_children > 0 && so.off_child_first != null) {
                        //if (type == 0x01) print("parsing children now");
                        R3Pointer off_current = R3Pointer.Goto(ref reader, so.off_child_first);
                        R3SuperObject.Read(reader, so.off_child_first, true, true, so);
                        //R3Pointer.Goto(ref reader, off_current);
                    }
                    if (so.off_brother_next != null && !IsParsed(so.off_brother_next)) {
                        hasNextBrother = true;
                        R3Pointer.Goto(ref reader, so.off_brother_next);
                        off_so = so.off_brother_next;
                    }
                }
            }
            return superObjects;
        }

        public static R3SuperObject FromOffset(R3Pointer offset) {
            if (offset == null) return null;
            R3Loader l = R3Loader.Loader;
            return l.superObjects.FirstOrDefault(so => so.off_superObject == offset);
        }

        public static bool IsParsed(R3Pointer offset) {
            return FromOffset(offset) != null;
        }
    }
}
