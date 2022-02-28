namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_BinInfo : BinarySerializable {
		public float CoordinateScale { get; set; }
		public ushort StructsCount { get; set; }
		public ushort Version { get; set; }
		public ushort LevelsCount { get; set; }
		public ushort VertexSegment { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CoordinateScale = s.Serialize<float>(CoordinateScale, name: nameof(CoordinateScale));
			StructsCount = s.Serialize<ushort>(StructsCount, name: nameof(StructsCount));
			Version = s.Serialize<ushort>(Version, name: nameof(Version));
			LevelsCount = s.Serialize<ushort>(LevelsCount, name: nameof(LevelsCount));
			VertexSegment = s.Serialize<ushort>(VertexSegment, name: nameof(VertexSegment));
		}
	}
}