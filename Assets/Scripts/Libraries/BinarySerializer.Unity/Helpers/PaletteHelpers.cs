using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BinarySerializer.Unity
{
	/// <summary>
	/// Helper methods for palettes
	/// </summary>
	public static class PaletteHelpers
	{
		/// <summary>
		/// Exports the specified palette to the gimp palette format
		/// </summary>
		/// <param name="outputPath">The path to export to</param>
		/// <param name="name">The palette name</param>
		/// <param name="palette">The palette</param>
		public static void ExportToGimp(string outputPath, string name, BaseColor[] palette)
		{
			// Create the file
			using FileStream fileStream = File.Create(outputPath);

			// Use a writer
			using var writer = new StreamWriter(fileStream);

			// Write header
			writer.WriteLine("GIMP Palette");
			writer.WriteLine("Name: " + name);
			writer.WriteLine("#");

			// Write colors
			foreach (var color in palette)
				writer.WriteLine($"{color.Red,-3} {color.Green,-3} {color.Blue,-3}");
		}

		/// <summary>
		/// Exports a palette to a .png file
		/// </summary>
		/// <param name="outputPath">The path to export to</param>
		/// <param name="palette">The palette</param>
		/// <param name="scale">The palette scale on the resulting texture</param>
		/// <param name="start">The color index to start from</param>
		/// <param name="length">The length of the palette to use or null to use the length of the color array</param>
		/// <param name="wrap">Optional color wrapping on the resulting texture or null to not wrap</param>
		/// <param name="reverseY">Indicates if the y-axis should be reversed</param>
		public static void ExportToPNG(
			string outputPath, 
			IList<BaseColor> palette, int scale = 16, 
			int start = 0, int? length = null, 
			int? wrap = null, bool reverseY = false)
		{
			int palLength = length ?? palette.Count;
			int palWrap = wrap ?? palLength;
			Texture2D tex = TextureHelpers.CreateTexture2D(
				Mathf.Min(palLength, palWrap) * scale, 
				Mathf.CeilToInt(palLength / (float)palWrap) * scale, 
				clear: true);

			for (int i = 0; i < palLength; i++)
			{
				int mainY = (tex.height / scale) - 1 - (i / palWrap);
				int mainX = i % palWrap;

				Color col = palette[start + i].GetColor();

				// Remove transparency
				col = new Color(col.r, col.g, col.b);

				for (int y = 0; y < scale; y++)
				{
					for (int x = 0; x < scale; x++)
					{
						var xx = mainX * scale + x;
						var yy = mainY * scale + y;

						if (reverseY)
							yy = tex.height - yy - 1;

						tex.SetPixel(xx, yy, col);
					}
				}
			}

			tex.Apply();

			Util.ByteArrayToFile(outputPath, tex.EncodeToPNG());
		}

		public static BaseColor[] CreateDummyPalette(int length, bool firstTransparent = true, int? wrap = null)
		{
			BaseColor[] pal = new BaseColor[length];

			wrap ??= length;

			if (firstTransparent)
				pal[0] = BaseColor.Clear;

			for (int i = firstTransparent ? 1 : 0; i < length; i++)
			{
				float val = (float)(i % wrap.Value) / (wrap.Value - 1);
				pal[i] = new CustomColor(val, val, val);
			}

			return pal;
		}
	}
}