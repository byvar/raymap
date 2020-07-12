using OpenSpace.Collide;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollideComponent))]
public class CollideComponentEditor : Editor {
    CollideComponent col => (CollideComponent)target;

    public override void OnInspectorGUI() {
        var flags = col.CollisionFlagsR2;

        EditorGUILayout.EnumPopup(col.type);
        EditorGUILayout.EnumFlagsField(flags);
    }
}