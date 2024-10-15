﻿using OpenSpace.Object;
using OpenSpace.Waypoints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenSpace;
using UnityEngine;

public class WayPointBehaviour : MonoBehaviour, IReferenceable {
	public GraphManager manager;
	public List<GraphNode> nodes = new List<GraphNode>();
	public List<OpenSpace.ROM.GraphNode> nodesROM = new List<OpenSpace.ROM.GraphNode>();
    public List<OpenSpace.PS1.Arc> arcsPS1 = new List<OpenSpace.PS1.Arc>();
    public List<GraphBehaviour> graphs = new List<GraphBehaviour>();
    public WayPoint wp = null;
    public OpenSpace.ROM.WayPoint wpROM = null;
    public OpenSpace.PS1.WayPoint wpPS1 = null;
    private List<WayPointBehaviour> targets = new List<WayPointBehaviour>();
	private List<LineRenderer> lines = new List<LineRenderer>();
    private List<Vector3> targetPositions = new List<Vector3>();
    private Vector3 currentPosition = Vector3.zero;
    public float radius;

    public List<WayPointBehaviour> Targets => targets;

    public ReferenceFields References { get => ((IReferenceable)wp).References; set => ((IReferenceable)wp).References = value; }

    public void Init() {
        currentPosition = transform.position;
        if (nodes.Count > 0) {
            foreach (GraphNode node in nodes) {
                for (int i = 0; i < node.arcList.list.Count; i++) {
                    Arc arc = node.arcList.list[i];
                    Color color = Color.white;
                    if (arc.weight == -1) {
                        color = Color.white;
                    } else {
                        Random.InitState((int)arc.capabilities * 33);
                        color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                    }
                    LineRenderer lr = new GameObject("Arc").AddComponent<LineRenderer>();
                    lr.transform.SetParent(transform);
                    lr.material = new Material(Shader.Find("Custom/Line"));
                    lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.material.color = color;
                    lr.positionCount = 2;
                    lr.useWorldSpace = true;
                    WayPointBehaviour tar = manager.waypoints.FirstOrDefault(w => w.nodes.Contains(arc.graphNode));
                    lr.SetPositions(new Vector3[] { transform.position, tar.transform.position });
                    var divisor = 30f;
					if(Legacy_Settings.s.game == Legacy_Settings.Game.R2Beta) divisor = 1;
					lr.widthMultiplier = (arc.weight > 0 ? arc.weight : divisor) / divisor;
                    lines.Add(lr);
                    targets.Add(tar);
                    targetPositions.Add(tar.transform.position);
                    UpdateLine(i);
                    //DrawLineThickness(transform.position, arc.graphNode.Gao.transform.position, arc.weight > 0 ? arc.weight : 100);
                }
            }
        } else if (nodesROM.Count > 0) {
            foreach (OpenSpace.ROM.GraphNode node in nodesROM) {
                for (int i = 0; i < node.num_arcs; i++) {
                    Color color = Color.white;
                    int weight = -1;
                    if (node.arcs_weights.Value != null && node.arcs_weights.Value.weights[i] != 0xFFFF) {
                        weight = node.arcs_weights.Value.weights[i];
                    }
                    if (weight == -1) {
                        color = Color.white;
                    } else {
                        uint caps = 0;
                        if (node.arcs_caps.Value != null) caps = node.arcs_caps.Value.caps[i];
                        Random.InitState((int)caps * 33);
                        color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                    }
                    LineRenderer lr = new GameObject("Arc").AddComponent<LineRenderer>();
                    lr.transform.SetParent(transform);
                    lr.material = new Material(Shader.Find("Custom/Line"));
                    lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.material.color = color;
                    lr.positionCount = 2;
                    lr.useWorldSpace = true;
                    WayPointBehaviour tar = manager.waypoints.FirstOrDefault(w => w.nodesROM.Contains(node.arcs_nodes.Value.nodes[i].Value));
                    lr.SetPositions(new Vector3[] { transform.position, tar.transform.position });
                    lr.widthMultiplier = (weight > 0 ? weight : 30f) / 30f;
                    lines.Add(lr);
                    targets.Add(tar);
                    targetPositions.Add(tar.transform.position);
                    UpdateLine(i);
                    //DrawLineThickness(transform.position, arc.graphNode.Gao.transform.position, arc.weight > 0 ? arc.weight : 100);
                }
            }
        } else if (arcsPS1.Count > 0) {
            for (int i = 0; i < arcsPS1.Count; i++) {
                OpenSpace.PS1.Arc arc = arcsPS1[i];
                OpenSpace.PS1.WayPoint otherNode = arc.node1 == wpPS1 ? arc.node2 : arc.node1;
                Color color = Color.white;
                /*if (arc.weight == -1) {
                    color = Color.white;
                } else {*/
                    Random.InitState((int)arc.ushort_0C * 33);
                    color = Random.ColorHSV(0, 1, 0.2f, 1f, 0.4f, 1.0f);
                //}
                LineRenderer lr = new GameObject("Arc").AddComponent<LineRenderer>();
                lr.transform.SetParent(transform);
                lr.material = new Material(Shader.Find("Custom/Line"));
                lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                lr.material.color = color;
                lr.positionCount = 2;
                lr.useWorldSpace = true;
                WayPointBehaviour tar = manager.waypoints.FirstOrDefault(w => w.wpPS1 == otherNode);
                lr.SetPositions(new Vector3[] { transform.position, tar.transform.position });
                //lr.widthMultiplier = (arc.weight > 0 ? arc.weight : 30f) / 30f;
                lr.widthMultiplier = 0.5f;
                lines.Add(lr);
                targets.Add(tar);
                targetPositions.Add(tar.transform.position);
                UpdateLine(i);
                //DrawLineThickness(transform.position, arc.graphNode.Gao.transform.position, arc.weight > 0 ? arc.weight : 100);
            }
        }
        CreateMesh();
    }

    void CreateMesh() {
        float defaultRadius = 1f;
        if (wpPS1 != null) {
            defaultRadius = (256f / OpenSpace.Loader.R2PS1Loader.CoordinateFactor);
        }
        MeshFilter mf = gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
        Material unityMat = Resources.Load("Material_WP") as Material;
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = radius > defaultRadius ? radius : defaultRadius;
        mr.material = unityMat;
        Mesh meshUnity = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, -1, -1);
        vertices[1] = new Vector3(0, -1, 1);
        vertices[2] = new Vector3(0, 1, -1);
        vertices[3] = new Vector3(0, 1, 1);
        Vector3[] normals = new Vector3[4];
        normals[0] = Vector3.forward;
        normals[1] = Vector3.forward;
        normals[2] = Vector3.forward;
        normals[3] = Vector3.forward;
        Vector3[] uvs = new Vector3[4];
        uvs[0] = new Vector3(0, 0, 1);
        uvs[1] = new Vector3(1, 0, 1);
        uvs[2] = new Vector3(0, 1, 1);
        uvs[3] = new Vector3(1, 1, 1);
        int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

        meshUnity.vertices = vertices;
        meshUnity.normals = normals;
        meshUnity.triangles = triangles;
        meshUnity.SetUVs(0, uvs.ToList());


        mf.mesh = meshUnity;


        if (radius > defaultRadius) {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
            sphere.transform.parent = transform;
            sphere.transform.localPosition = Vector3.zero;
            // No collider necessary
            GameObject.Destroy(sphere.GetComponent<SphereCollider>());
            MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
            sphereRenderer.material = new Material(manager.controller.collideTransparentMaterial);
            sphereRenderer.material.color = new Color(0.7f, 0f, 0.7f, 0.5f);
        }
    }

    void Update() {
        if (lines == null) return;
        bool updateAll = false;
        if (currentPosition != transform.position) {
            currentPosition = transform.position;
            updateAll = true;
        }
        for (int i = 0; i < lines.Count; i++) {
            bool updateThis = false;
            if (targetPositions[i] != targets[i].transform.position) {
                targetPositions[i] = targets[i].transform.position;
                updateThis = true;
            }
            if (updateAll || updateThis) {
                UpdateLine(i);
            }
        }
    }

    void UpdateLine(int i) {
        LineRenderer lr = lines[i];
        if (lr == null) return;
        Vector3 ArrowOrigin = currentPosition;
        Vector3 ArrowTarget = targetPositions[i];
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
            /*lr.positionCount = 2;
            lr.SetPositions(new Vector3[] { ArrowOrigin, ArrowTarget });*/
        } else {
            lr.positionCount = 2;
            lr.SetPositions(new Vector3[] { ArrowOrigin, ArrowTarget });
        }
    }

    // Use this for initialization
    //void OnDrawGizmos() {
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
        //Gizmos.DrawIcon(transform.position, "WPtex.png", false);
    //}

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
    public void SaveChanges(Writer writer)
    {
        LegacyPointer.Goto(ref writer, wp.offset);
        writer.Write(transform.position.x);
        writer.Write(transform.position.z);
        writer.Write(transform.position.y);
    }
}