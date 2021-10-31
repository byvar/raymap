using System.Collections;
using System.Collections.Generic;
using OpenSpace;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using UnityEngine;

public class NormalArrowManager : MonoBehaviour
{
    public GameObject ArrowPrefab;
    public bool WallArrows = false;
    public bool FloorArrows = true;
    public bool CeilingArrows = false;
    private List<GameObject> arrows = new List<GameObject>();

    public void ClearArrows()
    {
        if (arrows != null) {
            foreach (var arrow in arrows) {
                Destroy(arrow);
            }

            arrows.Clear();
        } else {
            arrows = new List<GameObject>();
        }
    }

    private static List<PhysicalObject> RetrievePhysicalObjectsFromIPO(IPO ipo)
    {
        List<PhysicalObject> pos = new List<PhysicalObject>();

        bool spoHasNoCollisionFlag = ipo.SuperObject?.flags.HasFlag(SuperObjectFlags.Flags.NoCollision) ?? false;

        if (spoHasNoCollisionFlag) {
            return pos;
        }
        pos.Add(ipo.data);

        foreach (var s in ipo.SuperObject.children) {
            if (s.data is IPO ipoChild) {
                pos.AddRange(RetrievePhysicalObjectsFromIPO(ipoChild));
                pos.Add(ipoChild.data);
            }
        }

        return pos;
    }

    public void GenerateArrows()
    {
        ClearArrows();

        var loader = MapLoader.Loader;

        List<PhysicalObject> pos = new List<PhysicalObject>();

        foreach (var loaderSector in loader.sectors) {
            if (loaderSector.isSectorVirtual!=0) {
                continue;
            }

            foreach (var s in loaderSector.SuperObject.children) {
                if (s.data is IPO ipo) {
                    pos.AddRange(RetrievePhysicalObjectsFromIPO(ipo));
                }
            }
        }

        foreach (PhysicalObject po in pos) {
            if (po?.collideMesh != null) {
                foreach (var e in po.collideMesh.elements) {
                    var mf = e?.Gao?.GetComponent<MeshFilter>();
                    if (mf != null) {
                        for (int i = 0; i < mf.mesh.triangles.Length; i += 3) {
                            var i0 = mf.mesh.triangles[i];
                            var i1 = mf.mesh.triangles[i+1];
                            var i2 = mf.mesh.triangles[i+2];

                            Vector3 v0 = mf.mesh.vertices[i0];
                            Vector3 v1 = mf.mesh.vertices[i1];
                            Vector3 v2 = mf.mesh.vertices[i2];

                            var centerPoint = (v0 + v1 + v2) / 3;

                            var side1 = v1 - v0;
                            var side2 = v2 - v0;

                            var norm = Vector3.Cross(side1, side2).normalized;

                            GenerateOneArrow(centerPoint + po.Gao.transform.position, norm);
                        }
                    }
                }
            }
        }
    }

    private void GenerateOneArrow(Vector3 centerPoint, Vector3 norm)
    {
        float sin45 = 0.70710678118f;

        if (norm.y < -sin45 && !CeilingArrows) return;
        if (norm.y >= -sin45 && norm.y<=sin45 && !WallArrows) return;
        if (norm.y > sin45 && !FloorArrows) return;

        var arrow = Instantiate(ArrowPrefab);
        arrow.transform.position = centerPoint;
        arrow.transform.rotation = Quaternion.LookRotation(norm, Vector3.up);

        arrows.Add(arrow);
    }
}
