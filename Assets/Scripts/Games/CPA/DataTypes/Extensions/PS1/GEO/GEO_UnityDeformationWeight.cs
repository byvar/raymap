using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GEO_UnityDeformationWeight : Unity_Data<GEO_DeformationVertexWeightSet> {
		private bool unityWeightCalculated = false;
		private BoneWeight unityWeight;
		public BoneWeight UnityWeight {
			get {
				if (!unityWeightCalculated) {
					unityWeight = new BoneWeight();
					unityWeight.boneIndex0 = 0;
					unityWeight.boneIndex1 = 0;
					unityWeight.boneIndex2 = 0;
					unityWeight.boneIndex3 = 0;
					unityWeight.weight0 = 0;
					unityWeight.weight1 = 0;
					unityWeight.weight2 = 0;
					unityWeight.weight3 = 0;
					List<KeyValuePair<ushort, ushort>> sortedWeights = LinkedObject.Weights.Select(w => new KeyValuePair<ushort, ushort>((ushort)(w.Bone + 1), w.Weight)).OrderByDescending(w => w.Value).ToList();
					UInt16 sortedWeightsSum = (UInt16)sortedWeights.Select(w => (Int32)(w.Value)).Sum();
					if (sortedWeightsSum < UInt16.MaxValue) {
						sortedWeights.Add(new KeyValuePair<ushort, ushort>(0, (ushort)(UInt16.MaxValue - sortedWeightsSum)));
					}
					sortedWeights = sortedWeights.OrderByDescending(w => w.Value).ToList();
					if (sortedWeights.Count > 0) {
						unityWeight.boneIndex0 = sortedWeights[0].Key;
						unityWeight.weight0 = (float)sortedWeights[0].Value / (float)UInt16.MaxValue;
					}
					if (sortedWeights.Count > 1) {
						unityWeight.boneIndex1 = sortedWeights[1].Key;
						unityWeight.weight1 = (float)sortedWeights[1].Value / (float)UInt16.MaxValue;
					}
					if (sortedWeights.Count > 2) {
						unityWeight.boneIndex2 = sortedWeights[2].Key;
						unityWeight.weight2 = (float)sortedWeights[2].Value / (float)UInt16.MaxValue;
					}
					if (sortedWeights.Count > 3) {
						unityWeight.boneIndex3 = sortedWeights[3].Key;
						unityWeight.weight3 = (float)sortedWeights[3].Value / (float)UInt16.MaxValue;
					}
					if (sortedWeights.Count > 4) {
						UnityEngine.Debug.Log("Unity does not support more than 4 bones affecting a vertex at once.");
					}
					unityWeightCalculated = true;
				}
				return unityWeight;
			}
		}
	}
}
