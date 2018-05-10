namespace LibGC.Texture
{
    class GcTexturePaletteFormatCodecRGB565 : GcTexturePaletteFormatCodec
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
