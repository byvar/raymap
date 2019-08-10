using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UntoldUnpack.Graphics {
	public static class Texture {
		public static Texture2D ToTexture2D(PicaDataTypes dataType, PicaPixelFormats pixelFormat, int width, int height, Stream inputStream) {
			BinaryReader reader = new BinaryReader(inputStream);

			return ToTexture2D(dataType, pixelFormat, width, height, reader);
		}

		public static Texture2D ToTexture2D(PicaDataTypes dataType, PicaPixelFormats pixelFormat, int width, int height, byte[] data) {
			using (MemoryStream inputStream = new MemoryStream(data)) {
				return ToTexture2D(dataType, pixelFormat, width, height, new BinaryReader(inputStream));
			}
		}

		public static Texture2D ToTexture2D(PicaDataTypes dataType, PicaPixelFormats pixelFormat, int width, int height, BinaryReader reader) {
			TileDecoderDelegate decoder = TileCodecs.GetDecoder(dataType, pixelFormat);
			Texture2D tex = new Texture2D(width, height, TextureFormat.BGRA32, false);

			byte[] targetData = new byte[height * width * 4];

			for (int y = 0; y < height; y += 8)
				for (int x = 0; x < width; x += 8)
					decoder(reader, targetData, x, y, (int)width, (int)height);
			
			tex.LoadRawTextureData(targetData);
			tex.Apply();

			return tex;
		}
	}
}
