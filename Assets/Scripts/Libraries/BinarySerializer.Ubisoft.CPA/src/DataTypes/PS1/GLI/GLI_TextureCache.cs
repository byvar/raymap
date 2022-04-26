using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GLI_TextureCache {
		public Context Context { get; private set; }

		public GLI_TextureCache(Context c) {
			Context = c;
		}

		private List<GLI_Texture> textures = new List<GLI_Texture>();

		public void Clear() {
			textures.Clear();
		}

		public void RegisterTexture(PS1_TSB tsb, PS1_CBA cba, int xMin, int xMax, int yMin, int yMax) {
			GLI_Texture b = new GLI_Texture(Context) {
				TSB = tsb,
				CBA = cba,
				xMin = xMin,
				xMax = xMax,
				yMin = yMin,
				yMax = yMax
			};

			bool newTexture = true;
			foreach (GLI_Texture u in textures) {
				if (u.HasOverlap(b)) {
					u.ExpandWithBounds(b);
					newTexture = false;
					break;
				}
			}

			if (newTexture) {
				textures.Add(b);
			}
		}

		/*public void CalculateTextures() {
			int i = 0;
			foreach (GLI_Texture b in textureBounds) {
				int w = b.xMax - b.xMin;
				int h = b.yMax - b.yMin;
				//print(w + " - " + h + " - " + b.xMin + " - " + b.yMin + " - " + b.pageInfo + " - " + b.paletteInfo);
				Texture2D tex = vram.GetTexture((ushort)w, (ushort)h, b.pageInfo, b.paletteInfo, b.xMin, b.yMin);
				if (tex == null) {
					Debug.LogWarning($"Corrupted texture found! Details: {w}x{h} - ({b.xMin},{b.yMin}) - Page:{b.pageInfo} - Pal:{b.paletteInfo}");
					tex = new Texture2D(w, h);
					tex.SetPixels(Enumerable.Repeat(Color.clear, w * h).ToArray());
					tex.Apply();
				}
				tex.wrapMode = TextureWrapMode.Clamp;
				b.texture = tex;
				if (exportTextures) {
					Util.ByteArrayToFile(gameDataBinFolder + "textures/main/" + lvlName + "/" + i++ + $"_{string.Format("{0:X4}", b.pageInfo)}_{b.xMin}_{b.yMin}_{w}_{h}" + ".png", tex.EncodeToPNG());
				}
			}
		}*/

		public GLI_Texture GetTexture(PS1_TSB tsb, PS1_CBA cba, int x, int y) {
			return textures.FirstOrDefault(
				t => t.CBA == cba && t.TSB == tsb &&
				x >= t.xMin && x < t.xMax &&
				y >= t.yMin && y < t.yMax);
		}
	}
}
