namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_Gradient : SNA_Description_Data {
		public GLI_FloatColor_RGBA UpLeft { get; set; }
		public GLI_FloatColor_RGBA UpRight { get; set; }
		public GLI_FloatColor_RGBA DownLeft { get; set; }
		public GLI_FloatColor_RGBA DownRight { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UpLeft = s.SerializeObject<GLI_FloatColor_RGBA>(UpLeft, name: nameof(UpLeft));
			UpRight = s.SerializeObject<GLI_FloatColor_RGBA>(UpRight, name: nameof(UpRight));
			DownLeft = s.SerializeObject<GLI_FloatColor_RGBA>(DownLeft, name: nameof(DownLeft));
			DownRight = s.SerializeObject<GLI_FloatColor_RGBA>(DownRight, name: nameof(DownRight));
		}
	}
}
