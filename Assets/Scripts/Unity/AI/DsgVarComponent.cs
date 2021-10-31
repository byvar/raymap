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
    // Normal
    public Perso perso;
    public DsgMem dsgMem;
    public DsgVar dsgVar;
    public DsgVarInfoEntry[] dsgVarEntries;

    // ROM
    public OpenSpace.ROM.Perso persoROM;
    public OpenSpace.ROM.DsgMem dsgMemROM;
    public OpenSpace.ROM.DsgVar dsgVarROM;

    public DsgVarEditableEntry[] editableEntries;

    public class DsgVarEditableEntry {
        public DsgVarInfoEntry entry;
        public OpenSpace.ROM.DsgVarInfo.Entry entryROM;
        public List<OpenSpace.ROM.DsgMemInfo> entriesMemROM;


        public int number;
        public string Name {
            get {
                if (entry != null) return entry.NiceVariableName;
                if (entryROM != null) return entryROM.NiceVariableName;
                return "null";
            }
        }
        public bool IsArray {
            get {
                return DsgVarInfoEntry.GetDsgVarTypeFromArrayType(Type) != DsgVarType.None;
            }
        }
        public DsgVarType Type {
            get {
                if(entry != null) return entry.type;
                if (entryROM != null) return entryROM.value.dsgVarType;
                return DsgVarType.None;
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
            public OpenSpace.ROM.DsgVarValue valROM;
            public DsgVarType type;

            public SuperObjectComponent AsSuperObject {
                get {
                    if (type != DsgVarType.SuperObject) return null;
                    if (val != null) {
                        if (val.valueSuperObject == null && val.valuePointer != null) {
                            val.valueSuperObject = MapLoader.Loader.superObjects.FirstOrDefault(p => (p.offset != null && p.offset == val.valuePointer));
                        }
                        return val.valueSuperObject?.Gao.GetComponent<SuperObjectComponent>();
                    }
                    return null;
                }
                set {
                    if (type != DsgVarType.SuperObject) return;
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
                    if (type != DsgVarType.Perso) return null;
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
                    if (type != DsgVarType.Perso) return;
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
            public OpenSpace.Object.Properties.State AsAction {
                get {
                    if (type != DsgVarType.Action) return null;
                    if (val != null) {
                        if (val.valueAction == null && val.valuePointer != null) {
                            OpenSpace.Object.Properties.State a = OpenSpace.Object.Properties.State.FromOffset(val.valuePointer);
                            if (a != null) {
                                val.valueAction = a;
                            }
                        }
                        return val.valueAction;
                    }
                    return null;
                }
                /*set {}*/
            }

            public ROMPersoBehaviour AsPersoROM {
                get {
                    if (type != DsgVarType.Perso) return null;
                    if (valROM != null) {
                        OpenSpace.ROM.Perso p = valROM.ValuePerso;
                        if (p == null) return null;
                        List<ROMPersoBehaviour> romPersos = MapLoader.Loader.controller.romPersos;
                        return romPersos.FirstOrDefault(r => r.perso == p);
                    }
                    return null;
                }
                set {
                    if (type != DsgVarType.Perso) return;
                    if (val != null) {
                        /*if (value != null) {
                            val.valuePerso = value.perso;
                            val.valuePointer = value.perso.offset;
                        } else {
                            val.valuePerso = null;
                            val.valuePointer = null;
                        }*/
                    }
                }
            }
            public WayPointBehaviour AsWayPoint {
                get {
                    if (type != DsgVarType.WayPoint) return null;
                    if (val != null) {
                        return val.valueWayPoint?.Gao.GetComponent<WayPointBehaviour>();
                    }
                    if (valROM != null) {
                        OpenSpace.ROM.WayPoint wp = valROM.ValueWayPoint;
                        if (wp == null) return null;
                        return MapLoader.Loader.controller.graphManager.waypoints.FirstOrDefault(w => w.wpROM == wp);
                    }
                    return null;
                }
                set {
                    if (type != DsgVarType.WayPoint) return;
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
                    if (type != DsgVarType.Graph) return null;
                    if (val != null) {
                        Graph graph = val.valueGraph;
                        if (graph == null) return null;
                        return MapLoader.Loader.controller.graphManager.graphDict[graph];
                    }
                    if (valROM != null) {
                        OpenSpace.ROM.Graph graph = valROM.ValueGraph;
                        if (graph == null) return null;
                        return MapLoader.Loader.controller.graphManager.graphROMDict[graph];
                    }
                    return null;
                }
                set {
                    if (type != DsgVarType.Graph) return;
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
                    if (valROM != null) return valROM.ValueBoolean;
                    return false;
                }
                set {
                    if (val != null) val.valueBool = value;
                }
            }
            public sbyte AsByte {
                get {
                    if (val != null) return val.valueByte;
                    if (valROM != null) return valROM.ValueByte;
                    return 0;
                }
                set {
                    if (val != null) val.valueByte = value;
                }
            }
            public byte AsUByte {
                get {
                    if (val != null) return val.valueUByte;
                    if (valROM != null) return valROM.ValueUByte;
                    return 0;
                }
                set {
                    if (val != null) val.valueUByte = value;
                }
            }
            public short AsShort {
                get {
                    if (val != null) return val.valueShort;
                    if (valROM != null) return valROM.ValueShort;
                    return 0;
                }
                set {
                    if (val != null) val.valueShort = value;
                }
            }
            public ushort AsUShort {
                get {
                    if (val != null) return val.valueUShort;
                    if (valROM != null) return valROM.ValueUShort;
                    return 0;
                }
                set {
                    if (val != null) val.valueUShort = value;
                }
            }
            public int AsInt {
                get {
                    if (val != null) return val.valueInt;
                    if (valROM != null) return valROM.ValueInt;
                    return 0;
                }
                set {
                    if (val != null) val.valueInt = value;
                }
            }
            public uint AsUInt {
                get {
                    if (val != null) return val.valueUInt;
                    if (valROM != null) return valROM.ValueUInt;
                    return 0;
                }
                set {
                    if (val != null) val.valueUInt = value;
                }
            }
            public float AsFloat {
                get {
                    if (val != null) return val.valueFloat;
                    if (valROM != null) return valROM.ValueFloat;
                    return 0;
                }
                set {
                    if (val != null) val.valueFloat = value;
                }
            }
            public Vector3 AsVector {
                get {
                    if (val != null) return val.valueVector;
                    if (valROM != null) return valROM.ValueVector;
                    return Vector3.zero;
                }
                set {
                    if (val != null) val.valueVector = value;
                }
            }
            public int AsText {
                get {
                    if (val != null) return val.valueText;
                    if (valROM != null) return valROM.ValueText;
                    return 0;
                }
                set {
                    if (val != null) val.valueText = value;
                }
            }
            public uint AsCaps {
                get {
                    if (val != null) return val.valueCaps;
                    if (valROM != null) return valROM.ValueCaps;
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
            public Value(OpenSpace.ROM.DsgVarValue value) {
                this.valROM = value;
                if (value != null) {
                    InitValue(value);
                }
            }
            // Create an empty array
            public Value(DsgVarType type, int length) {
                this.type = type;
                AsArray = new Value[length];
                for (int i = 0; i < AsArray.Length; i++) {
                    AsArray[i] = null;
                }
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
            public void InitValue(OpenSpace.ROM.DsgVarValue value) {
                this.type = value.dsgVarType;

                if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(type) != DsgVarType.None) {
                    AsArray = new Value[value.ValueArrayLength];
                    for (int i = 0; i < AsArray.Length; i++) {
                        AsArray[i] = null;
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
                if (valROM != null) return valROM.IsSameValue(value.valROM);
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

        public DsgVarEditableEntry(OpenSpace.ROM.DsgVarInfo.Entry modelEntry, List<OpenSpace.ROM.DsgMemInfo> currentEntries) {
            this.entryROM = modelEntry;
            this.entriesMemROM = currentEntries;
            if(modelEntry != null) valueModel = new Value(modelEntry.value);
            if (currentEntries.Count > 0) {
                if (DsgVarInfoEntry.GetDsgVarTypeFromArrayType(modelEntry.value.dsgVarType) != DsgVarType.None) {
                    // Check for array
                    valueInitial = new Value(modelEntry.value.dsgVarType, modelEntry.value.ValueArrayLength);
                    for (int i = 0; i < currentEntries.Count; i++) {
                        if (currentEntries[i].value.paramEntry == null) continue;  // Skip this, it just redefines the array anyway
                        ushort arrayIndex = currentEntries[i].value.paramEntry.Value.index_in_array;
                        valueInitial.AsArray[arrayIndex] = new Value(currentEntries[i].value);
                    }
                } else {
                    // Not an array
                    valueInitial = new Value(currentEntries[0].value);
                }
                //List<OpenSpace.ROM.DsgMemInfo> entriesNoArray = currentEntries.Where(e => e.value.paramEntry == null || e.value.paramEntry.Value == null).ToList();
            }

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

    public void SetPerso(OpenSpace.ROM.Perso perso) {
        this.persoROM = perso;
        if (perso != null && perso.brain?.Value != null) {
            dsgMemROM = perso.brain.Value.dsgMem.Value;
            dsgVarROM = perso.brain.Value.aiModel?.Value.dsgVar?.Value;
            if (dsgVarROM?.info?.Value == null) return;
            editableEntries = new DsgVarEditableEntry[dsgVarROM.info.Value.entries.Length];

            for (int i = 0; i < editableEntries.Length; i++) {
                List<OpenSpace.ROM.DsgMemInfo> memInfos = new List<OpenSpace.ROM.DsgMemInfo>();
                if (dsgMemROM?.info?.Value != null && dsgMemROM.info.Value.info.Length > 0) {
                    for (int j = 0; j < dsgMemROM.info.Value.info.Length; j++) {
                        OpenSpace.ROM.DsgMemInfo info = dsgMemROM.info.Value.info[j].Value;
                        OpenSpace.ROM.DsgVarInfo.Entry entry = dsgVarROM.info.Value.GetEntryFromIndex(info.value.index);
                        if (entry == dsgVarROM.info.Value.entries[i]) {
                            memInfos.Add(info);
                        }
                    }
                }
                DsgVarEditableEntry editableEntry = new DsgVarEditableEntry(dsgVarROM.info.Value.entries[i], memInfos);
                editableEntries[i] = editableEntry;
            }
        }
    }
}