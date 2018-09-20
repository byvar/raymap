using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SectorManager))]
public class SectorManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        SectorManager man = (SectorManager)target;
        if (GUILayout.Button("Recalculate static lighting")) {
            man.RecalculateSectorLighting();
        }
    }
}