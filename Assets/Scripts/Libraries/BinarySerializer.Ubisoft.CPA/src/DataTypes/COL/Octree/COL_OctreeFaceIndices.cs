namespace BinarySerializer.Ubisoft.CPA {
	public class COL_OctreeFaceIndices : BinarySerializable {
		public ushort ElementsCount { get; set; }
		public COL_OctreeFaceIndex[] FaceIndices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
			FaceIndices = s.SerializeObjectArray<COL_OctreeFaceIndex>(FaceIndices, ElementsCount, name: nameof(FaceIndices));
		}
	}
}
