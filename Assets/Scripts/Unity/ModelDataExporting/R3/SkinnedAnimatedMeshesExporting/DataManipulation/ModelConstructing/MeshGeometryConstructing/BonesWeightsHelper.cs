using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Unity.ModelDataExporting.R3.SkinnedAnimatedMeshesExporting.DataManipulation.ModelConstructing.MeshGeometryConstructing
{
    public class BonesWeightsHelper
    {
        public IEnumerable<BoneWeights> IterateBonesWeights(BoneWeight[] boneWeights, Transform[] bones)
        {
            List<BoneWeightsInfo> BoneWeightsInfos = GetBoneWeightsInfos(boneWeights, bones);
            foreach (var BoneWeightsInfo in BoneWeightsInfos)
            {
                yield return GetBoneWeightsFor(BoneWeightsInfo.BoneName, BoneWeightsInfo.ChannelName,
                    BoneWeightsInfo.BoneIndex, boneWeights);
            }
        }

        private List<BoneWeightsInfo> GetBoneWeightsInfos(BoneWeight[] boneWeights, Transform[] bones)
        {
            throw new NotImplementedException();
        }

        private BoneWeights GetBoneWeightsFor(string BoneName, string ChannelName, int BoneIndex, BoneWeight[] boneWeights)
        {
            var result = new BoneWeights();
            result.BoneName = ChannelName;
            for (int vertexIndex = 0; vertexIndex < boneWeights.Length; vertexIndex++)
            {
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
}
