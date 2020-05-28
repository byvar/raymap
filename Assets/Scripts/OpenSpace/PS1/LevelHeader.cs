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
		public uint num_geometricObjectsDynamic_cine;

		public Pointer off_dynamicWorld;
		public Pointer off_fatherSector;
		public Pointer off_inactiveDynamicWorld;
		public uint num_always;
		public Pointer off_always;
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
		public uint uint_138; // usually 0x28
		public uint uint_13C;
		public uint uint_140;
		public uint uint_144;
		public int initialStreamID; // ID of first cutscene stream upon entering level
		public Pointer off_animPositions;
		public Pointer off_animRotations;
		public Pointer off_animScales;

		// UI Textures
		public byte num_ui_textures;
		public byte byte_159;
		public ushort ushort_15A;
		public Pointer off_ui_textures_names;
		public Pointer off_ui_textures_width;
		public Pointer off_ui_textures_height;
		public Pointer off_ui_textures_pageInfo;
		public Pointer off_ui_textures_palette;
		public Pointer off_ui_textures_xInPage;
		public Pointer off_ui_textures_yInPage;

		public Pointer off_178;
		public int int_17C;

		public Pointer off_geometricObjects_dynamic;
		public Pointer off_geometricObjects_static;
		public uint num_geometricObjects_dynamic;

		public ushort ushort_18C;
		public ushort ushort_18E;
		public short short_190;
		public ushort ushort_192;
		public uint num_ipos;
		public Pointer off_ipos;
		public uint uint_19C;
		public uint bad_off_1A0;
		public Pointer off_sectors;
		public ushort num_sectors;
		public ushort ushort_1AA;
		public uint uint_1AC;
		public uint uint_1B0;
		public uint uint_1B4;
		public uint uint_1B8;
		public Pointer off_1BC; // 0x28 structs of 0x38 size
		public Pointer off_1C0; // 0x28 structs of 0x68 size. starts with 0x5.
		public Pointer off_1C4; // b structs of 0x10
		public uint uint_1C8; // b
		public uint uint_1CC;
		public ushort ushort_1D0;
		public ushort ushort_1D2;
		public Pointer off_1D4; // same offset as the first pointer in the first struct of off_1C4
		public uint num_ago_textures;
		public Pointer off_ago_textures_pageInfo;
		public Pointer off_ago_textures_palette;
		public Pointer off_ago_textures_xInPage;
		public Pointer off_ago_textures_yInPage;
		public Pointer off_ago_textures_unk1;
		public Pointer off_ago_textures_unk2;
		public uint uint_1F4;
		public uint uint_1F8;
		public uint uint_1FC;

		// Rush
		public Pointer off_rush_114;
		public ushort ushort_rush_118;
		public ushort ushort_rush_11A;
		public Pointer off_ago_textures_width;
		public Pointer off_ago_textures_height;



		// Parsed
		public PointerList<State> states;
		public SuperObject fatherSector;
		public SuperObject dynamicWorld;
		public SuperObject inactiveDynamicWorld;
		public ObjectsTable geometricObjectsStatic;
		public ObjectsTable geometricObjectsDynamic;
		public Perso[] persos;
		public AlwaysList[] always;
		public Sector[] sectors;

		public PS1AnimationVector[] animPositions;
		public PS1AnimationQuaternion[] animRotations;
		public PS1AnimationVector[] animScales;

		protected override void ReadInternal(Reader reader) {
			R2PS1Loader l = Load as R2PS1Loader;
			if (Settings.s.game == Settings.Game.RRush) {
				reader.ReadBytes(0x40);
			} else if (Settings.s.game == Settings.Game.DD) {
				reader.ReadBytes(0x58);
			} else if (Settings.s.game == Settings.Game.VIP) {
				reader.ReadBytes(0x28);
			} else if(Settings.s.game == Settings.Game.JungleBook) {
				reader.ReadBytes(0xEC);
			} else {
				reader.ReadBytes(0xCC);
				num_geometricObjectsDynamic_cine = reader.ReadUInt32();
				reader.ReadBytes(0x20); // after this we're at 0xf0
			}
			off_dynamicWorld = Pointer.Read(reader);
			off_fatherSector = Pointer.Read(reader);
			off_inactiveDynamicWorld = Pointer.Read(reader);
			num_always = reader.ReadUInt32();
			off_always = Pointer.Read(reader); //
			uint_104 = reader.ReadUInt32(); // x
			uint_108 = reader.ReadUInt32();
			off_10C = Pointer.Read(reader); // x structs of 0x14
			off_110 = Pointer.Read(reader);
			num_persos = reader.ReadUInt16();
			ushort_116 = reader.ReadUInt16();
			off_persos = Pointer.Read(reader);
			Load.print(off_dynamicWorld + " - " + off_fatherSector + " - " + off_inactiveDynamicWorld + " - " + off_always + " - " + off_10C + " - " + off_110 + " - " + off_persos);
			off_states = Pointer.Read(reader);
			num_states = reader.ReadUInt16();
			ushort_122 = reader.ReadUInt16();
			uint_124 = reader.ReadUInt32();
			uint_128 = reader.ReadUInt32();
			off_12C = Pointer.Read(reader); // this + 0x10 = main character
			off_130 = Pointer.Read(reader);
			uint_134 = reader.ReadUInt32();
			uint_138 = reader.ReadUInt32();
			uint_13C = reader.ReadUInt32(); // same as mainChar_states count
			uint_140 = reader.ReadUInt32();
			uint_144 = reader.ReadUInt32();
			initialStreamID = reader.ReadInt32(); // -1
			Load.print(Pointer.Current(reader));
			off_animPositions = Pointer.Read(reader); // 0x6 size
			off_animRotations = Pointer.Read(reader); // big array of structs of 0x8 size. 4 ushorts per struct
			off_animScales = Pointer.Read(reader);
			Load.print(off_12C + " - " + off_130 + " - " + off_animPositions + " - " + off_animRotations + " - " + off_animScales);
			Load.print(Pointer.Current(reader));
			if (Settings.s.game == Settings.Game.DD) {
				// Also stuff for big textures, but names and amount is stored in exe
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				reader.ReadUInt32();
				reader.ReadUInt32();
				Pointer.Read(reader);

				off_geometricObjects_dynamic = Pointer.Read(reader);
				num_geometricObjects_dynamic = reader.ReadUInt32();
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				Pointer.Read(reader);
				reader.ReadUInt32();
				off_geometricObjects_static = Pointer.Read(reader);
				reader.ReadUInt32();
			} else if (Settings.s.game == Settings.Game.VIP || Settings.s.game == Settings.Game.JungleBook) {
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				off_geometricObjects_dynamic = Pointer.Read(reader);
				off_geometricObjects_static = Pointer.Read(reader);
				num_geometricObjects_dynamic = reader.ReadUInt32();
			} else {
				// Vignette stuff, big textures
				num_ui_textures = reader.ReadByte();
				byte_159 = reader.ReadByte();
				ushort_15A = reader.ReadUInt16();
				off_ui_textures_names = Pointer.Read(reader);
				off_ui_textures_width = Pointer.Read(reader); // num_vignettes * ushort
				off_ui_textures_height = Pointer.Read(reader); // num_vignettes * ushort
				off_ui_textures_pageInfo = Pointer.Read(reader); // num_vignettes * ushort
				off_ui_textures_palette = Pointer.Read(reader);
				off_ui_textures_xInPage = Pointer.Read(reader);
				off_ui_textures_yInPage = Pointer.Read(reader); // still related to the vignette stuff

				ParseUITextures(reader);

				// Something else
				off_178 = Pointer.Read(reader);
				int_17C = reader.ReadInt32(); // -1

				off_geometricObjects_dynamic = Pointer.Read(reader);
				off_geometricObjects_static = Pointer.Read(reader);
				num_geometricObjects_dynamic = reader.ReadUInt32();
			}

			ushort_18C = reader.ReadUInt16();
			ushort_18E = reader.ReadUInt16();
			short_190 = reader.ReadInt16();
			ushort_192 = reader.ReadUInt16();
			num_ipos = reader.ReadUInt32(); // y
			off_ipos = Pointer.Read(reader); // y structs of 0x3c
			uint_19C = reader.ReadUInt32();
			bad_off_1A0 = reader.ReadUInt32(); //Pointer.Read(reader);
			off_sectors = Pointer.Read(reader); // num_1A8 structs of 0x54
			num_sectors = reader.ReadUInt16(); // actual sectors
											   //Load.print(off_sector_minus_one_things + " - " + bad_off_1A0 + " - " + off_sectors);

			if (Settings.s.game == Settings.Game.R2 || Settings.s.game == Settings.Game.RRush) {
				ushort_1AA = reader.ReadUInt16();
				uint_1AC = reader.ReadUInt32();
				uint_1B0 = reader.ReadUInt32();
				uint_1B4 = reader.ReadUInt32();
				uint_1B8 = reader.ReadUInt32();
				off_1BC = Pointer.Read(reader);
				off_1C0 = Pointer.Read(reader);

				if (Settings.s.game == Settings.Game.RRush) {
					off_rush_114 = Pointer.Read(reader);
					ushort_rush_118 = reader.ReadUInt16();
					ushort_rush_11A = reader.ReadUInt16();
				}

				off_1C4 = Pointer.Read(reader);
				uint_1C8 = reader.ReadUInt32();
				uint_1CC = reader.ReadUInt32();
				ushort_1D0 = reader.ReadUInt16();
				ushort_1D2 = reader.ReadUInt16();
				off_1D4 = Pointer.Read(reader);
				num_ago_textures = reader.ReadUInt32();
				off_ago_textures_pageInfo = Pointer.Read(reader);
				off_ago_textures_palette = Pointer.Read(reader);
				off_ago_textures_xInPage = Pointer.Read(reader);
				off_ago_textures_yInPage = Pointer.Read(reader);
				off_ago_textures_unk1 = Pointer.Read(reader);
				off_ago_textures_unk2 = Pointer.Read(reader);
				if (Settings.s.game == Settings.Game.RRush) {
					off_ago_textures_width = Pointer.Read(reader);
					off_ago_textures_height = Pointer.Read(reader);
					Load.print(Pointer.Current(reader));
					ParseAGOTextures(reader);
				} else {
					uint_1F4 = reader.ReadUInt32();
					uint_1F8 = reader.ReadUInt32();
					uint_1FC = reader.ReadUInt32();
					//ParseAGOTextures(reader);
				}
			}

			// Parse
			states = Load.FromOffsetOrRead<PointerList<State>>(reader, off_states, s => s.length = num_states);
			persos = Load.ReadArray<Perso>(num_persos, reader, off_persos);
			geometricObjectsDynamic = Load.FromOffsetOrRead<ObjectsTable>(reader, off_geometricObjects_dynamic, onPreRead: t => t.length = num_geometricObjects_dynamic - 2);
			geometricObjectsStatic = Load.FromOffsetOrRead<ObjectsTable>(reader, off_geometricObjects_static, onPreRead: t => {
				if (Settings.s.game == Settings.Game.R2) t.length = num_ipos;
			});
			always = Load.ReadArray<AlwaysList>(num_always, reader, off_always);
			sectors = Load.ReadArray<Sector>(num_sectors, reader, off_sectors);

			animPositions = Load.ReadArray<PS1AnimationVector>((off_animRotations.offset - off_animPositions.offset) / 6, reader, off_animPositions);
			animRotations = Load.ReadArray<PS1AnimationQuaternion>((off_animScales.offset - off_animRotations.offset) / 8, reader, off_animRotations);
			animScales = Load.ReadArray<PS1AnimationVector>(l.maxScaleVector[0] + 1, reader, off_animScales);
		}

		public void ParseUITextures(Reader reader) {
			Load.print("Num UI Textures: " + num_ui_textures);

			UITexture[] textures = new UITexture[num_ui_textures];
			for (int i = 0; i < textures.Length; i++) {
				textures[i] = new UITexture();
			}
			Pointer.DoAt(ref reader, off_ui_textures_names, () => {
				for (int i = 0; i < textures.Length; i++) {
					Pointer p = Pointer.Read(reader);
					Pointer.DoAt(ref reader, p, () => {
						textures[i].name = reader.ReadString(0x1C);
					});
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
				t.texture = vram.GetTexture(t.width, t.height, t.pageInfo, t.palette, t.xInPage, t.yInPage);
				Util.ByteArrayToFile(l.gameDataBinFolder + "textures/ui/" + t.name + ".png", t.texture.EncodeToPNG());
			}
		}
		public void ParseAGOTextures(Reader reader) {
			Load.print("Num AGO Textures: " + num_ago_textures);

			UITexture[] textures = new UITexture[num_ago_textures];
			for (int i = 0; i < textures.Length; i++) {
				textures[i] = new UITexture();
			}
			for (int i = 0; i < textures.Length; i++) {
				textures[i].name = "Tex_" + i;
			}
			Pointer.DoAt(ref reader, off_ago_textures_pageInfo, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].pageInfo = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ago_textures_palette, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].palette = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ago_textures_xInPage, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].xInPage = reader.ReadByte();
				}
			});
			Pointer.DoAt(ref reader, off_ago_textures_yInPage, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].yInPage = reader.ReadByte();
				}
			});
			Pointer.DoAt(ref reader, off_ago_textures_unk1, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].unk1 = reader.ReadUInt16();
				}
			});
			Pointer.DoAt(ref reader, off_ago_textures_unk2, () => {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].unk2 = reader.ReadUInt16();
				}
			});
			if (Settings.s.game == Settings.Game.RRush) {
				Pointer.DoAt(ref reader, off_ago_textures_width, () => {
					for (int i = 0; i < textures.Length; i++) {
						textures[i].width = reader.ReadUInt16();
					}
				});
				Pointer.DoAt(ref reader, off_ago_textures_height, () => {
					for (int i = 0; i < textures.Length; i++) {
						textures[i].height = reader.ReadUInt16();
					}
				});
			} else {
				for (int i = 0; i < textures.Length; i++) {
					textures[i].width = (ushort)(((textures[i].unk1 & 0x0080) != 0) ? 0x40 : 0x20);
					textures[i].height = textures[i].width;
				}
			}


			R2PS1Loader l = Load as R2PS1Loader;
			PS1VRAM vram = l.vram;
			for (int i = 0; i < textures.Length; i++) {
				UITexture t = textures[i];
				//Load.print(t.name + " - " + t.width + " - " + t.height + " - " + t.xInPage + " - " + t.yInPage);
				t.texture = vram.GetTexture(t.width, t.height, t.pageInfo, t.palette, t.xInPage, t.yInPage);
				Util.ByteArrayToFile(l.gameDataBinFolder + "textures/ago/" + Load.lvlName + "/" + t.name + ".png", t.texture.EncodeToPNG());
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

			public ushort unk1;
			public ushort unk2;

			public Texture2D texture;
		}
	}
}
