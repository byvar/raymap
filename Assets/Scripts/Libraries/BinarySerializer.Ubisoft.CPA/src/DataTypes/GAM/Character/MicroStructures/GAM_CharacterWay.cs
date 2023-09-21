namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterWay : BinarySerializable {
		public Pointer<WAY_Graph> Path { get; set; }
		public int WPIndex { get; set; }
		public bool IsCircular { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Path = s.SerializePointer<WAY_Graph>(Path, name: nameof(Path))?.ResolveObject(s);
			WPIndex = s.Serialize<int>(WPIndex, name: nameof(WPIndex));
			IsCircular = s.Serialize<bool>(IsCircular, name: nameof(IsCircular));
		}
	}
}
