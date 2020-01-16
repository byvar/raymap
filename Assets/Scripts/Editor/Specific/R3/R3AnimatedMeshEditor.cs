using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(R3AnimatedMesh))]
public class R3AnimatedMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        R3AnimatedMesh r3AnimatedMesh = (R3AnimatedMesh)target;

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

