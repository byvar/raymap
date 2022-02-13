using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class DeformVertexWeights : OpenSpaceStruct {
		public ushort num_weights;
		public ushort ind_vertex;
		public LegacyPointer off_weights;
		public short unk0;
		public short unk1;
		public short unk2;
		public short unk3;

		public DeformWeight[] weights;

		protected override void ReadInternal(Reader reader) {
			num_weights = reader.ReadUInt16();
			ind_vertex = reader.ReadUInt16();
			off_weights = LegacyPointer.Read(reader);

			unk0 = reader.ReadInt16();
			unk1 = reader.ReadInt16();
			unk2 = reader.ReadInt16();
			unk3 = reader.ReadInt16();

			weights = Load.ReadArray<DeformWeight>(num_weights, reader, off_weights);
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
                    List<KeyValuePair<ushort, ushort>> sortedWeights = weights.Select(w => new KeyValuePair<ushort,ushort>((ushort)(w.ind_bone + 1), w.weight)).OrderByDescending(w => w.Value).ToList();
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
                        MapLoader.Loader.print("Unity does not support more than 4 bones affecting a vertex at once.");
                    }
                    unityWeightCalculated = true;
                }
                return unityWeight;
            }
        }
    }
}
