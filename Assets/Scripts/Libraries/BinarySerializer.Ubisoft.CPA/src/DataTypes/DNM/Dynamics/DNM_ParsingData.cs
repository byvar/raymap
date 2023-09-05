namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_ParsingData : BinarySerializable {
		public MTH3D_Vector Position { get; set; }
		public float OutAlpha { get; set; } // LastAngleMoveCam
		public MTH3D_Vector Vector { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Position = s.SerializeObject<MTH3D_Vector>(Position, name: nameof(Position));
			OutAlpha = s.Serialize<float>(OutAlpha, name: nameof(OutAlpha));
			Vector = s.SerializeObject<MTH3D_Vector>(Vector, name: nameof(Vector));
		}
	}
}
