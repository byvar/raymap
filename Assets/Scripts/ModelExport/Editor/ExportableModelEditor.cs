using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExportableModel))]
public class ExportableModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ExportableModel r3AnimatedMesh = (ExportableModel)target;

        if (GUILayout.Button("Add Mesh to Export Objects Library"))
        {
            r3AnimatedMesh.AddToExportObjectsLibrary();
        }
        if (GUILayout.Button("Clear Export Objects Library"))
        {
            r3AnimatedMesh.ClearExportObjectsLibrary();
        }
    }
}

