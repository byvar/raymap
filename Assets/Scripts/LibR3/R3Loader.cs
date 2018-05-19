using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LibR3 {

    public class R3Loader {
        public string gameDataBinFolder;
        public string lvlName;

        public Material baseMaterial;
        public Material baseLightMaterial;
        public Material baseTransparentMaterial;
        public Material baseBlendMaterial;
        public Material baseBlendTransparentMaterial;
        public Material negativeLightProjectorMaterial;
        public bool allowDeadPointers = false;
        public bool forceDisplayBackfaces = false;
        public enum Mode { Rayman3PC, Rayman3GC, RaymanArenaPC, RaymanArenaGC };
        public Mode mode = Mode.Rayman3PC;

        public string[][] names;

        public R3Texture[] textures;
        public R3Material[] materials;
        public R3Texture overlightTexture;
        public R3Texture lightmapTexture;
        public R3Pointer[] persoInFix;
        public R3AnimationBank[] animationBanks;
        public R3Family[] families;

        uint off_textures_start_fix = 0;
        bool hasTransit;
        bool isLittleEndian = false;
        public List<R3SuperObject> superObjects = new List<R3SuperObject>();
        public List<R3Light> lights = new List<R3Light>();
        public List<R3Sector> sectors = new List<R3Sector>();
        //List<R3GeometricObject> parsedGO = new List<R3GeometricObject>();

        public Dictionary<uint, R3Pointer>[] ptrs_array = new Dictionary<uint, R3Pointer>[3];
        public FileStream[] fs_array = new FileStream[3];
        public EndianBinaryReader[] reader_array = new EndianBinaryReader[3];
        public EndianBinaryWriter[] writer_array = new EndianBinaryWriter[3];


        string[] lvlPaths = new string[3];
        string[] ptrPaths = new string[3];
        string[] tplPaths = new string[3];
        string[] cntPaths = null;
        CNT cnt = null;
        string menuTPLPath;

        public static class Mem {
            public const int Fix = 0;
            public const int Lvl = 1;
            public const int Transit = 2;
            // 3 is also transit
            public const int VertexBuffer = 4;
            public const int FixKeyFrames = 5;
            public const int LvlKeyFrames = 6;
        }
        public int[] loadOrder = new int[] { Mem.Fix, Mem.Transit, Mem.Lvl };

        
        static R3Loader loader = null;
        public static R3Loader Loader {
            get {
                if (loader == null) {
                    loader = new R3Loader();
                }
                return loader;
            }
        }

        public R3Loader() {
        }
        
        public void Load() {
            try {
                if (mode == Mode.RaymanArenaPC || mode == Mode.Rayman3PC) {
                    isLittleEndian = true;
                }

                if (gameDataBinFolder == null || !Directory.Exists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                menuTPLPath = Path.Combine(gameDataBinFolder, "menu.tpl");
                lvlPaths[0] = Path.Combine(gameDataBinFolder, "fix.lvl");
                ptrPaths[0] = Path.Combine(gameDataBinFolder, "fix.ptr");
                tplPaths[0] = Path.Combine(gameDataBinFolder, ((mode == Mode.RaymanArenaGC) ? "../common.tpl" : "fix.tpl"));

                lvlPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".lvl");
                ptrPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".ptr");
                tplPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ((mode == Mode.RaymanArenaGC) ? ".tpl" : "_Lvl.tpl"));

                lvlPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/transit.lvl");
                ptrPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/transit.ptr");
                tplPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "_Trans.tpl");
                hasTransit = File.Exists(lvlPaths[2]) && (new FileInfo(lvlPaths[2]).Length > 4);

                if (mode == Mode.Rayman3PC) {
                    cntPaths = new string[3];
                    cntPaths[0] = Path.Combine(gameDataBinFolder, "vignette.cnt");
                    cntPaths[1] = Path.Combine(gameDataBinFolder, "tex32_1.cnt");
                    cntPaths[2] = Path.Combine(gameDataBinFolder, "tex32_2.cnt");
                    cnt = new CNT(cntPaths);
                } else if (mode == Mode.RaymanArenaPC) {
                    cntPaths = new string[2];
                    cntPaths[0] = Path.Combine(gameDataBinFolder, "vignette.cnt");
                    cntPaths[1] = Path.Combine(gameDataBinFolder, "tex32.cnt");
                    cnt = new CNT(cntPaths);
                }

                for (int i = 0; i < 3; i++) {
                    if (File.Exists(lvlPaths[i]) && File.Exists(ptrPaths[i])) {
                        fs_array[i] = new FileStream(lvlPaths[i], FileMode.Open);
                        reader_array[i] = new EndianBinaryReader(fs_array[i], isLittleEndian);
                    }
                }
                LoadPTR();
                LoadFIX();
                LoadLVL();
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            } finally {
                for (int i = 0; i < 3; i++) {
                    if (fs_array[i] != null) {
                        reader_array[i].Close();
                        fs_array[i].Close();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            InitModdables();
        }

        public void InitModdables() {
            foreach (R3SuperObject so in superObjects) {
                GameObject gao = so.Gao;
                if (gao != null) {
                    Moddable mod = gao.AddComponent<Moddable>();
                    mod.mat = so.matrix;
                }
            }
        }

        public void SaveModdables() {
            EndianBinaryWriter writer = writer_array[1];
            if (writer == null) return;
            foreach (R3SuperObject so in superObjects) {
                GameObject gao = so.Gao;
                if (gao != null) {
                    Moddable mod = gao.GetComponent<Moddable>();
                    if (mod != null) {
                        mod.SaveChanges(writer);
                    }
                }
            }
        }

        public void Save() {
            try {
                for (int i = 0; i < 3; i++) {
                    if (File.Exists(lvlPaths[i]) && File.Exists(ptrPaths[i])) {
                        fs_array[i] = new FileStream(lvlPaths[i], FileMode.Open);
                        writer_array[i] = new EndianBinaryWriter(fs_array[i], isLittleEndian);
                    }
                }
                // Save changes
                SaveModdables();
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            } finally {
                for (int i = 0; i < 3; i++) {
                    if (fs_array[i] != null) {
                        writer_array[i].Close();
                        fs_array[i].Close();
                    }
                }
            }
        }

        void LoadPTR() {
            for (int i = 0; i < 3; i++) {
                ptrs_array[i] = new Dictionary<uint, R3Pointer>();
            }
            for (int k = 0; k < 3; k++) {
                int i = loadOrder[k];
                if (!File.Exists(ptrPaths[i])) continue;
                FileStream fs = new FileStream(ptrPaths[i], FileMode.Open);
                long totalSize = fs.Length;
                using (EndianBinaryReader reader = new EndianBinaryReader(fs, isLittleEndian)) {
                    uint num_ptrs = reader.ReadUInt32();
                    for (uint j = 0; j < num_ptrs; j++) {
                        int file = reader.ReadInt32();
                        uint ptr_ptr = reader.ReadUInt32();
                        reader_array[i].BaseStream.Seek(ptr_ptr + 4, SeekOrigin.Begin);
                        uint ptr = reader_array[i].ReadUInt32();
                        ptrs_array[i].Add(ptr_ptr, new R3Pointer(ptr, file));
                    }
                    long num_fillInPtrs = (totalSize - fs.Position) / 16;
                    for (uint j = 0; j < num_fillInPtrs; j++) {
                        uint ptr_ptr = reader.ReadUInt32(); // the address the pointer should be located at
                        int src_file = reader.ReadInt32(); // the file the pointer should be located in
                        uint ptr = reader.ReadUInt32();
                        int target_file = reader.ReadInt32();
                        ptrs_array[src_file][ptr_ptr] = new R3Pointer(ptr, target_file); // can overwrite if necessary
                    }
                }
                reader_array[i].BaseStream.Seek(0, SeekOrigin.Begin);
                fs.Close();
            }
        }

        void LoadFIX() {
            EndianBinaryReader reader = reader_array[Mem.Fix];
            // Read fix header
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (mode == Mode.Rayman3PC) {
                string timeStamp = new string(reader.ReadChars(0x18));
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            } else if (mode == Mode.RaymanArenaPC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            R3Pointer off_unk1 = R3Pointer.Read(reader);
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt16(); // 0x0A in R3, 0x09 in RA PC, so a number of something
            reader.ReadUInt16(); // 0
            R3Pointer off_unk2 = R3Pointer.Read(reader);
            R3Pointer off_unk3 = R3Pointer.Read(reader);
            uint num_lvlNames = reader.ReadUInt32();
            uint num_fixEntries1 = reader.ReadUInt32();
            // Read tables under header
            for (uint i = 0; i < num_fixEntries1; i++) {
                string savName = new string(reader.ReadChars(0xC));
            }
            for (uint i = 0; i < num_fixEntries1; i++) {
                string savMapName = new string(reader.ReadChars(0xC));
            }
            for (uint i = 0; i < num_lvlNames; i++) {
                string mapName = new string(reader.ReadChars(0x1E));
            }
            if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
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
            R3Pointer off_languages = R3Pointer.Read(reader);
            R3Pointer off_current = R3Pointer.Goto(ref reader, off_languages);
            for (uint i = 0; i < num_languages; i++) {
                string language_us = new string(reader.ReadChars(0x14));
                string language_loc = new string(reader.ReadChars(0x14));
            }
            R3Pointer.Goto(ref reader, off_current);
            uint num_textures = reader.ReadUInt32();
            print("Texture count fix: " + num_textures);
            textures = new R3Texture[num_textures];
            if (num_textures > 0) {
                for (uint i = 0; i < num_textures; i++) {
                    R3Pointer off_texture = R3Pointer.Read(reader);
                    off_current = R3Pointer.Goto(ref reader, off_texture);
                    textures[i] = R3Texture.Read(reader, off_texture);
                    R3Pointer.Goto(ref reader, off_current);
                }
                if (mode == Mode.Rayman3GC || mode == Mode.RaymanArenaGC) {
                    uint num_textures_menu = reader.ReadUInt32();
                    TPL fixTPL = new TPL(tplPaths[Mem.Fix]);
                    TPL menuTPL = new TPL(menuTPLPath);
                    for (uint i = 0; i < num_textures_menu; i++) {
                        R3Pointer off_texture = R3Pointer.Read(reader);
                        R3Texture tex = textures.Where(t => t.offset == off_texture).First();
                        tex.texture = menuTPL.textures[i];
                    }
                    for (int i = 0, j = 0; i < fixTPL.Count; i++, j++) {
                        while (textures[j].texture != null) j++;
                        textures[j].texture = fixTPL.textures[i];
                    }
                } else if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                    for (int i = 0; i < num_textures; i++) {
                        GF3 gf = cnt.GetGFByTGAName(textures[i].name);
                        if (gf != null) {
                            textures[i].texture = gf.GetTexture();
                        }
                    }
                }
                for (uint i = 0; i < num_textures; i++) {
                    reader.ReadUInt32(); // 0 or 8.
                }
            }
            // Defaults for Rayman 3 PC. Sizes are hardcoded in the exes and might differ for versions too :/
            int sz_inputStruct = 0x1adc;
            int sz_entryActions = 0x100;
            int sz_randomStructure = 0xDC;
            int sz_fontStructure = 0x12B2;
            int sz_videoStructure = 0x18;
            int sz_musicMarkerSlot = 0x28;
            int sz_binDataForMenu = 0x020C;

            if (mode == Mode.Rayman3GC) {
                sz_inputStruct = 0x1714;
                sz_entryActions = 0xD0;
                sz_fontStructure = 0x12E4;
                sz_binDataForMenu = 0x01F0;
            } else if (mode == Mode.RaymanArenaGC) {
                sz_inputStruct = 0x1714;
                sz_entryActions = 0x94;
                sz_fontStructure = 0x12E4;
            } else if (mode == Mode.RaymanArenaPC) {
                sz_entryActions = 0xDC;
            }
            reader.ReadBytes(sz_inputStruct); // Input struct
            R3Pointer keypadDefine = R3Pointer.Read(reader);
            reader.ReadBytes(sz_entryActions); // 3DOS_EntryActions
            uint num_persoInFix = reader.ReadUInt32();
            persoInFix = new R3Pointer[num_persoInFix];
            for (int i = 0; i < num_persoInFix; i++) {
                persoInFix[i] = R3Pointer.Read(reader);
            }
            reader.ReadBytes(sz_randomStructure);
            uint soundEventTableIndexInFix = reader.ReadUInt32();
            R3Pointer off_soundEventTable = R3Pointer.Read(reader);
            byte num_fontBitmap = reader.ReadByte();
            byte num_font = reader.ReadByte();
            for (int i = 0; i < num_font; i++) {
                reader.ReadBytes(sz_fontStructure); // Font definition
            }
            reader.Align(4); // Align position
            for (int i = 0; i < num_fontBitmap; i++) {
                R3Pointer off_fontTexture = R3Pointer.Read(reader);
            }
            reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                uint num_musicMarkerSlots = reader.ReadUInt32();
                for (int i = 0; i < num_musicMarkerSlots; i++) {
                    reader.ReadBytes(sz_musicMarkerSlot);
                }
                reader.ReadBytes(sz_binDataForMenu);
                if (mode == Mode.Rayman3PC) {
                    R3Pointer off_bgMaterialForMenu2D = R3Pointer.Read(reader);
                    R3Pointer off_fixMaterialForMenu2D = R3Pointer.Read(reader);
                    R3Pointer off_fixMaterialForSelectedFilms = R3Pointer.Read(reader);
                    R3Pointer off_fixMaterialForArcadeAndFilms = R3Pointer.Read(reader);
                    for (int i = 0; i < 35; i++) { // 35 is again hardcoded
                        R3Pointer off_menuPage = R3Pointer.Read(reader);
                    }
                }
            }
            R3Pointer off_animBankFix = R3Pointer.Read(reader); // Note: only one 0x104 bank in fix.
            print("Fix animation bank address: " + off_animBankFix);
            animationBanks = new R3AnimationBank[5]; // 1 in fix, 4 in lvl
            if (off_animBankFix != null) {
                off_current = R3Pointer.Goto(ref reader, off_animBankFix);
                animationBanks[0] = R3AnimationBank.Read(reader, off_animBankFix, 0, 1)[0];
                R3Pointer.Goto(ref reader, off_current);
            }
        }

        void LoadLVL() {
            long totalSize = fs_array[Mem.Lvl].Length;
            R3Pointer off_current = null;
            fs_array[Mem.Lvl].Seek(0, SeekOrigin.Begin); // precaution, but it won't read anything before now anyway
            EndianBinaryReader reader = reader_array[Mem.Lvl];
            reader.ReadUInt32();
            if (mode == Mode.Rayman3PC) {
                reader.ReadUInt32(); // fix checksum?
            }
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            if (mode == Mode.Rayman3PC) {
                string timeStamp = new string(reader.ReadChars(0x18));
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadUInt32();
            } else if (mode == Mode.RaymanArenaPC) {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }
            reader.ReadBytes(0x104); // vignette
            reader.ReadUInt32();
            if (mode == Mode.Rayman3GC || mode == Mode.RaymanArenaGC) {
                uint num_textures_total = reader.ReadUInt32();
                uint num_textures_fix = (uint)textures.Length;
                uint num_textures_lvl = num_textures_total - num_textures_fix;
                print("Textures to be read in this file: " + num_textures_lvl);
                Array.Resize(ref textures, (int)num_textures_total);

                // Let's load the textures now
                TPL lvlTPL = new TPL(tplPaths[Mem.Lvl]);
                TPL transitTPL = hasTransit ? new TPL(tplPaths[Mem.Transit]) : null;
                print("Lvl TPL Texture count: " + lvlTPL.Count);
                if (hasTransit) print("Transit TPL Texture count: " + transitTPL.Count);
                int transitTexturesSeen = 0;
                for (int i = 0; i < num_textures_lvl; i++) {
                    R3Pointer off_texture = R3Pointer.Read(reader);
                    off_current = R3Pointer.Goto(ref reader, off_texture);
                    textures[num_textures_fix + i] = R3Texture.Read(reader, off_texture);
                    R3Pointer.Goto(ref reader, off_current);
                }
                for (int i = 0; i < num_textures_lvl; i++) {
                    uint file_texture = reader.ReadUInt32();
                    if (hasTransit && file_texture == 6) {
                        textures[num_textures_fix + i].texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        textures[num_textures_fix + i].texture = lvlTPL.textures[i - transitTexturesSeen];
                    }
                }
            } else if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                uint num_textures_fix = reader.ReadUInt32();
                Array.Resize(ref textures, 1024); // Yeah, it's actually a 1024 array in the PC version.
                uint num_textures_lvl = 1024 - num_textures_fix;
                for (uint i = 0; i < num_textures_lvl; i++) {
                    R3Pointer off_texture = R3Pointer.Read(reader);
                    if (off_texture != null) {
                        off_current = R3Pointer.Goto(ref reader, off_texture);
                        textures[num_textures_fix + i] = R3Texture.Read(reader, off_texture);
                        R3Pointer.Goto(ref reader, off_current);
                    } else {
                        num_textures_lvl = i;
                        break;
                    }
                }
                for (uint i = num_textures_lvl + 1; i < 1024 - num_textures_fix; i++) {
                    reader.ReadUInt32(); // 0
                }
                uint num_textures_total = num_textures_fix + num_textures_lvl;
                Array.Resize(ref textures, (int)num_textures_total);
                int transitTexturesSeen = 0;
                for (uint i = 0; i < 1024 - num_textures_fix; i++) {
                    uint file_texture = reader.ReadUInt32();
                    if (file_texture == 0xC0DE0005) {
                        // texture is undefined
                    } else if (hasTransit && file_texture == 6) {
                        GF3 gf = cnt.GetGFByTGAName(textures[num_textures_fix + i].name);
                        if (gf != null) {
                            textures[num_textures_fix + i].texture = gf.GetTexture();
                        }
                        transitTexturesSeen++;
                        //textures[num_textures_fix + i].texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        GF3 gf = cnt.GetGFByTGAName(textures[num_textures_fix + i].name);
                        if (gf != null) {
                            textures[num_textures_fix + i].texture = gf.GetTexture();
                        }
                        //textures[num_textures_fix + i].texture = lvlTPL.textures[i - transitTexturesSeen];
                    }
                }
                if (!hasTransit) {
                    R3Pointer off_lightMapTexture = R3Pointer.Read(reader); // g_p_stLMTexture
                    if (off_lightMapTexture != null) {
                        off_current = R3Pointer.Goto(ref reader, off_lightMapTexture);
                        lightmapTexture = R3Texture.Read(reader, off_lightMapTexture);
                        R3Pointer.Goto(ref reader, off_current);
                    }
                    if (mode == Mode.Rayman3PC) {
                        R3Pointer off_overlightTexture = R3Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
                        if (off_overlightTexture != null) {
                            off_current = R3Pointer.Goto(ref reader, off_overlightTexture);
                            overlightTexture = R3Texture.Read(reader, off_overlightTexture);
                            R3Pointer.Goto(ref reader, off_current);
                        }
                    }
                }
            }

            R3Pointer off_actual_world = R3Pointer.Read(reader);
            R3Pointer off_dynamic_world = R3Pointer.Read(reader);
            if (mode == Mode.Rayman3PC) {
                reader.ReadUInt32();
            }
            R3Pointer off_inactiveDynamic_world = R3Pointer.Read(reader);
            R3Pointer off_fatherSector = R3Pointer.Read(reader); // It is I, Father Sector.
            R3Pointer off_firstSubMapPosition = R3Pointer.Read(reader);

            /* The following 7 values are the "Always" structure. The spawnable perso data is dynamically copied to these superobjects.
            There can be at most (num_always) objects of this type active in a level, and they get reused by other objects when they despawn.
            */
            uint num_always = reader.ReadUInt32();
            R3Pointer off_spawnable_perso_first = R3Pointer.Read(reader);
            R3Pointer off_spawnable_perso_last = R3Pointer.Read(reader);
            uint num_spawnable_perso = reader.ReadUInt32();
            R3Pointer off_always_reusableSO = R3Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            R3Pointer off_always_reusableUnknown1 = R3Pointer.Read(reader); // (num_always) * 0x2c blocks
            R3Pointer off_always_reusableUnknown2 = R3Pointer.Read(reader); // (num_always) * 0x4 blocks

            // Read object types
            names = new string[3][];
            for (int i = 0; i < 3; i++) {
                R3Pointer off_names_first = R3Pointer.Read(reader);
                R3Pointer off_names_last = R3Pointer.Read(reader);
                uint num_names = reader.ReadUInt32();
                off_current = R3Pointer.Goto(ref reader, off_names_first);
                names[i] = new string[num_names];
                for (int j = 0; j < num_names; j++) {
                    R3Pointer off_names_next = R3Pointer.Read(reader);
                    R3Pointer off_names_prev = R3Pointer.Read(reader);
                    R3Pointer off_header = R3Pointer.Read(reader);
                    R3Pointer off_name = R3Pointer.Read(reader);
                    uint type = reader.ReadUInt32();
                    /*It's actually more like this, but let's just read it as a uint32:
                    byte type1 = reader.ReadByte();
                    byte type2 = reader.ReadByte();
                    uint type3 = reader.ReadUInt16();*/
                    R3Pointer.Goto(ref reader, off_name);
                    names[i][j] = reader.ReadNullDelimitedString();
                    if (off_names_next != null) R3Pointer.Goto(ref reader, off_names_next);
                }
                R3Pointer.Goto(ref reader, off_current);
            }

            R3Pointer off_light = R3Pointer.Read(reader); // the offset of a light. It's just an ordinary light.
            R3Pointer off_characterLaunchingSoundEvents = R3Pointer.Read(reader);
            R3Pointer off_collisionGeoObj = R3Pointer.Read(reader);
            R3Pointer off_staticCollisionGeoObj = R3Pointer.Read(reader);
            if (!hasTransit) {
                reader.ReadUInt32(); // viewport related
            }

            R3Pointer off_unknown_first = R3Pointer.Read(reader);
            R3Pointer off_unknown_last = R3Pointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();

            R3Pointer off_familiesTable_first = R3Pointer.Read(reader);
            R3Pointer off_familiesTable_last = R3Pointer.Read(reader);
            uint num_familiesTable_entries = reader.ReadUInt32();

            R3Pointer off_alwaysActiveCharacters_first = R3Pointer.Read(reader);
            R3Pointer off_alwaysActiveCharacters_last = R3Pointer.Read(reader);
            uint num_alwaysActiveChars = reader.ReadUInt32();

            if (!hasTransit) {
                R3Pointer off_mainCharacters_first = R3Pointer.Read(reader);
                R3Pointer off_mainCharacters_last = R3Pointer.Read(reader);
                uint num_mainCharacters_entries = reader.ReadUInt32();
            }
            reader.ReadUInt32(); // only used if there was no transit in the previous lvl. Always 00165214 in R3GC?
            reader.ReadUInt32(); // related to "SOL". What is this? Good question.
            reader.ReadUInt32(); // same
            reader.ReadUInt32(); // same
            R3Pointer off_cineManager = R3Pointer.Read(reader);
            byte unk = reader.ReadByte();
            byte IPO_numRLItables = reader.ReadByte();
            reader.ReadUInt16();
            R3Pointer off_COL_taggedFacesTable = R3Pointer.Read(reader);
            uint num_COL_maxTaggedFaces = reader.ReadUInt32();
            off_collisionGeoObj = R3Pointer.Read(reader);
            off_staticCollisionGeoObj = R3Pointer.Read(reader);

            // The ptrsTable seems to be related to sound events. Perhaps cuuids.
            reader.ReadUInt32();
            uint num_ptrsTable = reader.ReadUInt32();
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                uint bool_ptrsTable = reader.ReadUInt32();
            }
            R3Pointer off_ptrsTable = R3Pointer.Read(reader);


            uint num_internalStructure = num_ptrsTable;
            if (mode == Mode.Rayman3GC) {
                reader.ReadUInt32();
            }
            R3Pointer off_internalStructure_first = R3Pointer.Read(reader);
            R3Pointer off_internalStructure_last = R3Pointer.Read(reader);
            if (!hasTransit && (mode == Mode.Rayman3PC || mode == Mode.Rayman3GC)) {
                uint num_geometric = reader.ReadUInt32();
                R3Pointer off_array_geometric = R3Pointer.Read(reader);
                R3Pointer off_array_geometric_RLI = R3Pointer.Read(reader);
                R3Pointer off_array_transition_flags = R3Pointer.Read(reader);
            } else if (mode == Mode.RaymanArenaPC || mode == Mode.RaymanArenaGC) {
                uint num_unk = reader.ReadUInt32();
                R3Pointer unk_first = R3Pointer.Read(reader);
                R3Pointer unk_last = R3Pointer.Read(reader);
            }

            uint num_visual_materials = reader.ReadUInt32();
            R3Pointer off_array_visual_materials = R3Pointer.Read(reader);
            if (mode == Mode.Rayman3PC || mode == Mode.Rayman3GC || mode == Mode.RaymanArenaPC) {
                R3Pointer off_dynamic_so_list = R3Pointer.Read(reader);

                // Parse SO list
                off_current = R3Pointer.Goto(ref reader, off_dynamic_so_list);
                R3Pointer off_so_list_first = R3Pointer.Read(reader);
                R3Pointer off_so_list_last = R3Pointer.Read(reader);
                R3Pointer off_so_list_current = off_so_list_first;
                uint num_so_list = reader.ReadUInt32();
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
                R3Pointer.Goto(ref reader, off_current);
            }

            // Parse materials list
            off_current = R3Pointer.Goto(ref reader, off_array_visual_materials);
            materials = new R3Material[num_visual_materials];
            for (uint i = 0; i < num_visual_materials; i++) {
                R3Pointer off_material = R3Pointer.Read(reader);
                R3Pointer off_current_mat = R3Pointer.Goto(ref reader, off_material);
                materials[i] = R3Material.Read(reader, off_material);
                materials[i].offset = off_material;
                R3Pointer.Goto(ref reader, off_current_mat);
            }
            R3Pointer.Goto(ref reader, off_current);

            if (num_familiesTable_entries > 0) {
                families = new R3Family[num_familiesTable_entries];
                GameObject familiesParent = new GameObject("Families");
                familiesParent.SetActive(false); // Families do not need to be visible
                off_current = R3Pointer.Goto(ref reader, off_familiesTable_first);
                R3Pointer off_familiesTable_current = off_familiesTable_first;
                for (uint i = 0; i < num_familiesTable_entries; i++) {
                    families[i] = R3Family.Read(reader, off_familiesTable_current);
                    families[i].Gao.transform.SetParent(familiesParent.transform,false);
                    off_familiesTable_current = families[i].off_family_next;
                    if (off_familiesTable_current != null) R3Pointer.Goto(ref reader, off_familiesTable_current);
                }
                R3Pointer.Goto(ref reader, off_current);
            }

            if (hasTransit) {
                R3Pointer startPointer = new R3Pointer(16, 2); // It's located at offset 20 in transit
                off_current = R3Pointer.Goto(ref reader, startPointer);
                if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                    R3Pointer off_current2;
                    R3Pointer off_lightMapTexture = R3Pointer.Read(reader); // g_p_stLMTexture
                    if (off_lightMapTexture != null) {
                        off_current2 = R3Pointer.Goto(ref reader, off_lightMapTexture);
                        lightmapTexture = R3Texture.Read(reader, off_lightMapTexture);
                        R3Pointer.Goto(ref reader, off_current2);
                    }
                    if (mode == Mode.Rayman3PC) {
                        R3Pointer off_overlightTexture = R3Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
                        if (off_overlightTexture != null) {
                            off_current2 = R3Pointer.Goto(ref reader, off_overlightTexture);
                            overlightTexture = R3Texture.Read(reader, off_overlightTexture);
                            R3Pointer.Goto(ref reader, off_current2);
                        }
                    }
                }
                off_actual_world = R3Pointer.Read(reader);
                off_dynamic_world = R3Pointer.Read(reader);
                off_inactiveDynamic_world = R3Pointer.Read(reader);
                R3Pointer.Goto(ref reader, off_current);
            }
            R3Pointer.Goto(ref reader, off_actual_world);
            List<R3SuperObject> superObjectsActual = R3SuperObject.Read(reader, off_actual_world, false, true);
            if (hasTransit) {
                R3Pointer.Goto(ref reader, off_dynamic_world);
                superObjectsActual.AddRange(R3SuperObject.Read(reader, off_dynamic_world, false, true));
            }
            // Parse special SO's
            if (off_spawnable_perso_first != null && num_spawnable_perso > 0) {
                GameObject spawnableParent = new GameObject("Spawnable persos");
                spawnableParent.transform.localPosition = Vector3.zero;
                R3Pointer.Goto(ref reader, off_spawnable_perso_first);
                for (uint i = 0; i < num_spawnable_perso; i++) {
                    R3Pointer off_spawnable_next = R3Pointer.Read(reader);
                    R3Pointer off_spawnable_prev = R3Pointer.Read(reader);
                    R3Pointer off_spawnable_header = R3Pointer.Read(reader);
                    uint index = reader.ReadUInt32();
                    R3Pointer off_spawnable_perso = R3Pointer.Read(reader);
                    if (off_spawnable_perso != null) {
                        R3Pointer.Goto(ref reader, off_spawnable_perso);
                        R3Perso perso = R3Perso.Read(reader, off_spawnable_perso, null);
                        if (perso != null) perso.Gao.transform.parent = spawnableParent.transform;
                    }
                    if (off_spawnable_next != null) R3Pointer.Goto(ref reader, off_spawnable_next);
                }
            }
            R3Pointer.Goto(ref reader, off_current);

            // off_current should be after the dynamic SO list positions.

            // Parse transformation matrices and other settings(state? :o) for fix characters
            uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) num_perso_with_settings_in_fix = reader.ReadUInt32();
            for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
                R3Pointer off_perso_so_with_settings_in_fix = null, off_matrix = null;
                R3SuperObject so = null;
                R3Matrix mat = null;
                if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                    off_perso_so_with_settings_in_fix = R3Pointer.Read(reader);
                    off_matrix = R3Pointer.Current(reader);
                    mat = R3Matrix.Read(reader, off_matrix);
                    reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
                    reader.ReadUInt32();
                    so = R3SuperObject.FromOffset(off_perso_so_with_settings_in_fix);
                } else if (mode == Mode.RaymanArenaGC || mode == Mode.RaymanArenaPC) {
                    off_matrix = R3Pointer.Current(reader);
                    mat = R3Matrix.Read(reader, off_matrix);
                    so = superObjects.Where(s => s.off_data == persoInFix[i]).FirstOrDefault();
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
            if (mode == Mode.Rayman3GC || mode == Mode.RaymanArenaGC) {
                reader.ReadBytes(0x800); // floats
            }
            R3Pointer off_animBankLvl = R3Pointer.Read(reader); // Note: 4 0x104 banks in lvl.
            print("Lvl animation bank address: " + off_animBankLvl);
            if (off_animBankLvl != null) {
                off_current = R3Pointer.Goto(ref reader, off_animBankLvl);
                R3AnimationBank[] banks = R3AnimationBank.Read(reader, off_animBankLvl, 1, 4);
                for (int i = 0; i < 4; i++) {
                    animationBanks[1 + i] = banks[i];
                }
                R3Pointer.Goto(ref reader, off_current);
            }
        }

        Texture2D CreateDummyTexture() {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f));
            texture.Apply();
            return texture;
        }

        public void print(string str) {
            MonoBehaviour.print(str);
        }
    }
}
