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

namespace OpenSpace {
    public class MapLoader {
        public string gameDataBinFolder;
        public string lvlName;

        public Material baseMaterial;
        public Material baseTransparentMaterial;
        public Material baseLightMaterial;
        public Material collideMaterial;
        public Material collideTransparentMaterial;
        public Material billboardMaterial;
        public Material billboardAdditiveMaterial;

        public bool allowDeadPointers = false;
        public bool forceDisplayBackfaces = false;
        public bool blockyMode = false;
        public enum Mode {
            Rayman3PC, Rayman3GC,
            RaymanArenaPC, RaymanArenaGC,
            Rayman2PC, Rayman2DC, Rayman2IOS,
            Rayman2PCDemo2, Rayman2PCDemo1,
            DonaldDuckPC,
            TonicTroublePC, TonicTroubleSEPC,
            PlaymobilHypePC, PlaymobilAlexPC, PlaymobilLauraPC
        };
        public Mode mode = Mode.Rayman3PC;

        public ObjectType[][] objectTypes;
        public TextureInfo[] textures;
        public TextureInfo overlightTexture;
        public TextureInfo lightmapTexture;
        public Pointer[] persoInFix;
        public AnimationBank[] animationBanks;
        public LinkedList<Family> families;

        public InputStructure inputStruct;
        public FontStructure fontStruct;
        public string[] levels;
        public string[] languages;
        public string[] languages_loc;

        uint off_textures_start_fix = 0;
        bool hasTransit;
        public bool HasTransit {
            get { return hasTransit; }
        }

        public SuperObject transitDynamicWorld;
        public SuperObject actualWorld;
        public SuperObject dynamicWorld;
        public SuperObject inactiveDynamicWorld;
        public SuperObject fatherSector;

        public List<SuperObject> superObjects = new List<SuperObject>();
        public List<VisualMaterial> visualMaterials = new List<VisualMaterial>();
        public List<GameMaterial> gameMaterials = new List<GameMaterial>();
        public List<CollideMaterial> collideMaterials = new List<CollideMaterial>();
        public List<LightInfo> lights = new List<LightInfo>();
        public List<Sector> sectors = new List<Sector>();
        public List<PhysicalObject> physicalObjects = new List<PhysicalObject>(); // only required for quick switching between visual & collision geometry
        public List<AIModel> aiModels = new List<AIModel>();
        public List<Behavior> behaviors = new List<Behavior>();
        public List<Perso> persos = new List<Perso>();
        public List<State> states = new List<State>();
        public List<Graph> graphs = new List<Graph>();
        public List<GraphNode> graphNodes = new List<GraphNode>();
        public List<WayPoint> isolateWaypoints = new List<WayPoint>();
        public List<KeypadEntry> keypadEntries = new List<KeypadEntry>();
        public List<MechanicsIDCard> mechanicsIDCards = new List<MechanicsIDCard>();
        public List<AnimationReference> animationReferences = new List<AnimationReference>();
        public List<AnimationMontreal> animationReferencesMontreal = new List<AnimationMontreal>();
        public List<ObjectList> objectLists = new List<ObjectList>();
        public List<ObjectList> uncategorizedObjectLists = new List<ObjectList>();
        public Dictionary<Pointer, string> strings = new Dictionary<Pointer, string>();
        public GameObject graphRoot = null;
        public GameObject isolateWaypointRoot = null;
        public GameObject familiesRoot = null;
        //List<R3GeometricObject> parsedGO = new List<R3GeometricObject>();

        public Dictionary<ushort, SNAMemoryBlock> relocation_global = new Dictionary<ushort, SNAMemoryBlock>();
        public FileWithPointers[] files_array = new FileWithPointers[7];


        string[] lvlNames = new string[7];
        string[] lvlPaths = new string[7];
        string[] ptrPaths = new string[7];
        string[] tplPaths = new string[7];
        string[] cntPaths = null;
        CNT cnt = null;
        DSB gameDsb = null;
        DSB lvlDsb = null;
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
                    case Mode.Rayman2DC: settings = Settings.R2DC; break;
                    case Mode.Rayman2PC: settings = Settings.R2PC; break;
                    case Mode.Rayman2PCDemo1: settings = Settings.R2PCDemo1; break;
                    case Mode.Rayman2PCDemo2: settings = Settings.R2PCDemo2; break;
                    case Mode.Rayman3GC: settings = Settings.R3GC; break;
                    case Mode.Rayman3PC: settings = Settings.R3PC; break;
                    case Mode.RaymanArenaGC: settings = Settings.RAGC; break;
                    case Mode.RaymanArenaPC: settings = Settings.RAPC; break;
                    case Mode.DonaldDuckPC: settings = Settings.DDPC; break;
                    case Mode.TonicTroublePC: settings = Settings.TTPC; break;
                    case Mode.TonicTroubleSEPC: settings = Settings.TTSEPC; break;
                    case Mode.PlaymobilHypePC: settings = Settings.PlaymobilHypePC; break;
                    case Mode.PlaymobilAlexPC: settings = Settings.PlaymobilAlexPC; break;
                    case Mode.PlaymobilLauraPC: settings = Settings.PlaymobilLauraPC; break;
                }
                Settings.s = settings;

                if (gameDataBinFolder == null || gameDataBinFolder.Trim().Equals("")) throw new Exception("GAMEDATABIN folder doesn't exist");
                if (lvlName == null || lvlName.Trim() == "") throw new Exception("No level name specified!");
                globals = new Globals();
                if (!Directory.Exists(gameDataBinFolder)) throw new Exception("GAMEDATABIN folder doesn't exist");
                gameDataBinFolder += "/";

                if (Settings.s.engineVersion < Settings.EngineVersion.R3 && Settings.s.platform != Settings.Platform.DC) {
                    if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                        gameDsb = new DSB("Game", gameDataBinFolder + "gamedsc.bin");
                    } else if (Settings.s.game == Settings.Game.TTSE) {
                        gameDsb = new DSB("Game", gameDataBinFolder + "GAME.DSC");
                    } else {
                        gameDsb = new DSB("Game", gameDataBinFolder + "Game.dsb");
                    }
                    gameDsb.Save(gameDataBinFolder + "Game_dsb.dmp");
                    gameDsb.ReadAllSections();
                    gameDsb.Dispose();
                }
                CreateCNT();

                if (lvlName.EndsWith(".exe")) {
                    if (!Settings.s.hasMemorySupport) throw new Exception("This game does not have memory support.");
                    Settings.s.loadFromMemory = true;
                    MemoryFile mem = new MemoryFile(lvlName);
                    files_array[0] = mem;
                    LoadMemory();
                } else {
                    if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                        if (Settings.s.platform == Settings.Platform.DC) {
                            // FIX
                            string fixDATPath = gameDataBinFolder + "FIX.DAT";
                            tplPaths[0] = gameDataBinFolder + "FIX.TEX";
                            DCDAT fixDAT = new DCDAT("fix", fixDATPath, 0);

                            // LEVEL
                            string lvlDATPath = gameDataBinFolder + lvlName + "/" + lvlName + ".DAT";
                            tplPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ".TEX";
                            DCDAT lvlDAT = new DCDAT(lvlName, lvlDATPath, 1);

                            files_array[0] = fixDAT;
                            files_array[1] = lvlDAT;

                            LoadDreamcast();

                            fixDAT.Dispose();
                            lvlDAT.Dispose();
                        } else {
                            hasTransit = false;
                            DAT dat = null;

                            string levelsFolder = gameDataBinFolder + gameDsb.levelsDataPath + "/";
                            string langDataPath = gameDataBinFolder + "../LangData/English/" + gameDsb.levelsDataPath + "/";
                            if (!Directory.Exists(langDataPath)) {
                                string langPath = gameDataBinFolder + "../LangData/";
                                if (Directory.Exists(langPath)) {
                                    DirectoryInfo dirInfo = new DirectoryInfo(langPath);
                                    DirectoryInfo firstLang = dirInfo.GetDirectories().FirstOrDefault();
                                    langDataPath = firstLang.FullName + "/" + gameDsb.levelsDataPath + "/";
                                }
                            }

                            if (mode == Mode.Rayman2PC || mode == Mode.DonaldDuckPC) {
                                string dataPath = levelsFolder + "LEVELS0.DAT";
                                if (File.Exists(dataPath)) {
                                    dat = new DAT("LEVELS0", gameDsb, dataPath);
                                }
                            }

                            // LEVEL DSB
                            string lvlDsbPath = levelsFolder + lvlName + "/" + lvlName + ".dsb";
                            if (Settings.s.engineVersion < Settings.EngineVersion.R2) {
                                lvlDsbPath = levelsFolder + lvlName + "/" + lvlName + ".DSC";
                            }
                            if (File.Exists(lvlDsbPath)) {
                                lvlDsb = new DSB(lvlName + ".dsc", lvlDsbPath);
                                lvlDsb.Save(levelsFolder + lvlName + "/" + lvlName + "_dsb.dmp");
                                //lvlDsb.ReadAllSections();
                                lvlDsb.Dispose();
                            }

                            // FIX
                            string fixSnaPath = levelsFolder + "fix.sna";
                            RelocationTable fixRtb = new RelocationTable(fixSnaPath, dat, "fix", RelocationType.RTB);
                            if (File.Exists(levelsFolder + lvlName + "/FixLvl.rtb")) {
                                // Fix -> Lvl pointers for Tonic Trouble
                                fixRtb.Add(new RelocationTable(levelsFolder + lvlName + "/FixLvl.rtb", dat, lvlName + "Fix", RelocationType.RTB));
                            }
                            SNA fixSna = new SNA("fix", fixSnaPath, fixRtb);
                            if (Directory.Exists(langDataPath)) {
                                string fixLangPath = langDataPath + "fix.lng";
                                RelocationTable fixLangRTG = new RelocationTable(fixLangPath, dat, "fixLang", RelocationType.RTG);
                                if (File.Exists(langDataPath + lvlName + "/FixLvl.rtg")) {
                                    fixLangRTG.Add(new RelocationTable(langDataPath + lvlName + "/FixLvl.rtg", dat, lvlName + "FixLang", RelocationType.RTG));
                                }
                                SNA fixLangSna = new SNA("fixLang", fixLangPath, fixLangRTG);
                                fixSna.AddSNA(fixLangSna);

                                string fixDlgPath = langDataPath + "fix.dlg";
                                RelocationTable fixRtd = new RelocationTable(fixDlgPath, dat, "fixLang", RelocationType.RTD);
                                fixSna.ReadDLG(fixDlgPath, fixRtd);
                            }
                            string fixGptPath = levelsFolder + "fix.gpt";
                            RelocationTable fixRtp = new RelocationTable(fixGptPath, dat, "fix", RelocationType.RTP);
                            fixSna.ReadGPT(fixGptPath, fixRtp);

                            string fixPtxPath = levelsFolder + "fix.ptx";
                            RelocationTable fixRtt = new RelocationTable(fixPtxPath, dat, "fix", RelocationType.RTT);
                            fixSna.ReadPTX(fixPtxPath, fixRtt);

                            if (File.Exists(levelsFolder + "fix.sda")) {
                                fixSna.ReadSDA(levelsFolder + "fix.sda");
                            }

                            // LEVEL
                            string lvlSnaPath = levelsFolder + lvlName + "/" + lvlName + ".sna";
                            RelocationTable lvlRtb = new RelocationTable(lvlSnaPath, dat, lvlName, RelocationType.RTB);
                            SNA lvlSna = new SNA(lvlName, lvlSnaPath, lvlRtb);
                            if (Directory.Exists(langDataPath)) {
                                string lvlLangPath = langDataPath + lvlName + "/" + lvlName + ".lng";
                                RelocationTable lvlLangRTG = new RelocationTable(lvlLangPath, dat, lvlName + "Lang", RelocationType.RTG);
                                SNA lvlLangSna = new SNA(lvlName + "Lang", lvlLangPath, lvlLangRTG);
                                lvlSna.AddSNA(lvlLangSna);

                                string lvlDlgPath = langDataPath + lvlName + "/" + lvlName + ".dlg";
                                RelocationTable lvlRtd = new RelocationTable(lvlDlgPath, dat, lvlName + "Lang", RelocationType.RTD);
                                lvlSna.ReadDLG(lvlDlgPath, lvlRtd);
                            }

                            string lvlGptPath = levelsFolder + lvlName + "/" + lvlName + ".gpt";
                            string lvlPtxPath = levelsFolder + lvlName + "/" + lvlName + ".ptx";
                            if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
                                RelocationTable lvlRtp = new RelocationTable(lvlGptPath, dat, lvlName, RelocationType.RTP);
                                lvlSna.ReadGPT(lvlGptPath, lvlRtp);
                                RelocationTable lvlRtt = new RelocationTable(lvlPtxPath, dat, lvlName, RelocationType.RTT);
                                lvlSna.ReadPTX(lvlPtxPath, lvlRtt);
                            } else {
                                lvlSna.ReadGPT(lvlGptPath, null);
                                lvlSna.ReadPTX(lvlPtxPath, null);
                            }
                            if (File.Exists(levelsFolder + lvlName + "/" + lvlName + ".sda")) {
                                lvlSna.ReadSDA(levelsFolder + lvlName + "/" + lvlName + ".sda");
                            }

                            fixSna.CreatePointers();
                            lvlSna.CreatePointers();

                            files_array[0] = fixSna;
                            files_array[1] = lvlSna;
                            files_array[2] = dat;

                            fixSna.CreateMemoryDump(levelsFolder + "fix.dmp", true);
                            lvlSna.CreateMemoryDump(levelsFolder + lvlName + "/" + lvlName + ".dmp", true);

                            LoadFIXSNA();
                            LoadLVLSNA();

                            fixSna.Dispose();
                            lvlSna.Dispose();
                            if (dat != null) dat.Dispose();
                        }
                    } else if (Settings.s.engineVersion == Settings.EngineVersion.R3) {

                        menuTPLPath = gameDataBinFolder + "menu.tpl";
                        lvlNames[0] = "fix";
                        lvlPaths[0] = gameDataBinFolder + "fix.lvl";
                        ptrPaths[0] = gameDataBinFolder + "fix.ptr";
                        tplPaths[0] = gameDataBinFolder + ((mode == Mode.RaymanArenaGC) ? "../common.tpl" : "fix.tpl");

                        lvlNames[1] = lvlName;
                        lvlPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ".lvl";
                        ptrPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ".ptr";
                        tplPaths[1] = gameDataBinFolder + lvlName + "/" + lvlName + ((mode == Mode.RaymanArenaGC) ? ".tpl" : "_Lvl.tpl");

                        lvlNames[2] = "transit";
                        lvlPaths[2] = gameDataBinFolder + lvlName + "/transit.lvl";
                        ptrPaths[2] = gameDataBinFolder + lvlName + "/transit.ptr";
                        tplPaths[2] = gameDataBinFolder + lvlName + "/" + lvlName + "_Trans.tpl";
                        hasTransit = File.Exists(lvlPaths[2]) && (new FileInfo(lvlPaths[2]).Length > 4);

                        lvlNames[4] = lvlName + "_vb";
                        lvlPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName + "_vb.lvl";
                        ptrPaths[4] = gameDataBinFolder + lvlName + "/" + lvlName + "_vb.ptr";

                        lvlNames[5] = "fixkf";
                        lvlPaths[5] = gameDataBinFolder + "fixkf.lvl";
                        ptrPaths[5] = gameDataBinFolder + "fixkf.ptr";

                        lvlNames[6] = lvlName + "kf";
                        lvlPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName + "kf.lvl";
                        ptrPaths[6] = gameDataBinFolder + lvlName + "/" + lvlName + "kf.ptr";

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
                }
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
            Writer writer = null;
            for (int i = 0; i < files_array.Length; i++) {
                if (files_array[i] != null && files_array[i].writer != null) {
                    writer = files_array[i].writer;
                    break;
                }
            }
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

            foreach (Family family in families) {
                GameObject gao = family.Gao;
                if (gao != null) {
                    FamilyComponent fc = gao.GetComponent<FamilyComponent>();
                    if (fc != null) {
                        fc.SaveChanges(writer);
                    }
                }

                foreach (LightInfo light in lights) {
                    light.Write(writer);
                }
            }
        }

        public void Save() {
            try {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) files_array[i].CreateWriter();
                }
                // Save changes
                SaveModdables();
            } catch (Exception e) {
                Debug.LogError(e.ToString());
            } finally {
                for (int i = 0; i < files_array.Length; i++) {
                    if (files_array[i] != null) {
                        files_array[i].Dispose();
                    }
                }
            }
        }

        #region FIX
        void LoadFIX() {
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
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
            Pointer off_identityMatrix = Pointer.Read(reader);
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
            Pointer.DoAt(ref reader, off_languages, () => {
                languages = new string[num_languages];
                languages_loc = new string[num_languages];
                for (uint i = 0; i < num_languages; i++) {
                    languages[i] = reader.ReadString(0x14);
                    languages_loc[i] = reader.ReadString(0x14);
                }
            });
            uint num_textures = reader.ReadUInt32();
            print("Texture count fix: " + num_textures);
            textures = new TextureInfo[num_textures];
            if (num_textures > 0) {
                for (uint i = 0; i < num_textures; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    Pointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                if (mode == Mode.Rayman3GC || mode == Mode.RaymanArenaGC) {
                    uint num_textures_menu = reader.ReadUInt32();
                    TPL fixTPL = new TPL(tplPaths[Mem.Fix]);
                    TPL menuTPL = new TPL(menuTPLPath);
                    for (uint i = 0; i < num_textures_menu; i++) {
                        Pointer off_texture = Pointer.Read(reader);
                        TextureInfo tex = textures.Where(t => t.offset == off_texture).First();
                        tex.Texture = menuTPL.textures[i];
                    }
                    for (int i = 0, j = 0; i < fixTPL.Count; i++, j++) {
                        while (textures[j].Texture != null) j++;
                        textures[j].Texture = fixTPL.textures[i];
                    }
                } else if (mode == Mode.Rayman3PC || mode == Mode.RaymanArenaPC) {
                    for (int i = 0; i < num_textures; i++) {
                        GF gf = cnt.GetGFByTGAName(textures[i].name);
                        if (gf != null) {
                            textures[i].Texture = gf.GetTexture();
                        }
                    }
                }
                for (uint i = 0; i < num_textures; i++) {
                    reader.ReadUInt32(); // 0 or 8.
                }
            }
            // Defaults for Rayman 3 PC. Sizes are hardcoded in the exes and might differ for versions too :/
            int sz_entryActions = 0x100;
            int sz_randomStructure = 0xDC;
            int sz_fontDefine = 0x12B2;
            int sz_videoStructure = 0x18;
            int sz_musicMarkerSlot = 0x28;
            int sz_binDataForMenu = 0x020C;

            if (mode == Mode.Rayman3GC) {
                sz_entryActions = 0xE8;
                sz_binDataForMenu = 0x01F0;
                sz_fontDefine = 0x12E4;
            } else if (mode == Mode.RaymanArenaGC) {
                sz_entryActions = 0xC4;
                sz_fontDefine = 0x12E4;
            } else if (mode == Mode.RaymanArenaPC) {
                sz_entryActions = 0xDC;
            }
            inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
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
            Pointer.DoAt(ref reader, off_animBankFix, () => {
                animationBanks[0] = AnimationBank.Read(reader, off_animBankFix, 0, 1, files_array[Mem.FixKeyFrames])[0];
            });
        }
        #endregion

        #region LVL
        void LoadLVL() {
            files_array[Mem.Lvl].GotoHeader();
            Reader reader = files_array[Mem.Lvl].reader;
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
                        textures[num_textures_fix + i].Texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        textures[num_textures_fix + i].Texture = lvlTPL.textures[i - transitTexturesSeen];
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
                        if (gf != null) textures[num_textures_fix + i].Texture = gf.GetTexture();
                        transitTexturesSeen++;
                        //textures[num_textures_fix + i].texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        GF gf = cnt.GetGFByTGAName(textures[num_textures_fix + i].name);
                        if (gf != null) textures[num_textures_fix + i].Texture = gf.GetTexture();
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
            Pointer.DoAt(ref reader, off_array_visual_materials, () => {
                for (uint i = 0; i < num_visual_materials; i++) {
                    Pointer off_material = Pointer.Read(reader);
                    Pointer.DoAt(ref reader, off_material, () => {
                        visualMaterials.Add(VisualMaterial.Read(reader, off_material));
                    });
                }
            });

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
            for (int i = 0; i < families.Count; i++) {
                if (families[i] != null && families[i].animBank > 4 && objectTypes[0][families[i].family_index].id != 0xFF) {
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


            ReadCrossReferences(reader);
        }
        #endregion

        #region FIXSNA
        void LoadFIXSNA() {
            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            Pointer off_current = Pointer.Current(reader);
            print("FIX GPT offset: " + off_current);
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
                for (int i = 0; i < Settings.s.numEntryActions; i++) {
                    Pointer.Read(reader); // 3DOS_EntryActions
                }

                Pointer off_IPT_keyAndPadDefine = Pointer.Read(reader);
                ReadKeypadDefine(reader, off_IPT_keyAndPadDefine);

                inputStruct = InputStructure.Read(reader, Pointer.Current(reader));
                print("Num entractions: " + inputStruct.num_entryActions);
                print("Off entryactions: " + inputStruct.off_entryActions);
                Pointer off_IPT_entryElementList = Pointer.Read(reader);
                print("Off entryelements: " + off_IPT_entryElementList);
                fontStruct = FontStructure.Read(reader, Pointer.Current(reader)); // FON_g_stGeneral
                off_current = Pointer.Current(reader);
                animationBanks = new AnimationBank[2]; // 1 in fix, 1 in lvl
                animationBanks[0] = AnimationBank.Read(reader, off_current, 0, 1, files_array[Mem.FixKeyFrames])[0];
                print("Fix animation bank: " + off_current);
                Pointer off_fixInfo = Pointer.Read(reader);
            }

            // Read PTX
            Pointer.DoAt(ref reader, sna.PTX, () => {
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
                            string texturePath = Path.ChangeExtension(gameDataBinFolder + "world/graphics/textures/" + textures[i].name, ".gf");
                            if (File.Exists(texturePath)) {
                                GF gf = new GF(texturePath);
                                if (gf != null) textures[i].Texture = gf.GetTexture();
                            }
                        } else {
                            //print(textures[i].name);
                            GF gf = cnt.GetGFByTGAName(textures[i].name);
                            if (gf != null) textures[i].Texture = gf.GetTexture();
                        }
                    }
                }
                /*uint num_texturesToCreate = reader.ReadUInt32();
                for (uint i = 0; i < num_texturesToCreate; i++) {
                    reader.ReadUInt32(); //1
                }
                uint currentMemoryChannel = reader.ReadUInt32();*/
            });
        }
        #endregion

        #region LVLSNA
        void LoadLVLSNA() {
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
            globals.off_spawnable_perso_first = Pointer.Read(reader);
            globals.off_spawnable_perso_last = Pointer.Read(reader);
            globals.num_spawnable_perso = reader.ReadUInt32();
            globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
                globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks
            } else {
                reader.ReadUInt32(); // 0x6F. In Montreal version this is a pointer to a pointer table for always
                FillLinkedListPointers(reader, globals.off_spawnable_perso_last, Pointer.Current(reader));
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

                levels = new string[80];
                for (int i = 0; i < 80; i++) {
                    levels[i] = reader.ReadString(0x1E);
                }
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
                off_current = Pointer.Current(reader);
                AnimationBank.Read(reader, off_current, 0, 1, files_array[Mem.LvlKeyFrames], append: true);
                animationBanks[1] = animationBanks[0];
            }
            string levelsFolder = gameDataBinFolder + gameDsb.levelsDataPath + "/";
            ((SNA)files_array[0]).CreateMemoryDump(levelsFolder + "fix.dmp", true);
            ((SNA)files_array[1]).CreateMemoryDump(levelsFolder + lvlName + "/" + lvlName + ".dmp", true);

            // Read PTX
            Pointer.DoAt(ref reader, sna.PTX, () => {
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
                            string texturePath = Path.ChangeExtension(gameDataBinFolder + "world/graphics/textures/" + textures[i].name, ".gf");
                            if (File.Exists(texturePath)) {
                                GF gf = new GF(texturePath);
                                if (gf != null) textures[i].Texture = gf.GetTexture();
                            }
                        } else {
                            GF gf = cnt.GetGFByTGAName(textures[i].name);
                            if (gf != null) textures[i].Texture = gf.GetTexture();
                        }
                    }
                }
            });

            // Read background game material (DD only)
            globals.backgroundGameMaterial = GameMaterial.FromOffsetOrRead(globals.off_backgroundGameMaterial, reader);

            // Parse actual world & always structure
            ReadFamilies(reader);

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

            ReadSuperObjects(reader);
            ReadAlways(reader);
            ReadCrossReferences(reader);
        }
        #endregion

        #region Memory
        public void LoadMemory() {
            MemoryFile mem = (MemoryFile)files_array[0];
            if (mem == null || mem.reader == null) throw new NullReferenceException("File not initialized!");
            Reader reader = mem.reader;

            // Read object names
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["objectTypes"], mem));
            objectTypes = new ObjectType[3][];
            for (uint i = 0; i < 3; i++) {
                Pointer off_names_header = Pointer.Current(reader);
                Pointer off_names_first = Pointer.Read(reader);
                Pointer off_names_last = Pointer.Read(reader);
                uint num_names = reader.ReadUInt32();

                ReadObjectNamesTable(reader, off_names_first, num_names, i);
            }

            // Read globals
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["actualWorld"], mem));
            globals.off_actualWorld = Pointer.Read(reader);
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["dynamicWorld"], mem));
            globals.off_dynamicWorld = Pointer.Read(reader);
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["inactiveDynamicWorld"], mem));
            globals.off_inactiveDynamicWorld = Pointer.Read(reader);
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["fatherSector"], mem));
            globals.off_fatherSector = Pointer.Read(reader);
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["firstSubmapPosition"], mem));
            globals.off_firstSubMapPosition = Pointer.Read(reader);
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["always"], mem));
            globals.num_always = reader.ReadUInt32();
            globals.off_spawnable_perso_first = Pointer.Read(reader);
            globals.off_spawnable_perso_last = Pointer.Read(reader);
            globals.num_spawnable_perso = reader.ReadUInt32();
            globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            globals.off_always_reusableUnknown1 = Pointer.Read(reader); // (num_always) * 0x2c blocks
            globals.off_always_reusableUnknown2 = Pointer.Read(reader); // (num_always) * 0x4 blocks
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["families"], mem));
            families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);

            animationBanks = new AnimationBank[2];

            // Read animations
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_stacks"], mem));
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                animationBanks[0] = AnimationBank.Read(reader, Pointer.Current(reader), 0, 1, null)[0];
                animationBanks[1] = animationBanks[0];
            } else {
                animationBanks = AnimationBank.Read(reader, Pointer.Current(reader), 0, 5, null);
            }

            // Read textures
            uint[] texMemoryChannels = new uint[1024];
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["textureMemoryChannels"], mem));
            for (int i = 0; i < 1024; i++) {
                texMemoryChannels[i] = reader.ReadUInt32();
            }
            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["textures"], mem));
            List<TextureInfo> textureInfos = new List<TextureInfo>();
            for (int i = 0; i < 1024; i++) {
                Pointer off_texture = Pointer.Read(reader);
                if (off_texture != null && texMemoryChannels[i] != 0xC0DE0005) {
                    Pointer off_current = Pointer.Goto(ref reader, off_texture);
                    TextureInfo texInfo = TextureInfo.Read(reader, off_texture);
                    //texInfo.ReadTextureFromData(reader); // Reading from GL memory doesn't seem to be possible sadly
                    // texInfo.Texture = Util.CreateDummyTexture();
                    GF gf = cnt.GetGFByTGAName(texInfo.name);
                    texInfo.Texture = gf != null ? gf.GetTexture() : null;
                    textureInfos.Add(texInfo);
                    Pointer.Goto(ref reader, off_current);
                }
            }
            textures = textureInfos.ToArray();
            
            // Parse materials list
            if (Settings.s.memoryAddresses.ContainsKey("visualMaterials") && Settings.s.memoryAddresses.ContainsKey("num_visualMaterials")) {
                Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["num_visualMaterials"], mem));
                uint num_visual_materials = reader.ReadUInt32();
                Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["visualMaterials"], mem));
                Pointer off_visualMaterials = Pointer.Read(reader);
                if (off_visualMaterials != null) {
                    Pointer.Goto(ref reader, off_visualMaterials);
                    for (uint i = 0; i < num_visual_materials; i++) {
                        Pointer off_material = Pointer.Read(reader);
                        Pointer off_current_mat = Pointer.Goto(ref reader, off_material);
                        visualMaterials.Add(VisualMaterial.Read(reader, off_material));
                        Pointer.Goto(ref reader, off_current_mat);
                    }
                }
            }

            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["inputStructure"], mem));
            inputStruct = InputStructure.Read(reader, Pointer.Current(reader));

            Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["fontStructure"], mem));
            fontStruct = FontStructure.Read(reader, Pointer.Current(reader));

            // Parse actual world & always structure
            ReadFamilies(reader);
            ReadSuperObjects(reader);
            ReadAlways(reader);
            ReadCrossReferences(reader);
        }
        #endregion

        #region Dreamcast
        public void LoadDreamcast() {
            textures = new TextureInfo[0];

            files_array[Mem.Fix].GotoHeader();
            Reader reader = files_array[Mem.Fix].reader;
            Pointer off_base_fix = Pointer.Current(reader);
            uint base_language = reader.ReadUInt32(); //Pointer off_language = Pointer.Read(reader);
            reader.ReadUInt32();
            uint num_text_language = reader.ReadUInt32();
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32(); // base
            Pointer off_text_general = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_text_general, () => {
                fontStruct = FontStructure.Read(reader, off_text_general);
            });
            Pointer off_inputStructure = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_inputStructure, () => {
                inputStruct = InputStructure.Read(reader, off_inputStructure);
            });
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            Pointer.Read(reader);
            Pointer off_levelNames = Pointer.Read(reader);
            Pointer off_languages = Pointer.Read(reader);
            uint num_levelNames = reader.ReadUInt32();
            uint num_languages = reader.ReadUInt32();
            reader.ReadUInt32(); // same as num_levelNames
            Pointer.DoAt(ref reader, off_levelNames, () => {
                lvlNames = new string[num_levelNames];
                for (uint i = 0; i < num_levelNames; i++) {
                    lvlNames[i] = reader.ReadString(0x1E);
                }
            });
            Pointer.DoAt(ref reader, off_languages, () => {
                languages = new string[num_languages];
                languages_loc = new string[num_languages];
                for (uint i = 0; i < num_languages; i++) {
                    languages[i] = reader.ReadString(0x14);
                    languages_loc[i] = reader.ReadString(0x14);
                }
            });
            if (languages != null && fontStruct != null) {
                for (int i = 0; i < num_languages; i++) {
                    string langFilePath = gameDataBinFolder + "/TEXTS/" + languages[i].ToUpper() + ".LNG";
                    files_array[2] = new DCDAT(languages[i], langFilePath, 2);
                    ((DCDAT)files_array[2]).SetHeaderOffset(base_language);
                    files_array[2].GotoHeader();
                    fontStruct.ReadLanguageTableDreamcast(files_array[2].reader, i, (ushort)num_text_language);
                    files_array[2].Dispose();
                }
            }
            Pointer off_events_fix = Pointer.Read(reader);
            uint num_events_fix = reader.ReadUInt32();
            uint num_textures_fix = reader.ReadUInt32();
            Pointer off_textures_fix = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_textures_fix, () => {
                Array.Resize(ref textures, (int)num_textures_fix);
                for (uint i = 0; i < num_textures_fix; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    textures[i] = null;
                    Pointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(tplPaths[0]);
                for (uint i = 0; i < num_textures_fix; i++) {
                    if (textures[i] != null && tex.Count > i) {
                        textures[i].Texture = tex.textures[i];
                    }
                }
            });


            files_array[Mem.Lvl].GotoHeader();
            reader = files_array[Mem.Lvl].reader;

            // Animation stuff
            Pointer off_animationBank = Pointer.Current(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            Pointer.Read(reader);
            reader.ReadUInt32();
            Pointer.Read(reader);

            // Globals
            globals.off_actualWorld = Pointer.Read(reader);
            globals.off_dynamicWorld = Pointer.Read(reader);
            globals.off_inactiveDynamicWorld = Pointer.Read(reader);
            globals.off_fatherSector = Pointer.Read(reader);
            reader.ReadUInt32();
            Pointer off_always = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_always, () => {
                globals.num_always = reader.ReadUInt32();
                globals.off_spawnable_perso_first = Pointer.Read(reader);
                globals.off_spawnable_perso_last = Pointer.Read(reader);
                globals.num_spawnable_perso = reader.ReadUInt32();
                FillLinkedListPointers(reader, globals.off_spawnable_perso_last, off_always + 4);
                globals.off_always_reusableSO = Pointer.Read(reader); // There are (num_always) empty SuperObjects starting with this one.
            });
            Pointer.Read(reader);
            Pointer off_objectTypes = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_objectTypes, () => {
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
            });
            Pointer.Read(reader);
            Pointer off_mainChar = Pointer.Read(reader);
            reader.ReadUInt32();
            uint num_persoInFixPointers = reader.ReadUInt32();
            Pointer off_persoInFixPointers = Pointer.Read(reader);

            //Pointer[] persoInFixPointers = new Pointer[num_persoInFixPointers];
            Pointer.DoAt(ref reader, off_persoInFixPointers, () => {
                for (int i = 0; i < num_persoInFixPointers; i++) {
                    Pointer off_perso = Pointer.Read(reader);
                    Pointer off_so = Pointer.Read(reader);
                    byte[] unk = reader.ReadBytes(4);
                    Pointer off_matrix = Pointer.Current(reader); // It's better to change the pointer instead of the data as that is reused in some places
                    byte[] matrixData = reader.ReadBytes(0x68);
                    byte[] soFlags = reader.ReadBytes(4);
                    byte[] brothersAndParent = reader.ReadBytes(12);

                    Pointer.DoAt(ref reader, off_perso, () => {
                        reader.ReadUInt32();
                        Pointer off_stdGame = Pointer.Read(reader);
                        if (off_stdGame != null && off_so != null) {
                            ((DCDAT)off_stdGame.file).OverwriteData(off_stdGame.FileOffset + 0xC, off_so.offset);
                        }
                    });
                    if (off_so != null) {
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x14, brothersAndParent);
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x20, off_matrix.offset);
                        ((DCDAT)off_so.file).OverwriteData(off_so.FileOffset + 0x30, soFlags);
                    }
                }

                /*if (off_perso != null) {
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
                        ((SNA)off_p3dData.file).OverwriteData(off_p3dData.offset + 0x18, matrixData);

                        if (Settings.s.engineVersion > Settings.EngineVersion.TT) {
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
                }*/
            });
            Pointer.Read(reader); // contains a pointer to the camera SO
            Pointer off_cameras = Pointer.Read(reader); // Double linkedlist of cameras
            Pointer off_families = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_families, () => {
                families = LinkedList<Family>.ReadHeader(reader, Pointer.Current(reader), type: LinkedList.Type.Double);
                families.FillPointers(reader, families.off_tail, families.off_head);
            });
            Pointer.Read(reader); // At this pointer: a double linkedlist of fix perso's with headers (soptr, next, prev, hdr)
            Pointer.Read(reader); // Rayman
            reader.ReadUInt32();
            Pointer.Read(reader); // Camera
            reader.ReadUInt32();
            reader.ReadUInt32();
            uint num_textures_lvl = reader.ReadUInt32();
            uint num_textures_total = num_textures_fix + num_textures_lvl;
            Pointer off_textures_lvl = Pointer.Read(reader);
            Pointer.DoAt(ref reader, off_textures_lvl, () => {
                Array.Resize(ref textures, (int)num_textures_total);
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    textures[i] = null;
                    Pointer.DoAt(ref reader, off_texture, () => {
                        textures[i] = TextureInfo.Read(reader, off_texture);
                    });
                }
                TEX tex = new TEX(tplPaths[1]);
                for (uint i = 0; i < num_textures_lvl; i++) {
                    if (textures[num_textures_fix + i] != null && tex.Count > i) {
                        textures[num_textures_fix + i].Texture = tex.textures[i];
                    }
                }
            });

            ReadFamilies(reader);

            Pointer.DoAt(ref reader, off_animationBank, () => {
                animationBanks = new AnimationBank[2];
                animationBanks[0] = AnimationBank.ReadDreamcast(reader, off_animationBank, off_events_fix, num_events_fix);
                animationBanks[1] = animationBanks[0];
            });
            
            ReadSuperObjects(reader);
            ReadAlways(reader);
            ReadCrossReferences(reader);

            // Parse transformation matrices and other settings for fix characters
            /*if (off_mainChar != null && off_matrix_mainChar != null) {
                SuperObject so = SuperObject.FromOffset(off_mainChar);
                Pointer.DoAt(ref reader, off_matrix_mainChar, () => {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    Pointer off_matrix = Pointer.Current(reader);
                    Matrix mat = Matrix.Read(reader, off_matrix);
                    if (so != null) {
                        so.off_matrix = off_matrix;
                        so.matrix = mat;
                        if (so.Gao != null) {
                            so.Gao.transform.localPosition = mat.GetPosition(convertAxes: true);
                            so.Gao.transform.localRotation = mat.GetRotation(convertAxes: true);
                            so.Gao.transform.localScale = mat.GetScale(convertAxes: true);
                        }
                    }
                });
            }*/
        }
        #endregion

        // Defining it this way, clicking the print will go straight to the code you want
        public Action<object> print = MonoBehaviour.print;

        /*public void print(string str) {
MonoBehaviour.print(str);
}*/

        public FileWithPointers GetFileByReader(Reader reader) {
            for (int i = 0; i < files_array.Length; i++) {
                FileWithPointers file = files_array[i];
                if (file != null && reader.Equals(file.reader)) {
                    return file;
                }
            }
            return null;
        }

        public FileWithPointers GetFileByWriter(Writer writer) {
            for (int i = 0; i < files_array.Length; i++) {
                FileWithPointers file = files_array[i];
                if (file != null && writer.Equals(file.writer)) {
                    return file;
                }
            }
            return null;
        }

        public FileWithPointers InitExtraLVL(string relativePath, int id) {
            string path = gameDataBinFolder + relativePath;
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

        public void CreateCNT() {
            if (Settings.s.engineVersion < Settings.EngineVersion.R3 && Settings.s.platform != Settings.Platform.DC) {
                List<string> cntPaths = new List<string>();
                if (gameDsb.bigfileTextures != null) cntPaths.Add(gameDataBinFolder + gameDsb.bigfileTextures);
                if (gameDsb.bigfileVignettes != null) cntPaths.Add(gameDataBinFolder + gameDsb.bigfileVignettes);
                if (cntPaths.Count > 0) {
                    cnt = new CNT(cntPaths.ToArray());
                }
            } else {
                if (mode == Mode.Rayman3PC) {
                    cntPaths = new string[3];
                    cntPaths[0] = gameDataBinFolder + "vignette.cnt";
                    cntPaths[1] = gameDataBinFolder + "tex32_1.cnt";
                    cntPaths[2] = gameDataBinFolder + "tex32_2.cnt";
                    cnt = new CNT(cntPaths);
                } else if (mode == Mode.RaymanArenaPC) {
                    cntPaths = new string[2];
                    cntPaths[0] = gameDataBinFolder + "vignette.cnt";
                    cntPaths[1] = gameDataBinFolder + "tex32.cnt";
                    cnt = new CNT(cntPaths);
                }
            }
        }

        public void FillLinkedListPointers(Reader reader, Pointer lastEntry, Pointer header, uint nextOffset = 0, uint prevOffset = 4, uint headerOffset = 8) {
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

        public void ReadObjectNamesTable(Reader reader, Pointer off_names_first, uint num_names, uint index) {
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

        public void ReadKeypadDefine(Reader reader, Pointer off_keypadDefine) {
            if (off_keypadDefine == null) return;
            //print("off keypad: " + off_keypadDefine);
            Pointer off_current = Pointer.Goto(ref reader, off_keypadDefine);
            bool readKeypadDefine = true;
            while (readKeypadDefine) {
                KeypadEntry entry = new KeypadEntry();
                entry.keycode = reader.ReadInt16();
                if (entry.keycode != -1) {
                    entry.unk2 = reader.ReadInt16();
                    /* Interestingly, some pointers in this list are not in the relocation table.
                     * and don't point to any key name, so they can't be read with Pointer.Read.
                     * Perhaps restoring this can help to restore debug functions... */
                    Pointer off_name = Pointer.GetPointerAtOffset(Pointer.Current(reader));
                    reader.ReadUInt32();
                    Pointer off_name2 = Pointer.GetPointerAtOffset(Pointer.Current(reader));
                    reader.ReadUInt32();
                    Pointer off_current_entry = Pointer.Current(reader);
                    if (off_name != null) {
                        Pointer.Goto(ref reader, off_name);
                        entry.name = reader.ReadNullDelimitedString();
                        //print(entry.name + " - " + entry.keycode + " - " + entry.unk2);
                    }
                    if (off_name2 != null) {
                        Pointer.Goto(ref reader, off_name2);
                        entry.name2 = reader.ReadNullDelimitedString();
                    }
                    Pointer.Goto(ref reader, off_current_entry);
                    keypadEntries.Add(entry);
                } else readKeypadDefine = false;
            }
            Pointer.Goto(ref reader, off_current);
        }

        public void ReadSuperObjects(Reader reader) {
            actualWorld = SuperObject.FromOffsetOrRead(globals.off_actualWorld, reader);
            dynamicWorld = SuperObject.FromOffsetOrRead(globals.off_dynamicWorld, reader);
            inactiveDynamicWorld = SuperObject.FromOffsetOrRead(globals.off_inactiveDynamicWorld, reader);
            transitDynamicWorld = SuperObject.FromOffsetOrRead(globals.off_transitDynamicWorld, reader);
            fatherSector = SuperObject.FromOffsetOrRead(globals.off_fatherSector, reader);

            if (actualWorld != null) actualWorld.Gao.name = "Actual World";
            if (dynamicWorld != null) dynamicWorld.Gao.name = "Dynamic World";
            if (inactiveDynamicWorld != null) inactiveDynamicWorld.Gao.name = "Inactive Dynamic World";
            if (transitDynamicWorld != null) transitDynamicWorld.Gao.name = "Transit Dynamic World (perso in fix)";
            if (fatherSector != null) fatherSector.Gao.name = "Father Sector";
        }

        public void ReadAlways(Reader reader) {
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

        public void ReadFamilies(Reader reader) {
            if (families.Count > 0) {
                familiesRoot = new GameObject("Families");
                familiesRoot.SetActive(false); // Families do not need to be visible
                families.ReadEntries(ref reader, (off_element) => {
                    Family f = Family.Read(reader, off_element);
                    f.Gao.transform.SetParent(familiesRoot.transform, false);
                    return f;
                }, LinkedList.Flags.HasHeaderPointers);
            }
        }

        public void ReadCrossReferences(Reader reader) {
            for (int i = 0; i < sectors.Count; i++) {
                sectors[i].ProcessPointers(reader);
            }
        }

        public bool AddIsolateWaypoint(WayPoint wayPoint)
        {
            if (isolateWaypointRoot == null) {
                isolateWaypointRoot = new GameObject("Isolate WayPoints");
                isolateWaypointRoot.SetActive(false);
            }

            foreach (WayPoint existingWaypoint in isolateWaypoints) {
                if (existingWaypoint.offset == wayPoint.offset) {
                    return false;
                }
            }

            isolateWaypoints.Add(wayPoint);
            GameObject wayPointGao = new GameObject("Isolate WayPoint");
            wayPointGao.transform.position = new Vector3(wayPoint.position.x, wayPoint.position.z, wayPoint.position.y);
            WaypointSprite wpSprite = wayPointGao.AddComponent<WaypointSprite>();

            wayPointGao.transform.SetParent(isolateWaypointRoot.transform);

            return true;
        }

        public void AddUncategorizedObjectList(ObjectList objList) {
            if (!uncategorizedObjectLists.Contains(objList)) uncategorizedObjectLists.Add(objList);
            objList.Gao.transform.SetParent(familiesRoot.transform);
        }

        public bool AddGraph(Graph graph) {
            if (graphRoot == null) {
                graphRoot = new GameObject("Graphs");
                graphRoot.SetActive(false);
            }
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

                var arcList = node.arcList.list;
                if (arcList.Count>0) {
                    foreach(var arc in arcList) {
                        Vector3 to = arc.graphNode.wayPoint.position;

                        WaypointLine wpLine = go_graphNode.AddComponent<WaypointLine>();

                        wpLine.from = new Vector3(node.wayPoint.position.x, node.wayPoint.position.z, node.wayPoint.position.y);
                        wpLine.to = new Vector3(to.x, to.z, to.y);

                        wpLine.weight = arc.weight;
                        wpLine.capabilities = arc.capabilities;
                    }
                }

                go_graphNode.transform.SetParent(go_graph.transform);
                nodeNum++;
            }

            return true;
        }
    }
}
