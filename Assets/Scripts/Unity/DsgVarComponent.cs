using UnityEngine;
using System.Collections;
using OpenSpace.Object;
using OpenSpace.AI;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using OpenSpace.Waypoints;
using OpenSpace;

public class DsgVarComponent : MonoBehaviour
{
    public Perso perso;
    public DsgMem dsgMem;
    public DsgVar dsgVar;
    public DsgVarInfoEntry[] dsgVarEntries;
    public DsgVarEditableEntry[] editableEntries;

    public class DsgVarEditableEntry {
        public DsgVarInfoEntry entry;
        public int number;
        public string Name {
            get {
                return entry.NiceVariableName;
            }
        }
        public bool IsArray {
            get {
                return DsgVarInfoEntry.GetDsgVarTypeFromArrayType(entry.type) != DsgVarInfoEntry.DsgVarType.None;
            }
        }
        public DsgVarInfoEntry.DsgVarType Type {
            get {
                return entry.type;
            }
        }
        public int ArrayLength {
            get {
                if (IsArray) {
                    object[] array = (object[])entry.value;
                    if (array != null) return array.Length;
                }
                return 0;
            }
        }

        // Values for different types
        public class Value {
            public DsgVarInfoEntry.DsgVarType type;

            public bool valueAsBool;
            public uint valueAsUInt;
            public int valueAsInt;
            public sbyte valueAsSByte;
            public byte valueAsByte;
            public short valueAsShort;
            public ushort valueAsUShort;
            public float valueAsFloat;
            public Vector3 valueAsVector;
            public GameObject valueAsSuperObjectGao;
            public GameObject valueAsWaypointGao;
            public GameObject valueAsPersoGao;
            public string valueAsString;
            public Value[] valueAsArray;

            public void InitValue(DsgVarInfoEntry.DsgVarType type, object value) {
                this.type = type;

                switch (type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean: valueAsBool = (bool)value; break;
                    case DsgVarInfoEntry.DsgVarType.Int: valueAsInt = (int)value; break;
                    case DsgVarInfoEntry.DsgVarType.UInt: valueAsUInt = (uint)value; break;
                    case DsgVarInfoEntry.DsgVarType.Short: valueAsShort = (short)value; break;
                    case DsgVarInfoEntry.DsgVarType.UShort: valueAsUShort = (ushort)value; break;
                    case DsgVarInfoEntry.DsgVarType.Byte: valueAsSByte = (sbyte)value; break;
                    case DsgVarInfoEntry.DsgVarType.UByte: valueAsByte = (byte)value; break;
                    case DsgVarInfoEntry.DsgVarType.Float: valueAsFloat = (float)value; break;
                    case DsgVarInfoEntry.DsgVarType.Vector: valueAsVector = (Vector3)value; break;
                    case DsgVarInfoEntry.DsgVarType.Text: valueAsUInt = (uint)value; break;
                    case DsgVarInfoEntry.DsgVarType.Perso:
                        if (value != null && value is Pointer) {
                            Perso perso = MapLoader.Loader.persos.FirstOrDefault(p => (p.SuperObject != null && p.SuperObject.offset == (Pointer)value)); // find perso that belongs to the superobject
                            if (perso != null) {
                                valueAsPersoGao = perso.Gao;
                            }
                        }
                        break;

                    case DsgVarInfoEntry.DsgVarType.SuperObject:
                        if (value != null) {
                            SuperObject spo = MapLoader.Loader.superObjects.FirstOrDefault(p => (p.offset != null && p.offset == (Pointer)value));
                            if (spo != null) {
                                valueAsSuperObjectGao = spo.Gao;
                            }
                        }
                        break;
                    case DsgVarInfoEntry.DsgVarType.WayPoint:
                        if (value != null) {
                            WayPoint wp = null;
                            if (value is Pointer) {
                                wp = WayPoint.FromOffset(value as Pointer);
                                //wp = (WayPoint)entry.value;
                            }

                            if (wp != null) {
                                valueAsWaypointGao = wp.Gao;
                            }
                        }
                        break;
                    case DsgVarInfoEntry.DsgVarType.ActionArray:
                    case DsgVarInfoEntry.DsgVarType.FloatArray:
                    case DsgVarInfoEntry.DsgVarType.Array11:
                    case DsgVarInfoEntry.DsgVarType.Array6:
                    case DsgVarInfoEntry.DsgVarType.Array9:
                    case DsgVarInfoEntry.DsgVarType.IntegerArray:
                    case DsgVarInfoEntry.DsgVarType.PersoArray:
                    case DsgVarInfoEntry.DsgVarType.SoundEventArray:
                    case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
                    case DsgVarInfoEntry.DsgVarType.TextArray:
                    case DsgVarInfoEntry.DsgVarType.TextRefArray:
                    case DsgVarInfoEntry.DsgVarType.VectorArray:
                    case DsgVarInfoEntry.DsgVarType.WayPointArray:
                        if (value.GetType().IsArray) {
                            object[] array = (object[])value;
                            DsgVarInfoEntry.DsgVarType elementType = DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type);
                            if (elementType != DsgVarInfoEntry.DsgVarType.None) {
                                valueAsArray = new Value[array.Length];
                                for (int i = 0; i < array.Length; i++) {
                                    valueAsArray[i] = new Value();
                                    valueAsArray[i].InitValue(elementType, array[i]);
                                }
                            }
                        }
                        break;
                }
            }

            public void Write(Pointer offset, Writer writer) {
                Pointer.DoAt(ref writer, offset, () => {
                    switch (type) {
                        case DsgVarInfoEntry.DsgVarType.Boolean:    writer.Write(valueAsBool); break;
                        case DsgVarInfoEntry.DsgVarType.Int:        writer.Write(valueAsInt); break;
                        case DsgVarInfoEntry.DsgVarType.UInt:       writer.Write(valueAsUInt); break;
                        case DsgVarInfoEntry.DsgVarType.Short:      writer.Write(valueAsShort); break;
                        case DsgVarInfoEntry.DsgVarType.UShort:     writer.Write(valueAsUShort); break;
                        case DsgVarInfoEntry.DsgVarType.Byte:       writer.Write(valueAsSByte); break;
                        case DsgVarInfoEntry.DsgVarType.UByte:      writer.Write(valueAsByte); break;
                        case DsgVarInfoEntry.DsgVarType.Float:      writer.Write(valueAsFloat); break;
                        case DsgVarInfoEntry.DsgVarType.Vector:     writer.Write(valueAsVector.x); writer.Write(valueAsVector.y); writer.Write(valueAsVector.z); break;
                    }
                });
            }

            public override string ToString() {
                string stringVal = "";

                switch (type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean: stringVal = valueAsBool.ToString().ToLower(); break;
                    case DsgVarInfoEntry.DsgVarType.Int:     stringVal = valueAsInt.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.UInt:    stringVal = valueAsUInt.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.Short:   stringVal = valueAsShort.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.UShort:  stringVal = valueAsUShort.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.Byte:    stringVal = valueAsSByte.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.UByte:   stringVal = valueAsByte.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.Float:   stringVal = valueAsFloat.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.Text:    stringVal = valueAsString.ToString(); break;
                    case DsgVarInfoEntry.DsgVarType.Vector:
                        float val_x = valueAsVector.x;
                        float val_y = valueAsVector.y;
                        float val_z = valueAsVector.z;

                        stringVal = "new Vector3(" + val_x + ", " + val_y + ", " + val_z + ")";
                        break;
                    case DsgVarInfoEntry.DsgVarType.Perso:
                        PersoBehaviour currentPersoBehaviour = valueAsPersoGao != null ? valueAsPersoGao.GetComponent<PersoBehaviour>() : null;

                        if (currentPersoBehaviour != null) {
                            stringVal += "Perso.GetByName(" + currentPersoBehaviour.perso.namePerso + ")";
                        } else {
                            stringVal += "null";
                        }

                        break;
                    case DsgVarInfoEntry.DsgVarType.SuperObject:
                        GameObject currentGao = valueAsSuperObjectGao != null ? valueAsSuperObjectGao : null;

                        if (currentGao != null) {
                            stringVal += "GameObject.GetByName(" + currentGao.name + ")";
                        } else {
                            stringVal += "null";
                        }
                        break;

                }

                return stringVal;
            }
        }
        public Value valueCurrent;
        public Value valueInitial;

        public DsgVarEditableEntry(int number, DsgVarInfoEntry entry)
        {
            this.number = number;
            this.entry = entry;
            //print(entry.typeNumber + " - " + entry.type + " - " + entry.debugValueOffset + " - " + entry.value);

            valueCurrent = new Value();
            valueCurrent.InitValue(entry.type, entry.value);

            if (entry.initialValue != null) {
                valueInitial = new Value();
                valueInitial.InitValue(entry.type, entry.initialValue);
            }
        }

        public void Write(DsgMem dsgMem, Writer writer) {
            valueCurrent.Write(dsgMem.memBuffer + entry.offsetInBuffer, writer);

            if (entry.initialValue != null && dsgMem.memBufferInitial!=null) {
                valueInitial.Write(dsgMem.memBufferInitial + entry.offsetInBuffer, writer);
            }
        }
    }

    public void Write(Writer writer)
    {
        foreach (DsgVarEditableEntry entry in this.editableEntries) {
            entry.Write(dsgMem, writer);
        }
    }

    public void SetDsgMem(DsgMem dsgMem)
    {
        this.dsgMem = dsgMem;
        this.dsgVar = dsgMem.dsgVar;
        this.dsgVarEntries = this.dsgVar.dsgVarInfos;
        this.editableEntries = new DsgVarEditableEntry[this.dsgVarEntries.Length];

        int i = 0;
        foreach (DsgVarInfoEntry entry in this.dsgVarEntries) {
            DsgVarEditableEntry editableEntry = new DsgVarEditableEntry(i, entry);
            editableEntries[i] = editableEntry;
            i++;
        }
    }

    public void SetPerso(Perso perso)
    {
        this.perso = perso;
        if (perso != null && perso.brain != null && perso.brain.mind != null && perso.brain.mind.dsgMem != null && perso.brain.mind.dsgMem.dsgVar != null) {
            SetDsgMem(perso.brain.mind.dsgMem);
        }
    }
}