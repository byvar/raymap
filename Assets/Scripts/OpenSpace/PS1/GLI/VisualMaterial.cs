using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1.GLI {
	public class VisualMaterial : IEquatable<VisualMaterial> {
		public TextureBounds texture;
		public byte materialFlags;
		public byte scroll;

        public SemiTransparentMode BlendMode {
            get {
                if (texture == null) return SemiTransparentMode.One;
                int abr = Util.ExtractBits(texture.pageInfo, 2, 5);
                return (SemiTransparentMode)abr;
            }

        }
		public float ScrollX {
            get {
                int scr = (scroll >> 5);
                if (scr == 0) return 0f;
                bool special = (materialFlags & 0x40) != 0; // sets 0x80
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
        public bool ScrollingEnabled => (scroll >> 5) != 0;
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
        public Material CreateMaterial() {
            TextureBounds b = texture;
            Material baseMaterial;
            SemiTransparentMode stMode = BlendMode;
            if (IsLight || stMode == SemiTransparentMode.MinusOne) {
                baseMaterial = MapLoader.Loader.baseLightMaterial;
            } else if (/*m.colors.Any(c => c.a != 1f) || */IsTransparent || stMode != SemiTransparentMode.One) {
                baseMaterial = MapLoader.Loader.baseTransparentMaterial;
            } else {
                baseMaterial = MapLoader.Loader.baseMaterial;
            }
            Material mat = new Material(baseMaterial);
            mat.SetInt("_NumTextures", 1);
            string textureName = "_Tex0";
            Texture2D tex = b.texture;
            if (ScrollingEnabled) tex.wrapMode = TextureWrapMode.Repeat;
            mat.SetTexture(textureName, tex);

            mat.SetVector(textureName + "Params", new Vector4(0,
                ScrollingEnabled ? 1f : 0f,
                0f, 0f));
            mat.SetVector(textureName + "Params2", new Vector4(
                0f, 0f,
                ScrollX, ScrollY));
            mat.SetVector("_AmbientCoef", Vector4.one);
            mat.SetFloat("_Prelit", 1f);
            switch (stMode) {
                case SemiTransparentMode.MinusOne:
                    mat.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
                    mat.SetInt("_SrcBlendMode", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlendMode", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
            }
            return mat;
        }
    }
}
