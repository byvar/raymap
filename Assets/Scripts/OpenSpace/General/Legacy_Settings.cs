﻿using BinarySerializer.Ubisoft.CPA;
using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using static OpenSpace.LevelTranslation;
using LinkedListType = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public class Legacy_Settings {

        public Legacy_Settings(Legacy_Settings s = null)
        {
            if (s != null) {
                this.mode = s.mode;
                this.engineVersion = s.engineVersion;
                this.game = s.game;
                this.platform = s.platform;
                this.endian = s.endian;
				this.linkedListType = s.linkedListType;
                this.hasObjectTypes = s.hasObjectTypes;
                this.hasNames = s.hasNames;
                this.hasDeformations = s.hasDeformations;
                this.numEntryActions = s.numEntryActions;
                this.hasExtraInputData = s.hasExtraInputData;
                this.hasMemorySupport = s.hasMemorySupport;
                this.memoryAddresses = s.memoryAddresses;
                this.loadFromMemory = s.loadFromMemory;
                this.encryption = s.encryption;
                this.encryptPointerFiles = s.encryptPointerFiles;
                this.hasLinkedListHeaderPointers = s.hasLinkedListHeaderPointers;
                this.snaCompression = s.snaCompression;
                this.aiTypes = s.aiTypes;
                this.textureAnimationSpeedModifier = s.textureAnimationSpeedModifier;
                this.luminosity = s.luminosity;
                this.saturate = s.saturate;
                this.caps = s.caps;
                this.levelTranslation = s.levelTranslation;
                this.linkUncategorizedObjectsToScriptFamily = s.linkUncategorizedObjectsToScriptFamily;
            }
		}

        #region Enums
        public enum Mode {
			Rayman2PC,
			Rayman2PCDemo_1999_08_18,
			Rayman2PCDemo_1999_09_04,
			Rayman2DC,
			Rayman2DCDevBuild_1999_11_22,
			Rayman2IOS,
			Rayman2IOSDemo,
			Rayman2PS1,
			Rayman2PS1Demo,
			Rayman2PS1Demo_SLUS_90095,
			Rayman2PS2,
			Rayman2N64,
			Rayman2DS,
			Rayman23DS,
			RaymanMPC,
			RaymanMPS2,
			RaymanMPS2Demo_2001_07_25,
			RaymanArenaPC,
			RaymanArenaPS2,
			RaymanArenaGC,
			RaymanArenaGCDemo_2002_03_07,
			RaymanArenaXbox,
			RaymanRushPS1,
			Rayman3PC,
			Rayman3PCDemo_2002_10_01,
			Rayman3PCDemo_2002_10_21,
			Rayman3PCDemo_2002_12_09,
			Rayman3PCDemo_2003_01_06,
			Rayman3MacOS,
			Rayman3GC,
			Rayman3PS2,
			Rayman3PS2Demo_2002_05_17,
			Rayman3PS2Demo_2002_08_07,
			Rayman3PS2DevBuild_2002_09_06,
			Rayman3PS2Demo_2002_10_29,
			Rayman3PS2Demo_2002_12_18,
			Rayman3Xbox,
			Rayman3Xbox360,
			Rayman3PS3,
			RaymanRavingRabbidsDS,
			RaymanRavingRabbidsDSDevBuild_2006_05_25,
			TonicTroublePC,
			TonicTroubleSEPC,
			TonicTroubleN64,
			DonaldDuckPC,
			DonaldDuckPCDemo,
			DonaldDuckDC,
			DonaldDuckN64,
			DonaldDuckPS1,
			DonaldDuckPKGC,
			DonaldDuckPKPS2,
			PlaymobilHypePC,
			PlaymobilLauraPC,
			PlaymobilLauraPCPentiumIII,
			PlaymobilAlexPC,
			DinosaurPC,
			LargoWinchPC,
			JungleBookPS1,
			VIPPS1,
			RedPlanetPC,
		};
        public enum EngineVersion {
            TT = 0,
            Montreal = 1,
            R2 = 2,
            R3 = 3
        };
        public enum Game { R3, RA, RM, RRush, R2, TT, TTSE, R2Demo, R2Revolution, DD, DDPK, PlaymobilHype, PlaymobilLaura, PlaymobilAlex, RRR, Dinosaur, LargoWinch, JungleBook, VIP, RedPlanet };
        public enum Platform { PC, MacOS, iOS, GC, DC, PS1, PS2, PS3, Xbox, Xbox360, DS, _3DS, N64 };
        public enum Endian { Little, Big };
        public enum Encryption { None, ReadInit, FixedInit, CalculateInit, Window, RedPlanet };
		public enum Caps { All, AllExceptExtension, Normal, None };
		public enum CapsType { All,
			LevelFolder, LevelFile, LevelRelocation,
			Fix, FixLvl, FixRelocation,
			LangFix, LangLevelFolder, LangLevelFile, LangLevelRelocation,
			DSB, LMFile, TextureFile };
        #endregion

        #region Variables
        public Mode mode = Mode.Rayman3PC;
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
		public AI_Types newAITypes;
        public float textureAnimationSpeedModifier = 1f;
        public float luminosity = 0.5f;
        public bool saturate = true;
		public Dictionary<CapsType, Caps> caps = new Dictionary<CapsType, Caps>();
        public LevelTranslation levelTranslation = null;
        public bool linkUncategorizedObjectsToScriptFamily = false;

        public bool IsLittleEndian {
            get { return endian == Endian.Little; }
        }

        #endregion

        public static void Init(Legacy_Settings newSettings) {
			s = newSettings;
        }

		public static Legacy_Settings GetSettings(Mode mode) {
			Legacy_Settings s = null;
			if (settingsDict.ContainsKey(mode)) {
				s = settingsDict[mode];
			}
			if (s != null) s.mode = mode;
			return s;
		}


        public static Legacy_Settings s = null;

        #region Settings

        public static Legacy_Settings R3PC => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
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

		public static Legacy_Settings R3PCDemo20021001 => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3PCDemo20021021 => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3PCDemo20021209 => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3PCDemo20030106 => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.PC,
			endian = Endian.Little,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3MacOS => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.R3,
			platform = Platform.MacOS,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			hasDeformations = true,
			aiTypes = AITypes.R3,
			hasMemorySupport = false,
			textureAnimationSpeedModifier = 10f,
			luminosity = 0.1f,
			saturate = false,
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};

		public static Legacy_Settings R3GC => new Legacy_Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.R3,
            platform = Platform.GC,
            endian = Endian.Big,
            linkedListType = LinkedListType.Double,
            hasNames = true,
            hasDeformations = true,
            aiTypes = AITypes.R3_GC,
            hasExtraInputData = true,
            hasLinkedListHeaderPointers = true,
            textureAnimationSpeedModifier = -10f,
            luminosity = 0.1f,
            saturate = false,
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3PS2 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings R3PS2Demo_20020517 => new Legacy_Settings() {
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
			levelTranslation = LevelTranslation.levelTranslation_r3,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
			hasNames = true
		};
		public static Legacy_Settings R3PS2Demo_20020807 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings R3PS2DevBuild => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings R3PS2Demo_20021029 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings R3PS2Demo_20021218 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            hasNames = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings R3Xbox => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings R3Xbox360 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.None }
			},
			hasNames = true,
		};
		public static Legacy_Settings R3PS3 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r3,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.None }
			},
			hasNames = true,
		};

		public static Legacy_Settings RMPC => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_pc,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings RMPS2 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_pc,
            hasNames = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings RMPS2Demo => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_pc,
            hasNames = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings RAPC => new Legacy_Settings() {
            engineVersion = EngineVersion.R3,
            game = Game.RA,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R3,
			newAITypes = new AI_Types_RA_PC(),
            hasDeformations = true,
            textureAnimationSpeedModifier = 10f,
            luminosity = 0.3f,
            saturate = false,
            levelTranslation = LevelTranslation.levelTranslation_rarena_pc,
            hasMemorySupport = true,
            memoryAddresses = new Dictionary<string, uint> { // Based on non-safedisc version (exe md5 checksum 2e2f94698692bda5c0900440e8a54151)
                { "actualWorld", 0x8B97E8 },
                { "dynamicWorld", 0x8B96D8 },
                { "inactiveDynamicWorld", 0x8B96D4 },
                { "fatherSector", 0x8B96D0 },
                { "firstSubmapPosition", 0x890078 },
                { "always", 0x64689C },
                { "anim_stacks", 0x8918A0 },
                { "anim_framesKF", 0x890A00 },
                { "anim_a3d", 0x891700 },
                { "anim_channels", 0x890D40 },
                { "anim_framesNumOfNTTO", 0x890BA0 },
                { "anim_hierarchies", 0x891220 },
                { "anim_morphData", 0x890520 },
                { "anim_keyframes", 0x890860 },
                { "anim_onlyFrames", 0x890ee0 },
                { "anim_vectors", 0x891560 },
                { "anim_events", 0x8906c0 },
                { "anim_NTTO", 0x891080 },
                { "anim_quaternions", 0x8913c0 },
                { "anim_deformations", 0x890380 },
                { "families", 0x8B898C },
                { "objectTypes", 0x8B9800 },
                { "textures", 0x881B80 },
                { "textureMemoryChannels", 0x880B80 },
                { "inputStructure", 0x831100 },
                { "localizationStructure", 0x897E40 },
                { "num_visualMaterials", 0x6464E0 },
                { "visualMaterials", 0x5DB05C },
                { "brightness", 0x60050C },
            },
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings RAPS2 => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_pc,
            hasNames = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings RAGC => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings RAGCDemo => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};
		public static Legacy_Settings RAXbox => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.Fix, Caps.None },
				{ CapsType.TextureFile, Caps.Normal },
			},
		};

        public static Legacy_Settings R2PC => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r2,
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
        public static Legacy_Settings R2PCDemo1 => new Legacy_Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2Demo,
            platform = Platform.PC,
            endian = Endian.Little,
            linkedListType = LinkedListType.Double,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
            aiTypes = AITypes.R2Demo,
            numEntryActions = 1,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.FixRelocation, Caps.AllExceptExtension },
				{ CapsType.LevelRelocation, Caps.AllExceptExtension },
			}
		};
        public static Legacy_Settings R2PCDemo2 => new Legacy_Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2Demo,
            platform = Platform.PC,
            endian = Endian.Little,
            numEntryActions = 7,
            linkedListType = LinkedListType.Double,
            aiTypes = AITypes.R2,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.FixRelocation, Caps.AllExceptExtension },
				{ CapsType.LevelRelocation, Caps.AllExceptExtension },
			}
		};
        public static Legacy_Settings R2DC => new Legacy_Settings() {
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
            levelTranslation = LevelTranslation.levelTranslation_r2,
            caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
        };
		public static Legacy_Settings R2DCDevBuild_1999_11_22 => new Legacy_Settings() {
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
			levelTranslation = LevelTranslation.levelTranslation_r2,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};
		public static Legacy_Settings R2PS2 => new Legacy_Settings() {
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
		public static Legacy_Settings R2IOS => new Legacy_Settings() {
            engineVersion = EngineVersion.R2,
            game = Game.R2,
            platform = Platform.iOS,
            endian = Endian.Little,
            numEntryActions = 43,
            linkedListType = LinkedListType.Double,
            encryption = Encryption.ReadInit,
            aiTypes = AITypes.R2,
            hasExtraInputData = true,
            levelTranslation = LevelTranslation.levelTranslation_r2,
            luminosity = 0.5f,
            saturate = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.All }
			}
		};
		public static Legacy_Settings R2PS1 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Single,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			numEntryActions = 1,
		};

		public static Legacy_Settings RRushPS1 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.RRush,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Single,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			numEntryActions = 1
		};

		public static Legacy_Settings R2DS => new Legacy_Settings() {
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
		public static Legacy_Settings RRRDS_20060525 => new Legacy_Settings() {
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
		public static Legacy_Settings RRRDS => new Legacy_Settings() {
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
		public static Legacy_Settings R23DS => new Legacy_Settings() {
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
		public static Legacy_Settings R2N64 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.R2,
			platform = Platform.N64,
			endian = Endian.Big,
			linkedListType = LinkedListType.Double,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2ROM,
            levelTranslation = LevelTranslation.levelTranslation_r2,
            numEntryActions = 1
		};
		public static Legacy_Settings DDN64 => new Legacy_Settings() {
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

        public static Legacy_Settings TTN64 = new Legacy_Settings()
        {
            engineVersion = EngineVersion.TT,
            game = Game.TT,
            platform = Platform.N64,
            endian = Endian.Big,
            linkedListType = LinkedListType.Double,
            encryption = Encryption.ReadInit,
            luminosity = 0.5f,
            saturate = true,
            aiTypes = AITypes.R2ROM,
            numEntryActions = 1
        };

		public static Legacy_Settings DDPC => new Legacy_Settings() {
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
			},
			hasMemorySupport = true,
            memoryAddresses = new Dictionary<string, uint> {
                { "actualWorld", 0x5AF2E8 },
                { "dynamicWorld", 0x5AEEF0 },
                { "inactiveDynamicWorld", 0x5AEEE4 },
                { "fatherSector", 0x5AEEE0 },
                { "firstSubmapPosition", 0x5ACE40 },
                { "always", 0x4A8EC8 },
                { "anim_stacks", 0x4A8EF0 },
                { "anim_framesKF", 0x5AD954 },
                { "anim_a3d", 0x5AD958 },
                { "anim_channels", 0x5AD95C },
                { "anim_framesNumOfNTTO", 0x5AD960 },
                { "anim_hierarchies", 0x5AD964 },
                { "anim_morphData", 0x5AD968 },
                { "anim_keyframes", 0x5AD96c },
                { "anim_onlyFrames", 0x5AD970 },
                { "anim_vectors", 0x5AD974 },
                { "anim_events", 0x5AD978 },
                { "anim_NTTO", 0x5AD97C },
                { "anim_quaternions", 0x5AD980 },
                { "engineStructure", 0x5AE2A0 },
                { "families", 0x5AE480 },
                { "objectTypes", 0x5AF300 },
                { "textures", 0x5B05A0 },
                { "textureMemoryChannels", 0x5AF580 },
                { "inputStructure", 0x5B7DA0 },
                { "localizationStructure", 0x5AD940 }
			}
		};

        public static Legacy_Settings DDPCDemo = new Legacy_Settings(Legacy_Settings.DDPC)
        {
            numEntryActions = 43
        };
        
        public static Legacy_Settings DDDC => new Legacy_Settings() {
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
		public static Legacy_Settings DDPS1 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.DD,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Single,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			numEntryActions = 1
		};

		public static Legacy_Settings RedPlanetPC => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.RedPlanet,
			platform = Platform.PC,
			endian = Endian.Little,
			numEntryActions = 31,
			linkedListType = LinkedListType.Double,
			aiTypes = AITypes.R2,
			encryption = Encryption.RedPlanet,
			luminosity = 0.5f,
			saturate = true,
			hasMemorySupport = false,
			hasExtraInputData = true,
			linkUncategorizedObjectsToScriptFamily = true,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.FixRelocation, Caps.AllExceptExtension },
				{ CapsType.LevelRelocation, Caps.AllExceptExtension },
			}
		};

		public static Legacy_Settings TTPC => new Legacy_Settings() {
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
		public static Legacy_Settings TTSEPC => new Legacy_Settings() {
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

        public static Legacy_Settings PlaymobilHypePC => new Legacy_Settings() {
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
        public static Legacy_Settings PlaymobilAlexPC => new Legacy_Settings() {
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
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.LevelRelocation, Caps.None },
				{ CapsType.FixRelocation, Caps.None },
				{ CapsType.LangFix, Caps.None },
				{ CapsType.LangLevelFile, Caps.None },
				{ CapsType.LangLevelRelocation, Caps.None },
				{ CapsType.LangLevelFolder, Caps.None }
			}
		};
        public static Legacy_Settings PlaymobilLauraPC => new Legacy_Settings() {
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
				{ CapsType.LevelRelocation, Caps.None },
				{ CapsType.FixRelocation, Caps.None },
				{ CapsType.LangFix, Caps.None },
				{ CapsType.LangLevelFile, Caps.None },
				{ CapsType.LangLevelRelocation, Caps.None },
				{ CapsType.LangLevelFolder, Caps.None }
			}
        };
		public static Legacy_Settings PlaymobilLauraPC_PentiumIII => new Legacy_Settings() {
			engineVersion = EngineVersion.Montreal,
			game = Game.PlaymobilLaura,
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
				{ CapsType.LevelFile, Caps.None },
				{ CapsType.LevelRelocation, Caps.None },
				{ CapsType.FixRelocation, Caps.None },
				{ CapsType.LangFix, Caps.None },
				{ CapsType.LangLevelFile, Caps.None },
				{ CapsType.LangLevelRelocation, Caps.None },
				{ CapsType.LangLevelFolder, Caps.None },
				{ CapsType.LevelFolder, Caps.None }
			}
		};

		public static Legacy_Settings DDPKGC => new Legacy_Settings() {
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

		public static Legacy_Settings DDPKPS2 => new Legacy_Settings() {
			engineVersion = EngineVersion.R3,
			game = Game.DDPK,
			platform = Platform.PS2,
			endian = Endian.Little,
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

		public static Legacy_Settings DinosaurPC => new Legacy_Settings() {
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
				{ CapsType.All, Caps.All }
			},
		};
		public static Legacy_Settings LargoWinchPC => new Legacy_Settings() {
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
		public static Legacy_Settings VIPPS1 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.VIP,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Single,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			hasDeformations = true,
			numEntryActions = 1
		};
		public static Legacy_Settings JungleBookPS1 => new Legacy_Settings() {
			engineVersion = EngineVersion.R2,
			game = Game.JungleBook,
			platform = Platform.PS1,
			endian = Endian.Little,
			linkedListType = LinkedListType.Single,
			encryption = Encryption.ReadInit,
			luminosity = 0.5f,
			saturate = true,
			aiTypes = AITypes.R2,
			hasDeformations = true,
			numEntryActions = 1
		};

		#endregion


		public static Dictionary<Mode, Legacy_Settings> settingsDict = new Dictionary<Mode, Legacy_Settings>() {
			{ Mode.Rayman2PC, R2PC },
			{ Mode.Rayman2PCDemo_1999_08_18, R2PCDemo1 },
			{ Mode.Rayman2PCDemo_1999_09_04, R2PCDemo2 },
			{ Mode.Rayman2DC, R2DC },
			{ Mode.Rayman2DCDevBuild_1999_11_22, R2DCDevBuild_1999_11_22 },
			{ Mode.Rayman2IOS, R2IOS },
			{ Mode.Rayman2IOSDemo, R2IOS },
			{ Mode.Rayman2PS1, R2PS1 },
			{ Mode.Rayman2PS1Demo, R2PS1 },
			{ Mode.Rayman2PS1Demo_SLUS_90095, R2PS1 },
			{ Mode.Rayman2PS2, R2PS2 },
			{ Mode.Rayman2N64, R2N64 },
			{ Mode.TonicTroubleN64, TTN64 },
			{ Mode.Rayman2DS, R2DS },
			{ Mode.Rayman23DS, R23DS },
			{ Mode.RaymanMPC, RMPC },
			{ Mode.RaymanMPS2, RMPS2 },
			{ Mode.RaymanMPS2Demo_2001_07_25, RMPS2Demo },
			{ Mode.RaymanArenaPC, RAPC },
			{ Mode.RaymanArenaPS2, RAPS2 },
			{ Mode.RaymanArenaGC, RAGC },
			{ Mode.RaymanArenaGCDemo_2002_03_07, RAGCDemo },
			{ Mode.RaymanArenaXbox, RAXbox },
			{ Mode.RaymanRushPS1, RRushPS1 },
			{ Mode.Rayman3PC, R3PC },
			{ Mode.Rayman3PCDemo_2002_10_01, R3PCDemo20021001 },
			{ Mode.Rayman3PCDemo_2002_10_21, R3PCDemo20021021 },
			{ Mode.Rayman3PCDemo_2002_12_09, R3PCDemo20021209 },
			{ Mode.Rayman3PCDemo_2003_01_06, R3PCDemo20030106 },
			{ Mode.Rayman3MacOS, R3MacOS },
			{ Mode.Rayman3GC, R3GC },
			{ Mode.Rayman3PS2, R3PS2 },
			{ Mode.Rayman3PS2Demo_2002_05_17, R3PS2Demo_20020517 },
			{ Mode.Rayman3PS2Demo_2002_08_07, R3PS2Demo_20020807 },
			{ Mode.Rayman3PS2DevBuild_2002_09_06, R3PS2DevBuild },
			{ Mode.Rayman3PS2Demo_2002_10_29, R3PS2Demo_20021029 },
			{ Mode.Rayman3PS2Demo_2002_12_18, R3PS2Demo_20021218 },
			{ Mode.Rayman3Xbox, R3Xbox },
			{ Mode.Rayman3Xbox360, R3Xbox360 },
			{ Mode.Rayman3PS3, R3PS3 },
			{ Mode.RaymanRavingRabbidsDS, RRRDS },
			{ Mode.RaymanRavingRabbidsDSDevBuild_2006_05_25, RRRDS_20060525 },
			{ Mode.TonicTroublePC, TTPC },
			{ Mode.TonicTroubleSEPC, TTSEPC },
			{ Mode.DonaldDuckPC, DDPC },
			{ Mode.DonaldDuckPCDemo, DDPCDemo },
			{ Mode.DonaldDuckDC, DDDC },
			{ Mode.DonaldDuckN64, DDN64 },
			{ Mode.DonaldDuckPS1, DDPS1 },
			{ Mode.DonaldDuckPKGC, DDPKGC },
			{ Mode.DonaldDuckPKPS2, DDPKPS2 },
			{ Mode.PlaymobilHypePC, PlaymobilHypePC },
			{ Mode.PlaymobilLauraPC, PlaymobilLauraPC },
			{ Mode.PlaymobilLauraPCPentiumIII, PlaymobilLauraPC_PentiumIII },
			{ Mode.PlaymobilAlexPC, PlaymobilAlexPC },
			{ Mode.DinosaurPC, DinosaurPC },
			{ Mode.LargoWinchPC, LargoWinchPC },
			{ Mode.VIPPS1, VIPPS1 },
			{ Mode.JungleBookPS1, JungleBookPS1 },
			{ Mode.RedPlanetPC, RedPlanetPC },
		};
	}
}