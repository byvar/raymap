using UnityEngine;
using System.Collections;
using OpenSpace.EngineObject;
using OpenSpace.AI;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace OpenSpace
{
    public class DsgVarComponent : MonoBehaviour
    {
        public Perso perso;
        public DsgMem dsgMem;
        public DsgVar dsgVar;
        public DsgVarInfoEntry[] dsgVarEntries;
        public DsgVarEditableEntry[] editableEntries;

        public class DsgVarEditableEntry
        {
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
            public GameObject valueAsPersoGao;

            public bool valueAsBool_initial;
            public uint valueAsUInt_initial;
            public int valueAsInt_initial;
            public sbyte valueAsSByte_initial;
            public byte valueAsByte_initial;
            public short valueAsShort_initial;
            public ushort valueAsUShort_initial;
            public float valueAsFloat_initial;
            public Vector3 valueAsVector_initial;
            public GameObject valueAsPersoGao_initial;

            public DsgVarEditableEntry(int number, DsgVarInfoEntry entry)
            {
                this.number = number;
                this.entry = entry;

                switch (entry.type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool   = (bool)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Int:     this.valueAsInt    = (int)     entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UInt:    this.valueAsUInt   = (uint)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Short:   this.valueAsShort  = (short)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UShort:  this.valueAsUShort = (ushort)  entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Byte:    this.valueAsSByte  = (sbyte)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.UByte:   this.valueAsByte   = (byte)    entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Float:   this.valueAsFloat  = (float)   entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Vector:  this.valueAsVector = (Vector3) entry.value;  break;
                    case DsgVarInfoEntry.DsgVarType.Perso:   this.valueAsPersoGao  = (GameObject)entry.value; break;
                }

                if (entry.initialValue!=null) {
                    switch (entry.type) {
                        case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool_initial = (bool)entry.initialValue;      break;
                        case DsgVarInfoEntry.DsgVarType.Int:     this.valueAsInt_initial = (int)entry.initialValue;        break;
                        case DsgVarInfoEntry.DsgVarType.UInt:    this.valueAsUInt_initial = (uint)entry.initialValue;      break;
                        case DsgVarInfoEntry.DsgVarType.Short:   this.valueAsShort_initial = (short)entry.initialValue;    break;
                        case DsgVarInfoEntry.DsgVarType.UShort:  this.valueAsUShort_initial = (ushort)entry.initialValue;  break;
                        case DsgVarInfoEntry.DsgVarType.Byte:    this.valueAsSByte_initial = (sbyte)entry.initialValue;    break;
                        case DsgVarInfoEntry.DsgVarType.UByte:   this.valueAsByte_initial = (byte)entry.initialValue;      break;
                        case DsgVarInfoEntry.DsgVarType.Float:   this.valueAsFloat_initial = (float)entry.initialValue;    break;
                        case DsgVarInfoEntry.DsgVarType.Vector:  this.valueAsVector_initial = (Vector3)entry.initialValue; break;
                        case DsgVarInfoEntry.DsgVarType.Perso:   this.valueAsPersoGao_initial = (GameObject)entry.value; break;
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
                }
            }

            public void DrawInspector()
            {
                GUILayout.Label(entry.type + " dsgVar_" + number);
                string stringVal = "";
                switch (entry.type) {
                    case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool = EditorGUILayout.Toggle(this.valueAsBool); break;
                    case DsgVarInfoEntry.DsgVarType.Int:    stringVal = GUILayout.TextField(this.valueAsInt.ToString()); Int32.TryParse(stringVal, out this.valueAsInt); break;
                    case DsgVarInfoEntry.DsgVarType.UInt:   stringVal = GUILayout.TextField(this.valueAsUInt.ToString()); UInt32.TryParse(stringVal, out this.valueAsUInt); break;
                    case DsgVarInfoEntry.DsgVarType.Short:  stringVal = GUILayout.TextField(this.valueAsShort.ToString()); Int16.TryParse(stringVal, out this.valueAsShort); break;
                    case DsgVarInfoEntry.DsgVarType.UShort: stringVal = GUILayout.TextField(this.valueAsUShort.ToString()); UInt16.TryParse(stringVal, out this.valueAsUShort); break;
                    case DsgVarInfoEntry.DsgVarType.Byte:   stringVal = GUILayout.TextField(this.valueAsSByte.ToString()); SByte.TryParse(stringVal, out this.valueAsSByte); break;
                    case DsgVarInfoEntry.DsgVarType.UByte:  stringVal = GUILayout.TextField(this.valueAsByte.ToString()); Byte.TryParse(stringVal, out this.valueAsByte); break;
                    case DsgVarInfoEntry.DsgVarType.Float:  stringVal = GUILayout.TextField(this.valueAsFloat.ToString()); Single.TryParse(stringVal, out this.valueAsFloat); break;
                    case DsgVarInfoEntry.DsgVarType.Vector:
                        float val_x = this.valueAsVector.x;
                        float val_y = this.valueAsVector.y;
                        float val_z = this.valueAsVector.z;
                        stringVal = GUILayout.TextField(this.valueAsVector.x.ToString()); Single.TryParse(stringVal, out val_x);
                        stringVal = GUILayout.TextField(this.valueAsVector.y.ToString()); Single.TryParse(stringVal, out val_y);
                        stringVal = GUILayout.TextField(this.valueAsVector.z.ToString()); Single.TryParse(stringVal, out val_z);
                        this.valueAsVector = new Vector3(val_x, val_y, val_z);
                        break;

                }

                if (entry.initialValue != null) {
                    GUILayout.Space(20);

                    switch (entry.type) {
                        case DsgVarInfoEntry.DsgVarType.Boolean: this.valueAsBool_initial = EditorGUILayout.Toggle(this.valueAsBool_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Int: stringVal = GUILayout.TextField(this.valueAsInt_initial.ToString()); Int32.TryParse(stringVal, out this.valueAsInt_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UInt: stringVal = GUILayout.TextField(this.valueAsUInt_initial.ToString()); UInt32.TryParse(stringVal, out this.valueAsUInt_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Short: stringVal = GUILayout.TextField(this.valueAsShort_initial.ToString()); Int16.TryParse(stringVal, out this.valueAsShort_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UShort: stringVal = GUILayout.TextField(this.valueAsUShort_initial.ToString()); UInt16.TryParse(stringVal, out this.valueAsUShort_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Byte: stringVal = GUILayout.TextField(this.valueAsSByte_initial.ToString()); SByte.TryParse(stringVal, out this.valueAsSByte_initial); break;
                        case DsgVarInfoEntry.DsgVarType.UByte: stringVal = GUILayout.TextField(this.valueAsByte_initial.ToString()); Byte.TryParse(stringVal, out this.valueAsByte_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Float: stringVal = GUILayout.TextField(this.valueAsFloat_initial.ToString()); Single.TryParse(stringVal, out this.valueAsFloat_initial); break;
                        case DsgVarInfoEntry.DsgVarType.Vector:
                            float val_x = this.valueAsVector_initial.x;
                            float val_y = this.valueAsVector_initial.y;
                            float val_z = this.valueAsVector_initial.z;
                            stringVal = GUILayout.TextField(this.valueAsVector_initial.x.ToString()); Single.TryParse(stringVal, out val_x);
                            stringVal = GUILayout.TextField(this.valueAsVector_initial.y.ToString()); Single.TryParse(stringVal, out val_y);
                            stringVal = GUILayout.TextField(this.valueAsVector_initial.z.ToString()); Single.TryParse(stringVal, out val_z);
                            this.valueAsVector_initial = new Vector3(val_x, val_y, val_z);
                            break;

                    }
                } else {
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public void Write(Writer writer)
        {
            foreach (DsgVarEditableEntry entry in this.editableEntries) {
                entry.Write(dsgMem, writer);
            }
        }

        public void SetPerso(Perso perso)
        {
            this.perso = perso;
            if (perso != null && perso.brain != null && perso.brain.mind != null && perso.brain.mind.dsgMem != null && perso.brain.mind.dsgMem.dsgVar != null) {
                this.dsgMem = perso.brain.mind.dsgMem;
                this.dsgVar = perso.brain.mind.dsgMem.dsgVar;
                this.dsgVarEntries = this.dsgVar.dsgVarInfos;
                this.editableEntries = new DsgVarEditableEntry[this.dsgVarEntries.Length];

                int i = 0;
                foreach(DsgVarInfoEntry entry in this.dsgVarEntries) {
                    DsgVarEditableEntry editableEntry = new DsgVarEditableEntry(i, entry);
                    editableEntries[i] = editableEntry;
                    i++;
                }
            }
        }
    }
}