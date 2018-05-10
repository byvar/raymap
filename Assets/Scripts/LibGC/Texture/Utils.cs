namespace LibGC.Texture
{
    /// <summary>
    /// Miscelaneous utilities used by GcTexture.
    /// </summary>
    static class Utils
    {
        /// <summary>Swaps a 16-bit value from little endian to big endian or vice versa.</summary>
        /// <param name="arr">The byte array that contains the value to swap.</param>
        /// <param name="idx">The initial position in the array that contains the value to swap.</param>
        public static void Swap16(byte[] arr, int idx)
        {
            byte tmp = arr[idx + 1];
            arr[idx+1] = arr[idx];
            arr[idx] = tmp;
        }

        /// <summary>Convert a 16-bit unsigned integer from the host byte order to big endian.</summary>
        /// <param name="dst">The array where the encoded 16-bit unsigned integer will be written.</param>
        /// <param name="dstIndex">The initial position in the destination array.</param>
        /// <param name="value">The 16-bit unsigned integer to write.</param>
        public static void WriteBigEndian16(byte[] dst, int dstIndex, ushort value)
        {
            dst[dstIndex + 0] = (byte)(value >> 8);
            dst[dstIndex + 1] = (byte)value;
        }

        /// <summary>Convert a 16-bit unsigned integer from big endian to the host byte order.</summary>
        /// <param name="src">The array where the encoded 16-bit unsigned integer will be read from.</param>
        /// <param name="srcIndex">The initial position in the source array.</param>
        /// <returns>The decoded 16-bit unsigned integer.</returns>
        public static ushort ReadBigEndian16(byte[] src, int srcIndex)
        {
            return (ushort)((src[srcIndex] << 8) | (src[srcIndex + 1] << 0));
        }

        /// <summary>Convert an integer to a 8-bit unsigned integer, clamping the values out of range to the nearest value.</summary>
        /// <param name="src">The integer to trim.</param>
        /// <returns>The trimmed 8-bit unsigned integer.</returns>
        public static byte Trim8(int src)
        {
            if (src < 0x00)
                return 0;

            if (src > 0xFF)
                return 0xFF;

            return (byte)src;
        }
    }
}
