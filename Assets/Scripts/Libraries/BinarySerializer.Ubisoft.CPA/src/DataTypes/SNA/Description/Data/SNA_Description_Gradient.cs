namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Gradient : SNA_Description_Data {
		public GLI_FloatColor UpLeft { get; set; }
		public GLI_FloatColor UpRight { get; set; }
		public GLI_FloatColor DownLeft { get; set; }
		public GLI_FloatColor DownRight { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UpLeft = s.SerializeObject<GLI_FloatColor>(UpLeft, name: nameof(UpLeft));
			UpRight = s.SerializeObject<GLI_FloatColor>(UpRight, name: nameof(UpRight));
			DownLeft = s.SerializeObject<GLI_FloatColor>(DownLeft, name: nameof(DownLeft));
			DownRight = s.SerializeObject<GLI_FloatColor>(DownRight, name: nameof(DownRight));
		}
	}
}
