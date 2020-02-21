using OpenSpace.Animation;
using OpenSpace.Cinematics;
using OpenSpace.FileFormat;
using OpenSpace.Input;
using OpenSpace.Object;
using OpenSpace.Object.Properties;
using OpenSpace.Text;
using OpenSpace.Visual;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenSpace.Loader {
	public class R3Loader : MapLoader {
		protected override async Task Load() {
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
					if (!Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
					Settings.s.loadFromMemory = true;
					MemoryFile mem = new MemoryFile(lvlName);
					files_array[0] = mem;
					await WaitIfNecessary();
					LoadMemory();
				} else {
					// Prepare paths
					string fixFolder = gameDataBinFolder;
					string lvlFolder = gameDataBinFolder + ConvertCase(lvlName + "/", Settings.CapsType.LevelFolder);

					paths["fix.lvl"] = fixFolder + ConvertCase("Fix.lvl", Settings.CapsType.Fix);
					paths["fix.ptr"] = fixFolder + ConvertCase("Fix.ptr", Settings.CapsType.Fix);
					paths["lvl.lvl"] = lvlFolder + ConvertCase(lvlName + ".lvl", Settings.CapsType.LevelFile);
					paths["lvl.ptr"] = lvlFolder + ConvertCase(lvlName + ".ptr", Settings.CapsType.LevelFile);
					paths["transit.lvl"] = lvlFolder + ConvertCase("transit.lvl", Settings.CapsType.LevelFile);
					paths["transit.ptr"] = lvlFolder + ConvertCase("transit.ptr", Settings.CapsType.LevelFile);
					if (Settings.s.platform == Settings.Platform.GC) {
						paths["menu.tpl"] = fixFolder + ConvertCase("menu.tpl", Settings.CapsType.Fix);
						paths["fix.tpl"] = fixFolder + ConvertCase((Settings.s.mode == Settings.Mode.RaymanArenaGC) ? "../common.tpl" : "Fix.tpl", Settings.CapsType.Fix);
						paths["lvl.tpl"] = lvlFolder + ConvertCase(lvlName + (Settings.s.game == Settings.Game.R3 ? "_Lvl" : "") + ".tpl", Settings.CapsType.TextureFile);
						paths["transit.tpl"] = lvlFolder + ConvertCase(lvlName + "_Trans.tpl", Settings.CapsType.TextureFile);
					} else if (Settings.s.platform == Settings.Platform.Xbox) {
						paths["fix.btf"] = fixFolder + ConvertCase("Fix.btf", Settings.CapsType.Fix);
						paths["fix.bhf"] = fixFolder + ConvertCase("Fix.bhf", Settings.CapsType.Fix);
						paths["lvl.btf"] = lvlFolder + ConvertCase(lvlName + ".btf", Settings.CapsType.TextureFile);
						paths["lvl.bhf"] = lvlFolder + ConvertCase(lvlName + ".bhf", Settings.CapsType.TextureFile);
						paths["transit.btf"] = lvlFolder + ConvertCase("transit.btf", Settings.CapsType.TextureFile);
						paths["transit.bhf"] = lvlFolder + ConvertCase("transit.bhf", Settings.CapsType.TextureFile);
					} else if (Settings.s.platform == Settings.Platform.Xbox360) {
						paths["fix.btf"] = fixFolder + ConvertCase("Fix.btf", Settings.CapsType.Fix);
						paths["fix.bhf"] = fixFolder + ConvertCase("Fix.bhf", Settings.CapsType.Fix);
						paths["lvl.btf"] = lvlFolder + ConvertCase(lvlName + "_2.btf", Settings.CapsType.TextureFile);
						paths["lvl.bhf"] = lvlFolder + ConvertCase(lvlName + "_2.bhf", Settings.CapsType.TextureFile);
						paths["transit.btf"] = lvlFolder + ConvertCase("transit_6.btf", Settings.CapsType.TextureFile);
						paths["transit.bhf"] = lvlFolder + ConvertCase("transit_6.bhf", Settings.CapsType.TextureFile);
					} else if (Settings.s.platform == Settings.Platform.PS2) {
						paths["fix.tbf"] = fixFolder + ConvertCase("Fix.tbf", Settings.CapsType.Fix);
						paths["lvl.tbf"] = lvlFolder + ConvertCase(lvlName + ".tbf", Settings.CapsType.TextureFile);
						paths["transit.tbf"] = lvlFolder + ConvertCase("transit.tbf", Settings.CapsType.TextureFile);
					}
					paths["lvl_vb.lvl"] = lvlFolder + ConvertCase(lvlName + "_vb.lvl", Settings.CapsType.LevelFile);
					paths["lvl_vb.ptr"] = lvlFolder + ConvertCase(lvlName + "_vb.ptr", Settings.CapsType.LevelFile);
					paths["fixkf.lvl"] = fixFolder + ConvertCase("Fixkf.lvl", Settings.CapsType.Fix);
					paths["fixkf.ptr"] = fixFolder + ConvertCase("Fixkf.ptr", Settings.CapsType.Fix);
					paths["lvlkf.lvl"] = lvlFolder + ConvertCase(lvlName + "kf.lvl", Settings.CapsType.LevelFile);
					paths["lvlkf.ptr"] = lvlFolder + ConvertCase(lvlName + "kf.ptr", Settings.CapsType.LevelFile);

					// Download files
					/*foreach (KeyValuePair<string, string> path in paths) {
						if (path.Value != null) await PrepareFile(path.Value);
					}*/



					// Fix
					lvlNames[0] = "fix";
					lvlPaths[0] = paths["fix.lvl"];
					ptrPaths[0] = paths["fix.ptr"];
					if (Settings.s.platform == Settings.Platform.GC) {
						await PrepareFile(paths["fix.tpl"]);
						await PrepareFile(paths["menu.tpl"]);
					} else if (Settings.s.platform == Settings.Platform.Xbox
						|| Settings.s.platform == Settings.Platform.Xbox360) {
						await PrepareFile(paths["fix.btf"]);
						await PrepareFile(paths["fix.bhf"]);
					} else if (Settings.s.platform == Settings.Platform.PS2) {
						await PrepareFile(paths["fix.tbf"]);
					}
					await PrepareFile(lvlPaths[0]);
					if (FileSystem.FileExists(lvlPaths[0])) {
						await PrepareFile(ptrPaths[0]);
					}

					// Level
					lvlNames[1] = lvlName;
					lvlPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + ".lvl";
					ptrPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + ".ptr";
					await PrepareFile(lvlPaths[1]);
					if (FileSystem.FileExists(lvlPaths[1])) {
						await PrepareFile(ptrPaths[1]);
						if (Settings.s.platform == Settings.Platform.GC) {
							await PrepareFile(paths["lvl.tpl"]);
						} else if (Settings.s.platform == Settings.Platform.Xbox
						|| Settings.s.platform == Settings.Platform.Xbox360) {
							await PrepareFile(paths["lvl.btf"]);
							await PrepareFile(paths["lvl.bhf"]);
						} else if (Settings.s.platform == Settings.Platform.PS2) {
							await PrepareFile(paths["lvl.tbf"]);
						}
					}

					// Transit
					lvlNames[2] = "transit";
					lvlPaths[2] = gameDataBinFolder + lvlName + "/transit.lvl";
					ptrPaths[2] = gameDataBinFolder + lvlName + "/transit.ptr";
					await PrepareFile(lvlPaths[2]);
					if (FileSystem.FileExists(lvlPaths[2])) {
						await PrepareFile(ptrPaths[2]);
						if (Settings.s.platform == Settings.Platform.GC) {
							await PrepareFile(paths["transit.tpl"]);
						} else if (Settings.s.platform == Settings.Platform.Xbox
						|| Settings.s.platform == Settings.Platform.Xbox360) {
							await PrepareFile(paths["transit.btf"]);
							await PrepareFile(paths["transit.bhf"]);
						} else if (Settings.s.platform == Settings.Platform.PS2) {
							await PrepareFile(paths["transit.tbf"]);
						}
					}
					hasTransit = FileSystem.FileExists(lvlPaths[2]) && (FileSystem.GetFileLength(lvlPaths[2]) > 4);

					// Vertex buffer
					lvlNames[4] = lvlName + "_vb";
					lvlPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "_vb.lvl";
					ptrPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "_vb.ptr";
					await PrepareFile(lvlPaths[4]);
					if (FileSystem.FileExists(lvlPaths[4])) {
						await PrepareFile(ptrPaths[4]);
					}

					// Fix Keyframes
					lvlNames[5] = "fixkf";
					lvlPaths[5] = gameDataBinFolder + "fixkf.lvl";
					ptrPaths[5] = gameDataBinFolder + "fixkf.ptr";
					await PrepareFile(lvlPaths[5]);
					if (FileSystem.FileExists(lvlPaths[5])) {
						await PrepareFile(ptrPaths[5]);
					}

					// Level Keyframes
					lvlNames[6] = lvlName + "kf";
					lvlPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "kf.lvl";
					ptrPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "kf.ptr";
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

		#region FIX
		Pointer off_animBankFix;

		async Task LoadFIX() {
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
			if (Settings.s.platform == Settings.Platform.PC
				|| Settings.s.platform == Settings.Platform.Xbox
				|| Settings.s.platform == Settings.Platform.Xbox360
				|| Settings.s.platform == Settings.Platform.PS3
				|| Settings.s.platform == Settings.Platform.PS2) {
				if (Settings.s.game == Settings.Game.R3) {
					string timeStamp = reader.ReadString(0x18);
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
				} else if (Settings.s.game == Settings.Game.RM || Settings.s.game == Settings.Game.RA || Settings.s.game == Settings.Game.Dinosaur) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
			}
			Pointer off_identityMatrix = Pointer.Read(reader);
			loadingState = "Loading text";
			await WaitIfNecessary();
			localization = FromOffsetOrRead<LocalizationStructure>(reader, Pointer.Current(reader), inline: true);
			if (Settings.s.platform != Settings.Platform.PS2) {
				uint num_lvlNames = reader.ReadUInt32();
				uint num_fixEntries1 = reader.ReadUInt32();
				// Read tables under header
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savName = new string(reader.ReadChars(0xC));
				}
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savMapName = new string(reader.ReadChars(0xC));
				}
				ReadLevelNames(reader, Pointer.Current(reader), num_lvlNames);
				if (Settings.s.platform == Settings.Platform.PC
					|| Settings.s.platform == Settings.Platform.Xbox
					|| Settings.s.platform == Settings.Platform.Xbox360
					|| Settings.s.platform == Settings.Platform.PS3) {
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
				ReadLevelNames(reader, Pointer.Current(reader), num_lvlNames);
				string firstMapName = new string(reader.ReadChars(0x1E));
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savName = new string(reader.ReadChars(0xC));
				}
				for (uint i = 0; i < num_fixEntries1; i++) {
					string savMapName = new string(reader.ReadChars(0xC));
				}
			}
			uint num_languages = reader.ReadUInt32();
			Pointer off_languages = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_languages, () => {
				ReadLanguages(reader, off_languages, num_languages);
			});
			if (Settings.s.platform == Settings.Platform.PS2 && localization != null) {
				for (int i = 0; i < localization.num_languages; i++) {
					if (localization.languages[i].off_textTable == null) {
						// Load text from file
						string fileName = "TEXTS/LANG" + i;
						loadingState = "Loading text files: " + (i + 1) + "/" + localization.num_languages;
						paths["lang" + i + ".lvl"] = gameDataBinFolder + fileName + ".LVL";
						paths["lang" + i + ".ptr"] = gameDataBinFolder + fileName + ".PTR";
						await PrepareFile(paths["lang" + i + ".lvl"]);
						if (FileSystem.FileExists(paths["lang" + i + ".lvl"])) {
							await PrepareFile(paths["lang" + i + ".ptr"]);
							int fileId = i + 207;
							FileWithPointers f = InitExtraLVL(fileName, fileId);
							Pointer.DoAt(ref reader, new Pointer(0, f), () => {

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
			print("Fix textures address: " + Pointer.Current(reader));
			await ReadTexturesFix(reader, Pointer.Current(reader));
			// Defaults for Rayman 3 PC. Sizes are hardcoded in the exes and might differ for versions too :/
			int sz_entryActions = 0x100;
			int sz_randomStructure = 0xDC;
			int sz_videoStructure = 0x18;
			int sz_musicMarkerSlot = 0x28;
			int sz_binDataForMenu = 0x020C;
			int num_menuPages = 35;

			if (Settings.s.mode == Settings.Mode.Rayman3GC) {
				sz_entryActions = 0xE8;
				sz_binDataForMenu = 0x01F0;
			} else if (Settings.s.mode == Settings.Mode.RaymanArenaGC
				|| Settings.s.mode == Settings.Mode.RaymanArenaGCDemo) {
				sz_entryActions = 0xC4;
			} else if (Settings.s.mode == Settings.Mode.RaymanArenaPC
				|| Settings.s.mode == Settings.Mode.RaymanMPC) {
				sz_entryActions = 0xDC;
			} else if (Settings.s.mode == Settings.Mode.DinosaurPC) {
				sz_entryActions = 0xD8;
				sz_randomStructure = 0xE0;
			} else if (Settings.s.mode == Settings.Mode.DonaldDuckPKGC) {
				sz_entryActions = 0xC0;
			} else if (Settings.s.mode == Settings.Mode.RaymanArenaXbox) {
				sz_entryActions = 0xF0;
			}
			if (Settings.s.platform == Settings.Platform.PS2) {
				sz_videoStructure = 0x108;
				sz_entryActions = 0x10C; // probably not right but oh well
				sz_binDataForMenu = 0x1A4;
			} else if (Settings.s.platform == Settings.Platform.Xbox) {
				sz_videoStructure = 0x108;
				sz_binDataForMenu = 0x1AC;
			} else if (Settings.s.platform == Settings.Platform.Xbox360) {
				sz_videoStructure = 0x108;
				sz_entryActions = 0x108;
				sz_binDataForMenu = 0x33C;
				num_menuPages = 96;
			} else if (Settings.s.platform == Settings.Platform.PS3) {
				sz_videoStructure = 0x108;
				sz_entryActions = 0x108;
				sz_binDataForMenu = 0x348;
				num_menuPages = 96;
			}
			if (Settings.s.platform != Settings.Platform.PS2) {
				loadingState = "Loading input structure";
				await WaitIfNecessary();
				inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				if (Settings.s.platform == Settings.Platform.PC
					|| Settings.s.platform == Settings.Platform.Xbox
					|| Settings.s.platform == Settings.Platform.Xbox360
					|| Settings.s.platform == Settings.Platform.PS3) {
					Pointer off_IPT_keyAndPadDefine = Pointer.Read(reader);
					ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);
				}
				reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
			}
			uint num_persoInFix = reader.ReadUInt32();
			persoInFix = new Pointer[num_persoInFix];
			for (int i = 0; i < persoInFix.Length; i++) {
				persoInFix[i] = Pointer.Read(reader);
			}
			reader.ReadBytes(sz_randomStructure);
			uint soundEventTableIndexInFix = reader.ReadUInt32();
			Pointer off_soundEventTable = Pointer.Read(reader);
			fonts = FromOffsetOrRead<FontStructure>(reader, Pointer.Current(reader), inline: true);
			reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
			if (Settings.s.platform == Settings.Platform.PS2) {
				loadingState = "Loading input structure";
				await WaitIfNecessary();
				inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
			}
			if (Settings.s.game == Settings.Game.R3) {
				uint num_musicMarkerSlots = reader.ReadUInt32();
				for (int i = 0; i < num_musicMarkerSlots; i++) {
					reader.ReadBytes(sz_musicMarkerSlot);
				}
				reader.ReadBytes(sz_binDataForMenu);
				if (Settings.s.platform == Settings.Platform.PC
					|| Settings.s.platform == Settings.Platform.Xbox
					|| Settings.s.platform == Settings.Platform.Xbox360
					|| Settings.s.platform == Settings.Platform.PS3) {
					Pointer off_bgMaterialForMenu2D = Pointer.Read(reader);
					Pointer off_fixMaterialForMenu2D = Pointer.Read(reader);
					Pointer off_fixMaterialForSelectedFilms = Pointer.Read(reader);
					Pointer off_fixMaterialForArcadeAndFilms = Pointer.Read(reader);
					for (int i = 0; i < num_menuPages; i++) {
						Pointer off_menuPage = Pointer.Read(reader);
					}
				}
			}
			/*loadingState = "Loading fixed animation bank";
			await WaitIfNecessary();*/
			if (Settings.s.game != Settings.Game.Dinosaur) {
				off_animBankFix = Pointer.Read(reader); // Note: only one 0x104 bank in fix.
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
		async Task LoadLVL() {
			loadingState = "Loading level memory";
			await WaitIfNecessary();
			files_array[Mem.Lvl].GotoHeader();
			Reader reader = files_array[Mem.Lvl].reader;
			long totalSize = reader.BaseStream.Length;
			//reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.R3
				&& (Settings.s.platform == Settings.Platform.PC
				|| Settings.s.platform == Settings.Platform.Xbox
				|| Settings.s.platform == Settings.Platform.Xbox360
				|| Settings.s.platform == Settings.Platform.PS3
				|| Settings.s.platform == Settings.Platform.PS2)) {
				reader.ReadUInt32(); // fix checksum?
			}
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			reader.ReadUInt32();
			if (Settings.s.platform == Settings.Platform.PC
				|| Settings.s.platform == Settings.Platform.Xbox
				|| Settings.s.platform == Settings.Platform.Xbox360
				|| Settings.s.platform == Settings.Platform.PS3
				|| Settings.s.platform == Settings.Platform.PS2) {
				if (Settings.s.game == Settings.Game.R3) {
					string timeStamp = reader.ReadString(0x18);
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					reader.ReadUInt32();
					if(Settings.s.platform != Settings.Platform.PS2) reader.ReadUInt32();
				} else if (Settings.s.game == Settings.Game.RM
					|| Settings.s.game == Settings.Game.RA
					|| Settings.s.game == Settings.Game.Dinosaur) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
			}
			if (Settings.s.platform != Settings.Platform.PS2) {
				reader.ReadBytes(0x104); // vignette
				if (Settings.s.game != Settings.Game.Dinosaur) {
					reader.ReadUInt32();
				}
			}
			loadingState = "Loading level textures";
			await ReadTexturesLvl(reader, Pointer.Current(reader));
			if ((Settings.s.platform == Settings.Platform.PC
				|| Settings.s.platform == Settings.Platform.Xbox
				|| Settings.s.platform == Settings.Platform.Xbox360
				|| Settings.s.platform == Settings.Platform.PS3)
				&& !hasTransit && Settings.s.game != Settings.Game.Dinosaur) {
				Pointer off_lightMapTexture = Pointer.Read(reader); // g_p_stLMTexture
				Pointer.DoAt(ref reader, off_lightMapTexture, () => {
					lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
				});
				if (Settings.s.game == Settings.Game.R3) {
					Pointer off_overlightTexture = Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
					Pointer.DoAt(ref reader, off_overlightTexture, () => {
						overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
					});
				}
			}
			Pointer off_animBankLvl = null;
			if (Settings.s.game == Settings.Game.Dinosaur) {
				// animation bank is read right here.
				off_animBankLvl = Pointer.Current(reader); // Note: only one 0x104 bank in fix.
				print("Lvl animation bank address: " + off_animBankLvl);
				animationBanks = new AnimationBank[5];
				AnimationBank[] banks = AnimationBank.Read(reader, off_animBankLvl, 0, 1, files_array[Mem.LvlKeyFrames]);
				animationBanks[0] = banks[0];
			}
			loadingState = "Loading globals";
			await WaitIfNecessary();
			globals.off_transitDynamicWorld = null;
			globals.off_actualWorld = Pointer.Read(reader);
			globals.off_dynamicWorld = Pointer.Read(reader);
			if (Settings.s.game == Settings.Game.R3
				&& (Settings.s.platform == Settings.Platform.PC
				|| Settings.s.platform == Settings.Platform.Xbox
				|| Settings.s.platform == Settings.Platform.Xbox360
				|| Settings.s.platform == Settings.Platform.PS3)) {
				reader.ReadUInt32(); // ???
			}
			globals.off_inactiveDynamicWorld = Pointer.Read(reader);
			globals.off_fatherSector = Pointer.Read(reader); // It is I, Father Sector.
			globals.off_firstSubMapPosition = Pointer.Read(reader);
			globals.num_always = reader.ReadUInt32();
			globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
			globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
			globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
			globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks

			// Read object types
			objectTypes = new ObjectType[3][];
			for (uint i = 0; i < 3; i++) {
				Pointer off_names_header = Pointer.Current(reader);
				Pointer off_names_first = Pointer.Read(reader);
				Pointer off_names_last = Pointer.Read(reader);
				uint num_names = reader.ReadUInt32();

				ReadObjectNamesTable(reader, off_names_first, num_names, i);
			}

			Pointer off_light = Pointer.Read(reader); // the offset of a light. It's just an ordinary light.
			Pointer off_characterLaunchingSoundEvents = Pointer.Read(reader);

			if (Settings.s.platform != Settings.Platform.PS2) {
				Pointer off_collisionGeoObj = Pointer.Read(reader);
				Pointer off_staticCollisionGeoObj = Pointer.Read(reader);
			}
			if (!hasTransit) {
				reader.ReadUInt32(); // viewport related <--- cameras in here
			}

			LinkedList<int> unknown = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
			
			families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

			LinkedList<int> alwaysActiveCharacters = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

			if (!hasTransit) {
				LinkedList<int> mainCharacters = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
			}
			if (Settings.s.platform == Settings.Platform.PS2) {
				Pointer off_mainCharacters_first = Pointer.Read(reader);
				uint num_mainCharacters_entries = reader.ReadUInt32();
			}

			reader.ReadUInt32(); // only used if there was no transit in the previous lvl. Always 00165214 in R3GC?
			reader.ReadUInt32(); // related to "SOL". What is this? Good question.
			reader.ReadUInt32(); // same
			if (Settings.s.game != Settings.Game.Dinosaur && Settings.s.platform != Settings.Platform.PS2) {
				reader.ReadUInt32(); // same
			}
			Pointer off_cineManager = Pointer.Read(reader);
			if (Settings.s.platform != Settings.Platform.PS2) {
				byte unk = reader.ReadByte();
				byte IPO_numRLItables = reader.ReadByte();
				reader.ReadUInt16();
			}
			Pointer off_COL_taggedFacesTable = Pointer.Read(reader);
			uint num_COL_maxTaggedFaces = reader.ReadUInt32();
			if (Settings.s.platform != Settings.Platform.PS2) {
				Pointer off_collisionGeoObj2 = Pointer.Read(reader);
				Pointer off_staticCollisionGeoObj2 = Pointer.Read(reader);
			}
			// The ptrsTable seems to be related to sound events. Perhaps cuuids.
			reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.Dinosaur) {
				for (int i = 0; i < 50; i++) {
					reader.ReadUInt32();
				}
				// Actually, the previous uint is an amount for this array of uints, but it's padded to always be 50 long
			}
			uint num_ptrsTable = reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.R3) {
				uint bool_ptrsTable = reader.ReadUInt32();
			}
			Pointer off_ptrsTable = Pointer.Read(reader);


			uint num_internalStructure = num_ptrsTable;
			if (Settings.s.mode == Settings.Mode.Rayman3GC) {
				reader.ReadUInt32();
			}
			Pointer off_internalStructure_first = Pointer.Read(reader);
			Pointer off_internalStructure_last = Pointer.Read(reader);

			if (Settings.s.platform != Settings.Platform.PS2 && !hasTransit && Settings.s.game == Settings.Game.R3) {
				uint num_geometric = reader.ReadUInt32();
				Pointer off_array_geometric = Pointer.Read(reader);
				Pointer off_array_geometric_RLI = Pointer.Read(reader);
				Pointer off_array_transition_flags = Pointer.Read(reader);
			} else if (Settings.s.game == Settings.Game.RA
				|| Settings.s.game == Settings.Game.RM
				|| Settings.s.game == Settings.Game.Dinosaur
				|| Settings.s.game == Settings.Game.DDPK) {
				uint num_unk = reader.ReadUInt32();
				Pointer unk_first = Pointer.Read(reader);
				if (Settings.s.game != Settings.Game.Dinosaur) {
					Pointer unk_last = Pointer.Read(reader);
				}
			}
			Pointer off_settingsForPersoInFix = null;
			if (Settings.s.platform == Settings.Platform.PS2) {
				off_settingsForPersoInFix = Pointer.Current(reader);
				uint num_persoInFix = reader.ReadUInt32();
				for (int i = 0; i < num_persoInFix; i++) {
					if (Settings.s.game == Settings.Game.R3) {
						Pointer off_mainChar = Pointer.Read(reader);
						reader.AlignOffset(0x10, 4); // 4 because LVL starts at 4
						Matrix.Read(reader, Pointer.Current(reader));
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
						Pointer.Read(reader);
					} else if (Settings.s.game == Settings.Game.RA
						|| Settings.s.game == Settings.Game.RM
						|| Settings.s.game == Settings.Game.Dinosaur) {
						Matrix.Read(reader, Pointer.Current(reader));
					}
				}
				reader.ReadUInt32();
				reader.ReadUInt32();
			}

			uint num_visual_materials = reader.ReadUInt32();
			Pointer off_array_visual_materials = Pointer.Read(reader);

			if (Settings.s.platform != Settings.Platform.PS2
				&& Settings.s.mode != Settings.Mode.RaymanArenaGC
				&& Settings.s.mode != Settings.Mode.RaymanArenaGCDemo
				&& Settings.s.mode != Settings.Mode.DonaldDuckPKGC) {
				Pointer off_dynamic_so_list = Pointer.Read(reader);

				// Parse SO list
				Pointer.DoAt(ref reader, off_dynamic_so_list, () => {
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
			Pointer.DoAt(ref reader, off_array_visual_materials, () => {
				for (uint i = 0; i < num_visual_materials; i++) {
					Pointer off_material = Pointer.Read(reader);
					Pointer.DoAt(ref reader, off_material, () => {
						//print(Pointer.Current(reader));
						visualMaterials.Add(VisualMaterial.Read(reader, off_material));
					});
				}
			});

			if (hasTransit) {
				loadingState = "Loading transit memory";
				await WaitIfNecessary();
				Pointer off_transit = new Pointer(16, files_array[Mem.Transit]); // It's located at offset 20 in transit
				Pointer.DoAt(ref reader, off_transit, () => {
					if (Settings.s.platform == Settings.Platform.PC
					|| Settings.s.platform == Settings.Platform.Xbox
					|| Settings.s.platform == Settings.Platform.Xbox360
					|| Settings.s.platform == Settings.Platform.PS3) {
						Pointer off_lightMapTexture = Pointer.Read(reader); // g_p_stLMTexture
						Pointer.DoAt(ref reader, off_lightMapTexture, () => {
							lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
						});
						if (Settings.s.game == Settings.Game.R3) {
							Pointer off_overlightTexture = Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
							Pointer.DoAt(ref reader, off_overlightTexture, () => {
								overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
							});
						}
					}
					globals.off_transitDynamicWorld = Pointer.Read(reader);
					globals.off_actualWorld = Pointer.Read(reader);
					globals.off_dynamicWorld = Pointer.Read(reader);
					globals.off_inactiveDynamicWorld = Pointer.Read(reader);
				});
			}

			// Parse actual world & always structure
			loadingState = "Loading families";
			await WaitIfNecessary();
			ReadFamilies(reader);
			loadingState = "Loading superobject hierarchy";
			await WaitIfNecessary();
			ReadSuperObjects(reader);
			loadingState = "Loading always structure";
			await WaitIfNecessary();
			ReadAlways(reader);


			Pointer.DoAt(ref reader, off_cineManager, () => {
				cinematicsManager = CinematicsManager.Read(reader, off_cineManager);
			});

			// off_current should be after the dynamic SO list positions.
			if (Settings.s.platform == Settings.Platform.PS2) {
				Pointer off_current = Pointer.Goto(ref reader, off_settingsForPersoInFix);
				await ReadSettingsForPersoInFix(reader);
				Pointer.Goto(ref reader, off_current);
			} else {
				await ReadSettingsForPersoInFix(reader);
			}

			if (Settings.s.platform == Settings.Platform.GC) {
				reader.ReadBytes(0x800); // floats
			}
			loadingState = "Loading animation banks";
			await WaitIfNecessary();
			if (Settings.s.game != Settings.Game.Dinosaur) {
				off_animBankLvl = Pointer.Read(reader); // Note: 4 0x104 banks in lvl.
				print("Lvl animation bank address: " + off_animBankLvl);
				animationBanks = new AnimationBank[5];
				if (off_animBankFix != off_animBankLvl) {
					Pointer.DoAt(ref reader, off_animBankFix, () => {
						animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
					});
				}
				Pointer.DoAt(ref reader, off_animBankLvl, () => {
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
			if (Settings.s.mode == Settings.Mode.RaymanArenaGCDemo) {
				extraAnimFolder = lvlName + "/";
			}
			for (int i = 0; i < families.Count; i++) {
				if (families[i] != null && families[i].animBank > 4 && objectTypes[0][families[i].family_index].id != 0xFF) {
					int animBank = families[i].animBank;
					loadingState = "Loading additional animation bank " + animBank;
					await WaitIfNecessary();
					int animFileID = objectTypes[0][families[i].family_index].id;
					if (Settings.s.mode == Settings.Mode.RaymanArenaGCDemo) {
						animFileID = animBank - 5;
					}
					string animName = extraAnimFolder + "ani" + animFileID.ToString();
					string kfName = extraAnimFolder + "key" + animFileID.ToString() + "kf";

					//print(animBank + " - " + objectTypes[0][families[i].family_index].id);
					int fileID = animBank + 102;
					int kfFileID = animBank + 2; // Anim bank will start at 5, so this will start at 7
					if (Settings.s.game == Settings.Game.RM) {
						fileID = animBank;
					}

					// Prepare files for WebGL
					await PrepareFile(gameDataBinFolder + animName + ".lvl");
					if (FileSystem.FileExists(gameDataBinFolder + animName + ".lvl")) {
						await PrepareFile(gameDataBinFolder + animName + ".ptr");
					}
					await PrepareFile(gameDataBinFolder + kfName + ".lvl");
					if (FileSystem.FileExists(gameDataBinFolder + kfName + ".lvl")) {
						await PrepareFile(gameDataBinFolder + kfName + ".ptr");
					}

					FileWithPointers animFile = InitExtraLVL(animName, fileID);
					FileWithPointers kfFile = InitExtraLVL(kfName, fileID);
					if (animFile != null) {
						if (animBank >= animationBanks.Length) {
							Array.Resize(ref animationBanks, animBank + 1);
						}
						Pointer off_animBankExtra = new Pointer(0, animFile);
						Pointer.DoAt(ref reader, off_animBankExtra, () => {
							int alignBytes = reader.ReadInt32();
							if (alignBytes > 0) reader.Align(4, alignBytes);
							off_animBankExtra = Pointer.Current(reader);
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

		protected async Task ReadSettingsForPersoInFix(Reader reader) {
			// Parse transformation matrices and other settings(state? :o) for fix characters
			loadingState = "Loading settings for persos in fix";
			await WaitIfNecessary();
			uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
			if (Settings.s.game == Settings.Game.R3) num_perso_with_settings_in_fix = reader.ReadUInt32();
			for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
				Pointer off_perso_so_with_settings_in_fix = null, off_matrix = null;
				SuperObject so = null;
				Matrix mat = null;
				if (Settings.s.game == Settings.Game.R3) {
					if (Settings.s.platform == Settings.Platform.PS2) {
						Pointer off_mainChar = Pointer.Read(reader);
						reader.AlignOffset(0x10, 4); // 4 because LVL starts at 4
						off_matrix = Pointer.Current(reader);
						mat = Matrix.Read(reader, off_matrix);
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
						off_perso_so_with_settings_in_fix = Pointer.Read(reader);
					} else {
						off_perso_so_with_settings_in_fix = Pointer.Read(reader);
						off_matrix = Pointer.Current(reader);
						mat = Matrix.Read(reader, off_matrix);
						reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
						reader.ReadUInt32();
					}
					so = SuperObject.FromOffset(off_perso_so_with_settings_in_fix);
				} else if (Settings.s.game == Settings.Game.RA
					|| Settings.s.game == Settings.Game.RM
					|| Settings.s.game == Settings.Game.Dinosaur) {
					off_matrix = Pointer.Current(reader);
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