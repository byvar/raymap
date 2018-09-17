using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointLine : MonoBehaviour {

    public Vector3 from;
    public Vector3 to;

    public uint capabilities;
    public int weight;

    // Use this for initialization
    void OnDrawGizmos()
    {
        Random.seed = (int)this.capabilities * 33;
        Gizmos.color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
        if (weight == -1) {
            Gizmos.color = Color.white;
        }
        DrawLineThickness(from, to, weight > 0 ? weight : 100);
    }

    public static void DrawLineThickness(Vector3 p1, Vector3 p2, float width)
    {
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
    }
}