using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PersoBehaviour))]
public class PersoBehaviourEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersoBehaviour pb = (PersoBehaviour)target;
        pb.stateIndex = EditorGUILayout.Popup(pb.stateIndex, pb.stateNames);


        if (GUILayout.Button("Print scripts")) {
            pb.PrintScripts();
        }
    }
}