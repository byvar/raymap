using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using OpenSpace;
using System.Linq;

[CustomEditor(typeof(ScriptComponent))]
public class ScriptComponentEditor : Editor {
    Vector2 refScrollPos;
    Vector2 scrollPos;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ScriptComponent script = (ScriptComponent)target;
        Script s = script.script;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(650f), GUILayout.ExpandWidth(true));
        EditorGUILayout.TextArea(script.TranslatedScript, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (s.behaviorOrMacro is Behavior) {
            var referencedBy = (s.behaviorOrMacro as Behavior).referencedBy;
            if (referencedBy?.Count>0) {
                GUILayout.Label("The behavior this script belongs to is referenced by");
                refScrollPos = GUILayout.BeginScrollView(refScrollPos, GUILayout.MaxHeight(300));
                referencedBy.ForEach(r =>
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{r.behaviorOrMacro.aiModel.name} - {r.behaviorOrMacro}");
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Show")) {

                        GameObject found = null;

                        foreach(var v in Object.FindObjectsOfType<ScriptComponent>()) {
                            if (v.script == r) {
                                found = v.gameObject;
                                break;
                            }
                        }

                        if (found!=null) {
                            EditorGUIUtility.PingObject(found);
                        } else {
                            Debug.LogWarning("Could not find object that uses script "+r);
                        }
                    }
                    GUILayout.EndHorizontal();
                });
                GUILayout.EndScrollView();
            } else {
                GUILayout.Label("The behavior this script belongs to is not referenced by any script in this map");
            }
        }
    }
}