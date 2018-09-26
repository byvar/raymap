using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LightManager))]
public class LightManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        LightManager man = (LightManager)target;
        if (man.sectorManager != null && man.IsLoaded) {
            if (GUILayout.Button("Recalculate static lighting")) {
                man.RecalculateSectorLighting();
            }
        }
    }
}