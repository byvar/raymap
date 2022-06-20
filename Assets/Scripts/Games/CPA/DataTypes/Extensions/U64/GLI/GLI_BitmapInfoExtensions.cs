using BinarySerializer.Unity;
using Raymap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BinarySerializer.Ubisoft.CPA.PS1.GLI_VisualMaterial;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public static class GLI_BitmapInfoExtensions {
		public static Texture2D GetTexture(this GLI_BitmapInfo bi, bool flip = false) {
			if (bi.Context?.GetCPASettings().Platform == Platform._3DS) {
				throw new NotImplementedException($"Not yet implemented for 3DS");
			}
			var w = bi.Texture?.Value?.Width ?? bi.Alpha?.Value?.Width ?? 0;
			var h = bi.Texture?.Value?.Height ?? bi.Alpha?.Value?.Height ?? 0;
			var tex = TextureHelpers.CreateTexture2D(w, h);

			GLI_PaletteRGBA16 pal;
			if (bi.Context?.GetCPASettings().Platform == Platform.N64) {
				pal = bi.PaletteN64?.Value;
			} else {
				pal = bi.Palette?.Value;
			}
			GLI_BitmapCI4 alpha = bi.Alpha?.Value;
			GLI_Bitmap main = bi.Texture?.Value;
			BaseColor[] palette = pal?.Palette;
			Util.TileEncoding enc4bpp = (bi.Context?.GetCPASettings().Platform == Platform.N64)
				? Util.TileEncoding.Linear_4bpp_ReverseOrder
				: Util.TileEncoding.Linear_4bpp;

			if (main != null) {
				switch (main) {
					case GLI_BitmapCI8 mainCI8:
						switch (bi.Type) {
							case GLI_BitmapInfo.BitmapType.Alpha3i5:
								if (palette == null) palette = PaletteHelpers.CreateDummyPalette(32, firstTransparent: false);
								tex.FillRegion(mainCI8.Bitmap, 0, palette.GetColors(), Util.TileEncoding.Linear_8bpp_A3i5, 0, 0, w, h, flipTextureY: flip);
								break;
							case GLI_BitmapInfo.BitmapType.Alpha5i3:
								if (palette == null) palette = PaletteHelpers.CreateDummyPalette(8, firstTransparent: false);
								tex.FillRegion(mainCI8.Bitmap, 0, palette.GetColors(), Util.TileEncoding.Linear_8bpp_A5i3, 0, 0, w, h, flipTextureY: flip);
								break;
							default:
								if (palette == null) palette = PaletteHelpers.CreateDummyPalette(256, firstTransparent: false);
								tex.FillRegion(mainCI8.Bitmap, 0, palette.GetColors(), Util.TileEncoding.Linear_8bpp, 0, 0, w, h, flipTextureY: flip);
								break;
						}
						break;
					case GLI_BitmapCI4 mainCI4:
						if (palette == null) palette = PaletteHelpers.CreateDummyPalette(16, firstTransparent: false);
						tex.FillRegion(mainCI4.Bitmap, 0, palette.GetColors(), enc4bpp, 0, 0, w, h, flipTextureY: flip);
						break;
					case GLI_BitmapRGBA16 mainRGBA16:
						tex.FillRegion(mainRGBA16.Bitmap, 0, 0, 0, w, h, flipTextureY: flip);
						break;
				}
			} else {
				for (int x = 0; x < w; x++) {
					for (int y = 0; y < h; y++) {
						tex.SetPixel(x, y, Color.white);
					}
				}
			}

			if (alpha != null) {
				tex.Apply();
				Texture2D at = TextureHelpers.CreateTexture2D(w, h);
				at.FillRegion(alpha.Bitmap, 0, PaletteHelpers.CreateDummyPalette(16, firstTransparent: false).GetColors(), enc4bpp, 0, 0, w, h, flipTextureY: flip);
				at.Apply();
				var texPixels = tex.GetPixels();
				var alphaPixels = at.GetPixels();
				for (int i = 0; i < texPixels.Length; i++) {
					texPixels[i] = new Color(texPixels[i].r, texPixels[i].g, texPixels[i].b, alphaPixels[i].r);
				}
				tex.SetPixels(texPixels);
			}


			tex.Apply();
			return tex;
			//return tb.Context.GetUnityLevel().GetUnityData<GLI_UnityTexture, GLI_Texture>(tb).Texture;
		}
	}
}
