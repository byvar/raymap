using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Controller cont = (Controller)target;
        if (GUILayout.Button("Save changes")) {
            cont.SaveChanges();
        }
    }
}