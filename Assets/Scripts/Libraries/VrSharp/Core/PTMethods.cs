using System;
using System.Collections.Generic;

namespace VrSharp
{
    public static class PTMethods
    {
        /// <summary>
        /// Checks to see if the array contains the values stored in compareTo at the specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Array to check</param>
        /// <param name="sourceIndex">Position in the array to check.</param>
        /// <param name="compareTo">The array to compare to.</param>
        /// <returns>True if the values in compareTo are in the array at the specified index.</returns>
        public static bool Contains<T>(T[] source, int sourceIndex, T[] compareTo)
        {
            if (source == null || compareTo == null)
                return false;

            if (sourceIndex < 0 || sourceIndex + compareTo.Length > source.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (!comparer.Equals(source[sourceIndex + i], compareTo[i])) return false;
            }

            return true;
        }

		public static byte[] GetBytesInEndian(byte[] array, int position, int length, bool isLittleEndian) {
			byte[] data = new byte[length];
			Array.Copy(array, position, data, 0, length);
			if (isLittleEndian != BitConverter.IsLittleEndian) Array.Reverse(data);
			return data;
		}

		public static uint ToUInt32(byte[] array, int position, bool isLittleEndian) {
			return BitConverter.ToUInt32(GetBytesInEndian(array, position, 4, isLittleEndian), 0);
		}
		public static int ToInt32(byte[] array, int position, bool isLittleEndian) {
			return BitConverter.ToInt32(GetBytesInEndian(array, position, 4, isLittleEndian), 0);
		}
		public static ushort ToUInt16(byte[] array, int position, bool isLittleEndian) {
			return BitConverter.ToUInt16(GetBytesInEndian(array, position, 2, isLittleEndian), 0);
		}
		public static short ToInt16(byte[] array, int position, bool isLittleEndian) {
			return BitConverter.ToInt16(GetBytesInEndian(array, position, 2, isLittleEndian), 0);
		}
	}
}