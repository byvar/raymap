namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class GEO_DeformationVertexWeightSet : BinarySerializable {
		public ushort WeightsCount { get; set; }
		public ushort VertexIndex { get; set; }
		public Pointer WeightsPointer { get; set; }

		public short[] Unknown { get; set; }

		public BoneWeight[] Weights { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			WeightsCount = s.Serialize<ushort>(WeightsCount, name: nameof(WeightsCount));
			VertexIndex = s.Serialize<ushort>(VertexIndex, name: nameof(VertexIndex));
			WeightsPointer = s.SerializePointer(WeightsPointer, allowInvalid: WeightsCount == 0, name: nameof(WeightsPointer));

			Unknown = s.SerializeArray<short>(Unknown, 4, name: nameof(Unknown));

			s.DoAt(WeightsPointer, () =>
				Weights = s.SerializeObjectArray<BoneWeight>(Weights, WeightsCount, name: nameof(Weights)));
		}

		public class BoneWeight : BinarySerializable {
			public ushort Bone { get; set; }
			public ushort Weight { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Bone = s.Serialize<ushort>(Bone, name: nameof(Bone));
				Weight = s.Serialize<ushort>(Weight, name: nameof(Weight));
			}
		}
	}
}