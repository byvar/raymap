﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_R2_DS_EU : AI_Types_R2_PC {

		#region Procedures
		protected override void InitProcedures() {
			var strProcs = new string[] {
				"Proc_SetHitPoints", // 0
                "Proc_SetHitPointsInit",
				"Proc_SetHitPointsToInit",
				"Proc_SetHitPointsToMax",
				"Proc_AddHitPoints",
				"Proc_SubHitPoints", // 5
                "Proc_SetHitPointsMax",
				"Proc_TransparentDisplay",
				"Proc_SetTransparency",
				"Proc_AddHitPointsMax",
				"Proc_SubHitPointsMax", // 10
                "Proc_TransparentDisplay",
				"Proc_SetTransparency",
				"Proc_ACT_SetDrawFlag",
				"Proc_ModuleTransparentDisplay",
				"Proc_ModuleTransparentDisplay2", // 15
                "Proc_SetModuleTransparency",
				"Proc_SetModuleTransparency2",
				"Proc_ListAffectWithPersoZDD",
				"Proc_ListAffectWithModuleZDD",
				"Proc_ListAffectWithPersoZDE", // 20
                "Proc_ListAffectWithModuleZDE",
				"Proc_ListAffectWithPersoTypeZDE",
				"Proc_ListAffectWithModuleTypeZDE",
				"Proc_ListAffectTypeZDEWithTypeZDE",
                // list procedure:
                "Proc_AddPersoInList", // 25
                "Proc_AddPersoInListAt",
				"Proc_DeletePersoAtInList",
				"Proc_FindPersoAndDeleteInList",
				"Proc_Select",
				"Proc_UnSelect", // 30
                "Proc_ListSort",
				"Proc_ListSortByFamily",
				"Proc_ListSortByModele",
				"Proc_FillListWithAllPersoOfAFamily",
				"Proc_FillListWithAllPersoOfAModel", // 35
                "Proc_None",
				"Proc_ChangeComport",
				"Proc_ChangeComportReflex",
				"Proc_ChangeMyComport",
				"Proc_ChangeMyComportReflex", // 70
                "Proc_ChangeMyComportAndMyReflex",
				"Proc_ChangeAction",
				"FOG_Proc_SetColor",
				"FOG_Proc_SetNearFarInf",
				"Proc_ChangeActionWithEvents",
				"FOG_Proc_RestoreFog",
				"FOG_Proc_SaveFog",
				"Procedure_Magnet_ActiveMagnet",
				"Procedure_Magnet_DeactiveMagnet",
				"Procedure_Magnet_SetStrength", // 50
                "Procedure_Magnet_SetFar",
				"Proc_IgnoreTraceFlagForNextPicking",
				"Procedure_Magnet_SetDuration",
				"Proc_FootPath_AddFootPrint",
				"Proc_FootPath_Clear", // 55
                "SinusEffect_SetFreq",
				"SinusEffect_SetAmplitude",
				"Proc_FactorAnimationFrameRate",
				"SinusEffect_SetFreq",
				"SinusEffect_SetRLIParams", // 60
                "SinusEffect_SetRLIParams",
				"Proc_SPO_SetDrawFlag",
				"Proc_SPO_SetEngineDisplayModeFlag",
				"Proc_DeactivateBut",
				"Proc_ActivateBut", // 65
                "Proc_None",
				"ActivateChannel",
				"DeactivateChannel",
				"Proc_ChangeMyComport",
				"Proc_ChangeMyComportReflex", // 70
                "Proc_ChangeMyComportAndMyReflex",
				"Proc_ChangeAction",
				"Proc_ChangeActionForce",
				"SOUND_SendSoundRequest",
				"Proc_ChangeActionWithEvents", // 75
                "Proc_Loop",
				"Proc_EndLoop",
				"Proc_Break",
                // MiscNoProcedure
                "Proc_PlayerIsDead",
				"Proc_RestoreCardParameters", // 80
                "Proc_BreakAI",
				"Proc_IgnoreTraceFlagForNextPicking",
                // MiscProcedure
                "Proc_SetMainActor",
				"Proc_ActivateObject",
				"Proc_DesactivateObject", // 85
                "Proc_ChangeMap",
				"Proc_SetActionReturn",
				"GMT_SetVisualGMTAsChromed",
                // MiscUltraProcedure
                "GMT_SetVisualGMTFrame",
				"Proc_PlayerIsDeadWithPlacement", // 90
                "fn_p_stMiscProcedure",
				"fn_p_stMiscMoreProcedure",
				"Proc_SwapLinkTableObjects",
				"Proc_ChangeCurrentObjectTable",
				"BuildObjectTableFromTableAndString",
				"BuildObjectTableFromFormattedString",
				"MOD_TakeModuleControl",
				"MOD_TakeManyModulesControl",
				"MOD_ReleaseModuleControl",
				"GAME_CopyGame",
				"Proc_RotatePersoAroundZ",
				"ActivateChannel",
				"DeactivateChannel",
				"ACT_PersoLightOn",
				"ACT_PersoLightOff",
				"ACT_SetPersoLightColor",
				"ACT_SetPersoLightNearFar",
				"ACT_SetPersoLightLittleBigAlpha",
				"Proc_ZoomPerso",
				"ACT_SetPersoLightPulse",
				"ACT_SetPersoLightParalleleType",
				"ACT_SetPersoLightSphericalType",
				"ACT_SetPersoLightHotSpotType",
				"ACT_SetPersoLightAmbientType",
				"SOUND_SendSoundRequest",
				"SOUND_SendVoiceRequest",
				"SOUND_SendMusicRequest",
				"SOUND_SendAmbianceRequest",
				"SOUND_SendMenuSndRequest",
				"DefaultDisplay",
				"VIG_DisplayVignette",
				"VIG_DeleteVignette",
				"PRT_SetParticleGeneratorOn",
				"PRT_SetParticleGeneratorOff",
				"PRT_SetParticleGenerator",
				"PRT_SetGenerationModeNone",
				"PRT_SetGenerationModeContinuous",
				"PRT_SetGenerationModeCrenel",
				"PRT_SetGenerationModeProbability",
				"PRT_SetGenerationNbConstant",
				"PRT_SetGenerationNbProbabilist",
				"PRT_SetInfiniteLifeTime",
				"PRT_SetConstantLifeTime",
				"PRT_SetProbabilistLifeTime",
				"ACT_Turn",
				"ACT_Turn2",
				"ACT_DeltaTurnPerso",
				"ACT_TurnPerso",
				"fn_p_stKillPersoAndClearVariableProcedure1",
				"fn_p_stKillPersoAndClearVariableProcedure2",
				"GMT_SetVisualGMTColor",
				"GMT_SetVisualGMTSpecularExponant",
				"Proc_ResetLastCollisionActor",
				"Proc_ClearCollisionReport",
				"Proc_SetGoThroughMechanicsHandling",
				"Proc_EraseLastGoThroughMaterial",
				"GMT_SetVisualGMTTextureScrollingCoef",
				"GMT_LockVisualGMT",
				"GMT_UnlockVisualGMT",
				"GMT_SetVisualGMTFrame",
				"OPTION_ChangeDetailsValue",
				"OPTION_ChangeMusicVolume",
				"OPTION_ChangeSfxVolume",
				"OPTION_ChangeVoiceVolume",
				"OPTION_ChangeActionKey",
				"fn_p_stOptionChangerProc",
				"ACT_SetPersoAbsolutePosition",
				"ACT_SetPersoAtModulePosition",
				"Proc_ForcePersoAveragePosition", // dynam procedure
                "Proc_RelativeMovePerso",
				"Proc_ChangePersoAnySighting",
				"Proc_ChangePersoSightingWithOffset",
				"Proc_RotatePersoAroundX",
				"Proc_RotatePersoAroundY",
				"Proc_RotatePersoAroundZ",
				"Proc_RotatePersoAroundVector",
				"Proc_RotatePersoAroundVectorWithOffset",
				"Proc_RotatePersoAroundXwithOffset",
				"Proc_RotatePersoAroundYwithOffset",
				"Proc_RotatePersoAroundZwithOffset",
				"Proc_SetFullPersoOrientation",
				"Proc_SetFullPersoOrientationWithOffset",
				"Proc_ZoomPerso",
				"Proc_StopMoveLimitXYZ",
				"Proc_ChangeManyCustomBits",
                // mechanic speed functions
                "Proc_ImposeSpeed",
				"Proc_ImposeSpeedX",
				"Proc_ImposeSpeedY",
				"Proc_ImposeSpeedZ",
				"Proc_ImposeSpeedXY",
				"Proc_ImposeSpeedXYZ",
				"fn_p_SetMechanicSpeedVector",
				"Proc_ImposeAbsoluteSpeed",
				"Proc_ImposeAbsoluteSpeedX",
				"Proc_ImposeAbsoluteSpeedY",
				"Proc_FixePositionZDM",
				"Proc_ImposeAbsoluteSpeedXY",
				"ACT_ChangeSpoFlag",
				"Proc_ProposeSpeed",
				"Proc_ProposeSpeedX",
				"Proc_ProposeSpeedY",
				"SHADOW_Display",
				"Proc_ProposeSpeedXY",
				"Proc_ProposeSpeedXYZ",
				"Proc_ProposeAbsoluteSpeed",
				"Proc_ProposeAbsoluteSpeedX",
				"Proc_ProposeAbsoluteSpeedY",
				"Proc_ProposeAbsoluteSpeedZ",
				"Proc_ProposeAbsoluteSpeedXYZ",
				"Proc_FixePositionPerso",
				"Proc_AddSpeed",
				"Proc_AddSpeedX",
				"Proc_AddSpeedY",
				"Proc_AddSpeedZ",
				"Proc_AddSpeedXY",
				"Proc_AddSpeedXYZ",
				"Proc_AddAbsoluteSpeed",
				"Proc_AddAbsoluteSpeedX",
				"Proc_AddAbsoluteSpeedY",
				"Proc_AddAbsoluteSpeedZ",
				"Proc_AddAbsoluteSpeedXY",
				"Proc_AddAbsoluteSpeedXYZ",
				"PAD_ReadAnalogJoystickMarioMode",
				"Proc_StopMorph",
				"Proc_Morphing",
				"Proc_ReleaseAllModulesControl",
				"Proc_ChangeModuleSighting",
				"Proc_SetModuleAbsolutePosition",
				"Proc_RelativeMoveModule",
				"Proc_ChangeModuleSightingWithOffset",
				"Proc_RotateModuleAroundX",
				"Proc_RotateModuleAroundY",
				"Proc_RotateModuleAroundZ",
				"Proc_RotateModuleAroundVector",
				"Proc_RotateModuleAroundVectorOffset",
				"Proc_RotateModuleAroundXwithOffset",
				"Proc_RotateModuleAroundYwithOffset",
				"Proc_RotateModuleAroundZwithOffset",
				"Proc_SetFullModuleOrientation",
				"Proc_SetFullModuleOrientationOffset",
				"Proc_ZoomModule",
				"Proc_SetColliderType",
				"Proc_SetColliderVector",
				"Proc_SetColliderReal",
				"Proc_ResetLastCollisionActor",
				"Proc_ClearCollisionReport",
				"Proc_SetGoThroughMechanicsHandling",
				"Proc_EraseLastGoThroughMaterial",
				"Proc_StringAddChar",
				"Proc_StringReplaceChar",
				"Proc_StringRemoveChar",
				"Proc_ChangeLanguage",
				"Proc_IntToText",
				"Proc_SetMechanicAnimation",
				"Proc_SetMechanicCollide",
				"ANI_ChangeCurrFrame",
				"Proc_SetMechanicTilt",
				"Proc_SetMechanicGI",
				"Proc_SetMechanicClimb",
				"Proc_SetMechanicOnGround",
				"Proc_SetMechanicSpider",
				"Proc_SetMechanicShoot",
				"Proc_SetMechanicSwim",
				"Proc_SetMechanicNeverFall",
				"Proc_SetMechanicCollisionControl",
				"Proc_SetMechanicKeepSpeedZ",
				"ZON_SetZDMSizeSphere",
				"Proc_SetMechanicInertia",
				"Proc_SetMechanicStream",
				"Proc_SetMechanicStickOnPlatform",
				"Proc_SetMechanicScale",
				"Proc_SetMechanicGravityFactor",
				"Proc_SetMechanicSlide",
				"Proc_SetMechanicMaxRebound",
				"Proc_SetMechanicSlopeLimit",
				"Proc_SetMechanicInertiaX",
				"Proc_SetMechanicInertiaY",
				"ACT_ChangeActorSighting",
				"fn_p_SetMechanicRealParameter",
				"Proc_SetMechanicInertiaXYZ",
				"Proc_SetMechanicTiltIntensity",
				"Proc_SetMechanicTiltInertia",
				"Proc_SetMechanicTiltOrigin",
				"Proc_SetMechanicSpeedMax",
				"Proc_SetMechanicStreamPriority",
				"Proc_SetMechanicStreamSpeed",
				"Proc_SetMechanicStreamFactor",
				"Proc_AddMechanicStreamSpeed",
				"Proc_AddMechanicStreamSpeedList",
				"Proc_MoveLimit",
				"Proc_MoveLimitX",
				"Proc_MoveLimitY",
				"Proc_MoveLimitZ",
				"Proc_MoveLimitXYZ",
				"Proc_StopMoveLimitX",
				"Proc_StopMoveLimitY",
				"Proc_StopMoveLimitZ",
				"Proc_StopMoveLimitXYZ",
				"Proc_SetPlatformLink",
				"Proc_FreePlatformLink",
				"Proc_SetScale",
				"Proc_SetSlideFactorXYZ",
				"Proc_SetSlideFactorX",
				"Proc_SetSlideFactorY",
				"Proc_SetSlideFactorZ",
				"Proc_SetClimbSpeedLimit",
				"fn_p_SetMechanicRealParameter",
				"fn_p_SetMechanicSpeedReal",
				"fn_p_SetMechanicRealParameter",
				"fn_p_SetMechanicRealParameter",
				"Proc_HierSetFather",
				"Proc_HierFreeFather",
				"Proc_HierListOfSon",
				"Proc_HierSetPlatformType",
				"Proc_HierLinkControl",
				"Proc_HierFreezeEngine",
				"Proc_FixePositionZDM",
				"Proc_FixePositionZDD",
				"Proc_FixePositionZDE",
				"Proc_ChangeLightIntensity",
				"Proc_SPO_ChangeFlag",
				"ACT_ChangeSpoFlag",
				"SCREEN_ChangeSize",
				"SCREEN_ChangeClip",
				"SCREEN_ChangePos",
				"Cam_ChangeConstants",
				"SCREEN_ChangeProportion",
				"SHADOW_Display",
				"SHADOW_ChangeHeight",
				"SHADOW_ChangeVectorProjection",
				"ZON_ForceActivationZDD",
				"Cam_ForceBestPos",
				"ZON_ForceActivationZDE",
				"ZON_ForceActivationZDR",
				"ZON_ForceDesactivationZDD",
				"ZON_ForceDesactivationZDM",
				"ZON_ForceDesactivationZDE",
				"ZON_ForceDesactivationZDR",
				"ZON_ReleaseActivationZDD",
				"ZON_ReleaseActivationZDM",
				"ZON_ReleaseActivationZDE",
				"ZON_ReleaseActivationZDR",
				"ZON_ActiveZDR",
				"ACT_SetNoColWithMap",
				"ACT_SetNoColWithProjectile",
				"ACT_SetNoColWithSeconfCharact",
				"ACT_SetNoColWithMainCharact",
				"ACT_SetNoColWithOtherSectors",
				"ACT_SetNoColZdeWithProjectile",
				"ACT_SetCollComputeFreq",
				"ACT_SetBrainComputeFreq",
				"ACT_SetLightComputeFreq",
				"MAP_SetUsedExitIdentifier",
				"MAP_GetUsedExitIdentifier",
				"MAP_SetGlobalCounter",
				"PAD_ReadAnalogJoystickMarioMode",
				"Proc_SetPadReadingDsgvars",
				"Proc_CancelKeyboardInertia",
				"Proc_SetPadCalibration",
				"fn_p_stSetAnalogJoystickAxes",
				"ACT_SetBooleanInArray",
				"TEXT_SetDefaultFormatCharacter",
				"VID_SelectDriver",
				"Proc_SelectShapnessMax_Dummy",
				"Proc_CenterScreen_Dummy",
				"VID_SelectResolution",
				"VID_ChangeBrightness",
				"TEXT_IntegerParameter",
				"TEXT_RealParameter",
				"VID_SetTextureFiltering",
				"VID_SetAntiAliasing",
				"SOUND_SetInStereoMode",
				"fn_p_stSetPrevMusicFadeOut",
				"Proc_SetSaturationBackGroundDistance",
				"Proc_SaveCurrentRequest",
				"Proc_RestoreRequest",
				"Proc_DrawSegment_Dummy",
				"Proc_UpdateChecksum",
				"script_proc_sub_470210",
				"Proc_FixePointsDeMagie",
				"Proc_FixePointsDeMagieMax",
				"Proc_InitPointsDeMagie",
				"Proc_InitPointsDeMagieMax",
				"Proc_AddMagicPoints",
				"Proc_AddMagicPointsMax",
				"Proc_SubMagicPoints",
				"Proc_SubMagicPointsMax",
				"Proc_FixePointsDair",
				"Proc_FixePointsDairMax",
				"Proc_InitPointsDair",
				"Proc_InitPointsDairMax",
				"Proc_AddAirPoints",
				"Proc_AddAirPointsMax",
				"Proc_SubAirPoints",
				"Proc_SubAirPointsMax",
				"Proc_FixePositionFade",
				"Proc_SetLocalPersoLight",
				"Proc_SetStaticLightOnOff",
				"Proc_SetStaticLightNearFar",
				"Proc_SetStaticLightColor",
				"Proc_ComputeLightEffect",
				"Proc_RLIPasDeRLI",
				"Proc_RLIFixe",
				"Proc_RLIBlend",
				"Proc_ChangeTypeOfWP",
				"CAPS_SetCapabilities",
				"CAPS_AddCapabilities",
				"CAPS_SubCapabilities",
				"CAPS_ChangeCapabilities",
				"NETWORK_ReinitGraph",
				"MAT_SetScrollSpeed",
				"MAT_SetScrollOnOff",
				"MAT_SetTextureOffset",
				"MAT_SetScrollPause",
				"ANI_ChangeCurrFrame",
				"ANI_ChangeRandomFrame",
				"ANI_FreezeAnim",
				"ANI_UnFreezeAnim",
				"SPO_SetSuperimposed",
				"SPO_SetSuperimposed2",
				"SPO_ReleaseSuperimposed",
				"SPO_SetCoordinates",
				"SPO_SwitchSuperimposedTab",
				"ACT_SetTractionFactor",
				"ACT_TurnToPositionAngle",
				"ACT_TurnToPositionSpeed",
				"ARRAY_Sort",
				"OPTION_ActiveJoystickAnal",
				"OPTION_UnActiveJoystickAnal",
				"ZON_SetZDMSizeSphere",
				"ZON_SetZDDSizeCone",
				"SOUND_SetVolumeAnim",
				"SOUND_SetVolumeMusic",
				"SOUND_SetVolumeAmbiance",
				"SOUND_SetDopplerEffect",
				"SOUND_PauseSound",
				"SOUND_ResumeSound",
				"SOUND_StopMenuSound",
				"TEXT_ChangeCharactere",
				"TEXT_FormatText",
				"MNU_ValidateSlotName",
				"fn_p_stCode4RestoreNomDuSlot",
				"ACT_ChangeActorSighting",
				"DEM_PlayDemo",
				"PAD_InitKeyBoardDirections",
				"PAD_SetCenterPosition",
				"PAD_SetMaximalValues",
				"ACT_ChangeTooFarLimit",
				"ACT_ChangeTransparencyZone",
				"Effect_SetBaseFrequenceForMenu",
				"Effect_SaveSinusContext",
				"Effect_RestoreSinusContext",
				"TEXT_SuperImposed",
				"Cam_UpdatePosition",
				"Cam_ChangeShiftTarget",
				"Cam_ChangeShiftPos",
				"Cam_ChangeDistMin",
				"Cam_ChangeDistMax",
				"Cam_ChangeBoundDistMin",
				"Cam_ChangeBoundDistMax",
				"Cam_ChangeAngleAlpha",
				"Cam_ChangeAngleShiftAlpha",
				"Cam_ChangeAngleTheta",
				"Cam_ChangeAngleShiftTheta",
				"Cam_ChangeLinearSpeed",
				"Cam_ChangeLinearIncreaseSpeed",
				"Cam_ChangeLinearDecreaseSpeed",
				"Cam_ChangeAngularSpeed",
				"Cam_ChangeAngularIncreaseSpeed",
				"Cam_ChangeAngularDecreaseSpeed",
				"Cam_ChangeTargetSpeed",
				"Cam_ChangeTargetIncreaseSpeed",
				"Cam_ChangeTargetDecreaseSpeed",
				"Cam_ChangeFocal",
				"Cam_ChangeZMin",
				"Cam_ChangeZMax",
				"Cam_ChangeTgtPerso",
				"Cam_ChangeSecondTgtPerso",
				"Cam_ChangeChannel",
				"Cam_Activate",
				"Cam_AssociateViewport",
				"Cam_ResetIAFlags",
				"Cam_SetFlagNoDynamicTarget",
				"Cam_SetFlagNoAverageMoveTgtPerso",
				"Cam_SetFlagNoParseCutAngle",
				"Cam_SetFlagNoVisibility",
				"Cam_SetFlagNoVisibilityWithDynHie",
				"Cam_SetFlagNoDynChangeTheta",
				"Cam_SetFlagNoShiftUntilPosReached",
				"Cam_SetFlagNoDynSpeed",
				"Cam_ResetDNMFlags",
				"Cam_SetFlagNoLinearParsing",
				"Cam_SetFlagNoLinearInertia",
				"Cam_SetFlagNoAngularParsing",
				"Cam_SetFlagNoAngularInertia",
				"Cam_SetFlagNoTargetParsing",
				"Cam_SetFlagNoTargetInertia",
				"Cam_SetFlagFixedOrientation",
				"Cam_SetFlagNoObstacle",
				"CAM_fn_p_stSetDNMFlags",
				"Cam_ChangeConstants",
				"Cam_SaveConstants",
				"Cam_RestoreConstants",
				"Cam_ShowInfo",
				"Cam_ForceTarget",
				"Cam_ForcePosition",
				"Cam_ForceRefAxis",
				"Cam_Reset",
				"Cam_ForceBestPos",
				"Cam_ForceNormalState",
				"Cam_ForceMovingOnRail",
				"Cam_SetCameraModeForEngine",
				"SetAGO",
				"ActivateMenuMap",
				"fn_p_stJFFTXTProcedure",
				"fn_p_stJFFTXTProcedure",
				"fn_p_stJFFTXTProcedure",
				"fn_p_stDummy", // Fade in?
                "fn_p_stDummy", // Fade out?
                "fn_p_stCheatCodeProcedure",
				"fn_p_stCheatCodeProcedure",
				"fn_p_stCheatCodeProcedure",
				"fn_p_stCheatCodeProcedure",
			};
		}
		#endregion

		// Re-checked
		// Commented with //NU//: never used

		// InterpretTypes list: same as PC
		// MetaActions list: same as PC

		#region DsgVar Types
		protected override void InitVariableTypes() {
			VariableTypes = new AI_DsgVarType[] {
				AI_DsgVarType.Boolean,
				AI_DsgVarType.SByte,
				AI_DsgVarType.UByte,
				AI_DsgVarType.Short,
				AI_DsgVarType.UShort,
				AI_DsgVarType.Int,
				AI_DsgVarType.UInt,
				AI_DsgVarType.Float,
				AI_DsgVarType.WayPoint,
				AI_DsgVarType.Perso,
				AI_DsgVarType.List,
				AI_DsgVarType.Vector,
				AI_DsgVarType.Comport,
				AI_DsgVarType.Action,
				AI_DsgVarType.Text,
				AI_DsgVarType.GameMaterial,
				AI_DsgVarType.Caps,
				AI_DsgVarType.Graph,
				AI_DsgVarType.PersoArray,
				AI_DsgVarType.VectorArray,
				AI_DsgVarType.FloatArray,
				AI_DsgVarType.IntegerArray,
				AI_DsgVarType.WayPointArray,
				AI_DsgVarType.TextArray,
				AI_DsgVarType.None
                // 24, the above, seems to be an array on 3DS. Doesn't seem to be treated at all on DS
                //24 on PC is DsgVarInfoEntry.DsgVarType.SuperObject and input on iOS.
            };
		}
		#endregion

		#region Keywords
		protected override void InitKeywords() {
			Keywords = new AI_Keyword[] {
				AI_Keyword.If,
				AI_Keyword.IfNot,
				AI_Keyword.If2,
				AI_Keyword.If4,
				AI_Keyword.If8,
				AI_Keyword.If16,
				AI_Keyword.IfDebug,
				AI_Keyword.IfNotU64,
				AI_Keyword.Then,
				AI_Keyword.Else,
				AI_Keyword.EngineGoto,
				AI_Keyword.Me,
				AI_Keyword.MainActor,
				AI_Keyword.Nobody,
				AI_Keyword.Nowhere,
				AI_Keyword.EmptyText,
				AI_Keyword.CapsNull,
				AI_Keyword.NoGraph,
				AI_Keyword.NoAction,
			};
		}
		#endregion

		#region Conditions
		protected override void InitConditions() {
			Conditions = new AI_Condition[] {
				AI_Condition.Et,
				AI_Condition.Ou,
				AI_Condition.Not,
				//NU//AI_Condition.XOr,
				AI_Condition.Equal,
				AI_Condition.Different,
				AI_Condition.Lesser,
				AI_Condition.Greater,
				AI_Condition.LesserOrEqual,
				AI_Condition.GreaterOrEqual,
				
				AI_Condition.CollidePersoZDDNoWithPerso,
				//NU//AI_Condition.CollideModuleZDDNoWithPerso,
				AI_Condition.CollidePersoAllZDDWithPerso,
				//NU//AI_Condition.CollidePersoZDDWithAnyPerso,
				//NU//AI_Condition.CollideModuleZDDWithAnyPerso,
				
				AI_Condition.CollidePersoZDENoWithPersoZDENo,
				//NU//AI_Condition.CollideModuleZDENoWithPersoZDENo,
				//NU//AI_Condition.CollidePersoZDENoWithModuleZDENo,
				//NU//AI_Condition.CollideModuleZDENoWithModuleZDENo,
				AI_Condition.CollidePersoZDENoWithPersoTypeZDE,
				//NU//AI_Condition.CollideModuleZDENoWithPersoTypeZDE,
				//NU//AI_Condition.CollidePersoTypeZDEWithPersoTypeZDE,
				AI_Condition.CollidePersoAllZDEWithPersoAllZDE,
				//NU//AI_Condition.CollidePersoTypeZDEWithPersoAllZDE,
				AI_Condition.CollidePersoAllZDEWithPersoTypeZDE,
				//NU//AI_Condition.CollidePersoZDENoWithTypeZDE,
				//NU//AI_Condition.CollideModuleZDENoWithTypeZDE,

				AI_Condition.CollideWithGround,
				AI_Condition.CollideWithWall,
				AI_Condition.CollideWithNothing,
				AI_Condition.CollideWithCeiling,
				AI_Condition.CollideWithPerso,
				AI_Condition.CollideWithWater,
				AI_Condition.CollideWithThisPerso,
				AI_Condition.ZDMCollideWithGround,
				//NU//AI_Condition.ZDMCollideWithWall,
				//NU//AI_Condition.ZDMCollideWithNothing,
				//NU//AI_Condition.ZDMCollideWithCeiling,
				AI_Condition.IsPersoInList,
				//NU//AI_Condition.IsModelInList,
				//NU//AI_Condition.IsFamilyInList,
				AI_Condition.ListEmptyTest,
				AI_Condition.UserEvent_IsSet,
				AI_Condition.UserEvent_IsSet2,
				AI_Condition.PressedBut,
				AI_Condition.JustPressedBut,
				AI_Condition.ReleasedBut,
				AI_Condition.JustReleasedBut,
				AI_Condition.IsTimeElapsed,
				AI_Condition.IsValidObject,
				AI_Condition.IsValidWayPoint,
				//NU//AI_Condition.IsValidGMT,
				//NU//AI_Condition.IsValidAction,
				AI_Condition.IsValidText,
				AI_Condition.SeePerso,
				AI_Condition.IsActivable,
				AI_Condition.IsAlreadyHandled,
				AI_Condition.Alw_IsMine,
				//NU//AI_Condition.IsPersoLightOn,
				//NU//AI_Condition.IsPersoLightPulseOn,
				//NU//AI_Condition.IsPersoLightGyroPhareOn,
				AI_Condition.IsZDMCollideWithObstacle,
				AI_Condition.IsZDMCollideWithWall,
				AI_Condition.IsZDMCollideWithGround,
				AI_Condition.IsZDMCollideWithCeiling,
				AI_Condition.CmtIdentifierContainsMask,
				AI_Condition.HitByCollider,
				//NU//AI_Condition.IsTypeOfGMTCollide,
				AI_Condition.IsInComport,
				AI_Condition.IsInReflexComport,
				AI_Condition.IsInAction,
				AI_Condition.ChangeActionEnable,
				AI_Condition.EngineReinitRequested,
				//NU//AI_Condition.IsThereMechEvent,
				AI_Condition.CollisionWP,
				AI_Condition.IsCustomBitSet,
				AI_Condition.IsPersoActive,
				AI_Condition.CheckActionEnd,
				AI_Condition.IsCurrentStateCustomBitSet,

				AI_Condition.IsGiBlock,
				AI_Condition.IsMechanicBlock,
				//NU//AI_Condition.IsMechanicAnimation,
				//NU//AI_Condition.IsMechanicCollide,
				//NU//AI_Condition.IsMechanicGravity,
				//NU//AI_Condition.IsMechanicTilt,
				//NU//AI_Condition.IsMechanicGi,
				//NU//AI_Condition.IsMechanicClimb,
				//NU//AI_Condition.IsMechanicOnGround,
				//NU//AI_Condition.IsMechanicSpider,
				//NU//AI_Condition.IsMechanicShoot,
				//NU//AI_Condition.IsMechanicSwim,
				//NU//AI_Condition.IsMechanicNeverFall,
				//NU//AI_Condition.IsMechanicCollisionControl,
				//NU//AI_Condition.IsMechanicKeepSpeedZ,
				//NU//AI_Condition.IsMechanicSpeedLimit,
				//NU//AI_Condition.IsMechanicInertia,
				AI_Condition.IsMechanicStream,
				//NU//AI_Condition.IsMechanicStickOnPlatform,
				//NU//AI_Condition.IsMechanicPatformCrash,
				//NU//AI_Condition.IsMechanicScale,
				//NU//AI_Condition.IsMechanicExec,
				AI_Condition.CanFall,
				AI_Condition.IsMechanicCrash,

				AI_Condition.IsNullVector,
				AI_Condition.HierIsSonOfActor,
				//NU//AI_Condition.IsMorphing,
				AI_Condition.CheckAnimEnd,
				AI_Condition.HasTheCapability,
				//NU//AI_Condition.HasOneOfTheCapabilities,
				AI_Condition.HasTheCapabilityNumber,
				//NU//AI_Condition.PersoHasTheCapability,
				//NU//AI_Condition.PersoHasOneOfTheCapabilities,
				//NU//AI_Condition.PersoHasTheCapabilityNumber,
				//NU//AI_Condition.MagnetIsActivated,
				//NU//AI_Condition.NEstPasEnCollisionAvecMap,
				//NU//AI_Condition.NEstPasEnCollisionAvecProjectile,
				//NU//AI_Condition.NEstPasEnCollisionAvecSecondCharact,
				//NU//AI_Condition.NEstPasEnCollisionAvecMainCharact,
				//NU//AI_Condition.NEstPasEnCollisionAvecAutresSecteurs,
				AI_Condition.IsInFamily,
				AI_Condition.IsInModel,

				//NU//AI_Condition.AJoypadIsConnected,
				AI_Condition.AKeyJustPressed,
				AI_Condition.AButtonPadJustPressed,
				AI_Condition.IsInDemoMode,
				AI_Condition.IsInStereoMode,
				AI_Condition.IsMusicPlaying,
				AI_Condition.IsShapnessMax,

				AI_Condition.IsSlotDataCorrupt,
				AI_Condition.IsCheatMenu,
				//AI_Condition.IsUSBuild, // only in R2 DS US

				//NU//AI_Condition.Cam_IsActive,
				//NU//AI_Condition.Cam_IsViewportOwner,
				//NU//AI_Condition.Cam_IsFlagNoDynamicTarget,
				//NU//AI_Condition.Cam_IsFlagNoAverageMoveTgtPerso,
				//NU//AI_Condition.Cam_IsFlagNoParseCutAngle,
				//NU//AI_Condition.Cam_IsFlagNoVisibility,
				//NU//AI_Condition.Cam_IsFlagNoVisibilityWithDynHie,
				//NU//AI_Condition.Cam_IsFlagNoDynChangeTheta,
				//NU//AI_Condition.Cam_IsFlagNoShiftUntilPosReached,
				//NU//AI_Condition.Cam_IsFlagNoDynSpeed,
				//NU//AI_Condition.Cam_IsFlagNoLinearParsing,
				//NU//AI_Condition.Cam_IsFlagNoLinearInertia,
				//NU//AI_Condition.Cam_IsFlagNoAngularParsing,
				//NU//AI_Condition.Cam_IsFlagNoAngularInertia,
				//NU//AI_Condition.Cam_IsFlagNoTargetParsing,
				//NU//AI_Condition.Cam_IsFlagNoTargetInertia,
				//NU//AI_Condition.Cam_IsFlagNoObstacle,
				//NU//AI_Condition.Cam_IsFlagFixedOrientation,
				AI_Condition.Cam_IsFlagForcedPosition,
				//NU//AI_Condition.Cam_IsFlagForcedTarget,
				//NU//AI_Condition.Cam_IsFlagForcedAxis,
			};
		}
		#endregion

		#region Fields
		protected override void InitFields() {
			Fields = new AI_Field[] {
				AI_Field.Position,
				AI_Field.Orientation,
				AI_Field.Speed,
				AI_Field.NormSpeed,
				AI_Field.AbsoluteAxisX,
				AI_Field.AbsoluteAxisY,
				AI_Field.AbsoluteAxisZ,
				AI_Field.PrevComportIntell,
				AI_Field.PrevComportReflex,
				AI_Field.ShadowScaleX,
				AI_Field.ShadowScaleY,
			};
		}
		#endregion

		#region Functions
		protected override void InitFunctions() {
			Functions = new AI_Function[] {
				AI_Function.GetPersoAbsolutePosition,
				AI_Function.GetMyAbsolutePosition,
				AI_Function.GetAngleAroundZToPerso,
				AI_Function.DistanceToPerso,
				AI_Function.DistanceXToPerso,
				AI_Function.DistanceYToPerso,
				AI_Function.DistanceZToPerso,
				AI_Function.DistanceXYToPerso,
				AI_Function.DistanceXZToPerso,
				//NU//AI_Function.DistanceYZToPerso,
				AI_Function.DistanceToPersoCenter,
				//NU//AI_Function.DistanceXToPersoCenter,
				AI_Function.DistanceYToPersoCenter,
				AI_Function.DistanceZToPersoCenter,
				AI_Function.DistanceXYToPersoCenter,
				//NU//AI_Function.DistanceXZToPersoCenter,
				//NU//AI_Function.DistanceYZToPersoCenter,
				AI_Function.DistanceToWP,
				AI_Function.GetWPAbsolutePosition,
				AI_Function.Int,
				AI_Function.RandomInt,
				AI_Function.Real,
				AI_Function.Sinus,
				AI_Function.Cosinus,
				AI_Function.Square,
				AI_Function.SquareRoot,
				AI_Function.RandomReal,
				AI_Function.MinimumReal,
				AI_Function.MaximumReal,
				AI_Function.DegreeToRadian,
				AI_Function.RadianToDegree,
				AI_Function.AbsoluteValue,
				AI_Function.LimitRealInRange,
				AI_Function.Sign,
				//NU//AI_Function.Cube,
				AI_Function.Modulo,
				AI_Function.TernInf,
				AI_Function.TernSup,
				AI_Function.TernEq,
				AI_Function.TernInfEq,
				AI_Function.TernSupEq,
				AI_Function.TernOp,
				AI_Function.TemporalRealCombination,
				AI_Function.GetHitPoints,
				//NU//AI_Function.AddAndGetHitPoints,
				AI_Function.SubAndGetHitPoints,
				AI_Function.GetHitPointsMax,
				AI_Function.AddAndGetHitPointsMax,
				//NU//AI_Function.SubAndGetHitPointsMax,
				AI_Function.ListSize,
				AI_Function.GivePersoInList,
				AI_Function.AbsoluteVector,
				AI_Function.RelativeVector,
				AI_Function.VecteurLocalToGlobal,
				AI_Function.VecteurGlobalToLocal,
				//NU//AI_Function.GetMagnetStrength,
				//NU//AI_Function.GetMagnetFar,
				//NU//AI_Function.GetMagnetNear,
				//NU//AI_Function.GetMagnetDuration,
				//NU//AI_Function.SPO_GetDrawFlag,
				AI_Function.GetTime,
				AI_Function.GetElapsedTime,
				AI_Function.GetDeltaT,
				AI_Function.GetFrameLength,
				AI_Function.InputAnalogicValue,
				//NU//AI_Function.VitessePadAnalogique,
				AI_Function.GenerateObject,
				AI_Function.CountGeneratedObjects,
				AI_Function.GetGlobalCounter,
				//NU//AI_Function.GetSubMapId,
				//NU//AI_Function.AddColor,
				//NU//AI_Function.AddRed,
				//NU//AI_Function.AddGreen,
				//NU//AI_Function.AddBlue,
				//NU//AI_Function.AddAlpha,
				AI_Function.ColorRedGreenBlueAlpha,
				AI_Function.ColorRedGreenBlue,
				//NU//AI_Function.ColorRed,
				//NU//AI_Function.ColorGreen,
				//NU//AI_Function.ColorBlue,
				//NU//AI_Function.ColorAlpha,
				//NU//AI_Function.GetVisualGMTColor,
				//NU//AI_Function.GetVisualGMTSpecularCoef,
				//NU//AI_Function.GetVisualGMTSpecularExponent,
				//NU//AI_Function.GetVisualGMTDiffuseCoef,
				//NU//AI_Function.GetVisualGMTAmbientCoef,
				//NU//AI_Function.GetVisualGMTTextureScrollingCoefU,
				//NU//AI_Function.GetVisualGMTTextureScrollingCoefV,
				//NU//AI_Function.GetVisualGMTFrame,
				//NU//AI_Function.GetVisualGMTNumberOfFrames,
				AI_Function.SaveGame,
				AI_Function.LoadGame,
				AI_Function.EraseGame,
				AI_Function.GetCurrentSlot,
				AI_Function.IsAValidSlotName,
				AI_Function.RepairCorruptSlot,

				// FuncRay2
				//NU//AI_Function.LitPointsDeMagie,
				//NU//AI_Function.LitPointsDeMagieMax,
				//NU//AI_Function.AjouteEtLitPointsDeMagie,
				//NU//AI_Function.AjouteEtLitPointsDeMagieMax,
				//NU//AI_Function.EnleveEtLitPointsDeMagie,
				//NU//AI_Function.EnleveEtLitPointsDeMagieMax,
				AI_Function.LitPointsDair,
				AI_Function.LitPointsDairMax,
				//NU//AI_Function.AjouteEtLitPointsDair,
				//NU//AI_Function.AjouteEtLitPointsDairMax,
				AI_Function.EnleveEtLitPointsDair,
				//NU//AI_Function.EnleveEtLitPointsDairMax,
				//NU//AI_Function.PersoLePlusProche,
				//NU//AI_Function.PersoLePlusProcheDansSecteurCourant,
				//NU//AI_Function.NearerActorInFieldOfVision,
				//NU//AI_Function.GetNbActivePerso,
				AI_Function.CibleLaPlusProche,
				AI_Function.CibleLaPlusProcheAvecAngles,
				AI_Function.ReseauWPLePlusProche,
				AI_Function.NetworkCloserWPOfType,
				AI_Function.ReseauWPLePlusDansLAxe,
				//NU//AI_Function.ReseauWPLePlusDansLAxe2,
				AI_Function.NetworkNextWPWithCapa,
				//NU//AI_Function.NetworkAffectTypeOfConnectedWP,
				//NU//AI_Function.NetworkAffectTypeOfConnectedWPWithCapa,
				AI_Function.ReseauCheminLePlusCourt,
				AI_Function.NetworkBuildOrderedPath,
				AI_Function.NetworkBuildOrderedPathCircular,
				AI_Function.NetworkAllocateGraphToMSWay,
				//NU//AI_Function.NetworkAllocateGraphToMSWayCircular,
				AI_Function.ReseauLitIndexCourant,
				AI_Function.ReseauForceIndexCourant,
				AI_Function.ReseauLitPremierIndex,
				AI_Function.ReseauLitDernierIndex,
				AI_Function.ReseauIncrementIndex,
				AI_Function.ReseauDecrementIndex,
				AI_Function.ReseauLitWPAIndex,
				AI_Function.ReseauLitCapaciteLiaisonAIndex,
				//NU//AI_Function.ReseauChangeCapaciteLiaisonAIndex,
				//NU//AI_Function.ReseauLitPoidsLiaisonAIndex,
				//NU//AI_Function.ReseauChangePoidsLiaisonAIndex,
				AI_Function.NetworkGetIndexOfWPInMSWay,
				AI_Function.NetworkForceWPToCurrent,
				//NU//AI_Function.NetworkTestTheEnds,
				AI_Function.ReseauLitCapaciteLiaisonDansGraph,
				AI_Function.ReseauChangeCapaciteLiaisonDansGraph,
				//NU//AI_Function.ReseauLitPoidsLiaisonDansGraph,
				//NU//AI_Function.ReseauChangePoidsLiaisonDansGraph,
				AI_Function.NetworkGetTypeOfWP,
				//NU//AI_Function.GetCapabilities,
				//NU//AI_Function.CapabilityAtBitNumber,
				AI_Function.GetScrollSpeed,
				AI_Function.GetNbFrame,
				AI_Function.DotProduct,
				AI_Function.CrossProduct,
				AI_Function.Normalize,
				//NU//AI_Function.GetSPOCoordinates,
				AI_Function.GetTractionFactor,
				AI_Function.GetCenterZDEType,
				//NU//AI_Function.GetCenterZDMType,
				//NU//AI_Function.GetCenterZDRType,
				//NU//AI_Function.GetCenterZDDType,
				AI_Function.TextAffiche,
				//NU//AI_Function.GetCPUCounter,

				// DefFunc1
				//NU//AI_Function.VitesseHorizontaleDuPerso,
				//NU//AI_Function.VitesseVerticaleDuPerso,
				AI_Function.GetPersoZoomFactor,
				AI_Function.GetPersoSighting,
				AI_Function.GetPersoHorizon,
				AI_Function.GetPersoBanking,
				AI_Function.LitPositionZDM,
				AI_Function.LitPositionZDE,
				//NU//AI_Function.LitPositionZDD,
				//NU//AI_Function.LitCentreZDM,
				//NU//AI_Function.LitCentreZDE,
				AI_Function.LitCentreZDD,
				//NU//AI_Function.LitAxeZDM,
				//NU//AI_Function.LitAxeZDE,
				//NU//AI_Function.LitAxeZDD,
				AI_Function.LitDimensionZDM,
				//NU//AI_Function.LitDimensionZDE,
				AI_Function.LitDimensionZDD,
				//NU//AI_Function.VecteurPointAxe, // Was not NeverUsed in Rayman_Z!
				AI_Function.VecteurPointSegment,
				AI_Function.VectorContribution,
				AI_Function.VectorCombination,
				AI_Function.TemporalVectorCombination,
				AI_Function.ScaledVector,
				AI_Function.GetVectorNorm,
				AI_Function.RotateVector,
				AI_Function.VectorAngle,
				AI_Function.VectorCos,
				//NU//AI_Function.VectorSin,
				AI_Function.GetNormalCollideVector,
				AI_Function.GetNormalCollideVector2,
				AI_Function.GetCollidePoint,
				AI_Function.GetCollidePoint2,
				AI_Function.GetHandsCollidePoint,
				//NU//AI_Function.GetCollideRate,
				AI_Function.GetCollideRate2,
				AI_Function.GetCollideMaterialType,
				AI_Function.GetCollideMaterialType2,
				AI_Function.GetCollisionPoint,
				AI_Function.GetCollisionVector,
				//NU//AI_Function.GetCollisionPerso, // Was not NeverUsed in Rayman_Z!
				AI_Function.GetCollisionPointMaterial,
				//NU//AI_Function.GetLastTraversedMaterialType,
				//NU//AI_Function.GetLastTraversedMaterial,
				//NU//AI_Function.GetCurrentCollidedGMT,
				AI_Function.GetColliderType,
				AI_Function.GetColliderVector,
				AI_Function.GetColliderReal,
				AI_Function.GetLastCollisionActor,
				AI_Function.ComputeRebondVector,
				AI_Function.GetModuleAbsolutePosition,
				AI_Function.GetModuleRelativePosition,
				//NU//AI_Function.GetModuleZoomFactor,
				AI_Function.GetModuleSighting,
				AI_Function.CastIntegerToChannel,
				AI_Function.GetSlotDate,
				AI_Function.GetSlotName,
				AI_Function.GetSlotScore,
				AI_Function.GetStringCharAt,
				//NU//AI_Function.GetFormattedTextInfo,
				//NU//AI_Function.GetInputEntryName,
				AI_Function.GetMechanicGravityFactor,
				//NU//AI_Function.GetMechanicSlide,
				//NU//AI_Function.GetMechanicRebound,
				//NU//AI_Function.GetMechanicSlopeLimit,
				//NU//AI_Function.GetMechanicInertiaX,
				//NU//AI_Function.GetMechanicInertiaY,
				//NU//AI_Function.GetMechanicInertiaZ,
				//NU//AI_Function.GetMechanicTiltIntensity,
				//NU//AI_Function.GetMechanicTiltInertia,
				//NU//AI_Function.GetMechanicTiltOrigin,
				//NU//AI_Function.GetMechanicMaxSpeed,
				//NU//AI_Function.GetMechanicStreamPriority,
				//NU//AI_Function.GetMechanicStreamSpeed,
				//NU//AI_Function.GetMechanicStreamFactor,
				//NU//AI_Function.GetSlideFactorX,
				//NU//AI_Function.GetSlideFactorY,
				//NU//AI_Function.GetSlideFactorZ,
				AI_Function.JumpImpulsion,
				AI_Function.GetSpeedAnim,
				AI_Function.HierGetFather,
				//NU//AI_Function.GetActivationZDD,
				//NU//AI_Function.GetActivationZDM,
				//NU//AI_Function.GetActivationZDE,
				//NU//AI_Function.GetActivationZDR,
				//NU//AI_Function.GetCollisionFrequency,
				AI_Function.GetBrainFrequency,
				//NU//AI_Function.GetLightFrequency,
				AI_Function.GetBooleanInArray,
				AI_Function.GetNumberOfBooleanInArray,
				//NU//AI_Function.GetButtonName,
				//NU//AI_Function.GetDriversAvailable,
				AI_Function.GetCurrentLanguageId,
				//NU//AI_Function.GetNbLanguages,
				//NU//AI_Function.GetLanguageText,
				//NU//AI_Function.TextToInt,
				AI_Function.GetMusicVolume,
				AI_Function.GetSfxVolume,
				AI_Function.SlotIsValid,
				AI_Function.NbAvailableResolution,
				AI_Function.CurrentResolution,
				AI_Function.GetBrightness,
				//NU//AI_Function.NameResolution,
				AI_Function.GetNbSlotsAvailable,
				//NU//AI_Function.GetTextureFiltering,
				//NU//AI_Function.GetAntiAliasing,
				//NU//AI_Function.GetSaturationDistance,
				//NU//AI_Function.GetBackgroundDistance,
				AI_Function.GetTooFarLimit,
				AI_Function.GetTransparencyZoneMin,
				AI_Function.GetTransparencyZoneMax,
				AI_Function.ExecuteVariable,
				AI_Function.ComputeProtectKey,
				AI_Function.Xor,
				AI_Function.DivUnsigned,
				AI_Function.MulUnsigned,
				AI_Function.AddUnsigned,
				AI_Function.SubUnsigned,
				AI_Function.GetMemoryValue,

				// DS exclusive
				AI_Function.GetCheats,
				AI_Function.GetBacklight,
				AI_Function.DoneAnalogCalibration,

				// DefFunCa
				AI_Function.Cam_GetShiftTarget,
				AI_Function.Cam_GetShiftPos,
				AI_Function.Cam_GetDistMin,
				AI_Function.Cam_GetDistMax,
				//NU//AI_Function.Cam_GetBoundDistMin,
				//NU//AI_Function.Cam_GetBoundDistMax,
				//NU//AI_Function.Cam_GetAngleAlpha,
				//NU//AI_Function.Cam_GetAngleShiftAlpha,
				AI_Function.Cam_GetAngleTheta,
				//NU//AI_Function.Cam_GetAngleShiftTheta,
				//NU//AI_Function.Cam_GetLinearSpeed,
				//NU//AI_Function.Cam_GetLinearIncreaseSpeed,
				//NU//AI_Function.Cam_GetLinearDecreaseSpeed,
				//NU//AI_Function.Cam_GetAngularSpeed,
				//NU//AI_Function.Cam_GetAngularIncreaseSpeed,
				//NU//AI_Function.Cam_GetAngularDecreaseSpeed,
				//NU//AI_Function.Cam_GetTargetSpeed,
				//NU//AI_Function.Cam_GetTargetIncreaseSpeed,
				//NU//AI_Function.Cam_GetTargetDecreaseSpeed,
				AI_Function.Cam_GetFocal,
				//NU//AI_Function.Cam_GetZMin,
				//NU//AI_Function.Cam_GetZMax,
				//NU//AI_Function.Cam_GetTargetedSuperObject,
				//NU//AI_Function.Cam_GetTypeOfViewport,
				//NU//AI_Function.Cam_GetCameraOfViewport,
				//NU//AI_Function.Cam_GetMainCamera,
				AI_Function.Cam_ComputeTargetWithTgtPerso,
				AI_Function.Cam_GetCurrentTargetPosition,
				//NU//AI_Function.Cam_GetSectorCameraType,
				AI_Function.Cam_GetBestPos,
			};
		}
		#endregion

	}

}
