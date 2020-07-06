using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ROMScriptComponent))]
public class ROMScriptComponentEditor : Editor {
    Vector2 scrollPos;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ROMScriptComponent script = (ROMScriptComponent)target;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(650f), GUILayout.ExpandWidth(true));
        EditorGUILayout.TextArea(script.TranslatedScript, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }
}