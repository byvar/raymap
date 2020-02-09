using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphBehaviour : MonoBehaviour {
	public GraphManager manager;
    public List<WayPointBehaviour> nodes = new List<WayPointBehaviour>();
    public OpenSpace.ROM.Graph graphROM;
    public OpenSpace.Waypoints.Graph graph;
}