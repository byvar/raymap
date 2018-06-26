using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PersoBehaviour))]
public class PersoBehaviourEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersoBehaviour pb = (PersoBehaviour)target;
        pb.stateIndex = EditorGUILayout.Popup(pb.stateIndex, pb.stateNames);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Previous state")) pb.SetState(pb.stateIndex - 1);
        if (GUILayout.Button("Next state")) pb.SetState(pb.stateIndex + 1);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print scripts")) pb.PrintScripts();
    }
}