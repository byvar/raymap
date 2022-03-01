namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BitmapRGBA16 : GLI_Bitmap {

		public RGBA5551Color[] Bitmap { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Bitmap = s.SerializeObjectArray<RGBA5551Color>(Bitmap, Length, name: nameof(Bitmap));
		}
	}

}
