using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public Reference<VisualMaterialTextures> textures;
		public ushort num_textures;
		public ushort flags;
		public ushort diffuseCoef;
		public ushort ambientCoef;
		public float scrollSpeedX;
		public float scrollSpeedY;
		public ushort num_animTextures;

		public byte r;
		public byte g;
		public byte b;
		public byte a;

		protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			if (Settings.s.platform == Settings.Platform.N64) {
				r = reader.ReadByte();
				g = reader.ReadByte();
				b = reader.ReadByte();
				a = reader.ReadByte();
			} else {
				diffuseCoef = reader.ReadUInt16();
				ambientCoef = reader.ReadUInt16();
			}
			scrollSpeedX = reader.ReadSingle();
			scrollSpeedY = reader.ReadSingle();
			textures = new Reference<VisualMaterialTextures>(reader);
			num_textures = reader.ReadUInt16();
			num_animTextures = reader.ReadUInt16();
			flags = reader.ReadUInt16();
			textures.Resolve(reader, (vmt) => { vmt.length = num_textures; });
		}

		public Material GetMaterial(Hint hints, GameObject gao = null) {
			Material mat;
			bool billboard = (hints & Hint.Billboard) == Hint.Billboard;
			if (textures.Value != null && num_textures > 0) {
				TextureInfo texInfo = textures.Value.vmTex[0].texRef.Value.texInfo;
				if (texInfo.RenderTransparent || texInfo.RenderWater1 || texInfo.RenderWater2) {
					if (texInfo.AlphaIsTransparency || texInfo.RenderWater1 || texInfo.RenderWater2) {
						mat = new Material(MapLoader.Loader.baseTransparentMaterial);
					} else {
						if (Settings.s.platform == Settings.Platform._3DS) {
							mat = new Material(MapLoader.Loader.baseLightMaterial);
						} else {
							mat = new Material(MapLoader.Loader.baseTransparentMaterial);
							mat.SetFloat("_DisableLightingLocal", 1f);
						}
						//mat = new Material(MapLoader.Loader.baseLightMaterial);
					}
				} else {
					mat = new Material(MapLoader.Loader.baseMaterial);
				}
				mat.SetInt("_NumTextures", 1);
				string textureName = "_Tex0";
				mat.SetTexture(textureName, textures.Value.vmTex[0].texRef.Value.texInfo.Value.Texture);
				mat.SetVector(textureName + "Params", new Vector4(0,
					(scrollSpeedX != 0 || scrollSpeedY != 0) ? 1f : 0f,
					0f, 0f));
				mat.SetVector(textureName + "Params2", new Vector4(
					0f, 0f, ScrollX, ScrollY));
			} else {
				mat = new Material(MapLoader.Loader.baseMaterial);
			}
			Vector4 ambient, diffuse;
			if (Settings.s.platform == Settings.Platform.N64) {
				ambient = new Vector4(0.25f, 0.25f, 0.25f, 1f);
				diffuse = new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
			} else {
				ambient = ParseColorRGBA5551(ambientCoef) + new Vector4(0.25f, 0.25f, 0.25f, 1f);
				diffuse = ParseColorRGBA5551(diffuseCoef) + new Vector4(0, 0, 0, 1f);
			}
			mat.SetVector("_AmbientCoef", ambient);
			mat.SetVector("_DiffuseCoef", diffuse);
			if (billboard) mat.SetFloat("_Billboard", 1f);
			if (gao != null && num_textures > 1) {
				MultiTextureMaterial mtmat = gao.AddComponent<MultiTextureMaterial>();
				mtmat.visMatROM = this;
				mtmat.mat = mat;
			}
			return mat;
		}

		public float ScrollX {
			get {
				return scrollSpeedX * Mathf.Abs(Settings.s.textureAnimationSpeedModifier);
			}
		}
		public float ScrollY {
			get {
				return scrollSpeedY * Settings.s.textureAnimationSpeedModifier;
			}
		}

		public static ushort flags_renderBackFaces = 0x0100;

		public bool RenderBackFaces {
			get { return (flags & flags_renderBackFaces) != 0; }
		}

		public enum Hint {
			None = 0,
			Transparent = 1,
			Billboard = 2
		}

		static uint ExtractBits(int number, int count, int offset) {
			return (uint)(((1 << count) - 1) & (number >> (offset)));
		}

		static Vector4 ParseColorRGBA5551(ushort shortCol) {
			uint alpha, blue, green, red;
			if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform._3DS) {
				alpha = ExtractBits(shortCol, 1, 15);
				blue = ExtractBits(shortCol, 5, 10);
				green = ExtractBits(shortCol, 5, 5);
				red = ExtractBits(shortCol, 5, 0);
			} else {
				alpha = ExtractBits(shortCol, 1, 0);
				blue = ExtractBits(shortCol, 5, 1);
				green = ExtractBits(shortCol, 5, 6);
				red = ExtractBits(shortCol, 5, 11);
			}
			return new Vector4(red / 31.0f, green / 31.0f, blue / 31.0f, alpha);
		}
		static Vector4 ParseColorRGB565(ushort shortCol) {
			uint blue, green, red;
			if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform._3DS) {
				red = ExtractBits(shortCol, 5, 0);
				green = ExtractBits(shortCol, 6, 5);
				blue = ExtractBits(shortCol, 5, 11);
			} else {
				red = ExtractBits(shortCol, 5, 11);
				green = ExtractBits(shortCol, 6, 5);
				blue = ExtractBits(shortCol, 5, 0);
			}
			return new Vector4(red / 31.0f, green / 63.0f, blue / 31.0f, 1f);
		}
		static Vector4 ParseColorRGBA4444(ushort shortCol) {
			uint alpha, blue, green, red;
			if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform._3DS) {
				alpha = ExtractBits(shortCol, 4, 12);
				blue = ExtractBits(shortCol, 4, 8);
				green = ExtractBits(shortCol, 4, 4);
				red = ExtractBits(shortCol, 4, 0);
			} else {
				alpha = ExtractBits(shortCol, 4, 0);
				blue = ExtractBits(shortCol, 4, 4);
				green = ExtractBits(shortCol, 4, 8);
				red = ExtractBits(shortCol, 4, 12);
			}
			return new Vector4(red / 15.0f, green / 15.0f, blue / 15.0f, alpha / 15.0f);
		}
	}
}
