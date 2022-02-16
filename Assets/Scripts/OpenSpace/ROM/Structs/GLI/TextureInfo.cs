using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System.IO;
using UnityEngine;

namespace OpenSpace.ROM {
	/// <summary>
	/// Texture definition
	/// </summary>
	public class TextureInfo : ROMStruct {
        private Texture2D texture;
		public ushort texture_index;
		public ushort palette_index;
		public ushort alpha_index;
		public ushort flags;
		public ushort flags2;
		public ushort wExponent;
		public ushort hExponent;
		public ushort color_size;
		public ushort bpp;
		public ushort palette_num_colors;
		public string name;
		public byte[] textureBytes;
		public byte[] paletteBytes;

		public LegacyPointer off_texture;
		public LegacyPointer off_alpha;
		public LegacyPointer off_palette;
		public GF64 mainTex;
		public GF64 alphaTex;


        public static ushort flags_isTransparent = 0x0008;
		public static ushort flags_isMirrorY     = 0x0010;
		public static ushort flags_isMirrorX     = 0x0020;
		public static ushort flags_isRepeatV     = 0x0040;
		public static ushort flags_isRepeatU     = 0x0080;
		public static ushort flags_renderWater1   = 0x0100;
		public static ushort flags_renderWater2   = 0x0200;
		public static ushort flags_renderTransparent   = 0x0400;
		public static ushort flags_alphaIsTransparency = 0x0800;

		public bool IsTransparent {
			get { return (flags & flags_isTransparent) != 0; }
		}
		public bool IsMirrorX {
			get { return (flags & flags_isMirrorX) != 0; }
		}
		public bool IsMirrorY {
			get { return (flags & flags_isMirrorY) != 0; }
		}
		public bool IsRepeatU {
			get { return (flags & flags_isRepeatU) != 0; }
		}
		public bool IsRepeatV {
			get { return (flags & flags_isRepeatV) != 0; }
		}
		public bool RenderTransparent {
			get { return (flags & flags_renderTransparent) != 0; }
		}
		public bool AlphaIsTransparency {
			get { return (flags & flags_alphaIsTransparency) != 0; }
		}
		public bool RenderWater1 {
			get { return (flags & flags_renderWater1) != 0; }
		}
		public bool RenderWater2 {
			get { return (flags & flags_renderWater2) != 0; }
		}


		public Texture2D Texture {
			get { return texture; }
			set {
				texture = value;
				if (texture != null) {
					if (!IsRepeatU) {
						texture.wrapModeU = TextureWrapMode.Clamp;
					}
					if (!IsRepeatV) {
						texture.wrapModeV = TextureWrapMode.Clamp;
					}
					if (IsMirrorX) {
						texture.wrapModeU = TextureWrapMode.Mirror;
					}
					if (IsMirrorY) {
						/*if (Settings.s.platform == Settings.Platform.N64) {
							Texture2D flipped = new Texture2D(texture.width, texture.height);

							int w = texture.width;
							int h = texture.height;


							for (int x = 0; x < w; x++) {
								for (int y = 0; y < h; y++) {
									flipped.SetPixel(x, h - y - 1, texture.GetPixel(x, y));
								}
							}
							flipped.Apply();
							texture = flipped;

							if (!IsRepeatU) {
								texture.wrapModeU = TextureWrapMode.Clamp;
							}
							if (IsMirrorX) {
								texture.wrapModeU = TextureWrapMode.Mirror;
							}
						}*/
						texture.wrapModeV = TextureWrapMode.Mirror;
					}
				}
			}
		}

	

		protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform._3DS) {
				wExponent = reader.ReadByte();
				hExponent = reader.ReadByte();
				flags = reader.ReadUInt16();
				flags2 = reader.ReadUInt16();
				color_size = reader.ReadUInt16();
				bpp = reader.ReadUInt16();
				name = reader.ReadString(200);
				off_texture = LegacyPointer.Current(reader);
				textureBytes = reader.ReadBytes(color_size); // max size: 0x10000
				Texture2D rawTex = new ETC(textureBytes, 1 << wExponent, 1 << hExponent, bpp == 32).texture;
				if (l.exportTextures) {
					if (!File.Exists(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", rawTex.EncodeToPNG());
					}
				}
				Texture = rawTex;
			} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.DS || Legacy_Settings.s.platform == Legacy_Settings.Platform.N64) {
				texture_index = reader.ReadUInt16();
				palette_index = reader.ReadUInt16();
				alpha_index = reader.ReadUInt16();
				wExponent = reader.ReadByte();
				hExponent = reader.ReadByte();
				flags = reader.ReadUInt16();
				color_size = reader.ReadUInt16();
				if (Legacy_Settings.s.platform == Legacy_Settings.Platform.DS) {
					palette_num_colors = reader.ReadUInt16();
				} else {
					palette_num_colors = 16;
				}
				ushort ind_texture = 0xFFFF;
				bool rgba16 = alpha_index != 0xFFFF && texture_index != 0xFFFF;
				GF64.Format format = GF64.Format.I4;
				if ((flags & 1) != 0) {
					ind_texture = (ushort)((texture_index + l.ind_textureTable_i4) & 0xFFFF);
				} else if ((flags & 2) != 0) {
					format = GF64.Format.I8;
					ind_texture = (ushort)((texture_index + l.ind_textureTable_i8) & 0xFFFF);
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.N64) {
						palette_num_colors = 256;
					}
				} else if ((flags & 4) != 0) {
					format = GF64.Format.RGBA;
					ind_texture = (ushort)((texture_index + l.ind_textureTable_rgba) & 0xFFFF);
				}
				if (ind_texture != 0xFFFF) {
					off_texture = l.texturesTable[ind_texture];
					if(l.texturesTableSeen != null) l.texturesTableSeen[ind_texture] = true;
				}
				if (alpha_index != 0xFFFF) {
					off_alpha = l.texturesTable[alpha_index];
					if (l.texturesTableSeen != null) l.texturesTableSeen[alpha_index] = true;
				}
				off_palette = null;
				/*if (Settings.s.platform == Settings.Platform.DS) {
					format = rgba16 ? GF64.Format.RGBA : (palette_num_colors == 16 ? GF64.Format.I4 : GF64.Format.I8);
				}*/
				if (palette_index != 0xFFFF) {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.DS) {
						off_palette = l.palettesTable[palette_index & 0x7FFF];
						if(l.palettesTableSeen != null) l.palettesTableSeen[palette_index & 0x7FFF] = true;
					} else {
						off_palette = l.GetStructPtr(FATEntry.Type.Palette, palette_index, global: true);
					}
				}
				/*l.print(((1 << hExponent) * (1 << wExponent)) + "\t"
					+ (1 << wExponent) + "\t" + (1 << hExponent) + "\t"
					+ (texture_index == 0xFFFF ? "-" : texture_index.ToString()) + "\t"
					+ (alpha_index == 0xFFFF ? "-" : alpha_index.ToString()) + "\t"
					+ (palette_index == 0xFFFF ? "-" : (palette_index & 0x7FFF).ToString()) + "\t"
					+ String.Format("{0:X4}", flags) + "\t"
					+ color_size + "\t"
					+ palette_num_colors + "\t"
					+ off_texture + "\t"
					+ off_alpha + "\t"
					+ off_palette + "\t");*/
				if (off_texture != null) {
					mainTex = new GF64(reader,
						off_texture,
						(1 << wExponent),
						(1 << hExponent),
						format,
						off_palette,
						palette_num_colors);
				}
				if (off_alpha != null) {
					alphaTex = new GF64(reader,
					off_alpha,
					(1 << wExponent),
					(1 << hExponent),
					GF64.Format.I4Alpha,
					null,
					palette_num_colors);
					if (mainTex != null) {
						mainTex.LoadAlphaTexture(alphaTex);
					}
				}
				Texture2D rawTex = mainTex != null ? mainTex.texture : alphaTex?.texture;
				if (l.exportTextures) {
                    if (rawTex != null) {
                        string palette = (palette_index != 0xFFFF ? "_P" + (palette_index & 0x7FFF) : "");
                        string alpha = (alpha_index != 0xFFFF ? "_A" + (alpha_index & 0x7FFF) : "");
                        string main = (texture_index != 0xFFFF ? "_T" + (texture_index & 0x7FFF) : "");
                        if (!File.Exists(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png")) {
                            Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png", rawTex.EncodeToPNG());
                        }
                    } else {
                        Debug.LogWarning("No mainTex or alphaTex for tex " + Offset);
                    }
				}
				Texture = rawTex;
			}
        }
    }
}
