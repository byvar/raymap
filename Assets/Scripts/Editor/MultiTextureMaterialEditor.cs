using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MultiTextureMaterial))]
public class MultiTextureMaterialEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MultiTextureMaterial mat = (MultiTextureMaterial)target;
        mat.textureIndex = EditorGUILayout.Popup(mat.textureIndex, mat.textureNames);
    }
}