using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.Exporter;
using OpenSpace;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Controller cont = (Controller)target;
        if (GUILayout.Button("Write changes")) {
            cont.SaveChanges();
        }

        cont.ExportFlags = (MapExporter.ExportFlags) EditorGUILayout.EnumFlagsField(cont.ExportFlags);

        if (GUILayout.Button("Export map")) {
            MapExporter exporter = new MapExporter(MapLoader.Loader, cont.ExportPath, cont.ExportFlags);
            exporter.Export();
        }

        if (GUILayout.Button("Print all translated scripts")) {
            if (cont.romPersos?.Count>0) {
                foreach(var romPerso in cont.romPersos) {
                    Debug.Log("romPerso: " + romPerso.name);
                    romPerso.PrintTranslatedScripts();
                }
            }
        }
    }
}