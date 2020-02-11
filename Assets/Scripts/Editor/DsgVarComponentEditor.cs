using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using OpenSpace;
using System;
using UnityEditor.IMGUI.Controls;
using System.Linq;

[CustomEditor(typeof(DsgVarComponent))]
public class DsgVarComponentEditor : Editor {
    private DsgVarsTreeView treeviewDsgVars;
    private TreeViewState treeviewDsgVarsState;
    private MultiColumnHeaderState m_MultiColumnHeaderState;
    private static LocalizationDropdown localizationDropdown = null;

    public Vector2 scrollPosition = new Vector2(0, 0);

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        DsgVarComponent c = (DsgVarComponent)target;

        if (c.editableEntries != null) {
            Rect rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 400f);
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


        /*if (GUILayout.Button("Print initial dsgvar assignments")) {

            string printResult = "";

            foreach (DsgVarComponent.DsgVarEditableEntry dsgVarEntry in c.editableEntries) {

                printResult += DsgVarUtil.DsgVarEntryToCSharpAssignment(dsgVarEntry.entry) + Environment.NewLine;
            }

            MapLoader.Loader.print(printResult);
        }*/

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
        bool visibleCurrent = false;
        bool visibleInitial = false;
        bool visibleModel = false;
        if (pb.editableEntries.Length > 0) {
            int id = 0;
            foreach (DsgVarComponent.DsgVarEditableEntry entry in pb.editableEntries) {
                DsgVarsTreeElement el = new DsgVarsTreeElement(entry.Name, 0, id) { entry = entry };
                if (entry.valueCurrent != null) visibleCurrent = true;
                if (entry.valueInitial != null) visibleInitial = true;
                if (entry.valueModel != null) visibleModel = true;
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
            // Activate columns
            List<int> visibleColumns = m_MultiColumnHeaderState.visibleColumns.ToList();
            if (visibleCurrent != visibleColumns.Contains((int)DsgVarsTreeView.Columns.CurrentValue)) {
                if (visibleCurrent) {
                    visibleColumns.Add((int)DsgVarsTreeView.Columns.CurrentValue);
                } else {
                    visibleColumns.Remove((int)DsgVarsTreeView.Columns.CurrentValue);
                }
            }
            if (visibleInitial != visibleColumns.Contains((int)DsgVarsTreeView.Columns.InitialValue)) {
                if (visibleInitial) {
                    visibleColumns.Add((int)DsgVarsTreeView.Columns.InitialValue);
                } else {
                    visibleColumns.Remove((int)DsgVarsTreeView.Columns.InitialValue);
                }
            }
            if (visibleModel != visibleColumns.Contains((int)DsgVarsTreeView.Columns.ModelValue)) {
                if (visibleModel) {
                    visibleColumns.Add((int)DsgVarsTreeView.Columns.ModelValue);
                } else {
                    visibleColumns.Remove((int)DsgVarsTreeView.Columns.ModelValue);
                }
            }
            m_MultiColumnHeaderState.visibleColumns = visibleColumns.ToArray();
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

    public static void DrawDsgVarCurrent(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, int? arrayIndex) {
        DsgVarComponent.DsgVarEditableEntry.Value value = dsgVarEntry.valueCurrent;
        if (value != null) {
            DrawDsgVarValue(rect, dsgVarEntry, value, arrayIndex);
        }
    }
    public static void DrawDsgVarInitial(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, int? arrayIndex) {
        DsgVarComponent.DsgVarEditableEntry.Value value = dsgVarEntry.valueInitial;
        if (value != null) {
            DrawDsgVarValue(rect, dsgVarEntry, value, arrayIndex);
        }
    }
    public static void DrawDsgVarModel(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, int? arrayIndex) {
        DsgVarComponent.DsgVarEditableEntry.Value value = dsgVarEntry.valueModel;
        if (value != null) {
            DrawDsgVarValue(rect, dsgVarEntry, value, arrayIndex);
        }
    }

    public static void DrawDsgVarValue(Rect rect, DsgVarComponent.DsgVarEditableEntry dsgVarEntry, DsgVarComponent.DsgVarEditableEntry.Value value, int? arrayIndex) {
        string stringVal;
        switch (value.type) {
            case DsgVarInfoEntry.DsgVarType.Boolean:
                value.AsBoolean = EditorGUI.Toggle(rect, value.AsBoolean);
                break;
            case DsgVarInfoEntry.DsgVarType.Int:
                value.AsInt = EditorGUI.IntField(rect, value.AsInt);
                break;
            case DsgVarInfoEntry.DsgVarType.UInt:
                stringVal = EditorGUI.TextField(rect, value.AsUInt.ToString());
                if(UInt32.TryParse(stringVal, out uint r_uint)) value.AsUInt = r_uint;
                break;
            case DsgVarInfoEntry.DsgVarType.Caps:
                stringVal = EditorGUI.TextField(rect, value.AsCaps.ToString());
                if (UInt32.TryParse(stringVal, out uint r_caps)) value.AsCaps = r_caps;
                break;
            case DsgVarInfoEntry.DsgVarType.Short:
                stringVal = EditorGUI.TextField(rect, value.AsShort.ToString());
                if(Int16.TryParse(stringVal, out short r_short)) value.AsShort = r_short;
                break;
            case DsgVarInfoEntry.DsgVarType.UShort:
                stringVal = EditorGUI.TextField(rect, value.AsUShort.ToString());
                if(UInt16.TryParse(stringVal, out ushort r_ushort)) value.AsUShort = r_ushort;
                break;
            case DsgVarInfoEntry.DsgVarType.Byte:
                stringVal = EditorGUI.TextField(rect, value.AsByte.ToString());
                if (SByte.TryParse(stringVal, out sbyte r_sbyte)) value.AsByte = r_sbyte;
                break;
            case DsgVarInfoEntry.DsgVarType.UByte:
                stringVal = EditorGUI.TextField(rect, value.AsUByte.ToString());
                if (Byte.TryParse(stringVal, out byte r_byte)) value.AsUByte = r_byte;
                break;
            case DsgVarInfoEntry.DsgVarType.Float:
                value.AsFloat = EditorGUI.FloatField(rect, value.AsFloat);
                break;
            case DsgVarInfoEntry.DsgVarType.Text:
                int? newTextID = DrawText(rect, value.AsText);
                if (newTextID.HasValue) value.AsText = newTextID.Value;
                //GUILayout.Label(MapLoader.Loader.localization.GetTextForHandleAndLanguageID((int)value.AsUInt, 0));
                break;
            case DsgVarInfoEntry.DsgVarType.Vector:
                value.AsVector = EditorGUI.Vector3Field(rect, "", value.AsVector);
                break;
            case DsgVarInfoEntry.DsgVarType.Perso:
                if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
                    ROMPersoBehaviour currentPersoBehaviour = value.AsPersoROM != null ? value.AsPersoROM : null;
                    ROMPersoBehaviour selectedPersoBehaviour = ((ROMPersoBehaviour)EditorGUI.ObjectField(rect, currentPersoBehaviour, typeof(ROMPersoBehaviour), true));

                    if (selectedPersoBehaviour != null && selectedPersoBehaviour.gameObject != null) {
                        value.AsPersoROM = selectedPersoBehaviour;
                    }
                } else {
                    PersoBehaviour currentPersoBehaviour = value.AsPerso != null ? value.AsPerso : null;
                    PersoBehaviour selectedPersoBehaviour = ((PersoBehaviour)EditorGUI.ObjectField(rect, currentPersoBehaviour, typeof(PersoBehaviour), true));

                    if (selectedPersoBehaviour != null && selectedPersoBehaviour.gameObject != null) {
                        value.AsPerso = selectedPersoBehaviour;
                    }
                }
                break;
            case DsgVarInfoEntry.DsgVarType.SuperObject:
                SuperObjectComponent currentGao = value.AsSuperObject != null ? value.AsSuperObject : null;
                SuperObjectComponent selectedGao = ((SuperObjectComponent)EditorGUI.ObjectField(rect, currentGao, typeof(SuperObjectComponent), true));

                if (selectedGao != null && selectedGao.gameObject != null) {
                    value.AsSuperObject = selectedGao;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.WayPoint:
                WayPointBehaviour currentWaypointGao = value.AsWayPoint != null ? value.AsWayPoint : null;
                WayPointBehaviour selectedWaypointGao = ((WayPointBehaviour)EditorGUI.ObjectField(rect, currentWaypointGao, typeof(WayPointBehaviour), true));

                if (selectedWaypointGao != null && selectedWaypointGao.gameObject != null) {
                    value.AsWayPoint = selectedWaypointGao;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.Graph:
                GraphBehaviour currentGraphGao = value.AsGraph != null ? value.AsGraph : null;
                GraphBehaviour selectedGraphGao = ((GraphBehaviour)EditorGUI.ObjectField(rect, currentGraphGao, typeof(GraphBehaviour), true));

                if (selectedGraphGao != null && selectedGraphGao.gameObject != null) {
                    value.AsGraph = selectedGraphGao;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.ActionArray:
            case DsgVarInfoEntry.DsgVarType.FloatArray:
            case DsgVarInfoEntry.DsgVarType.Array11:
            case DsgVarInfoEntry.DsgVarType.GraphArray:
            case DsgVarInfoEntry.DsgVarType.Array9:
            case DsgVarInfoEntry.DsgVarType.IntegerArray:
            case DsgVarInfoEntry.DsgVarType.PersoArray:
            case DsgVarInfoEntry.DsgVarType.SoundEventArray:
            case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
            case DsgVarInfoEntry.DsgVarType.TextArray:
            case DsgVarInfoEntry.DsgVarType.TextRefArray:
            case DsgVarInfoEntry.DsgVarType.VectorArray:
            case DsgVarInfoEntry.DsgVarType.WayPointArray:
                if (value.AsArray != null) {
                    if (arrayIndex.HasValue) {
                        if (value.AsArray[arrayIndex.Value] != null) {
                            DrawDsgVarValue(rect, dsgVarEntry, value.AsArray[arrayIndex.Value], arrayIndex);
                        }
                    } else {
                        EditorGUI.LabelField(rect, "Length: " + value.AsArray.Length);
                    }
                }
                break;
        }
    }

    public static int? DrawText(Rect rect, int textId) {
        int indent = EditorGUI.indentLevel;
        string locIdPreview = textId + " - ";
        if (textId == -1) {
            locIdPreview += "None";
        } else {
            if (MapLoader.Loader.localization != null) {
                locIdPreview += MapLoader.Loader.localization.GetTextForHandleAndLanguageID(textId, 0);
            } else if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
                locIdPreview += (MapLoader.Loader as OpenSpace.Loader.R2ROMLoader).localizationROM?.Lookup(textId);
            }
        }
        EditorGUI.indentLevel = 0;
        int? result = null;
        if (EditorGUI.DropdownButton(rect, new GUIContent(locIdPreview), FocusType.Passive)) {
            if (localizationDropdown == null) {
                localizationDropdown = new LocalizationDropdown(new UnityEditor.IMGUI.Controls.AdvancedDropdownState()) {
                    name = "Text ID"
                };
            }
            localizationDropdown.rect = rect;
            localizationDropdown.Show(rect);
        }
        if (localizationDropdown != null
            && localizationDropdown.selection.HasValue
            && localizationDropdown.rect == rect) {
            result = localizationDropdown.selection.Value;
            localizationDropdown.selection = null;
        }

        EditorGUI.indentLevel = indent;
        return result;
    }
}