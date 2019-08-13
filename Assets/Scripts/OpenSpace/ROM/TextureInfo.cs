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
		public ushort texture_index_alpha;
		public ushort flags;
		public ushort wExponent;
		public ushort hExponent;
		public ushort size;
		public ushort bpp;
		public ushort palette_num_colors;
		public string name;
		public byte[] textureBytes;
		public byte[] paletteBytes;

		public Pointer off_texture;
		public Pointer off_palette;


        public static uint flags_isTransparent = (1 << 3);

        public TextureInfo(Pointer offset) {
            this.offset = offset;
        }

        public Texture2D Texture {
            get { return texture; }
            set {
                texture = value;
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
				tex.size = reader.ReadUInt16();
				tex.bpp = reader.ReadUInt16();
				tex.name = reader.ReadString(200);
				tex.off_texture = Pointer.Current(reader);
				tex.textureBytes = reader.ReadBytes(tex.size); // max size: 0x10000
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
				tex.texture_index_alpha = reader.ReadUInt16();
				tex.wExponent = reader.ReadByte();
				tex.hExponent = reader.ReadByte();
				tex.flags = reader.ReadUInt16();
				tex.size = reader.ReadUInt16();
				if (Settings.s.platform == Settings.Platform.DS) {
					tex.palette_num_colors = reader.ReadUInt16();
				} else {
					tex.palette_num_colors = 16;
				}
				uint ind_texture = 0;
				uint table_index = 0;
				uint? alpha_texture = null;
				bool rgba16 = false;
				Pointer off_alpha_texture = null;
				if (Settings.s.platform == Settings.Platform.DS) {
					if ((tex.flags & 1) != 0) {
						table_index = 1;
						ind_texture = (tex.texture_index + l.ind_textureTable1) & 0xFFFF;
					} else if ((tex.flags & 2) != 0) {
						table_index = 2;
						ind_texture = (tex.texture_index + l.ind_textureTable2) & 0xFFFF;
					} else if ((tex.flags & 4) != 0) {
						table_index = 3;
						ind_texture = (tex.texture_index + l.ind_textureTable3) & 0xFFFF;
					}
				} else {
					rgba16 = tex.texture_index_alpha != 0xFFFF && tex.texture_index != 0xFFFF;
					// If both texture_index and texture_index2: texture_index2 is the real one, texture_index is palette_index.
					ushort actualTexIndex = tex.texture_index_alpha != 0xFFFF ? tex.texture_index_alpha : tex.texture_index;
					ind_texture = (uint)(actualTexIndex) & 0xFFFF;
					if (rgba16) {
						ind_texture = l.ind_textureTable3 + tex.texture_index;
						alpha_texture = (uint)(tex.texture_index_alpha & 0xFFFF);
					}
				}
				l.print(((1 << tex.hExponent) * (1 << tex.wExponent)) + "\t"
					+ (1 << tex.wExponent) + "\t" + (1 << tex.hExponent) + "\t"
					+ (tex.texture_index == 0xFFFF ? "-" : tex.texture_index.ToString()) + "\t"
					+ (tex.texture_index_alpha == 0xFFFF ? "-" : tex.texture_index_alpha.ToString()) + "\t"
					+ (tex.palette_index == 0xFFFF ? "-" : (tex.palette_index & 0x7FFF).ToString()) + "\t"
					+ String.Format("{0:X4}", tex.flags) + "\t"
					+ tex.size);

				tex.off_texture = l.texturesTable[ind_texture];
				l.texturesTableSeen[ind_texture] = true;
				if (alpha_texture.HasValue) {
					off_alpha_texture = l.texturesTable[alpha_texture.Value];
					l.texturesTableSeen[alpha_texture.Value] = true;
				}
				tex.off_palette = null;
				GF64.Format format = tex.palette_num_colors == 16 ? GF64.Format.I4 : GF64.Format.I8;
				if (Settings.s.platform == Settings.Platform.DS) {
					if (tex.palette_index != 0xFFFF) {
						tex.off_palette = l.palettesTable[tex.palette_index & 0x7FFF];
						l.palettesTableSeen[tex.palette_index & 0x7FFF] = true;
					}
				} else {
					/*
					ushort actualPaletteIndex = tex.texture_index2 != 0xFFFF ? tex.texture_index : tex.palette_index;
					if (actualPaletteIndex != 0xFFFF) {
						tex.off_palette = l.palettesTable[actualPaletteIndex & 0x7FFF];
						l.palettesTableSeen[actualPaletteIndex & 0x7FFF] = true;
						l.print((l.palettesTable[(actualPaletteIndex & 0x7FFF) + 1].offset - l.palettesTable[(actualPaletteIndex & 0x7FFF)].offset) / 2);
					}*/
					if (rgba16) format = GF64.Format.RGBA16;
					if (tex.palette_index != 0xFFFF) {
						tex.off_palette = l.GetStructPtr(FATEntry.Type.Palette, tex.palette_index, global: true);
					}
				}
				//print(((1 << hExponent) * (1 << wExponent)) + "\t" + (1 << wExponent) + "\t" + (1 << hExponent) + "\t" + table_index + "\t" + field3 + "\t" + size + "\t" + palette_num_colors + "\t" + off_texture + "\t" + off_palette);
				GF64 mainTex = new GF64(reader,
					tex.off_texture,
					(1 << tex.wExponent),
					(1 << tex.hExponent),
					format,
					tex.off_palette,
					tex.palette_num_colors);
				if (off_alpha_texture != null) {
					GF64 alphaTex = new GF64(reader,
					off_alpha_texture,
					(1 << tex.wExponent),
					(1 << tex.hExponent),
					GF64.Format.I4,
					null,
					tex.palette_num_colors);
					mainTex.LoadAlphaTexture(alphaTex);
				}
				tex.texture = mainTex.texture;
				if (l.exportTextures) {
					if (!File.Exists(l.gameDataBinFolder + "/textures/" + table_index + "_" + tex.texture_index + "_" + tex.palette_index + ".png")) {
						Util.ByteArrayToFile(l.gameDataBinFolder + "/textures/" + table_index + "_" + tex.texture_index + "_" + tex.palette_index + ".png", tex.texture.EncodeToPNG());
					}
				}
			}
			return tex;
        }
    }
}
