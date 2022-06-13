namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_PaletteRGBA16 : U64_Struct {
		public int Length { get; set; } = 256;

		public BaseColor[] Palette { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			switch (s.GetCPASettings().GetEndian) {
				case Endian.Little:
					Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, Length, name: nameof(Palette));
					break;
				case Endian.Big:
					Palette = s.SerializeObjectArray<ABGR1555Color>((ABGR1555Color[])Palette, Length, name: nameof(Palette));
					break;
			}
		}
	}

}
