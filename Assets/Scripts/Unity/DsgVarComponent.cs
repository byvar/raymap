using UnityEngine;
using System.Collections;
using OpenSpace.Object;
using OpenSpace.AI;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Linq;
using OpenSpace.Waypoints;

namespace OpenSpace
{
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

            // Values for different types
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

            public bool valueAsBool_initial;
            public uint valueAsUInt_initial;
            public int valueAsInt_initial;
            public sbyte valueAsSByte_initial;
            public byte valueAsByte_initial;
            public short valueAsShort_initial;
            public ushort valueAsUShort_initial;
            public float valueAsFloat_initial;
            public Vector3 valueAsVector_initial;
            public GameObject valueAsSuperObjectGao_initial;
            public GameObject valueAsPersoGao_initial;
            public GameObject valueAsWaypointGao_initial;
            public string valueAsString_initial;

            public DsgVarEditableEntry(int number, DsgVarInfoEntry entry)
            {
                this.number = number;
                this.entry = entry;
				//print(entry.typeNumber + " - " + entry.type + " - " + entry.offset + " - " + entry.offsetInBuffer + " - " + entry.value);

                switch (entry.type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool       = (bool)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Int:     this.valueAsInt        = (int)     entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UInt:    this.valueAsUInt       = (uint)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Short:   this.valueAsShort      = (short)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UShort:  this.valueAsUShort     = (ushort)  entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Byte:    this.valueAsSByte      = (sbyte)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UByte:   this.valueAsByte       = (byte)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Float:   this.valueAsFloat      = (float)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Vector:  this.valueAsVector     = (Vector3) entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Text:    this.valueAsUInt        = (uint)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Perso:
                        if (entry.value != null && entry.value is Pointer) {
                            Perso perso = MapLoader.Loader.persos.FirstOrDefault(p => (p.SuperObject!=null && p.SuperObject.offset == (Pointer)entry.value)); // find perso that belongs to the superobject
                            if (perso!=null) {
                                this.valueAsPersoGao = perso.Gao;
                            }
                        }
                        break;

                    case DsgVarInfoEntry.DsgVarType.SuperObject:
                        if (entry.value != null) {
                            SuperObject spo = MapLoader.Loader.superObjects.FirstOrDefault(p => (p.offset!=null && p.offset == (Pointer)entry.value));
                            if (spo != null) {
                                this.valueAsSuperObjectGao = spo.Gao;
                            }
                        }
                        break;
                    case DsgVarInfoEntry.DsgVarType.Waypoint:

                        if (entry.value != null) {
                            WayPoint wp = null;
                            if (entry.value is Pointer) {
								wp = WayPoint.FromOffset(entry.value as Pointer);
                                //wp = (WayPoint)entry.value;
                            }
                            
                            if (wp != null) {
                                this.valueAsWaypointGao = wp.Gao;
                            }
                        }
                        break;

                }

                if (entry.initialValue!=null) {
                    switch (entry.type) {
                        case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool_initial       = (bool)entry.initialValue;         break;
                        case DsgVarInfoEntry.DsgVarType.Int:     this.valueAsInt_initial        = (int)entry.initialValue;          break;
                        case DsgVarInfoEntry.DsgVarType.UInt:    this.valueAsUInt_initial       = (uint)entry.initialValue;         break;
                        case DsgVarInfoEntry.DsgVarType.Short:   this.valueAsShort_initial      = (short)entry.initialValue;        break;
                        case DsgVarInfoEntry.DsgVarType.UShort:  this.valueAsUShort_initial     = (ushort)entry.initialValue;       break;
                        case DsgVarInfoEntry.DsgVarType.Byte:    this.valueAsSByte_initial      = (sbyte)entry.initialValue;        break;
                        case DsgVarInfoEntry.DsgVarType.UByte:   this.valueAsByte_initial       = (byte)entry.initialValue;         break;
                        case DsgVarInfoEntry.DsgVarType.Float:   this.valueAsFloat_initial      = (float)entry.initialValue;        break;
                        case DsgVarInfoEntry.DsgVarType.Vector:  this.valueAsVector_initial     = (Vector3)entry.initialValue;      break;
                        case DsgVarInfoEntry.DsgVarType.Text:    this.valueAsUInt_initial        = (uint)entry.initialValue;       break;
                        case DsgVarInfoEntry.DsgVarType.Perso:
                            if (entry.initialValue != null) {
                                Perso perso = MapLoader.Loader.persos.FirstOrDefault(p => p.SuperObject.offset == (Pointer)entry.initialValue);
                                if (perso != null) {
                                    this.valueAsPersoGao_initial = perso.Gao;
                                }
                            }
                        break;
                        case DsgVarInfoEntry.DsgVarType.SuperObject:
                            if (entry.initialValue != null) {
                                SuperObject spo = MapLoader.Loader.superObjects.FirstOrDefault(p => p.offset == (Pointer)entry.initialValue);
                                if (spo != null) {
                                    this.valueAsSuperObjectGao_initial = spo.Gao;
                                }
                            }
                        break;
                        case DsgVarInfoEntry.DsgVarType.Waypoint:

                            if (entry.initialValue != null) {
                                WayPoint wp = null;
                                if (entry.initialValue is Pointer) {
									wp = WayPoint.FromOffset(entry.initialValue as Pointer);
                                    //wp = (WayPoint)entry.initialValue;
                                }

                                if (wp != null) {
                                    this.valueAsWaypointGao_initial = wp.Gao;
                                }
                            }
                            break;
                    }
                }
            }

            public void Write(DsgMem dsgMem, Writer writer)
            {
                Pointer.Goto(ref writer, dsgMem.memBuffer + entry.offsetInBuffer);
                switch (entry.type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean:    writer.Write(this.valueAsBool); break;
                    case DsgVarInfoEntry.DsgVarType.Int:        writer.Write(this.valueAsInt); break;
                    case DsgVarInfoEntry.DsgVarType.UInt:       writer.Write(this.valueAsUInt); break;
                    case DsgVarInfoEntry.DsgVarType.Short:      writer.Write(this.valueAsShort); break;
                    case DsgVarInfoEntry.DsgVarType.UShort:     writer.Write(this.valueAsUShort); break;
                    case DsgVarInfoEntry.DsgVarType.Byte:       writer.Write(this.valueAsSByte); break;
                    case DsgVarInfoEntry.DsgVarType.UByte:      writer.Write(this.valueAsByte); break;
                    case DsgVarInfoEntry.DsgVarType.Float:      writer.Write(this.valueAsFloat); break;
                    case DsgVarInfoEntry.DsgVarType.Vector:     writer.Write(this.valueAsVector.x); writer.Write(this.valueAsVector.y); writer.Write(this.valueAsVector.z); break;
                }

                if (entry.initialValue != null && dsgMem.memBufferInitial!=null) {
                    Pointer.Goto(ref writer, dsgMem.memBufferInitial + entry.offsetInBuffer);

                    switch (entry.type) {
                        case DsgVarInfoEntry.DsgVarType.Boolean:    writer.Write(this.valueAsBool_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Int:        writer.Write(this.valueAsInt_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UInt:       writer.Write(this.valueAsUInt_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Short:      writer.Write(this.valueAsShort_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UShort:     writer.Write(this.valueAsUShort_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Byte:       writer.Write(this.valueAsSByte_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UByte:      writer.Write(this.valueAsByte_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Float:      writer.Write(this.valueAsFloat_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Vector:     writer.Write(this.valueAsVector_initial.x); writer.Write(this.valueAsVector_initial .y); writer.Write(this.valueAsVector_initial.z); break;
                    }
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
}