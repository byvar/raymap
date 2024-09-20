using System;
using BinarySerializer.PlayStation.PS1;

namespace BinarySerializer.Ubisoft.CPA.PS1 {
	public class GLI_VisualMaterial : IEquatable<GLI_VisualMaterial> {
		public Context Context { get; set; }

		public GLI_Texture Texture { get; set; }
		public byte MaterialFlags { get; set; }
		public byte Scroll { get; set; }

		public GLI_VisualMaterial(Context c) {
			Context = c;
		}

		public SemiTransparentMode BlendMode {
			get {
				if (Texture == null) return SemiTransparentMode.One;
				int abr = (int)Texture.TSB.ABR;
				return (SemiTransparentMode)abr;
			}

		}
		public float ScrollX {
			get {
				int scr = (Scroll >> 5);
				if (scr == 0) return 0f;
				bool special = (MaterialFlags & 0x40) != 0; // sets 0x80
				int width = 0;
				float speed = 1f;
				ScrollMode mode = (ScrollMode)scr;
				switch (mode) {
					case ScrollMode.None:
						return 0f;
					case ScrollMode.Width64:
						width = 64;
						speed /= 2f;
						if (special) speed *= 4;
						break;
					case ScrollMode.Width128_2:
					case ScrollMode.Width128_3:
					case ScrollMode.Width128_5:
						width = 128;
						if (!special) {
							speed /= 2;
						} else {
							speed *= 4;
						}
						break;

				}
				if (width == 0) return 0f;
				float scrf = (speed / width);
				return scrf;
				//return (scr & 0b1000) != 0 ? -scrf : scrf;
			}
		}
		public float ScrollY => 0f;
		public bool ScrollingEnabled => (Scroll >> 5) != 0;
		public bool IsLight => (MaterialFlags & 0x80) == 0x80;

		public override bool Equals(System.Object obj) {
			return obj is GLI_VisualMaterial && this == (GLI_VisualMaterial)obj;
		}
		public override int GetHashCode() {
			return Texture.GetHashCode() ^ MaterialFlags.GetHashCode() ^ Scroll.GetHashCode();
		}

		public bool Equals(GLI_VisualMaterial other) {
			return this == (GLI_VisualMaterial)other;
		}

		public static bool operator ==(GLI_VisualMaterial x, GLI_VisualMaterial y) {
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			return x.Texture.Equals(y.Texture) && x.Scroll == y.Scroll && x.MaterialFlags == y.MaterialFlags;
		}
		public static bool operator !=(GLI_VisualMaterial x, GLI_VisualMaterial y) {
			return !(x == y);
		}

		public enum ScrollMode {
			None = 0,
			Width64,
			Width128_2,
			Width128_3,
			Backwards,
			Width128_5,
		}
		public enum SemiTransparentMode {
			Point5 = 0,
			One,
			MinusOne,
			Point25
		}
	}
}
