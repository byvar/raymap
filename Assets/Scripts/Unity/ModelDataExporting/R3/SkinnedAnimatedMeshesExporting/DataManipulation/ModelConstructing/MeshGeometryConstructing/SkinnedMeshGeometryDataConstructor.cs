using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.MathDescription;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing
{
    public class MeshDataHelper
    {
        public static List<Tuple<int, int, int>> GetTrianglesList(int[] triangles)
        {
            var result = new List<Tuple<int, int, int>>();
            for (int i = 0; i < triangles.Length / 3; i++)
            {
                result.Add(
                    new Tuple<int, int, int>(triangles[i * 3], triangles[i * 3 + 1], triangles[i * 3 + 2])
                    );
            }
            return result;
        }

        public static List<Vector3d> GetNormals(Vector3[] normals)
        {
            return normals.Select(x => new Vector3d(x.x, x.y, x.z)).ToList();
        }

        public static List<List<Vector2d>> GetUvMaps(Mesh mesh)
        {
            var result = new List<List<Vector2d>>();
            result.Add(mesh.uv.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv2.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv3.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv4.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv5.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv6.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv7.Select(x => new Vector2d(x.x, x.y)).ToList());
            result.Add(mesh.uv8.Select(x => new Vector2d(x.x, x.y)).ToList());
            return result;
        }
    }

    public class SkinnedMeshGeometryDataConstructor
    {
        BonesWeightsHelper boneWeightsHelper = new BonesWeightsHelper();

        public MeshGeometry ConstructFrom(Mesh mesh, Transform[] bones)
        {
            var result = new MeshGeometry();
            List<Vector3d> verticesList = GetVerticesList(mesh.vertices);
            List<Tuple<int, int, int>> trianglesList = MeshDataHelper.GetTrianglesList(mesh.triangles);
            Dictionary<string, Dictionary<int, float>> bonesWeights = GetBonesWeights(mesh.boneWeights, bones);
            result.vertices = verticesList;
            result.triangles = trianglesList;
            result.bonesWeights = bonesWeights;
            return result;
        }

        private Dictionary<string, Dictionary<int, float>> GetBonesWeights(BoneWeight[] boneWeights, Transform[] bones)
        {
            var result = new Dictionary<string, Dictionary<int, float>>();
            foreach (var weights in boneWeightsHelper.IterateBonesWeights(boneWeights, bones))
            {
                result.Add(weights.BoneName, weights.Weights);
            }
            return result;
        }

        private List<Vector3d> GetVerticesList(Vector3[] vertices)
        {
            return vertices.Select(x => new Vector3d(x.x, x.y, x.z)).ToList();
        }
    }
}
