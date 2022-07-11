namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_IndexedAlignedBox : BinarySerializable {
		public ushort MinPoint { get; set; }
		public ushort MaxPoint { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MinPoint = s.Serialize<ushort>(MinPoint, name: nameof(MinPoint));
			MaxPoint = s.Serialize<ushort>(MaxPoint, name: nameof(MaxPoint));
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
		}
	}
}
