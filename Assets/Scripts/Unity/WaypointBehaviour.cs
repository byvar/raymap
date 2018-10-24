using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointBehaviour : MonoBehaviour {
    public GraphNode node = null;
    public LineRenderer[] lines = null;

    void Start() {
        if (node != null) {
            lines = new LineRenderer[node.arcList.list.Count];
            for (int i = 0; i < node.arcList.list.Count; i++) {
                Arc arc = node.arcList.list[i];
                Color color = Color.white;
                if (arc.weight == -1) {
                    color = Color.white;
                } else {
                    Random.InitState((int)arc.capabilities * 33);
                    color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                }
                lines[i] = new GameObject("Arc").AddComponent<LineRenderer>();
                lines[i].transform.SetParent(transform);
                lines[i].material = new Material(Shader.Find("Custom/Line"));
                lines[i].gameObject.hideFlags |= HideFlags.HideInHierarchy;
                lines[i].material.color = color;
                lines[i].positionCount = 2;
                lines[i].useWorldSpace = true;
                lines[i].SetPositions(new Vector3[] { transform.position, arc.graphNode.Gao.transform.position });
                lines[i].widthMultiplier = (arc.weight > 0 ? arc.weight : 30f) / 30f;

                //DrawLineThickness(transform.position, arc.graphNode.Gao.transform.position, arc.weight > 0 ? arc.weight : 100);
            }
        }
    }

    void Update() {
        if (lines == null) return;
        for(int i = 0; i < node.arcList.list.Count; i++) {
            Arc arc = node.arcList.list[i];
            LineRenderer lr = lines[i];
            if (lr == null) continue;
            Vector3 ArrowOrigin = transform.position;
            Vector3 ArrowTarget = arc.graphNode.Gao.transform.position;
            //lr.SetPositions(new Vector3[] { transform.position, arc.graphNode.Gao.transform.position });
            float AdaptiveSize = 1f / Vector3.Distance(ArrowOrigin, ArrowTarget);
            if (AdaptiveSize < 0.5f) {
                lr.widthCurve = new AnimationCurve(
                    new Keyframe(0, 0f),
                    new Keyframe(AdaptiveSize, 0.4f),
                    new Keyframe(0.999f - AdaptiveSize, 0.4f),  // neck of arrow
                    new Keyframe(1 - AdaptiveSize, 1f), // 20f / (arc.weight > 0 ? arc.weight : 30f)),  // max width of arrow head
                    new Keyframe(1, 0f)); // tip of arrow
                lr.positionCount = 5;
                lr.SetPositions(new Vector3[] {
                ArrowOrigin,
                Vector3.Lerp(ArrowOrigin, ArrowTarget, AdaptiveSize),
                Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.999f - AdaptiveSize),
                Vector3.Lerp(ArrowOrigin, ArrowTarget, 1 - AdaptiveSize),
                ArrowTarget });
            } else {
                lr.positionCount = 2;
                lr.SetPositions(new Vector3[] { ArrowOrigin, ArrowTarget });
            }
        }
    }

    // Use this for initialization
    void OnDrawGizmos() {
        /*if (node != null) {
            foreach (Arc arc in node.arcList.list) {
                if (arc.weight == -1) {
                    Gizmos.color = Color.white;
                } else {
                    Random.InitState((int)arc.capabilities * 33);
                    Gizmos.color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                }
                DrawLineThickness(transform.position, arc.graphNode.Gao.transform.position, arc.weight > 0 ? arc.weight : 100);
            }
        }*/
        Gizmos.DrawIcon(transform.position, "WPtex.png", false);
    }

    /*public static void DrawLineThickness(Vector3 p1, Vector3 p2, float width) {
        int count = Mathf.CeilToInt(width); // how many lines are needed.
        if (count == 1)
            Gizmos.DrawLine(p1, p2);
        else {
            Camera c = Camera.current;
            if (c == null) {
                Debug.LogError("Camera.current is null");
                return;
            }
            Vector3 v1 = (p2 - p1).normalized; // line direction
            Vector3 v2 = (c.transform.position - p1).normalized; // direction to camera
            Vector3 n = Vector3.Cross(v1, v2); // normal vector
            for (int i = 0; i < count; i++) {
                Vector3 o = n * width * ((float)i / (count - 1) - 0.5f) * 0.01f;
                Gizmos.DrawLine(p1 + o, p2 + o);
            }
        }
    }*/
}