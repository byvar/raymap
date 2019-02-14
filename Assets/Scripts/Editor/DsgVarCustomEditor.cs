using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using OpenSpace;
using System;

[CustomEditor(typeof(DsgVarComponent))]
public class DsgVarCustomEditor : Editor {
    public Vector2 scrollPosition = new Vector2(0, 0);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DsgVarComponent c = (DsgVarComponent)target;

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        GUILayout.Label("Type/name");
        GUILayout.Label("Current value");
        GUILayout.Label("Initial value");

        GUILayout.EndHorizontal();

        foreach (DsgVarComponent.DsgVarEditableEntry entry in c.editableEntries) {

            GUILayout.BeginHorizontal();

            DrawDsgVarEntry(entry);

            GUILayout.EndHorizontal();
        }


        if (GUILayout.Button("Print initial dsgvar assignments")) {

            string printResult = "";

            foreach (DsgVarComponent.DsgVarEditableEntry dsgVarEntry in c.editableEntries) {

                printResult += DsgVarEntryToCSharpAssignment(dsgVarEntry) + Environment.NewLine;
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

        GUILayout.EndVertical();
    }

    public void DrawDsgVarEntry(DsgVarComponent.DsgVarEditableEntry dsgVarEntry)
    {
        GUILayout.Label(dsgVarEntry.entry.type + "_" + dsgVarEntry.number);
        string stringVal = "";
        switch (dsgVarEntry.entry.type) {
            case DsgVarInfoEntry.DsgVarType.Boolean: dsgVarEntry.valueAsBool = EditorGUILayout.Toggle(dsgVarEntry.valueAsBool); break;
            case DsgVarInfoEntry.DsgVarType.Int: stringVal = GUILayout.TextField(dsgVarEntry.valueAsInt.ToString()); Int32.TryParse(stringVal, out dsgVarEntry.valueAsInt); break;
            case DsgVarInfoEntry.DsgVarType.UInt: stringVal = GUILayout.TextField(dsgVarEntry.valueAsUInt.ToString()); UInt32.TryParse(stringVal, out dsgVarEntry.valueAsUInt); break;
            case DsgVarInfoEntry.DsgVarType.Short: stringVal = GUILayout.TextField(dsgVarEntry.valueAsShort.ToString()); Int16.TryParse(stringVal, out dsgVarEntry.valueAsShort); break;
            case DsgVarInfoEntry.DsgVarType.UShort: stringVal = GUILayout.TextField(dsgVarEntry.valueAsUShort.ToString()); UInt16.TryParse(stringVal, out dsgVarEntry.valueAsUShort); break;
            case DsgVarInfoEntry.DsgVarType.Byte: stringVal = GUILayout.TextField(dsgVarEntry.valueAsSByte.ToString()); SByte.TryParse(stringVal, out dsgVarEntry.valueAsSByte); break;
            case DsgVarInfoEntry.DsgVarType.UByte: stringVal = GUILayout.TextField(dsgVarEntry.valueAsByte.ToString()); Byte.TryParse(stringVal, out dsgVarEntry.valueAsByte); break;
            case DsgVarInfoEntry.DsgVarType.Float: stringVal = GUILayout.TextField(dsgVarEntry.valueAsFloat.ToString()); Single.TryParse(stringVal, out dsgVarEntry.valueAsFloat); break;
            case DsgVarInfoEntry.DsgVarType.Text: stringVal = GUILayout.TextField(dsgVarEntry.valueAsString.ToString()); dsgVarEntry.valueAsString = stringVal; break;
            case DsgVarInfoEntry.DsgVarType.Vector:
                float val_x = dsgVarEntry.valueAsVector.x;
                float val_y = dsgVarEntry.valueAsVector.y;
                float val_z = dsgVarEntry.valueAsVector.z;
                stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector.x.ToString()); Single.TryParse(stringVal, out val_x);
                stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector.y.ToString()); Single.TryParse(stringVal, out val_y);
                stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector.z.ToString()); Single.TryParse(stringVal, out val_z);
                dsgVarEntry.valueAsVector = new Vector3(val_x, val_y, val_z);
                break;
            case DsgVarInfoEntry.DsgVarType.Perso:
                PersoBehaviour currentPersoBehaviour = dsgVarEntry.valueAsPersoGao != null ? dsgVarEntry.valueAsPersoGao.GetComponent<PersoBehaviour>() : null;
                PersoBehaviour selectedPersoBehaviour = ((PersoBehaviour)EditorGUILayout.ObjectField(currentPersoBehaviour, typeof(PersoBehaviour), true));

                if (selectedPersoBehaviour != null && selectedPersoBehaviour.gameObject != null) {
                    dsgVarEntry.valueAsPersoGao = selectedPersoBehaviour.gameObject;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.SuperObject:
                GameObject currentGao = dsgVarEntry.valueAsSuperObjectGao != null ? dsgVarEntry.valueAsSuperObjectGao : null;
                GameObject selectedGao = ((GameObject)EditorGUILayout.ObjectField(currentGao, typeof(GameObject), true));

                if (selectedGao != null) {
                    dsgVarEntry.valueAsSuperObjectGao = selectedGao;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.Waypoint:
                GameObject currentWaypointGao = dsgVarEntry.valueAsWaypointGao != null ? dsgVarEntry.valueAsWaypointGao : null;
                GameObject selectedWaypointGao = ((GameObject)EditorGUILayout.ObjectField(currentWaypointGao, typeof(GameObject), true));

                if (selectedWaypointGao != null) {
                    dsgVarEntry.valueAsWaypointGao = selectedWaypointGao;
                }
                break;
            case DsgVarInfoEntry.DsgVarType.ActionArray:
            case DsgVarInfoEntry.DsgVarType.FloatArray:
            case DsgVarInfoEntry.DsgVarType.IntegerArray:
            case DsgVarInfoEntry.DsgVarType.PersoArray:
            case DsgVarInfoEntry.DsgVarType.SoundEventArray:
            case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
            case DsgVarInfoEntry.DsgVarType.TextArray:
            case DsgVarInfoEntry.DsgVarType.TextRefArray:
            case DsgVarInfoEntry.DsgVarType.VectorArray:
            case DsgVarInfoEntry.DsgVarType.WayPointArray:

                if (dsgVarEntry.entry.value.GetType().IsArray) {
                    object[] array = (object[])dsgVarEntry.entry.value;

                    GUILayout.BeginVertical();
                    for (int i = 0; i < array.Length; i++) {

                        if (array[i] != null) {
                            GUILayout.TextField(array[i].ToString());
                        }

                    }
                    GUILayout.EndVertical();
                }
                

                break;

        }

        if (dsgVarEntry.entry.initialValue != null) {
            GUILayout.Space(20);

            switch (dsgVarEntry.entry.type) {
                case DsgVarInfoEntry.DsgVarType.Boolean: dsgVarEntry.valueAsBool_initial = EditorGUILayout.Toggle(dsgVarEntry.valueAsBool_initial); break;
                case DsgVarInfoEntry.DsgVarType.Int: stringVal = GUILayout.TextField(dsgVarEntry.valueAsInt_initial.ToString()); Int32.TryParse(stringVal, out dsgVarEntry.valueAsInt_initial); break;
                case DsgVarInfoEntry.DsgVarType.UInt: stringVal = GUILayout.TextField(dsgVarEntry.valueAsUInt_initial.ToString()); UInt32.TryParse(stringVal, out dsgVarEntry.valueAsUInt_initial); break;
                case DsgVarInfoEntry.DsgVarType.Short: stringVal = GUILayout.TextField(dsgVarEntry.valueAsShort_initial.ToString()); Int16.TryParse(stringVal, out dsgVarEntry.valueAsShort_initial); break;
                case DsgVarInfoEntry.DsgVarType.UShort: stringVal = GUILayout.TextField(dsgVarEntry.valueAsUShort_initial.ToString()); UInt16.TryParse(stringVal, out dsgVarEntry.valueAsUShort_initial); break;
                case DsgVarInfoEntry.DsgVarType.Byte: stringVal = GUILayout.TextField(dsgVarEntry.valueAsSByte_initial.ToString()); SByte.TryParse(stringVal, out dsgVarEntry.valueAsSByte_initial); break;
                case DsgVarInfoEntry.DsgVarType.UByte: stringVal = GUILayout.TextField(dsgVarEntry.valueAsByte_initial.ToString()); Byte.TryParse(stringVal, out dsgVarEntry.valueAsByte_initial); break;
                case DsgVarInfoEntry.DsgVarType.Float: stringVal = GUILayout.TextField(dsgVarEntry.valueAsFloat_initial.ToString()); Single.TryParse(stringVal, out dsgVarEntry.valueAsFloat_initial); break;
                case DsgVarInfoEntry.DsgVarType.Text: stringVal = GUILayout.TextField(dsgVarEntry.valueAsString_initial.ToString()); dsgVarEntry.valueAsString_initial = stringVal; break;
                case DsgVarInfoEntry.DsgVarType.Vector:
                    float val_x = dsgVarEntry.valueAsVector_initial.x;
                    float val_y = dsgVarEntry.valueAsVector_initial.y;
                    float val_z = dsgVarEntry.valueAsVector_initial.z;
                    stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector_initial.x.ToString()); Single.TryParse(stringVal, out val_x);
                    stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector_initial.y.ToString()); Single.TryParse(stringVal, out val_y);
                    stringVal = GUILayout.TextField(dsgVarEntry.valueAsVector_initial.z.ToString()); Single.TryParse(stringVal, out val_z);
                    dsgVarEntry.valueAsVector_initial = new Vector3(val_x, val_y, val_z);
                    break;
                case DsgVarInfoEntry.DsgVarType.Perso:
                    PersoBehaviour currentPersoBehaviour = dsgVarEntry.valueAsPersoGao_initial != null ? dsgVarEntry.valueAsPersoGao_initial.GetComponent<PersoBehaviour>() : null;
                    PersoBehaviour selectedPersoBehaviour = ((PersoBehaviour)EditorGUILayout.ObjectField(currentPersoBehaviour, typeof(PersoBehaviour), true));

                    if (selectedPersoBehaviour != null && selectedPersoBehaviour.gameObject != null) {
                        dsgVarEntry.valueAsPersoGao_initial = selectedPersoBehaviour.gameObject;
                    }
                    break;
                case DsgVarInfoEntry.DsgVarType.SuperObject:
                    GameObject currentGao = dsgVarEntry.valueAsSuperObjectGao_initial != null ? dsgVarEntry.valueAsSuperObjectGao_initial : null;
                    GameObject selectedGao = ((GameObject)EditorGUILayout.ObjectField(currentGao, typeof(GameObject), true));

                    if (selectedGao != null) {
                        dsgVarEntry.valueAsSuperObjectGao_initial = selectedGao;
                    }
                    break;

            }
        } else {
            GUILayout.FlexibleSpace();
        }
    }

    public string DsgVarEntryToCSharpAssignment(DsgVarComponent.DsgVarEditableEntry dsgVarEntry)
    {
        string text = "";
        string typeText = "";

        switch (dsgVarEntry.entry.type) {
            case DsgVarInfoEntry.DsgVarType.None:
                break;
            case DsgVarInfoEntry.DsgVarType.Boolean:
                typeText = "bool";  break;
            case DsgVarInfoEntry.DsgVarType.Byte:
                typeText = "byte"; break;
            case DsgVarInfoEntry.DsgVarType.UByte:
                typeText = "unsigned byte"; break;
            case DsgVarInfoEntry.DsgVarType.Short:
                typeText = "short"; break;
            case DsgVarInfoEntry.DsgVarType.UShort:
                typeText = "unsigned short"; break;
            case DsgVarInfoEntry.DsgVarType.Int:
                typeText = "int"; break;
            case DsgVarInfoEntry.DsgVarType.UInt:
                typeText = "unsigned int"; break;
            case DsgVarInfoEntry.DsgVarType.Float:
                typeText = "float"; break;
            case DsgVarInfoEntry.DsgVarType.Vector:
                typeText = "Vector3"; break;
            case DsgVarInfoEntry.DsgVarType.List:
                typeText = "List"; break;
            case DsgVarInfoEntry.DsgVarType.Comport:
                typeText = "Comport"; break;
            case DsgVarInfoEntry.DsgVarType.Action:
                typeText = "Action"; break;
            case DsgVarInfoEntry.DsgVarType.Caps:
                typeText = "Caps"; break;
            case DsgVarInfoEntry.DsgVarType.Input:
                typeText = "Input"; break;
            case DsgVarInfoEntry.DsgVarType.SoundEvent:
                typeText = "SoundEvent"; break;
            case DsgVarInfoEntry.DsgVarType.Light:
                typeText = "Light"; break;
            case DsgVarInfoEntry.DsgVarType.GameMaterial:
                typeText = "GameMaterial"; break;
            case DsgVarInfoEntry.DsgVarType.VisualMaterial:
                typeText = "VisualMaterial"; break;
            case DsgVarInfoEntry.DsgVarType.Perso:
                typeText = "Perso"; break;
            case DsgVarInfoEntry.DsgVarType.Waypoint:
                typeText = "Waypoint"; break;
            case DsgVarInfoEntry.DsgVarType.Graph:
                typeText = "Graph"; break;
            case DsgVarInfoEntry.DsgVarType.Text:
                typeText = "string"; break;
            case DsgVarInfoEntry.DsgVarType.SuperObject:
                typeText = "SuperObject"; break;
            case DsgVarInfoEntry.DsgVarType.SOLinks:
                typeText = "SOLinks"; break;
            case DsgVarInfoEntry.DsgVarType.PersoArray:
                typeText = "List<Perso>"; break;
            case DsgVarInfoEntry.DsgVarType.VectorArray:
                typeText = "List<Vector3>"; break;
            case DsgVarInfoEntry.DsgVarType.FloatArray:
                typeText = "List<float>"; break;
            case DsgVarInfoEntry.DsgVarType.IntegerArray:
                typeText = "List<int>"; break;
            case DsgVarInfoEntry.DsgVarType.WayPointArray:
                typeText = "List<Waypoint>"; break;
            case DsgVarInfoEntry.DsgVarType.TextArray:
                typeText = "List<String>"; break;
            case DsgVarInfoEntry.DsgVarType.TextRefArray:
                typeText = "List<TextRef>"; break;
            case DsgVarInfoEntry.DsgVarType.Array6:
                typeText = "List<Unknown_6>"; break;
            case DsgVarInfoEntry.DsgVarType.Array9:
                typeText = "List<Unknown_7>"; break;
            case DsgVarInfoEntry.DsgVarType.SoundEventArray:
                typeText = "List<SoundEvent>"; break;
            case DsgVarInfoEntry.DsgVarType.Array11:
                typeText = "List<Unknown_11>"; break;
            case DsgVarInfoEntry.DsgVarType.Way:
                typeText = "Way"; break;
            case DsgVarInfoEntry.DsgVarType.ActionArray:
                typeText = "List<Action>"; break;
            case DsgVarInfoEntry.DsgVarType.SuperObjectArray:
                typeText = "List<SuperObject>"; break;
        }

        text += typeText + " " + dsgVarEntry.entry.type + "_" + dsgVarEntry.number;

        if (dsgVarEntry.entry.initialValue != null) {
            text += " = ";

            string stringVal = "";

            switch (dsgVarEntry.entry.type) {
                case DsgVarInfoEntry.DsgVarType.Boolean: stringVal = dsgVarEntry.valueAsBool_initial.ToString().ToLower(); break;
                case DsgVarInfoEntry.DsgVarType.Int: stringVal = (dsgVarEntry.valueAsInt_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.UInt: stringVal = (dsgVarEntry.valueAsUInt_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.Short: stringVal = (dsgVarEntry.valueAsShort_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.UShort: stringVal = (dsgVarEntry.valueAsUShort_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.Byte: stringVal = (dsgVarEntry.valueAsSByte_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.UByte: stringVal = (dsgVarEntry.valueAsByte_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.Float: stringVal = (dsgVarEntry.valueAsFloat_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.Text: stringVal = (dsgVarEntry.valueAsString_initial.ToString()); break;
                case DsgVarInfoEntry.DsgVarType.Vector:
                    float val_x = dsgVarEntry.valueAsVector_initial.x;
                    float val_y = dsgVarEntry.valueAsVector_initial.y;
                    float val_z = dsgVarEntry.valueAsVector_initial.z;

                    stringVal = "new Vector3("+val_x+", "+val_y+", "+val_z+")";
                    break;
                case DsgVarInfoEntry.DsgVarType.Perso:
                    PersoBehaviour currentPersoBehaviour = dsgVarEntry.valueAsPersoGao_initial != null ? dsgVarEntry.valueAsPersoGao_initial.GetComponent<PersoBehaviour>() : null;
                    
                    if (currentPersoBehaviour!=null) {
                        stringVal += "Perso.GetByName("+currentPersoBehaviour.perso.namePerso+")";
                    } else {
                        stringVal += "null";
                    }

                    break;
                case DsgVarInfoEntry.DsgVarType.SuperObject:
                    GameObject currentGao = dsgVarEntry.valueAsSuperObjectGao_initial != null ? dsgVarEntry.valueAsSuperObjectGao_initial : null;

                    if (currentGao != null) {
                        stringVal += "GameObject.GetByName(" + currentGao.name + ")";
                    } else {
                        stringVal += "null";
                    }
                    break;

            }

            text += stringVal;
        }

        text += ";";

        return text;
    }
}