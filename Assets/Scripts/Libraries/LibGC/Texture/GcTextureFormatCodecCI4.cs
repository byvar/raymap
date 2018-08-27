using System;

namespace LibGC.Texture
{
    class GcTextureFormatCodecCI4 : GcTextureFormatCodec
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
            get { return 1 << 4; }
        }

        public override bool IsSupported
        {
            get { return false; }
        }

        protected override void DecodeTile(byte[] dst, int dstPos, int stride, byte[] src, int srcPos)
        {
            throw new NotImplementedException();
        }

        protected override bool EncodingWantsGrayscale
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool EncodingWantsDithering
        {
            get { throw new NotImplementedException(); }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            throw new NotImplementedException();
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            throw new NotImplementedException();
        }
    }
}
