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
        pb.poListIndex = EditorGUILayout.Popup(pb.poListIndex, pb.poListNames);
        pb.stateIndex = EditorGUILayout.Popup(pb.stateIndex, pb.stateNames);

        GUILayout.BeginHorizontal();
        GUI.enabled = pb.stateIndex > 0;
        if (GUILayout.Button("Previous state")) pb.SetState(pb.stateIndex - 1);
        GUI.enabled = (pb.stateNames != null && pb.stateIndex < pb.stateNames.Length - 1);
        if (GUILayout.Button("Next state")) pb.SetState(pb.stateIndex + 1);
        GUI.enabled = true;
        GUILayout.EndHorizontal();
		
        if (GUILayout.Button("Print scripts")) pb.PrintScripts();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Print DsgVar")) pb.PrintDsgVar();
        if (GUILayout.Button("Print DsgVar from Mind->DsgMem")) pb.PrintDsgVarFromMindMem();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Print Animation Debug Info")) pb.PrintAnimationDebugInfo();
    }
}