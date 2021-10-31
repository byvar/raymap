using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NormalArrowManager))]
public class NormalArrowManagerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NormalArrowManager m = (NormalArrowManager)target;
        if (GUILayout.Button("Generate Arrows")) {
            m.GenerateArrows();
        }
    }
}