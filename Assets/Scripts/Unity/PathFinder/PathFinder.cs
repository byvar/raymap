using OpenSpace;
using OpenSpace.Collide;
using OpenSpace.Loader;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour {
    
    public GameObject markerPrefab;
    public float agentRadius = 0.8f;
    public float agentHeight = 0.8f;
    public float agentMaxStep = 5;
    public float agentSlope = 45;

    public List<GameObject> waypoints = new List<GameObject>();
    private List<Vector3> waypointPositions = new List<Vector3>();

    [HideInInspector]
    public State state = State.Default;

    public enum State {
        Default,
        BeginAdd,
        Adding
    }

    private NavMeshDataInstance navMeshDataInstance;

    // Start is called before the first frame update
    void Start()
    {
        MapLoader.Loader.onPostLoad.Add(() =>
        {
            CreateNavMesh();
        });
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> newWayPointPositions = waypoints.Select(wp => wp.transform.position).ToList();
        if (!newWayPointPositions.SequenceEqual(waypointPositions)) {
            waypointPositions = newWayPointPositions;
            ForceUpdate();
        }

        if (state == State.BeginAdd) {
            if (Input.GetMouseButtonDown(0)) {
                state = State.Adding;
            }
        } else if (state == State.Adding) {

            if (Input.GetMouseButton(1)) {
                state = State.Default;
            }

            if (Input.GetMouseButtonUp(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                var hits = Physics.RaycastAll(ray, 10000, 1 << LayerMask.NameToLayer("Collide") | 1 << LayerMask.NameToLayer("Visual")).ToList();

                // closest hits first
                hits.Sort((a, b) =>
                {
                    var distA = (a.point - Camera.main.transform.position).sqrMagnitude;
                    var distB = (b.point - Camera.main.transform.position).sqrMagnitude;
                    return distA < distB ? 1 : distA > distB ? -1 : 0;
                })
;
                foreach (var hit in hits) {
                    if (!(hit.collider is MeshCollider)) {
                        continue;
                    }

                    GameObject marker = GameObject.Instantiate(markerPrefab);
                    marker.transform.parent = transform;
                    marker.transform.position = hit.point;
                    waypoints.Add(marker);
                    
                    ForceUpdate();
                    break;
                }
            }
        }
    }

    private Bounds GetBoundsForUniverse()
    {
        IEnumerable<BoundingVolume> boundingVolumes = null;
        if (Settings.s.platform == Settings.Platform.PS1) {
            boundingVolumes = (MapLoader.Loader as R2PS1Loader).levelHeader.sectors.Select(ps1Sector => ps1Sector.BoundingVolume);
        } else {
            boundingVolumes = MapLoader.Loader.sectors.Select(s => s.SuperObject.boundingVolume);
        }

        var universeBoundingVolume = BoundingVolume.CombineBoxes(boundingVolumes.ToArray());
        return new Bounds(universeBoundingVolume.boxCenter, universeBoundingVolume.boxSize);
    }

    public void CreateNavMesh()
    {
        if (navMeshDataInstance.valid) {
            NavMesh.RemoveNavMeshData(navMeshDataInstance);
        }

        var relevantMeshColliders = GetRelevantMeshColliders();
        var sources = GetSourcesFromMeshColliders(relevantMeshColliders);
        var settings = NavMesh.CreateSettings();
        settings.agentRadius = agentRadius;
        settings.agentClimb = agentMaxStep;
        settings.agentHeight = agentHeight;
        settings.agentSlope = agentSlope;

        var navMeshData = NavMeshBuilder.BuildNavMeshData(settings, sources, GetBoundsForUniverse(), Vector3.zero, Quaternion.identity);

        navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
        NavMesh.CalculateTriangulation();
    }

    private List<NavMeshBuildSource> GetSourcesFromMeshColliders(List<MeshCollider> meshColliders)
    {
        return meshColliders.Select(mc =>
        {
            var source = new NavMeshBuildSource();
            source.shape = NavMeshBuildSourceShape.Mesh;
            source.sourceObject = mc.sharedMesh;
            source.transform = mc.gameObject.transform.localToWorldMatrix;
            source.size = mc.bounds.size;
            return source;
        }).ToList();
    }

    public void ForceUpdate()
    {
        List<Vector3> corners = new List<Vector3>();

        for (int i = 0; i < waypointPositions.Count-1; i++) {

            Vector3 start = waypointPositions[i];
            Vector3 end = waypointPositions[i+1];

            NavMeshPath path = new NavMeshPath();
            for (float oy = -1; oy < 1; oy += 0.1f) {
                NavMesh.CalculatePath(start + new Vector3(0, oy, 0), end + new Vector3(0, oy, 0), -1, path);
                if (path.status == NavMeshPathStatus.PathComplete) {
                    break;
                }
            }

            if (path.status == NavMeshPathStatus.PathComplete) {
                corners.AddRange(path.corners);
            } else if (path.status == NavMeshPathStatus.PathInvalid) {
                corners.Add(start);
                corners.Add(end);
            } else {
                corners.AddRange(path.corners);
                corners.Add(end);
            }
        }
        var r = GetComponent<LineRenderer>();
        r.SetPositions(corners.ToArray());
        r.positionCount = corners.Count;
    }

    private static List<MeshCollider> GetRelevantMeshColliders()
    {
        // CollideComponent is generic, thanks Droolie :)
        return Resources.FindObjectsOfTypeAll<CollideComponent>()
            .Where((cc) => {
                var cmt = cc.col;
                return cmt == null || !(cmt.DeathWarp || cmt.FallTrigger || cmt.Water || cmt.HurtTrigger || cmt.LavaDeathWarp || cmt.NoCollision); 
            })
            .Select(cc => cc.gameObject.GetComponent<MeshCollider>())
            .Where(c=>c!=null).ToList();
    }
}