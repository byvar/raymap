using OpenSpace;
using OpenSpace.Object;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphManager : MonoBehaviour {
	public Controller controller;
    //bool loaded = false;
    public List<WayPointBehaviour> waypoints;
	public List<GraphBehaviour> graphs;
	public Dictionary<Graph, GraphBehaviour> graphDict = new Dictionary<Graph, GraphBehaviour>();
	public Dictionary<OpenSpace.ROM.Graph, GraphBehaviour> graphROMDict = new Dictionary<OpenSpace.ROM.Graph, GraphBehaviour>();
	private GameObject graphRoot = null;
	private GameObject isolateWaypointRoot = null;

	// Use this for initialization
	void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

	public void AddWaypoint(WayPointBehaviour wp) {
		waypoints.Add(wp);
		wp.manager = this;
	}

	public void UpdateViewGraphs() {
		bool viewGraphs = controller.viewGraphs;
		if (graphRoot != null) graphRoot.SetActive(viewGraphs);
		if (isolateWaypointRoot != null) isolateWaypointRoot.SetActive(viewGraphs);
	}

	public void Init() {
		if (MapLoader.Loader is OpenSpace.Loader.R2ROMLoader) {
			OpenSpace.Loader.R2ROMLoader l = MapLoader.Loader as OpenSpace.Loader.R2ROMLoader;
			foreach (OpenSpace.ROM.WayPoint wp in l.waypointsROM) {
				AddWaypoint(wp.GetGameObject());
			}
			if (graphRoot == null && l.graphsROM.Count > 0) {
				graphRoot = new GameObject("Graphs");
				graphRoot.transform.SetParent(transform);
				graphRoot.SetActive(false);
			}
			foreach (OpenSpace.ROM.Graph graph in l.graphsROM) {
				GameObject go_graph = new GameObject("Graph " + graph.Offset);
				go_graph.transform.SetParent(graphRoot.transform);
				GraphBehaviour gb = go_graph.AddComponent<GraphBehaviour>();
				gb.graphROM = graph;
				graphROMDict[graph] = gb;

				for (int i = 0; i < graph.num_nodes; i++) {
					OpenSpace.ROM.GraphNode node = graph.nodes.Value.nodes[i].Value;
					if (node.waypoint.Value != null) {
						WayPointBehaviour wp = waypoints.FirstOrDefault(w => w.wpROM == node.waypoint.Value);
						if (wp != null) {
							gb.nodes.Add(wp);
							wp.nodesROM.Add(node);
							wp.name = "GraphNode[" + i + "].WayPoint (" + wp.wpROM.Offset + ")";
							if (i == 0) {
								go_graph.transform.position = wp.transform.position;
							}
							wp.transform.SetParent(go_graph.transform);
						}
					}
				}
			}
		} else {
			MapLoader l = MapLoader.Loader;
			foreach (WayPoint wp in l.waypoints) {
				AddWaypoint(wp.Gao.GetComponent<WayPointBehaviour>());
			}
			if (graphRoot == null && l.graphs.Count > 0) {
				graphRoot = new GameObject("Graphs");
				graphRoot.transform.SetParent(transform);
				graphRoot.SetActive(false);
			}
			foreach (Graph graph in l.graphs) {
				GameObject go_graph = new GameObject(graph.name ?? "Graph " + graph.offset.ToString());
				go_graph.transform.SetParent(graphRoot.transform);
				GraphBehaviour gb = go_graph.AddComponent<GraphBehaviour>();
				gb.graph = graph;
				graphDict[graph] = gb;

				for (int i = 0; i < graph.nodes.Count; i++) {
					GraphNode node = graph.nodes[i];
					if (node == null) continue;
					if (node.wayPoint != null) {
						WayPointBehaviour wp = waypoints.FirstOrDefault(w => w.wp == node.wayPoint);
						if (wp != null) {
							gb.nodes.Add(wp);
							wp.nodes.Add(node);
							wp.name = "GraphNode[" + i + "].WayPoint (" + wp.wp.offset + ")";
							if (i == 0) {
								go_graph.transform.position = wp.transform.position;
							}
							wp.transform.SetParent(go_graph.transform);
						}
					}
				}
			}
		}

		List<WayPointBehaviour> isolateWaypoints = waypoints.Where(w => w.nodes.Count == 0 && w.nodesROM.Count == 0).ToList();
		if (isolateWaypointRoot == null && isolateWaypoints.Count > 0) {
			isolateWaypointRoot = new GameObject("Isolate WayPoints");
			isolateWaypointRoot.transform.SetParent(transform);
			isolateWaypointRoot.SetActive(false);
		}
		foreach (WayPointBehaviour wp in isolateWaypoints) {
			wp.name = "Isolate WayPoint @" + (wp.wpROM != null ? wp.wpROM.Offset : wp.wp.offset);
			wp.transform.SetParent(isolateWaypointRoot.transform);
		}
		foreach (WayPointBehaviour wp in waypoints) {
			wp.Init();
		}
		UpdateViewGraphs();
		//loaded = true;
	}
}
