using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.Waypoints;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using Boo.Lang;

[CustomEditor(typeof(PathFinder))]
public class PathFinderEditor : Editor {

    private string json;

    private PathFinder pf;

    public override void OnInspectorGUI() {

        pf = (PathFinder)target;

        if (pf.state == PathFinder.State.Default) {
            if (GUILayout.Button("Add waypoint")) {
                pf.state = PathFinder.State.BeginAdd;
            }
        } else if (GUILayout.Button("Done")) {
            pf.state = PathFinder.State.Default;
        }

        if (GUILayout.Button("Remove last waypoint")) {
            RemoveLastWayPoint();
        }

        if (GUILayout.Button("Remove all waypoints")) {
            RemoveAllWayPoints();
        }

        if (GUILayout.Button("Rebuild NavMesh")) {
            pf.CreateNavMesh();
            pf.ForceUpdate();
        }

        GUILayout.Space(20);

        DrawDefaultInspector();


        GUILayout.Space(20);

        json = EditorGUILayout.TextArea(json);
        if (GUILayout.Button("Export waypoints to JSON")) {
            json = JsonConvert.SerializeObject(pf.GetWayPointPositions());
        }
        if (GUILayout.Button("Import waypoints from JSON")) {
            RemoveAllWayPoints();
            pf.ImportJSON(json);
        }

    }

    private void RemoveAllWayPoints()
    {
        while (pf.waypoints.Count > 0) {
            RemoveLastWayPoint();
        }
    }

    private void RemoveLastWayPoint()
    {
        if (pf.waypoints.Count > 0) {
            Destroy(pf.waypoints[pf.waypoints.Count - 1]);
            pf.waypoints.RemoveAt(pf.waypoints.Count - 1);
        }
    }
}