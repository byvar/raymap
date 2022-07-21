namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_2DVertex : BinarySerializable {
		public float X { get; set; }
		public float Y { get; set; }
		public float OoZ { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			OoZ = s.Serialize<float>(OoZ, name: nameof(OoZ));
		}
	}
}
