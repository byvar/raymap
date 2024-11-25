﻿using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using static OpenSpace.LevelTranslation;
using LinkedListType = OpenSpace.LinkedList.Type;

namespace OpenSpace {
    public class Settings {

        public Settings(Settings s = null)
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

		public static Dictionary<string, Mode> cmdModeNameDict = new Dictionary<string, Mode>() {
			{ "r3_gc", Mode.Rayman3GC },
			{ "r3_pc", Mode.Rayman3PC },
			{ "r3_pc_demo_20021001", Mode.Rayman3PCDemo_2002_10_01 },
			{ "r3_pc_demo_20021021", Mode.Rayman3PCDemo_2002_10_21 },
			{ "r3_pc_demo_20021209", Mode.Rayman3PCDemo_2002_12_09 },
			{ "r3_pc_demo_20030106", Mode.Rayman3PCDemo_2003_01_06 },
			{ "r3_macos", Mode.Rayman3MacOS },
			{ "r3_ps2", Mode.Rayman3PS2 },
			{ "r3_ps2_demo_20020517", Mode.Rayman3PS2Demo_2002_05_17 },
			{ "r3_ps2_demo_20020807", Mode.Rayman3PS2Demo_2002_08_07 },
			{ "r3_ps2_devbuild_20020906", Mode.Rayman3PS2DevBuild_2002_09_06 },
			{ "r3_ps2_demo_20021029", Mode.Rayman3PS2Demo_2002_10_29 },
			{ "r3_ps2_demo_20021218", Mode.Rayman3PS2Demo_2002_12_18 },
			{ "r3_xbox", Mode.Rayman3Xbox },
			{ "r3_x360", Mode.Rayman3Xbox360 },
			{ "r3_ps3", Mode.Rayman3PS3 },
			{ "ra_gc", Mode.RaymanArenaGC },
			{ "ra_gc_demo_20020307", Mode.RaymanArenaGCDemo_2002_03_07 },
			{ "ra_xbox", Mode.RaymanArenaXbox },
			{ "ra_ps2", Mode.RaymanArenaPS2 },
			{ "ra_pc", Mode.RaymanArenaPC },
			{ "rm_ps2", Mode.RaymanMPS2 },
			{ "rm_ps2_demo_20010725", Mode.RaymanMPS2Demo_2001_07_25 },
			{ "rm_pc", Mode.RaymanMPC },
			{ "rr_ps1", Mode.RaymanRushPS1 },
			{ "r2_pc", Mode.Rayman2PC },
			{ "r2_pc_demo_19990818", Mode.Rayman2PCDemo_1999_08_18 },
			{ "r2_pc_demo_19990904", Mode.Rayman2PCDemo_1999_09_04 },
			{ "r2_dc", Mode.Rayman2DC },
			{ "r2_ios", Mode.Rayman2IOS },
			{ "r2_ios_demo", Mode.Rayman2IOSDemo },
			{ "r2_ps1", Mode.Rayman2PS1 },
			{ "r2_ps2", Mode.Rayman2PS2 },
			{ "r2_ds", Mode.Rayman2DS },
			{ "rrr_ds", Mode.RaymanRavingRabbidsDS },
			{ "rrr_ds_devbuild_20060525", Mode.RaymanRavingRabbidsDSDevBuild_2006_05_25 },
			{ "r2_3ds", Mode.Rayman23DS },
			{ "r2_n64", Mode.Rayman2N64 },
			{ "dd_pc", Mode.DonaldDuckPC },
			{ "dd_dc", Mode.DonaldDuckDC },
			{ "dd_n64", Mode.DonaldDuckN64 },
			{ "dd_ps1", Mode.DonaldDuckPS1 },
			{ "ddpk_gc", Mode.DonaldDuckPKGC },
			{ "tt_pc", Mode.TonicTroublePC },
			{ "ttse_pc", Mode.TonicTroubleSEPC },
			{ "playmobil_hype_pc", Mode.PlaymobilHypePC },
			{ "playmobil_alex_pc", Mode.PlaymobilAlexPC },
			{ "playmobil_laura_pc", Mode.PlaymobilLauraPC },
			{ "dinosaur_pc", Mode.DinosaurPC },
			{ "largowinch_pc", Mode.LargoWinchPC },
			{ "jb_ps1", Mode.JungleBookPS1 },
			{ "vip_ps1", Mode.VIPPS1 },
			{ "redplanet_pc", Mode.RedPlanetPC },
		};

        #region Enums
        public enum Mode {
            [Description("Rayman 2 (PC)")] Rayman2PC,
            [Description("R2 (PC) Demo (1999/08/19)")] Rayman2PCDemo_1999_08_18,
            [Description("R2 (PC) Demo (1999/09/11)")] Rayman2PCDemo_1999_09_04,
            [Description("Rayman 2 (DC)")] Rayman2DC,
            [Description("Rayman 2 (iOS)")] Rayman2IOS,
			[Description("Rayman 2 (iOS) Demo")] Rayman2IOSDemo,
			[Description("Rayman 2 (PS1)")] Rayman2PS1,
            [Description("Rayman 2 (PS2)")] Rayman2PS2,
            [Description("Rayman 2 (N64)")] Rayman2N64,
            [Description("Rayman 2 (DS)")] Rayman2DS,
            [Description("Rayman 2 (3DS)")] Rayman23DS,
            [Description("Rayman M (PC)")] RaymanMPC,
            [Description("Rayman M (PS2)")] RaymanMPS2,
            [Description("RM (PS2) Demo (2001/07/25)")] RaymanMPS2Demo_2001_07_25,
            [Description("Rayman Arena (PC)")] RaymanArenaPC,
            [Description("Rayman Arena (PS2)")] RaymanArenaPS2,
            [Description("Rayman Arena (GC)")] RaymanArenaGC,
            [Description("RA (GC) Demo (2002/03/07)")] RaymanArenaGCDemo_2002_03_07,
            [Description("Rayman Arena (Xbox)")] RaymanArenaXbox,
            [Description("Rayman Rush (PS1)")] RaymanRushPS1,
            [Description("Rayman 3 (PC)")] Rayman3PC,
			[Description("R3 (PC) Demo (2002/10/01)")] Rayman3PCDemo_2002_10_01,
			[Description("R3 (PC) Demo (2002/10/21)")] Rayman3PCDemo_2002_10_21,
			[Description("R3 (PC) Demo (2002/12/09)")] Rayman3PCDemo_2002_12_09,
			[Description("R3 (PC) Demo (2003/01/06)")] Rayman3PCDemo_2003_01_06,
			[Description("Rayman 3 (MacOS)")] Rayman3MacOS,
			[Description("Rayman 3 (GC)")] Rayman3GC,
            [Description("Rayman 3 (PS2)")] Rayman3PS2,
			[Description("R3 (PS2) Demo (2002/05/17)")] Rayman3PS2Demo_2002_05_17,
			[Description("R3 (PS2) Demo (2002/08/07)")] Rayman3PS2Demo_2002_08_07,
            [Description("R3 (PS2) Dev Build (2002/09/06)")] Rayman3PS2DevBuild_2002_09_06,
            [Description("R3 (PS2) Demo (2002/10/29)")] Rayman3PS2Demo_2002_10_29,
            [Description("R3 (PS2) Demo (2002/12/18)")] Rayman3PS2Demo_2002_12_18,
            [Description("Rayman 3 (Xbox)")] Rayman3Xbox,
            [Description("Rayman 3 (Xbox 360)")] Rayman3Xbox360,
            [Description("Rayman 3 (PS3)")] Rayman3PS3,
            [Description("Rayman Raving Rabbids (DS)")] RaymanRavingRabbidsDS,
			[Description("Rayman Raving Rabbids (DS) Dev Build (2006/05/25)")] RaymanRavingRabbidsDSDevBuild_2006_05_25,
			[Description("Tonic Trouble (PC)")] TonicTroublePC,
            [Description("Tonic Trouble: SE (PC)")] TonicTroubleSEPC,
            [Description("Tonic Trouble (N64 NTSC)")] TonicTroubleN64,
			[Description("Donald Duck: Quack Attack (PC)")] DonaldDuckPC,
            [Description("Donald Duck: Quack Attack (PC) Demo")] DonaldDuckPCDemo,
            [Description("Donald Duck: Quack Attack (DC)")] DonaldDuckDC,
            [Description("Donald Duck: Quack Attack (N64)")] DonaldDuckN64,
			[Description("Donald Duck: Quack Attack (PS1)")] DonaldDuckPS1,
			[Description("Donald Duck: PK (GC)")] DonaldDuckPKGC,
            [Description("Playmobil: Hype (PC)")] PlaymobilHypePC,
            [Description("Playmobil: Laura (PC)")] PlaymobilLauraPC,
            [Description("Playmobil: Alex (PC)")] PlaymobilAlexPC,
            [Description("Disney's Dinosaur (PC)")] DinosaurPC,
            [Description("Largo Winch (PC)")] LargoWinchPC,
			[Description("Jungle Book: Groove Party (PS1)")] JungleBookPS1,
			[Description("VIP (PS1)")] VIPPS1,
			[Description("Red Planet (PC)")] RedPlanetPC,
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

        public static void Init(Mode mode) {
			s = GetSettings(mode);
        }

		public static Settings GetSettings(Mode mode) {
			Settings s = null;
			if (settingsDict.ContainsKey(mode)) {
				s = settingsDict[mode];
			}
			if (s != null) s.mode = mode;
			return s;
		}

		public string CmdModeName {
			get {
				if(!cmdModeNameDict.Any(c => c.Value == mode)) return null;
				return cmdModeNameDict.FirstOrDefault(c => c.Value == mode).Key;
			}
		}

		public string DisplayName {
			get {
				Type genericEnumType = mode.GetType();
				MemberInfo[] memberInfo = genericEnumType.GetMember(mode.ToString());
				if ((memberInfo != null && memberInfo.Length > 0)) {
					var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
					if ((_Attribs != null && _Attribs.Count() > 0)) {
						return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
					}
				}
				return mode.ToString();
			}
		}


        public static Settings s = null;

        #region Settings

        public static Settings R3PC => new Settings() {
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

		public static Settings R3PCDemo20021001 => new Settings() {
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
		public static Settings R3PCDemo20021021 => new Settings() {
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
		public static Settings R3PCDemo20021209 => new Settings() {
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
		public static Settings R3PCDemo20030106 => new Settings() {
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
		public static Settings R3MacOS => new Settings() {
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

		public static Settings R3GC => new Settings() {
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
		public static Settings R3PS2 => new Settings() {
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
		public static Settings R3PS2Demo_20020517 => new Settings() {
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
		public static Settings R3PS2Demo_20020807 => new Settings() {
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
		public static Settings R3PS2DevBuild => new Settings() {
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
		public static Settings R3PS2Demo_20021029 => new Settings() {
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
		public static Settings R3PS2Demo_20021218 => new Settings() {
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
		public static Settings R3Xbox => new Settings() {
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
		public static Settings R3Xbox360 => new Settings() {
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
		public static Settings R3PS3 => new Settings() {
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

		public static Settings RMPC => new Settings() {
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
		public static Settings RMPS2 => new Settings() {
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
		public static Settings RMPS2Demo => new Settings() {
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
		public static Settings RAPC => new Settings() {
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
		public static Settings RAPS2 => new Settings() {
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
		public static Settings RAGC => new Settings() {
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
		public static Settings RAGCDemo => new Settings() {
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
		public static Settings RAXbox => new Settings() {
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

        public static Settings R2PC => new Settings() {
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
        public static Settings R2PCDemo1 => new Settings() {
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
        public static Settings R2PCDemo2 => new Settings() {
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
        public static Settings R2DC => new Settings() {
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
		public static Settings R2PS2 => new Settings() {
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
			levelTranslation = LevelTranslation.levelTranslation_r2,
			hasObjectTypes = false,
			caps = new Dictionary<CapsType, Caps>() {
				{ CapsType.All, Caps.None }
			}
		};
		public static Settings R2IOS => new Settings() {
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
		public static Settings R2PS1 => new Settings() {
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

		public static Settings RRushPS1 => new Settings() {
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

		public static Settings R2DS => new Settings() {
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
		public static Settings RRRDS_20060525 => new Settings() {
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
		public static Settings RRRDS => new Settings() {
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
		public static Settings R23DS => new Settings() {
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
		public static Settings R2N64 => new Settings() {
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
		public static Settings DDN64 => new Settings() {
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

        public static Settings TTN64 = new Settings()
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

		public static Settings DDPC => new Settings() {
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

        public static Settings DDPCDemo = new Settings(Settings.DDPC)
        {
            numEntryActions = 43
        };
        
        public static Settings DDDC => new Settings() {
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
		public static Settings DDPS1 => new Settings() {
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

		public static Settings RedPlanetPC => new Settings() {
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

		public static Settings TTPC => new Settings() {
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
		public static Settings TTSEPC => new Settings() {
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

        public static Settings PlaymobilHypePC => new Settings() {
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
        public static Settings PlaymobilAlexPC => new Settings() {
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
        public static Settings PlaymobilLauraPC => new Settings() {
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

		public static Settings DDPKGC => new Settings() {
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

		public static Settings DinosaurPC => new Settings() {
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
		public static Settings LargoWinchPC => new Settings() {
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
		public static Settings VIPPS1 => new Settings() {
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
		public static Settings JungleBookPS1 => new Settings() {
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


		public static Dictionary<Mode, Settings> settingsDict = new Dictionary<Mode, Settings>() {
			{ Mode.Rayman2PC, R2PC },
			{ Mode.Rayman2PCDemo_1999_08_18, R2PCDemo1 },
			{ Mode.Rayman2PCDemo_1999_09_04, R2PCDemo2 },
			{ Mode.Rayman2DC, R2DC },
			{ Mode.Rayman2IOS, R2IOS },
			{ Mode.Rayman2IOSDemo, R2IOS },
			{ Mode.Rayman2PS1, R2PS1 },
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
			{ Mode.PlaymobilHypePC, PlaymobilHypePC },
			{ Mode.PlaymobilLauraPC, PlaymobilLauraPC },
			{ Mode.PlaymobilAlexPC, PlaymobilAlexPC },
			{ Mode.DinosaurPC, DinosaurPC },
			{ Mode.LargoWinchPC, LargoWinchPC },
			{ Mode.VIPPS1, VIPPS1 },
			{ Mode.JungleBookPS1, JungleBookPS1 },
			{ Mode.RedPlanetPC, RedPlanetPC },
		};
	}
}