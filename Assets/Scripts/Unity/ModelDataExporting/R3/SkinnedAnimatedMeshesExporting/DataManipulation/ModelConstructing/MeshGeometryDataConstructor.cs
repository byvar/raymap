using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.Model.AnimatedExportObjectModelDescription;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing
{
    public class BoneWeights
    {
        public string BoneName;
        public Dictionary<int, float> Weights;
    }

    public class BoneWeightsInfo
    {
        public string BoneName;
        public string ChannelName;
        public int BoneIndex;
    }

    public class BonesWeightsHelper
    {
        public IEnumerable<BoneWeights> IterateBonesWeights(BoneWeight[] boneWeights)
        {
            List<BoneWeightsInfo> BoneWeightsInfos = GetBoneWeightsInfos(boneWeights);
            foreach (var BoneWeightsInfo in BoneWeightsInfos)
            {
                yield return GetBoneWeightsFor(BoneWeightsInfo.BoneName, BoneWeightsInfo.ChannelName,
                    BoneWeightsInfo.BoneIndex, boneWeights);
            }
        }

        private List<BoneWeightsInfo> GetBoneWeightsInfos(BoneWeight[] boneWeights)
        {
            throw new NotImplementedException();
        }

        private BoneWeights GetBoneWeightsFor(string BoneName, string ChannelName, int BoneIndex, BoneWeight[] boneWeights)
        {
            var result = new BoneWeights();
            result.BoneName = ChannelName;
            for (int vertexIndex = 0; vertexIndex < boneWeights.Length; vertexIndex++) {
                var weight = boneWeights[vertexIndex];
                if (weight.boneIndex0 == BoneIndex)
                {
                    result.Weights.Add(vertexIndex, weight.weight0);
                }
                else if (weight.boneIndex1 == BoneIndex)
                {
                    result.Weights.Add(vertexIndex, weight.weight1);
                }
                else if (weight.boneIndex2 == BoneIndex)
                {
                    result.Weights.Add(vertexIndex, weight.weight2);
                }
                else if (weight.boneIndex3 == BoneIndex)
                {
                    result.Weights.Add(vertexIndex, weight.weight3);
                }
            }
            return result;
        }
    }

    public class MeshGeometryDataConstructor
    {
        BonesWeightsHelper boneWeightsHelper = new BonesWeightsHelper();

        public MeshGeometry ConstructFrom(Mesh mesh)
        {
            var result = new MeshGeometry();
            List<Vector3d> verticesList = GetVerticesList(mesh.vertices);
            List<Tuple<int, int, int>> trianglesList = GetTrianglesList(mesh.triangles);
            Dictionary<string, Dictionary<int, float>> bonesWeights = GetBonesWeights(mesh.boneWeights);
            return result;
        }

        private List<Tuple<int, int, int>> GetTrianglesList(int[] triangles)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, Dictionary<int, float>> GetBonesWeights(BoneWeight[] boneWeights)
        {
            var result = new Dictionary<string, Dictionary<int, float>>();
            foreach (var weights in boneWeightsHelper.IterateBonesWeights(boneWeights))
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
