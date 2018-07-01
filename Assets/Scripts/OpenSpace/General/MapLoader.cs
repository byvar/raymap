using OpenSpace.AI;
using OpenSpace.Animation;
using OpenSpace.EngineObject;
using OpenSpace.FileFormat;
using OpenSpace.FileFormat.Texture;
using OpenSpace.Waypoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace {
    public class MapLoader {
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
        public enum Mode { Rayman3PC, Rayman3GC, RaymanArenaPC, RaymanArenaGC, Rayman2PC, Rayman2IOS };
        public Mode mode = Mode.Rayman3PC;

        public ObjectType[][] objectTypes;

        public TextureInfo[] textures;
        public VisualMaterial[] materials;
        public TextureInfo overlightTexture;
        public TextureInfo lightmapTexture;
        public Pointer[] persoInFix;
        public AnimationBank[] animationBanks;
        public Family[] families;

        uint off_textures_start_fix = 0;
        bool hasTransit;
        public bool HasTransit {
            get { return hasTransit; }
        }

        public List<SuperObject> superObjects = new List<SuperObject>();
        public List<LightInfo> lights = new List<LightInfo>();
        public List<Sector> sectors = new List<Sector>();
        public List<PhysicalObject> physicalObjects = new List<PhysicalObject>(); // only required for quick switching between visual & collision geometry
        public List<AIModel> aiModels = new List<AIModel>();
        public List<Perso> persos = new List<Perso>();
        public List<State> states = new List<State>();
        public List<Graph> graphs = new List<Graph>();
        public Dictionary<Pointer, string> strings = new Dictionary<Pointer, string>();
        public GameObject graphRoot;
        //List<R3GeometricObject> parsedGO = new List<R3GeometricObject>();
        
        public Dictionary<ushort, SNAMemoryBlock> relocation_global = new Dictionary<ushort, SNAMemoryBlock>();
        public FileWithPointers[] files_array = new FileWithPointers[7];


        string[] lvlNames = new string[7];
        string[] lvlPaths = new string[7];
        string[] ptrPaths = new string[7];
        string[] tplPaths = new string[7];
        string[] cntPaths = null;
        CNT cnt = null;
        string menuTPLPath;

        public Globals globals = null;
        public Settings settings = null;

        public static class Mem {
            public const int Fix = 0;
            public const int Lvl = 1;
            public const int Transit = 2;
            // 3 is also transit
            public const int VertexBuffer = 4;
            public const int FixKeyFrames = 5;
            public const int LvlKeyFrames = 6;
        }
        public int[] loadOrder = new int[] { Mem.Fix, Mem.Transit, Mem.Lvl, Mem.VertexBuffer, Mem.FixKeyFrames, Mem.LvlKeyFrames };

        
        static MapLoader loader = null;
        public static MapLoader Loader {
            get {
                if (loader == null) {
                    loader = new MapLoader();
                }
                return loader;
            }
        }

        public MapLoader() {
        }
        
        public void Load() {
            try {
                switch (mode) {
                    case Mode.Rayman2IOS: settings = Settings.R2IOS; break;
                    case Mode.Rayman2PC: settings = Settings.R2PC; break;
                    case Mode.Rayman3GC: settings = Settings.R3GC; break;
                    case Mode.Rayman3PC: settings = Settings.R3PC; break;
                    case Mode.RaymanArenaGC: settings = Settings.RAGC; break;
                    case Mode.RaymanArenaPC: settings = Settings.RAPC; break;
                }
                Settings.s = settings;

                graphRoot = new GameObject("Graphs");
                graphRoot.SetActive(false);

                if (gameDataBinFolder == null || !Directory.Exists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
                if (settings.engineMode == Settings.EngineMode.R2) {
                    hasTransit = false;
                    DAT dat = null;
                    if (mode == Mode.Rayman2PC) {
                        string dataPath = Path.Combine(gameDataBinFolder, "levels0.dat");
                        dat = new DAT("levels0", dataPath);
                    }
                    
                    /*DSB dsb = new DSB(lvlName, Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".dsb"));
                    DSB gameDsb = new DSB("Game", Path.Combine(gameDataBinFolder, "Game.dsb"));
                    gameDsb.Save(Path.Combine(gameDataBinFolder, "Game_dec.data"));
                    dsb.Dispose();
                    gameDsb.Dispose();*/

                    string fixSnaPath = Path.Combine(gameDataBinFolder, "fix.sna");
                    RelocationTable fixRtb = new RelocationTable(fixSnaPath, dat, "fix", RelocationType.RTB);
                    SNA fixSna = new SNA("fix", fixSnaPath, fixRtb);
                    string fixGptPath = Path.Combine(gameDataBinFolder, "fix.gpt");
                    RelocationTable fixRtp = new RelocationTable(fixGptPath, dat, "fix", RelocationType.RTP);
                    fixSna.ReadGPT(fixGptPath, fixRtp);
                    
                    string fixPtxPath = Path.Combine(gameDataBinFolder, "fix.ptx");
                    RelocationTable fixRtt = new RelocationTable(fixPtxPath, dat, "fix", RelocationType.RTT);
                    fixSna.ReadPTX(fixPtxPath, fixRtt);

                    string lvlSnaPath = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".sna");
                    RelocationTable lvlRtb = new RelocationTable(lvlSnaPath, dat, lvlName, RelocationType.RTB);
                    SNA lvlSna = new SNA(lvlName, lvlSnaPath, lvlRtb);
                    string lvlGptPath = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".gpt");
                    RelocationTable lvlRtp = new RelocationTable(lvlGptPath, dat, lvlName, RelocationType.RTP);
                    lvlSna.ReadGPT(lvlGptPath, lvlRtp);
                    string lvlPtxPath = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".ptx");
                    RelocationTable lvlRtt = new RelocationTable(lvlPtxPath, dat, lvlName, RelocationType.RTT);
                    lvlSna.ReadPTX(lvlPtxPath, lvlRtt);


                    fixSna.CreatePointers();
                    lvlSna.CreatePointers();

                    files_array[0] = fixSna;
                    files_array[1] = lvlSna;
                    files_array[2] = dat;

                    fixSna.CreateMemoryDump(Path.Combine(gameDataBinFolder, "fix.dmp"), true);
                    lvlSna.CreateMemoryDump(Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".dmp"), true);

                    if (mode != Mode.Rayman2IOS) {
                        cntPaths = new string[2];
                        cntPaths[0] = Path.Combine(gameDataBinFolder, "Vignette.cnt");
                        cntPaths[1] = Path.Combine(gameDataBinFolder, "Textures.cnt");
                        cnt = new CNT(cntPaths);
                    }

                    LoadFIXSNA();
                    LoadLVLSNA();

                    fixSna.Dispose();
                    lvlSna.Dispose();
                    if(dat != null) dat.Dispose();

                } else if (settings.engineMode == Settings.EngineMode.R3) {
                    
                    menuTPLPath = Path.Combine(gameDataBinFolder, "menu.tpl");
                    lvlNames[0] = "fix";
                    lvlPaths[0] = Path.Combine(gameDataBinFolder, "fix.lvl");
                    ptrPaths[0] = Path.Combine(gameDataBinFolder, "fix.ptr");
                    tplPaths[0] = Path.Combine(gameDataBinFolder, ((mode == Mode.RaymanArenaGC) ? "../common.tpl" : "fix.tpl"));

                    lvlNames[1] = lvlName;
                    lvlPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".lvl");
                    ptrPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".ptr");
                    tplPaths[1] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ((mode == Mode.RaymanArenaGC) ? ".tpl" : "_Lvl.tpl"));

                    lvlNames[2] = "transit";
                    lvlPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/transit.lvl");
                    ptrPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/transit.ptr");
                    tplPaths[2] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "_Trans.tpl");
                    hasTransit = File.Exists(lvlPaths[2]) && (new FileInfo(lvlPaths[2]).Length > 4);

                    lvlNames[4] = lvlName + "_vb";
                    lvlPaths[4] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "_vb.lvl");
                    ptrPaths[4] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "_vb.ptr");

                    lvlNames[5] = "fixkf";
                    lvlPaths[5] = Path.Combine(gameDataBinFolder, "fixkf.lvl");
                    ptrPaths[5] = Path.Combine(gameDataBinFolder, "fixkf.ptr");

                    lvlNames[6] = lvlName + "kf";
                    lvlPaths[6] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "kf.lvl");
                    ptrPaths[6] = Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + "kf.ptr");

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

                    for (int i = 0; i < lvlPaths.Length; i++) {
                        if (lvlPaths[i] == null) continue;
                        if (File.Exists(lvlPaths[i])) {
                            files_array[i] = new LVL(lvlNames[i], lvlPaths[i], i);
                        }
                    }
                    for (int i = 0; i < loadOrder.Length; i++) {
                        int j = loadOrder[i];
                        if (files_array[j] != null && File.Exists(ptrPaths[j])) {
                            ((LVL)files_array[j]).ReadPTR(ptrPaths[j]);
                        }
                    }
                    LoadFIX();
                    LoadLVL();
                }
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            } finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
                if (cnt != null) cnt.Dispose();
            }
            InitModdables();
        }

        public void InitModdables() {
            foreach (SuperObject so in superObjects) {
                GameObject gao = so.Gao;
                if (gao != null) {
                    Moddable mod = gao.AddComponent<Moddable>();
                    mod.mat = so.matrix;
                }
            }
        }

        public void SaveModdables() {
            EndianBinaryWriter writer = files_array[1].writer;
            if (writer == null) return;
            foreach (SuperObject so in superObjects) {
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
                        FileStream stream = new FileStream(lvlPaths[i], FileMode.Open);
                        files_array[i].writer = new EndianBinaryWriter(stream, settings.IsLittleEndian);
                    }
                }
                // Save changes
                SaveModdables();
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            } finally {
                for (int i = 0; i < 3; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
            }
        }

        #region FIX
        void LoadFIX() {
            files_array[Mem.Fix].GotoHeader();
            EndianBinaryReader reader = files_array[Mem.Fix].reader;
            // Read fix header
            //reader.ReadUInt32();
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
            Pointer off_unk1 = Pointer.Read(reader);
            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0
            reader.ReadUInt16(); // 0x0A in R3, 0x09 in RA PC, so a number of something
            reader.ReadUInt16(); // 0
            Pointer off_unk2 = Pointer.Read(reader);
            Pointer off_unk3 = Pointer.Read(reader);
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
            Pointer off_languages = Pointer.Read(reader);
            Pointer off_current = Pointer.Goto(ref reader, off_languages);
            for (uint i = 0; i < num_languages; i++) {
                string language_us = new string(reader.ReadChars(0x14));
                string language_loc = new string(reader.ReadChars(0x14));
            }
            Pointer.Goto(ref reader, off_current);
            uint num_textures = reader.ReadUInt32();
            print("Texture count fix: " + num_textures);
            textures = new TextureInfo[num_textures];
            if (num_textures > 0) {
                for (uint i = 0; i < num_textures; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    off_current = Pointer.Goto(ref reader, off_texture);
                    textures[i] = TextureInfo.Read(reader, off_texture);
                    Pointer.Goto(ref reader, off_current);
                }
                if (mode == Mode.Rayman3GC || mode == Mode.RaymanArenaGC) {
                    uint num_textures_menu = reader.ReadUInt32();
                    TPL fixTPL = new TPL(tplPaths[Mem.Fix]);
                    TPL menuTPL = new TPL(menuTPLPath);
                    for (uint i = 0; i < num_textures_menu; i++) {
                        Pointer off_texture = Pointer.Read(reader);
                        TextureInfo tex = textures.Where(t => t.offset == off_texture).First();
                        tex.texture = menuTPL.textures[i];
                    }
                    for (int i = 0, j = 0; i < fixTPL.Count; i++, j++) {
                        while (textures[j].texture != null) j++;
                        textures[j].texture = fixTPL.textures[i];
                    }
                } else if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                    for (int i = 0; i < num_textures; i++) {
                        GF gf = cnt.GetGFByTGAName(textures[i].name);
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
            Pointer keypadDefine = Pointer.Read(reader);
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
                reader.ReadBytes(sz_fontStructure); // Font definition
            }
            reader.Align(4); // Align position
            for (int i = 0; i < num_fontBitmap; i++) {
                Pointer off_fontTexture = Pointer.Read(reader);
            }
            reader.ReadBytes(sz_videoStructure); // Contains amount of videos and pointer to video filename table
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                uint num_musicMarkerSlots = reader.ReadUInt32();
                for (int i = 0; i < num_musicMarkerSlots; i++) {
                    reader.ReadBytes(sz_musicMarkerSlot);
                }
                reader.ReadBytes(sz_binDataForMenu);
                if (mode == Mode.Rayman3PC) {
                    Pointer off_bgMaterialForMenu2D = Pointer.Read(reader);
                    Pointer off_fixMaterialForMenu2D = Pointer.Read(reader);
                    Pointer off_fixMaterialForSelectedFilms = Pointer.Read(reader);
                    Pointer off_fixMaterialForArcadeAndFilms = Pointer.Read(reader);
                    for (int i = 0; i < 35; i++) { // 35 is again hardcoded
                        Pointer off_menuPage = Pointer.Read(reader);
                    }
                }
            }
            Pointer off_animBankFix = Pointer.Read(reader); // Note: only one 0x104 bank in fix.
            print("Fix animation bank address: " + off_animBankFix);
            animationBanks = new AnimationBank[5]; // 1 in fix, 4 in lvl
            if (off_animBankFix != null) {
                off_current = Pointer.Goto(ref reader, off_animBankFix);
                animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
                Pointer.Goto(ref reader, off_current);
            }
        }
        #endregion

        #region LVL
        void LoadLVL() {
            files_array[Mem.Lvl].GotoHeader();
            EndianBinaryReader reader = files_array[Mem.Lvl].reader;
            long totalSize = reader.BaseStream.Length;
            Pointer off_current = null;
            //reader.ReadUInt32();
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
                    Pointer off_texture = Pointer.Read(reader);
                    off_current = Pointer.Goto(ref reader, off_texture);
                    textures[num_textures_fix + i] = TextureInfo.Read(reader, off_texture);
                    Pointer.Goto(ref reader, off_current);
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
                    Pointer off_texture = Pointer.Read(reader);
                    if (off_texture != null) {
                        off_current = Pointer.Goto(ref reader, off_texture);
                        textures[num_textures_fix + i] = TextureInfo.Read(reader, off_texture);
                        Pointer.Goto(ref reader, off_current);
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
                        GF gf = cnt.GetGFByTGAName(textures[num_textures_fix + i].name);
                        if (gf != null) textures[num_textures_fix + i].texture = gf.GetTexture();
                        transitTexturesSeen++;
                        //textures[num_textures_fix + i].texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        GF gf = cnt.GetGFByTGAName(textures[num_textures_fix + i].name);
                        if (gf != null) textures[num_textures_fix + i].texture = gf.GetTexture();
                        //textures[num_textures_fix + i].texture = lvlTPL.textures[i - transitTexturesSeen];
                    }
                }
                if (!hasTransit) {
                    Pointer off_lightMapTexture = Pointer.Read(reader); // g_p_stLMTexture
                    if (off_lightMapTexture != null) {
                        off_current = Pointer.Goto(ref reader, off_lightMapTexture);
                        lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
                        Pointer.Goto(ref reader, off_current);
                    }
                    if (mode == Mode.Rayman3PC) {
                        Pointer off_overlightTexture = Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
                        if (off_overlightTexture != null) {
                            off_current = Pointer.Goto(ref reader, off_overlightTexture);
                            overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
                            Pointer.Goto(ref reader, off_current);
                        }
                    }
                }
            }
            globals.off_transitDynamicWorld = null;
            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            if (mode == Mode.Rayman3PC) reader.ReadUInt32();
            globals.off_inactiveDynamicWorld = Pointer.Read(reader);
            globals.off_fatherSector = Pointer.Read(reader); // It is I, Father Sector.
            globals.off_firstSubMapPosition = Pointer.Read(reader);
            
            globals.num_always = reader.ReadUInt32();
            globals.off_spawnable_perso_first = Pointer.Read(reader);
            globals.off_spawnable_perso_last = Pointer.Read(reader);
            globals.num_spawnable_perso = reader.ReadUInt32();
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
                reader.ReadUInt32(); // viewport related
            }

            Pointer off_unknown_first = Pointer.Read(reader);
            Pointer off_unknown_last = Pointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();

            globals.off_familiesTable_first = Pointer.Read(reader);
            globals.off_familiesTable_last = Pointer.Read(reader);
            globals.num_familiesTable_entries = reader.ReadUInt32();

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
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                uint bool_ptrsTable = reader.ReadUInt32();
            }
            Pointer off_ptrsTable = Pointer.Read(reader);


            uint num_internalStructure = num_ptrsTable;
            if (mode == Mode.Rayman3GC) {
                reader.ReadUInt32();
            }
            Pointer off_internalStructure_first = Pointer.Read(reader);
            Pointer off_internalStructure_last = Pointer.Read(reader);
            if (!hasTransit && (mode == Mode.Rayman3PC || mode == Mode.Rayman3GC)) {
                uint num_geometric = reader.ReadUInt32();
                Pointer off_array_geometric = Pointer.Read(reader);
                Pointer off_array_geometric_RLI = Pointer.Read(reader);
                Pointer off_array_transition_flags = Pointer.Read(reader);
            } else if (mode == Mode.RaymanArenaPC || mode == Mode.RaymanArenaGC) {
                uint num_unk = reader.ReadUInt32();
                Pointer unk_first = Pointer.Read(reader);
                Pointer unk_last = Pointer.Read(reader);
            }

            uint num_visual_materials = reader.ReadUInt32();
            Pointer off_array_visual_materials = Pointer.Read(reader);
            if (mode == Mode.Rayman3PC || mode == Mode.Rayman3GC || mode == Mode.RaymanArenaPC) {
                Pointer off_dynamic_so_list = Pointer.Read(reader);

                // Parse SO list
                off_current = Pointer.Goto(ref reader, off_dynamic_so_list);
                Pointer off_so_list_first = Pointer.Read(reader);
                Pointer off_so_list_last = Pointer.Read(reader);
                Pointer off_so_list_current = off_so_list_first;
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
                Pointer.Goto(ref reader, off_current);
            }

            // Parse materials list
            off_current = Pointer.Goto(ref reader, off_array_visual_materials);
            materials = new VisualMaterial[num_visual_materials];
            for (uint i = 0; i < num_visual_materials; i++) {
                Pointer off_material = Pointer.Read(reader);
                Pointer off_current_mat = Pointer.Goto(ref reader, off_material);
                materials[i] = VisualMaterial.Read(reader, off_material);
                materials[i].offset = off_material;
                Pointer.Goto(ref reader, off_current_mat);
            }
            Pointer.Goto(ref reader, off_current);

            if (hasTransit) {
                Pointer startPointer = new Pointer(16, files_array[Mem.Transit]); // It's located at offset 20 in transit
                off_current = Pointer.Goto(ref reader, startPointer);
                if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                    Pointer off_current2;
                    Pointer off_lightMapTexture = Pointer.Read(reader); // g_p_stLMTexture
                    if (off_lightMapTexture != null) {
                        off_current2 = Pointer.Goto(ref reader, off_lightMapTexture);
                        lightmapTexture = TextureInfo.Read(reader, off_lightMapTexture);
                        Pointer.Goto(ref reader, off_current2);
                    }
                    if (mode == Mode.Rayman3PC) {
                        Pointer off_overlightTexture = Pointer.Read(reader); // *(_DWORD *)(GLI_BIG_GLOBALS + 370068)
                        if (off_overlightTexture != null) {
                            off_current2 = Pointer.Goto(ref reader, off_overlightTexture);
                            overlightTexture = TextureInfo.Read(reader, off_overlightTexture);
                            Pointer.Goto(ref reader, off_current2);
                        }
                    }
                }
                globals.off_transitDynamicWorld = Pointer.Read(reader);
                globals.off_actualWorld = Pointer.Read(reader);
                globals.off_dynamicWorld = Pointer.Read(reader);
                globals.off_inactiveDynamicWorld = Pointer.Read(reader);
                Pointer.Goto(ref reader, off_current);
            }

            // Parse actual world & always structure
            ReadFamilies(reader);
            ReadSuperObjects(reader);
            ReadAlways(reader);

            // off_current should be after the dynamic SO list positions.

            // Parse transformation matrices and other settings(state? :o) for fix characters
            uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
            if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) num_perso_with_settings_in_fix = reader.ReadUInt32();
            for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
                Pointer off_perso_so_with_settings_in_fix = null, off_matrix = null;
                SuperObject so = null;
                Matrix mat = null;
                if (mode == Mode.Rayman3GC || mode == Mode.Rayman3PC) {
                    off_perso_so_with_settings_in_fix = Pointer.Read(reader);
                    off_matrix = Pointer.Current(reader);
                    mat = Matrix.Read(reader, off_matrix);
                    reader.ReadUInt32(); // is one of these the state? doesn't appear to change tho
                    reader.ReadUInt32();
                    so = SuperObject.FromOffset(off_perso_so_with_settings_in_fix);
                } else if (mode == Mode.RaymanArenaGC || mode == Mode.RaymanArenaPC) {
                    off_matrix = Pointer.Current(reader);
                    mat = Matrix.Read(reader, off_matrix);
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
            Pointer off_animBankLvl = Pointer.Read(reader); // Note: 4 0x104 banks in lvl.
            print("Lvl animation bank address: " + off_animBankLvl);
            if (off_animBankLvl != null) {
                off_current = Pointer.Goto(ref reader, off_animBankLvl);
                AnimationBank[] banks = AnimationBank.Read(reader, off_animBankLvl, 1, 4, files_array[Mem.LvlKeyFrames]);
                for (int i = 0; i < 4; i++) {
                    animationBanks[1 + i] = banks[i];
                }
                Pointer.Goto(ref reader, off_current);
            }
            // Load additional animation banks
            for (int i = 0; i < families.Length; i++) {
                if (families[i].animBank > 4 && objectTypes[0][families[i].family_index].id != 0xFF) {
                    int animBank = families[i].animBank;
                    string animName = "Anim/ani" + objectTypes[0][families[i].family_index].id.ToString();
                    string kfName = "Anim/key" + objectTypes[0][families[i].family_index].id.ToString() + "kf";
                    int fileID = animBank + 102;
                    int kfFileID = animBank + 2; // Anim bank will start at 5, so this will start at 7
                    FileWithPointers animFile = InitExtraLVL(animName, fileID);
                    FileWithPointers kfFile = InitExtraLVL(kfName, fileID);
                    if (animFile != null) {
                        if (animBank >= animationBanks.Length) {
                            Array.Resize(ref animationBanks, animBank + 1);
                        }
                        Pointer off_animBankExtra = new Pointer(0, animFile);
                        off_current = Pointer.Goto(ref reader, off_animBankExtra);
                        int alignBytes = reader.ReadInt32();
                        if (alignBytes > 0) reader.Align(4, alignBytes);
                        off_animBankExtra = Pointer.Current(reader);
                        animationBanks[animBank] = AnimationBank.Read(reader, off_animBankExtra, (uint)animBank, 1, kfFile)[0];
                        Pointer.Goto(ref reader, off_current);
                    }
                }
            }
        }
        #endregion

        #region FIXSNA
        void LoadFIXSNA() {
            files_array[Mem.Fix].GotoHeader();
            EndianBinaryReader reader = files_array[Mem.Fix].reader;
            print("FIX GPT offset: " + Pointer.Current(reader));
            Pointer off_identityMatrix = Pointer.Read(reader);
            reader.ReadBytes(50 * 4);
            uint matrixInStack = reader.ReadUInt32();
            Pointer off_collisionGeoObj = Pointer.Read(reader);
            Pointer off_staticCollisionGeoObj = Pointer.Read(reader);
            reader.ReadBytes(0xAC); // 3DOS_EntryActions
            Pointer off_IPT_keyAndPadDefine = Pointer.Read(reader);
            if (mode == Mode.Rayman2IOS) {
                reader.ReadBytes(0x2BC); // IPT_g_hInputStructure
            } else {
                reader.ReadBytes(0xB20); // IPT_g_hInputStructure
            }
            Pointer off_IPT_entryElementList = Pointer.Read(reader);
            reader.ReadBytes(0x14); // FON_g_stGeneral
            Pointer off_current = Pointer.Current(reader);
            animationBanks = new AnimationBank[2]; // 1 in fix, 1 in lvl
            animationBanks[0] = AnimationBank.Read(reader, off_current, 0, 1, files_array[Mem.FixKeyFrames])[0];
            print("Fix animation bank: " + off_current);
            Pointer off_fixInfo = Pointer.Read(reader);

            // Read PTX
            ((SNA)files_array[Mem.Fix]).GotoPTX();
            uint num_textureMemoryChannels = reader.ReadUInt32();
            uint num_textures = reader.ReadUInt32();
            print("Texture count fix: " + num_textures);
            textures = new TextureInfo[num_textures];
            for (uint i = 0; i < num_textures; i++) {
                Pointer off_texture = Pointer.Read(reader);
                if (off_texture != null) {
                    off_current = Pointer.Goto(ref reader, off_texture);
                    textures[i] = TextureInfo.Read(reader, off_texture);
                    Pointer.Goto(ref reader, off_current);
                } else textures[i] = null;
            }
            for (int i = 0; i < num_textures; i++) {
                if (textures[i] != null) {
                    if (mode == Mode.Rayman2IOS) {
                        string texturePath = Path.ChangeExtension(Path.Combine(gameDataBinFolder, "../graphics/textures/" + textures[i].name), ".gf");
                        if (File.Exists(texturePath)) {
                            GF gf = new GF(texturePath);
                            if (gf != null) textures[i].texture = gf.GetTexture();
                        }
                    } else {
                        GF gf = cnt.GetGFByTGAName(textures[i].name);
                        if (gf != null) textures[i].texture = gf.GetTexture();
                    }
                }
            }
            /*uint num_texturesToCreate = reader.ReadUInt32();
            for (uint i = 0; i < num_texturesToCreate; i++) {
                reader.ReadUInt32(); //1
            }
            uint currentMemoryChannel = reader.ReadUInt32();*/

        }
        #endregion

        #region LVLSNA
        void LoadLVLSNA() {
            EndianBinaryReader reader = files_array[Mem.Lvl].reader;
            Pointer off_current;

            // First read GPT
            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;
            print("LVL GPT offset: " + Pointer.Current(reader));

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
                        Pointer.Goto(ref reader, off_stdGame);
                        reader.ReadUInt32(); // type 0
                        reader.ReadUInt32(); // type 1
                        reader.ReadUInt32(); // type 2
                        Pointer off_superObject = Pointer.Read(reader);
                        if (off_superObject != null) {
                            // First read everything from the GPT
                            Pointer.Goto(ref reader, off_current);
                            
                            Pointer off_newSuperObject = Pointer.Read(reader);
                            byte[] matrixData = reader.ReadBytes(0x58);
                            byte[] renderBits = reader.ReadBytes(4);
                            byte[] floatData = reader.ReadBytes(4);
                            Pointer off_nextBrother = Pointer.Read(reader);
                            Pointer off_prevBrother = Pointer.Read(reader);
                            Pointer off_father = Pointer.Read(reader);

                            // Then fill everything in
                            off_current = Pointer.Goto(ref reader, off_newSuperObject);
                            uint newSOtype = reader.ReadUInt32();
                            Pointer off_newSOengineObject = Pointer.Read(reader);
                            if (newSOtype == 2) {
                                persoInFixPointers[i] = off_newSOengineObject;
                            } else {
                                persoInFixPointers[i] = null;
                            }
                            Pointer.Goto(ref reader, off_newSOengineObject);
                            Pointer off_renderInfo = Pointer.Read(reader);
                            ((SNA)off_renderInfo.file).OverwriteData(off_renderInfo.offset + 0x18, matrixData);

                            FileWithPointers file = off_newSuperObject.file;
                            file.AddPointer(off_newSuperObject.offset + 0x14, off_nextBrother);
                            file.AddPointer(off_newSuperObject.offset + 0x18, off_prevBrother);
                            file.AddPointer(off_newSuperObject.offset + 0x1C, off_father);
                            ((SNA)file).OverwriteData(off_newSuperObject.offset + 0x30, renderBits);
                            ((SNA)file).OverwriteData(off_newSuperObject.offset + 0x38, floatData);

                        }
                    }
                    Pointer.Goto(ref reader, off_current);
                }
            }

            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            globals.off_inactiveDynamicWorld = Pointer.Read(reader);
            globals.off_fatherSector = Pointer.Read(reader);
            globals.off_firstSubMapPosition = Pointer.Read(reader);

            globals.num_always = reader.ReadUInt32();
            globals.off_spawnable_perso_first = Pointer.Read(reader);
            globals.off_spawnable_perso_last = Pointer.Read(reader);
            globals.num_spawnable_perso = reader.ReadUInt32();
            globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
            globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks

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
            reader.ReadByte();
            string mapName = new string(reader.ReadChars(0x1E)).TrimEnd('\0');
            reader.ReadChars(0x1E);
            string mapName2 = new string(reader.ReadChars(0x1E)).TrimEnd('\0');
            reader.ReadByte();
            reader.ReadBytes(0x178); // don't know what this data is

            Pointer off_unknown_first = Pointer.Read(reader);
            Pointer off_unknown_last = Pointer.Read(reader);
            uint num_unknown = reader.ReadUInt32();
            
            globals.off_familiesTable_first = Pointer.Read(reader);
            globals.off_familiesTable_last = Pointer.Read(reader);
            globals.num_familiesTable_entries = reader.ReadUInt32();
            FillLinkedListPointers(reader, globals.off_familiesTable_last, globals.off_familiesTable_first);

            Pointer off_alwaysActiveCharacters_first = Pointer.Read(reader);
            Pointer off_alwaysActiveCharacters_last = Pointer.Read(reader);
            uint num_alwaysActiveChars = reader.ReadUInt32();

            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            
            for (uint i = 0; i < 2; i++) {
                Pointer off_matrix = Pointer.Current(reader);
                Matrix mat = Matrix.Read(reader, off_matrix);
            }

            /*uint num_perso_with_settings_in_fix = (uint)persoInFix.Length;
            for (int i = 0; i < num_perso_with_settings_in_fix; i++) {
                Pointer off_matrix = null;
                R3SuperObject so = null;
                R3Matrix mat = null;
                off_matrix = Pointer.Current(reader);
                mat = R3Matrix.Read(reader, off_matrix);
                so = superObjects.Where(s => s.off_data == persoInFix[i]).FirstOrDefault();
                if (so != null) {
                    so.off_matrix = off_matrix;
                    so.matrix = mat;
                    if (so.Gao != null) {
                        so.Gao.transform.localPosition = mat.GetPosition(convertAxes: true);
                        so.Gao.transform.localRotation = mat.GetRotation(convertAxes: true);
                        so.Gao.transform.localScale = mat.GetScale(convertAxes: true);
                    }
                }
            }*/

            reader.ReadUInt32();
            reader.ReadUInt16();

            for (int i = 0; i < 80; i++) {
                new string(reader.ReadChars(0x1E)).TrimEnd('\0');
            }
            uint num_mapNames = reader.ReadUInt32();
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();
            // End of engineStructure
            Pointer off_light = Pointer.Read(reader); // the offset of a light. It's just an ordinary light.
            Pointer off_mainChar = Pointer.Read(reader); // superobject
            Pointer off_characterLaunchingSoundEvents = Pointer.Read(reader);
            Pointer off_shadowPolygonVisualMaterial = Pointer.Read(reader);
            Pointer off_shadowPolygonGameMaterialInit = Pointer.Read(reader);
            Pointer off_shadowPolygonGameMaterial = Pointer.Read(reader);
            Pointer off_textureOfTextureShadow = Pointer.Read(reader);
            Pointer off_col_taggedFacesTable = Pointer.Read(reader);
            for (int i = 0; i < 10; i++) {
                Pointer.Read(reader);
                Pointer.Read(reader);
            }
            Pointer.Read(reader);
            off_current = Pointer.Current(reader);
            AnimationBank.Read(reader, off_current, 0, 1, files_array[Mem.LvlKeyFrames], append: true);
            animationBanks[1] = animationBanks[0];


            ((SNA)files_array[0]).CreateMemoryDump(Path.Combine(gameDataBinFolder, "fix.dmp"), true);
            ((SNA)files_array[1]).CreateMemoryDump(Path.Combine(gameDataBinFolder, lvlName + "/" + lvlName + ".dmp"), true);


            // Read PTX
            ((SNA)files_array[Mem.Lvl]).GotoPTX();
            uint num_textures_fix = (uint)textures.Length;
            uint num_textureMemoryChannels = reader.ReadUInt32();
            uint num_textures_lvl = reader.ReadUInt32();
            uint num_textures_total = num_textures_fix + num_textures_lvl;
            Array.Resize(ref textures, (int)num_textures_total);
            for (uint i = num_textures_fix; i < num_textures_total; i++) {
                Pointer off_texture = Pointer.Read(reader);
                if (off_texture != null) {
                    off_current = Pointer.Goto(ref reader, off_texture);
                    textures[i] = TextureInfo.Read(reader, off_texture);
                    Pointer.Goto(ref reader, off_current);
                } else textures[i] = null;
            }
            uint num_texturesToCreate = reader.ReadUInt32();
            for (uint i = 0; i < num_textures_fix; i++) { // ?
                reader.ReadUInt32(); //1
            }
            uint currentMemoryChannel = reader.ReadUInt32();
            for (uint i = num_textures_fix; i < num_textures_total; i++) {
                if (textures[i] != null) {
                    if (mode == Mode.Rayman2IOS) {
                        string texturePath = Path.ChangeExtension(Path.Combine(gameDataBinFolder, "../graphics/textures/" + textures[i].name), ".gf");
                        if (File.Exists(texturePath)) {
                            GF gf = new GF(texturePath);
                            if (gf != null) textures[i].texture = gf.GetTexture();
                        }
                    } else {
                        GF gf = cnt.GetGFByTGAName(textures[i].name);
                        if (gf != null) textures[i].texture = gf.GetTexture();
                    }
                }
                /*uint file_texture = reader.ReadUInt32();
                if (file_texture == 0xC0DE0005) {
                    // texture is undefined
                    textures[i] = null;
                } else if (textures[i] != null) {
                    GF3 gf = cnt.GetGFByTGAName(textures[i].name);
                    if (gf != null) {
                        textures[i].texture = gf.GetTexture();
                    }
                } else {
                    print("Error in texture loading!");
                }*/
            }

            materials = new VisualMaterial[0];

            // Parse actual world & always structure
            ReadFamilies(reader);
            ReadSuperObjects(reader);
            ReadAlways(reader);
        }
        #endregion
        
        public void print(string str) {
            MonoBehaviour.print(str);
        }

        public FileWithPointers GetFileByReader(EndianBinaryReader reader) {
            for (int i = 0; i < files_array.Length; i++) {
                FileWithPointers file = files_array[i];
                if (file != null && reader.Equals(file.reader)) {
                    return file;
                }
            }
            return null;
        }

        public FileWithPointers GetFileByWriter(EndianBinaryWriter writer) {
            for (int i = 0; i < files_array.Length; i++) {
                FileWithPointers file = files_array[i];
                if (file != null && writer.Equals(file.writer)) {
                    return file;
                }
            }
            return null;
        }

        public FileWithPointers InitExtraLVL(string relativePath, int id) {
            string path = Path.Combine(gameDataBinFolder, relativePath);
            string lvlName = Path.GetFileNameWithoutExtension(relativePath);
            string lvlPath = Path.ChangeExtension(path, "lvl");
            string ptrPath = Path.ChangeExtension(path, "ptr");
            if (File.Exists(lvlPath)) {
                Array.Resize(ref files_array, files_array.Length + 1);
                LVL lvl = new LVL(lvlName, lvlPath, id);
                files_array[files_array.Length - 1] = lvl;
                if (File.Exists(ptrPath)) {
                    lvl.ReadPTR(ptrPath);
                }
                return lvl;
            } else {
                return null;
            }
        }

        public void FillLinkedListPointers(EndianBinaryReader reader, Pointer lastEntry, Pointer header, uint nextOffset = 0, uint prevOffset = 4, uint headerOffset = 8) {
            Pointer current_entry = lastEntry;
            Pointer next_entry = null;
            Pointer off_current = Pointer.Current(reader);
            while (current_entry != null) {
                Pointer.Goto(ref reader, current_entry);
                current_entry.file.AddPointer(current_entry.offset + nextOffset, next_entry);
                if (header != null) {
                    current_entry.file.AddPointer(current_entry.offset + headerOffset, header);
                }
                next_entry = current_entry;
                current_entry = Pointer.GetPointerAtOffset(current_entry + prevOffset);
            }
            Pointer.Goto(ref reader, off_current);
        }

        public void ReadObjectNamesTable(EndianBinaryReader reader, Pointer off_names_first, uint num_names, uint index) {
            Pointer off_current = Pointer.Goto(ref reader, off_names_first);
            objectTypes[index] = new ObjectType[num_names];
            for (int j = 0; j < num_names; j++) {
                objectTypes[index][j] = new ObjectType();
                Pointer off_names_next = Pointer.Read(reader);
                Pointer off_names_prev = Pointer.Read(reader);
                Pointer off_header = Pointer.Read(reader);
                Pointer off_name = Pointer.Read(reader);
                objectTypes[index][j].unk1 = reader.ReadByte();
                objectTypes[index][j].id = reader.ReadByte();
                objectTypes[index][j].unk2 = reader.ReadUInt16();
                Pointer.Goto(ref reader, off_name);
                objectTypes[index][j].name = reader.ReadNullDelimitedString();

                if (off_names_next != null) Pointer.Goto(ref reader, off_names_next);
            }
            Pointer.Goto(ref reader, off_current);
        }

        public void ReadSuperObjects(EndianBinaryReader reader) {
            Pointer off_current = Pointer.Goto(ref reader, globals.off_actualWorld);
            List<SuperObject> superObjectsActual = SuperObject.Read(reader, globals.off_actualWorld, false, true);
            if (hasTransit && globals.off_transitDynamicWorld != null) {
                Pointer.Goto(ref reader, globals.off_transitDynamicWorld);
                superObjectsActual.AddRange(SuperObject.Read(reader, globals.off_transitDynamicWorld, false, true));
            }


            SuperObject actualWorld = SuperObject.FromOffset(globals.off_actualWorld);
            SuperObject dynamicWorld = SuperObject.FromOffset(globals.off_dynamicWorld);
            SuperObject inactiveDynamicWorld = SuperObject.FromOffset(globals.off_inactiveDynamicWorld);
            SuperObject transitDynamicWorld = SuperObject.FromOffset(globals.off_transitDynamicWorld);
            SuperObject fatherSector = SuperObject.FromOffset(globals.off_fatherSector);
            if (actualWorld != null) actualWorld.Gao.name = "Actual World";
            if (dynamicWorld != null) dynamicWorld.Gao.name = "Dynamic World";
            if (inactiveDynamicWorld != null) inactiveDynamicWorld.Gao.name = "Inactive Dynamic World";
            if (transitDynamicWorld != null) transitDynamicWorld.Gao.name = "Transit Dynamic World (perso in fix)";
            if (fatherSector != null) fatherSector.Gao.name = "Father Sector";

            Pointer.Goto(ref reader, off_current);
        }

        public void ReadAlways(EndianBinaryReader reader) {
            // Parse spawnable SO's
            if (globals.off_spawnable_perso_first != null && globals.num_spawnable_perso > 0) {
                GameObject spawnableParent = new GameObject("Spawnable persos");
                spawnableParent.transform.localPosition = Vector3.zero;
                Pointer off_current = Pointer.Goto(ref reader, globals.off_spawnable_perso_first);
                for (uint i = 0; i < globals.num_spawnable_perso; i++) {
                    Pointer off_spawnable_next = Pointer.Read(reader);
                    Pointer off_spawnable_prev = Pointer.Read(reader);
                    Pointer off_spawnable_header = Pointer.Read(reader);
                    uint index = reader.ReadUInt32();
                    Pointer off_spawnable_perso = Pointer.Read(reader);
                    if (off_spawnable_perso != null) {
                        Pointer.Goto(ref reader, off_spawnable_perso);
                        Perso perso = Perso.Read(reader, off_spawnable_perso, null);
                        if (perso != null) {
                            perso.Gao.transform.parent = spawnableParent.transform;
                        }
                    }
                    if (off_spawnable_next != null) Pointer.Goto(ref reader, off_spawnable_next);
                }
                Pointer.Goto(ref reader, off_current);
            }
        }

        public void ReadFamilies(EndianBinaryReader reader) {
            if (globals.num_familiesTable_entries > 0) {
                families = new Family[globals.num_familiesTable_entries];
                GameObject familiesParent = new GameObject("Families");
                familiesParent.SetActive(false); // Families do not need to be visible
                Pointer off_current = Pointer.Goto(ref reader, globals.off_familiesTable_first);
                Pointer off_familiesTable_current = globals.off_familiesTable_first;
                for (uint i = 0; i < globals.num_familiesTable_entries; i++) {
                    families[i] = Family.Read(reader, off_familiesTable_current);
                    families[i].Gao.transform.SetParent(familiesParent.transform, false);
                    off_familiesTable_current = families[i].off_family_next;
                    if (off_familiesTable_current != null) Pointer.Goto(ref reader, off_familiesTable_current);
                }
                Pointer.Goto(ref reader, off_current);
            }
        }

        public bool AddGraph(Graph graph)
        {
            foreach (Graph existingGraph in graphs) {
                if (existingGraph.offset == graph.offset) {
                    return false;
                }
            }

            graphs.Add(graph);

            GameObject go_graph = new GameObject("Graph " + graph.offset.ToString());
            go_graph.transform.SetParent(graphRoot.transform);

            int nodeNum = 0;
            foreach (GraphNode node in graph.nodeList) {
                GameObject go_graphNode = new GameObject("GraphNode[" + nodeNum + "].WayPoint");
                go_graphNode.transform.position = new Vector3(node.wayPoint.position.x, node.wayPoint.position.z, node.wayPoint.position.y);
                WaypointSprite wpSprite = go_graphNode.AddComponent<WaypointSprite>();

                go_graphNode.transform.SetParent(go_graph.transform);
                nodeNum++;
            }

            return true;
        }
    }
}
