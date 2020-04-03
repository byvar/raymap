using OpenSpace.Collide;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollideComponent))]
public class CollideComponentEditor : Editor {
    CollideComponent col => (CollideComponent)target;

    public override void OnInspectorGUI() {
        var flags = CollideMaterial.CollisionFlags_R2.None;

        if (col.col != null) flags = col.col.identifier_R2;
        else if (col.colROM != null) flags = (CollideMaterial.CollisionFlags_R2)col.colROM.identifier;

        EditorGUILayout.EnumPopup(col.type);
        EditorGUILayout.EnumFlagsField(flags);
    }
}