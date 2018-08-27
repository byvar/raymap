namespace LibGC.Texture
{
    class GcTexturePaletteFormatCodecIA8 : GcTexturePaletteFormatCodec
    {
        public override int BitsPerPixel
        {
            get { return 16; }
        }

        public override bool IsSupported
        {
            get { return false; }
        }
    }
}
