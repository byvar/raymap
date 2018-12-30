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
using OpenSpace.Cinematics;

namespace OpenSpace.Loader {
    public class R3Loader : MapLoader {
        public override IEnumerator Load() {
            try {
                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
				gameDataBinFolder += "/";
				yield return controller.StartCoroutine(FileSystem.CheckDirectory(gameDataBinFolder));
				if (!FileSystem.DirectoryExists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");

                loadingState = "Initializing files";
				yield return controller.StartCoroutine(CreateCNT());

				if (lvlName.EndsWith(".exe")) {
                    if (!Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
                    Settings.s.loadFromMemory = true;
                    MemoryFile mem = new MemoryFile(lvlName);
                    files_array[0] = mem;
                    yield return null;
                    LoadMemory();
                } else {
                    // Fix
                    menuTPLPath = gameDataBinFolder + "menu.tpl";
                    lvlNames[0] = "fix";
                    lvlPaths[0] = gameDataBinFolder + "fix.lvl";
                    ptrPaths[0] = gameDataBinFolder + "fix.ptr";
                    tplPaths[0] = gameDataBinFolder + ((Settings.s.mode == Settings.Mode.RaymanArenaGC) ? "../common.tpl" : "fix.tpl");
                    yield return controller.StartCoroutine(PrepareFile(menuTPLPath));
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[0]));
                    if (FileSystem.FileExists(lvlPaths[0])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[0]));
                        yield return controller.StartCoroutine(PrepareFile(tplPaths[0]));
                    }

                    // Level
                    lvlNames[1] = lvlName;
                    lvlPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + ".lvl";
                    ptrPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + ".ptr";
                    tplPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ((Settings.s.mode == Settings.Mode.RaymanArenaGC) ? ".tpl" : "_Lvl.tpl");
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[1]));
                    if (FileSystem.FileExists(lvlPaths[1])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[1]));
                        yield return controller.StartCoroutine(PrepareFile(tplPaths[1]));
                    }

                    // Transit
                    lvlNames[2] = "transit";
                    lvlPaths[2] = gameDataBinFolder + lvlName + "/transit.lvl";
                    ptrPaths[2] = gameDataBinFolder + lvlName + "/transit.ptr";
                    tplPaths[2] = gameDataBinFolder + lvlName + "/" + lvlName + "_Trans.tpl";
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[2]));
                    if (FileSystem.FileExists(lvlPaths[2])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[2]));
                        yield return controller.StartCoroutine(PrepareFile(tplPaths[2]));
                    }
                    hasTransit = FileSystem.FileExists(lvlPaths[2]) && (FileSystem.GetFileLength(lvlPaths[2]) > 4);

                    // Vertex buffer
                    lvlNames[4] = lvlName + "_vb";
                    lvlPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "_vb.lvl";
                    ptrPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "_vb.ptr";
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[4]));
                    if (FileSystem.FileExists(lvlPaths[4])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[4]));
                    }

                    // Fix Keyframes
                    lvlNames[5] = "fixkf";
                    lvlPaths[5] = gameDataBinFolder + "fixkf.lvl";
                    ptrPaths[5] = gameDataBinFolder + "fixkf.ptr";
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[5]));
                    if (FileSystem.FileExists(lvlPaths[5])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[5]));
                    }

                    // Level Keyframes
                    lvlNames[6] = lvlName + "kf";
                    lvlPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "kf.lvl";
                    ptrPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName.ToLower() + "kf.ptr";
                    yield return controller.StartCoroutine(PrepareFile(lvlPaths[6]));
                    if (FileSystem.FileExists(lvlPaths[6])) {
                        yield return controller.StartCoroutine(PrepareFile(ptrPaths[6]));
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

                    yield return controller.StartCoroutine(LoadFIX());
                    yield return controller.StartCoroutine(LoadLVL());
                }
            } finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
						if (!(files_array[i] is MemoryFile)) files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            yield return null;
            InitModdables();
        }

        #region FIX
        IEnumerator LoadFIX() {
            loadingState = "Loading fixed memory";
            yield return null;
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            // Read fix header
            //reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (Settings.s.mode == Settings.Mode.Rayman3PC) {
                string timeStamp = reader.ReadString(0x18);
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            } else if (Settings.s.mode == Settings.Mode.RaymanArenaPC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            Pointer off_identityMatrix = Pointer.Read(reader);
            loadingState = "Loading text";
            yield return null;
            fontStruct = FontStructure.Read(reader, Pointer.Current(reader));
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
            if (Settings.s.platform == Settings.Platform.PC) {
                reader.ReadChars(0x1E);
                reader.ReadChars(0x1E); // two zero entries
            }
            string firstMapName = new string(reader.ReadChars(0x1E));
            if (reader.BaseStream.Position % 4 == 0) {
                reader.ReadUInt32();
            } else {
                reader.ReadUInt16();
            }
            uint num_languages = reader.ReadUInt32();
            Pointer off_languages = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_languages, () => {
                ReadLanguages(reader, off_languages, num_languages);
            });
            loadingState = "Loading fixed textures";
            yield return controller.StartCoroutine(ReadTexturesFix(reader, Pointer.Current(reader)));
            // Defaults for Rayman 3 PC. Sizes are hardcoded in the exes and might differ for versions too :/
            int sz_entryActions = 0x100;
            int sz_randomStructure = 0xDC;
            int sz_fontDefine = 0x12B2;
            int sz_videoStructure = 0x18;
            int sz_musicMarkerSlot = 0x28;
            int sz_binDataForMenu = 0x020C;

            if (Settings.s.mode == Settings.Mode.Rayman3GC) {
                sz_entryActions = 0xE8;
                sz_binDataForMenu = 0x01F0;
                sz_fontDefine = 0x12E4;
            } else if (Settings.s.mode == Settings.Mode.RaymanArenaGC) {
                sz_entryActions = 0xC4;
                sz_fontDefine = 0x12E4;
            } else if (Settings.s.mode == Settings.Mode.RaymanArenaPC) {
                sz_entryActions = 0xDC;
            }
            loadingState = "Loading input structure";
            yield return null;
            inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
			foreach (EntryAction ea in inputStruct.entryActions) {
				print(ea.ToString());
			}
			if (Settings.s.platform == Settings.Platform.PC) {
                Pointer off_IPT_keyAndPadDefine = Pointer.Read(reader);
                ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);
            }
            reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
            uint num_persoInFix = reader.ReadUInt32();
            persoInFix = new Pointer[num_persoInFix];
            for (int i = 0; i < num_persoInFix; i++) {
                persoInFix[i] = Pointer.Read(reader);
            }
            reader.ReadBytes(sz_randomStructure);
            uint soundEventTableIndexInFix = reader.ReadUInt32();
            Pointer off_soundEventTable = Pointer.Read(reader);
            byte num_fontBitmap = reader.ReadByte();
            byte num_font = reader.ReadByte();
            for (int i = 0; i < num_font; i++) {
                reader.ReadBytes(sz_fontDefine); // Font definition
            }
            reader.Align(4); // Align position
            for (int i = 0; i < num_fontBitmap; i++) {
                Pointer off_fontTexture = Pointer.Read(reader);
            }
            reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
            if (Settings.s.game == Settings.Game.R3) {
                uint num_musicMarkerSlots = reader.ReadUInt32();
                for (int i = 0; i < num_musicMarkerSlots; i++) {
                    reader.ReadBytes(sz_musicMarkerSlot);
                }
                reader.ReadBytes(sz_binDataForMenu);
                if (Settings.s.platform == Settings.Platform.PC) {
                    Pointer off_bgMaterialForMenu2D = Pointer.Read(reader);
                    Pointer off_fixMaterialForMenu2D = Pointer.Read(reader);
                    Pointer off_fixMaterialForSelectedFilms = Pointer.Read(reader);
                    Pointer off_fixMaterialForArcadeAndFilms = Pointer.Read(reader);
                    for (int i = 0; i < 35; i++) { // 35 is again hardcoded
                        Pointer off_menuPage = Pointer.Read(reader);
                    }
                }
            }
            loadingState = "Loading fixed animation bank";
            yield return null;
            Pointer off_animBankFix = Pointer.Read(reader); // Note: only one 0x104 bank in fix.
            print("Fix animation bank address: " + off_animBankFix);
            animationBanks = new AnimationBank[5]; // 1 in fix, 4 in lvl
            Pointer.DoAt(ref reader, off_animBankFix, () => {
                animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
            });
        }
        #endregion

        #region LVL
        IEnumerator LoadLVL() {
            loadingState = "Loading level memory";
            yield return null;
            files_array[Mem.Lvl].GotoHeader();
            Reader reader = files_array[Mem.Lvl].reader;
            long totalSize = reader.BaseStream.Length;
            //reader.ReadUInt32();
            if (Settings.s.mode == Settings.Mode.Rayman3PC) {
                reader.ReadUInt32(); // fix checksum?
            }
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (Settings.s.mode == Settings.Mode.Rayman3PC) {
                string timeStamp = new string(reader.ReadChars(0x18));
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            } else if (Settings.s.mode == Settings.Mode.RaymanArenaPC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            reader.ReadBytes(0x104); // vignette
            reader.ReadUInt32();
            loadingState = "Loading level textures";
            yield return controller.StartCoroutine(ReadTexturesLvl(reader, Pointer.Current(reader)));
            if (Settings.s.platform == Settings.Platform.PC && !hasTransit) {
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
            loadingState = "Loading globals";
            yield return null;
            globals.off_transitDynamicWorld = null;
            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            if (Settings.s.mode == Settings.Mode.Rayman3PC) reader.ReadUInt32();
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
            Pointer off_collisionGeoObj = Pointer.Read(reader);
            Pointer off_staticCollisionGeoObj = Pointer.Read(reader);
            if (!hasTransit) {
                reader.ReadUInt32(); // viewport related <--- cameras in here
            }

            Pointer off_unknown_first = Pointer.Read(reader);
            Pointer off_unknown_last = Pointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();

            families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

            Pointer off_alwaysActiveCharacters_first = Pointer.Read(reader);
            Pointer off_alwaysActiveCharacters_last = Pointer.Read(reader);
            uint num_alwaysActiveChars = reader.ReadUInt32();

            if (!hasTransit) {
                Pointer off_mainCharacters_first = Pointer.Read(reader);
                Pointer off_mainCharacters_last = Pointer.Read(reader);
                uint num_mainCharacters_entries = reader.ReadUInt32();
            }
            reader.ReadUInt32(); // only used if there was no transit in the previous lvl. Always 00165214 in R3GC?
            reader.ReadUInt32(); // related to "SOL". What is this? Good question.
            reader.ReadUInt32(); // same
            reader.ReadUInt32(); // same
            Pointer off_cineManager = Pointer.Read(reader);
            byte unk = reader.ReadByte();
            byte IPO_numRLItables = reader.ReadByte();
            reader.ReadUInt16();
            Pointer off_COL_taggedFacesTable = Pointer.Read(reader);
            uint num_COL_maxTaggedFaces = reader.ReadUInt32();
            off_collisionGeoObj = Pointer.Read(reader);
            off_staticCollisionGeoObj = Pointer.Read(reader);

            // The ptrsTable seems to be related to sound events. Perhaps cuuids.
            reader.ReadUInt32();
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
            if (!hasTransit && Settings.s.game == Settings.Game.R3) {
                uint num_geometric = reader.ReadUInt32();
                Pointer off_array_geometric = Pointer.Read(reader);
                Pointer off_array_geometric_RLI = Pointer.Read(reader);
                Pointer off_array_transition_flags = Pointer.Read(reader);
            } else if (Settings.s.game == Settings.Game.RA) {
                uint num_unk = reader.ReadUInt32();
                Pointer unk_first = Pointer.Read(reader);
                Pointer unk_last = Pointer.Read(reader);
            }
            uint num_visual_materials = reader.ReadUInt32();
            Pointer off_array_visual_materials = Pointer.Read(reader);
            if (Settings.s.mode != Settings.Mode.RaymanArenaGC) {
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
            yield return null;
            Pointer.DoAt(ref reader, off_array_visual_materials, () => {
                for (uint i = 0; i < num_visual_materials; i++) {
                    Pointer off_material = Pointer.Read(reader);
                    Pointer.DoAt(ref reader, off_material, () => {
                        visualMaterials.Add(VisualMaterial.Read(reader, off_material));
                    });
                }
            });

            if (hasTransit) {
                loadingState = "Loading transit memory";
                yield return null;
                Pointer off_transit = new Pointer(16, files_array[Mem.Transit]); // It's located at offset 20 in transit
                Pointer.DoAt(ref reader, off_transit, () => {
                    if (Settings.s.platform == Settings.Platform.PC) {
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
            yield return null;
            ReadFamilies(reader);
            loadingState = "Loading superobject hierarchy";
            yield return null;
            ReadSuperObjects(reader);
            loadingState = "Loading always structure";
            yield return null;
            ReadAlways(reader);


			Pointer.DoAt(ref reader, off_cineManager, () => {
				cinematicsManager = CinematicsManager.Read(reader, off_cineManager);
			});

			// off_current should be after the dynamic SO list positions.

			// Parse transformation matrices and other settings(state? :o) for fix characters
			loadingState = "Loading settings for persos in fix";
            yield return null;
            uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
            if (Settings.s.game == Settings.Game.R3) num_perso_with_settings_in_fix = reader.ReadUInt32();
            for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
                Pointer off_perso_so_with_settings_in_fix = null, off_matrix = null;
                SuperObject so = null;
                Matrix mat = null;
                if (Settings.s.game == Settings.Game.R3) {
                    off_perso_so_with_settings_in_fix = Pointer.Read(reader);
                    off_matrix = Pointer.Current(reader);
                    mat = Matrix.Read(reader, off_matrix);
                    reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
                    reader.ReadUInt32();
                    so = SuperObject.FromOffset(off_perso_so_with_settings_in_fix);
                } else if (Settings.s.game == Settings.Game.RA) {
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
            if (Settings.s.platform == Settings.Platform.GC) {
                reader.ReadBytes(0x800); // floats
            }
            loadingState = "Loading level animation banks";
            yield return null;
            Pointer off_animBankLvl = Pointer.Read(reader); // Note: 4 0x104 banks in lvl.
            print("Lvl animation bank address: " + off_animBankLvl);
            Pointer.DoAt(ref reader, off_animBankLvl, () => {
                AnimationBank[] banks = AnimationBank.Read(reader, off_animBankLvl, 1, 4, files_array[Mem.LvlKeyFrames]);
                for (int i = 0; i < 4; i++) {
                    animationBanks[1 + i] = banks[i];
                }
            });
            // Load additional animation banks
            for (int i = 0; i < families.Count; i++) {
                if (families[i] != null && families[i].animBank > 4 && objectTypes[0][families[i].family_index].id != 0xFF) {
                    int animBank = families[i].animBank;
                    loadingState = "Loading additional animation bank " + animBank;
                    yield return null;
                    string animName = "Anim/ani" + objectTypes[0][families[i].family_index].id.ToString();
                    string kfName = "Anim/key" + objectTypes[0][families[i].family_index].id.ToString() + "kf";
                    int fileID = animBank + 102;
                    int kfFileID = animBank + 2; // Anim bank will start at 5, so this will start at 7

                    // Prepare files for WebGL
                    yield return controller.StartCoroutine(PrepareFile(gameDataBinFolder + animName + ".lvl"));
                    if (FileSystem.FileExists(gameDataBinFolder + animName + ".lvl")) {
                        yield return controller.StartCoroutine(PrepareFile(gameDataBinFolder + animName + ".ptr"));
                    }
                    yield return controller.StartCoroutine(PrepareFile(gameDataBinFolder + kfName + ".lvl"));
                    if (FileSystem.FileExists(gameDataBinFolder + kfName + ".lvl")) {
                        yield return controller.StartCoroutine(PrepareFile(gameDataBinFolder + kfName + ".ptr"));
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
            yield return null;
            ReadCrossReferences(reader);
        }
        #endregion
    }
}