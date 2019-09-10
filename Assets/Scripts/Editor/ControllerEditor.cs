using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.Exporter;
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
            MapExporter exporter = new MapExporter(MapLoader.Loader, cont.ExportPath);
            exporter.Export();
        }
    }
}