namespace LibGC.Texture
{
    /// <summary>
    /// A lightweight structure for representing a color as its 8-bit color components.
    /// </summary>
    public struct ColorRGBA
    {
        /// <summary>The red component (0-255) of the color.</summary>
        public byte Red;
        /// <summary>The green component (0-255) of the color.</summary>
        public byte Green;
        /// <summary>The blue component (0-255) of the color.</summary>
        public byte Blue;
        /// <summary>The alpha component (0-255) of the color.</summary>
        public byte Alpha;

        /// <summary>Create a RGBA color from its color components.</summary>
        /// <param name="red">The red component (0-255) of the color.</param>
        /// <param name="green">The green component (0-255) of the color.</param>
        /// <param name="blue">The blue component (0-255) of the color.</param>
        /// <param name="alpha">The alpha component (0-255) of the color.</param>
        public ColorRGBA(byte red, byte green, byte blue, byte alpha)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
            this.Alpha = alpha;
        }

        /// <summary>Creates a color from its intensity (grayscale) and alpha components.</summary>
        /// <param name="intensity">The intensity component (0-255) of the color.</param>
        /// <param name="alpha">The alpha component (0-255) of the color.</param>
        public static ColorRGBA FromIntensityAlpha(byte intensity, byte alpha)
        {
            return new ColorRGBA(intensity, intensity, intensity, alpha);
        }


        /// <summary>Get the grayscale intensity of this color.</summary>
        /// <returns>The grayscale intensity of this color.</returns>
        public byte Intensity()
        {
            // 30% red, 59% green, 11% blue is the standard way to calculate the intensity of a color
            return (byte)((Red * 0.30) + (Green * 0.59) + (Blue * 0.11));
        }

        /// <summary>
        /// Read a ARGB8 color from a byte array.
        /// </summary>
        /// <param name="src">The source byte array to read the color from.</param>
        /// <param name="srcIndex">The initial position in the source byte array.</param>
        /// <returns>An instance of the color read from the byte array.</returns>
        public static ColorRGBA Read(byte[] src, int srcIndex)
        {
            return new ColorRGBA(src[srcIndex + 0], src[srcIndex + 1], src[srcIndex + 2], src[srcIndex + 3]);
        }

        /// <summary>
        /// Write a ARGB8 color to a byte array.
        /// </summary>
        /// <param name="dst">The destination byte array to write the color to.</param>
        /// <param name="dstIndex">The initial position in the destination byte array.</param>
        public void Write(byte[] dst, int dstIndex)
        {
            dst[dstIndex + 0] = Red;
            dst[dstIndex + 1] = Green;
            dst[dstIndex + 2] = Blue;
            dst[dstIndex + 3] = Alpha;
        }
    };
}
