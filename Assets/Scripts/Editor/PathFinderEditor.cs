using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.Waypoints;

[CustomEditor(typeof(PathFinder))]
public class PathFinderEditor : Editor {
    
    public override void OnInspectorGUI() {

        PathFinder pf = (PathFinder)target;

        DrawDefaultInspector();

        GUI.enabled = pf.state == PathFinder.State.Default;
        if (GUILayout.Button("Add waypoint")) {
            pf.state = PathFinder.State.BeginAdd;
        }
        if (GUILayout.Button("Remove last waypoint")) {
            pf.waypoints.RemoveAt(pf.waypoints.Count - 1); 
        }
        if (GUILayout.Button("Force Update")) {
            pf.ForceUpdate();
        }
        if (GUILayout.Button("Rebuild NavMesh")) {
            pf.CreateNavMesh();
        }

        GUI.enabled = true;
        if (pf.state!=PathFinder.State.Default && GUILayout.Button("Done")) {
            pf.state = PathFinder.State.Default;
        }
    }
}