namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVarList : BinarySerializable {
		public byte ElementsCount { get; set; }
		public byte MaxElementsCount { get; set; }
		public Pointer<HIE_SuperObject>[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<byte>(ElementsCount, name: nameof(ElementsCount));
			MaxElementsCount = s.Serialize<byte>(MaxElementsCount, name: nameof(MaxElementsCount));
			s.Align(4, Offset);
			Elements = s.SerializePointerArray<HIE_SuperObject>(Elements, MaxElementsCount, name: nameof(Elements))
				?.ResolveObject(s);
		}
	}
}
