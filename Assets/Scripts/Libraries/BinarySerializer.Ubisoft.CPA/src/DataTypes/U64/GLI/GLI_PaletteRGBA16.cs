namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_PaletteRGBA16 : U64_Struct {
		public int Length { get; set; } = 256;

		public RGBA5551Color[] Palette { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, Length, name: nameof(Palette));
		}
	}

}
