using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinarySerializer.Ubisoft.CPA {
	public class CPA_Settings_Defines {
		#region Rayman 3
		public static CPA_Settings R3PC => new CPA_Settings(EngineVersion.Rayman3, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			MemoryAddresses = new Dictionary<string, uint> {
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
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};

		public static CPA_Settings R3PCDemo20021001 => new CPA_Settings(EngineVersion.Rayman3, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3PCDemo20021021 => new CPA_Settings(EngineVersion.Rayman3, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3PCDemo20021209 => new CPA_Settings(EngineVersion.Rayman3, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3PCDemo20030106 => new CPA_Settings(EngineVersion.Rayman3, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3MacOS => new CPA_Settings(EngineVersion.Rayman3, Platform.MacOS) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};

		public static CPA_Settings R3GC => new CPA_Settings(EngineVersion.Rayman3, Platform.GC) {
			StaticListType = LST2_ListType.DoubleLinked,
			Defines = CPA_EngineDefines.AllDebug,
			HasDeformations = true,
			AITypes = new AI_Types_R3_GC(),
			COLTypes = new COL_Types_R2(),
			HasExtraInputData = true,
			HasLinkedListHeaderPointers = true,
			TextureAnimationSpeedModifier = -10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3PS2 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			//StaticListType = LST2_ListType.Optimized,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings R3PS2Demo_20020517 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
			Defines = CPA_EngineDefines.AllDebug
		};
		public static CPA_Settings R3PS2Demo_20020807 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings R3PS2DevBuild => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings R3PS2Demo_20021029 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings R3PS2Demo_20021218 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = false,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			Defines = CPA_EngineDefines.AllDebug,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings R3Xbox => new CPA_Settings(EngineVersion.Rayman3, Platform.Xbox) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings R3Xbox360 => new CPA_Settings(EngineVersion.Rayman3, Platform.Xbox360) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.None }
			},
			Defines = CPA_EngineDefines.AllDebug,
		};
		public static CPA_Settings R3PS3 => new CPA_Settings(EngineVersion.Rayman3, Platform.PS3) {
			StaticListType = LST2_ListType.DoubleLinked,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasMemorySupport = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_r3,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.None }
			},
			Defines = CPA_EngineDefines.AllDebug,
		};
		#endregion

		#region Rayman M
		public static CPA_Settings RMPC => new CPA_Settings(EngineVersion.RaymanM, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.3f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_pc,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings RMPS2 => new CPA_Settings(EngineVersion.RaymanM, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_pc,
			Defines = CPA_EngineDefines.AllDebug,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings RMPS2Demo => new CPA_Settings(EngineVersion.RaymanM, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_pc,
			Defines = CPA_EngineDefines.AllDebug,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings RAPC => new CPA_Settings(EngineVersion.RaymanM, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.3f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_pc,
			HasMemorySupport = true,
			MemoryAddresses = new Dictionary<string, uint> { // Based on non-safedisc version (exe md5 checksum 2e2f94698692bda5c0900440e8a54151)
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
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings RAPS2 => new CPA_Settings(EngineVersion.RaymanM, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.5f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_pc,
			Defines = CPA_EngineDefines.AllDebug,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings RAGC => new CPA_Settings(EngineVersion.RaymanArena, Platform.GC) {
			StaticListType = LST2_ListType.OptimizedArray,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = -10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings RAGCDemo => new CPA_Settings(EngineVersion.RaymanArena, Platform.GC) {
			StaticListType = LST2_ListType.OptimizedArray,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = -10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings RAXbox => new CPA_Settings(EngineVersion.RaymanArena, Platform.Xbox) {
			StaticListType = LST2_ListType.DoubleLinked,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = 10f,
			Luminosity = 0.1f,
			Saturate = false,
			LevelTranslation = LevelTranslation.levelTranslation_rarena_xboxgc,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};

		public static CPA_Settings RRushPS1 => new CPA_Settings(EngineVersion.RaymanRush_PS1, Platform.PS1) {
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1
		};
		#endregion

		#region Rayman 2
		public static CPA_Settings R2PC => new CPA_Settings(EngineVersion.Rayman2, Platform.PC) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.SingleLinked,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2_PC(),
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			HasMemorySupport = true,
			LevelTranslation = LevelTranslation.levelTranslation_r2,
			LinkUncategorizedObjectsToScriptFamily = true,
			MemoryAddresses = new Dictionary<string, uint> {
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
		public static CPA_Settings R2PCBeta_1998_07_22 => new CPA_Settings(EngineVersion.Rayman2Beta_19980722, Platform.PC) {
			StaticListType = LST2_ListType.SingleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC_Beta_1998_07_22(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2_Demo(),
			EntryActionsCount = 1,
			IsPressDemo = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.AllExceptExtension },
			}
		};
		public static CPA_Settings R2PCDemo1 => new CPA_Settings(EngineVersion.Rayman2Demo, Platform.PC) {
			StaticListType = LST2_ListType.SingleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC_Demo1(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2_Demo(),
			EntryActionsCount = 1,
			IsPressDemo = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.AllExceptExtension },
			}
		};
		public static CPA_Settings R2PCDemo2 => new CPA_Settings(EngineVersion.Rayman2Demo, Platform.PC) {
			EntryActionsCount = 7,
			StaticListType = LST2_ListType.SingleLinked,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2_Demo(),
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			IsPressDemo = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.AllExceptExtension },
			}
		};
		public static CPA_Settings R2DC => new CPA_Settings(EngineVersion.Rayman2, Platform.DC) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.None,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			HasExtraInputData = false,
			LevelTranslation = LevelTranslation.levelTranslation_r2,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			}
		};
		public static CPA_Settings R2DCDevBuild_1999_11_22 => new CPA_Settings(EngineVersion.Rayman2, Platform.DC) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.None,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			HasExtraInputData = false,
			LevelTranslation = LevelTranslation.levelTranslation_r2,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			}
		};
		public static CPA_Settings R2PS2 => new CPA_Settings(EngineVersion.Rayman2Revolution, Platform.PS2) {
			EntryActionsCount = 42,
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.None,
			Luminosity = 0.5f,
			Saturate = false,
			AITypes = new AI_Types_R2_PS2(),
			COLTypes = new COL_Types_R2(),
			//textureAnimationSpeedModifier = 2f,
			HasExtraInputData = false,
			HasObjectTypes = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.None }
			}
		};
		public static CPA_Settings R2IOS => new CPA_Settings(EngineVersion.Rayman2, Platform.iOS) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.SingleLinked,
			Encryption = Encryption.ReadInit,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2_iOS(),
			HasExtraInputData = true,
			LevelTranslation = LevelTranslation.levelTranslation_r2,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			}
		};
		public static CPA_Settings R2PS1 => new CPA_Settings(EngineVersion.Rayman2_PS1, Platform.PS1) {
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1,
		};

		public static CPA_Settings R2DS => new CPA_Settings(EngineVersion.Rayman2, Platform.DS) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1,
			TextureAnimationSpeedModifier = -1f,
		};
		public static CPA_Settings R23DS => new CPA_Settings(EngineVersion.Rayman2_3D, Platform._3DS) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1,
			TextureAnimationSpeedModifier = -1f,
		};
		public static CPA_Settings R2N64 => new CPA_Settings(EngineVersion.Rayman2, Platform.N64) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			LevelTranslation = LevelTranslation.levelTranslation_r2,
			EntryActionsCount = 1
		};
		#endregion

		#region Rayman Raving Rabbids DS
		public static CPA_Settings RRRDS_20060525 => new CPA_Settings(EngineVersion.Rayman4DS_20060525, Platform.DS) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1,
			TextureAnimationSpeedModifier = -1f,
		};
		public static CPA_Settings RRRDS => new CPA_Settings(EngineVersion.RaymanRavingRabbids, Platform.DS) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1,
			TextureAnimationSpeedModifier = -1f,
		};
		#endregion

		#region Tonic Trouble
		public static CPA_Settings TTPC => new CPA_Settings(EngineVersion.TonicTrouble, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_TTSE(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_TT(),
			Encryption = Encryption.Window,
			EncryptPointerFiles = true,
			HasLinkedListHeaderPointers = true,
			Luminosity = 1f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.FixRelocation, PathCapitalization.None },
				{ PathCapitalizationType.DSB, PathCapitalization.None }
			}
		};
		public static CPA_Settings TTN64 = new CPA_Settings(EngineVersion.TonicTrouble, Platform.N64) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1
		};
		public static CPA_Settings TTSEPC => new CPA_Settings(EngineVersion.TonicTroubleSE, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_TTSE(),
			COLTypes = new COL_Types_R2(),
			HasLinkedListHeaderPointers = true,
			Luminosity = 0.5f,
			Saturate = true,
			HasExtraInputData = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			}
		};
		#endregion

		#region Montreal: Playmobil
		public static CPA_Settings PlaymobilHypePC => new CPA_Settings(EngineVersion.PlaymobilHype, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_Hype_PS2(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_Montreal(),
			HasLinkedListHeaderPointers = true,
			SNA_Compression = true,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixLvl, PathCapitalization.None }
			}
		};
		public static CPA_Settings PlaymobilAlexPC => new CPA_Settings(EngineVersion.PlaymobilAlex, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_Hype_PS2(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_Montreal(),
			HasLinkedListHeaderPointers = true,
			SNA_Compression = true,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.FixRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangFix, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFolder, PathCapitalization.None }
			}
		};
		public static CPA_Settings PlaymobilLauraPC => new CPA_Settings(EngineVersion.PlaymobilLaura, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_Hype_PS2(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_Montreal(),
			HasLinkedListHeaderPointers = true,
			SNA_Compression = false,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.FixRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangFix, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFolder, PathCapitalization.None }
			}
		};
		public static CPA_Settings PlaymobilLauraPC_PentiumIII => new CPA_Settings(EngineVersion.PlaymobilLauraPentiumIII, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			EntryActionsCount = 1,
			AITypes = new AI_Types_Hype_PS2(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_Montreal(),
			HasLinkedListHeaderPointers = true,
			SNA_Compression = false,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.FixRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangFix, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFile, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelRelocation, PathCapitalization.None },
				{ PathCapitalizationType.LangLevelFolder, PathCapitalization.None },
				{ PathCapitalizationType.LevelFolder, PathCapitalization.None }
			}
		};
		#endregion

		#region Donald Duck Quack Attack
		public static CPA_Settings DDPC => new CPA_Settings(EngineVersion.DonaldDuckQuackAttack, Platform.PC) {
			EntryActionsCount = 44, // 43 for demo
			StaticListType = LST2_ListType.SingleLinked,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2(),
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension }
			},
			HasMemorySupport = true,
			MemoryAddresses = new Dictionary<string, uint> {
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

		public static CPA_Settings DDPCDemo = new CPA_Settings(EngineVersion.DonaldDuckQuackAttackDemo, Platform.PC) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.SingleLinked,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			SNATypes = new SNA_Types_R2(),
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension }
			},
		};

		public static CPA_Settings DDDC => new CPA_Settings(EngineVersion.DonaldDuckQuackAttack, Platform.DC) {
			EntryActionsCount = 43,
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.None,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			HasExtraInputData = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			}
		};
		public static CPA_Settings DDN64 => new CPA_Settings(EngineVersion.DonaldDuckQuackAttack, Platform.N64) {
			StaticListType = LST2_ListType.DoubleLinked,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_DS_EU(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1
		};
		public static CPA_Settings DDPS1 => new CPA_Settings(EngineVersion.DonaldDuckQuackAttack_PS1, Platform.PS1) {
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			EntryActionsCount = 1
		};
		#endregion

		#region Other Disney
		public static CPA_Settings DDPKGC => new CPA_Settings(EngineVersion.DonaldDuckPK, Platform.GC) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = -10f,
			Luminosity = 0.1f,
			Saturate = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};
		public static CPA_Settings DDPKPS2 => new CPA_Settings(EngineVersion.DonaldDuckPK, Platform.PS2) {
			StaticListType = LST2_ListType.OptimizedArray,
			HasDeformations = true,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = -10f,
			Luminosity = 0.1f,
			Saturate = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
				{ PathCapitalizationType.Fix, PathCapitalization.None },
				{ PathCapitalizationType.TextureFile, PathCapitalization.Normal },
			},
		};

		public static CPA_Settings DinosaurPC => new CPA_Settings(EngineVersion.Dinosaur, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			AITypes = new AI_Types_R3_PS2(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			TextureAnimationSpeedModifier = 1f,
			Luminosity = 0.3f,
			Saturate = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.All, PathCapitalization.All }
			},
		};
		public static CPA_Settings JungleBookPS1 => new CPA_Settings(EngineVersion.JungleBook_PS1, Platform.PS1) {
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			EntryActionsCount = 1
		};
		#endregion

		#region Other licensed
		public static CPA_Settings LargoWinchPC => new CPA_Settings(EngineVersion.LargoWinch, Platform.PC) {
			StaticListType = LST2_ListType.DoubleLinked,
			AITypes = new AI_Types_LargoWinch_PC(),
			COLTypes = new COL_Types_R2(),
			TextureAnimationSpeedModifier = 1f,
			Luminosity = 0.5f,
			Saturate = false,
			HasExtraInputData = true,
			HasObjectTypes = false,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.LevelFolder, PathCapitalization.Normal },
				{ PathCapitalizationType.LMFile, PathCapitalization.Normal },
				{ PathCapitalizationType.LevelFile, PathCapitalization.None },
			},
			HasDeformations = true
		};
		public static CPA_Settings VIPPS1 => new CPA_Settings(EngineVersion.VIP_PS1, Platform.PS1) {
			StaticListType = LST2_ListType.OptimizedArray,
			Encryption = Encryption.ReadInit,
			Luminosity = 0.5f,
			Saturate = true,
			AITypes = new AI_Types_R2_PC(),
			COLTypes = new COL_Types_R2(),
			HasDeformations = true,
			EntryActionsCount = 1
		};
		#endregion

		#region Other
		public static CPA_Settings RedPlanetPC => new CPA_Settings(EngineVersion.RedPlanet, Platform.PC) {
			EntryActionsCount = 31,
			StaticListType = LST2_ListType.SingleLinked,
			AITypes = new AI_Types_R2_PC(),
			SNATypes = new SNA_Types_R2_Demo(),
			Encryption = Encryption.RedPlanet,
			Luminosity = 0.5f,
			Saturate = true,
			HasMemorySupport = false,
			HasExtraInputData = true,
			LinkUncategorizedObjectsToScriptFamily = true,
			PathCapitalization = new Dictionary<PathCapitalizationType, PathCapitalization>() {
				{ PathCapitalizationType.FixRelocation, PathCapitalization.AllExceptExtension },
				{ PathCapitalizationType.LevelRelocation, PathCapitalization.AllExceptExtension },
			}
		};
		#endregion

		public static CPA_Settings GetSettings(CPA_GameMode mode) {
			CPA_Settings settings = mode switch {
				CPA_GameMode.Rayman2PC => R2PC,
				CPA_GameMode.Rayman2PCBeta_1998_07_22 => R2PCBeta_1998_07_22,
				CPA_GameMode.Rayman2PCDemo_1999_08_18 => R2PCDemo1,
				CPA_GameMode.Rayman2PCDemo_1999_09_04 => R2PCDemo2,
				CPA_GameMode.Rayman2DC => R2DC,
				CPA_GameMode.Rayman2DCDevBuild_1999_11_22 => R2DCDevBuild_1999_11_22,
				CPA_GameMode.Rayman2IOS => R2IOS,
				CPA_GameMode.Rayman2IOSDemo => R2IOS,
				CPA_GameMode.Rayman2PS1 => R2PS1,
				CPA_GameMode.Rayman2PS1Demo => R2PS1,
				CPA_GameMode.Rayman2PS1Demo_SLUS_90095 => R2PS1,
				CPA_GameMode.Rayman2PS2 => R2PS2,
				CPA_GameMode.Rayman2N64 => R2N64,
				CPA_GameMode.TonicTroubleN64 => TTN64,
				CPA_GameMode.Rayman2DS => R2DS,
				CPA_GameMode.Rayman23DS => R23DS,
				CPA_GameMode.RaymanMPC => RMPC,
				CPA_GameMode.RaymanMPS2 => RMPS2,
				CPA_GameMode.RaymanMPS2Demo_2001_07_25 => RMPS2Demo,
				CPA_GameMode.RaymanArenaPC => RAPC,
				CPA_GameMode.RaymanArenaPS2 => RAPS2,
				CPA_GameMode.RaymanArenaGC => RAGC,
				CPA_GameMode.RaymanArenaGCDemo_2002_03_07 => RAGCDemo,
				CPA_GameMode.RaymanArenaXbox => RAXbox,
				CPA_GameMode.RaymanRushPS1 => RRushPS1,
				CPA_GameMode.Rayman3PC => R3PC,
				CPA_GameMode.Rayman3PCDemo_2002_10_01 => R3PCDemo20021001,
				CPA_GameMode.Rayman3PCDemo_2002_10_21 => R3PCDemo20021021,
				CPA_GameMode.Rayman3PCDemo_2002_12_09 => R3PCDemo20021209,
				CPA_GameMode.Rayman3PCDemo_2003_01_06 => R3PCDemo20030106,
				CPA_GameMode.Rayman3MacOS => R3MacOS,
				CPA_GameMode.Rayman3GC => R3GC,
				CPA_GameMode.Rayman3PS2 => R3PS2,
				CPA_GameMode.Rayman3PS2Demo_2002_05_17 => R3PS2Demo_20020517,
				CPA_GameMode.Rayman3PS2Demo_2002_08_07 => R3PS2Demo_20020807,
				CPA_GameMode.Rayman3PS2DevBuild_2002_09_06 => R3PS2DevBuild,
				CPA_GameMode.Rayman3PS2Demo_2002_10_29 => R3PS2Demo_20021029,
				CPA_GameMode.Rayman3PS2Demo_2002_12_18 => R3PS2Demo_20021218,
				CPA_GameMode.Rayman3Xbox => R3Xbox,
				CPA_GameMode.Rayman3Xbox360 => R3Xbox360,
				CPA_GameMode.Rayman3PS3 => R3PS3,
				CPA_GameMode.RaymanRavingRabbidsDS => RRRDS,
				CPA_GameMode.RaymanRavingRabbidsDSDevBuild_2006_05_25 => RRRDS_20060525,
				CPA_GameMode.TonicTroublePC => TTPC,
				CPA_GameMode.TonicTroubleSEPC => TTSEPC,
				CPA_GameMode.DonaldDuckPC => DDPC,
				CPA_GameMode.DonaldDuckPCDemo => DDPCDemo,
				CPA_GameMode.DonaldDuckDC => DDDC,
				CPA_GameMode.DonaldDuckN64 => DDN64,
				CPA_GameMode.DonaldDuckPS1 => DDPS1,
				CPA_GameMode.DonaldDuckPKGC => DDPKGC,
				CPA_GameMode.DonaldDuckPKPS2 => DDPKPS2,
				CPA_GameMode.PlaymobilHypePC => PlaymobilHypePC,
				CPA_GameMode.PlaymobilLauraPC => PlaymobilLauraPC,
				CPA_GameMode.PlaymobilLauraPCPentiumIII => PlaymobilLauraPC_PentiumIII,
				CPA_GameMode.PlaymobilAlexPC => PlaymobilAlexPC,
				CPA_GameMode.DinosaurPC => DinosaurPC,
				CPA_GameMode.LargoWinchPC => LargoWinchPC,
				CPA_GameMode.VIPPS1 => VIPPS1,
				CPA_GameMode.JungleBookPS1 => JungleBookPS1,
				CPA_GameMode.RedPlanetPC => RedPlanetPC,

				_ => throw new Exception($"CPA_GameMode value {mode} is not a valid CPA mode")
			};
			settings.Mode = mode;
			settings.Init();
			return settings;
		}
	}
}
