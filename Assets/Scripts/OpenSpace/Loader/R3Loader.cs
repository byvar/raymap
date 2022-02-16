using BinarySerializer.Unity;
using Cysharp.Threading.Tasks;
using OpenSpace.Animation;
using OpenSpace.Cinematics;
using OpenSpace.FileFormat;
using OpenSpace.Input;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Text;
using OpenSpace.Visual;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.Loader {
	public class R3Loader : MapLoader {
		protected override async UniTask Load() {
			try {
				if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
				if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
				globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");

				loadingState = "Initializing files";
				await CreateCNT();

				if (lvlName.EndsWith(".exe")) {
					if (!Legacy_Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
					Legacy_Settings.s.loadFromMemory = true;
					MemoryFile mem = new MemoryFile(lvlName);
					files_array[0] = mem;
					await WaitIfNecessary();
					await LoadMemory();
				} else {
					// Prepare paths
					string fixFolder = gameDataBinFolder;
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Legacy_Settings.CapsType.LevelFolder);

					paths["fix.lvl"] = fixFolder + ConvertCase("Fix.lvl", Legacy_Settings.CapsType.Fix);
					paths["fix.ptr"] = fixFolder + ConvertCase("Fix.ptr", Legacy_Settings.CapsType.Fix);
					paths["lvl.lvl"] = lvlFolder + ConvertCase(lvlName + ".lvl", Legacy_Settings.CapsType.LevelFile);
					paths["lvl.ptr"] = lvlFolder + ConvertCase(lvlName + ".ptr", Legacy_Settings.CapsType.LevelFile);
					paths["transit.lvl"] = lvlFolder + ConvertCase("transit.lvl", Legacy_Settings.CapsType.LevelFile);
					paths["transit.ptr"] = lvlFolder + ConvertCase("transit.ptr", Legacy_Settings.CapsType.LevelFile);
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.GC) {
						paths["menu.tpl"] = fixFolder + ConvertCase("menu.tpl", Legacy_Settings.CapsType.Fix);
						paths["fix.tpl"] = fixFolder + ConvertCase((Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGC) ? "../common.tpl" : "Fix.tpl", Legacy_Settings.CapsType.Fix);
						paths["lvl.tpl"] = lvlFolder + ConvertCase(lvlName + (Legacy_Settings.s.game == Legacy_Settings.Game.R3 ? "_Lvl" : "") + ".tpl", Legacy_Settings.CapsType.TextureFile);
						paths["transit.tpl"] = lvlFolder + ConvertCase(lvlName + "_Trans.tpl", Legacy_Settings.CapsType.TextureFile);
					} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox) {
						paths["fix.btf"] = fixFolder + ConvertCase("Fix.btf", Legacy_Settings.CapsType.Fix);
						paths["fix.bhf"] = fixFolder + ConvertCase("Fix.bhf", Legacy_Settings.CapsType.Fix);
						paths["lvl.btf"] = lvlFolder + ConvertCase(lvlName + ".btf", Legacy_Settings.CapsType.TextureFile);
						paths["lvl.bhf"] = lvlFolder + ConvertCase(lvlName + ".bhf", Legacy_Settings.CapsType.TextureFile);
						paths["transit.btf"] = lvlFolder + ConvertCase("transit.btf", Legacy_Settings.CapsType.TextureFile);
						paths["transit.bhf"] = lvlFolder + ConvertCase("transit.bhf", Legacy_Settings.CapsType.TextureFile);
					} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360) {
						paths["fix.btf"] = fixFolder + ConvertCase("Fix.btf", Legacy_Settings.CapsType.Fix);
						paths["fix.bhf"] = fixFolder + ConvertCase("Fix.bhf", Legacy_Settings.CapsType.Fix);
						paths["lvl.btf"] = lvlFolder + ConvertCase(lvlName + "_2.btf", Legacy_Settings.CapsType.TextureFile);
						paths["lvl.bhf"] = lvlFolder + ConvertCase(lvlName + "_2.bhf", Legacy_Settings.CapsType.TextureFile);
						paths["transit.btf"] = lvlFolder + ConvertCase("transit_6.btf", Legacy_Settings.CapsType.TextureFile);
						paths["transit.bhf"] = lvlFolder + ConvertCase("transit_6.bhf", Legacy_Settings.CapsType.TextureFile);
					} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						if (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM) {
							paths["fix.tbf"] = fixFolder + ConvertCase("Textures.txc", Legacy_Settings.CapsType.Fix);
						} else {
							paths["fix.tbf"] = fixFolder + ConvertCase("Fix.tbf", Legacy_Settings.CapsType.Fix);
						}
						paths["lvl.tbf"] = lvlFolder + ConvertCase(lvlName + ".tbf", Legacy_Settings.CapsType.TextureFile);
						paths["transit.tbf"] = lvlFolder + ConvertCase("transit.tbf", Legacy_Settings.CapsType.TextureFile);
					}
					paths["lvl_vb.lvl"] = lvlFolder + ConvertCase(lvlName + "_vb.lvl", Legacy_Settings.CapsType.LevelFile);
					paths["lvl_vb.ptr"] = lvlFolder + ConvertCase(lvlName + "_vb.ptr", Legacy_Settings.CapsType.LevelFile);
					paths["fixkf.lvl"] = fixFolder + ConvertCase("Fixkf.lvl", Legacy_Settings.CapsType.Fix);
					paths["fixkf.ptr"] = fixFolder + ConvertCase("Fixkf.ptr", Legacy_Settings.CapsType.Fix);
					paths["lvlkf.lvl"] = lvlFolder + ConvertCase(lvlName + "kf.lvl", Legacy_Settings.CapsType.LevelFile);
					paths["lvlkf.ptr"] = lvlFolder + ConvertCase(lvlName + "kf.ptr", Legacy_Settings.CapsType.LevelFile);

					// Download files
					/*foreach (KeyValuePair<string, string> path in paths) {
						if (path.Value != null) await PrepareFile(path.Value);
					}*/



					// Fix
					lvlNames[0] = "fix";
					lvlPaths[0] = paths["fix.lvl"];
					ptrPaths[0] = paths["fix.ptr"];
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.GC) {
						await PrepareFile(paths["fix.tpl"]);
						await PrepareFile(paths["menu.tpl"]);
					} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
						|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360) {
						await PrepareFile(paths["fix.btf"]);
						await PrepareFile(paths["fix.bhf"]);
					} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						await PrepareFile(paths["fix.tbf"]);
					}
					await PrepareFile(lvlPaths[0]);
					if (FileSystem.FileExists(lvlPaths[0])) {
						await PrepareFile(ptrPaths[0]);
					}

					// Level
					lvlNames[1] = lvlName;
					lvlPaths[1] = paths["lvl.lvl"];
					ptrPaths[1] = paths["lvl.ptr"];
					await PrepareFile(lvlPaths[1]);
					if (FileSystem.FileExists(lvlPaths[1])) {
						await PrepareFile(ptrPaths[1]);
						if (Legacy_Settings.s.platform == Legacy_Settings.Platform.GC) {
							await PrepareFile(paths["lvl.tpl"]);
						} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
						|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360) {
							await PrepareFile(paths["lvl.btf"]);
							await PrepareFile(paths["lvl.bhf"]);
						} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
							await PrepareFile(paths["lvl.tbf"]);
						}
					}

					// Transit
					lvlNames[2] = "transit";
					lvlPaths[2] = paths["transit.lvl"];
					ptrPaths[2] = paths["transit.ptr"];
					await PrepareFile(lvlPaths[2]);
					if (FileSystem.FileExists(lvlPaths[2])) {
						await PrepareFile(ptrPaths[2]);
						if (Legacy_Settings.s.platform == Legacy_Settings.Platform.GC) {
							await PrepareFile(paths["transit.tpl"]);
						} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
						|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360) {
							await PrepareFile(paths["transit.btf"]);
							await PrepareFile(paths["transit.bhf"]);
						} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
							await PrepareFile(paths["transit.tbf"]);
						}
					}
					hasTransit = FileSystem.FileExists(lvlPaths[2]) && (FileSystem.GetFileLength(lvlPaths[2]) > 4);

					// Vertex buffer
					lvlNames[4] = lvlName + "_vb";
					lvlPaths[4] = paths["lvl_vb.lvl"];
					ptrPaths[4] = paths["lvl_vb.ptr"];
					await PrepareFile(lvlPaths[4]);
					if (FileSystem.FileExists(lvlPaths[4])) {
						await PrepareFile(ptrPaths[4]);
					}

					// Fix Keyframes
					lvlNames[5] = "fixkf";
					lvlPaths[5] = paths["fixkf.lvl"];
					ptrPaths[5] = paths["fixkf.ptr"];
					await PrepareFile(lvlPaths[5]);
					if (FileSystem.FileExists(lvlPaths[5])) {
						await PrepareFile(ptrPaths[5]);
					}

					// Level Keyframes
					lvlNames[6] = lvlName + "kf";
					lvlPaths[6] = paths["lvlkf.lvl"];
					ptrPaths[6] = paths["lvlkf.ptr"];
					await PrepareFile(lvlPaths[6]);
					if (FileSystem.FileExists(lvlPaths[6])) {
						await PrepareFile(ptrPaths[6]);
					}

					for (int i = 0; i < lvlPaths.Length; i++) {
						if (lvlPaths[i] == null) continue;
						if (FileSystem.FileExists(lvlPaths[i])) {
							files_array[i] = new LVL(lvlNames[i], lvlPaths[i], i);
						}
					}
					for (int i = 0; i < loadOrder.Length; i++) {
						int j = loadOrder[i];
						if (files_array[j] != null && FileSystem.FileExists(ptrPaths[j])) {
							((LVL)files_array[j]).ReadPTR(ptrPaths[j]);
						}
					}
					// Export PS2 vignette textures
					if (exportTextures && Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
							ExportR3PS2Textures();
						} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RM || Legacy_Settings.s.game == Legacy_Settings.Game.RA) {
							ExportRAPS2Textures();
						}
					}

					await LoadFIX();
					await LoadLVL();
				}
			} finally {
				for (int i = 0; i < files_array.Length; i++) {
					if (files_array[i] != null) {
						if (!(files_array[i] is MemoryFile)) files_array[i].Dispose();
					}
				}
				if (cnt != null) cnt.Dispose();
			}
			await WaitIfNecessary();
			InitModdables();
		}
		void ExportRAPS2Textures() {
			ExportSingleFileTBF("../LSBIN/CODE", "TXR");
			ExportSingleFileTBF("../LSBIN/GLOBOX", "TXR");
			ExportSingleFileTBF("../LSBIN/HUNCH", "TXR");
			ExportSingleFileTBF("../LSBIN/HUNCH2", "TXR");
			ExportSingleFileTBF("../LSBIN/NOISE", "TXR");
			ExportSingleFileTBF("../LSBIN/PIRATEB", "TXR");
			ExportSingleFileTBF("../LSBIN/RAYMAN", "TXR");
			ExportSingleFileTBF("../LSBIN/RAZORW", "TXR");
			ExportSingleFileTBF("../LSBIN/TEENSIES", "TXR");
			ExportSingleFileTBF("../LSBIN/TILY", "TXR");
			ExportSingleFileTBF("../LSBIN/TVLOOK", "TXR");
			ExportSingleFileTBF("../VIG/RASTFNT", "TXR");
			ExportVIG("FLAGS1", "SCR", 512, 512);
			ExportVIG("RAYM", "SCR", 512, 512);
			ExportVIG("SONYDEMO", "SCR", 512, 512);
			ExportVIG("UBISOFT", "SCR", 512, 512);
			ExportVIG("RAYMLOGO", "SCR", 256, 256);
		}
		void ExportR3PS2Textures() {
			ExportSingleFileTBF("MENU/MNU_AB~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_BL~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_DE~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_DO~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_FO~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_JA~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_LA~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_MA~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_MO~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_OP~1", "TGA");
			ExportSingleFileTBF("MENU/MNU_ABRA", "TGA");
			ExportSingleFileTBF("MENU/MNU_ARCA", "TGA");
			ExportSingleFileTBF("MENU/MNU_BAD", "TGA");
			ExportSingleFileTBF("MENU/MNU_BLAC", "TGA");
			ExportSingleFileTBF("MENU/MNU_BONU", "TGA");
			ExportSingleFileTBF("MENU/MNU_BOUC", "TGA");
			ExportSingleFileTBF("MENU/MNU_DESE", "TGA");
			ExportSingleFileTBF("MENU/MNU_DONJ", "TGA");
			ExportSingleFileTBF("MENU/MNU_EXTR", "TGA");
			ExportSingleFileTBF("MENU/MNU_FILM", "TGA");
			ExportSingleFileTBF("MENU/MNU_FIX", "TGA");
			ExportSingleFileTBF("MENU/MNU_FLMS", "TGA");
			ExportSingleFileTBF("MENU/MNU_FORE", "TGA");
			ExportSingleFileTBF("MENU/MNU_FRT", "TGA");
			ExportSingleFileTBF("MENU/MNU_GAME", "TGA");
			ExportSingleFileTBF("MENU/MNU_GEN", "TGA");
			ExportSingleFileTBF("MENU/MNU_GLC", "TGA");
			ExportSingleFileTBF("MENU/MNU_GOOD", "TGA");
			ExportSingleFileTBF("MENU/MNU_JARD", "TGA");
			ExportSingleFileTBF("MENU/MNU_JEU", "TGA");
			ExportSingleFileTBF("MENU/MNU_LAND", "TGA");
			ExportSingleFileTBF("MENU/MNU_MARA", "TGA");
			ExportSingleFileTBF("MENU/MNU_MER", "TGA");
			ExportSingleFileTBF("MENU/MNU_MONT", "TGA");
			ExportSingleFileTBF("MENU/MNU_OK", "TGA");
			ExportSingleFileTBF("MENU/MNU_OPT", "TGA");
			ExportSingleFileTBF("MENU/MNU_OPTI", "TGA");
			ExportSingleFileTBF("MENU/MNU_PHTS", "TGA");
			ExportSingleFileTBF("MENU/MNU_SAV", "TGA");
			ExportSingleFileTBF("MENU/MNU_SECR", "TGA");
			ExportSingleFileTBF("MENU/MNU_STAR", "TGA");
			ExportSingleFileTBF("../LSBIN/BACK1", "TXR");
			ExportSingleFileTBF("../LSBIN/CAG_GUN", "TXR");
			ExportSingleFileTBF("../LSBIN/CODE", "TXR");
			ExportSingleFileTBF("../LSBIN/NOISE", "TXR");
			ExportSingleFileTBF("../LSBIN/TVLOOK", "TXR");
			ExportSingleFileTBF("../LSBIN/LODMECA", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS201", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS202", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS203", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS204", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS205", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS206", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS207", "TXR");
			ExportSingleFileTBF("../LSBIN/LODPS208", "TXR");
			ExportVIG("FRA/SEA_10", "RAW", 512, 512);
			ExportVIG("FRA/KNAAR_00", "RAW", 512, 512);
			ExportVIG("FRA/KNAAR_70", "RAW", 512, 512);
			ExportVIG("FRA/MOOR_10", "RAW", 512, 512);
			ExportVIG("FRA/SWAMP_WO", "RAW", 512, 512);
			ExportVIG("FLAGS1", "SCR", 512, 512);
			ExportVIG("FLAGS2", "SCR", 512, 512);
			ExportVIG("FLAGSN", "SCR", 512, 512);
			ExportVIG("FLAGSP", "SCR", 512, 512);
			ExportVIG("LOADING", "SCR", 512, 512);
			ExportVIG("SONYDEMO", "SCR", 512, 512);
			ExportVIG("UBISOFT", "SCR", 512, 512);
			ExportVIG("UBITEX", "SCR", 256, 256);
		}
		void ExportSingleFileTBF(string name, string ext) {
			string p = gameDataBinFolder + name + "." + ext;
			if (!FileSystem.FileExists(p)) return;
			FileFormat.Texture.TBF tbf = new FileFormat.Texture.TBF(p);
			Util.ByteArrayToFile(gameDataBinFolder + "textures/VIG/" + name + ".png", tbf.textures[0].EncodeToPNG());
		}
		void ExportVIG(string name, string extension, int width, int height) {
			string p = gameDataBinFolder + "VIG/" + name + "." + extension;
			if (!FileSystem.FileExists(p)) return;
			Texture2D tex = new Texture2D(width, height);
			using (Reader reader = new Reader(FileSystem.GetFileReadStream(p), true)) {
				for (int y = 0; y < height; y++) {
					for (int x = 0; x < width; x++) {
						byte r = reader.ReadByte();
						byte g = reader.ReadByte();
						byte b = reader.ReadByte();
						tex.SetPixel(x, height-1-y, new Color(r / 255f, g / 255f, b / 255f, 1f));
					}
				}
			}
			tex.Apply();
			Util.ByteArrayToFile(gameDataBinFolder + "textures/VIG/" + name + ".png", tex.EncodeToPNG());
		}

		#region FIX
		LegacyPointer off_animBankFix;

		async UniTask LoadFIX() {
			loadingState = "Loading fixed memory";
			await WaitIfNecessary();
			files_array[Mem.Fix].GotoHeader();
			Reader reader = files_array[Mem.Fix].reader;
			// Read fix header
			//reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3
					&& (Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_01
					&& Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_21)) {
					string timeStamp = reader.ReadString(0x18);
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					if (Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
						reader.ReadUInt32();
						reader.ReadUInt32();
					}
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RM || Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur ||Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						string timeStamp = reader.ReadString(0x14);
						reader.ReadUInt32();
						reader.ReadUInt32();
						reader.ReadUInt32();
						reader.ReadUInt32();
					}
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
			}
			LegacyPointer off_identityMatrix = LegacyPointer.Read(reader);
			loadingState = "Loading text";
			await WaitIfNecessary();
			localization = FromOffsetOrRead<LocalizationStructure>(reader, LegacyPointer.Current(reader), inline: true);
			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				uint num_lvlNames = reader.ReadUInt32();
				uint num_fixEntries1 = reader.ReadUInt32();
				// Read tables under header
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savName = new string(reader.ReadChars(0xC));
				}
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savMapName = new string(reader.ReadChars(0xC));
				}
				ReadLevelNames(reader, LegacyPointer.Current(reader), num_lvlNames);
				if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3) {
					reader.ReadChars(0x1E);
					reader.ReadChars(0x1E); // two zero entries
				}
				string firstMapName = new string(reader.ReadChars(0x1E));
				//print(firstMapName);
				if (reader.BaseStream.Position % 4 == 0) {
					reader.ReadUInt32();
				} else {
					reader.ReadUInt16();
				}
			} else {
				// PS2
				byte num_lvlNames = reader.ReadByte();
				byte num_fixEntries1 = reader.ReadByte();
				reader.ReadBytes(2); // padding
				ReadLevelNames(reader, LegacyPointer.Current(reader), num_lvlNames);
				string firstMapName = new string(reader.ReadChars(0x1E));
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savName = new string(reader.ReadChars(0xC));
				}
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savMapName = new string(reader.ReadChars(0xC));
				}
				if (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM || Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
					reader.Align(4);
				}
			}
			uint num_languages = reader.ReadUInt32();
			LegacyPointer off_languages = LegacyPointer.Read(reader);
			LegacyPointer.DoAt(ref reader, off_languages, () => {
				ReadLanguages(reader, off_languages, num_languages);
			});
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2 && localization != null && Legacy_Settings.s.game == Legacy_Settings.Game.R3 && Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
				for (int i = 0; i < localization.num_languages; i++) {
					if (localization.languages[i].off_textTable == null) {
						// Load text from file
						string filePath = ConvertCase("Texts/", Legacy_Settings.CapsType.LangLevelFolder);
						string fileName = "Lang" + i;
						loadingState = "Loading text files: " + (i + 1) + "/" + localization.num_languages;
						paths["lang" + i + ".lvl"] = gameDataBinFolder + ConvertCase(fileName + ".lvl", Legacy_Settings.CapsType.LangLevelFile);
						paths["lang" + i + ".ptr"] = gameDataBinFolder + ConvertCase(fileName + ".ptr", Legacy_Settings.CapsType.LangLevelFile);
						await PrepareFile(paths["lang" + i + ".lvl"]);
						if (FileSystem.FileExists(paths["lang" + i + ".lvl"])) {
							await PrepareFile(paths["lang" + i + ".ptr"]);
							int fileId = i + 207;
							FileWithPointers f = InitExtraLVL(fileName, paths["lang" + i + ".lvl"], paths["lang" + i + ".ptr"], fileId);
							LegacyPointer.DoAt(ref reader, new LegacyPointer(0, f), () => {

								string timeStamp = reader.ReadString(0x18);
								reader.ReadUInt32();
								reader.ReadUInt32();
								reader.ReadUInt32();
								reader.ReadUInt32();
								localization.ReadLanguageTablePS2(reader, i);
							});
						}
					}
				}
			}
			loadingState = "Loading fixed textures";
			print("Fix textures address: " + LegacyPointer.Current(reader));
			await ReadTexturesFix(reader, LegacyPointer.Current(reader));
			// Defaults for Rayman 3 PC. Sizes are hardcoded in the exes and might differ for versions too :/
			int sz_entryActions = 0x100;
			int sz_randomStructure = 0xDC;
			int sz_videoStructure = 0x18;
			int sz_musicMarkerSlot = 0x28;
			int sz_binDataForMenu = 0x020C;
			int num_menuPages = 35;

			if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3GC) {
				sz_entryActions = 0xE8;
				sz_binDataForMenu = 0x01F0;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGC
				|| Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07) {
				sz_entryActions = 0xC4;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaPC
				|| Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanMPC) {
				sz_entryActions = 0xDC;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.DinosaurPC) {
				sz_entryActions = 0xD8;
				sz_randomStructure = 0xE0;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.DonaldDuckPKGC) {
				sz_entryActions = 0xC0;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaXbox) {
				sz_entryActions = 0xF0;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PCDemo_2003_01_06) {
				sz_binDataForMenu = 0x1a4;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PCDemo_2002_12_09) {
				sz_binDataForMenu = 0x1ac;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PCDemo_2002_10_21) {
				sz_entryActions = 0xFC;
				sz_binDataForMenu = 0x1F4;
			} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PCDemo_2002_10_01) {
				sz_entryActions = 0xFC;
				sz_binDataForMenu = 0x10C;
				num_menuPages = 25;
			}
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				sz_videoStructure = 0x108;
				if (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM) {
					sz_entryActions = 0xE8;
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2DevBuild_2002_09_06) {
						sz_entryActions = 0xF8;
						sz_binDataForMenu = 0x78;
					} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
						sz_videoStructure = 0x18;
						sz_entryActions = 0xCC;
						sz_binDataForMenu = 0;
					} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_08_07) {
						sz_entryActions = 0xF8;
						sz_binDataForMenu = 0;
					} else if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_10_29) {
						sz_entryActions = 0x108; // probably not right but oh well
						sz_binDataForMenu = 0x1F4;
					} else {
						sz_entryActions = 0x10C; // probably not right but oh well
						sz_binDataForMenu = 0x1A4;
					}
				}
			} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox) {
				sz_videoStructure = 0x108;
				sz_binDataForMenu = 0x1AC;
			} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360) {
				sz_videoStructure = 0x108;
				sz_entryActions = 0x108;
				sz_binDataForMenu = 0x33C;
				num_menuPages = 96;
			} else if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3) {
				sz_videoStructure = 0x108;
				sz_entryActions = 0x108;
				sz_binDataForMenu = 0x348;
				num_menuPages = 96;
			}
			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				loadingState = "Loading input structure";
				await WaitIfNecessary();

				inputStruct = InputStructure.Read(reader, LegacyPointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3) {
					LegacyPointer off_IPT_keyAndPadDefine = LegacyPointer.Read(reader);
					ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);
				}
				reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
			}
			uint num_persoInFix = reader.ReadUInt32();
			persoInFix = new LegacyPointer[num_persoInFix];
			for (int i = 0; i < persoInFix.Length; i++) {
				persoInFix[i] = LegacyPointer.Read(reader);
			}
			reader.ReadBytes(sz_randomStructure);
			uint soundEventTableIndexInFix = reader.ReadUInt32();
			LegacyPointer off_soundEventTable = LegacyPointer.Read(reader);
			fonts = FromOffsetOrRead<FontStructure>(reader, LegacyPointer.Current(reader), inline: true);
			if (Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
				reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
			}
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				loadingState = "Loading input structure";
				await WaitIfNecessary();
				inputStruct = InputStructure.Read(reader, LegacyPointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
			}
			if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
				reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
			}
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R3 && Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17) {
				uint num_musicMarkerSlots = reader.ReadUInt32();
				for (int i = 0; i < num_musicMarkerSlots; i++) {
					reader.ReadBytes(sz_musicMarkerSlot);
				}
				reader.ReadBytes(sz_binDataForMenu);
				if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3) {
					LegacyPointer off_bgMaterialForMenu2D = LegacyPointer.Read(reader);
					LegacyPointer off_fixMaterialForMenu2D = LegacyPointer.Read(reader);
					if (Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_01 && Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_21) {
						LegacyPointer off_fixMaterialForSelectedFilms = LegacyPointer.Read(reader);
						LegacyPointer off_fixMaterialForArcadeAndFilms = LegacyPointer.Read(reader);
					}
					for (int i = 0; i < num_menuPages; i++) {
						LegacyPointer off_menuPage = LegacyPointer.Read(reader);
					}
				}
			}
			/*loadingState = "Loading fixed animation bank";
			await WaitIfNecessary();*/
			if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
				off_animBankFix = LegacyPointer.Read(reader); // Note: only one 0x104 bank in fix.
														//print(Pointer.Current(reader));
				print("Fix animation bank address: " + off_animBankFix);
			}
			/*if (off_animBankFix.file == files_array[Mem.Lvl]) {
				animationBanks = new AnimationBank[4]; // 1 in fix, 4 in lvl
				Pointer.DoAt(ref reader, off_animBankFix, () => {
					animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
				});
			} else {
				animationBanks = new AnimationBank[5]; // 1 in fix, 4 in lvl
				Pointer.DoAt(ref reader, off_animBankFix, () => {
					animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
				});
			}*/
		}
		#endregion

		#region LVL
		async UniTask LoadLVL() {
			loadingState = "Loading level memory";
			await WaitIfNecessary();
			files_array[Mem.Lvl].GotoHeader();
			Reader reader = files_array[Mem.Lvl].reader;
			long totalSize = reader.BaseStream.Length;
			//reader.ReadUInt32();
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R3
				&& (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2)) {
				reader.ReadUInt32(); // fix checksum?
			}
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2 &&
				(Legacy_Settings.s.game == Legacy_Settings.Game.RM || Legacy_Settings.s.game == Legacy_Settings.Game.RA)) {
				reader.ReadUInt32(); // fix checksum?
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3
					&& (Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_01
					&& Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PCDemo_2002_10_21
					&& Legacy_Settings.s.mode != Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17)) {
					string timeStamp = reader.ReadString(0x18);
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					if(Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) reader.ReadUInt32();
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RM
					|| Legacy_Settings.s.game == Legacy_Settings.Game.RA
					|| Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur
					|| Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						string timeStamp = reader.ReadString(0x18);
						reader.ReadUInt32();
						reader.ReadUInt32();
					}
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
			}
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS) {
				reader.ReadBytes(0x404); // vignette
			} else if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				reader.ReadBytes(0x104); // vignette
				if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
					reader.ReadUInt32();
				}
			}
			loadingState = "Loading level textures";
			await ReadTexturesLvl(reader, LegacyPointer.Current(reader));
			if ((Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3)
				&& !hasTransit && Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
				LegacyPointer off_lightMapTexture = LegacyPointer.Read(reader); // g_p_stLMTexture
				LegacyPointer.DoAt(ref reader, off_lightMapTexture, () => {
					lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
				});
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					LegacyPointer off_overlightTexture = LegacyPointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
					LegacyPointer.DoAt(ref reader, off_overlightTexture, () => {
						overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
					});
				}
			}
			LegacyPointer off_animBankLvl = null;
			await WaitIfNecessary();
			if (Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur) {
				// animation bank is read right here.
				off_animBankLvl = LegacyPointer.Current(reader); // Note: only one 0x104 bank in fix.
				print("Lvl animation bank address: " + off_animBankLvl);
				animationBanks = new AnimationBank[5];
				AnimationBank[] banks = AnimationBank.Read(reader, off_animBankLvl, 0, 1, files_array[Mem.LvlKeyFrames]);
				animationBanks[0] = banks[0];
			}
			loadingState = "Loading globals";
			await WaitIfNecessary();
			globals.off_transitDynamicWorld = null;
			globals.off_actualWorld = LegacyPointer.Read(reader);
			globals.off_dynamicWorld = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R3
				&& (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
				|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3)) {
				reader.ReadUInt32(); // ???
			}
			globals.off_inactiveDynamicWorld = LegacyPointer.Read(reader);
			globals.off_fatherSector = LegacyPointer.Read(reader); // It is I, Father Sector.
			globals.off_firstSubMapPosition = LegacyPointer.Read(reader);
			globals.num_always = reader.ReadUInt32();
			globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, LegacyPointer.Current(reader), LinkedList.Type.Double);
			globals.off_always_reusableSO = LegacyPointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
			globals.off_always_reusableUnknown1 = LegacyPointer.Read(reader); // (num_always) * 0x2c blocks
			globals.off_always_reusableUnknown2 = LegacyPointer.Read(reader); // (num_always) * 0x4 blocks

			// Read object types
			objectTypes = new ObjectType[3][];
			for (uint i = 0; i < 3; i++) {
				LegacyPointer off_names_header = LegacyPointer.Current(reader);
				LegacyPointer off_names_first = LegacyPointer.Read(reader);
				LegacyPointer off_names_last = LegacyPointer.Read(reader);
				uint num_names = reader.ReadUInt32();

				ReadObjectNamesTable(reader, off_names_first, num_names, i);
			}
			await WaitIfNecessary();

			LegacyPointer off_light = LegacyPointer.Read(reader); // the offset of a light. It's just an ordinary light.
			LegacyPointer off_characterLaunchingSoundEvents = LegacyPointer.Read(reader);

			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				LegacyPointer off_collisionGeoObj = LegacyPointer.Read(reader);
				LegacyPointer off_staticCollisionGeoObj = LegacyPointer.Read(reader);
			}
			if (!hasTransit) {
				reader.ReadUInt32(); // viewport related <--- cameras in here
			}

			LinkedList<int> unknown = LinkedList<int>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);

			families = LinkedList<Family>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);

			LinkedList<int> alwaysActiveCharacters = LinkedList<int>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);

			if (!hasTransit) {
				LinkedList<int> mainCharacters = LinkedList<int>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);
			}
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				LegacyPointer off_mainCharacters_first = LegacyPointer.Read(reader);
				uint num_mainCharacters_entries = reader.ReadUInt32();
			}

			reader.ReadUInt32(); // only used if there was no transit in the previous lvl. Always 00165214 in R3GC?
			reader.ReadUInt32(); // related to "SOL". What is this? Good question.
			reader.ReadUInt32(); // same
			if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur && Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				reader.ReadUInt32(); // same
			}
			LegacyPointer off_cineManager = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				byte unk = reader.ReadByte();
				byte IPO_numRLItables = reader.ReadByte();
				reader.ReadUInt16();
			}

			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2 && (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM)) {
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.AlignOffset(0x10, 4); // 4 because LVL starts at 4
				Matrix identity = Matrix.Read(reader, LegacyPointer.Current(reader));
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
			}
			LegacyPointer off_COL_taggedFacesTable = LegacyPointer.Read(reader);
			uint num_COL_maxTaggedFaces = reader.ReadUInt32();
			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				LegacyPointer off_collisionGeoObj2 = LegacyPointer.Read(reader);
				LegacyPointer off_staticCollisionGeoObj2 = LegacyPointer.Read(reader);
			}
			// The ptrsTable seems to be related to sound events. Perhaps cuuids.
			reader.ReadUInt32();
			if (Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur) {
				for (int i = 0; i < 50; i++) {
					reader.ReadUInt32();
				}
				// Actually, the previous uint is an amount for this array of uints, but it's padded to always be 50 long
			}
			uint num_ptrsTable = reader.ReadUInt32();
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
				uint bool_ptrsTable = reader.ReadUInt32();
			}
			LegacyPointer off_ptrsTable = LegacyPointer.Read(reader);


			uint num_internalStructure = num_ptrsTable;
			if (Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3GC
				|| Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_05_17
				|| Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2Demo_2002_12_18
				|| (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2 && (Legacy_Settings.s.game == Legacy_Settings.Game.RA || Legacy_Settings.s.game == Legacy_Settings.Game.RM))) {
				reader.ReadUInt32();
			}
			LegacyPointer off_internalStructure_first = LegacyPointer.Read(reader);
			LegacyPointer off_internalStructure_last = LegacyPointer.Read(reader);

			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2) {
				if (!hasTransit && Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					uint num_geometric = reader.ReadUInt32();
					LegacyPointer off_array_geometric = LegacyPointer.Read(reader);
					LegacyPointer off_array_geometric_RLI = LegacyPointer.Read(reader);
					LegacyPointer off_array_transition_flags = LegacyPointer.Read(reader);
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RA
					|| Legacy_Settings.s.game == Legacy_Settings.Game.RM
					|| Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur
					|| Legacy_Settings.s.game == Legacy_Settings.Game.DDPK) {
					uint num_unk = reader.ReadUInt32();
					LegacyPointer unk_first = LegacyPointer.Read(reader);
					if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
						LegacyPointer unk_last = LegacyPointer.Read(reader);
					}
				}
			}
			LegacyPointer off_settingsForPersoInFix = null;
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				off_settingsForPersoInFix = LegacyPointer.Current(reader);
				uint num_persoInFix = (uint)persoInFix.Length;
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					num_persoInFix = reader.ReadUInt32();
				}
				for (int i = 0; i < num_persoInFix; i++) {
					if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
						LegacyPointer.Read(reader);
						reader.AlignOffset(0x10, 4); // 4 because LVL starts at 4
						Matrix.Read(reader, LegacyPointer.Current(reader));
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
					} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RA
						|| Legacy_Settings.s.game == Legacy_Settings.Game.RM
						|| Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur) {
						Matrix.Read(reader, LegacyPointer.Current(reader));
					}
				}
				LegacyPointer.Read(reader);
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3 || Legacy_Settings.s.game == Legacy_Settings.Game.DDPK) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
			}

			uint num_visual_materials = reader.ReadUInt32();
			LegacyPointer off_array_visual_materials = LegacyPointer.Read(reader);

			if (Legacy_Settings.s.platform != Legacy_Settings.Platform.PS2
				&& Legacy_Settings.s.mode != Legacy_Settings.Mode.RaymanArenaGC
				&& Legacy_Settings.s.mode != Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07
				&& Legacy_Settings.s.mode != Legacy_Settings.Mode.DonaldDuckPKGC) {
				LegacyPointer off_dynamic_so_list = LegacyPointer.Read(reader);

				// Parse SO list
				LegacyPointer.DoAt(ref reader, off_dynamic_so_list, () => {
					LinkedList<SuperObject>.ReadHeader(reader, off_dynamic_so_list);
					/*Pointer off_so_list_first = Pointer.Read(reader);
                    Pointer off_so_list_last = Pointer.Read(reader);
                    Pointer off_so_list_current = off_so_list_first;
                    uint num_so_list = reader.ReadUInt32();*/
					/*if (experimentalObjectLoading) {
                        for (uint i = 0; i < num_so_list; i++) {
                            R3Pointer.Goto(ref reader, off_so_list_current);
                            R3Pointer off_so_list_next = R3Pointer.Read(reader);
                            R3Pointer off_so_list_prev = R3Pointer.Read(reader);
                            R3Pointer off_so_list_start = R3Pointer.Read(reader);
                            R3Pointer off_so = R3Pointer.Read(reader);
                            R3Pointer.Goto(ref reader, off_so);
                            ParseSuperObject(reader, off_so, true, true);
                            off_so_list_current = off_so_list_next;
                        }
                    }*/
				});
			}

			// Parse materials list
			loadingState = "Loading visual materials";
			await WaitIfNecessary();
			LegacyPointer.DoAt(ref reader, off_array_visual_materials, () => {
				for (uint i = 0; i < num_visual_materials; i++) {
					LegacyPointer off_material = LegacyPointer.Read(reader);
					LegacyPointer.DoAt(ref reader, off_material, () => {
						//print(Pointer.Current(reader));
						visualMaterials.Add(VisualMaterial.Read(reader, off_material));
					});
				}
			});

			if (hasTransit) {
				loadingState = "Loading transit memory";
				await WaitIfNecessary();
				LegacyPointer off_transit = new LegacyPointer(16, files_array[Mem.Transit]); // It's located at offset 20 in transit
				LegacyPointer.DoAt(ref reader, off_transit, () => {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PC
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.MacOS
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.Xbox360
					|| Legacy_Settings.s.platform == Legacy_Settings.Platform.PS3) {
						LegacyPointer off_lightMapTexture = LegacyPointer.Read(reader); // g_p_stLMTexture
						LegacyPointer.DoAt(ref reader, off_lightMapTexture, () => {
							lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
						});
						if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
							LegacyPointer off_overlightTexture = LegacyPointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
							LegacyPointer.DoAt(ref reader, off_overlightTexture, () => {
								overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
							});
						}
					}
					globals.off_transitDynamicWorld = LegacyPointer.Read(reader);
					globals.off_actualWorld = LegacyPointer.Read(reader);
					globals.off_dynamicWorld = LegacyPointer.Read(reader);
					globals.off_inactiveDynamicWorld = LegacyPointer.Read(reader);
				});
			}

			// Parse actual world & always structure
			loadingState = "Loading families";
			await WaitIfNecessary();
			ReadFamilies(reader);
			loadingState = "Loading superobject hierarchy";
			await WaitIfNecessary();
			await ReadSuperObjects(reader);
			loadingState = "Loading always structure";
			await WaitIfNecessary();
			ReadAlways(reader);


			await WaitIfNecessary();
			LegacyPointer.DoAt(ref reader, off_cineManager, () => {
				cinematicsManager = CinematicsManager.Read(reader, off_cineManager);
			});

			// off_current should be after the dynamic SO list positions.
			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				LegacyPointer off_current = LegacyPointer.Goto(ref reader, off_settingsForPersoInFix);
				await ReadSettingsForPersoInFix(reader);
				LegacyPointer.Goto(ref reader, off_current);
			} else {
				await ReadSettingsForPersoInFix(reader);
			}

			if (Legacy_Settings.s.platform == Legacy_Settings.Platform.GC) {
				reader.ReadBytes(0x800); // floats
			}
			loadingState = "Loading animation banks";
			await WaitIfNecessary();
			if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
				off_animBankLvl = LegacyPointer.Read(reader); // Note: 4 0x104 banks in lvl.
				//print(Pointer.Current(reader));
				print("Lvl animation bank address: " + off_animBankLvl);
				animationBanks = new AnimationBank[5];
				if (off_animBankFix != off_animBankLvl) {
					LegacyPointer.DoAt(ref reader, off_animBankFix, () => {
						animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
					});
				}
				await WaitIfNecessary();
				LegacyPointer.DoAt(ref reader, off_animBankLvl, () => {
					AnimationBank[] banks = AnimationBank.Read(reader, off_animBankLvl, 1, 4, files_array[Mem.LvlKeyFrames]);
					for (int i = 0; i < 4; i++) {
						animationBanks[1 + i] = banks[i];
					}
				});
				if (off_animBankFix == off_animBankLvl) {
					animationBanks[0] = animationBanks[1];
				}
			}
			// Load additional animation banks
			string extraAnimFolder = "Anim/";
			if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07 || Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
				extraAnimFolder = lvlName + "/";
			}
			extraAnimFolder = ConvertCase(extraAnimFolder, Legacy_Settings.CapsType.LevelFolder);
			for (int i = 0; i < families.Count; i++) {
				if (families[i] != null && families[i].animBank > 4 && objectTypes[0][families[i].family_index].id != 0xFF) {
					int animBank = families[i].animBank;
					loadingState = "Loading additional animation bank " + animBank;
					await WaitIfNecessary();
					int animFileID = objectTypes[0][families[i].family_index].id;
					if (Legacy_Settings.s.mode == Legacy_Settings.Mode.RaymanArenaGCDemo_2002_03_07 || Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						animFileID = animBank - 5;
					}
					string animName = "ani" + animFileID.ToString();
					string kfName = "key" + animFileID.ToString() + "kf";

					//print(animBank + " - " + objectTypes[0][families[i].family_index].id);
					int fileID = animBank + 102;
					int kfFileID = animBank + 2; // Anim bank will start at 5, so this will start at 7
					if (Legacy_Settings.s.game == Legacy_Settings.Game.RM || (Legacy_Settings.s.game == Legacy_Settings.Game.RA && Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2)) {
						fileID = animBank;
					}

					// Prepare files for WebGL
					string animFileLvl = gameDataBinFolder + extraAnimFolder + ConvertCase(animName + ".lvl", Legacy_Settings.CapsType.LevelFile);
					string animFilePtr = gameDataBinFolder + extraAnimFolder + ConvertCase(animName + ".ptr", Legacy_Settings.CapsType.LevelFile);
					string kfFileLvl = gameDataBinFolder + extraAnimFolder + ConvertCase(kfName + ".lvl", Legacy_Settings.CapsType.LevelFile);
					string kfFilePtr = gameDataBinFolder + extraAnimFolder + ConvertCase(kfName + ".ptr", Legacy_Settings.CapsType.LevelFile);
					await PrepareFile(animFileLvl);
					if (FileSystem.FileExists(animFileLvl)) {
						await PrepareFile(animFilePtr);
					}
					await PrepareFile(kfFileLvl);
					if (FileSystem.FileExists(kfFileLvl)) {
						await PrepareFile(kfFilePtr);
					}

					FileWithPointers animFile = InitExtraLVL(animName, animFileLvl, animFilePtr, fileID);
					FileWithPointers kfFile = InitExtraLVL(kfName, kfFileLvl, kfFilePtr, fileID);
					if (animFile != null) {
						if (animBank >= animationBanks.Length) {
							Array.Resize(ref animationBanks, animBank + 1);
						}
						LegacyPointer off_animBankExtra = new LegacyPointer(0, animFile);
						LegacyPointer.DoAt(ref reader, off_animBankExtra, () => {
							if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
								string timestamp = reader.ReadString(0x18);
								reader.ReadUInt32();
								reader.ReadUInt32();
								reader.ReadUInt32();
								reader.ReadUInt32();
								reader.ReadUInt32();
							} else {
								int alignBytes = reader.ReadInt32();
								if (alignBytes > 0) reader.Align(4, alignBytes);
							}
							off_animBankExtra = LegacyPointer.Current(reader);
							animationBanks[animBank] = AnimationBank.Read(reader, off_animBankExtra, (uint)animBank, 1, kfFile)[0];
						});
					}
				}
			}

			loadingState = "Filling in cross-references";
			await WaitIfNecessary();
			ReadCrossReferences(reader);
		}
		#endregion

		protected async UniTask ReadSettingsForPersoInFix(Reader reader) {
			// Parse transformation matrices and other settings(state? :o) for fix characters
			loadingState = "Loading settings for persos in fix";
			await WaitIfNecessary();
			uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) num_perso_with_settings_in_fix = reader.ReadUInt32();
			for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
				LegacyPointer off_perso_so_with_settings_in_fix = null, off_matrix = null;
				SuperObject so = null;
				Matrix mat = null;
				if (Legacy_Settings.s.game == Legacy_Settings.Game.R3) {
					if (Legacy_Settings.s.platform == Legacy_Settings.Platform.PS2) {
						off_perso_so_with_settings_in_fix = LegacyPointer.Read(reader);
						reader.AlignOffset(0x10, 4); // 4 because LVL starts at 4
						off_matrix = LegacyPointer.Current(reader);
						mat = Matrix.Read(reader, off_matrix);
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
					} else {
						off_perso_so_with_settings_in_fix = LegacyPointer.Read(reader);
						off_matrix = LegacyPointer.Current(reader);
						mat = Matrix.Read(reader, off_matrix);
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
					}
					so = SuperObject.FromOffset(off_perso_so_with_settings_in_fix);
				} else if (Legacy_Settings.s.game == Legacy_Settings.Game.RA
					|| Legacy_Settings.s.game == Legacy_Settings.Game.RM
					|| Legacy_Settings.s.game == Legacy_Settings.Game.DDPK
					|| Legacy_Settings.s.game == Legacy_Settings.Game.Dinosaur) {
					off_matrix = LegacyPointer.Current(reader);
					mat = Matrix.Read(reader, off_matrix);
					so = superObjects.FirstOrDefault(s => s.off_data == persoInFix[i]);
				}
				if (so != null) {
					so.off_matrix = off_matrix;
					so.matrix = mat;
					if (so.Gao != null) {
						so.Gao.transform.localPosition = mat.GetPosition(convertAxes: true);
						so.Gao.transform.localRotation = mat.GetRotation(convertAxes: true);
						so.Gao.transform.localScale = mat.GetScale(convertAxes: true);
					}
				}
			}
		}
	}
}