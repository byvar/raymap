﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_R2_PC_Demo1 : AI_Types_R2_PC {

		#region Procedures
		protected override void InitProcedures() {
			procedureTable = new string[] {
				"Proc_SetHitPoints", // 0
                "Proc_SetHitPointsInit",
				"Proc_SetHitPointsToInit",
				"Proc_SetHitPointsToMax",
				"Proc_AddHitPoints",
				"Proc_SubHitPoints", // 5
                "Proc_SetHitPointsMax",
				"Proc_SetHitPointsMaxToInit",
				"Proc_SetHitPointsMaxToMax",
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
                "Proc_DeleteFamilyInList",
				"Proc_DeleteModelInList",
                // list ensemble procedure
                "Proc_ListUnion",
				"Proc_ListInter",
				"Proc_ListDiff", // 40
                "Proc_ListAdd",
				"FOG_Proc_Activate",
				"FOG_Proc_SetColor",
				"FOG_Proc_SetNearFarInf",
				"FOG_Proc_SetBlend", // 45
                "FOG_Proc_RestoreFog",
				"FOG_Proc_SaveFog",
				"Procedure_Magnet_ActiveMagnet",
				"Procedure_Magnet_DeactiveMagnet",
				"Procedure_Magnet_SetStrength", // 50
                "Procedure_Magnet_SetFar",
				"Procedure_Magnet_SetNear",
				"Procedure_Magnet_SetDuration",
				"Proc_FootPath_AddFootPrint",
				"Proc_FootPath_Clear", // 55
                "SinusEffect_SetFreq",
				"SinusEffect_SetAmplitude",
				"SinusEffect_SetState",
				"SinusEffect_SetFreq",
				"SinusEffect_SetRLIParams", // 60
                "SinusEffect_SetRLIParams",
				"Proc_SPO_SetDrawFlag",
				"Proc_SPO_SetEngineDisplayModeFlag",
				"Proc_DeactivateBut",
				"Proc_ActivateBut", // 65
                "Proc_None",
				"Proc_ChangeComport",
				"Proc_ChangeComportReflex",
				"Proc_ChangeMyComport",
				"Proc_ChangeMyComportReflex", // 70
                "Proc_ChangeMyComportAndMyReflex",
				"Proc_ChangeAction",
				"Proc_ChangeActionForce",
				"Proc_ChangeActionRandom",
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
				"Proc_FactorAnimationFrameRate",
                // MiscUltraProcedure
                "Proc_ForcePersoHandling",
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
				"GAME_QuitGame",
				"ActivateChannel",
				"DeactivateChannel",
				"ACT_PersoLightOn",
				"ACT_PersoLightOff",
				"ACT_SetPersoLightColor",
				"ACT_SetPersoLightNearFar",
				"ACT_SetPersoLightLittleBigAlpha",
				"ACT_SetPersoLightGyrophare",
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
				"GMT_SetVisualGMTSpecularCoef",
				"GMT_SetVisualGMTDiffuseCoef",
				"GMT_SetVisualGMTAmbientCoef",
				"GMT_SetVisualGMTAsChromed",
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
				"Proc_ChangeOneCustomBit",
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
				"Proc_ImposeAbsoluteSpeedZ",
				"Proc_ImposeAbsoluteSpeedXY",
				"Proc_ImposeAbsoluteSpeedXYZ",
				"Proc_ProposeSpeed",
				"Proc_ProposeSpeedX",
				"Proc_ProposeSpeedY",
				"Proc_ProposeSpeedZ",
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
				"Proc_PrepareMorph",
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
				"Proc_SetMechanicGravity",
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
				"Proc_SetMechanicSpeedLimit",
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
				"Proc_SetMechanicInertiaZ",
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
				"SCREEN_ChangePosPercent",
				"SCREEN_ChangeProportion",
				"SHADOW_Display",
				"SHADOW_ChangeHeight",
				"SHADOW_ChangeVectorProjection",
				"ZON_ForceActivationZDD",
				"ZON_ForceActivationZDM",
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
				"fn_p_stCheatCodeProcedure"
			};
		}
		#endregion

		// Re-checked

		// DsgVarTypes list: same as PC
		// InterpretTypes list: same as PC
		// Keywords list: same as PC
		// Conditions list: same as PC
		// Operators list: same as PC
		// MetaActions list: same as PC

		#region Fields
		protected override void InitFields() { // Matches DS/N64 version
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
				AI_Function.DistanceYZToPerso,
				AI_Function.DistanceToPersoCenter,
				AI_Function.DistanceXToPersoCenter,
				AI_Function.DistanceYToPersoCenter,
				AI_Function.DistanceZToPersoCenter,
				AI_Function.DistanceXYToPersoCenter,
				AI_Function.DistanceXZToPersoCenter,
				AI_Function.DistanceYZToPersoCenter,
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
				AI_Function.Cube,
				AI_Function.Modulo,
				AI_Function.TernInf,
				AI_Function.TernSup,
				AI_Function.TernEq,
				AI_Function.TernInfEq,
				AI_Function.TernSupEq,
				AI_Function.TernOp,
				AI_Function.TemporalRealCombination,
				AI_Function.GetHitPoints,
				AI_Function.AddAndGetHitPoints,
				AI_Function.SubAndGetHitPoints,
				AI_Function.GetHitPointsMax,
				AI_Function.AddAndGetHitPointsMax,
				AI_Function.SubAndGetHitPointsMax,
				AI_Function.ListSize,
				AI_Function.GivePersoInList,
				AI_Function.AbsoluteVector,
				AI_Function.RelativeVector,
				AI_Function.VecteurLocalToGlobal,
				AI_Function.VecteurGlobalToLocal,
				AI_Function.GetMagnetStrength,
				AI_Function.GetMagnetFar,
				AI_Function.GetMagnetNear,
				AI_Function.GetMagnetDuration,
				AI_Function.SPO_GetDrawFlag,
				AI_Function.GetTime,
				AI_Function.GetElapsedTime,
				AI_Function.GetDeltaT,
				AI_Function.GetFrameLength,
				AI_Function.InputAnalogicValue,
				AI_Function.VitessePadAnalogique,
				AI_Function.GenerateObject,
				AI_Function.CountGeneratedObjects,
				AI_Function.GetGlobalCounter,
				AI_Function.GetSubMapId,
				AI_Function.AddColor,
				AI_Function.AddRed,
				AI_Function.AddGreen,
				AI_Function.AddBlue,
				AI_Function.AddAlpha,
				AI_Function.ColorRedGreenBlueAlpha,
				AI_Function.ColorRedGreenBlue,
				AI_Function.ColorRed,
				AI_Function.ColorGreen,
				AI_Function.ColorBlue,
				AI_Function.ColorAlpha,
				AI_Function.GetVisualGMTColor,
				AI_Function.GetVisualGMTSpecularCoef,
				AI_Function.GetVisualGMTSpecularExponent,
				AI_Function.GetVisualGMTDiffuseCoef,
				AI_Function.GetVisualGMTAmbientCoef,
				AI_Function.GetVisualGMTTextureScrollingCoefU,
				AI_Function.GetVisualGMTTextureScrollingCoefV,
				AI_Function.GetVisualGMTFrame,
				AI_Function.GetVisualGMTNumberOfFrames,
				AI_Function.SaveGame,
				AI_Function.LoadGame,
				AI_Function.EraseGame,
				AI_Function.GetCurrentSlot,
				AI_Function.IsAValidSlotName,
				AI_Function.LitPointsDeMagie,
				AI_Function.LitPointsDeMagieMax,
				AI_Function.AjouteEtLitPointsDeMagie,
				AI_Function.AjouteEtLitPointsDeMagieMax,
				AI_Function.EnleveEtLitPointsDeMagie,
				AI_Function.EnleveEtLitPointsDeMagieMax,
				AI_Function.LitPointsDair,
				AI_Function.LitPointsDairMax,
				AI_Function.AjouteEtLitPointsDair,
				AI_Function.AjouteEtLitPointsDairMax,
				AI_Function.EnleveEtLitPointsDair,
				AI_Function.EnleveEtLitPointsDairMax,
				AI_Function.PersoLePlusProche,
				AI_Function.PersoLePlusProcheDansSecteurCourant,
				AI_Function.NearerActorInFieldOfVision,
				AI_Function.GetNbActivePerso,
				AI_Function.CibleLaPlusProche,
				AI_Function.CibleLaPlusProcheAvecAngles,
				AI_Function.ReseauWPLePlusProche,
				AI_Function.NetworkCloserWPOfType,
				AI_Function.ReseauWPLePlusDansLAxe,
				AI_Function.ReseauWPLePlusDansLAxe2,
				AI_Function.NetworkNextWPWithCapa,
				AI_Function.NetworkAffectTypeOfConnectedWP,
				AI_Function.NetworkAffectTypeOfConnectedWPWithCapa,
				AI_Function.ReseauCheminLePlusCourt,
				AI_Function.NetworkBuildOrderedPath,
				AI_Function.NetworkBuildOrderedPathCircular,
				AI_Function.NetworkAllocateGraphToMSWay,
				AI_Function.NetworkAllocateGraphToMSWayCircular,
				AI_Function.ReseauLitIndexCourant,
				AI_Function.ReseauForceIndexCourant,
				AI_Function.ReseauLitPremierIndex,
				AI_Function.ReseauLitDernierIndex,
				AI_Function.ReseauIncrementIndex,
				AI_Function.ReseauDecrementIndex,
				AI_Function.ReseauLitWPAIndex,
				AI_Function.ReseauLitCapaciteLiaisonAIndex,
				AI_Function.ReseauChangeCapaciteLiaisonAIndex,
				AI_Function.ReseauLitPoidsLiaisonAIndex,
				AI_Function.ReseauChangePoidsLiaisonAIndex,
				AI_Function.NetworkGetIndexOfWPInMSWay,
				AI_Function.NetworkForceWPToCurrent,
				AI_Function.NetworkTestTheEnds,
				AI_Function.ReseauLitCapaciteLiaisonDansGraph,
				AI_Function.ReseauChangeCapaciteLiaisonDansGraph,
				AI_Function.ReseauLitPoidsLiaisonDansGraph,
				AI_Function.ReseauChangePoidsLiaisonDansGraph,
				AI_Function.NetworkGetTypeOfWP,
				AI_Function.GetCapabilities,
				AI_Function.CapabilityAtBitNumber,
				AI_Function.GetScrollSpeed,
				AI_Function.GetNbFrame,
				AI_Function.DotProduct,
				AI_Function.CrossProduct,
				AI_Function.Normalize,
				AI_Function.GetSPOCoordinates,
				AI_Function.GetTractionFactor,
				AI_Function.GetCenterZDEType,
				AI_Function.GetCenterZDMType,
				AI_Function.GetCenterZDRType,
				AI_Function.GetCenterZDDType,
				AI_Function.TextAffiche,
				AI_Function.GetCPUCounter,
				AI_Function.VitesseHorizontaleDuPerso,
				AI_Function.VitesseVerticaleDuPerso,
				AI_Function.GetPersoZoomFactor,
				AI_Function.GetPersoSighting,
				AI_Function.GetPersoHorizon,
				AI_Function.GetPersoBanking,
				AI_Function.LitPositionZDM,
				AI_Function.LitPositionZDE,
				AI_Function.LitPositionZDD,
				AI_Function.LitCentreZDM,
				AI_Function.LitCentreZDE,
				AI_Function.LitCentreZDD,
				AI_Function.LitAxeZDM,
				AI_Function.LitAxeZDE,
				AI_Function.LitAxeZDD,
				AI_Function.LitDimensionZDM,
				AI_Function.LitDimensionZDE,
				AI_Function.LitDimensionZDD,
				AI_Function.VecteurPointAxe,
				AI_Function.VecteurPointSegment,
				AI_Function.VectorContribution,
				AI_Function.VectorCombination,
				AI_Function.TemporalVectorCombination,
				AI_Function.ScaledVector,
				AI_Function.GetVectorNorm,
				AI_Function.RotateVector,
				AI_Function.VectorAngle,
				AI_Function.VectorCos,
				AI_Function.VectorSin,
				AI_Function.GetNormalCollideVector,
				AI_Function.GetNormalCollideVector2,
				AI_Function.GetCollidePoint,
				AI_Function.GetCollidePoint2,
				AI_Function.GetHandsCollidePoint,
				AI_Function.GetCollideRate,
				AI_Function.GetCollideRate2,
				AI_Function.GetCollideMaterialType,
				AI_Function.GetCollideMaterialType2,
				AI_Function.GetCollisionPoint,
				AI_Function.GetCollisionVector,
				AI_Function.GetCollisionPerso,
				AI_Function.GetCollisionPointMaterial,
				AI_Function.GetLastTraversedMaterialType,
				AI_Function.GetLastTraversedMaterial,
				AI_Function.GetCurrentCollidedGMT,
				AI_Function.GetColliderType,
				AI_Function.GetColliderVector,
				AI_Function.GetColliderReal,
				AI_Function.GetLastCollisionActor,
				AI_Function.ComputeRebondVector,
				AI_Function.GetModuleAbsolutePosition,
				AI_Function.GetModuleRelativePosition,
				AI_Function.GetModuleZoomFactor,
				AI_Function.GetModuleSighting,
				AI_Function.CastIntegerToChannel,
				AI_Function.GetSlotName,
				AI_Function.GetSlotScore,
				AI_Function.GetStringCharAt,
				AI_Function.GetFormattedTextInfo,
				AI_Function.GetInputEntryName,
				AI_Function.GetMechanicGravityFactor,
				AI_Function.GetMechanicSlide,
				AI_Function.GetMechanicRebound,
				AI_Function.GetMechanicSlopeLimit,
				AI_Function.GetMechanicInertiaX,
				AI_Function.GetMechanicInertiaY,
				AI_Function.GetMechanicInertiaZ,
				AI_Function.GetMechanicTiltIntensity,
				AI_Function.GetMechanicTiltInertia,
				AI_Function.GetMechanicTiltOrigin,
				AI_Function.GetMechanicMaxSpeed,
				AI_Function.GetMechanicStreamPriority,
				AI_Function.GetMechanicStreamSpeed,
				AI_Function.GetMechanicStreamFactor,
				AI_Function.GetSlideFactorX,
				AI_Function.GetSlideFactorY,
				AI_Function.GetSlideFactorZ,
				AI_Function.JumpImpulsion,
				AI_Function.GetSpeedAnim,
				AI_Function.HierGetFather,
				AI_Function.GetActivationZDD,
				AI_Function.GetActivationZDM,
				AI_Function.GetActivationZDE,
				AI_Function.GetActivationZDR,
				AI_Function.GetCollisionFrequency,
				AI_Function.GetBrainFrequency,
				AI_Function.GetLightFrequency,
				AI_Function.ReadCharAtKeyPosition,
				AI_Function.ExecuteVariable,
				AI_Function.GetBooleanInArray,
				AI_Function.GetNumberOfBooleanInArray,
				AI_Function.GetButtonName,
				AI_Function.GetDriversAvailable,
				AI_Function.GetCurrentLanguageId,
				AI_Function.GetNbLanguages,
				AI_Function.GetLanguageText,
				AI_Function.TextToInt,
				AI_Function.GetMusicVolume,
				AI_Function.GetSfxVolume,
				AI_Function.SlotIsValid,
				AI_Function.NbAvailableResolution,
				AI_Function.CurrentResolution,
				AI_Function.GetBrightness,
				AI_Function.NameResolution,
				AI_Function.GetNbSlotsAvailable,
				AI_Function.GetTextureFiltering,
				AI_Function.GetAntiAliasing,
				AI_Function.GetSaturationDistance,
				AI_Function.GetBackgroundDistance,
				AI_Function.GetTooFarLimit,
				AI_Function.GetTransparencyZoneMin,
				AI_Function.GetTransparencyZoneMax,
				AI_Function.ComputeProtectKey,
				AI_Function.Xor,
				AI_Function.DivUnsigned,
				AI_Function.MulUnsigned,
				AI_Function.AddUnsigned,
				AI_Function.SubUnsigned,
				AI_Function.GetMemoryValue,
				AI_Function.Cam_GetShiftTarget,
				AI_Function.Cam_GetShiftPos,
				AI_Function.Cam_GetDistMin,
				AI_Function.Cam_GetDistMax,
				AI_Function.Cam_GetBoundDistMin,
				AI_Function.Cam_GetBoundDistMax,
				AI_Function.Cam_GetAngleAlpha,
				AI_Function.Cam_GetAngleShiftAlpha,
				AI_Function.Cam_GetAngleTheta,
				AI_Function.Cam_GetAngleShiftTheta,
				AI_Function.Cam_GetLinearSpeed,
				AI_Function.Cam_GetLinearIncreaseSpeed,
				AI_Function.Cam_GetLinearDecreaseSpeed,
				AI_Function.Cam_GetAngularSpeed,
				AI_Function.Cam_GetAngularIncreaseSpeed,
				AI_Function.Cam_GetAngularDecreaseSpeed,
				AI_Function.Cam_GetTargetSpeed,
				AI_Function.Cam_GetTargetIncreaseSpeed,
				AI_Function.Cam_GetTargetDecreaseSpeed,
				AI_Function.Cam_GetFocal,
				AI_Function.Cam_GetZMin,
				AI_Function.Cam_GetZMax,
				AI_Function.Cam_GetTargetedSuperObject,
				AI_Function.Cam_GetTypeOfViewport,
				AI_Function.Cam_GetCameraOfViewport,
				AI_Function.Cam_GetMainCamera,
				AI_Function.Cam_ComputeTargetWithTgtPerso,
				AI_Function.Cam_GetCurrentTargetPosition,
				AI_Function.Cam_GetSectorCameraType,
				AI_Function.Cam_GetBestPos,
			};
		}
		#endregion

	}

}
