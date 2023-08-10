using BinarySerializer;
using BinarySerializer.PS1;
using System;
using System.Linq;
using BinarySerializer.Unity;
using UnityEngine;

namespace Raymap
{
	public static class PS1Helpers
	{
		public static void FillTexture(
			this VRAM vram,
			Texture2D tex,
			int width, int height,
			TIM.TIM_ColorFormat colorFormat,
			int texX, int texY,
			int clutX, int clutY,
			int texturePageOriginX = 0, int texturePageOriginY = 0,
			int texturePageOffsetX = 0, int texturePageOffsetY = 0,
			int texturePageX = 0, int texturePageY = 0,
			bool flipX = false, bool flipY = false,
			bool useDummyPal = false)
		{
			var dummyPal = useDummyPal ? PaletteHelpers.CreateDummyPalette(colorFormat == TIM.TIM_ColorFormat.BPP_8 ? 256 : 16) : null;

			texturePageOriginX *= 2;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte paletteIndex;

					if (colorFormat == TIM.TIM_ColorFormat.BPP_8)
					{
						paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
							texturePageOriginX + texturePageOffsetX + x,
							texturePageOriginY + texturePageOffsetY + y);
					}
					else if (colorFormat == TIM.TIM_ColorFormat.BPP_4)
					{
						paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
							texturePageOriginX + (texturePageOffsetX + x) / 2,
							texturePageOriginY + texturePageOffsetY + y);

						if (x % 2 == 0)
							paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
						else
							paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
					}
					else
					{
						throw new Exception($"Non-supported color format");
					}

					// Get the color from the palette
					var c = useDummyPal ? dummyPal[paletteIndex] : vram.GetColor1555(0, 0, clutX + paletteIndex, clutY);

					// http://hitmen.c02.at/files/docs/psx/psx.pdf page 31
					if (c.Red == 0 && c.Green == 0 && c.Blue == 0 && c.Alpha == 0)
						continue;

					c.Alpha = 1;

					var texOffsetX = flipX ? width - x - 1 : x;
					var texOffsetY = flipY ? height - y - 1 : y;

					// Set the pixel
					tex.SetPixel(texX + texOffsetX, tex.height - (texY + texOffsetY) - 1, c.GetColor());
				}
			}
		}

		public static Texture2D GetTexture(
			this VRAM vram, 
			int width, int height, 
			TSB tsb, CBA cba, 
			int x, int y, 
			bool flipX = false, bool flipY = false)
		{
			Texture2D tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

			FillTexture(
				vram: vram, 
				tex: tex, 
				width: width, height: height, 
				colorFormat: tsb.TP switch
				{
					TSB.TexturePageTP.CLUT_4Bit => TIM.TIM_ColorFormat.BPP_4,
					TSB.TexturePageTP.CLUT_8Bit => TIM.TIM_ColorFormat.BPP_8,
					//PS1_TSB.TexturePageTP.Direct_15Bit => ,
					_ => throw new ArgumentOutOfRangeException()
				}, 
				texX: 0, texY: 0, 
				clutX: cba.ClutX * 16, clutY: cba.ClutY, 
				texturePageOriginX: 0, texturePageOriginY: 0, 
				texturePageOffsetX: x, texturePageOffsetY: y, 
				texturePageX: tsb.TX, texturePageY: tsb.TY, 
				flipX: flipX, flipY: flipY);

			tex.Apply();

			return tex;
		}

		public static Texture2D FillMapTexture(this VRAM vram, TIM tim, CEL cel, BGD map, Texture2D tex = null)
		{
			tex ??= TextureHelpers.CreateTexture2D(map.MapWidth * map.CellWidth, map.MapHeight * map.CellHeight, clear: true);

			for (int mapY = 0; mapY < map.MapHeight; mapY++)
			{
				for (int mapX = 0; mapX < map.MapWidth; mapX++)
				{
					var cellIndex = map.Map[mapY * map.MapWidth + mapX];

					if (cellIndex == 0xFF)
						continue;

					var cell = cel.Cells[cellIndex];

					if (cell.ABE)
						Debug.LogWarning($"CEL ABE flag is set!");

					vram.FillTexture(
						tex: tex,
						width: map.CellWidth,
						height: map.CellHeight,
						colorFormat: tim.ColorFormat,
						texX: mapX * map.CellWidth,
						texY: mapY * map.CellHeight,
						clutX: cell.ClutX * 16,
						clutY: cell.ClutY,
						texturePageOriginX: tim.Region.XPos,
						texturePageOriginY: tim.Region.YPos,
						texturePageOffsetX: cell.XOffset,
						texturePageOffsetY: cell.YOffset);
				}
			}

			tex.Apply();

			return tex;
		}

		public static Texture2D GetTexture(this TIM tim, bool flipTextureY = true, Color[] palette = null, bool onlyFirstTransparent = false, bool noPal = false)
		{
			if (tim.Region.XPos == 0 && tim.Region.YPos == 0)
				return null;

			var pal = noPal ? null : palette ?? tim.Clut?.Palette?.GetColors().ToArray();

			if (onlyFirstTransparent && pal != null)
				for (int i = 0; i < pal.Length; i++)
					pal[i].a = i == 0 ? 0 : 1;

			return GetTexture(tim.ImgData, pal, tim.Region.Width, tim.Region.Height, tim.ColorFormat, flipTextureY);
		}

		public static Texture2D GetTexture(byte[] imgData, Color[] pal, int width, int height, TIM.TIM_ColorFormat colorFormat, bool flipTextureY = true)
		{
			Util.TileEncoding encoding;

			int palLength;

			switch (colorFormat)
			{
				case TIM.TIM_ColorFormat.BPP_4:
					width *= 2 * 2;
					encoding = Util.TileEncoding.Linear_4bpp;
					palLength = 16;
					break;

				case TIM.TIM_ColorFormat.BPP_8:
					width *= 2;
					encoding = Util.TileEncoding.Linear_8bpp;
					palLength = 256;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			pal ??= PaletteHelpers.CreateDummyPalette(palLength).GetColors().ToArray();

			var tex = TextureHelpers.CreateTexture2D(width, height);

			tex.FillRegion(
				imgData: imgData,
				imgDataOffset: 0,
				pal: pal,
				encoding: encoding,
				regionX: 0,
				regionY: 0,
				regionWidth: tex.width,
				regionHeight: tex.height,
				flipTextureY: flipTextureY);

			tex.Apply();

			return tex;
		}

		/// <summary>
		/// Exports the v-ram as an image
		/// </summary>
		/// <param name="vram">The VRAM</param>
		/// <param name="outputPath">The path to export to</param>
		public static void ExportToFile(this VRAM vram, string outputPath)
		{
			Texture2D vramTex = TextureHelpers.CreateTexture2D(16 * 128, 2 * 256);

			for (int x = 0; x < 16 * 128; x++)
			{
				for (int y = 0; y < 2 * 256; y++)
				{
					byte val = vram.GetPixel8(0, y / 256, x, y % 256);
					vramTex.SetPixel(x, (2 * 256) - 1 - y, new Color(val / 255f, val / 255f, val / 255f));
				}
			}

			vramTex.Apply();
			Util.ByteArrayToFile(outputPath, vramTex.EncodeToPNG());
		}
	}
}