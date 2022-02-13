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
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using BinarySerializer.Unity;

namespace OpenSpace.Loader {
    public class R2Loader : MapLoader {
		protected override async UniTask Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				await FileSystem.CheckDirectory(gameDataBinFolder);
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");

                loadingState = "Initializing files";
                await WaitIfNecessary();
                string gameDsbPath = gameDataBinFolder + ConvertCase("Game.dsb", CPA_Settings.CapsType.DSB);
                if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                    gameDsbPath = gameDataBinFolder + ConvertCase("gamedsc.bin", CPA_Settings.CapsType.DSB);
                } else if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    gameDsbPath = gameDataBinFolder + ConvertCase("GAME.DSC", CPA_Settings.CapsType.DSB);
				}
				await PrepareFile(gameDsbPath);
				gameDsb = new DSB("Game", gameDsbPath);
				if (FileSystem.mode != FileSystem.Mode.Web) {
					gameDsb.Save(gameDataBinFolder + ConvertCase("Game_dsb.dmp", CPA_Settings.CapsType.DSB));
				}
                gameDsb.ReadAllSections();
                gameDsb.Dispose();

				await CreateCNT();

				if (CPA_Settings.s.game == CPA_Settings.Game.R2) {
					string comportsPath = gameDataBinFolder + "R2DC_Comports.json";
					await PrepareFile(comportsPath);
				}

				if (lvlName.EndsWith(".exe")) {
                    if (!CPA_Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
                    CPA_Settings.s.loadFromMemory = true;
                    MemoryFile mem = new MemoryFile(lvlName);
                    files_array[0] = mem;
                    await WaitIfNecessary();
                    await LoadMemory();
                } else {
                    hasTransit = false;
                    DAT dat = null;
					string levelsSubFolder = ConvertCase(ConvertPath(gameDsb.levelsDataPath), CPA_Settings.CapsType.All) + "/";
					string levelsFolder = gameDataBinFolder + levelsSubFolder;
                    string langDataPath = gameDataBinFolder
						+ ConvertCase("../LangData/English/", CPA_Settings.CapsType.All)
						+ levelsSubFolder;
					if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
						await FileSystem.CheckDirectory(langDataPath);
						if (FileSystem.mode != FileSystem.Mode.Web && !FileSystem.DirectoryExists(langDataPath)) {
							string langPath = gameDataBinFolder + ConvertCase("../LangData/", CPA_Settings.CapsType.All);
							await FileSystem.CheckDirectory(langPath);
							if (FileSystem.DirectoryExists(langPath)) {
								DirectoryInfo dirInfo = new DirectoryInfo(langPath);
								DirectoryInfo firstLang = dirInfo.GetDirectories().FirstOrDefault();
								if (firstLang != null) {
									langDataPath = firstLang.FullName + "/" + levelsSubFolder;
									await FileSystem.CheckDirectory(langDataPath);
								}
							}
						}
					}

                    await WaitIfNecessary();
					bool hasRelocationFiles = true;
                    if (CPA_Settings.s.mode == CPA_Settings.Mode.Rayman2PC || CPA_Settings.s.mode == CPA_Settings.Mode.DonaldDuckPC) {
                        string dataPath = levelsFolder + "LEVELS0.DAT";
						await PrepareBigFile(dataPath, 512*1024);
                        if (FileSystem.FileExists(dataPath)) {
                            dat = new DAT("LEVELS0", gameDsb, dataPath);
							hasRelocationFiles = false;
                        }
					}

					// Prepare folder names
					string lvlFolder = ConvertCase(lvlName + "/", CPA_Settings.CapsType.LevelFolder);
					string langLvlFolder = ConvertCase(lvlName + "/", CPA_Settings.CapsType.LangLevelFolder);

					// Prepare paths
					paths["fix.sna"] = levelsFolder + ConvertCase("Fix.sna", CPA_Settings.CapsType.Fix);
					paths["fix.rtb"] = levelsFolder + ConvertCase("Fix.rtb", CPA_Settings.CapsType.FixRelocation);
					paths["fix.gpt"] = levelsFolder + ConvertCase("Fix.gpt", CPA_Settings.CapsType.Fix);
					paths["fix.rtp"] = levelsFolder + ConvertCase("Fix.rtp", CPA_Settings.CapsType.FixRelocation);
					paths["fix.ptx"] = levelsFolder + ConvertCase("Fix.ptx", CPA_Settings.CapsType.Fix);
					paths["fix.rtt"] = levelsFolder + ConvertCase("Fix.rtt", CPA_Settings.CapsType.FixRelocation);
					if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R2) {
						paths["fixlvl.rtb"] = levelsFolder + lvlFolder + ConvertCase("FixLvl.rtb", CPA_Settings.CapsType.FixLvl);
					} else {
						paths["fixlvl.rtb"] = null;
					}
					if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
						paths["fix.sda"] = levelsFolder + ConvertCase("Fix.sda", CPA_Settings.CapsType.Fix);
						paths["fix.lng"] = langDataPath + ConvertCase("Fix.lng", CPA_Settings.CapsType.LangFix);
						paths["fix.rtg"] = langDataPath + ConvertCase("Fix.rtg", CPA_Settings.CapsType.FixRelocation);
						paths["fix.dlg"] = langDataPath + ConvertCase("Fix.dlg", CPA_Settings.CapsType.LangFix);
						paths["fix.rtd"] = langDataPath + ConvertCase("Fix.rtd", CPA_Settings.CapsType.FixRelocation);
						paths["fixlvl.rtg"] = langDataPath + langLvlFolder + ConvertCase("FixLvl.rtg", CPA_Settings.CapsType.FixLvl);
					} else {
						paths["fix.sda"] = null;
						paths["fix.lng"] = null;
						paths["fix.rtg"] = null;
						paths["fix.dlg"] = null;
						paths["fix.rtd"] = null;
						paths["fixlvl.rtg"] = null;
					}

					paths["lvl.sna"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sna", CPA_Settings.CapsType.LevelFile);
					paths["lvl.gpt"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".gpt", CPA_Settings.CapsType.LevelFile);
					paths["lvl.ptx"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".ptx", CPA_Settings.CapsType.LevelFile);
					if (hasRelocationFiles) {
						paths["lvl.rtb"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtb", CPA_Settings.CapsType.LevelRelocation);
						paths["lvl.rtp"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtp", CPA_Settings.CapsType.LevelRelocation);
						paths["lvl.rtt"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtt", CPA_Settings.CapsType.LevelRelocation);
					} else {
						paths["lvl.rtb"] = null;
						paths["lvl.rtp"] = null;
						paths["lvl.rtt"] = null;
					}
					if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
						paths["lvl.sda"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sda", CPA_Settings.CapsType.LevelFile);
						paths["lvl.lng"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".lng", CPA_Settings.CapsType.LangLevelFile);
						paths["lvl.rtg"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtg", CPA_Settings.CapsType.LangLevelFile);
						paths["lvl.dlg"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".dlg", CPA_Settings.CapsType.LangLevelFile);
						paths["lvl.rtd"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtd", CPA_Settings.CapsType.LangLevelRelocation);
					} else {
						paths["lvl.sda"] = null;
						paths["lvl.lng"] = null;
						paths["lvl.rtg"] = null;
						paths["lvl.dlg"] = null;
						paths["lvl.rtd"] = null;
					}
					paths["lvl.dsb"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".dsb", CPA_Settings.CapsType.DSB);
					if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R2) {
						paths["lvl.dsb"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".dsc", CPA_Settings.CapsType.DSB);
                    }

					// Download files
					foreach (KeyValuePair<string, string> path in paths) {
						if(path.Value != null) await PrepareFile(path.Value);
					}

					// LEVEL DSB
					await WaitIfNecessary();
					if (FileSystem.FileExists(paths["lvl.dsb"])) {
                        lvlDsb = new DSB(lvlName + ".dsc", paths["lvl.dsb"]);
						if (FileSystem.mode != FileSystem.Mode.Web) {
							lvlDsb.Save(levelsFolder + lvlFolder + lvlName + "_dsb.dmp");
						}
                        //lvlDsb.ReadAllSections();
                        lvlDsb.Dispose();
                    }

					// FIX
                    RelocationTable fixRtb = new RelocationTable(paths["fix.rtb"], dat, "Fix", RelocationType.RTB);
					await fixRtb.Init();
					if (FileSystem.FileExists(paths["fixlvl.rtb"])) {
						// Fix -> Lvl pointers for Tonic Trouble
						RelocationTable fixLvlRtb = new RelocationTable(paths["fixlvl.rtb"], dat, lvlName + "Fix", RelocationType.RTB);
						await fixLvlRtb.Init();
						fixRtb.Add(fixLvlRtb);
                    }
                    SNA fixSna = new SNA("Fix", paths["fix.sna"], fixRtb);
					if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal && FileSystem.DirectoryExists(langDataPath)) {
						RelocationTable fixLangRTG = new RelocationTable(paths["fix.rtg"], dat, "fixLang", RelocationType.RTG);
						await fixLangRTG.Init();
						if (FileSystem.FileExists(paths["fixlvl.rtg"])) {
							RelocationTable fixLvlRTG = new RelocationTable(paths["fixlvl.rtg"], dat, lvlName + "FixLang", RelocationType.RTG);
							await fixLvlRTG.Init();
							fixLangRTG.Add(fixLvlRTG);
                        }
                        SNA fixLangSna = new SNA("fixLang", paths["fix.lng"], fixLangRTG);
                        await WaitIfNecessary();
                        fixSna.AddSNA(fixLangSna);

                        await WaitIfNecessary();
                        RelocationTable fixRtd = new RelocationTable(paths["fix.rtd"], dat, "fixLang", RelocationType.RTD);
						await fixRtd.Init();
						fixSna.ReadDLG(paths["fix.dlg"], fixRtd);
					}
					RelocationTable fixRtp = new RelocationTable(paths["fix.rtp"], dat, "fix", RelocationType.RTP);
					await fixRtp.Init();
					fixSna.ReadGPT(paths["fix.gpt"], fixRtp);
					
					RelocationTable fixRtt = new RelocationTable(paths["fix.rtt"], dat, "fix", RelocationType.RTT);
					await fixRtt.Init();
					fixSna.ReadPTX(paths["fix.ptx"], fixRtt);
					
					if (FileSystem.FileExists(paths["fix.sda"])) {
                        fixSna.ReadSDA(paths["fix.sda"]);
                    }

					// LEVEL
                    RelocationTable lvlRtb = new RelocationTable(paths["lvl.rtb"], dat, lvlName, RelocationType.RTB);
					await lvlRtb.Init();
					SNA lvlSna = new SNA(lvlName, paths["lvl.sna"], lvlRtb);
					if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal && FileSystem.DirectoryExists(langDataPath)) {
						RelocationTable lvlLangRTG = new RelocationTable(paths["lvl.rtg"], dat, lvlName + "Lang", RelocationType.RTG);
						await lvlLangRTG.Init();
						SNA lvlLangSna = new SNA(lvlName + "Lang", paths["lvl.lng"], lvlLangRTG);
                        await WaitIfNecessary();
                        lvlSna.AddSNA(lvlLangSna);
						await WaitIfNecessary();
                        RelocationTable lvlRtd = new RelocationTable(paths["lvl.rtd"], dat, lvlName + "Lang", RelocationType.RTD);
						await lvlRtd.Init();
						lvlSna.ReadDLG(paths["lvl.dlg"], lvlRtd);
                    }

                    if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
                        RelocationTable lvlRtp = new RelocationTable(paths["lvl.rtp"], dat, lvlName, RelocationType.RTP);
						await lvlRtp.Init();
						lvlSna.ReadGPT(paths["lvl.gpt"], lvlRtp);
                        RelocationTable lvlRtt = new RelocationTable(paths["lvl.rtt"], dat, lvlName, RelocationType.RTT);
						await lvlRtt.Init();
						lvlSna.ReadPTX(paths["lvl.ptx"], lvlRtt);
                    } else {
                        lvlSna.ReadGPT(paths["lvl.gpt"], null);
                        lvlSna.ReadPTX(paths["lvl.ptx"], null);
					}
					if (FileSystem.FileExists(paths["lvl.sda"])) {
                        await WaitIfNecessary();
                        lvlSna.ReadSDA(paths["lvl.sda"]);
                    }

                    await WaitIfNecessary();
                    fixSna.CreatePointers();
                    await WaitIfNecessary();
                    lvlSna.CreatePointers();

                    files_array[0] = fixSna;
                    files_array[1] = lvlSna;
                    files_array[2] = dat;

					if (FileSystem.mode != FileSystem.Mode.Web) {
						await WaitIfNecessary();
						fixSna.CreateMemoryDump(levelsFolder + "fix.dmp", true);
						await WaitIfNecessary();
						lvlSna.CreateMemoryDump(levelsFolder + lvlFolder + lvlName + ".dmp", true);
					}

                    await LoadFIXSNA();
                    await LoadLVLSNA();

                    await WaitIfNecessary();
                    fixSna.Dispose();
                    lvlSna.Dispose();
                    if (dat != null) dat.Dispose();
                }
            } finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        if(!(files_array[i] is MemoryFile)) files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            await WaitIfNecessary();
            InitModdables();
        }

        #region FIXSNA
        async UniTask LoadFIXSNA() {
            loadingState = "Loading fixed memory";
            await WaitIfNecessary();
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            print("FIX GPT offset: " + LegacyPointer.Current(reader));
            SNA sna = (SNA)files_array[Mem.Fix];

            if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.TT) {
                // Tonic Trouble
                inputStruct = new InputStructure(null);
                uint stringCount = CPA_Settings.s.game == CPA_Settings.Game.TTSE ? 351 : (uint)gameDsb.textFiles.Sum(t => t.strings.Count);
                LegacyPointer.Read(reader);
                LegacyPointer.Read(reader);
                LegacyPointer.Read(reader);
                if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    for (int i = 0; i < 50; i++) LegacyPointer.Read(reader);
                } else {
                    for (int i = 0; i < 100; i++) LegacyPointer.Read(reader);
                }
                reader.ReadUInt32(); // 0x35
                if (CPA_Settings.s.game != CPA_Settings.Game.TTSE) reader.ReadBytes(0x80); // contains strings like MouseXPos, input related. first dword of this is a pointer to inputstructure probably
                reader.ReadBytes(0x90);
                LegacyPointer.Read(reader);
                reader.ReadUInt32(); // 0x28
                reader.ReadUInt32(); // 0x1
                if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) LegacyPointer.Read(reader);
                for (int i = 0; i < 100; i++) LegacyPointer.Read(reader);
                for (int i = 0; i < 100; i++) LegacyPointer.Read(reader);
                reader.ReadUInt32(); // 0x1
                if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    reader.ReadBytes(0xB4);
                } else {
                    if (stringCount != 598) { // English version and probably other versions have 603 strings. It's a hacky way to check which version.
                        reader.ReadBytes(0x2CC);
                    } else { // French version: 598
                        reader.ReadBytes(0x2C0);
                    }
                }
                reader.ReadBytes(0x1C);

                // Init strings
                reader.ReadUInt32(); // 0
                reader.ReadUInt32(); // 1
                reader.ReadUInt32(); // ???
                LegacyPointer.Read(reader);
                for (int i = 0; i < stringCount; i++) LegacyPointer.Read(reader); // read num_loaded_strings pointers here
                reader.ReadBytes(0xC); // dword_51A728. probably a table of some sort: 2 ptrs and a number
                if (CPA_Settings.s.game != CPA_Settings.Game.TTSE) { // There's more but what is even the point in reading all this
                    reader.ReadUInt32();
                    LegacyPointer.Read(reader);
                    reader.ReadBytes(0x14);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    LegacyPointer.Read(reader);
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadUInt32(); // 0, so can be pointer too
                    reader.ReadBytes(0x30);
                    reader.ReadBytes(0x960);
                }
            } else if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                uint num_strings = 0;
                inputStruct = new InputStructure(null);

                // SDA
                LegacyPointer.DoAt(ref reader, sna.SDA, () => {
                    print(LegacyPointer.Current(reader));
                    reader.ReadUInt32();
                    reader.ReadUInt32(); // same as next
                    num_strings = reader.ReadUInt32();
                    uint indexOfTextGlobal = reader.ReadUInt32(); // dword_6EEE78
                    uint dword_83EC58 = reader.ReadUInt32();
                    print(num_strings + " - " + LegacyPointer.Current(reader));
                });

                // DLG
                LegacyPointer.DoAt(ref reader, sna.DLG, () => {
                    LegacyPointer off_strings = LegacyPointer.Read(reader);
                    for (int i = 0; i < num_strings; i++) {
                        LegacyPointer.Read(reader);
                    }
                    reader.ReadUInt32();
                });

                // GPT
                sna.GotoHeader();
                LegacyPointer.Read(reader);
                LegacyPointer off_mainLight = LegacyPointer.Read(reader);
                uint lpPerformanceCount = reader.ReadUInt32();
                LegacyPointer.Read(reader);
                LegacyPointer off_defaultMaterial = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject1 = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject2 = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject3 = LegacyPointer.Read(reader);
                reader.ReadBytes(0x90); // FON_ related
                reader.ReadBytes(0x3D54); // FON_ related
                for (int i = 0; i < 100; i++) LegacyPointer.Read(reader); // matrix in stack
                uint matrixInStack = reader.ReadUInt32(); // number of matrix in stack
                reader.ReadBytes(0xC);
                reader.ReadBytes(0x20);
                reader.ReadUInt32();
                reader.ReadUInt32();
                LegacyPointer.Read(reader);
                LegacyPointer.Read(reader);
                for (int i = 0; i < num_strings; i++) {
                    LegacyPointer.Read(reader);
                }
                LinkedList<int> fontDefinitions = LinkedList<int>.ReadHeader(reader, LegacyPointer.Current(reader));
                LegacyPointer.Read(reader);
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                LegacyPointer off_geometricObject4 = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject5 = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject6 = LegacyPointer.Read(reader);
                LegacyPointer off_visualmaterial1 = LegacyPointer.Read(reader);
                LegacyPointer off_visualmaterial2 = LegacyPointer.Read(reader);
                for (int i = 0; i < 10; i++) {
                    LegacyPointer off_texture = LegacyPointer.Read(reader);
                }
                LegacyPointer off_visualmaterial3 = LegacyPointer.Read(reader);
                LegacyPointer off_gamematerial = LegacyPointer.Read(reader);
                uint geometricElementIndexGlobal = reader.ReadUInt32();
                LegacyPointer off_texture2 = LegacyPointer.Read(reader);
                LegacyPointer off_geometricObject7 = LegacyPointer.Read(reader);
                for (uint i = 0; i < 7; i++) {
                    LegacyPointer.Read(reader); // Material for stencils. Order: corner, border, center, side, redarrow, bullet, and another one
                }
                LegacyPointer dword_5DCB9C = LegacyPointer.Read(reader);

                // Now comes INV_fn_vSnaMultilanguageLoading


                print(LegacyPointer.Current(reader));
            } else {
                LegacyPointer off_identityMatrix = LegacyPointer.Read(reader);
                reader.ReadBytes(50 * 4);
                uint matrixInStack = reader.ReadUInt32();
                LegacyPointer off_collisionGeoObj = LegacyPointer.Read(reader);
                LegacyPointer off_staticCollisionGeoObj = LegacyPointer.Read(reader);
                loadingState = "Loading input structure";
                await WaitIfNecessary();
                for (int i = 0; i < CPA_Settings.s.numEntryActions; i++) {
                    LegacyPointer.Read(reader); // 3DOS_EntryActions
                }
                LegacyPointer off_IPT_keyAndPadDefine = LegacyPointer.Read(reader);
                ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);

                inputStruct = InputStructure.Read(reader, LegacyPointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				print("Num entractions: " + inputStruct.num_entryActions);
                print("Off entryactions: " + inputStruct.off_entryActions);
                LegacyPointer off_IPT_entryElementList = LegacyPointer.Read(reader);
                print("Off entryelements: " + off_IPT_entryElementList);

                loadingState = "Loading text";
                await WaitIfNecessary();
                localization = FromOffsetOrRead<LocalizationStructure>(reader, LegacyPointer.Current(reader), inline: true); // FON_g_stGeneral

                loadingState = "Loading fixed animation bank";
                await WaitIfNecessary();
                animationBanks = new AnimationBank[2]; // 1 in fix, 1 in lvl
                animationBanks[0] = AnimationBank.Read(reader, LegacyPointer.Current(reader), 0, 1, files_array[Mem.FixKeyFrames])[0];
                print("Fix animation bank: " + animationBanks[0].off_header);
                LegacyPointer off_fixInfo = LegacyPointer.Read(reader);
            }

            // Read PTX
            loadingState = "Loading fixed textures";
            await WaitIfNecessary();
			// Can't yield inside a lambda, so we must do it the old fashioned way, with off_current
			if (sna.PTX != null) {
				LegacyPointer off_current = LegacyPointer.Goto(ref reader, sna.PTX);
				await ReadTexturesFix(reader, LegacyPointer.Current(reader));
				LegacyPointer.Goto(ref reader, off_current);
			}
            /*Pointer.DoAt(ref reader, sna.PTX, () => {
                ReadTexturesFix(reader, Pointer.Current(reader));
            });*/
        }
        #endregion

        #region LVLSNA
        async UniTask LoadLVLSNA() {
            loadingState = "Loading level memory";
            await WaitIfNecessary();
            Reader reader = files_array[Mem.Lvl].reader;
            LegacyPointer off_current;
            SNA sna = (SNA)files_array[Mem.Lvl];

            // First read GPT
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;
            print("LVL GPT offset: " + LegacyPointer.Current(reader));

            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {

                // SDA
                /*sna.GotoSDA();
                print(Pointer.Current(reader));
                reader.ReadUInt32();
                reader.ReadUInt32(); // same as next
                uint num_strings = reader.ReadUInt32();
                uint indexOfTextGlobal = reader.ReadUInt32(); // dword_6EEE78
                uint dword_83EC58 = reader.ReadUInt32();
                print(num_strings + " - " + Pointer.Current(reader));

                // DLG
                sna.GotoDLG();
                Pointer off_strings = Pointer.Read(reader);
                for (int i = 0; i < num_strings; i++) {
                    Pointer.Read(reader);
                }
                reader.ReadUInt32();*/

                // GPT
                sna.GotoHeader();
                if (CPA_Settings.s.game != CPA_Settings.Game.PlaymobilLaura) {
                    LegacyPointer.Read(reader); // sound related
                }
                LegacyPointer.Read(reader);
                LegacyPointer.Read(reader);
                reader.ReadUInt32();
            }
            if (CPA_Settings.s.engineVersion != CPA_Settings.EngineVersion.Montreal) {
                loadingState = "Reading settings for persos in fix";
                await WaitIfNecessary();
                // Fill in fix -> lvl pointers for perso's in fix
                uint num_persoInFixPointers = reader.ReadUInt32();
                LegacyPointer[] persoInFixPointers = new LegacyPointer[num_persoInFixPointers];
                for (int i = 0; i < num_persoInFixPointers; i++) {
                    LegacyPointer off_perso = LegacyPointer.Read(reader);
                    if (off_perso != null) {
                        off_current = LegacyPointer.Goto(ref reader, off_perso);
                        reader.ReadUInt32();
                        LegacyPointer off_stdGame = LegacyPointer.Read(reader);
                        if (off_stdGame != null) {
                            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
                                LegacyPointer.Goto(ref reader, off_stdGame);
                                reader.ReadUInt32(); // type 0
                                reader.ReadUInt32(); // type 1
                                reader.ReadUInt32(); // type 2
                                LegacyPointer off_superObject = LegacyPointer.Read(reader);
                                LegacyPointer.Goto(ref reader, off_current);
                                if (off_superObject == null) continue;
                            } else {
                                LegacyPointer.Goto(ref reader, off_current);
                            }
                            // First read everything from the GPT
                            LegacyPointer off_newSuperObject = null, off_nextBrother = null, off_prevBrother = null, off_father = null;
                            byte[] matrixData = null, floatData = null, renderBits = null;
                            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
                                off_newSuperObject = LegacyPointer.Read(reader);
                                matrixData = reader.ReadBytes(0x58);
                                renderBits = reader.ReadBytes(4);
                                floatData = reader.ReadBytes(4);
                                off_nextBrother = LegacyPointer.Read(reader);
                                off_prevBrother = LegacyPointer.Read(reader);
                                off_father = LegacyPointer.Read(reader);
                            } else {
                                matrixData = reader.ReadBytes(0x58);
                                off_newSuperObject = LegacyPointer.Read(reader);
                                LegacyPointer.DoAt(ref reader, off_stdGame + 0xC, () => {
                                    ((SNA)off_stdGame.file).AddPointer(off_stdGame.offset + 0xC, off_newSuperObject);
                                });
                            }

                            // Then fill everything in
                            off_current = LegacyPointer.Goto(ref reader, off_newSuperObject);
                            uint newSOtype = reader.ReadUInt32();
                            LegacyPointer off_newSOengineObject = LegacyPointer.Read(reader);
                            if (SuperObject.GetSOType(newSOtype) == SuperObject.Type.Perso) {
                                persoInFixPointers[i] = off_newSOengineObject;
                                LegacyPointer.Goto(ref reader, off_newSOengineObject);
								LegacyPointer off_p3dData = LegacyPointer.Read(reader);
                                if (CPA_Settings.s.game == CPA_Settings.Game.R2Demo) {
                                    ((SNA)off_p3dData.file).OverwriteData(off_p3dData.FileOffset + 0x1C, matrixData);
                                } else {
                                    ((SNA)off_p3dData.file).OverwriteData(off_p3dData.FileOffset + 0x18, matrixData);
                                }

                                if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.TT) {
                                    FileWithPointers file = off_newSuperObject.file;
                                    file.AddPointer(off_newSuperObject.FileOffset + 0x14, off_nextBrother);
                                    file.AddPointer(off_newSuperObject.FileOffset + 0x18, off_prevBrother);
                                    file.AddPointer(off_newSuperObject.FileOffset + 0x1C, off_father);
                                    ((SNA)file).OverwriteData(off_newSuperObject.FileOffset + 0x30, renderBits);
                                    ((SNA)file).OverwriteData(off_newSuperObject.FileOffset + 0x38, floatData);
                                }
                            } else {
                                persoInFixPointers[i] = null;
                            }

                        }
                        LegacyPointer.Goto(ref reader, off_current);
                    }
                }
            }
            loadingState = "Loading globals";
            await WaitIfNecessary();
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                globals.off_actualWorld = LegacyPointer.Read(reader);
                globals.off_dynamicWorld = LegacyPointer.Read(reader);
                globals.off_inactiveDynamicWorld = LegacyPointer.Read(reader);
                globals.off_fatherSector = LegacyPointer.Read(reader);
                globals.off_firstSubMapPosition = LegacyPointer.Read(reader);
            } else {
                globals.off_actualWorld = LegacyPointer.Read(reader);
                globals.off_dynamicWorld = LegacyPointer.Read(reader);
                globals.off_fatherSector = LegacyPointer.Read(reader);
                uint soundEventIndex = reader.ReadUInt32(); // In Montreal version this is a pointer, also sound event related
                if (CPA_Settings.s.game == CPA_Settings.Game.PlaymobilLaura) {
                    LegacyPointer.Read(reader);
                }
            }

            globals.num_always = reader.ReadUInt32();
            globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, LegacyPointer.Current(reader), LinkedList.Type.Double);
            globals.off_always_reusableSO = LegacyPointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                globals.off_always_reusableUnknown1 = LegacyPointer.Read(reader); // (num_always) * 0x2c blocks
                globals.off_always_reusableUnknown2 = LegacyPointer.Read(reader); // (num_always) * 0x4 blocks
            } else {
                reader.ReadUInt32(); // 0x6F. In Montreal version this is a pointer to a pointer table for always
                globals.spawnablePersos.FillPointers(reader, globals.spawnablePersos.off_tail, globals.spawnablePersos.offset);
            }

            if (CPA_Settings.s.game == CPA_Settings.Game.DD) reader.ReadUInt32();
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                LegacyPointer dword_4A6B1C_always_header = LegacyPointer.Read(reader);
                LegacyPointer dword_4A6B20_always_last = LegacyPointer.Read(reader);

                LegacyPointer v28 = LegacyPointer.Read(reader);
                LegacyPointer v31 = LegacyPointer.Read(reader);
                LegacyPointer v32 = LegacyPointer.Read(reader);
                LegacyPointer v33 = LegacyPointer.Read(reader);

                // These things aren't parsed, but in raycap they're null. This way we'll notice when they aren't.
                if (v28 != null) print("v28 is not null, it's " + v28);
                if (v31 != null) print("v31 is not null, it's " + v31);
                if (v32 != null) print("v32 is not null, it's " + v32);
                if (v33 != null) print("v33 is not null, it's " + v33);

                // Fill in pointers for the unknown table related to "always".
                FillLinkedListPointers(reader, dword_4A6B20_always_last, dword_4A6B1C_always_header);
            }

            // Fill in pointers for the object type tables and read them
            objectTypes = new ObjectType[3][];
            for (uint i = 0; i < 3; i++) {
                LegacyPointer off_names_header = LegacyPointer.Current(reader);
                LegacyPointer off_names_first = LegacyPointer.Read(reader);
                LegacyPointer off_names_last = LegacyPointer.Read(reader);
                uint num_names = reader.ReadUInt32();

                FillLinkedListPointers(reader, off_names_last, off_names_header);
                ReadObjectNamesTable(reader, off_names_first, num_names, i);
            }

            // Begin of engineStructure
            loadingState = "Loading engine structure";
            await WaitIfNecessary();
            print("Start of EngineStructure: " + LegacyPointer.Current(reader));
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                reader.ReadByte();
                string mapName = reader.ReadString(0x1E);
                reader.ReadChars(0x1E);
                string mapName2 = reader.ReadString(0x1E);
                reader.ReadByte();
                reader.ReadBytes(0x178); // don't know what this data is
            } else {
                reader.ReadByte();
                string mapName = reader.ReadString(0x104);
                reader.ReadChars(0x104);
                string mapName2 = reader.ReadString(0x104);
                if (CPA_Settings.s.game == CPA_Settings.Game.PlaymobilLaura) {
                    reader.ReadChars(0x104);
                    reader.ReadChars(0x104);
                }
                string mapName3 = reader.ReadString(0x104);
                if (CPA_Settings.s.game == CPA_Settings.Game.TT) {
                    reader.ReadBytes(0x47F7); // don't know what this data is
                } else if (CPA_Settings.s.game == CPA_Settings.Game.TTSE) {
                    reader.ReadBytes(0x240F);
                } else if (CPA_Settings.s.game == CPA_Settings.Game.PlaymobilLaura) {
                    reader.ReadBytes(0x240F); // don't know what this data is
                } else { // Hype & Alex
                    reader.ReadBytes(0x2627); // don't know what this data is
                }
            }
            LegacyPointer off_unknown_first = LegacyPointer.Read(reader);
            LegacyPointer off_unknown_last = LegacyPointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();

            families = LinkedList<Family>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);
            families.FillPointers(reader, families.off_tail, families.off_head);

            if (CPA_Settings.s.game == CPA_Settings.Game.PlaymobilLaura) {
                LinkedList<int>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);
            }

            LinkedList<SuperObject> alwaysActiveCharacters = LinkedList<SuperObject>.ReadHeader(reader, LegacyPointer.Current(reader), type: LinkedList.Type.Double);

            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                if (CPA_Settings.s.game == CPA_Settings.Game.RedPlanet ||CPA_Settings.s.game == CPA_Settings.Game.R2Demo) reader.ReadUInt32();
                LegacyPointer off_languages = LegacyPointer.Read(reader);
                reader.ReadUInt32();

                LegacyPointer.DoAt(ref reader, off_languages, () => {
                    ReadLanguages(reader, off_languages, localization.num_languages);
                });

                for (uint i = 0; i < 2; i++) {
                    LegacyPointer off_matrix = LegacyPointer.Current(reader);
                    Matrix mat = Matrix.Read(reader, off_matrix);
                }

                reader.ReadUInt32();
                reader.ReadUInt16();

                ReadLevelNames(reader, LegacyPointer.Current(reader), 80);
                uint num_mapNames = reader.ReadUInt32();
                Array.Resize(ref levels, (int)num_mapNames);
                reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadUInt32();

                if (CPA_Settings.s.game == CPA_Settings.Game.DD) {
                    reader.ReadUInt32();
                }

                // End of engineStructure
                LegacyPointer off_light = LegacyPointer.Read(reader); // the offset of a light. It's just an ordinary light.
                LegacyPointer off_mainChar = LegacyPointer.Read(reader); // superobject
                LegacyPointer off_characterLaunchingSoundEvents = LegacyPointer.Read(reader);
                if (CPA_Settings.s.game == CPA_Settings.Game.DD) {
                    globals.off_backgroundGameMaterial = LegacyPointer.Read(reader);
                }
                LegacyPointer off_shadowPolygonVisualMaterial = LegacyPointer.Read(reader);
                LegacyPointer off_shadowPolygonGameMaterialInit = LegacyPointer.Read(reader);
                LegacyPointer off_shadowPolygonGameMaterial = LegacyPointer.Read(reader);
                LegacyPointer off_textureOfTextureShadow = LegacyPointer.Read(reader);
                LegacyPointer off_col_taggedFacesTable = LegacyPointer.Read(reader);

                for (int i = 0; i < 10; i++) {
                    LegacyPointer off_elementForShadow = LegacyPointer.Read(reader);
                    LegacyPointer off_geometricShadowObject = LegacyPointer.Read(reader);
                }
                LegacyPointer.Read(reader); // DemoSOList

                if (CPA_Settings.s.game == CPA_Settings.Game.R2Demo || CPA_Settings.s.game == CPA_Settings.Game.RedPlanet || CPA_Settings.s.mode == CPA_Settings.Mode.DonaldDuckPCDemo) {
                    LegacyPointer.Read(reader);
                }

                if (CPA_Settings.s.mode == CPA_Settings.Mode.DonaldDuckPCDemo) {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }

                loadingState = "Loading level animation bank";
                //print("Animation bank: " + Pointer.Current(reader));
                await WaitIfNecessary();
                AnimationBank.Read(reader, LegacyPointer.Current(reader), 0, 1, files_array[Mem.LvlKeyFrames], append: true);
                animationBanks[1] = animationBanks[0];
            }
            if (FileSystem.mode != FileSystem.Mode.Web) {
                string levelsFolder = gameDataBinFolder + ConvertPath(gameDsb.levelsDataPath) + "/";
                ((SNA)files_array[0]).CreateMemoryDump(levelsFolder + "fix.dmp", true);
                ((SNA)files_array[1]).CreateMemoryDump(levelsFolder + lvlName + "/" + lvlName + ".dmp", true);
            }

            // Read PTX
            loadingState = "Loading level textures";
            await WaitIfNecessary();

			// Can't yield inside a lambda, so we must do it the old fashioned way, with off_current
			if (sna.PTX != null) {
				off_current = LegacyPointer.Goto(ref reader, sna.PTX);
				await ReadTexturesLvl(reader, LegacyPointer.Current(reader));
				LegacyPointer.Goto(ref reader, off_current);
			}
			/*Pointer.DoAt(ref reader, sna.PTX, () => {
                ReadTexturesLvl(reader, Pointer.Current(reader));
            });*/

            // Read background game material (DD only)
            globals.backgroundGameMaterial = GameMaterial.FromOffsetOrRead(globals.off_backgroundGameMaterial, reader);

            // Parse actual world & always structure
            loadingState = "Loading families";
            await WaitIfNecessary();
            ReadFamilies(reader);

            loadingState = "Creating animation bank";
            await WaitIfNecessary();
            if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.Montreal) {
                animationBanks = new AnimationBank[2];
                animationBanks[0] = new AnimationBank(null) {
                    animations = new Animation.Component.AnimA3DGeneral[0]
                };
                animationBanks[1] = animationBanks[0];
            } else if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.TT) {
                uint maxAnimIndex = 0;
                foreach (State s in states) {
                    if (s.anim_ref != null && s.anim_ref.anim_index > maxAnimIndex) maxAnimIndex = s.anim_ref.anim_index;
                }
                animationBanks = new AnimationBank[2];
                animationBanks[0] = new AnimationBank(null) {
                    animations = new Animation.Component.AnimA3DGeneral[maxAnimIndex + 1]
                };
                foreach (State s in states) {
                    if (s.anim_ref != null) animationBanks[0].animations[s.anim_ref.anim_index] = s.anim_ref.a3d;
                }
                animationBanks[1] = animationBanks[0];
            }

            loadingState = "Loading superobject hierarchy";
            await WaitIfNecessary();
            await ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            await WaitIfNecessary();
            ReadAlways(reader);
            loadingState = "Filling in cross-references";
            await WaitIfNecessary();
            ReadCrossReferences(reader);

			// TODO: Make more generic
			if (CPA_Settings.s.game == CPA_Settings.Game.R2) {
				loadingState = "Filling in comport names";
				await WaitIfNecessary();
				string path = gameDataBinFolder + "R2DC_Comports.json";

                if (!FileSystem.FileExists(path)) {
                    path = "Assets/StreamingAssets/R2DC_Comports.json"; // Offline, the json doesn't exist, so grab it from StreamingAssets
                }

                Stream stream = FileSystem.GetFileReadStream(path);
				if (stream != null) {
					ReadAndFillComportNames(stream);
				}
			}
		}
        #endregion
        
    }
}
