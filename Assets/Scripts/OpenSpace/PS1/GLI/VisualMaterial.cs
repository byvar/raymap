using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1.GLI {
	public class VisualMaterial : IEquatable<VisualMaterial> {
		public TextureBounds texture;
		public byte materialFlags;
		public byte scroll;

		public float ScrollX => (scroll >> 4) / 15f / 10f;
		public float ScrollY => (scroll & 0xF) / 15f / 10f; // factor is still wrong, maybe it should be 256 or 255?
        public bool ScrollingEnabled => scroll != 0;
        public bool IsTransparent => texture?.IsTransparent ?? false;
        public bool IsLight => (materialFlags & 0x80) == 0x80;

        public override bool Equals(System.Object obj) {
            return obj is VisualMaterial && this == (VisualMaterial)obj;
        }
        public override int GetHashCode() {
            return texture.GetHashCode() ^ materialFlags.GetHashCode() ^ scroll.GetHashCode();
        }

        public bool Equals(VisualMaterial other) {
            return this == (VisualMaterial)other;
        }

        public static bool operator ==(VisualMaterial x, VisualMaterial y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.texture.Equals(y.texture) && x.scroll == y.scroll && x.materialFlags == y.materialFlags;
        }
        public static bool operator !=(VisualMaterial x, VisualMaterial y) {
            return !(x == y);
        }
    }
}
