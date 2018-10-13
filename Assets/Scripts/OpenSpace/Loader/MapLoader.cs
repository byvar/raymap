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
using OpenSpace.Loader;

namespace OpenSpace {
    public class MapLoader {
        public string loadingState = "Loading";
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

        protected uint off_textures_start_fix = 0;
        protected bool hasTransit;
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


        protected string[] lvlNames = new string[7];
        protected string[] lvlPaths = new string[7];
        protected string[] ptrPaths = new string[7];
        protected string[] tplPaths = new string[7];
        protected string[] cntPaths = null;
        protected CNT cnt = null;
        protected DSB gameDsb = null;
        protected DSB lvlDsb = null;
        protected string menuTPLPath;

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

        
        private static MapLoader loader = null;
        public static MapLoader Loader {
            get {
                if (loader == null) {
                    if (Settings.s == null) return null;
                    if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                        if (Settings.s.platform == Settings.Platform.DC) {
                            loader = new R2DCLoader();
                        } else {
                            loader = new R2Loader();
                        }
                    } else {
                        loader = new R3Loader();
                    }
                    //loader = new MapLoader();
                }
                return loader;
            }
        }

        public MapLoader() {
        }
        
        public virtual IEnumerator Load() {
            yield return null;
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

        // Necessary for running StartCoroutine
        public Controller controller;
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
            string lvlName = relativePath;
            string lvlPath = path + ".lvl";
            string ptrPath = path + ".ptr";
            if (FileSystem.FileExists(lvlPath)) {
                Array.Resize(ref files_array, files_array.Length + 1);
                LVL lvl = new LVL(lvlName, lvlPath, id);
                files_array[files_array.Length - 1] = lvl;
                if (FileSystem.FileExists(ptrPath)) {
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
                if (Settings.s.platform == Settings.Platform.PC) {
                    if (Settings.s.game == Settings.Game.R3) {
                        cntPaths = new string[3];
                        cntPaths[0] = gameDataBinFolder + "vignette.cnt";
                        cntPaths[1] = gameDataBinFolder + "tex32_1.cnt";
                        cntPaths[2] = gameDataBinFolder + "tex32_2.cnt";
                        cnt = new CNT(cntPaths);
                    } else if (Settings.s.game == Settings.Game.RA) {
                        cntPaths = new string[2];
                        cntPaths[0] = gameDataBinFolder + "vignette.cnt";
                        cntPaths[1] = gameDataBinFolder + "tex32.cnt";
                        cnt = new CNT(cntPaths);
                    }
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

        public void ReadLevelNames(Reader reader, Pointer off_levels, uint num_levels) {
            levels = new string[num_levels];
            for (uint i = 0; i < num_levels; i++) {
                levels[i] = reader.ReadString(0x1E);
            }
        }

        public void ReadLanguages(Reader reader, Pointer off_languages, uint num_languages) {
            languages = new string[num_languages];
            languages_loc = new string[num_languages];
            for (uint i = 0; i < num_languages; i++) {
                languages[i] = reader.ReadString(0x14);
                languages_loc[i] = reader.ReadString(0x14);
            }
        }

        public void ReadTexturesFix(Reader reader, Pointer off_textures) {
            uint num_textureMemoryChannels = 0;
            if (Settings.s.engineVersion <= Settings.EngineVersion.R2) num_textureMemoryChannels = reader.ReadUInt32();
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
                if (Settings.s.platform == Settings.Platform.GC) {
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
                } else if (Settings.s.platform == Settings.Platform.iOS) {
                    for (int i = 0; i < num_textures; i++) {
                        string texturePath = gameDataBinFolder + "WORLD/GRAPHICS/TEXTURES/" + textures[i].name.ToUpper().Substring(0, textures[i].name.LastIndexOf('.')) + ".GF";
                        if (FileSystem.FileExists(texturePath)) {
                            GF gf = new GF(texturePath);
                            if (gf != null) textures[i].Texture = gf.GetTexture();
                        }
                    }
                } else {
                    for (int i = 0; i < num_textures; i++) {
                        GF gf = cnt.GetGFByTGAName(textures[i].name);
                        if (gf != null) textures[i].Texture = gf.GetTexture();
                    }
                }
                if (Settings.s.engineVersion == Settings.EngineVersion.R3) {
                    for (uint i = 0; i < num_textures; i++) {
                        reader.ReadUInt32(); // 0 or 8.
                    }
                }
            }
        }

        public void ReadTexturesLvl(Reader reader, Pointer off_textures) {
            uint num_textures_fix = (uint)textures.Length,
                num_memoryChannels = 0,
                num_textures_lvl = 0,
                num_textures_total = 0;

            if (Settings.s.engineVersion <= Settings.EngineVersion.R2) {
                num_textures_fix = (uint)textures.Length;
                num_memoryChannels = reader.ReadUInt32();
                num_textures_lvl = reader.ReadUInt32();
                num_textures_total = num_textures_fix + num_textures_lvl;
            } else {
                num_textures_total = Settings.s.platform == Settings.Platform.GC ? reader.ReadUInt32() : 1024;
                num_textures_fix = Settings.s.platform == Settings.Platform.GC ? (uint)textures.Length : reader.ReadUInt32();
                num_textures_lvl = num_textures_total - num_textures_fix;
            }
            Array.Resize(ref textures, (int)num_textures_total);
            for (uint i = num_textures_fix; i < num_textures_total; i++) {
                Pointer off_texture = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_texture, () => {
                    textures[i] = TextureInfo.Read(reader, off_texture);
                });
            }
            if (Settings.s.engineVersion <= Settings.EngineVersion.R2) {
                uint num_texturesToCreate = reader.ReadUInt32();
                for (uint i = 0; i < num_textures_fix; i++) { // ?
                    reader.ReadUInt32(); //1
                }
                uint currentMemoryChannel = reader.ReadUInt32();
            }
            if (Settings.s.platform == Settings.Platform.GC) {
                // Load textures from TPL
                TPL lvlTPL = new TPL(tplPaths[Mem.Lvl]);
                TPL transitTPL = hasTransit ? new TPL(tplPaths[Mem.Transit]) : null;
                print("Lvl TPL Texture count: " + lvlTPL.Count);
                if (hasTransit) print("Transit TPL Texture count: " + transitTPL.Count);
                int transitTexturesSeen = 0;
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    uint file_texture = reader.ReadUInt32();
                    if (hasTransit && file_texture == 6) {
                        textures[i].Texture = transitTPL.textures[transitTexturesSeen++];
                    } else {
                        textures[i].Texture = lvlTPL.textures[i - num_textures_fix - transitTexturesSeen];
                    }
                }
            } else if (Settings.s.platform == Settings.Platform.iOS) {
                // Load textures from separate GF files
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    string texturePath = gameDataBinFolder + "WORLD/GRAPHICS/TEXTURES/" + textures[i].name.ToUpper().Substring(0, textures[i].name.LastIndexOf('.')) + ".GF";
                    if (FileSystem.FileExists(texturePath)) {
                        GF gf = new GF(texturePath);
                        if (gf != null) textures[i].Texture = gf.GetTexture();
                    }
                }
            } else {
                // Load textures from CNT
                int transitTexturesSeen = 0;
                for (uint i = num_textures_fix; i < num_textures_total; i++) {
                    uint file_texture = Settings.s.engineVersion == Settings.EngineVersion.R3 ? reader.ReadUInt32() : 0;
                    if (file_texture == 0xC0DE2005 || textures[i] == null) continue; // texture is undefined
                    if (hasTransit && file_texture == 6) transitTexturesSeen++;
                    GF gf = cnt.GetGFByTGAName(textures[i].name);
                    if (gf != null) textures[i].Texture = gf.GetTexture();
                }
            }
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



        protected IEnumerator PrepareFile(string path) {
            if (Application.platform == RuntimePlatform.WebGLPlayer) {
                string state = loadingState;
                loadingState = "Downloading file: " + path;
                yield return controller.StartCoroutine(FileSystem.DownloadFile(path));
                loadingState = state;
                yield return null;
            }
        }
    }
}
