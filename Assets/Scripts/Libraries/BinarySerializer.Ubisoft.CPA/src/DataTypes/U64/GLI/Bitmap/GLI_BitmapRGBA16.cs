namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BitmapRGBA16 : GLI_Bitmap {

		public BaseColor[] Bitmap { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			switch (s.GetCPASettings().GetEndian) {
				case Endian.Little:
					Bitmap = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Bitmap, Length, name: nameof(Bitmap));
					break;
				case Endian.Big:
					Bitmap = s.SerializeObjectArray<ABGR1555Color>((ABGR1555Color[])Bitmap, Length, name: nameof(Bitmap));
					break;
			}
		}
	}

}
