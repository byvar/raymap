namespace BinarySerializer.Nintendo.DS {
	public class DS3D_Command_TexCoord : DS3D_CommandData {
		// S, T texture coordinates in Texels. To get UVs, divide by texture width or 64
		public FixedPointInt16 S { get; set; }
		public FixedPointInt16 T { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            S = s.SerializeObject<FixedPointInt16>(S, onPreSerialize: u => u.Pre_PointPosition = 4, name: nameof(S));
            T = s.SerializeObject<FixedPointInt16>(T, onPreSerialize: u => u.Pre_PointPosition = 4, name: nameof(T));
        }
		public override bool UseShortLog => true;
		public override string ToString() => $"{GetType()}({S.ShortLog}, {T.ShortLog})";
	}
}
