using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.MathDescription;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing
{
    public class SkinnedMeshGeometryDataConstructor
    {
        BonesWeightsHelper boneWeightsHelper = new BonesWeightsHelper();

        public MeshGeometry ConstructFrom(Mesh mesh, Transform[] bones)
        {
            var result = new MeshGeometry();
            List<Vector3d> verticesList = GetVerticesList(mesh.vertices);
            List<Tuple<int, int, int>> trianglesList = GetTrianglesList(mesh.triangles);
            Dictionary<string, Dictionary<int, float>> bonesWeights = GetBonesWeights(mesh.boneWeights, bones);
            result.vertices = verticesList;
            result.triangles = trianglesList;
            result.bonesWeights = bonesWeights;
            return result;
        }

        private List<Tuple<int, int, int>> GetTrianglesList(int[] triangles)
        {
            throw new NotImplementedException();
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
