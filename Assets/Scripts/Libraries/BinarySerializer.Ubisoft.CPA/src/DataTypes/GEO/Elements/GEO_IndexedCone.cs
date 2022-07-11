namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_IndexedCone : BinarySerializable {
		public ushort TopPoint { get; set; }
		public ushort BasePoint { get; set; }
		public float BaseRadius { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TopPoint = s.Serialize<ushort>(TopPoint, name: nameof(TopPoint));
			BasePoint = s.Serialize<ushort>(BasePoint, name: nameof(BasePoint));
			BaseRadius = s.Serialize<float>(BaseRadius, name: nameof(BaseRadius));
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
		}
	}
}
