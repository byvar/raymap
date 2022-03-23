using ImageMagick;
using UnityEngine;

namespace BinarySerializer.Unity 
{
	public static class TextureHelpers 
	{
		/// <summary>
		/// Creates a new <see cref="Texture2D"/>
		/// </summary>
		/// <param name="width">The texture width</param>
		/// <param name="height">The texture height</param>
		/// <param name="clear">Indicates if the image should start as fully transparent</param>
		/// <param name="applyClear">Indicates if the clear transparent pixels should be applied</param>
		/// <returns>The texture</returns>
		public static Texture2D CreateTexture2D(int width, int height, bool clear = false, bool applyClear = false) {
			var tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};

			if (clear) {
				tex.SetPixels(new Color[width * height]);

				if (applyClear)
					tex.Apply();
			}

			return tex;
		}

		public static MagickImage ToMagickImage(this Texture2D tex) {
			var pixels = tex.GetPixels();
			var bytes = new byte[pixels.Length * 4];

			for (int i = 0; i < pixels.Length; i++) {
				bytes[i * 4 + 0] = (byte)(pixels[i].a * 255);
				bytes[i * 4 + 1] = (byte)(pixels[i].b * 255);
				bytes[i * 4 + 2] = (byte)(pixels[i].g * 255);
				bytes[i * 4 + 3] = (byte)(pixels[i].r * 255);
			}

			var img = new MagickImage(bytes, new PixelReadSettings(tex.width, tex.height, StorageType.Char, PixelMapping.ABGR));
			img.Flip();
			return img;
		}

		public static MagickImage ToMagickImage(this Sprite sprite) {
			var pixels = sprite.texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height);
			var bytes = new byte[pixels.Length * 4];

			for (int i = 0; i < pixels.Length; i++) {
				bytes[i * 4 + 0] = (byte)(pixels[i].a * 255);
				bytes[i * 4 + 1] = (byte)(pixels[i].b * 255);
				bytes[i * 4 + 2] = (byte)(pixels[i].g * 255);
				bytes[i * 4 + 3] = (byte)(pixels[i].r * 255);
			}

			var img = new MagickImage(bytes, new PixelReadSettings((int)sprite.rect.width, (int)sprite.rect.height, StorageType.Char, PixelMapping.ABGR));
			img.Flip();
			return img;
		}

		public static Texture2D Crop(this Texture2D tex, RectInt rect, bool destroyTex, bool flipY = true) {
			var newTex = CreateTexture2D(rect.width, rect.height);

			if (flipY)
				rect.y = tex.height - rect.height - rect.y;

			newTex.SetPixels(tex.GetPixels(rect.x, rect.y, rect.width, rect.height));

			newTex.Apply();

			if (destroyTex)
				Object.DestroyImmediate(tex);

			return newTex;
		}

		public static void ResizeImageData(this Texture2D texture2D, int targetX, int targetY, bool mipmap = true, FilterMode filter = FilterMode.Bilinear) {
			//create a temporary RenderTexture with the target size
			RenderTexture rt = RenderTexture.GetTemporary(targetX, targetY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

			//set the active RenderTexture to the temporary texture so we can read from it
			RenderTexture.active = rt;

			//Copy the texture data on the GPU - this is where the magic happens [(;]
			Graphics.Blit(texture2D, rt);
			//resize the texture to the target values (this sets the pixel data as undefined)
			texture2D.Resize(targetX, targetY, texture2D.format, mipmap);
			texture2D.filterMode = filter;

			try {
				//reads the pixel values from the temporary RenderTexture onto the resized texture
				texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetX, targetY), 0, 0);
				//actually upload the changed pixels to the graphics card
				texture2D.Apply();
			} catch {
				Debug.LogError("Read/Write is not enabled on texture " + texture2D.name);
			}


			RenderTexture.ReleaseTemporary(rt);
		}

		public static void Export(this Texture2D texture2D, string filePath, bool includesExt = false)
		{
			if (!includesExt)
				filePath = $"{filePath}.png";

			Util.ByteArrayToFile(filePath, texture2D.EncodeToPNG());
		}
	}
}