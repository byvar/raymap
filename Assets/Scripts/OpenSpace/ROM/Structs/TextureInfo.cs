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
		public ushort wExponent;
		public ushort hExponent;
		public ushort color_size;
		public ushort bpp;
		public ushort palette_num_colors;
		public string name;
		public byte[] textureBytes;
		public byte[] paletteBytes;

		public Pointer off_texture;
		public Pointer off_alpha;
		public Pointer off_palette;
		public GF64 mainTex;
		public GF64 alphaTex;


        public static uint flags_isTransparent = (1 << 3);


        public Texture2D Texture {
            get { return texture; }
            set {
				texture = value;
				/*if (Settings.s.platform == Settings.Platform.DS) {
					if ((flags & 0x0A00) == 0x0A00) { // strange transparency thing
						Color[] colors = texture.GetPixels();
						for (int i = 0; i < colors.Length; i++) {
							colors[i].a = 1f;
						}
						texture.SetPixels(colors);
						texture.Apply();
					}
				}*/
            }
        }

		protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			if (Settings.s.platform == Settings.Platform._3DS) {
				wExponent = reader.ReadByte();
				hExponent = reader.ReadByte();
				reader.ReadUInt16();
				flags = reader.ReadUInt16();
				color_size = reader.ReadUInt16();
				bpp = reader.ReadUInt16();
				name = reader.ReadString(200);
				off_texture = Pointer.Current(reader);
				textureBytes = reader.ReadBytes(color_size); // max size: 0x10000
				texture = new ETC(textureBytes, 1 << wExponent, 1 << hExponent, bpp == 32).texture;
				if (l.exportTextures) {
					if (!File.Exists(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", texture.EncodeToPNG());
					}
				} else {
					l.print(name);
				}
			} else if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform.N64) {
				texture_index = reader.ReadUInt16();
				palette_index = reader.ReadUInt16();
				alpha_index = reader.ReadUInt16();
				wExponent = reader.ReadByte();
				hExponent = reader.ReadByte();
				flags = reader.ReadUInt16();
				color_size = reader.ReadUInt16();
				if (Settings.s.platform == Settings.Platform.DS) {
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
					if (Settings.s.platform == Settings.Platform.N64) {
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
					if (Settings.s.platform == Settings.Platform.DS) {
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
				Texture = mainTex != null ? mainTex.texture : alphaTex?.texture;
				if (l.exportTextures) {

                    if (Texture != null) {

                        string palette = (palette_index != 0xFFFF ? "_P" + (palette_index & 0x7FFF) : "");
                        string alpha = (alpha_index != 0xFFFF ? "_A" + (alpha_index & 0x7FFF) : "");
                        string main = (texture_index != 0xFFFF ? "_T" + (texture_index & 0x7FFF) : "");
                        if (!File.Exists(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png")) {
                            Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png", Texture.EncodeToPNG());
                        }

                    } else {
                        Debug.LogWarning("No mainTex or alphaTex for tex " + Offset);
                    }
				}
			}
        }
    }
}
