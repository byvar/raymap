using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using OpenSpace;

[CustomEditor(typeof(DsgVarComponent))]
public class DsgVarCustomEditor : Editor
{
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

            entry.DrawInspector();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }
}