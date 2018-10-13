using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ScriptComponent))]
public class ScriptComponentEditor : Editor {
    Vector2 scrollPos;
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ScriptComponent script = (ScriptComponent)target;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(650f), GUILayout.ExpandWidth(true));
        EditorGUILayout.TextArea(script.TranslatedScript, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();
    }
}