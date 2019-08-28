using Newtonsoft.Json;
using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM {
    /// <summary>
    /// Texture definition
    /// </summary>
    public class TextureInfo {
        public Pointer offset;
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

        public TextureInfo(Pointer offset) {
            this.offset = offset;
        }

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

		public static TextureInfo Read(Reader reader, ushort index) {
			Pointer off = (MapLoader.Loader as R2DSLoader).GetStructPtr(FATEntry.Type.TextureInfo, index);
			TextureInfo result = null;
			Pointer.DoAt(ref reader, off, () => {
				result = Read(reader, off);
			});
			return result;
		}

        public static TextureInfo Read(Reader reader, Pointer offset) {
			R2DSLoader l = MapLoader.Loader as R2DSLoader;
            TextureInfo tex = new TextureInfo(offset);
			if (Settings.s.platform == Settings.Platform._3DS) {
				tex.wExponent = reader.ReadByte();
				tex.hExponent = reader.ReadByte();
				reader.ReadUInt16();
				tex.flags = reader.ReadUInt16();
				tex.color_size = reader.ReadUInt16();
				tex.bpp = reader.ReadUInt16();
				tex.name = reader.ReadString(200);
				tex.off_texture = Pointer.Current(reader);
				tex.textureBytes = reader.ReadBytes(tex.color_size); // max size: 0x10000
				tex.texture = new ETC(tex.textureBytes, 1 << tex.wExponent, 1 << tex.hExponent, tex.bpp == 32).texture;
				if (l.exportTextures) {
					if (!File.Exists(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(tex.name) + "/" + Path.GetFileNameWithoutExtension(tex.name) + ".png")) {
						Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + Path.GetDirectoryName(tex.name) + "/" + Path.GetFileNameWithoutExtension(tex.name) + ".png", tex.texture.EncodeToPNG());
					}
				} else {
					l.print(tex.name);
				}
			} else if (Settings.s.platform == Settings.Platform.DS || Settings.s.platform == Settings.Platform.N64) {
				tex.texture_index = reader.ReadUInt16();
				tex.palette_index = reader.ReadUInt16();
				tex.alpha_index = reader.ReadUInt16();
				tex.wExponent = reader.ReadByte();
				tex.hExponent = reader.ReadByte();
				tex.flags = reader.ReadUInt16();
				tex.color_size = reader.ReadUInt16();
				if (Settings.s.platform == Settings.Platform.DS) {
					tex.palette_num_colors = reader.ReadUInt16();
				} else {
					tex.palette_num_colors = 16;
				}
				ushort ind_texture = 0xFFFF;
				bool rgba16 = tex.alpha_index != 0xFFFF && tex.texture_index != 0xFFFF;
				GF64.Format format = GF64.Format.I4;
				if ((tex.flags & 1) != 0) {
					ind_texture = (ushort)((tex.texture_index + l.ind_textureTable_i4) & 0xFFFF);
				} else if ((tex.flags & 2) != 0) {
					format = GF64.Format.I8;
					ind_texture = (ushort)((tex.texture_index + l.ind_textureTable_i8) & 0xFFFF);
					tex.palette_num_colors = 256;
				} else if ((tex.flags & 4) != 0) {
					format = GF64.Format.RGBA;
					ind_texture = (ushort)((tex.texture_index + l.ind_textureTable_rgba) & 0xFFFF);
				}
				if (ind_texture != 0xFFFF) {
					tex.off_texture = l.texturesTable[ind_texture];
					l.texturesTableSeen[ind_texture] = true;
				}
				if (tex.alpha_index != 0xFFFF) {
					tex.off_alpha = l.texturesTable[tex.alpha_index];
					l.texturesTableSeen[tex.alpha_index] = true;
				}
				tex.off_palette = null;
				//GF64.Format format = rgba16 ? GF64.Format.RGBA5551 : (tex.palette_num_colors == 16 ? GF64.Format.I4 : GF64.Format.I8);
				if (tex.palette_index != 0xFFFF) {
					if (Settings.s.platform == Settings.Platform.DS) {
						tex.off_palette = l.palettesTable[tex.palette_index & 0x7FFF];
						l.palettesTableSeen[tex.palette_index & 0x7FFF] = true;
					} else {
						tex.off_palette = l.GetStructPtr(FATEntry.Type.Palette, tex.palette_index, global: true);
					}
				}
				l.print(((1 << tex.hExponent) * (1 << tex.wExponent)) + "\t"
					+ (1 << tex.wExponent) + "\t" + (1 << tex.hExponent) + "\t"
					+ (tex.texture_index == 0xFFFF ? "-" : tex.texture_index.ToString()) + "\t"
					+ (tex.alpha_index == 0xFFFF ? "-" : tex.alpha_index.ToString()) + "\t"
					+ (tex.palette_index == 0xFFFF ? "-" : (tex.palette_index & 0x7FFF).ToString()) + "\t"
					+ String.Format("{0:X4}", tex.flags) + "\t"
					+ tex.color_size + "\t"
					+ tex.palette_num_colors + "\t"
					+ tex.off_texture + "\t"
					+ tex.off_alpha + "\t"
					+ tex.off_palette + "\t");
				//print(((1 << hExponent) * (1 << wExponent)) + "\t" + (1 << wExponent) + "\t" + (1 << hExponent) + "\t" + table_index + "\t" + field3 + "\t" + size + "\t" + palette_num_colors + "\t" + off_texture + "\t" + off_palette);
				if (tex.off_texture != null) {
					tex.mainTex = new GF64(reader,
						tex.off_texture,
						(1 << tex.wExponent),
						(1 << tex.hExponent),
						format,
						tex.off_palette,
						tex.palette_num_colors);
				}
				if (tex.off_alpha != null) {
					tex.alphaTex = new GF64(reader,
					tex.off_alpha,
					(1 << tex.wExponent),
					(1 << tex.hExponent),
					GF64.Format.I4Alpha,
					null,
					tex.palette_num_colors);
					if (tex.mainTex != null) {
						tex.mainTex.LoadAlphaTexture(tex.alphaTex);
					}
				}
				tex.Texture = tex.mainTex != null ? tex.mainTex.texture : tex.alphaTex?.texture;
				if (l.exportTextures) {

                    if (tex.Texture != null) {

                        string palette = (tex.palette_index != 0xFFFF ? "_P" + (tex.palette_index & 0x7FFF) : "");
                        string alpha = (tex.alpha_index != 0xFFFF ? "_A" + (tex.alpha_index & 0x7FFF) : "");
                        string main = (tex.texture_index != 0xFFFF ? "_T" + (tex.texture_index & 0x7FFF) : "");
                        if (!File.Exists(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png")) {
                            Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + format + main + alpha + palette + ".png", tex.Texture.EncodeToPNG());
                        }

                    } else {
                        Debug.LogWarning("No mainTex or alphaTex for tex " + tex.offset);
                    }
				}
			}
			return tex;
        }
    }
}
