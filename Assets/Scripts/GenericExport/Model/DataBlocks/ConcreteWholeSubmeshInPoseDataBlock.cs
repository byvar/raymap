using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public class ConcreteWholeSubmeshInPoseDataBlock : DataBlock
    {
        public ObjectTransform transform;
        public List<ExportVector3> vertices = new List<ExportVector3>();
        public List<int> triangles = new List<int>();

        public ConcreteWholeSubmeshInPoseDataBlock(
            ObjectTransform transform, List<ExportVector3> vertices, List<int> triangles)
        {
            this.transform = transform;
            this.vertices = vertices;
            this.triangles = triangles;
        }

        public static ConcreteWholeSubmeshInPoseDataBlock FromSubmesh(Transform child)
        {
            Mesh mesh = child.GetComponent<SkinnedMeshRenderer>() != null ?
                child.GetComponent<SkinnedMeshRenderer>().sharedMesh : child.GetComponent<MeshFilter>().mesh;

            return new ConcreteWholeSubmeshInPoseDataBlock(
                transform: new ObjectTransform(
                    position: ExportVector3.FromVector3(child.position),
                    rotation: ExportQuaternion.FromQuaternion(child.rotation),
                    scale: ExportVector3.FromVector3(child.lossyScale)
                    ),
                vertices: mesh.vertices.Select(x => new ExportVector3(x.x, x.y, x.z)).ToList(),
                triangles: mesh.triangles.ToList()
            );
        }
    }
}
