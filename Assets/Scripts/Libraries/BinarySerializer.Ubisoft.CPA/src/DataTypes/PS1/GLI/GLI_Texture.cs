using System;
using BinarySerializer.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GLI_Texture {
		public Context Context { get; private set; }

		public TSB TSB; // Page Info
		public CBA CBA; // Palette info
		public int xMin;
		public int xMax;
		public int yMin;
		public int yMax;
		public int Width => xMax - xMin;
		public int Height => yMax - yMin;

		public TSB.TexturePageTP BitDepth => TSB.TP;

		public GLI_Texture(Context c) {
			Context = c;
		}

		public bool HasOverlap(GLI_Texture b) {
			// Check CBA & TSB equality
			if (b.CBA != CBA || b.TSB != TSB)
				return false;

			// Check overlap
			bool xOverlap = (xMin >= b.xMin || xMin <= b.xMax) && (xMax >= b.xMin || xMax <= b.xMax);
			bool yOverlap = (yMin >= b.yMin || yMin <= b.yMax) && (yMax >= b.yMin || yMax <= b.yMax);
			return xOverlap && yOverlap;
		}

		public void ExpandWithBounds(GLI_Texture b) {
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
