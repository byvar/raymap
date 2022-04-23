using System;
using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GLI_TextureBounds {

		public PS1_TSB TSB; // Page Info
		public PS1_CBA CBA; // Palette info
		public int xMin;
		public int xMax;
		public int yMin;
		public int yMax;
		public int Width => xMax - xMin;
		public int Height => yMax - yMin;

		private bool? isTransparent;
		public bool IsTransparent {
			get {
				if (!isTransparent.HasValue) {
					/*Color[] cols = texture.GetPixels();
					foreach (Color col in cols) {
						if (col.a != 1f) {
							isTransparent = true;
							return isTransparent.Value;
						}
					}
					isTransparent = false;*/
				}
				return isTransparent.Value;
			}
		}

		public PS1_TSB.TexturePageTP BitDepth => TSB.TP;

		public bool HasOverlap(GLI_TextureBounds b) {
			// Check CBA equality
			if (b.CBA.ClutX != CBA.ClutX || b.CBA.ClutY != CBA.ClutY)
				return false;

			// Check TSB equality
			if (b.TSB.TP != TSB.TP || b.TSB.TX != TSB.TX || b.TSB.TY != TSB.TY || b.TSB.ABR != TSB.ABR)
				return false;

			// Check overlap
			bool xOverlap = (xMin >= b.xMin || xMin <= b.xMax) && (xMax >= b.xMin || xMax <= b.xMax);
			bool yOverlap = (yMin >= b.yMin || yMin <= b.yMax) && (yMax >= b.yMin || yMax <= b.yMax);
			return xOverlap && yOverlap;
		}

		public void ExpandWithBounds(GLI_TextureBounds b) {
			xMin = Math.Min(xMin, b.xMin);
			xMax = Math.Max(xMax, b.xMax);
			yMin = Math.Min(yMin, b.yMin);
			yMax = Math.Max(yMax, b.yMax);
		}

		public MTH2D_Vector CalculateUV(int x, int y) {
			float relativeX = (x - xMin) / (float)(Width - 1);
			float relativeY = (y - yMin) / (float)(Height - 1);
			return new MTH2D_Vector(relativeX, 1f - relativeY);
		}
	}
}
