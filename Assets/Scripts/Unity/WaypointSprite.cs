using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointSprite : MonoBehaviour {

    // Use this for initialization
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "WPtex.png", false);
    }
}