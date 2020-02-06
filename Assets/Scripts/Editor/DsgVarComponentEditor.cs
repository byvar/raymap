using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using OpenSpace;
using System;
using UnityEditor.IMGUI.Controls;

[CustomEditor(typeof(DsgVarComponent))]
public class DsgVarComponentEditor : Editor {
    DsgVarsTreeView treeviewDsgVars;
    TreeViewState treeviewDsgVarsState;
    MultiColumnHeaderState m_MultiColumnHeaderState;

    public Vector2 scrollPosition = new Vector2(0, 0);

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        DsgVarComponent c = (DsgVarComponent)target;

        if (c.editableEntries != null) {
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 200f);
            InitDsgVarsTreeIfNeeded(rect, c);
            if (treeviewDsgVars.target != c) {
                treeviewDsgVars.target = c;
                treeviewDsgVars.treeModel.SetData(GetData());
                treeviewDsgVars.Reload();
            }
            treeviewDsgVars.OnGUI(rect);
        }
        //GUILayout.BeginVertical();
        /*GUILayout.BeginHorizontal();

        GUILayout.Label("Type/name");
        GUILayout.Label("Current value");
        GUILayout.Label("Initial value");

        GUILayout.EndHorizontal();

        foreach (DsgVarComponent.DsgVarEditableEntry entry in c.editableEntries) {

            GUILayout.BeginHorizontal();

            DrawDsgVarEntry(entry);

            GUILayout.EndHorizontal();
        }*/


        if (GUILayout.Button("Print initial dsgvar assignments")) {

            string printResult = "";

            foreach (DsgVarComponent.DsgVarEditableEntry dsgVarEntry in c.editableEntries) {

                printResult += DsgVarUtil.DsgVarEntryToCSharpAssignment(dsgVarEntry.entry) + Environment.NewLine;
            }

            MapLoader.Loader.print(printResult);
        }

        if (GUILayout.Button("Print dsgvar value offsets")) {

            string printResult = "";

            DsgMem dsgMem = c.dsgMem;
            foreach (DsgVarComponent.DsgVarEditableEntry dsgVarEntry in c.editableEntries) {
                Pointer offsetOfValue = (dsgMem.memBuffer + dsgVarEntry.entry.offsetInBuffer);
                printResult += dsgVarEntry.entry.NiceVariableName + " " + offsetOfValue +Environment.NewLine;
            }

            MapLoader.Loader.print(printResult);
        }

        //GUILayout.EndVertical();
    }



    IList<DsgVarsTreeElement> GetData() {
        DsgVarComponent pb = (DsgVarComponent)target;
        List<DsgVarsTreeElement> tr = new List<DsgVarsTreeElement>();
        tr.Add(new DsgVarsTreeElement("Hidden root", -1, -1));
        if (pb.editableEntries.Length > 0) {
            int id = 0;
            foreach (DsgVarComponent.DsgVarEditableEntry entry in pb.editableEntries) {
                DsgVarsTreeElement el = new DsgVarsTreeElement(entry.Name, 0, id) { entry = entry };
                tr.Add(el);
                id++;
                if (entry.IsArray) {
                    el.children = new List<TreeElement>();
                    int length = entry.ArrayLength;
                    for (int i = 0; i < length; i++) {
                        DsgVarsTreeElement el2 = new DsgVarsTreeElement(entry.Name + "[" + i + "]", 1, id) { entry = entry, arrayIndex = i };
                        tr.Add(el2);
                        id++;
                    }
                }
            }
        }
        return tr;
    }

    void InitDsgVarsTreeIfNeeded(Rect transitionsRect, DsgVarComponent target) {
        if (treeviewDsgVars == null || treeviewDsgVarsState == null || treeviewDsgVars.target != target) {
            treeviewDsgVarsState = new TreeViewState();

            bool firstInit = m_MultiColumnHeaderState == null;
            var headerState = DsgVarsTreeView.CreateDefaultMultiColumnHeaderState(transitionsRect.width);
            if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
            m_MultiColumnHeaderState = headerState;

            var multiColumnHeader = new MultiColumnHeader(headerState);
            if (firstInit)
                multiColumnHeader.ResizeToFit();

            var treeModel = new TreeModel<DsgVarsTreeElement>(GetData());

            treeviewDsgVars = new DsgVarsTreeView(treeviewDsgVarsState, multiColumnHeader, treeModel) {
                target = target
            };
        }
    }

    public static void DrawDsgVarCurrent(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, bool useDsgMem, int? arrayIndex) {
        DsgVarComponent.DsgVarEditableEntry.Value value = dsgVarEntry.valueCurrent;
        if (value != null) {
            DrawDsgVarValue(rect, dsgVarEntry, value, useDsgMem, arrayIndex);
        }
    }
    public static void DrawDsgVarInitial(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, bool useDsgMem, int? arrayIndex) {
        DsgVarComponent.DsgVarEditableEntry.Value value = dsgVarEntry.valueInitial;
        if (value != null) {
            DrawDsgVarValue(rect, dsgVarEntry, value, useDsgMem, arrayIndex);
        }
    }

    public static void DrawDsgVarValue(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, DsgVarComponent.DsgVarEditableEntry.Value value, bool useDsgMem, int? arrayIndex) {
        string stringVal;
        switch (value.type) {
            case DsgVarInfoEntry.DsgVarType.Boolean:
                value.valueAsBool = EditorGUI.Toggle(rect, value.valueAsBool);
                break;
            case DsgVarInfoEntry.DsgVarType.Int:
                value.valueAsInt = EditorGUI.IntField(rect, value.valueAsInt);
                break;
            case DsgVarInfoEntry.DsgVarType.UInt:
                stringVal = EditorGUI.TextField(rect, value.valueAsUInt.ToString());
                UInt32.TryParse(stringVal, out value.valueAsUInt);
                break;
            case DsgVarInfoEntry.DsgVarType.Caps:
                stringVal = EditorGUI.TextField(rect, value.valueAsUInt.ToString());
                UInt32.TryParse(stringVal, out value.valueAsUInt);
                break;
            case DsgVarInfoEntry.DsgVarType.Short:
                stringVal = EditorGUI.TextField(rect, value.valueAsShort.ToString());
                Int16.TryParse(stringVal, out value.valueAsShort);
                break;
            case DsgVarInfoEntry.DsgVarType.UShort:
                stringVal = EditorGUI.TextField(rect, value.valueAsUShort.ToString());
                UInt16.TryParse(stringVal, out value.valueAsUShort);
                break;
            case DsgVarInfoEntry.DsgVarType.Byte:
                stringVal = EditorGUI.TextField(rect, value.valueAsSByte.ToString());
                SByte.TryParse(stringVal, out value.valueAsSByte);
                break;
            case DsgVarInfoEntry.DsgVarType.UByte:
                stringVal = EditorGUI.TextField(rect, value.valueAsByte.ToString());
                Byte.TryParse(stringVal, out value.valueAsByte);
                break;
            case DsgVarInfoEntry.DsgVarType.Float:
                value.valueAsFloat = EditorGUI.FloatField(rect, value.valueAsFloat);
                break;
            case DsgVarInfoEntry.DsgVarType.Text:
                stringVal = EditorGUI.TextField(rect, value.valueAsUInt.ToString());
                UInt32.TryParse(stringVal, out value.valueAsUInt);
                //GUILayout.Label(MapLoader.Loader.localization.GetTextForHandleAndLanguageID((int)value.valueAsUInt, 0));
                break;
            case DsgVarInfoEntry.DsgVarType.Vector:
                value.valueAsVector = EditorGUI.Vector3Field(rect, "", value.valueAsVector);
                break;
            case DsgVarInfoEntry.DsgVarType.Perso:
                PersoBehaviour currentPersoBehaviour = value.valueAsPersoGao != null ? value.valueAsPersoGao.GetComponent<PersoBehaviour>() : null;
                PersoBehaviour selectedPersoBehaviour = ((PersoBehaviour)EditorGUI.ObjectField(rect, currentPersoBehaviour, typeof(PersoBehaviour), true));

                if (selectedPersoBehaviour != null && selectedPersoBehaviour.gameObject != null) {
                    value.valueAsPersoGao = selectedPersoBehaviour.gameObject;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.SuperObject:
                SuperObjectComponent currentGao = value.valueAsSuperObjectGao != null ? value.valueAsSuperObjectGao.GetComponent<SuperObjectComponent>() : null;
                SuperObjectComponent selectedGao = ((SuperObjectComponent)EditorGUI.ObjectField(rect, currentGao, typeof(SuperObjectComponent), true));

                if (selectedGao != null && selectedGao.gameObject != null) {
                    value.valueAsSuperObjectGao = selectedGao.gameObject;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.WayPoint:
                WaypointBehaviour currentWaypointGao = value.valueAsWaypointGao != null ? value.valueAsWaypointGao.GetComponent<WaypointBehaviour>() : null;
                WaypointBehaviour selectedWaypointGao = ((WaypointBehaviour)EditorGUI.ObjectField(rect, currentWaypointGao, typeof(WaypointBehaviour), true));

                if (selectedWaypointGao != null && selectedWaypointGao.gameObject != null) {
                    value.valueAsWaypointGao = selectedWaypointGao.gameObject;
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
                if (dsgVarEntry.entry.value.GetType().IsArray) {
                    if (arrayIndex.HasValue) {
                        if (value.valueAsArray != null) {
                            DrawDsgVarValue(rect, dsgVarEntry, value.valueAsArray[arrayIndex.Value], useDsgMem, arrayIndex);
                        }
                    } else {
                        object[] array = (object[])dsgVarEntry.entry.value;
                        EditorGUI.LabelField(rect, "Length: " + array.Length);
                    }
                }
                break;
        }
    }

    }