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

namespace OpenSpace.Loader {
    public class R2Loader : MapLoader {
        public override IEnumerator Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				yield return controller.StartCoroutine(FileSystem.CheckDirectory(gameDataBinFolder));
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");

                loadingState = "Initializing files";
                yield return null;
                string gameDsbPath = gameDataBinFolder + ConvertCase("Game.dsb", Settings.CapsType.DSB);
                if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                    gameDsbPath = gameDataBinFolder + ConvertCase("gamedsc.bin", Settings.CapsType.DSB);
                } else if (Settings.s.game == Settings.Game.TTSE) {
                    gameDsbPath = gameDataBinFolder + ConvertCase("GAME.DSC", Settings.CapsType.DSB);
				}
				yield return controller.StartCoroutine(PrepareFile(gameDsbPath));
				gameDsb = new DSB("Game", gameDsbPath);
				if (FileSystem.mode != FileSystem.Mode.Web) {
					gameDsb.Save(gameDataBinFolder + ConvertCase("Game_dsb.dmp", Settings.CapsType.DSB));
				}
                gameDsb.ReadAllSections();
                gameDsb.Dispose();

				yield return controller.StartCoroutine(CreateCNT());

				if (Settings.s.game == Settings.Game.R2) {
					string comportsPath = gameDataBinFolder + "R2DC_Comports.json";
					yield return controller.StartCoroutine(PrepareFile(comportsPath));
				}

				if (lvlName.EndsWith(".exe")) {
                    if (!Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
                    Settings.s.loadFromMemory = true;
                    MemoryFile mem = new MemoryFile(lvlName);
                    files_array[0] = mem;
                    yield return null;
                    LoadMemory();
                } else {
                    hasTransit = false;
                    DAT dat = null;

                    string levelsFolder = gameDataBinFolder + ConvertCase(gameDsb.levelsDataPath, Settings.CapsType.All) + "/";
                    string langDataPath = gameDataBinFolder
						+ ConvertCase("../LangData/English/", Settings.CapsType.All)
						+ ConvertCase(gameDsb.levelsDataPath, Settings.CapsType.All) + "/";
					if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
						yield return controller.StartCoroutine(FileSystem.CheckDirectory(langDataPath));
						if (FileSystem.mode != FileSystem.Mode.Web && !FileSystem.DirectoryExists(langDataPath)) {
							string langPath = gameDataBinFolder + ConvertCase("../LangData/", Settings.CapsType.All);
							yield return controller.StartCoroutine(FileSystem.CheckDirectory(langPath));
							if (FileSystem.DirectoryExists(langPath)) {
								DirectoryInfo dirInfo = new DirectoryInfo(langPath);
								DirectoryInfo firstLang = dirInfo.GetDirectories().FirstOrDefault();
								if (firstLang != null) {
									langDataPath = firstLang.FullName + "/" + ConvertCase(gameDsb.levelsDataPath, Settings.CapsType.All) + "/";
									yield return controller.StartCoroutine(FileSystem.CheckDirectory(langDataPath));
								}
							}
						}
					}

                    yield return null;
					bool hasRelocationFiles = true;
                    if (Settings.s.mode == Settings.Mode.Rayman2PC || Settings.s.mode == Settings.Mode.DonaldDuckPC) {
                        string dataPath = levelsFolder + "LEVELS0.DAT";
						yield return controller.StartCoroutine(PrepareBigFile(dataPath, 512*1024));
                        if (FileSystem.FileExists(dataPath)) {
                            dat = new DAT("LEVELS0", gameDsb, dataPath);
							hasRelocationFiles = false;
                        }
					}

					// Prepare folder names
					string lvlFolder = ConvertCase(lvlName + "/", Settings.CapsType.LevelFolder);
					string langLvlFolder = ConvertCase(lvlName + "/", Settings.CapsType.LangLevelFolder);

					// Prepare paths
					Dictionary<string, string> paths = new Dictionary<string, string>();
					paths["fix.sna"] = levelsFolder + ConvertCase("Fix.sna", Settings.CapsType.Fix);
					paths["fix.rtb"] = levelsFolder + ConvertCase("Fix.rtb", Settings.CapsType.FixRelocation);
					paths["fix.gpt"] = levelsFolder + ConvertCase("Fix.gpt", Settings.CapsType.Fix);
					paths["fix.rtp"] = levelsFolder + ConvertCase("Fix.rtp", Settings.CapsType.FixRelocation);
					paths["fix.ptx"] = levelsFolder + ConvertCase("Fix.ptx", Settings.CapsType.Fix);
					paths["fix.rtt"] = levelsFolder + ConvertCase("Fix.rtt", Settings.CapsType.FixRelocation);
					if (Settings.s.engineVersion < Settings.EngineVersion.R2) {
						paths["fixlvl.rtb"] = levelsFolder + lvlFolder + ConvertCase("FixLvl.rtb", Settings.CapsType.FixLvl);
					} else {
						paths["fixlvl.rtb"] = null;
					}
					if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
						paths["fix.sda"] = levelsFolder + ConvertCase("Fix.sda", Settings.CapsType.Fix);
						paths["fix.lng"] = langDataPath + ConvertCase("Fix.lng", Settings.CapsType.LangFix);
						paths["fix.rtg"] = langDataPath + ConvertCase("Fix.rtg", Settings.CapsType.FixRelocation);
						paths["fix.dlg"] = langDataPath + ConvertCase("Fix.dlg", Settings.CapsType.LangFix);
						paths["fix.rtd"] = langDataPath + ConvertCase("Fix.rtd", Settings.CapsType.FixRelocation);
						paths["fixlvl.rtg"] = langDataPath + lvlFolder + ConvertCase("FixLvl.rtg", Settings.CapsType.FixLvl);
					} else {
						paths["fix.sda"] = null;
						paths["fix.lng"] = null;
						paths["fix.rtg"] = null;
						paths["fix.dlg"] = null;
						paths["fix.rtd"] = null;
						paths["fixlvl.rtg"] = null;
					}

					paths["lvl.sna"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sna", Settings.CapsType.LevelFile);
					paths["lvl.gpt"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".gpt", Settings.CapsType.LevelFile);
					paths["lvl.ptx"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".ptx", Settings.CapsType.LevelFile);
					if (hasRelocationFiles) {
						paths["lvl.rtb"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtb", Settings.CapsType.LevelFile);
						paths["lvl.rtp"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtp", Settings.CapsType.LevelFile);
						paths["lvl.rtt"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".rtt", Settings.CapsType.LevelFile);
					} else {
						paths["lvl.rtb"] = null;
						paths["lvl.rtp"] = null;
						paths["lvl.rtt"] = null;
					}
					if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
						paths["lvl.sda"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".sda", Settings.CapsType.LevelFile);
						paths["lvl.lng"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".lng", Settings.CapsType.LangLevelFile);
						paths["lvl.rtg"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtg", Settings.CapsType.LangLevelFile);
						paths["lvl.dlg"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".dlg", Settings.CapsType.LangLevelFile);
						paths["lvl.rtd"] = langDataPath + langLvlFolder + ConvertCase(lvlName + ".rtd", Settings.CapsType.LangLevelFile);
					} else {
						paths["lvl.sda"] = null;
						paths["lvl.lng"] = null;
						paths["lvl.rtg"] = null;
						paths["lvl.dlg"] = null;
						paths["lvl.rtd"] = null;
					}
					paths["lvl.dsb"] = levelsFolder + lvlFolder + ConvertCase(lvlName + ".dsb", Settings.CapsType.DSB);
					if (Settings.s.engineVersion < Settings.EngineVersion.R2) {
						paths["lvl.dsb"] = levelsFolder + lvlFolder + lvlName + ".DSC";
					}

					// Download files
					foreach (KeyValuePair<string, string> path in paths) {
						if(path.Value != null) yield return controller.StartCoroutine(PrepareFile(path.Value));
					}

					// LEVEL DSB
					yield return null;
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
					yield return controller.StartCoroutine(fixRtb.Init());
					if (FileSystem.FileExists(paths["fixlvl.rtb"])) {
						// Fix -> Lvl pointers for Tonic Trouble
						RelocationTable fixLvlRtb = new RelocationTable(paths["fixlvl.rtb"], dat, lvlName + "Fix", RelocationType.RTB);
						yield return controller.StartCoroutine(fixLvlRtb.Init());
						fixRtb.Add(fixLvlRtb);
                    }
                    SNA fixSna = new SNA("Fix", paths["fix.sna"], fixRtb);
					if (Settings.s.engineVersion == Settings.EngineVersion.Montreal && FileSystem.DirectoryExists(langDataPath)) {
						RelocationTable fixLangRTG = new RelocationTable(paths["fix.rtg"], dat, "fixLang", RelocationType.RTG);
						yield return controller.StartCoroutine(fixLangRTG.Init());
						if (FileSystem.FileExists(paths["fixlvl.rtg"])) {
							RelocationTable fixLvlRTG = new RelocationTable(paths["fixlvl.rtg"], dat, lvlName + "FixLang", RelocationType.RTG);
							yield return controller.StartCoroutine(fixLvlRTG.Init());
							fixLangRTG.Add(fixLvlRTG);
                        }
                        SNA fixLangSna = new SNA("fixLang", paths["fix.lng"], fixLangRTG);
                        yield return null;
                        fixSna.AddSNA(fixLangSna);

                        yield return null;
                        RelocationTable fixRtd = new RelocationTable(paths["fix.rtd"], dat, "fixLang", RelocationType.RTD);
						yield return controller.StartCoroutine(fixRtd.Init());
						fixSna.ReadDLG(paths["fix.dlg"], fixRtd);
					}
					RelocationTable fixRtp = new RelocationTable(paths["fix.rtp"], dat, "fix", RelocationType.RTP);
					yield return controller.StartCoroutine(fixRtp.Init());
					fixSna.ReadGPT(paths["fix.gpt"], fixRtp);
					
					RelocationTable fixRtt = new RelocationTable(paths["fix.rtt"], dat, "fix", RelocationType.RTT);
					yield return controller.StartCoroutine(fixRtt.Init());
					fixSna.ReadPTX(paths["fix.ptx"], fixRtt);
					
					if (FileSystem.FileExists(paths["fix.sda"])) {
                        fixSna.ReadSDA(paths["fix.sda"]);
                    }

					// LEVEL
                    RelocationTable lvlRtb = new RelocationTable(paths["lvl.rtb"], dat, lvlName, RelocationType.RTB);
					yield return controller.StartCoroutine(lvlRtb.Init());
					SNA lvlSna = new SNA(lvlName, paths["lvl.sna"], lvlRtb);
					if (Settings.s.engineVersion == Settings.EngineVersion.Montreal && FileSystem.DirectoryExists(langDataPath)) {
						RelocationTable lvlLangRTG = new RelocationTable(paths["lvl.rtg"], dat, lvlName + "Lang", RelocationType.RTG);
						yield return controller.StartCoroutine(lvlLangRTG.Init());
						SNA lvlLangSna = new SNA(lvlName + "Lang", paths["lvl.lng"], lvlLangRTG);
                        yield return null;
                        lvlSna.AddSNA(lvlLangSna);
						yield return null;
                        RelocationTable lvlRtd = new RelocationTable(paths["lvl.rtd"], dat, lvlName + "Lang", RelocationType.RTD);
						yield return controller.StartCoroutine(lvlRtd.Init());
						lvlSna.ReadDLG(paths["lvl.dlg"], lvlRtd);
                    }

                    if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                        RelocationTable lvlRtp = new RelocationTable(paths["lvl.rtp"], dat, lvlName, RelocationType.RTP);
						yield return controller.StartCoroutine(lvlRtp.Init());
						lvlSna.ReadGPT(paths["lvl.gpt"], lvlRtp);
                        RelocationTable lvlRtt = new RelocationTable(paths["lvl.rtt"], dat, lvlName, RelocationType.RTT);
						yield return controller.StartCoroutine(lvlRtt.Init());
						lvlSna.ReadPTX(paths["lvl.ptx"], lvlRtt);
                    } else {
                        lvlSna.ReadGPT(paths["lvl.gpt"], null);
                        lvlSna.ReadPTX(paths["lvl.ptx"], null);
					}
					if (FileSystem.FileExists(paths["lvl.sda"])) {
                        yield return null;
                        lvlSna.ReadSDA(paths["lvl.sda"]);
                    }

                    yield return null;
                    fixSna.CreatePointers();
                    yield return null;
                    lvlSna.CreatePointers();

                    files_array[0] = fixSna;
                    files_array[1] = lvlSna;
                    files_array[2] = dat;

					if (FileSystem.mode != FileSystem.Mode.Web) {
						yield return null;
						fixSna.CreateMemoryDump(levelsFolder + "fix.dmp", true);
						yield return null;
						lvlSna.CreateMemoryDump(levelsFolder + lvlFolder + lvlName + ".dmp", true);
					}

                    yield return controller.StartCoroutine(LoadFIXSNA());
                    yield return controller.StartCoroutine(LoadLVLSNA());

                    yield return null;
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
            yield return null;
            InitModdables();
        }

        #region FIXSNA
        IEnumerator LoadFIXSNA() {
            loadingState = "Loading fixed memory";
            yield return null;
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            print("FIX GPT offset: " + Pointer.Current(reader));
            SNA sna = (SNA)files_array[Mem.Fix];

            if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
                // Tonic Trouble
                inputStruct = new InputStructure(null);
                uint stringCount = Settings.s.game == Settings.Game.TTSE ? 351 : (uint)gameDsb.textFiles.Sum(t => t.strings.Count);
                Pointer.Read(reader);
                Pointer.Read(reader);
                Pointer.Read(reader);
                if (Settings.s.game == Settings.Game.TTSE) {
                    for (int i = 0; i < 50; i++) Pointer.Read(reader);
                } else {
                    for (int i = 0; i < 100; i++) Pointer.Read(reader);
                }
                reader.ReadUInt32(); // 0x35
                if (Settings.s.game != Settings.Game.TTSE) reader.ReadBytes(0x80); // contains strings like MouseXPos, input related. first dword of this is a pointer to inputstructure probably
                reader.ReadBytes(0x90);
                Pointer.Read(reader);
                reader.ReadUInt32(); // 0x28
                reader.ReadUInt32(); // 0x1
                if (Settings.s.game == Settings.Game.TTSE) Pointer.Read(reader);
                for (int i = 0; i < 100; i++) Pointer.Read(reader);
                for (int i = 0; i < 100; i++) Pointer.Read(reader);
                reader.ReadUInt32(); // 0x1
                if (Settings.s.game == Settings.Game.TTSE) {
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
                Pointer.Read(reader);
                for (int i = 0; i < stringCount; i++) Pointer.Read(reader); // read num_loaded_strings pointers here
                reader.ReadBytes(0xC); // dword_51A728. probably a table of some sort: 2 ptrs and a number
                if (Settings.s.game != Settings.Game.TTSE) { // There's more but what is even the point in reading all this
                    reader.ReadUInt32();
                    Pointer.Read(reader);
                    reader.ReadBytes(0x14);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
                    Pointer.Read(reader);
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
            } else if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                uint num_strings = 0;
                inputStruct = new InputStructure(null);

                // SDA
                Pointer.DoAt(ref reader, sna.SDA, () => {
                    print(Pointer.Current(reader));
                    reader.ReadUInt32();
                    reader.ReadUInt32(); // same as next
                    num_strings = reader.ReadUInt32();
                    uint indexOfTextGlobal = reader.ReadUInt32(); // dword_6EEE78
                    uint dword_83EC58 = reader.ReadUInt32();
                    print(num_strings + " - " + Pointer.Current(reader));
                });

                // DLG
                Pointer.DoAt(ref reader, sna.DLG, () => {
                    Pointer off_strings = Pointer.Read(reader);
                    for (int i = 0; i < num_strings; i++) {
                        Pointer.Read(reader);
                    }
                    reader.ReadUInt32();
                });

                // GPT
                sna.GotoHeader();
                Pointer.Read(reader);
                Pointer off_mainLight = Pointer.Read(reader);
                uint lpPerformanceCount = reader.ReadUInt32();
                Pointer.Read(reader);
                Pointer off_defaultMaterial = Pointer.Read(reader);
                Pointer off_geometricObject1 = Pointer.Read(reader);
                Pointer off_geometricObject2 = Pointer.Read(reader);
                Pointer off_geometricObject3 = Pointer.Read(reader);
                reader.ReadBytes(0x90); // FON_ related
                reader.ReadBytes(0x3D54); // FON_ related
                for (int i = 0; i < 100; i++) Pointer.Read(reader); // matrix in stack
                uint matrixInStack = reader.ReadUInt32(); // number of matrix in stack
                reader.ReadBytes(0xC);
                reader.ReadBytes(0x20);
                reader.ReadUInt32();
                reader.ReadUInt32();
                Pointer.Read(reader);
                Pointer.Read(reader);
                for (int i = 0; i < num_strings; i++) {
                    Pointer.Read(reader);
                }
                LinkedList<int> fontDefinitions = LinkedList<int>.ReadHeader(reader, Pointer.Current(reader));
                Pointer.Read(reader);
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                Pointer off_geometricObject4 = Pointer.Read(reader);
                Pointer off_geometricObject5 = Pointer.Read(reader);
                Pointer off_geometricObject6 = Pointer.Read(reader);
                Pointer off_visualmaterial1 = Pointer.Read(reader);
                Pointer off_visualmaterial2 = Pointer.Read(reader);
                for (int i = 0; i < 10; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                }
                Pointer off_visualmaterial3 = Pointer.Read(reader);
                Pointer off_gamematerial = Pointer.Read(reader);
                uint geometricElementIndexGlobal = reader.ReadUInt32();
                Pointer off_texture2 = Pointer.Read(reader);
                Pointer off_geometricObject7 = Pointer.Read(reader);
                for (uint i = 0; i < 7; i++) {
                    Pointer.Read(reader); // Material for stencils. Order: corner, border, center, side, redarrow, bullet, and another one
                }
                Pointer dword_5DCB9C = Pointer.Read(reader);

                // Now comes INV_fn_vSnaMultilanguageLoading


                print(Pointer.Current(reader));
            } else {
                Pointer off_identityMatrix = Pointer.Read(reader);
                reader.ReadBytes(50 * 4);
                uint matrixInStack = reader.ReadUInt32();
                Pointer off_collisionGeoObj = Pointer.Read(reader);
                Pointer off_staticCollisionGeoObj = Pointer.Read(reader);

                loadingState = "Loading input structure";
                yield return null;
                for (int i = 0; i < Settings.s.numEntryActions; i++) {
                    Pointer.Read(reader); // 3DOS_EntryActions
                }
                Pointer off_IPT_keyAndPadDefine = Pointer.Read(reader);
                ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);

                inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
				foreach (EntryAction ea in inputStruct.entryActions) {
					print(ea.ToString());
				}
				print("Num entractions: " + inputStruct.num_entryActions);
                print("Off entryactions: " + inputStruct.off_entryActions);
                Pointer off_IPT_entryElementList = Pointer.Read(reader);
                print("Off entryelements: " + off_IPT_entryElementList);

                loadingState = "Loading text";
                yield return null;
                fontStruct = FontStructure.Read(reader, Pointer.Current(reader)); // FON_g_stGeneral

                loadingState = "Loading fixed animation bank";
                yield return null;
                animationBanks = new AnimationBank[2]; // 1 in fix, 1 in lvl
                animationBanks[0] = AnimationBank.Read(reader, Pointer.Current(reader), 0, 1, files_array[Mem.FixKeyFrames])[0];
                print("Fix animation bank: " + animationBanks[0].off_header);
                Pointer off_fixInfo = Pointer.Read(reader);
            }

            // Read PTX
            loadingState = "Loading fixed textures";
            yield return null;
			// Can't yield inside a lambda, so we must do it the old fashioned way, with off_current
			if (sna.PTX != null) {
				Pointer off_current = Pointer.Goto(ref reader, sna.PTX);
				yield return controller.StartCoroutine(ReadTexturesFix(reader, Pointer.Current(reader)));
				Pointer.Goto(ref reader, off_current);
			}
            /*Pointer.DoAt(ref reader, sna.PTX, () => {
                ReadTexturesFix(reader, Pointer.Current(reader));
            });*/
        }
        #endregion

        #region LVLSNA
        IEnumerator LoadLVLSNA() {
            loadingState = "Loading level memory";
            yield return null;
            Reader reader = files_array[Mem.Lvl].reader;
            Pointer off_current;
            SNA sna = (SNA)files_array[Mem.Lvl];

            // First read GPT
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;
            print("LVL GPT offset: " + Pointer.Current(reader));

            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {

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
                if (Settings.s.game != Settings.Game.PlaymobilLaura) {
                    Pointer.Read(reader); // sound related
                }
                Pointer.Read(reader);
                Pointer.Read(reader);
                reader.ReadUInt32();
            }
            if (Settings.s.engineVersion != Settings.EngineVersion.Montreal) {
                loadingState = "Reading settings for persos in fix";
                yield return null;
                // Fill in fix -> lvl pointers for perso's in fix
                uint num_persoInFixPointers = reader.ReadUInt32();
                Pointer[] persoInFixPointers = new Pointer[num_persoInFixPointers];
                for (int i = 0; i < num_persoInFixPointers; i++) {
                    Pointer off_perso = Pointer.Read(reader);
                    if (off_perso != null) {
                        off_current = Pointer.Goto(ref reader, off_perso);
                        reader.ReadUInt32();
                        Pointer off_stdGame = Pointer.Read(reader);
                        if (off_stdGame != null) {
                            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                                Pointer.Goto(ref reader, off_stdGame);
                                reader.ReadUInt32(); // type 0
                                reader.ReadUInt32(); // type 1
                                reader.ReadUInt32(); // type 2
                                Pointer off_superObject = Pointer.Read(reader);
                                Pointer.Goto(ref reader, off_current);
                                if (off_superObject == null) continue;
                            } else {
                                Pointer.Goto(ref reader, off_current);
                            }
                            // First read everything from the GPT
                            Pointer off_newSuperObject = null, off_nextBrother = null, off_prevBrother = null, off_father = null;
                            byte[] matrixData = null, floatData = null, renderBits = null;
                            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                                off_newSuperObject = Pointer.Read(reader);
                                matrixData = reader.ReadBytes(0x58);
                                renderBits = reader.ReadBytes(4);
                                floatData = reader.ReadBytes(4);
                                off_nextBrother = Pointer.Read(reader);
                                off_prevBrother = Pointer.Read(reader);
                                off_father = Pointer.Read(reader);
                            } else {
                                matrixData = reader.ReadBytes(0x58);
                                off_newSuperObject = Pointer.Read(reader);
                                Pointer.DoAt(ref reader, off_stdGame + 0xC, () => {
                                    ((SNA)off_stdGame.file).AddPointer(off_stdGame.offset + 0xC, off_newSuperObject);
                                });
                            }

                            // Then fill everything in
                            off_current = Pointer.Goto(ref reader, off_newSuperObject);
                            uint newSOtype = reader.ReadUInt32();
                            Pointer off_newSOengineObject = Pointer.Read(reader);
                            if (SuperObject.GetSOType(newSOtype) == SuperObject.Type.Perso) {
                                persoInFixPointers[i] = off_newSOengineObject;
                            } else {
                                persoInFixPointers[i] = null;
                            }
                            Pointer.Goto(ref reader, off_newSOengineObject);
                            Pointer off_p3dData = Pointer.Read(reader);
                            ((SNA)off_p3dData.file).OverwriteData(off_p3dData.FileOffset + 0x18, matrixData);

                            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                                FileWithPointers file = off_newSuperObject.file;
                                file.AddPointer(off_newSuperObject.FileOffset + 0x14, off_nextBrother);
                                file.AddPointer(off_newSuperObject.FileOffset + 0x18, off_prevBrother);
                                file.AddPointer(off_newSuperObject.FileOffset + 0x1C, off_father);
                                ((SNA)file).OverwriteData(off_newSuperObject.FileOffset + 0x30, renderBits);
                                ((SNA)file).OverwriteData(off_newSuperObject.FileOffset + 0x38, floatData);
                            }

                        }
                        Pointer.Goto(ref reader, off_current);
                    }
                }
            }
            loadingState = "Loading globals";
            yield return null;
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                globals.off_actualWorld = Pointer.Read(reader);
                globals.off_dynamicWorld = Pointer.Read(reader);
                globals.off_inactiveDynamicWorld = Pointer.Read(reader);
                globals.off_fatherSector = Pointer.Read(reader);
                globals.off_firstSubMapPosition = Pointer.Read(reader);
            } else {
                globals.off_actualWorld = Pointer.Read(reader);
                globals.off_dynamicWorld = Pointer.Read(reader);
                globals.off_fatherSector = Pointer.Read(reader);
                uint soundEventIndex = reader.ReadUInt32(); // In Montreal version this is a pointer, also sound event related
                if (Settings.s.game == Settings.Game.PlaymobilLaura) {
                    Pointer.Read(reader);
                }
            }

            globals.num_always = reader.ReadUInt32();
            globals.spawnablePersos = LinkedList<Perso>.ReadHeader(reader, Pointer.Current(reader), LinkedList.Type.Double);
            globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
                globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks
            } else {
                reader.ReadUInt32(); // 0x6F. In Montreal version this is a pointer to a pointer table for always
                globals.spawnablePersos.FillPointers(reader, globals.spawnablePersos.off_tail, globals.spawnablePersos.offset);
            }

            if (Settings.s.game == Settings.Game.DD) reader.ReadUInt32();
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                Pointer dword_4A6B1C_always_header = Pointer.Read(reader);
                Pointer dword_4A6B20_always_last = Pointer.Read(reader);

                Pointer v28 = Pointer.Read(reader);
                Pointer v31 = Pointer.Read(reader);
                Pointer v32 = Pointer.Read(reader);
                Pointer v33 = Pointer.Read(reader);

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
                Pointer off_names_header = Pointer.Current(reader);
                Pointer off_names_first = Pointer.Read(reader);
                Pointer off_names_last = Pointer.Read(reader);
                uint num_names = reader.ReadUInt32();

                FillLinkedListPointers(reader, off_names_last, off_names_header);
                ReadObjectNamesTable(reader, off_names_first, num_names, i);
            }

            // Begin of engineStructure
            loadingState = "Loading engine structure";
            yield return null;
            print("Start of EngineStructure: " + Pointer.Current(reader));
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
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
                if (Settings.s.game == Settings.Game.PlaymobilLaura) {
                    reader.ReadChars(0x104);
                    reader.ReadChars(0x104);
                }
                string mapName3 = reader.ReadString(0x104);
                if (Settings.s.game == Settings.Game.TT) {
                    reader.ReadBytes(0x47F7); // don't know what this data is
                } else if (Settings.s.game == Settings.Game.TTSE) {
                    reader.ReadBytes(0x240F);
                } else if (Settings.s.game == Settings.Game.PlaymobilLaura) {
                    reader.ReadBytes(0x240F); // don't know what this data is
                } else { // Hype & Alex
                    reader.ReadBytes(0x2627); // don't know what this data is
                }
            }
            Pointer off_unknown_first = Pointer.Read(reader);
            Pointer off_unknown_last = Pointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();

            families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            families.FillPointers(reader, families.off_tail, families.off_head);

            if (Settings.s.game == Settings.Game.PlaymobilLaura) {
                LinkedList<int>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
            }

            LinkedList<SuperObject> alwaysActiveCharacters = LinkedList<SuperObject>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {

                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();

                for (uint i = 0; i < 2; i++) {
                    Pointer off_matrix = Pointer.Current(reader);
                    Matrix mat = Matrix.Read(reader, off_matrix);
                }

                reader.ReadUInt32();
                reader.ReadUInt16();

                ReadLevelNames(reader, Pointer.Current(reader), 80);
                uint num_mapNames = reader.ReadUInt32();
                Array.Resize(ref levels, (int)num_mapNames);
                reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadUInt32();

                if (Settings.s.game == Settings.Game.DD || Settings.s.game == Settings.Game.R2Demo) {
                    reader.ReadUInt32();
                }

                // End of engineStructure
                Pointer off_light = Pointer.Read(reader); // the offset of a light. It's just an ordinary light.
                Pointer off_mainChar = Pointer.Read(reader); // superobject
                Pointer off_characterLaunchingSoundEvents = Pointer.Read(reader);
                if (Settings.s.game == Settings.Game.DD) {
                    globals.off_backgroundGameMaterial = Pointer.Read(reader);
                }
                Pointer off_shadowPolygonVisualMaterial = Pointer.Read(reader);
                Pointer off_shadowPolygonGameMaterialInit = Pointer.Read(reader);
                Pointer off_shadowPolygonGameMaterial = Pointer.Read(reader);
                Pointer off_textureOfTextureShadow = Pointer.Read(reader);
                Pointer off_col_taggedFacesTable = Pointer.Read(reader);

                for (int i = 0; i < 10; i++) {
                    Pointer off_elementForShadow = Pointer.Read(reader);
                    Pointer off_geometricShadowObject = Pointer.Read(reader);
                }
                Pointer.Read(reader);
                if (Settings.s.game == Settings.Game.R2Demo) {
                    Pointer.Read(reader);
                }
                loadingState = "Loading level animation bank";
                yield return null;
                AnimationBank.Read(reader, Pointer.Current(reader), 0, 1, files_array[Mem.LvlKeyFrames], append: true);
                animationBanks[1] = animationBanks[0];
            }
            if (FileSystem.mode != FileSystem.Mode.Web) {
                string levelsFolder = gameDataBinFolder + gameDsb.levelsDataPath + "/";
                ((SNA)files_array[0]).CreateMemoryDump(levelsFolder + "fix.dmp", true);
                ((SNA)files_array[1]).CreateMemoryDump(levelsFolder + lvlName + "/" + lvlName + ".dmp", true);
            }

            // Read PTX
            loadingState = "Loading level textures";
            yield return null;

			// Can't yield inside a lambda, so we must do it the old fashioned way, with off_current
			if (sna.PTX != null) {
				off_current = Pointer.Goto(ref reader, sna.PTX);
				yield return controller.StartCoroutine(ReadTexturesLvl(reader, Pointer.Current(reader)));
				Pointer.Goto(ref reader, off_current);
			}
			/*Pointer.DoAt(ref reader, sna.PTX, () => {
                ReadTexturesLvl(reader, Pointer.Current(reader));
            });*/

            // Read background game material (DD only)
            globals.backgroundGameMaterial = GameMaterial.FromOffsetOrRead(globals.off_backgroundGameMaterial, reader);

            // Parse actual world & always structure
            loadingState = "Loading families";
            yield return null;
            ReadFamilies(reader);

            loadingState = "Creating animation bank";
            yield return null;
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                animationBanks = new AnimationBank[2];
                animationBanks[0] = new AnimationBank(null) {
                    animations = new Animation.Component.AnimA3DGeneral[0]
                };
                animationBanks[1] = animationBanks[0];
            } else if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
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
            yield return null;
            ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            yield return null;
            ReadAlways(reader);
            loadingState = "Filling in cross-references";
            yield return null;
            ReadCrossReferences(reader);

			// TODO: Make more generic
			if (Settings.s.game == Settings.Game.R2) {
				loadingState = "Filling in comport names";
				yield return null;
				string path = gameDataBinFolder + "R2DC_Comports.json";
				Stream stream = FileSystem.GetFileReadStream(path);
				if (stream != null) {
					ReadAndFillComportNames(stream);
				}
			}
		}
        #endregion
        
    }
}
