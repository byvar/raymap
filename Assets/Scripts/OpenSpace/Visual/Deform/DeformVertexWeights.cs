using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual.Deform {
    public class DeformVertexWeights {
        public ushort vertexIndex;
        public Dictionary<byte, ushort> boneWeights;

        public DeformVertexWeights(ushort vertexIndex) {
            this.vertexIndex = vertexIndex;
            boneWeights = new Dictionary<byte, ushort>();
        }

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
                    List<KeyValuePair<byte, ushort>> sortedWeights = boneWeights.OrderByDescending(w => w.Value).ToList();
                    UInt16 sortedWeightsSum = (UInt16)sortedWeights.Select(w => (Int32)(w.Value)).Sum();
                    if (sortedWeightsSum < UInt16.MaxValue) {
                        sortedWeights.Add(new KeyValuePair<byte, ushort>(0, (ushort)(UInt16.MaxValue - sortedWeightsSum)));
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
                        MapLoader.Loader.print("Unity does not support more than 4 bones affecting a vertex at once.");
                    }
                    unityWeightCalculated = true;
                }
                return unityWeight;
            }
        }

        // Call after clone
        public void Reset() {
            unityWeightCalculated = false;
        }

        public DeformVertexWeights Clone() {
            DeformVertexWeights w = (DeformVertexWeights)MemberwiseClone();
            w.Reset();
            return w;
        }
    }
}
