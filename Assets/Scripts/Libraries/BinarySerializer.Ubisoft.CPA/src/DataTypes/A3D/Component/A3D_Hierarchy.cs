namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Hierarchy : BinarySerializable {
		public ushort Child { get; set; }
		public ushort Parent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Child = s.Serialize<ushort>(Child, name: nameof(Child));
			Parent = s.Serialize<ushort>(Parent, name: nameof(Parent));
		}

		public override string ShortLog => $"Hierarchy(Child: {Child}, Parent: {Parent})";
		public override bool UseShortLog => true;
	}
}