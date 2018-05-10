using System;

namespace LibGC.Texture
{
    class GcTextureFormatCodecCMPR : GcTextureFormatCodec
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
            DecodeDxtBlock(dst, dstPos + 0 * 4 + 0 * stride, stride, src, srcPos + 0); // x + 0, y + 0
            DecodeDxtBlock(dst, dstPos + 4 * 4 + 0 * stride, stride, src, srcPos + 8); // x + 4, y + 0
            DecodeDxtBlock(dst, dstPos + 0 * 4 + 4 * stride, stride, src, srcPos + 16); // x + 0, y + 4
            DecodeDxtBlock(dst, dstPos + 4 * 4 + 4 * stride, stride, src, srcPos + 24); // x + 4, y + 4
        }

        protected override bool EncodingWantsGrayscale
        {
            get { return false; }
        }

        protected override bool EncodingWantsDithering
        {
            get { return false; /* Current CMPR encoder does its own dithering */ }
        }

        protected override ColorRGBA TrimColor(ColorRGBA color)
        {
            throw new InvalidOperationException();
        }

        protected override void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            EncodeDxtBlock(src, srcPos + 0 * 4 + 0 * stride, stride, dst, dstPos + 0); // x + 0, y + 0
            EncodeDxtBlock(src, srcPos + 4 * 4 + 0 * stride, stride, dst, dstPos + 8); // x + 4, y + 0
            EncodeDxtBlock(src, srcPos + 0 * 4 + 4 * stride, stride, dst, dstPos + 16); // x + 0, y + 4
            EncodeDxtBlock(src, srcPos + 4 * 4 + 4 * stride, stride, dst, dstPos + 24); // x + 4, y + 4
        }

        /// <summary>
        /// Decode a block/tile/subtile (4x4 pixels) of DXT1/CMPR data.
        /// </summary>
        /// <param name="dst">The destination buffer (RGBA pixels).</param>
        /// <param name="dstIndex">The initial position in the destination buffer.</param>
        /// <param name="stride">The distance, in bytes, between rows of the destination buffer.</param>
        /// <param name="src">The source block of CMPR data (8 bytes).</param>
        /// <param name="srcIndex">The initial position in the source buffer.</param>
        static void DecodeDxtBlock(byte[] dst, int dstIndex, int stride, byte[] src, int srcIndex)
        {
            ushort c1 = Utils.ReadBigEndian16(src, srcIndex);
            srcIndex += 2;
            ushort c2 = Utils.ReadBigEndian16(src, srcIndex);
            srcIndex += 2;

            ColorRGBA color1 = ColorConversion.RGB565ToColor(c1);
            ColorRGBA color2 = ColorConversion.RGB565ToColor(c2);
            byte b1 = color1.Blue;
            byte b2 = color2.Blue;
            byte g1 = color1.Green;
            byte g2 = color2.Green;
            byte r1 = color1.Red;
            byte r2 = color2.Red;

            ColorRGBA[] colors = new ColorRGBA[4];
            colors[0] = new ColorRGBA(r1, g1, b1, 0xFF);
            colors[1] = new ColorRGBA(r2, g2, b2, 0xFF);

            if (c1 > c2)
            {
                byte b3 = (byte)(((b2 - b1) >> 1) - ((b2 - b1) >> 3));
                byte g3 = (byte)(((g2 - g1) >> 1) - ((g2 - g1) >> 3));
                byte r3 = (byte)(((r2 - r1) >> 1) - ((r2 - r1) >> 3));
                colors[2] = new ColorRGBA((byte)(r1 + r3), (byte)(g1 + g3), (byte)(b1 + b3), 0xFF);
                colors[3] = new ColorRGBA((byte)(r2 - r3), (byte)(g2 - g3), (byte)(b2 - b3), 0xFF);
            }
            else
            {
                colors[2] = new ColorRGBA( // Average
                    (byte)((r1 + r2 + 1) / 2),
                    (byte)((g1 + g2 + 1) / 2),
                    (byte)((b1 + b2 + 1) / 2),
                    0xFF
                );
                colors[3] = new ColorRGBA(r2, g2, b2, 0x00);  // Color2 but transparent
            }

            for (int ty = 0; ty < 4; ty++)
            {
                int val = src[srcIndex++];
                for (int tx = 0; tx < 4; tx++)
                {
                    colors[(val >> 6) & 3].Write(dst, dstIndex + 4 * tx + ty * stride);
                    val <<= 2;
                }
            }
        }


        /// <summary>
        /// Convert the "color bits" block of a DXT1/CMPR from the standard format to the Gx format or viceversa.
        /// In DXT1, the color bits are read from the MSB to the LSB of each byte.
        /// In Gx,   the color bits are read from the LSB to the MSB of each byte.
        /// </summary>
        /// <param name="x">The color bits to swap.</param>
        /// <returns>The swapped color bits.</returns>
        static byte SwapCmprColorBits(byte x)
        {
            // AABBCCDD to DDCCBBAA (each character represents 2 bits)
            return (byte)(
                ((x & 0x03) << 6) | // XXXXXXDD -> DDXXXXXX
                ((x & 0x0C) << 2) | // XXXXCCXX -> XXCCXXXX
                ((x & 0x30) >> 2) | // XXBBXXXX -> XXXXBBXX
                ((x & 0xC0) >> 6));  // AAXXXXXX -> XXXXXXAA
        }

        /// <summary>
        /// Encode a block/tile/subtile (4x4 pixels) to DXT1/CMPR data.
        /// </summary>
        /// <param name="src">The source buffer (RGBA pixels).</param>
        /// <param name="srcPos">The initial position in the source buffer.</param>
        /// <param name="stride">The distance, in bytes, between rows of the source buffer.</param>
        /// <param name="dst">The destination buffer (CMPR data - 8 bytes).</param>
        /// <param name="dstPos">The initial position in the destination buffer.</param>
        void EncodeDxtBlock(byte[] src, int srcPos, int stride, byte[] dst, int dstPos)
        {
            // Extract a 4x4 tile from the texture
            byte[] tile = new byte[4 * 4 * 4];
            for (int y = 0, pos = srcPos; y < 4; y++, pos += stride)
                Array.Copy(src, pos, tile, y * 4 * 4, 4 * 4);

            // Compress the DXT block
            StbDxt.CompressDxtBlock(dst, dstPos, tile, 0, false,
                StbDxt.CompressionMode.Dither | StbDxt.CompressionMode.HighQual);

            // Convert from the standard DXT format to the Gx format
            Utils.Swap16(dst, dstPos + 0);
            Utils.Swap16(dst, dstPos + 2);
            for (int i = 0; i < 4; i++)
                dst[dstPos + 4 + i] = SwapCmprColorBits(dst[dstPos + 4 + i]);
        }
    }
}
