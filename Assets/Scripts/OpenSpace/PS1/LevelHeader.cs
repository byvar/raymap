using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class LevelHeader : OpenSpaceStruct {
		public Pointer off_0F0;
		public Pointer off_0F4;
		public Pointer off_0F8;
		public uint uint_0FC;
		public Pointer off_100;
		public uint uint_104;
		public uint uint_108;
		public Pointer off_10C;
		public Pointer off_110;
		public ushort num_persos;
		public ushort ushort_116;
		public Pointer off_persos;
		public Pointer off_states;
		public ushort num_states;
		public ushort ushort_122;
		public uint uint_124;
		public uint uint_128;
		public Pointer off_12C;
		public Pointer off_130;
		public uint uint_134;
		public uint num_always;
		public uint uint_13C;
		public uint uint_140;
		public uint uint_144;
		public int int_148;
		public Pointer off_14C;
		public Pointer off_150;
		public Pointer off_154;
		public uint uint_158;

		// UI Textures
		public Pointer off_ui_textures_names;
		public Pointer off_ui_textures_width;
		public Pointer off_ui_textures_height;
		public Pointer off_ui_textures_pageInfo;
		public Pointer off_ui_textures_palette;
		public Pointer off_ui_textures_xInPage;
		public Pointer off_ui_textures_yInPage;

		public Pointer off_178;
		public int int_17C;

		public Pointer off_180;
		public Pointer off_sector_meshes;
		public uint uint_188;

		public ushort ushort_18C;
		public ushort ushort_18E;
		public short short_190;
		public ushort ushort_192;
		public uint num_sectors;
		public Pointer off_sector_matrices;
		public uint uint_19C;
		public uint bad_off_1A0;
		public Pointer off_1A4;
		public ushort num_1A8;

		// Parsed
		public PointerList<UnkStruct1> states;

		protected override void ReadInternal(Reader reader) {
			reader.ReadBytes(0xF0);
			off_0F0 = Pointer.Read(reader);
			off_0F4 = Pointer.Read(reader);
			off_0F8 = Pointer.Read(reader);
			uint_0FC = reader.ReadUInt32();
			off_100 = Pointer.Read(reader); // pointer to struct in big array of structs of 0x18 size
			uint_104 = reader.ReadUInt32();
			uint_108 = reader.ReadUInt32();
			off_10C = Pointer.Read(reader);
			off_110 = Pointer.Read(reader);
			num_persos = reader.ReadUInt16();
			ushort_116 = reader.ReadUInt16();
			off_persos = Pointer.Read(reader);
			Perso[] persos = Load.ReadArray<Perso>(num_persos, reader, off_persos);
			Load.print(off_0F0 + " - " + off_0F4 + " - " + off_0F8 + " - " + off_100 + " - " + off_10C + " - " + off_110 + " - " + off_persos);
			off_states = Pointer.Read(reader);
			num_states = reader.ReadUInt16();
			ushort_122 = reader.ReadUInt16();
			states = Load.FromOffsetOrRead<PointerList<UnkStruct1>>(reader, off_states, s => s.length = num_states);
			uint_124 = reader.ReadUInt32();
			uint_128 = reader.ReadUInt32();
			off_12C = Pointer.Read(reader);
			off_130 = Pointer.Read(reader);
			uint_134 = reader.ReadUInt32();
			num_always = reader.ReadUInt32();
			uint_13C = reader.ReadUInt32(); // same as mainChar_states count
			uint_140 = reader.ReadUInt32();
			uint_144 = reader.ReadUInt32();
			int_148 = reader.ReadInt32(); // -1
			off_14C = Pointer.Read(reader);
			off_150 = Pointer.Read(reader); // big array of structs of 0x6 size. 3 ushorts per struct
			off_154 = Pointer.Read(reader);
			uint_158 = reader.ReadUInt32();
			Load.print(off_12C + " - " + off_130 + " - " + off_14C + " - " + off_150 + " - " + off_154);

			// Vignette stuff, big textures
			off_ui_textures_names = Pointer.Read(reader);
			off_ui_textures_width = Pointer.Read(reader); // num_vignettes * ushort
			off_ui_textures_height = Pointer.Read(reader); // num_vignettes * ushort
			off_ui_textures_pageInfo = Pointer.Read(reader); // num_vignettes * ushort
			off_ui_textures_palette = Pointer.Read(reader);
			off_ui_textures_xInPage = Pointer.Read(reader);
			off_ui_textures_yInPage = Pointer.Read(reader); // still related to the vignette stuff

			ParseUITextures(reader);

			Load.print(Pointer.Current(reader));
			// Something else
			off_178 = Pointer.Read(reader);
			int_17C = reader.ReadInt32(); // -1

			off_180 = Pointer.Read(reader); // big array of pointers, 2 pointers per thing
			off_sector_meshes = Pointer.Read(reader); // 2 x 0 uint, then y structs of 8
			uint_188 = reader.ReadUInt32(); // x things
			Load.print(off_178 + " - " + off_180 + " - " + off_sector_meshes + " - " + uint_188);

			ushort_18C = reader.ReadUInt16();
			ushort_18E = reader.ReadUInt16();
			short_190 = reader.ReadInt16();
			ushort_192 = reader.ReadUInt16();
			num_sectors = reader.ReadUInt32(); // y
			off_sector_matrices = Pointer.Read(reader); // y structs of 0x3c
			uint_19C = reader.ReadUInt32();
			bad_off_1A0 = reader.ReadUInt32(); //Pointer.Read(reader);
			off_1A4 = Pointer.Read(reader); // num_1A8 structs of 0x54
			num_1A8 = reader.ReadUInt16();
			Load.print(off_sector_matrices + " - " + bad_off_1A0 + " - " + off_1A4);

			ParseSectors(reader);
		}

		public void ParseUITextures(Reader reader) {
			uint num_ui_textures = (off_ui_textures_height.offset - off_ui_textures_width.offset) / 2;
			Load.print("Num UI Textures: " + num_ui_textures);

			UITexture[] textures = new UITexture[num_ui_textures];
			for (int i = 0; i < textures.Length; i++) {
				textures[i] = new UITexture();
			}
			Pointer.DoAt(ref reader, off_ui_textures_names, () => {
				for (int i = 0; i < textures.Length; i++) {
					Pointer p = Pointer.GetPointerAtOffset(Pointer.Current(reader));
					Pointer.DoAt(ref reader, p, () => {
						textures[i].name = reader.ReadString(0x1C);
					});
					reader.ReadUInt32();
					if (p == null) {
						Array.Resize(ref textures, i);
						break;
					}
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_width, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].width = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_height, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].height = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_pageInfo, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].pageInfo = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_palette, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].palette = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_xInPage, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].xInPage = reader.ReadByte();
				}
			});
			Pointer.DoAt(ref reader, off_ui_textures_yInPage, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].yInPage = reader.ReadByte();
				}
			});


			R2PS1Loader l = Load as R2PS1Loader;
			PS1VRAM vram = l.vram;
			for (int i = 0; i < textures.Length; i++) {
				UITexture t = textures[i];
				//Load.print(t.name + " - " + t.width + " - " + t.height + " - " + t.xInPage + " - " + t.yInPage);
				t.texture = vram.GetTexture(t.width, t.height, t.pageInfo, t.palette, PS1VRAM.PixelMode.Byte, t.xInPage, t.yInPage);
				Util.ByteArrayToFile(l.gameDataBinFolder + "ui_tex/" + t.name + ".png", t.texture.EncodeToPNG());
			}
		}

		public void ParseSectors(Reader reader) {
			uint count = num_sectors;
			Pointer[] sectorMeshPtrs = new Pointer[count];
			Pointer.DoAt(ref reader, off_sector_meshes, () => {
				reader.ReadUInt32();
				reader.ReadUInt32();
				for (int i = 0; i < count; i++) {
					reader.ReadUInt32();
					sectorMeshPtrs[i] = Pointer.Read(reader);
				}
			});
			PS1StaticGeometricObject[] ipos = new PS1StaticGeometricObject[count];
			for (int i = 0; i < count; i++) {
				ipos[i] = Load.FromOffsetOrRead<PS1StaticGeometricObject>(reader, sectorMeshPtrs[i]);
			}
		}

		public class UITexture {
			public string name;
			public ushort width;
			public ushort height;
			public ushort pageInfo;
			public ushort palette;
			public byte xInPage;
			public byte yInPage;

			public Texture2D texture;
		}
	}
}
