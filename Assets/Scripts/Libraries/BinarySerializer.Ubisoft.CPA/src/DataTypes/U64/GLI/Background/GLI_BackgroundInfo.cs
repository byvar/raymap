namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_BackgroundInfo : U64_Struct {
		public uint CompressedSize { get; set; }
		public U64_Reference<GLI_BackgroundCI8> Background { get; set; }
		public ushort PalettesCount { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<GLI_PaletteRGBA16>> Palettes { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CompressedSize = s.Serialize<uint>(CompressedSize, name: nameof(CompressedSize));
			Background = s.SerializeObject<U64_Reference<GLI_BackgroundCI8>>(Background, name: nameof(Background))
				?.Resolve(s, onPreSerialize: (_,b) => b.Pre_Length = CompressedSize);
			PalettesCount = s.Serialize<ushort>(PalettesCount, name: nameof(PalettesCount));
			Palettes = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<GLI_PaletteRGBA16>>>(Palettes, name: nameof(Palettes));

			Palettes?.Resolve(s, PalettesCount);
		}
	}

}
