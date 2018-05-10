using System;

namespace LibGC.Texture
{
    /**
     * Some quick notes on texture decoding (and encoding).
     *
     * I4, I8, IA4, IA8, RGB565, RGB5A3
     * ---------------------------------
     * Those are simple color encodings, just decode the color and put it on the right pixel.
     *
     * RGBA8
     * -----
     * Each 64 bytes (4x4 tile, 32 bits per pixel) of the source buffer form a tile.
     *
     * However, the first 32 bytes store the alpha-red components,
     * and the last 32 bytes store the green-blue components.
     *
     * So you first have 16 alpha-red byte pairs, disposed as a 4x4 tile,
     * then another 16 green-blue byte pairs, also disposed as a 4x4 tile.
     *
     * CMPR
     * ----
     * This is pretty much like standard DXT1, but changing the bit/byte ordering.
     * See EncodeDxtBlock() for more info.
     */

    /// <summary>
    /// Base class to be implemented by all texture formats supported by GcTexture.
    /// </summary>
    public abstract class GcTextureFormatCodec
    {
        /// <summary>Width of every tile of a texture in the specified texture format.</summary>
        public abstract int TileWidth { get; }

        /// <summary>Height of every tile of a texture in the specified texture format.</summary>
        public abstract int TileHeight { get; }

        /// <summary>Number of bits used by every pixel of a texture in the specified texture format.</summary>
        public abstract int BitsPerPixel { get; }

        /// <summary>Calculate the texture size, in bytes, or a texture with the specified dimensions.</summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>The texture size, in bytes, or a texture with the specified dimensions.</returns>
        public int CalcTextureSize(int width, int height)
        {
            int expandedWidth = (width + TileWidth - 1) & ~(TileWidth - 1);
            int expandedHeight = (height + TileHeight - 1) & ~(TileHeight - 1);
            return (expandedWidth * expandedHeight * BitsPerPixel) / 8;
        }

        /// <summary>Number of palette entries used by this texture format. Is not palettized, returns 0.</summary>
        public abstract int PaletteCount { get; }

        /// <summary>true if encoding and decoding from/to this format is supported.</summary>
        public abstract bool IsSupported { get; }

        /// <summary>Decode a tile encoded in this current texture format.</summary>
        /// <param name="src">Source buffer, containing the data encoded in this texture format.</param>
        /// <param name="srcPos">Initial position in the source buffer.</param>
        /// <param name="dst">Destination buffer, which will contain the data encoded as RGBA pixels.</param>
        /// <param name="dstPos">Initial position in the destination buffer.</param>
        /// <param name="stride">Number of bytes between each row of the destination buffer.</param>
        protected abstract void DecodeTile(byte[] dst, int dstPos, int stride, byte[] src, int srcPos);

        /// <summary>Decodes a texture.</summary>
        /// <param name="dst">The destination buffer (RGBA pixels).</param>
        /// <param name="dstPos">Initial position on the destination buffer.</param>
        /// <param name="width">The width of the texture to decode.</param>
        /// <param name="height">The height of the texture to decode.</param>
        /// <param name="stride">The distance, in bytes, between rows of the destination buffer.</param>
        /// <param name="src">The source buffer of texture data.</param>
        /// <param name="srcPos">Initial position on the source buffer.</param>
        /// <param name="srcPal">Source palette buffer (RGBA colors).</param>
        /// <param name="srcPalPos">Initial position on the source palette buffer.</param>
        public void DecodeTexture(byte[] dst, int dstPos, int width, int height, int stride,
            byte[] src, int srcPos, byte[] srcPal, int srcPalPos)
        {
            // Check that the arguments to the method are valid
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (dstPos < 0)
                throw new ArgumentOutOfRangeException("dstPos");
            if (dstPos + height * stride > dst.Length) // Image doesn't fit in destination buffer
                throw new ArgumentException("dst");
            if (width < 0)
                throw new ArgumentOutOfRangeException("width");
            if (height < 0)
                throw new ArgumentOutOfRangeException("height");
            if (stride < 0)
                throw new ArgumentOutOfRangeException("stride");
            if (stride < width * 4)
                throw new ArgumentOutOfRangeException("stride", "Stride is too small to contain a row of data.");
            if (src == null)
                throw new ArgumentNullException("src");
            if (srcPos < 0)
                throw new ArgumentOutOfRangeException("srcPos");

            if (srcPal != null || srcPalPos != 0) // Palettes not supported yet
                throw new NotImplementedException();

            // Get the properties of eeach tile of the image
            int tileWidth = TileWidth, tileHeight = TileHeight;
            int tileSize = (TileWidth * TileHeight * BitsPerPixel) / 8;

            // Decode each tile in the input buffer
            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < width; x += tileWidth)
                {
                    if (x + tileWidth <= width && y + tileHeight <= height)
                    {
                        // Tile is contained completely in the destination buffer
                        // We can do the decoding directly to the destination buffer
                        DecodeTile(dst, dstPos + x * 4 + y * stride, stride, src, srcPos);
                    }
                    else
                    {
                        // Tile is not completely contained in the destination buffer
                        // We have to decode the tile to a temporary array,
                        // then copy the contained part in the destination buffer
                        byte[] tileTmp = new byte[tileWidth * tileHeight * 4];
                        DecodeTile(tileTmp, 0, tileWidth * 4, src, srcPos);
                        for (int ty = 0; ty < Math.Min(height - y, tileHeight); ty++)
                        {
                            Array.Copy(tileTmp, ty * tileWidth * 4,
                                dst, dstPos + x * 4 + (y + ty) * stride, Math.Min(width - x, tileWidth) * 4);
                        }
                    }
                    srcPos += tileSize;
                }
            }
        }

        /// <summary>true if, before encoding each tile, the encoder wants the image to be converted to grayscale.</summary>
        protected abstract bool EncodingWantsGrayscale { get; }

        /// <summary>true if, before encoding each tile, the encoder wants the image to be dithered.</summary>
        protected abstract bool EncodingWantsDithering { get; }

        /// <summary>
        /// Returns the nearest approximation for the given ColorRGBA in the current pixel format.
        /// Only needs to be implemented if EncodingWantsDithering is true.
        /// </summary>
        /// <param name="color">The color to trim.</param>
        /// <returns>The trimmed color.</returns>
        protected abstract ColorRGBA TrimColor(ColorRGBA color);

        /// <summary>
        /// Dither an image using Floyd-Steinberg's dither.
        /// </summary>
        /// <param name="src">The image buffer (RGBA pixels).</param>
        /// <param name="srcPos">Initial position on the source buffer.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="stride">The distance, in bytes, between rows of the image buffer.</param>
        private void DitherImage(byte[] src, int srcPos, int width, int height, int stride)
        {
            int srcCurrentPos = srcPos;
            for (int y = 0; y < height; y++, srcCurrentPos += stride - width * 4)
            {
                for (int x = 0; x < width; x++, srcCurrentPos += 4)
                {
                    ColorRGBA pixel = ColorRGBA.Read(src, srcCurrentPos);
                    ColorRGBA newPixel = TrimColor(pixel);
                    newPixel.Write(src, srcCurrentPos);

                    int errorR = pixel.Red - newPixel.Red;
                    int errorG = pixel.Green - newPixel.Green;
                    int errorB = pixel.Blue - newPixel.Blue;
                    int errorA = pixel.Alpha - newPixel.Alpha;

                    // Distributes error to pixel at (deltaX, deltaY) of current pixel, using the specified weight.

                    /* Floyd-Steinberg dither.
                     * Distributes the error at pixel X to neighboring pixels using those weights:
                     *       -1  0  1
                     *      /--------
                     *   -1 | 0  0  0
                     *    0 | 0  X  7
                     *    1 | 3  5  1
                     */
                    DistributeDitheringError(src, srcCurrentPos, width, height, stride, x, y, 1, 0, 7.0f/16.0f, errorR, errorG, errorB, errorA);
                    DistributeDitheringError(src, srcCurrentPos, width, height, stride, x, y, -1, 1, 3.0f/16.0f, errorR, errorG, errorB, errorA);
                    DistributeDitheringError(src, srcCurrentPos, width, height, stride, x, y, 0, 1, 5.0f/16.0f, errorR, errorG, errorB, errorA);
                    DistributeDitheringError(src, srcCurrentPos, width, height, stride, x, y, 1, 1, 1.0f/16.0f, errorR, errorG, errorB, errorA);
                }
            }
        }

        static void DistributeDitheringError(byte[] src, int srcPos, int width, int height, int stride, int x, int y,
            int deltaX, int deltaY, float weight, int errorR, int errorG, int errorB, int errorA)
        {
            if ((x + deltaX) < width && (y + deltaY) < height)
            {
                ColorRGBA origPx = ColorRGBA.Read(src, srcPos + 4 * deltaX + stride * deltaY);
                ColorRGBA newPx = new ColorRGBA(Utils.Trim8((int)(origPx.Red + errorR * weight)),
                                      Utils.Trim8((int)(origPx.Green + errorG * weight)),
                                      Utils.Trim8((int)(origPx.Blue + errorB * weight)),
                                      Utils.Trim8((int)(origPx.Alpha + errorA * weight)));
                newPx.Write(src, srcPos + 4 * deltaX + stride * deltaY);
            }
        }

        /// <summary>
        /// Convert an image to grayscale.
        /// </summary>
        /// <param name="src">The image buffer (RGBA pixels).</param>
        /// <param name="srcPos">Initial position on the source buffer.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="stride">The distance, in bytes, between rows of the image buffer.</param>
        static void GrayscaleImage(byte[] src, int srcPos, int width, int height, int stride)
        {
            int pos = srcPos;
            for (int y = 0; y < height; y++, pos += stride - width * 4)
            {
                for (int x = 0; x < width; x++, pos += 4)
                {
                    ColorRGBA srcColor = ColorRGBA.Read(src, pos);
                    ColorRGBA grayscaleColor = ColorRGBA.FromIntensityAlpha(srcColor.Intensity(), srcColor.Alpha);
                    grayscaleColor.Write(src, pos);
                }
            }
        }

        /// <summary>Encode a tile in this current texture format.</summary>
        /// <param name="src">Source buffer, containing the RGBA pixel data.</param>
        /// <param name="srcPos">Initial position in the source buffer.</param>
        /// <param name="stride">Number of bytes between each row of the source buffer.</param>
        /// <param name="dst">Destination buffer, which will contain the data encoded as this pixel format.</param>
        /// <param name="dstPos">Initial position in the destination buffer.</param>
        protected abstract void EncodeTile(byte[] src, int srcPos, int stride, byte[] dst, int dstPos);

        /// <summary>Encodes a texture.</summary>
        /// <param name="src">The source buffer (RGBA pixels).</param>
        /// <param name="srcPos">Initial position on the source buffer.</param>
        /// <param name="width">The width of the texture to decode.</param>
        /// <param name="height">The height of the texture to decode.</param>
        /// <param name="stride">The distance, in bytes, between rows of the destination buffer.</param>
        /// <param name="dst">The source buffer of texture data.</param>
        /// <param name="dstPos">Initial position on the source buffer.</param>
        /// <param name="srcPal">Source palette buffer (RGBA colors).</param>
        /// <param name="srcPalPos">Initial position on the source palette buffer.</param>
        public void EncodeTexture(byte[] src, int srcPos, int width, int height, int stride,
            byte[] dst, int dstPos, byte[] srcPal, int srcPalPos)
        {
            // Check that the arguments to the method are valid
            if (src == null)
                throw new ArgumentNullException("dst");
            if (srcPos < 0)
                throw new ArgumentOutOfRangeException("dstPos");
            if (srcPos + height * stride > src.Length) // Image doesn't fit in source buffer
                throw new ArgumentOutOfRangeException("src", "src is too small to contain an image with the given parameters.");
            if (width < 0)
                throw new ArgumentOutOfRangeException("width");
            if (height < 0)
                throw new ArgumentOutOfRangeException("height");
            if (stride < 0)
                throw new ArgumentOutOfRangeException("stride");
            if (stride < width * 4)
                throw new ArgumentOutOfRangeException("stride", "Stride is too small to contain a row of input data.");
            if (dst == null)
                throw new ArgumentNullException("dst");
            if (dstPos < 0)
                throw new ArgumentOutOfRangeException("dstPos");

            if (srcPal != null || srcPalPos != 0) // Palettes not supported yet
                throw new NotImplementedException();

            // Get the properties of eeach tile of the image
            int tileWidth = TileWidth, tileHeight = TileHeight;
            int tileSize = (TileWidth * TileHeight * BitsPerPixel) / 8;

            // Preprocess the image (grayscaling and/or dithering)
            byte[] processedSrc;
            int processedSrcPos;
            if (EncodingWantsGrayscale || EncodingWantsDithering)
            {
                processedSrc = new byte[height * stride];
                processedSrcPos = 0;
                src.CopyTo(processedSrc, srcPos);

                // Even though grayscaling is not strictly necessary, it improves the dithering quality later
                if (EncodingWantsGrayscale)
                {
                    GrayscaleImage(processedSrc, processedSrcPos, width, height, stride);
                }

                // Image dithering
                if (EncodingWantsDithering)
                {
                    DitherImage(processedSrc, processedSrcPos, width, height, stride);
                }
            }
            else
            {
                processedSrc = src;
                processedSrcPos = srcPos;
            }

            // Encode each tile in the input buffer
            for (int y = 0; y < height; y += tileHeight)
            {
                for (int x = 0; x < width; x += tileWidth)
                {
                    if (x + tileWidth <= width && y + tileHeight <= height)
                    {
                        // Tile is contained completely in the source buffer
                        // We can do the encoding directly to the destination buffer
                        EncodeTile(processedSrc, processedSrcPos + x * 4 + y * stride, stride, dst, dstPos);
                    }
                    else
                    {
                        // Tile is not completely contained in the source buffer
                        // We have to copy the (not encoded) tile to a temporary array,
                        // then encode this tile to the destination buffer
                        byte[] tileTmp = new byte[tileWidth * tileHeight * 4];
                        for (int ty = 0; ty < Math.Min(height - y, tileHeight); ty++)
                        {
                            Array.Copy(processedSrc, processedSrcPos + x * 4 + (y + ty) * stride,
                                tileTmp, ty * tileWidth * 4, Math.Min(width - x, tileWidth) * 4);
                        }
                        EncodeTile(tileTmp, 0, tileWidth * 4, dst, dstPos);
                    }
                    dstPos += tileSize;
                }
            }
        }

        /// <summary>Get the codec associated to the given texture format.</summary>
        /// <param name="palFmt">The texture format.</param>
        /// <returns>The codec associated to the given texture format.</returns>
        public static GcTextureFormatCodec GetCodec(GcTextureFormat palFmt)
        {
            switch (palFmt)
            {
                case GcTextureFormat.I4: return new GcTextureFormatCodecI4();
                case GcTextureFormat.I8: return new GcTextureFormatCodecI8();
                case GcTextureFormat.IA4: return new GcTextureFormatCodecIA4();
                case GcTextureFormat.IA8: return new GcTextureFormatCodecIA8();
                case GcTextureFormat.RGB565: return new GcTextureFormatCodecRGB565();
                case GcTextureFormat.RGB5A3: return new GcTextureFormatCodecRGB5A3();
                case GcTextureFormat.RGBA8: return new GcTextureFormatCodecRGBA8();
                case GcTextureFormat.CI4: return new GcTextureFormatCodecCI4();
                case GcTextureFormat.CI8: return new GcTextureFormatCodecCI8();
                case GcTextureFormat.CI14X2: return new GcTextureFormatCodecCI14X2();
                case GcTextureFormat.CMPR: return new GcTextureFormatCodecCMPR();
                default:
                    throw new ArgumentOutOfRangeException("palFmt");
            }
        }
    }
}
