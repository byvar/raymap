using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.Scripts.OpenSpace.Exporter;
using OpenSpace;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Controller cont = (Controller)target;
        if (GUILayout.Button("Write changes")) {
            cont.SaveChanges();
        }

        if (GUILayout.Button("Export map")) {
            Exporter exporter = new Exporter(MapLoader.Loader, cont.exportPath);
            exporter.Export();
        }
    }
}