using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1.GLI {
	public class TextureBounds {
        public ushort pageInfo;
        public ushort paletteInfo;
        public int xMin;
        public int xMax;
        public int yMin;
        public int yMax;
        public int Width => xMax - xMin;
        public int Height => yMax - yMin;

        public Texture2D texture;
        private bool? isTransparent;
        public bool IsTransparent {
            get {
                if (!isTransparent.HasValue) {
                    Color[] cols = texture.GetPixels();
                    foreach (Color col in cols) {
                        if (col.a != 1f) {
                            isTransparent = true;
                            return isTransparent.Value;
                        }
                    }
                    isTransparent = false;
                }
                return isTransparent.Value;
            }
        }

        public bool HasOverlap(TextureBounds b) {
            if (b.pageInfo != pageInfo || b.paletteInfo != paletteInfo) {
                return false;
            }

            bool xOverlap = (xMin >= b.xMin || xMin <= b.xMax) && (xMax >= b.xMin || xMax <= b.xMax);
            bool yOverlap = (yMin >= b.yMin || yMin <= b.yMax) && (yMax >= b.yMin || yMax <= b.yMax);
            return xOverlap && yOverlap;
        }

        public void ExpandWithBounds(TextureBounds b) {
            xMin = Math.Min(xMin, b.xMin);
            xMax = Math.Max(xMax, b.xMax);
            yMin = Math.Min(yMin, b.yMin);
            yMax = Math.Max(yMax, b.yMax);
        }

        public Vector2 CalculateUV(int x, int y) {
            float relativeX = (x - xMin) / (float)(Width-1);
            float relativeY = (y - yMin) / (float)(Height-1);
            return new Vector2(relativeX, 1f - relativeY);
        }
    }
}
