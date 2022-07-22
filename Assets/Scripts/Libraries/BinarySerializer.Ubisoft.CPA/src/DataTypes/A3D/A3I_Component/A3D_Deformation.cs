namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Deformation : BinarySerializable {
		public ushort Channel1 { get; set; }
		public ushort Frontier1 { get; set; } // Bone
		public ushort Channel2 { get; set; } // Linked channel (controlling Channel1 / controlled by Channel1)
		public ushort Frontier2 { get; set; } // Linked bone

		public override void SerializeImpl(SerializerObject s) {
			Channel1 = s.Serialize<ushort>(Channel1, name: nameof(Channel1));
			Frontier1 = s.Serialize<ushort>(Frontier1, name: nameof(Frontier1));
			Channel2 = s.Serialize<ushort>(Channel2, name: nameof(Channel2));
			Frontier2 = s.Serialize<ushort>(Frontier2, name: nameof(Frontier2));
		}
	}
}