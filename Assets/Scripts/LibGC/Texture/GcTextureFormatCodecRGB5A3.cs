namespace LibGC.Texture
{
    class GcTextureFormatCodecRGB5A3 : GcTextureFormatCodec
    {
        public override int TileWidth
        {
            get { return 4; }
        }

        public override int TileHeight
        {
            get { return 4; }
        }

        public override int BitsPerPixel
        {
            get { return 16; }
        }

        public override int PaletteCount
        {
            get { return 0; }
        }

        public override bool IsSupported
        {
            get { return true; }
        }

        protected override void DecodeTile(byte[] dst, int dstPos, int stride, byte[] src, int srcPos)
        {
            int srcCurrentPos = srcPos, dstCurrentPos = dstPos;
            for (int ty = 0; ty < TileHeight; ty++, dstCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, dstCurrentPos += 4)
                {
                    ushort color16 = Utils.ReadBigEndian16(src, srcCurrentPos);
                    ColorConversion.RGB5A3ToColor(color16).Write(dst, dstCurrentPos);
                    srcCurrentPos += 2;
                }
            }
        }

        protected override bool EncodingWantsGrayscale
        {
            get { return false; }
        }

        protected override bool EncodingWantsDithering
        {
            get { return true; }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            return ColorConversion.RGB5A3ToColor(ColorConversion.ColorToRGB5A3(color));
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            int srcCurrentPos = srcPos, dstCurrentPos = dstPos;
            for (int ty = 0; ty < TileHeight; ty++, srcCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, srcCurrentPos += 4)
                {
                    ushort color16 = ColorConversion.ColorToRGB5A3(ColorRGBA.Read(src, srcCurrentPos));
                    Utils.WriteBigEndian16(dst, dstCurrentPos, color16);
                    dstCurrentPos += 2;
                }
            }
        }
    }
}
