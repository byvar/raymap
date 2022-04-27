namespace BinarySerializer.Nintendo.N64 {
	public class RSP_Vertex : BinarySerializable {
		// Position
		public short X { get; set; }
		public short Y { get; set; }
		public short Z { get; set; }

		public ushort Flag { get; set; } // No meaning
		
		// Texture coordinates
		public short U { get; set; }
		public short V { get; set; }

		// Union
		public RGBA8888Color Color { get; set; }
		public sbyte NormalX { get; set; }
		public sbyte NormalY { get; set; }
		public sbyte NormalZ { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<short>(X, name: nameof(X));
			Y = s.Serialize<short>(Y, name: nameof(Y));
			Z = s.Serialize<short>(Z, name: nameof(Z));

			U = s.Serialize<short>(U, name: nameof(U));
			V = s.Serialize<short>(V, name: nameof(V));

			s.DoAt(s.CurrentPointer, () => {
				NormalX = s.Serialize<sbyte>(NormalX, name: nameof(NormalX));
				NormalY = s.Serialize<sbyte>(NormalY, name: nameof(NormalY));
				NormalZ = s.Serialize<sbyte>(NormalZ, name: nameof(NormalZ));
			});
			Color = s.SerializeObject<RGBA8888Color>(Color, name: nameof(Color));
		}
		public override bool UseShortLog => true;
		public override string ToString() => $"Vertex(Pos({X}, {Y}, {Z}), UV({U}, {V}), Normal({NormalX}, {NormalY}, {NormalZ}, Color({Color}))";
	}
}
