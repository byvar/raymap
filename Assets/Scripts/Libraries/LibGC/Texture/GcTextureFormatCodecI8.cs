namespace LibGC.Texture
{
    class GcTextureFormatCodecI8 : GcTextureFormatCodec
    {
        public override int TileWidth
        {
            get { return 8; }
        }

        public override int TileHeight
        {
            get { return 4; }
        }

        public override int BitsPerPixel
        {
            get { return 8; }
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
                    ColorConversion.I8ToColor(src[srcCurrentPos]).Write(dst, dstCurrentPos);
                    srcCurrentPos++;
                }
            }
        }

        protected override bool EncodingWantsGrayscale
        {
            get { return true; }
        }

        protected override bool EncodingWantsDithering
        {
            get { return true; }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            return ColorConversion.I8ToColor(ColorConversion.ColorToI8(color));
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            int srcCurrentPos = srcPos, dstCurrentPos = dstPos;
            for (int ty = 0; ty < TileHeight; ty++, srcCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, srcCurrentPos += 4)
                {
                    dst[dstCurrentPos] = ColorConversion.ColorToI8(ColorRGBA.Read(src, srcCurrentPos));
                    dstCurrentPos++;
                }
            }
        }
    }
}
