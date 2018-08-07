using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using OpenSpace;

[CustomEditor(typeof(DsgVarComponent))]
public class DesignerVariableCustomEditor : Editor
{
    public Vector2 scrollPosition = new Vector2(0, 0);

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DsgVarComponent c = (DsgVarComponent)target;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, new GUIStyle());

        int number = 0;
        foreach(DsgVarInfoEntry entry in c.dsgVarEntries) {

            GUILayout.BeginHorizontal();
            GUILayout.Button(entry.type + " dsgVar_"+number);
            GUILayout.EndHorizontal();

            number++;
        }
        GUILayout.EndScrollView();
    }
}