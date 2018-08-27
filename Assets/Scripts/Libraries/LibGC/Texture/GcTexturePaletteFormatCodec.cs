using System;

namespace LibGC.Texture
{
    /// <summary>
    /// Base class to be implemented by all texture palette formats supported by GcTexture.
    /// </summary>
    abstract class GcTexturePaletteFormatCodec
    {
        /// <summary>Number of bits used by every color of a palette in the specified texture palette format.</summary>
        public abstract int BitsPerPixel { get; }

        /// <summary>true if encoding and decoding from/to this format is supported.</summary>
        public abstract bool IsSupported { get; }

        /// <summary>Get the codec associated to the given texture palette format.</summary>
        /// <param name="palFmt">The texture palette format.</param>
        /// <returns>The codec associated to the given texture palette format.</returns>
        public static GcTexturePaletteFormatCodec GetCodec(GcTexturePaletteFormat palFmt)
        {
            switch (palFmt)
            {
                case GcTexturePaletteFormat.IA8: return new GcTexturePaletteFormatCodecIA8();
                case GcTexturePaletteFormat.RGB565: return new GcTexturePaletteFormatCodecRGB565();
                case GcTexturePaletteFormat.RGB5A3: return new GcTexturePaletteFormatCodecRGB5A3();
                default:
                    throw new ArgumentOutOfRangeException("palFmt");
            }
        }
    }
}
