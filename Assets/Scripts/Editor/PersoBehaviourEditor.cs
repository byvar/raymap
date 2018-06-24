using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;

[CustomEditor(typeof(PersoBehaviour))]
public class PersoBehaviourEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PersoBehaviour pb = (PersoBehaviour)target;
        pb.stateIndex = EditorGUILayout.Popup(pb.stateIndex, pb.stateNames);

        if (GUILayout.Button("Print scripts")) {
            pb.PrintScripts();
        }

        if (GUILayout.Button("Print DsgVar")) {
            pb.PrintDsgVar();
        }

        if (GUILayout.Button("Print DsgVar from Mind->DsgMem")) {
            pb.PrintDsgVarFromMindMem();
        }
    }
}