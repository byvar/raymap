using System.Collections.Generic;
using System.IO;
using BinarySerializer;
using BinarySerializer.Unity;
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
		public static void ExportPaletteToGimp(string outputPath, string name, BaseColor[] palette)
		{
			// Create the file
			using var fileStream = File.Create(outputPath);

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
		/// Exports a palette to a file
		/// </summary>
		/// <param name="outputPath">The path to export to</param>
		/// <param name="palette">The palette</param>
		public static void ExportPalette(string outputPath, IList<BaseColor> palette, int scale = 16, int offset = 0, int? optionalLength = null, int? optionalWrap = null, bool reverseY = false)
		{
			int length = optionalLength ?? palette.Count;
			int wrap = optionalWrap ?? length;
			var tex = TextureHelpers.CreateTexture2D(Mathf.Min(length, wrap) * scale, Mathf.CeilToInt(length / (float)wrap) * scale, clear: true);

			for (int i = 0; i < length; i++)
			{
				int mainY = (tex.height / scale) - 1 - (i / wrap);
				int mainX = i % wrap;

				Color col = palette[offset + i].GetColor();

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