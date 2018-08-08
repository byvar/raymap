using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(CustomBitsComponent))]
public class CustomBitsEditor : Editor {

    

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CustomBitsComponent c = (CustomBitsComponent)target;

        GUILayout.Label(c.title);

        GUILayout.BeginVertical();
        for (int j = 0; j < 4; j++) {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 8; i++) {
                int flagNum = ((j * 8) + i);
                GUILayout.Label(flagNum.ToString("D2"));
                c.SetFlag(flagNum, EditorGUILayout.Toggle(c.GetFlag(flagNum)));
            }
            GUILayout.EndHorizontal();
        }
    }
}