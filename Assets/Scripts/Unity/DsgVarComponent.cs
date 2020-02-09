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

public class DsgVarComponent : MonoBehaviour {
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
                    if (valueModel != null) return valueModel.AsArray.Length;
                    if (valueCurrent != null) return valueCurrent.AsArray.Length;
                    if (valueInitial != null) return valueInitial.AsArray.Length;
                }
                return 0;
            }
        }

        // Unity interface for the different DSGVar Types
        public class Value {
            public DsgVarValue val;
            public DsgVarInfoEntry.DsgVarType type;

            public SuperObjectComponent AsSuperObject {
                get {
                    if (type != DsgVarInfoEntry.DsgVarType.SuperObject) return null;
                    if (val != null) {
                        if (val.valueSuperObject == null && val.valuePointer != null) {
                            val.valueSuperObject = MapLoader.Loader.superObjects.FirstOrDefault(p => (p.offset != null && p.offset == val.valuePointer));
                        }
                        return val.valueSuperObject?.Gao.GetComponent<SuperObjectComponent>();
                    }
                    return null;
                }
                set {
                    if (type != DsgVarInfoEntry.DsgVarType.SuperObject) return;
                    if (val != null) {
                        if (value != null) {
                            val.valueSuperObject = value.so;
                            val.valuePointer = value.so.offset;
                        } else {
                            val.valueSuperObject = null;
                            val.valuePointer = null;
                        }
                    }
                }
            }
            public PersoBehaviour AsPerso {
                get {
                    if (type != DsgVarInfoEntry.DsgVarType.Perso) return null;
                    if (val != null) {
                        if (val.valuePerso == null && val.valuePointer != null) {
                            SuperObject so = SuperObject.FromOffset(val.valuePointer);
                            if (so != null) {
                                val.valuePerso = so.data as Perso;
                            }
                        }
                        return val.valuePerso?.Gao.GetComponent<PersoBehaviour>();
                    }
                    return null;
                }
                set {
                    if (type != DsgVarInfoEntry.DsgVarType.Perso) return;
                    if (val != null) {
                        if (value != null) {
                            val.valuePerso = value.perso;
                            val.valuePointer = value.perso.offset;
                        } else {
                            val.valuePerso = null;
                            val.valuePointer = null;
                        }
                    }
                }
            }
            public WayPointBehaviour AsWayPoint {
                get {
                    if (type != DsgVarInfoEntry.DsgVarType.WayPoint) return null;
                    if (val != null) {
                        return val.valueWayPoint?.Gao.GetComponent<WayPointBehaviour>();
                    }
                    return null;
                }
                set {
                    if (type != DsgVarInfoEntry.DsgVarType.WayPoint) return;
                    if (val != null) {
                        if (value != null) {
                            val.valueWayPoint = value.wp;
                            val.valuePointer = value.wp.offset;
                        } else {
                            val.valueWayPoint = null;
                            val.valuePointer = null;
                        }
                    }
                }
            }
            public GraphBehaviour AsGraph {
                get {
                    if (type != DsgVarInfoEntry.DsgVarType.Graph) return null;
                    if (val != null) {
                        Graph graph = val.valueGraph;
                        if (graph == null) return null;
                        return MapLoader.Loader.controller.graphManager.graphDict[graph];
                    }
                    return null;
                }
                set {
                    if (type != DsgVarInfoEntry.DsgVarType.Graph) return;
                    if (val != null) {
                        if (value != null) {
                            val.valueGraph = value.graph;
                            val.valuePointer = value.graph.offset;
                        } else {
                            val.valueGraph = null;
                            val.valuePointer = null;
                        }
                    }
                }
            }
            public bool AsBoolean {
                get {
                    if (val != null) return val.valueBool;
                    return false;
                }
                set {
                    if (val != null) val.valueBool = value;
                }
            }
            public sbyte AsByte {
                get {
                    if (val != null) return val.valueByte;
                    return 0;
                }
                set {
                    if (val != null) val.valueByte = value;
                }
            }
            public byte AsUByte {
                get {
                    if (val != null) return val.valueUByte;
                    return 0;
                }
                set {
                    if (val != null) val.valueUByte = value;
                }
            }
            public short AsShort {
                get {
                    if (val != null) return val.valueShort;
                    return 0;
                }
                set {
                    if (val != null) val.valueShort = value;
                }
            }
            public ushort AsUShort {
                get {
                    if (val != null) return val.valueUShort;
                    return 0;
                }
                set {
                    if (val != null) val.valueUShort = value;
                }
            }
            public int AsInt {
                get {
                    if (val != null) return val.valueInt;
                    return 0;
                }
                set {
                    if (val != null) val.valueInt = value;
                }
            }
            public uint AsUInt {
                get {
                    if (val != null) return val.valueUInt;
                    return 0;
                }
                set {
                    if (val != null) val.valueUInt = value;
                }
            }
            public float AsFloat {
                get {
                    if (val != null) return val.valueFloat;
                    return 0;
                }
                set {
                    if (val != null) val.valueFloat = value;
                }
            }
            public Vector3 AsVector {
                get {
                    if (val != null) return val.valueVector;
                    return Vector3.zero;
                }
                set {
                    if (val != null) val.valueVector = value;
                }
            }
            public int AsText {
                get {
                    if (val != null) return val.valueText;
                    return 0;
                }
                set {
                    if (val != null) val.valueText = value;
                }
            }
            public uint AsCaps {
                get {
                    if (val != null) return val.valueCaps;
                    return 0;
                }
                set {
                    if (val != null) val.valueCaps = value;
                }
            }

            public Value[] AsArray;

            public Value(DsgVarValue value) {
                this.val = value;
                if (value != null) InitValue(value);
            }

            public void InitValue(DsgVarValue value) {
                this.type = value.type;

                if (value.valueArray != null) {
                    AsArray = new Value[value.arrayLength];
                    for (int i = 0; i < AsArray.Length; i++) {
                        AsArray[i] = new Value(value.valueArray[i]);
                    }
                }
            }

            public void Write(Writer writer) {
                if (val != null) val.Write(writer);
            }

            public override string ToString() {
                if (val != null) return val.ToString();
                return base.ToString();
            }
            public bool IsSameValue(Value value) {
                if (value == null) return false;
                if (val != null) return val.IsSameValue(value.val);
                return true;
            }
        }
        public Value valueCurrent;
        public Value valueInitial;
        public Value valueModel;

        public DsgVarEditableEntry(DsgVarInfoEntry entry, DsgVarValue current, DsgVarValue initial, DsgVarValue model) {
            this.entry = entry;
            if (current != null) valueCurrent = new Value(current);
            if (initial != null) valueInitial = new Value(initial);
            if (model != null) valueModel = new Value(model);
        }

        public void Write(Writer writer) {
            if (valueCurrent != null) valueCurrent.Write(writer);
            if (valueInitial != null) valueInitial.Write(writer);
            if (valueModel != null) valueModel.Write(writer);
        }

        public bool IsCurrentDifferentFromInitial {
            get {
                if (valueCurrent == null) return false;
                if (valueInitial == null) return false;
                return !(valueCurrent.IsSameValue(valueInitial));
            }
        }
        public bool IsDifferentFromModel {
            get {
                if (valueModel == null) return false;
                if (valueCurrent == null) {
                    if (valueInitial == null) return false;
                    return (!valueInitial.IsSameValue(valueModel));
                }
                return !(valueCurrent.IsSameValue(valueModel));
            }
        }
    }

    public void Write(Writer writer)
    {
        foreach (DsgVarEditableEntry entry in this.editableEntries) {
            entry.Write(writer);
        }
    }

    public void SetPerso(Perso perso)
    {
        this.perso = perso;
        if (perso != null && perso.brain != null && perso.brain.mind != null) {
            dsgMem = perso.brain.mind.dsgMem;
            if (dsgMem != null) {
                dsgVar = dsgMem.dsgVar;
            } else {
                dsgVar = perso.brain.mind.AI_model?.dsgVar;
            }
            dsgVarEntries = dsgVar.dsgVarInfos;
            editableEntries = new DsgVarEditableEntry[dsgVarEntries.Length];

            for(int i = 0; i < editableEntries.Length; i++) {
                DsgVarEditableEntry editableEntry = new DsgVarEditableEntry(
                    dsgVar.dsgVarInfos[i],
                    dsgMem?.values?[i],
                    dsgMem?.valuesInitial?[i],
                    dsgVar?.defaultValues?[i]);
                editableEntries[i] = editableEntry;
            }
        }
    }
}