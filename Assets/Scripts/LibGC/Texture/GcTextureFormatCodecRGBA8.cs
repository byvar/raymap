using System;

namespace LibGC.Texture
{
    class GcTextureFormatCodecRGBA8 : GcTextureFormatCodec
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
            get { return 32; }
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
            int dstCurrentPos = dstPos;
            for (int ty = 0; ty < TileHeight; ty++, dstCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, dstCurrentPos += 4)
                {
                    int srcMv = srcPos + ty * 8 + tx * 2;
                    new ColorRGBA(src[srcMv + 1], src[srcMv + 32], src[srcMv + 33], src[srcMv + 0])
                        .Write(dst, dstCurrentPos);
                }
            }
        }

        protected override bool EncodingWantsGrayscale
        {
            get { return false; }
        }

        protected override bool EncodingWantsDithering
        {
            get { return false; /* No color depth reduction here */ }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            throw new InvalidOperationException();
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            int srcCurrentPos = srcPos;
            for (int ty = 0; ty < TileHeight; ty++, srcCurrentPos += stride - TileWidth * 4)
            {
                for (int tx = 0; tx < TileWidth; tx++, srcCurrentPos += 4)
                {
                    ColorRGBA color = ColorRGBA.Read(src, srcCurrentPos);
                    int dstMv = dstPos + ty * 8 + tx * 2;

                    dst[dstMv + 0] = color.Alpha;
                    dst[dstMv + 1] = color.Red;
                    dst[dstMv + 32] = color.Green;
                    dst[dstMv + 33] = color.Blue;
                }
            }
        }
    }
}
