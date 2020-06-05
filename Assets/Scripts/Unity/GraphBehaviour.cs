using OpenSpace.Object;
using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphBehaviour : MonoBehaviour, IReferenceable {
	public GraphManager manager;
    public List<WayPointBehaviour> nodes = new List<WayPointBehaviour>();
    public OpenSpace.ROM.Graph graphROM;
    public OpenSpace.Waypoints.Graph graph;

    public ReferenceFields References { get => ((IReferenceable)graph).References; set => ((IReferenceable)graph).References = value; }
}