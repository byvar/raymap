using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace;

[CustomEditor(typeof(FamilyComponent))]
public class FamilyEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        FamilyComponent fc = (FamilyComponent)target;
        foreach(MechanicsIDCard idCard in fc.idCards) {
            GUILayout.BeginVertical();
            GUILayout.Label("Gravity: " + idCard.gravity);
            GUILayout.Label("MaxInertia: " + idCard.maxInertia);

            MechanicsIDCardFlags editableFlags = new MechanicsIDCardFlags();
            editableFlags.SetRawFlags((int)idCard.flags);
            GUILayout.Label("Flags: "+editableFlags.flagPreview);
            uint oldFlags = idCard.flags;
            idCard.flags = (uint)editableFlags.DrawEditorAndReturnFlags();

            if (oldFlags!=idCard.flags) {
                fc.dirty = true;
            }

            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        GUILayout.EndVertical();

    }
}