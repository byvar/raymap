using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;

[CustomEditor(typeof(CinematicSwitcher))]
public class CinematicSwitcherEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CinematicSwitcher pb = (CinematicSwitcher)target;
        pb.cinematicIndex = EditorGUILayout.Popup("Current cinematic", pb.cinematicIndex, pb.cinematicNames);
    }
}