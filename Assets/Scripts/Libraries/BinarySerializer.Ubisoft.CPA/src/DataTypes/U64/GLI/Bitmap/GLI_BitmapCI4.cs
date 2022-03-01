namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BitmapCI4 : GLI_Bitmap {
		public byte[] Bitmap { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Bitmap = s.SerializeArray<byte>(Bitmap, (Length / 2) + (Length % 2), name: nameof(Bitmap));
		}
	}

}
