using OpenSpace.Collide;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollideComponent))]
public class CollideComponentEditor : Editor {
    CollideComponent col => (CollideComponent)target;

    public override void OnInspectorGUI() {
        float spacing = 23;
        GUILayoutUtility.GetRect(Screen.width, spacing * 16);

        for (int flag = 0; flag < 16; flag++) {
            float p = flag * spacing;
            var rect = new Rect(20, p, Screen.width, spacing - 1);

            EditorGUI.DrawRect(new Rect(rect.x - 8, rect.y + 1, rect.width - 20, rect.height),
                new Color(0, 0, 0, 1f * flag % 2 == 1 ?
                0.2f : 0.15f));

            EditorGUI.LabelField(rect, ((CollideMaterial.Type)(1 << flag)).ToString());
            if (col.col != null && col.col.type.HasFlag((CollideMaterial.Type)flag))
                EditorGUI.LabelField(new Rect(150 + rect.x, rect.y, rect.width, rect.height), "Yes");
        }
    }
}