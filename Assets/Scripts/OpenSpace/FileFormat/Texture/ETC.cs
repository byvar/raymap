using System;
using System.IO;
using UnityEngine;

namespace OpenSpace.FileFormat.Texture {
    public class ETC {
        public int width, height;
		public bool hasAlpha;
		public Texture2D texture;

        public ETC(byte[] bytes, int width, int height, bool hasAlpha) : this(new MemoryStream(bytes), width, height, hasAlpha) { }
        public ETC(string filePath, int width, int height, bool hasAlpha) : this(FileSystem.GetFileReadStream(filePath), width, height, hasAlpha) { }

        public ETC(Stream stream, int width, int height, bool hasAlpha) {
			MapLoader l = MapLoader.Loader;

			this.width = width;
			this.height = height;
			this.hasAlpha = hasAlpha;

			texture = UntoldUnpack.Graphics.Texture.ToTexture2D(
				UntoldUnpack.Graphics.PicaDataTypes.UnsignedByte,
				hasAlpha ? UntoldUnpack.Graphics.PicaPixelFormats.ETC1AlphaRGB8A4NativeDMP : UntoldUnpack.Graphics.PicaPixelFormats.ETC1RGB8NativeDMP,
				width,
				height,
				stream);
		}

        static uint extractBits(int number, int count, int offset) {
            return (uint)(((1 << count) - 1) & (number >> (offset)));
        }
    }
}