using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GenericExport.Model.DataBlocks
{
    public static class BytesHasher {
        public static string GetHash(List<byte> data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var hashedData = sha256Hash.ComputeHash(data.ToArray());
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < hashedData.Length; i++)
                {
                    sBuilder.Append(hashedData[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }

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

        public string GetGeometryDataHash()
        {
            var geometryBytes = vertices.SelectMany(x => x.GetBytes()).Concat(triangles.SelectMany(x => BitConverter.GetBytes(x))).ToList();
            return BytesHasher.GetHash(geometryBytes);
        }
    }
}
