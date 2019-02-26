using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkedListType = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public class Settings {
        public enum Mode {
            Rayman3PC, Rayman3GC,
            RaymanArenaPC, RaymanArenaGC,
            Rayman2PC, Rayman2DC, Rayman2IOS, Rayman2PS1, Rayman2PS2,
            Rayman2PCDemo2, Rayman2PCDemo1,
            DonaldDuckPC, DonaldDuckDC,
            TonicTroublePC, TonicTroubleSEPC,
            PlaymobilHypePC, PlaymobilAlexPC, PlaymobilLauraPC
        };
        public Mode mode = Mode.Rayman3PC;

        public enum EngineVersion {
            TT = 0,
            Montreal = 1,
            R2 = 2,
            R3 = 3
        };
        public enum Game { R3, RA, R2, TT, TTSE, R2Demo, R2Revolution, DD, PlaymobilHype, PlaymobilLaura, PlaymobilAlex };
        public enum Platform { PC, iOS, GC, DC, PS1, PS2 };
        public enum Endian { Little, Big };
        public enum Encryption { None, ReadInit, FixedInit, CalculateInit, Window };
		public enum Caps { All, AllExceptExtension, Normal, None };
		public enum CapsType { All, LevelFolder, LevelFile, Fix, FixLvl, FixRelocation, LangFix, LangLevelFolder, LangLevelFile, DSB };
        
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
            switch (mode) {
                case Mode.Rayman2IOS: s = Settings.R2IOS; break;
                case Mode.Rayman2DC: s = Settings.R2DC; break;
                case Mode.Rayman2PC: s = Settings.R2PC; break;
				case Mode.Rayman2PS1: s = Settings.R2PS1; break;
				case Mode.Rayman2PS2: s = Settings.R2PS2; break;
                case Mode.Rayman2PCDemo1: s = Settings.R2PCDemo1; break;
                case Mode.Rayman2PCDemo2: s = Settings.R2PCDemo2; break;
                case Mode.Rayman3GC: s = Settings.R3GC; break;
                case Mode.Rayman3PC: s = Settings.R3PC; break;
                case Mode.RaymanArenaGC: s = Settings.RAGC; break;
                case Mode.RaymanArenaPC: s = Settings.RAPC; break;
                case Mode.DonaldDuckPC: s = Settings.DDPC; break;
				case Mode.DonaldDuckDC: s = Settings.DDDC; break;
                case Mode.TonicTroublePC: s = Settings.TTPC; break;
                case Mode.TonicTroubleSEPC: s = Settings.TTSEPC; break;
                case Mode.PlaymobilHypePC: s = Settings.PlaymobilHypePC; break;
                case Mode.PlaymobilAlexPC: s = Settings.PlaymobilAlexPC; break;
                case Mode.PlaymobilLauraPC: s = Settings.PlaymobilLauraPC; break;
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
                { "fontStructure", 0x007A84E0 },
                { "num_visualMaterials", 0x005F5E80 },
                { "visualMaterials", 0x005BFAD4 },
                { "brightness", 0x005F5E20 },
            }
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
            saturate = false
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
            saturate = false
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
            saturate = false
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
                { "fontStructure", 0x00500260 }
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
            aiTypes = AITypes.R2,
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
			saturate = true,
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
    }
}