
using System;

namespace LibGC.Texture
{
    /// <summary>
    /// Utilities for converting between different color formats.
    /// </summary>
    static class ColorConversion
    {
        /* !! LOOKUP TABLES FOR COLOR DEPTH CONVERSION
         *
         * Those tables contain the best (nearest) value for converting
         * a N-bit color component to a M-bit color component.
         * 
         * That is, for example, for 5 bits, the values from 0 to 31 represent the
         * range of values from 0.0 (minimum) to 1.0 (maximum), while in 8 bits,
         * the values 0 to 255 represent the values from 0.0 (minimum) to 1.0 (maximum).
         * Those tables provide the scaling values from one range to the other.
         */
        private static readonly byte[] Bits3To8;
        private static readonly byte[] Bits8To3;

        private static readonly byte[] Bits4To8;
        private static readonly byte[] Bits8To4;

        private static readonly byte[] Bits5To8;
        private static readonly byte[] Bits8To5;

        private static readonly byte[] Bits6To8;
        private static readonly byte[] Bits8To6;

        /// <summary>Create the depth conversion table from values of 'srcBits' bits to values of 'dstBits' bits.</summary>
        /// <param name="srcBits">The number of bits of the source values.</param>
        /// <param name="dstBits">The number of bits of the destination values.</param>
        /// <returns></returns>
        static byte[] MakeDepthConversionTable(int srcBits, int dstBits)
        {
	        int srcEntries = 1 << srcBits;
	        int dstEntries = 1 << dstBits;

            // There are ways to compute those tables without using floating point math
            // (by "shifting" bit strings), but they are a bit more complicated to understand
            byte[] result = new byte[srcEntries];
            for (int i = 0; i < srcEntries; i ++)
            {
                // In the original values, (srcEntries - 1) is the maximum value in the source range,
                // while (dstEntries - 1) is the maximum value in the destination range
                result[i] = (byte) Math.Round((double) i / (srcEntries - 1) * (dstEntries - 1));
            }
            return result;
        }

        static ColorConversion()
        {
            Bits3To8 = MakeDepthConversionTable(3, 8);
            Bits8To3 = MakeDepthConversionTable(8, 3);

            Bits4To8 = MakeDepthConversionTable(4, 8);
            Bits8To4 = MakeDepthConversionTable(8, 4);

            Bits5To8 = MakeDepthConversionTable(5, 8);
            Bits8To5 = MakeDepthConversionTable(8, 5);

            Bits6To8 = MakeDepthConversionTable(6, 8);
            Bits8To6 = MakeDepthConversionTable(8, 6);
            
        }

        public static ColorRGBA I4ToColor(byte v)
        {
            return ColorRGBA.FromIntensityAlpha(Bits4To8[v], 0xFF);
        }

        public static byte ColorToI4(ColorRGBA c)
        {
            return Bits8To4[c.Intensity()];
        }

        public static ColorRGBA IA4ToColor(byte v)
        {
            return ColorRGBA.FromIntensityAlpha(Bits4To8[v & 0x0F], Bits4To8[v >> 4]);
        }

        public static byte ColorToIA4(ColorRGBA c)
        {
            return (byte)(Bits8To4[c.Intensity()] | (Bits8To4[c.Alpha] << 4));
        }

        public static ColorRGBA I8ToColor(byte v)
        {
            return ColorRGBA.FromIntensityAlpha(v, 0xFF);
        }

        public static byte ColorToI8(ColorRGBA c)
        {
            return c.Intensity();
        }

        public static ColorRGBA RGB565ToColor(ushort v)
        {
            return new ColorRGBA(
                Bits5To8[(v >> 11) & 0x1F],
                Bits6To8[(v >> 5) & 0x3F],
                Bits5To8[v & 0x1F],
                0xFF
            );
        }

        public static ushort ColorToRGB565(ColorRGBA c)
        {
            return (ushort)
                ((Bits8To5[c.Red] << 11) |
                (Bits8To6[c.Green] << 5) |
                Bits8To5[c.Blue]);
        }

        public static ColorRGBA RGB5A3ToColor(ushort v)
        {
            if ((v & 0x8000) != 0)
            {
                // RGB555
                return new ColorRGBA(
                    Bits5To8[(v >> 10) & 0x1F],
                    Bits5To8[(v >> 5) & 0x1F],
                    Bits5To8[v & 0x1F],
                    0xFF
                );
            }
            else
            {
                // RGB4A3
                return new ColorRGBA(
                    Bits4To8[(v >> 8) & 0x0F],
                    Bits4To8[(v >> 4) & 0x0F],
                    Bits4To8[v & 0x0F],
                    Bits3To8[(v >> 12) & 0x07]
                );
            }
        }

        public static ushort ColorToRGB5A3(ColorRGBA c)
        {
            if (c.Alpha >= 0xE0)
            {
                // RGB555
                return (ushort)(
                    0x8000 |
                    (Bits8To5[c.Red] << 10) |
                    (Bits8To5[c.Green] << 5) |
                    Bits8To5[c.Blue]);
            }
            else
            {
                // RGB4A3
                return (ushort)(
                    (Bits8To3[c.Alpha] << 12) |
                    (Bits8To4[c.Red] << 8) |
                    (Bits8To4[c.Green] << 4) |
                    Bits8To4[c.Blue]);
            }
        }
    }
}
