using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.Collide;
using OpenSpace.Object;
using OpenSpace.FileFormat;
using OpenSpace.FileFormat.Texture;
using OpenSpace.Input;
using OpenSpace.Text;
using OpenSpace.Visual;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using OpenSpace.Object.Properties;
using System.Collections;
using System.Text.RegularExpressions;
using lzo.net;
using System.IO.Compression;

namespace OpenSpace.Loader {
	public class R2DSLoader : MapLoader {
		public DSBIN data;
		public DSBIN fat;
		public DSFATTable[] fatTables;
		public int currentLevel = 0;

		public override IEnumerator Load() {
			try {
				if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
				if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
				globals = new Globals();
				gameDataBinFolder += "/";
				yield return controller.StartCoroutine(FileSystem.CheckDirectory(gameDataBinFolder));
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
				loadingState = "Initializing files";
				files_array[SMem.Data] = new DSBIN("data.bin", gameDataBinFolder + "data.bin", SMem.Data);
				files_array[SMem.Fat] = new DSBIN("fat.bin", gameDataBinFolder + "fat.bin", SMem.Fat);

				yield return controller.StartCoroutine(LoadFat());

				/*for (int i = 0; i < fatTables[5 + 2].entries.Length; i++) {
					print(fatTables[5 + 2].entries[i].type + " - " + fatTables[5 + 2].entries[i].index + " - " + String.Format("0x{0:X8}", fatTables[5 + 2].entries[i].off_data));
				}*/

				yield return controller.StartCoroutine(LoadData());

				/*List<DSFATEntry> entries = new List<DSFATEntry>();
				for (int i = 0; i < fatTables.Length; i++) {
					for (int j = 0; j < fatTables[i].entries.Length; j++) {
						entries.Add(fatTables[i].entries[j]);
					}
				}
				entries.Sort((a,b) => (a.off_data.CompareTo(b.off_data)));*/
				/*IEnumerable<KeyValuePair<DSFATEntry, int>> groups = entries.GroupBy(e => e.type).Select(g => new KeyValuePair<DSFATEntry, int>(g.First(e1 => e1.unk1 == g.Max(e2 => e2.unk1)), g.Count()));
				foreach (KeyValuePair<DSFATEntry, int> g in groups) {
					print("Type: " + g.Key.type + " - Unk1: " + g.Key.unk1 + " - Amount: " + g.Value);
				}*/
				/*for (int i = 0; i < entries.Count; i++) {
					DSFATEntry entry = entries[i];
					uint nextOffset;
					if (i < entries.Count - 1) {
						nextOffset = entries[i + 1].off_data;
					} else {
						nextOffset = (uint)data.reader.BaseStream.Length;
					}
					uint size = nextOffset - entry.off_data;
					if (entry.EntryType == DSFATEntry.Type.Text) {
						Pointer off = new Pointer(entry.off_data, data);
						Pointer.DoAt(ref reader, off, () => {
							string bytes = reader.ReadNullDelimitedString();*/
							//print(entry.unk1 + " - " + bytes);
							/*string name = gameDataBinFolder + "ext/" + "t" + entry.tableIndex + " _e" + entry.entryIndex + "_ " + String.Format("0x{0:X8}", entry.off_data) + ".bin";
							Util.ByteArrayToFile(name, bytes);*/
				/*		});
					}
				}*/
			} finally {
				for (int i = 0; i < files_array.Length; i++) {
					if (files_array[i] != null) {
						files_array[i].Dispose();
					}
				}
				if (cnt != null) cnt.Dispose();
			}
			yield return null;
			InitModdables();
		}
		
		public IEnumerator LoadFat() {
			data = files_array[SMem.Data] as DSBIN;
			fat = files_array[SMem.Fat] as DSBIN;
			Reader reader = files_array[SMem.Fat].reader;

			uint num_tables = reader.ReadUInt32();
			fatTables = new DSFATTable[num_tables + 2];
			for (uint i = 0; i < num_tables + 2; i++) {
				fatTables[i] = DSFATTable.Read(reader, Pointer.Current(reader));
				foreach (DSFATEntry e in fatTables[i].entries) {
					e.tableIndex = i;
				}
			}
			yield return null;
		}

		public IEnumerator LoadData() {
			data = files_array[SMem.Data] as DSBIN;
			Reader reader = files_array[SMem.Data].reader;

			if (exportTextures) {
				string state = loadingState;
				loadingState = "Exporting textures";
				yield return null;
				ExportTextures(reader);
				loadingState = state;
				yield return null;
			}
			reader.ReadUInt16();
			reader.ReadUInt16();
			ushort num_textureTables = reader.ReadUInt16(); // for texture data. not referenced in fat.bin
			reader.ReadUInt16();
			ushort num_levels = reader.ReadUInt16();
			reader.ReadUInt16();

			// Read fix texture list
			Pointer off_engineStruct = GetStructPtr(DSFATEntry.Type.EngineStructure, 0x8000);
			Pointer.DoAt(ref reader, off_engineStruct, () => {
				reader.ReadBytes(12);
				ushort ind_shadowTexture = reader.ReadUInt16();
				LoadTexture(reader, ind_shadowTexture);
				reader.ReadUInt16();
				ushort ind_characterMaterial = reader.ReadUInt16();
				Pointer off_visualMaterial = GetStructPtr(DSFATEntry.Type.VisualMaterial, ind_characterMaterial);
				Pointer.DoAt(ref reader, off_visualMaterial, () => {
					reader.ReadBytes(12);
					ushort ind_vmTextureList = reader.ReadUInt16();
					ushort num_vmTextureList = reader.ReadUInt16();
					Pointer off_textureList = GetStructPtr(DSFATEntry.Type.VisualMaterialTextures, ind_vmTextureList);
					Pointer.DoAt(ref reader, off_textureList, () => {
						for (ushort i = 0; i < num_vmTextureList; i++) {
							ushort ind_texture = reader.ReadUInt16();
							ushort map_id = reader.ReadUInt16();
							LoadTexture(reader, ind_texture);
						}
					});
				});
				ushort ind_noCtrlTextureList = reader.ReadUInt16();
				Pointer off_noCtrlTextureList = GetStructPtr(DSFATEntry.Type.NoCtrlTextureList, ind_noCtrlTextureList);
				Pointer.DoAt(ref reader, off_noCtrlTextureList, () => {
					for (int i = 0; i < 5; i++) {
						ushort ind_fixTexRef = reader.ReadUInt16();
						LoadTexture(reader, ind_fixTexRef);
					}
				});
			});

			// Read languages table
			Pointer off_numLang = GetStructPtr(DSFATEntry.Type.NumLanguages, 0);
			Pointer.DoAt(ref reader, off_numLang, () => {
				ushort num_languages = reader.ReadUInt16();
				print("Number of languages: " + num_languages);
				for (ushort i = 0; i < num_languages; i++) {
					Pointer off_lang = GetStructPtr(DSFATEntry.Type.LanguageTable, i);
					Pointer.DoAt(ref reader, off_lang, () => {
						ushort ind_txtTable = reader.ReadUInt16();
						ushort num_txtTable = reader.ReadUInt16();
						ushort ind_136Table = reader.ReadUInt16();
						ushort num_136Table = reader.ReadUInt16();
						reader.ReadUInt16();
						string name = reader.ReadString(0x12);
						print(name);
						Pointer off_txtTable = GetStructPtr(DSFATEntry.Type.TextTable, ind_txtTable);
						Pointer.DoAt(ref reader, off_txtTable, () => {
							for (ushort j = 0; j < num_txtTable; j++) {
								ushort ind_strRef = reader.ReadUInt16();
								Pointer off_strRef = GetStructPtr(DSFATEntry.Type.StringReference, ind_strRef);
								Pointer.DoAt(ref reader, off_strRef, () => {
									ushort strLen = reader.ReadUInt16();
									ushort ind_str = reader.ReadUInt16();
									Pointer off_str = GetStructPtr(DSFATEntry.Type.String, ind_str);
									Pointer.DoAt(ref reader, off_str, () => {
										string str = reader.ReadString(strLen);
										print(str);
									});
								});
							}
						});
					});
				}
			});

			yield return null;
		}

		public void ExportTextures(Reader reader) {
			// Textures in data.bin
			for (int i = 0; i < fatTables.Length; i++) {
				for (int j = 0; j < fatTables[i].entries.Length; j++) {
					if (fatTables[i].entries[j].EntryType != DSFATEntry.Type.TextureInfo) continue;
					Pointer ptr = new Pointer(fatTables[i].entries[j].off_data, files_array[SMem.Data]);
					Pointer.DoAt(ref reader, ptr, () => {
						ReadTextureInfo(reader, true);
					});
				}
			}
			// Stored separately
			for (int i = 1; i < 25; i++) {
				ExportEtcFile("LoadingAnimation/Course_" + i.ToString("D2") + ".etc", 64, 64, false);
			}
			ExportEtcFile("LoadingAnimation/home.etc", 64, 64, true);
			foreach (string file in Directory.EnumerateFiles(gameDataBinFolder + "/vignette")) {
				string fileName = file.Substring((gameDataBinFolder + "/vignette\\").Length);
				ExportEtcFile("vignette/" + fileName, 512, 256, false);
			}
		}

		public void LoadTexture(Reader reader, ushort ind_texRef) {
			Pointer off_texRef = GetStructPtr(DSFATEntry.Type.TextureInfoReference, ind_texRef);
			Pointer.DoAt(ref reader, off_texRef, () => {
				ushort ind_texInfo = reader.ReadUInt16();
				Pointer off_texInfo = GetStructPtr(DSFATEntry.Type.TextureInfo, ind_texInfo);
				Pointer.DoAt(ref reader, off_texInfo, () => {
					ReadTextureInfo(reader, false);
				});
			});
		}

		public void ReadTextureInfo(Reader reader, bool export) {
			if (Settings.s.platform == Settings.Platform._3DS) {
				byte wExponent = reader.ReadByte();
				byte hExponent = reader.ReadByte();
				reader.ReadUInt16();
				reader.ReadUInt16();
				ushort size = reader.ReadUInt16();
				ushort bitdepth = reader.ReadUInt16();
				string name = reader.ReadString(200);
				byte[] textureBytes = reader.ReadBytes(size); // max size: 0x10000
				if (export) {
					if (!File.Exists(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Texture2D tex = ParseTexture(textureBytes, 1 << wExponent, 1 << hExponent, bitdepth == 32);
						Util.ByteArrayToFile(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", tex.EncodeToPNG());
					}
				} else {
					print(name);
				}
			}
		}

		public void ExportEtcFile(string name, int w, int h, bool hasAlpha) {
			if (Settings.s.platform == Settings.Platform._3DS) {
				using (Reader reader = new Reader(FileSystem.GetFileReadStream(gameDataBinFolder + name))) {
					byte[] textureBytes = reader.ReadBytes((int)reader.BaseStream.Length);
					if (!File.Exists(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png")) {
						Texture2D tex = ParseTexture(textureBytes, w, h, hasAlpha);
						Util.ByteArrayToFile(gameDataBinFolder + "/textures/" + Path.GetDirectoryName(name) + "/" + Path.GetFileNameWithoutExtension(name) + ".png", tex.EncodeToPNG());
					}
				}
			}
		}

		private Texture2D ParseTexture(byte[] bytes, int width, int height, bool hasAlpha) {
			// it's ETC1AlphaRGB8A4NativeDMP if bitdepth = 32 else ETC1RGB8NativeDMP
			// https://github.com/xdanieldzd/UntoldUnpack/blob/master/UntoldUnpack/Graphics/TileCodecs.cs
			return UntoldUnpack.Graphics.Texture.ToTexture2D(
				UntoldUnpack.Graphics.PicaDataTypes.UnsignedByte,
				hasAlpha ? UntoldUnpack.Graphics.PicaPixelFormats.ETC1AlphaRGB8A4NativeDMP : UntoldUnpack.Graphics.PicaPixelFormats.ETC1RGB8NativeDMP,
				width,
				height,
				new MemoryStream(bytes));


			// https://gist.github.com/smealum/8807124
			// https://github.com/Sage-of-Mirrors/Green-Eclipse/wiki/CTXB-Specification ?
			// https://wiki.cloudmodding.com/oot/3D:CMB_format
		}

		/*private uint gpuTextureIndex(uint x, uint y, uint w, uint h) {
			return (((y >> 3) * (w >> 3) + (x >> 3)) << 6) + ((x & 1) | ((y & 1) << 1) | ((x & 2) << 1) | ((y & 2) << 2) | ((x & 4) << 2) | ((y & 4) << 3));
		}*/

		public DSFATEntry GetEntry(ushort type, ushort index) {
			type = (ushort)(type & 0x7FFF);
			index = (ushort)(index & 0x7FFF);

			DSFATEntry levelEntry = fatTables[currentLevel + 2].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (levelEntry != null) return levelEntry;

			DSFATEntry fix2Entry = fatTables[1].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (fix2Entry != null) return fix2Entry;

			DSFATEntry fixEntry = fatTables[0].entries.FirstOrDefault(e => e.type == type && e.index == index);
			if (fixEntry != null) return fixEntry;

			return null;
		}

		public DSFATEntry GetEntry(DSFATEntry.Type type, ushort index) {
			index = (ushort)(index & 0x7FFF);

			DSFATEntry levelEntry = fatTables[currentLevel + 2].entries.FirstOrDefault(e => e.EntryType == type && e.index == index);
			if (levelEntry != null) return levelEntry;

			DSFATEntry fix2Entry = fatTables[1].entries.FirstOrDefault(e => e.EntryType == type && e.index == index);
			if (fix2Entry != null) return fix2Entry;

			DSFATEntry fixEntry = fatTables[0].entries.FirstOrDefault(e => e.EntryType == type && e.index == index);
			if (fixEntry != null) return fixEntry;

			return null;
		}

		public Pointer GetStructPtr(ushort type, ushort index) {
			DSFATEntry entry = GetEntry(type, index);
			if (entry != null) {
				return new Pointer(entry.off_data, files_array[SMem.Data]);
			} else {
				return null;
			}
		}

		public Pointer GetStructPtr(DSFATEntry.Type type, ushort index) {
			DSFATEntry entry = GetEntry(type, index);
			if (entry != null) {
				return new Pointer(entry.off_data, files_array[SMem.Data]);
			} else {
				return null;
			}
		}
	}

	public class DSFATEntry {
		public Pointer offset;
		public uint off_data;
		public ushort type;
		public ushort index;

		// Calculated
		public uint tableIndex;
		public uint entryIndexWithinTable;
		public uint size;

		public static DSFATEntry Read(Reader reader, Pointer offset) {
			DSFATEntry entry = new DSFATEntry();
			entry.offset = offset;
			entry.off_data = reader.ReadUInt32();
			entry.type = reader.ReadUInt16();
			entry.index = reader.ReadUInt16();
			return entry;
		}

		public static Dictionary<ushort, Type> TypesDS = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStructure },
			{ 5, Type.ObjectListEntry }, // ?
			{ 17, Type.TextureInfoReference }, // size: 0x2
			{ 34, Type.VisualMaterial },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 63, Type.TextureInfo }, // size: 14
			{ 91, Type.NoCtrlTextureList },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringReference },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> TypesN64 = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStructure },
			{ 5, Type.ObjectListEntry }, // ?
			{ 17, Type.TextureInfoReference }, // size: 0x2
			{ 34, Type.VisualMaterial },
			{ 43, Type.LevelList },
			{ 44, Type.LevelHeader },
			{ 63, Type.TextureInfo }, // size: 14
			{ 91, Type.NoCtrlTextureList },
			{ 132, Type.NumLanguages },
			{ 133, Type.StringReference },
			{ 134, Type.LanguageTable },
			{ 135, Type.TextTable },
			{ 136, Type.Language_136 },
			{ 137, Type.Language_137 },
			{ 156, Type.VisualMaterialTextures },
			{ 157, Type.String },
		};

		public static Dictionary<ushort, Type> Types3DS = new Dictionary<ushort, Type>() {
			{ 0, Type.EngineStructure },
			{ 17, Type.TextureInfoReference }, // size: 0x2
			{ 30, Type.VisualMaterial },
			{ 39, Type.LevelList },
			{ 40, Type.LevelHeader },
			{ 59, Type.TextureInfo }, // size: 0x100D2. contains the actual texture data too!
			{ 87, Type.NoCtrlTextureList },
			{ 128, Type.NumLanguages },
			{ 129, Type.StringReference },
			{ 130, Type.LanguageTable },
			{ 131, Type.TextTable },
			{ 132, Type.Language_136 },
			{ 133, Type.Language_137 },
			{ 152, Type.VisualMaterialTextures },
			{ 153, Type.String },
		};

		public enum Type {
			Unknown,
			EngineStructure,
			ObjectListEntry, // Size: 0x14
			LevelList, // Size: 0x40 * num_levels (0x46 or 70 in Rayman 2)
			LevelHeader, // Size: 0x38
			NumLanguages,
			LanguageTable,
			TextTable,
			StringReference,
			String,
			Language_136,
			Language_137,
			TextureInfo,
			TextureInfoReference,
			VisualMaterialTextures,
			NoCtrlTextureList,
			VisualMaterial,

		}

		public Type EntryType {
			get {
				Dictionary<ushort, Type> dict = null;
				switch (Settings.s.platform) {
					case Settings.Platform._3DS: dict = Types3DS; break;
					case Settings.Platform.N64: dict = TypesN64; break;
					default: dict = TypesDS; break;
				}
				if (dict.ContainsKey(type)) {
					return dict[type];
				} else return Type.Unknown;
			}
		}
	}

	public class DSFATTable {
		public Pointer offset;
		public Pointer off_table;
		public uint num_entries;
		public DSFATEntry[] entries;

		public static DSFATTable Read(Reader reader, Pointer offset) {
			DSFATTable t = new DSFATTable();
			t.offset = offset;
			t.off_table = Pointer.Read(reader);
			t.num_entries = reader.ReadUInt32();
			t.entries = new DSFATEntry[t.num_entries];
			Pointer.DoAt(ref reader, t.off_table, () => {
				for (int i = 0; i < t.entries.Length; i++) {
					t.entries[i] = DSFATEntry.Read(reader, Pointer.Current(reader));
					t.entries[i].entryIndexWithinTable = (uint)i;
				}
			});
			return t;
		}
	}


	public static class SMem {
		public const int Data = 0;
		public const int Fat = 1;
	}
}
