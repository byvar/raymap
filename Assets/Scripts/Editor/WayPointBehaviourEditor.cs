using UnityEngine;
using System.Collections;
using UnityEditor;
using OpenSpace.Waypoints;

[CustomEditor(typeof(WayPointBehaviour))]
public class WayPointBehaviourEditor : Editor {
    
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        /*WaypointBehaviour wpBehaviour = (WaypointBehaviour)target;
        var node = wpBehaviour.node;
        GUILayout.Label("List of arcs offsets:");
        int i = 0;
		if (node?.arcList?.list != null) {
            foreach (var arc in node.arcList.list) {
                //arc.
                GUILayout.Label("Arc #" + i + ", caps @" + (arc.offset + 0x10).ToString() + ", points to " + arc.graphNode.offset.ToString());
                i++;
            }
        }*/
    }
}