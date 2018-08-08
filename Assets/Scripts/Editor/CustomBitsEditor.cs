using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.AI;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(CustomBitsComponent))]
public class CustomBitsEditor : Editor {

    

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();

        CustomBitsComponent c = (CustomBitsComponent)target;
        int len = c.hasAiCustomBits ? 4 : 2;
        for (int k = 0; k < len; k++) {
            CustomBitsComponent.CustomBitsType type = (CustomBitsComponent.CustomBitsType)k;
            GUILayout.Label(type.ToString());
            GUILayout.BeginVertical();
            for (int j = 0; j < 4; j++) {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < 8; i++) {
                    int flagNum = ((j * 8) + i);
                    GUILayout.Label(flagNum.ToString("D2"));
                    c.SetFlag(type, flagNum, EditorGUILayout.Toggle(c.GetFlag(type, flagNum)));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}