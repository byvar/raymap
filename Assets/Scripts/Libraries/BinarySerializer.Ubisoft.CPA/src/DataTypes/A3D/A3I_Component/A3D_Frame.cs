namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Frame : BinarySerializable, ISerializerShortLog {
		public ushort NTTOIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			NTTOIndex = s.Serialize<ushort>(NTTOIndex, name: nameof(NTTOIndex));
		}

		public string ShortLog => ToString();
		public override string ToString() => $"Frame(NTTO: {NTTOIndex})";
	}
}