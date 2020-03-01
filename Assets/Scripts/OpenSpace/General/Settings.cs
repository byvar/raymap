using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LinkedListType = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public class Settings {
        public enum Mode {
			[Description("Rayman 2 (PC)")] Rayman2PC,
			[Description("Rayman 2 (DC)")] Rayman2DC,
			[Description("Rayman 2 (iOS)")] Rayman2IOS,
			[Description("Rayman 2 (PS1)")] Rayman2PS1,
			[Description("Rayman 2 (PS2)")] Rayman2PS2,
			[Description("Rayman 2 (N64)")] Rayman2N64,
			[Description("Rayman 2 (DS)")] Rayman2DS,
			[Description("Rayman 2 (3DS)")] Rayman23DS,
			[Description("R2 (PC) Demo 1 (1999/08/19)")] Rayman2PCDemo1,
			[Description("R2 (PC) Demo 2 (1999/09/11)")] Rayman2PCDemo2,
			[Description("Rayman M (PC)")] RaymanMPC,
			[Description("Rayman M (PS2)")] RaymanMPS2,
			[Description("Rayman Arena (PC)")] RaymanArenaPC,
			[Description("Rayman Arena (PS2)")] RaymanArenaPS2,
			[Description("Rayman Arena (GC)")] RaymanArenaGC,
			[Description("Rayman Arena Demo (GC)")] RaymanArenaGCDemo,
			[Description("Rayman Arena (Xbox)")] RaymanArenaXbox,
			[Description("Rayman Rush (PS1)")] RaymanRushPS1,
			[Description("Rayman 3 (PC)")] Rayman3PC,
			[Description("Rayman 3 (GC)")] Rayman3GC,
			[Description("Rayman 3 (PS2)")] Rayman3PS2,
			[Description("R3 (PS2) Demo (2002/08/07)")] Rayman3PS2Demo_2002_08_07,
			[Description("R3 (PS2) Dev Build (2002/09/06)")] Rayman3PS2DevBuild,
			[Description("R3 (PS2) Demo (2002/10/29)")] Rayman3PS2Demo_2002_10_29,
			[Description("R3 (PS2) Demo (2002/12/18)")] Rayman3PS2Demo_2002_12_18,
			[Description("Rayman 3 (Xbox)")] Rayman3Xbox,
			[Description("Rayman 3 (Xbox 360)")] Rayman3Xbox360,
			[Description("Rayman 3 (PS3)")] Rayman3PS3,
			[Description("Rayman Raving Rabbids (DS)")] RaymanRavingRabbidsDS,
			[Description("Tonic Trouble (PC)")] TonicTroublePC,
			[Description("Tonic Trouble: SE (PC)")] TonicTroubleSEPC,
			[Description("Donald Duck: Quack Attack (PC)")] DonaldDuckPC,
			[Description("Donald Duck: Quack Attack (DC)")] DonaldDuckDC,
			[Description("Donald Duck: Quack Attack (N64)")] DonaldDuckN64,
			[Description("Donald Duck: PK (GC)")] DonaldDuckPKGC,
			[Description("Playmobil: Hype (PC)")] PlaymobilHypePC,
			[Description("Playmobil: Laura (PC)")] PlaymobilLauraPC,
			[Description("Playmobil: Alex (PC)")] PlaymobilAlexPC,
			[Description("Disney's Dinosaur (PC)")] DinosaurPC,
			[Description("Largo Winch (PC)")] LargoWinchPC,
		};
        public Mode mode = Mode.Rayman3PC;
		
		public static Dictionary<string, Mode> cmdModeNameDict = new Dictionary<string, Mode>() {
			{ "r3_gc", Mode.Rayman3GC },
			{ "r3_pc", Mode.Rayman3PC },
			{ "r3_ps2", Mode.Rayman3PS2 },
			{ "r3_demo_ps2_20020807", Mode.Rayman3PS2Demo_2002_08_07 },
			{ "r3_devbuild_ps2", Mode.Rayman3PS2DevBuild },
			{ "r3_demo_ps2_20021029", Mode.Rayman3PS2Demo_2002_10_29 },
			{ "r3_demo_ps2_20021218", Mode.Rayman3PS2Demo_2002_12_18 },
			{ "r3_xbox", Mode.Rayman3Xbox },
			{ "r3_xbox360", Mode.Rayman3Xbox360 },
			{ "r3_ps3", Mode.Rayman3PS3 },
			{ "ra_gc", Mode.RaymanArenaGC },
			{ "ra_demo_gc", Mode.RaymanArenaGCDemo },
			{ "ra_xbox", Mode.RaymanArenaXbox },
			{ "ra_ps2", Mode.RaymanArenaPS2 },
			{ "ra_pc", Mode.RaymanArenaPC },
			{ "rm_ps2", Mode.RaymanMPS2 },
			{ "rm_pc", Mode.RaymanMPC },
			{ "r2_pc", Mode.Rayman2PC },
			{ "r2_dc", Mode.Rayman2DC },
			{ "r2_ios", Mode.Rayman2IOS },
			{ "r2_ps1", Mode.Rayman2PS1 },
			{ "r2_ps2", Mode.Rayman2PS2 },
			{ "r2_ds", Mode.Rayman2DS },
			{ "rrr_ds", Mode.RaymanRavingRabbidsDS },
			{ "r2_3ds", Mode.Rayman23DS },
			{ "r2_n64", Mode.Rayman2N64 },
			{ "dd_pc", Mode.DonaldDuckPC },
			{ "dd_dc", Mode.DonaldDuckDC },
			{ "dd_n64", Mode.DonaldDuckN64 },
			{ "ddpk_gc", Mode.DonaldDuckPKGC },
			{ "tt_pc", Mode.TonicTroublePC },
			{ "ttse_pc", Mode.TonicTroubleSEPC },
			{ "r2_demo1_pc", Mode.Rayman2PCDemo1 },
			{ "r2_demo2_pc", Mode.Rayman2PCDemo2 },
			{ "playmobil_hype_pc", Mode.PlaymobilHypePC },
			{ "playmobil_alex_pc", Mode.PlaymobilAlexPC },
			{ "playmobil_laura_pc", Mode.PlaymobilLauraPC },
			{ "dinosaur_pc", Mode.DinosaurPC },
			{ "largowinch_pc", Mode.LargoWinchPC },
		};

        public enum EngineVersion {
            TT = 0,
            Montreal = 1,
            R2 = 2,
            R3 = 3
        };
        public enum Game { R3, RA, RM, RRush, R2, TT, TTSE, R2Demo, R2Revolution, DD, DDPK, PlaymobilHype, PlaymobilLaura, PlaymobilAlex, RRR, Dinosaur, LargoWinch };
        public enum Platform { PC, iOS, GC, DC, PS1, PS2, PS3, Xbox, Xbox360, DS, _3DS, N64 };
        public enum Endian { Little, Big };
        public enum Encryption { None, ReadInit, FixedInit, CalculateInit, Window };
		public enum Caps { All, AllExceptExtension, Normal, None };
		public enum CapsType { All, LevelFolder, LevelFile, Fix, FixLvl, FixRelocation, LangFix, LangLevelFolder, LangLevelFile, DSB, LMFile, TextureFile };
        
        public EngineVersion engineVersion;
        public Game game;
        public Platform platform;
        public Endian endian;
        public LinkedListType linkedListType;
		public bool hasObjectTypes = true;
        public bool hasNames = false;
        public bool hasDeformations = false;
        public int numEntryActions = 0;
        public bool hasExtraInputData = false;
        public bool hasMemorySupport = false;
        public Dictionary<string, uint> memoryAddresses = null;
        public bool loadFromMemory = false;
        public Encryption encryption = Encryption.None;
        public bool encryptPointerFiles = false;
        public bool hasLinkedListHeaderPointers = false;
        public bool snaCompression = false;
        public AITypes aiTypes;
        public float textureAnimationSpeedModifier = 1f;
        public float luminosity = 0.5f;
        public bool saturate = true;
		public Dictionary<CapsType, Caps> caps = new Dictionary<CapsType, Caps>();
        public bool linkUncategorizedObjectsToScriptFamily = false;

        public bool IsLittleEndian {
            get { return endian == Endian.Little; }
        }

        public static void Init(Mode mode) {
			if (settingsDict.ContainsKey(mode)) {
				s = settingsDict[mode];
			}
            if (s != null) s.mode = mode;
        }


        public static Settings s = null;
        public static Settings R3PC = new Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.R3,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            hasDeformations = true,
            aiTypes = AITypes.R3,
            hasMemorySupport = true,
            textureAnimationSpeedModifier = 10f,
            luminosity = 0.1f,
            saturate = false,
            memoryAddresses = new Dictionary<string, uint> {
                { "actualWorld", 0x007D9A4C },
                { "dynamicWorld", 0x007D9934 },
                { "inactiveDynamicWorld", 0x007D9924 },
                { "fatherSector", 0x007D9920 },
                { "firstSubmapPosition", 0x007A837C },
                { "always", 0x005D29F8 },
                { "anim_stacks", 0x007D0980 },
                { "anim_framesKF", 0x007CFAE0 },
                { "anim_a3d", 0x007D07E0 },
                { "anim_channels", 0x007CFE20 },
                { "anim_framesNumOfNTTO", 0x007CFC80 },
                { "anim_hierarchies", 0x007D0300 },
                { "anim_morphData", 0x007CF600 },
                { "anim_keyframes", 0x007CF940 },
                { "anim_onlyFrames", 0x007CFFC0 },
                { "anim_vectors", 0x007D0640 },
                { "anim_events", 0x007CF7A0 },
                { "anim_NTTO", 0x007D0160 },
                { "anim_quaternions", 0x007D04A0 },
                { "anim_deformations", 0x007CF460 },
                { "families", 0x007D83AC },
                { "objectTypes", 0x007D9A60 },
                { "textures", 0x007E4AA0 },
                { "textureMemoryChannels", 0x007E3AA0 },
                { "inputStructure", 0x0083F7E0 },
                { "localizationStructure", 0x007A84E0 },
                { "num_visualMaterials", 0x005F5E80 },
                { "visualMaterials", 0x005BFAD4 },
                { "brightness", 0x005F5E20 },
            },
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
        public static Settings R3GC = new Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.R3,
            platform = Platform.GC,
            endian = Endian.Big,
            linkedListType = LinkedListType.Double,
            hasNames = true,
            hasDeformations = true,
            aiTypes = AITypes.R3,
            hasExtraInputData = true,
            hasLinkedListHeaderPointers = true,
            textureAnimationSpeedModifier = -10f,
            luminosity = 0.1f,
            saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings R3PS2 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings R3PS2Demo_20020807 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings R3PS2DevBuild = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings R3PS2Demo_20021029 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings R3PS2Demo_20021218 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			hasNames = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings R3Xbox = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.Xbox,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = true,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings R3Xbox360 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.Xbox360,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = true,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
			hasNames = true,
		};
		public static Settings R3PS3 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PS3,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = true,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
			hasNames = true,
		};

		public static Settings RMPC = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.RM,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			aiTypes = AITypes.R3,
			hasDeformations = true,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.3f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings RMPS2 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.RM,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings RAPC = new Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.RA,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R3,
            hasDeformations = true,
            textureAnimationSpeedModifier = 10f,
            luminosity = 0.3f,
            saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings RAPS2 = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.RA,
			platform = Platform.PS2,
			endian = Endian.Little,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.5f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Settings RAGC = new Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.RA,
            platform = Platform.GC,
            endian = Endian.Big,
            linkedListType = LinkedListType.Minimize,
            aiTypes = AITypes.R3,
            hasDeformations = true,
            textureAnimationSpeedModifier = -10f,
            luminosity = 0.1f,
            saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings RAGCDemo = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.RA,
			platform = Platform.GC,
			endian = Endian.Big,
			linkedListType = LinkedListType.Minimize,
			aiTypes = AITypes.R3,
			hasDeformations = true,
			textureAnimationSpeedModifier = -10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings RAXbox = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.RA,
			platform = Platform.Xbox,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			aiTypes = AITypes.R3,
			hasDeformations = true,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};

		public static Settings R2PC = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R2,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
            hasMemorySupport = true,
            linkUncategorizedObjectsToScriptFamily = true,
            memoryAddresses = new Dictionary<string, uint> {
                { "actualWorld", 0x005013C8 },
                { "dynamicWorld", 0x00500FD0 },
                { "inactiveDynamicWorld", 0x00500FC4 },
                { "fatherSector", 0x00500FC0 },
                { "firstSubmapPosition", 0x004FF760 },
                { "always", 0x004A6B18 },
                { "anim_stacks", 0x004A6B38 },
                { "anim_framesKF", 0x00500274 },
                { "anim_a3d", 0x00500278 },
                { "anim_channels", 0x0050027C },
                { "anim_framesNumOfNTTO", 0x00500280 },
                { "anim_hierarchies", 0x00500284 },
                { "anim_morphData", 0x00500288 },
                { "anim_keyframes", 0x0050028C },
                { "anim_onlyFrames", 0x00500290 },
                { "anim_vectors", 0x00500294 },
                { "anim_events", 0x00500298 },
                { "anim_NTTO", 0x0050029C },
                { "anim_quaternions", 0x005002A0 },
                { "engineStructure", 0x00500380 },
                { "families", 0x00500560 },
                { "objectTypes", 0x005013E0 },
                { "textures", 0x00502680 },
                { "textureMemoryChannels", 0x00501660 },
                { "inputStructure", 0x00509E60 },
                { "localizationStructure", 0x00500260 }
            }
        };
        public static Settings R2PCDemo1 = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2Demo,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
            aiTypes = AITypes.R2Demo,
            numEntryActions = 1
        };
        public static Settings R2PCDemo2 = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2Demo,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 7,
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R2,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true
        };
        public static Settings R2DC = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2,
            platform = Platform.DC,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Minimize,
            encryption = Encryption.None,
            luminosity = 0.5f,
            saturate = true,
            aiTypes = AITypes.R2,
            hasExtraInputData = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
        };
		public static Settings R2PS2 = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2Revolution,
			platform = Platform.PS2,
			endian = Endian.Little,
			numEntryActions = 42,
			linkedListType = LinkedListType.Minimize,
			encryption = Encryption.None,
			luminosity = 0.5f,
			saturate = false,
			aiTypes = AITypes.Revolution,
			//textureAnimationSpeedModifier = 2f,
			hasExtraInputData = false,
			hasObjectTypes = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.None }
			}
		};
		public static Settings R2IOS = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2,
            platform = Platform.iOS,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Double,
            encryption = Encryption.ReadInit,
            aiTypes = AITypes.R2,
            hasExtraInputData = true,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};
		public static Settings R2PS1 = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			numEntryActions = 1,
		};

		public static Settings RRushPS1 = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.RRush,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			numEntryActions = 1
		};

		public static Settings R2DS = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform.DS,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
			numEntryActions = 1,
			textureAnimationSpeedModifier = -1f,
		};
		public static Settings RRRDS = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.RRR,
			platform = Platform.DS,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
			numEntryActions = 1,
			textureAnimationSpeedModifier = -1f,
		};
		public static Settings R23DS = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform._3DS,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
			numEntryActions = 1,
			textureAnimationSpeedModifier = -1f,
		};
		public static Settings R2N64 = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform.N64,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
			numEntryActions = 1
		};
		public static Settings DDN64 = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.DD,
			platform = Platform.N64,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
			numEntryActions = 1
		};

		public static Settings DDPC = new Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.DD,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 44, // 43 for demo
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R2,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.FixRelocation, Caps.AllExceptExtension }
			}
        };
		public static Settings DDDC = new Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.DD,
			platform = Platform.DC,
			endian = Endian.Little,
			numEntryActions = 43,
			linkedListType = LinkedListType.Minimize,
			encryption = Encryption.None,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			hasExtraInputData = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};

		public static Settings TTPC = new Settings() {
			engineVersion = EngineVersion.TT,
			game = Game.TT,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			numEntryActions = 1,
			aiTypes = AITypes.TTSE,
			encryption = Encryption.Window,
			encryptPointerFiles = true,
			hasLinkedListHeaderPointers = true,
			luminosity = 1f,
			saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.Fix, Caps.None },
				{ CapsType.FixRelocation, Caps.None },
				{ CapsType.DSB, Caps.None }
			}
        };
		public static Settings TTSEPC = new Settings() {
			engineVersion = EngineVersion.TT,
			game = Game.TTSE,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			numEntryActions = 1,
			aiTypes = AITypes.TTSE,
			hasLinkedListHeaderPointers = true,
			luminosity = 0.5f,
			saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};

        public static Settings PlaymobilHypePC = new Settings() {
            engineVersion = EngineVersion.Montreal,
            game = Game.PlaymobilHype,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            numEntryActions = 1,
            aiTypes = AITypes.Hype,
            hasLinkedListHeaderPointers = true,
            snaCompression = true,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.FixLvl, Caps.None }
			}
		};
        public static Settings PlaymobilAlexPC = new Settings() {
            engineVersion = EngineVersion.Montreal,
            game = Game.PlaymobilAlex,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            numEntryActions = 1,
            aiTypes = AITypes.Hype,
            hasLinkedListHeaderPointers = true,
            snaCompression = true,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};
        public static Settings PlaymobilLauraPC = new Settings() {
            engineVersion = EngineVersion.Montreal,
            game = Game.PlaymobilLaura,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            numEntryActions = 1,
            aiTypes = AITypes.Hype,
            hasLinkedListHeaderPointers = true,
            snaCompression = false,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.FixRelocation, Caps.None },
				{ CapsType.LangFix, Caps.None },
				{ CapsType.LangLevelFile, Caps.None },
				{ CapsType.LangLevelFolder, Caps.None }
			}
        };

		public static Settings DDPKGC = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.DDPK,
			platform = Platform.GC,
			endian = Endian.Big,
			linkedListType = LinkedListType.Minimize,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			textureAnimationSpeedModifier = -10f,
			luminosity = 0.1f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};

		public static Settings DinosaurPC = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.Dinosaur,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			aiTypes = AITypes.R3,
			hasDeformations = true,
			textureAnimationSpeedModifier = 1f,
			luminosity = 0.3f,
			saturate = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Settings LargoWinchPC = new Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.LargoWinch,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			aiTypes = AITypes.Largo,
			textureAnimationSpeedModifier = 1f,
			luminosity = 0.5f,
			saturate = false,
			hasExtraInputData = true,
			hasObjectTypes = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFolder, Caps.Normal },
				{ CapsType.LMFile, Caps.Normal },
				{ CapsType.LevelFile, Caps.None },
			},
			hasDeformations = true
		};


		public static Dictionary<Mode, Settings> settingsDict = new Dictionary<Mode, Settings>() {
			{ Mode.Rayman2PC, R2PC },
			{ Mode.Rayman2DC, R2DC },
			{ Mode.Rayman2IOS, R2IOS },
			{ Mode.Rayman2PS1, R2PS1 },
			{ Mode.Rayman2PS2, R2PS2 },
			{ Mode.Rayman2N64, R2N64 },
			{ Mode.Rayman2DS, R2DS },
			{ Mode.Rayman23DS, R23DS },
			{ Mode.Rayman2PCDemo1, R2PCDemo1 },
			{ Mode.Rayman2PCDemo2, R2PCDemo2 },
			{ Mode.RaymanMPC, RMPC },
			{ Mode.RaymanMPS2, RMPS2 },
			{ Mode.RaymanArenaPC, RAPC },
			{ Mode.RaymanArenaPS2, RAPS2 },
			{ Mode.RaymanArenaGC, RAGC },
			{ Mode.RaymanArenaGCDemo, RAGCDemo },
			{ Mode.RaymanArenaXbox, RAXbox },
			{ Mode.RaymanRushPS1, RRushPS1 },
			{ Mode.Rayman3PC, R3PC },
			{ Mode.Rayman3GC, R3GC },
			{ Mode.Rayman3PS2, R3PS2 },
			{ Mode.Rayman3PS2Demo_2002_08_07, R3PS2Demo_20020807 },
			{ Mode.Rayman3PS2DevBuild, R3PS2DevBuild },
			{ Mode.Rayman3PS2Demo_2002_10_29, R3PS2Demo_20021029 },
			{ Mode.Rayman3PS2Demo_2002_12_18, R3PS2Demo_20021218 },
			{ Mode.Rayman3Xbox, R3Xbox },
			{ Mode.Rayman3Xbox360, R3Xbox360 },
			{ Mode.Rayman3PS3, R3PS3 },
			{ Mode.RaymanRavingRabbidsDS, RRRDS },
			{ Mode.TonicTroublePC, TTPC },
			{ Mode.TonicTroubleSEPC, TTSEPC },
			{ Mode.DonaldDuckPC, DDPC },
			{ Mode.DonaldDuckDC, DDDC },
			{ Mode.DonaldDuckN64, DDN64 },
			{ Mode.DonaldDuckPKGC, DDPKGC },
			{ Mode.PlaymobilHypePC, PlaymobilHypePC },
			{ Mode.PlaymobilLauraPC, PlaymobilLauraPC },
			{ Mode.PlaymobilAlexPC, PlaymobilAlexPC },
			{ Mode.DinosaurPC, DinosaurPC },
			{ Mode.LargoWinchPC, LargoWinchPC },
		};
	}
}