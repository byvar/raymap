namespace LibGC.Texture
{
    class GcTextureFormatCodecI4 : GcTextureFormatCodec
    {
        public override int TileWidth
        {
            get { return 8; }
        }

        public override int TileHeight
        {
            get { return 8; }
        }

        public override int BitsPerPixel
        {
            get { return 4; }
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
                    if ((tx & 1) == 0)
                    {
                        ColorConversion.I4ToColor((byte)(src[srcCurrentPos] >> 4)).Write(dst, dstCurrentPos);
                    }
                    else if ((tx & 1) == 1)
                    {
                        ColorConversion.I4ToColor((byte)(src[srcCurrentPos] & 0x0F)).Write(dst, dstCurrentPos);
                        srcCurrentPos++;
                    }
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
            return ColorConversion.I4ToColor(ColorConversion.ColorToI4(color));
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            int srcCurrentPos = srcPos, dstCurrentPos = dstPos;
            for (int ty = 0; ty < TileHeight; ty++, srcCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, srcCurrentPos += 4)
                {
                    if ((tx & 1) == 0)
                    {
                        dst[dstCurrentPos] = (byte)(ColorConversion.ColorToI4(ColorRGBA.Read(src, srcCurrentPos)) << 4);
                    }
                    else if ((tx & 1) == 1)
                    {
                        dst[dstCurrentPos] |= ColorConversion.ColorToI4(ColorRGBA.Read(src, srcCurrentPos));
                        dstCurrentPos++;
                    }
                }
            }
        }
    }
}
