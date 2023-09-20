namespace BinarySerializer.Ubisoft.CPA {
	public class MGT_MagnetModification : BinarySerializable {
		public ushort ModifiedObjectIndex { get; set; }
		public ushort PointsCount { get; set; }
		public Pointer<HIE_SuperObject> SuperObject { get; set; }
		public Pointer<byte[]> ModifiedPointsBitField { get; set; }
		public Pointer<byte[]> InfluencedPointsBitField { get; set; }
		public Pointer<uint[]> CurrentDuration { get; set; }
		public ushort ActivatedMagnetIndex { get; set; }
		public bool Used { get; set; }

		public int PointBytesCount => (int)System.Math.Ceiling(PointsCount / 8f);

		public override void SerializeImpl(SerializerObject s) {
			ModifiedObjectIndex = s.Serialize<ushort>(ModifiedObjectIndex, name: nameof(ModifiedObjectIndex));
			PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
			SuperObject = s.SerializePointer<HIE_SuperObject>(SuperObject, name: nameof(SuperObject))?.ResolveObject(s);
			ModifiedPointsBitField = s.SerializePointer<byte[]>(ModifiedPointsBitField, name: nameof(ModifiedPointsBitField))?.ResolveArray(s, PointBytesCount);
			InfluencedPointsBitField = s.SerializePointer<byte[]>(InfluencedPointsBitField, name: nameof(InfluencedPointsBitField))?.ResolveArray(s, PointBytesCount);
			CurrentDuration = s.SerializePointer<uint[]>(CurrentDuration, name: nameof(CurrentDuration))?.ResolveArray(s, PointsCount);
			ActivatedMagnetIndex = s.Serialize<ushort>(ActivatedMagnetIndex, name: nameof(ActivatedMagnetIndex));
			Used = s.Serialize<bool>(Used, name: nameof(Used));
		}
	}
}
