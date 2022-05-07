﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_R2_PC : AI_Types {
		#region Functions
		protected override void InitFunctions() {
			functionTable = new string[] {
				"Func_GetPersoAbsolutePosition", // 0
                "Func_GetMyAbsolutePosition",
				"Func_GetAngleAroundZToPerso",
				"Func_DistanceToPerso",
				"Func_DistanceXToPerso",
				"Func_DistanceYToPerso", // 5
                "Func_DistanceZToPerso",
				"Func_DistanceXYToPerso",
				"Func_DistanceXZToPerso",
				"Func_DistanceYZToPerso",
				"Func_DistanceToPersoCenter", // 10
                "Func_DistanceXToPersoCenter",
				"Func_DistanceYToPersoCenter",
				"Func_DistanceZToPersoCenter",
				"Func_DistanceXYToPersoCenter",
				"Func_DistanceXZToPersoCenter", // 15
                "Func_DistanceYZToPersoCenters",
				"Func_DistanceToWP",
				"fn_p_stGetWpAbsolutePosition",
				"Func_Int",
				"Func_RandomInt", // 20
                "Func_Real",
				"Func_Sinus",
				"Func_Cosinus",
				"Func_Square",
				"Func_SquareRoot", // 25
                "Func_RandomReal",
				"Func_MinimumReal",
				"Func_MaximumReal",
				"Func_DegreeToRadian",
				"Func_RadianToDegree", // 30
                "Func_AbsoluteValue",
				"Func_LimitRealInRange",
				"Func_Sign",
				"Func_Cube",
				"Func_Modulo", // 35
                "Func_TernInf",
				"Func_TernSup",
				"Func_TernEq",
				"Func_TernInfEq",
				"Func_TernSupEq", // 40
                "Func_TernOp",
				"fn_p_stRealFunction",
				"Func_GetHitPoints",
				"Func_AddAndGetHitPoints",
				"Func_SubAndGetHitPoints", // 45
                "Func_GetHitPointsMax",
				"Func_AddAndGetHitPointsMax",
				"Func_SubAndGetHitPointsMax",
				"Func_ListSize",
				"Func_GivePersoInList", // 50
                "Func_AbsoluteVector",
				"Func_RelativeVector",
				"Func_VectorLocalToGlobal",
				"Func_VectorGlobalToLocal",
				"Func_MAGNETGetStrength", // 55
                "Func_MAGNETGetFar",
				"Func_MAGNETGetNear",
				"Func_MAGNETGetDuration",
				"Func_SPO_GetDrawFlag",
				"Func_GetTime", // 60
                "Func_ElapsedTime",
				"Func_GetDeltaTime",
				"Func_GetFrameLength",
				"Func_GetInputAnalogicValue",
				"Func_VitessePadAnalogique", // 65
                "Func_GenerateObject",
				"Func_CountGeneratedObjects",
				"Func_GetGlobalCounter",
				"Func_GetSubMapId",
				"Func_AddColor",
				"Func_AddRed",
				"Func_AddGreen",
				"Func_AddBlue",
				"Func_AddAlpha",
				"Func_ColorRGBA",
				"Func_ColorRGB",
				"Func_ColorRed",
				"Func_ColorGreen",
				"Func_ColorBlue",
				"Func_ColorAlpha",
				"Func_GetVisualGMTColor",
				"Func_GetVisualGMTSpecularCoef",
				"Func_GetVisualGMTSpecularExponant",
				"Func_GetVisualGMTDiffuseCoef",
				"Func_GetVisualGMTAmbientCoef",
				"Func_GetVisualGMTTextureScrollingCoefU",
				"Func_GetVisualGMTTextureScrollingCoefV",
				"Func_GetVisualGMTFrame",
				"Func_GetVisualGMTNumberOfFrames",
				"Func_SaveGame",
				"Func_LoadGame",
				"Func_EraseGame",
				"fn_p_stSaveGameFunction",
				"fn_p_stSaveGameFunction",
				"fn_p_stIsAValidSlotName",
				"RAY_GetMagicPoints",
				"RAY_GetMagicPointsMax",
				"RAY_AddAndGetMagicPoints",
				"RAY_AddAndGetMagicPointsMax",
				"RAY_RemoveAndGetMagicPoints",
				"RAY_RemoveAndGetMagicPointsMax",
				"RAY_GetAirPoints",
				"RAY_GetAirPointsMax",
				"RAY_AddAndGetAirPoints",
				"RAY_AddAndGetAirPointsMax",
				"RAY_RemoveAndGetAirPoints",
				"RAY_RemoveAndGetAirPointsMax",
				"ACT_NearestActor",
				"ACT_NearestActorInCurrentSector",
				"ACT_NearerActorInFieldOfVision",
				"Func_GetNbActivePerso",
				"Func_CibleLaPlusProche",
				"Func_CibleLaPlusProcheavecAngles",
				"NETWORK_CloserWPn",
				"NETWORK_CloserWPOfType",
				"NETWORK_CloserWPnInAxis",
				"NETWORK_CloserWPnInAxis2",
				"Func_NetworkNextWPWithCapa",
				"Func_NetworkAffectTypeOfConnectedWP",
				"fn_p_stTravelOnAGraph",
				"fn_p_stGraphToWayFunction",
				"fn_p_stGraphToWayFunction",
				"fn_p_stGraphToWayFunction",
				"fn_p_stGraphToWayFunction",
				"fn_p_stGraphToWayFunction",
				"NETWORK_GetCurrentIndex",
				"NETWORK_SetCurrentIndex",
				"NETWORK_GetFirstIndex",
				"NETWORK_GetLastIndex",
				"NETWORK_IncrementIndex",
				"NETWORK_DecrementIndex",
				"NETWORK_GetWPAIndex",
				"NETWORK_GetLinkCapacityToIndex",
				"NETWORK_ChangeLinkCapacityToIndex",
				"NETWORK_GetLinkWeightToIndex",
				"NETWORK_ChangeLinkWeightToIndex",
				"NETWORK_GetIndexOfWPInMSWay",
				"NETWORK_ForceWPToCurrent",
				"NETWORK_TestExtremities",
				"NETWORK_GetLinkCapInGraph",
				"NETWORK_SetLinkCapInGraph",
				"NETWORK_GetLinkWeightInGraph",
				"NETWORK_SetLinkWeightInGraph",
				"NETWORK_GetTypeOfWP",
				"Func_CapsGetCapabilities",
				"Func_CapabilityAtBitNumber",
				"Func_GetScrollSpeed",
				"Func_GetNbFrame",
				"Func_DotProduct",
				"Func_CrossProduct",
				"Func_Normalize",
				"Func_GetSPOCoordinates",
				"Func_ACTGetTractionFactor",
				"Func_GetCenterZDEType",
				"Func_GetCenterZDMType",
				"Func_GetCenterZDRType",
				"Func_GetCenterZDDType",
				"TEXT_DisplayText",
				"Func_GetCPUCounter",
				"ACT_HorizontalPersoSpeed",
				"ACT_VerticalPersoSpeed",
				"Func_GetPersoZoomFactor", // 3d func
                "Func_GetPersoSighting",
				"Func_GetPersoHorizon", // 3d func
                "Func_GetPersoBanking", // 3d func
                "ZON_GetZDMPosition",
				"ZON_GetZDEPosition",
				"ZON_GetZDDPosition",
				"ZON_GetZDMCenter",
				"ZON_GetZDECenter",
				"ZON_GetZDDCenter",
				"ZON_GetZDMAxis",
				"ZON_GetZDEAxis",
				"ZON_GetZDDAxis",
				"ZON_GetZDMDimension",
				"ZON_GetZDEDimension",
				"ZON_GetZDDDimension",
				"VEC_PointAxisVector",
				"VEC_PointSegmentVector",
				"VEC_VectorContribution",
				"VEC_VectorCombination",
				"VEC_TemporalVectorCombination",
				"VEC_ScaledVector",
				"VEC_GetVectorNorm",
				"VEC_RotateVector",
				"VEC_AngleVector",
				"VEC_CosVector",
				"VEC_SinVector",
				"Func_GetNormalCollideVector",
				"Func_GetNormalCollideVector2",
				"COL_GetCollidePoint",
				"COL_GetCollidePoint2",
				"COL_GetHandsCollidePoint",
				"fn_p_stGetCollidePoint_194",
				"fn_p_stGetCollidePoint_195",
				"fn_p_stGetCollidePoint_196",
				"fn_p_stGetCollidePoint_197",
				"fn_p_stGetCollision",
				"fn_p_stGetCollision",
				"fn_p_stGetCollision",
				"Func_CollisionPointMaterial",
				"Func_GetLastTraversedMaterialType", // GameMaterial function
                "Func_GetLastTraversedMaterial",
				"Func_GetCurrentCollidedGMT",
				"Func_GetColliderType", // collidecommunicationfunc
                "Func_GetColliderVector", // collidecommunicationfunc
                "Func_GetColliderReal", // collidecommunicationfunc
                "fn_p_stGetLastCollisionActor",
				"fn_p_stComputeRebondVector",
				"MOD_GetModuleAbsolutePosition",
				"MOD_GetModuleRelativePosition  ",
				"MOD_GetModuleZoomFactor",
				"MOD_GetModuleSighting",
				"MOD_CastIntegerToChannel",
				"Func_GetSlotName", // names func
                "Func_GetStringCharAt", // names func
                "Func_GetFormattedTextInfo", // names func
                "Func_GetInputEntryName", // names func
                "fn_p_stNamesFunc",
				"fn_p_stNamesFunc",
				"Proc_GetMechanicGravityFactor",
				"Proc_GetMechanicSlide",
				"Proc_GetMechanicRebound",
				"Proc_GetMechanicSlopeLimit",
				"Proc_GetMechanicInertiaX",
				"Proc_GetMechanicInertiaY",
				"Proc_GetMechanicInertiaZ",
				"Proc_GetMechanicTiltIntensity",
				"Proc_GetMechanicTiltInertia",
				"Proc_GetMechanicTiltOrigin",
				"Proc_GetMechanicMaxSpeed",
				"Proc_GetMechanicStreamPriority",
				"Proc_GetMechanicStreamSpeed",
				"Proc_GetMechanicStreamFactor",
				"Proc_GetSlideFactorX",
				"Proc_GetSlideFactorY",
				"Proc_GetSlideFactorZ",
				"Proc_JumpImpulsion",
				"Proc_GetSpeedAnim",
				"HIER_GetFather",
				"ZON_GetZDDActivation", // priviliged activation
                "ZON_GetZDMActivation",
				"ZON_GetZDEActivation",
				"ZON_GetZDRActivation",
				"ACT_GetCollComputationFrequency",
				"ACT_GetBrainComputationFrequency",
				"ACT_GetLightComputationFrequency",
				"ACT_GetBooleanInArray",
				"ACT_GetNumberOfBooleanInArray",
				"BUT_GetButtonName",
				"VID_GetDriversAvailable",
				"TEXT_GetCurrentLanguageId",
				"TEXT_GetNbLanguages",
				"TEXT_GetLanguageText",
				"TEXT_TextToInt",
				"OPTION_GetMusicVolume",
				"OPTION_GetSfxVolume",
				"OPTION_SlotIsValid",
				"VID_NbAvailableResolution",
				"VID_CurrentResolution",
				"VID_SaveCurrentResolution",
				"VID_IsDisplayModeAvailable",
				"VID_GetBrightness",
				"VID_NameResolution",
				"OPTION_GetNbSlotsAvailable",
				"VID_GetTextureFiltering",
				"VID_GetAntiAliasing",
				"fn_p_stGetMSSoundValues",
				"fn_p_stGetMSSoundValues",
				"fn_p_stGetStdGameLimit",
				"fn_p_stGetStdGameLimit",
				"fn_p_stGetStdGameLimit",
				"fn_p_ExecuteVariable",
				"ACT_ComputeProtectKey",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"fn_p_stComputeNot",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"fn_p_stComputeXor",
				"Cam_GetShiftTarget",
				"Cam_GetShiftPos",
				"Cam_GetDistMin",
				"Cam_GetDistMax",
				"Cam_GetBoundDistMin",
				"Cam_GetBoundDistMax",
				"Cam_GetAngleAlpha",
				"Cam_GetAngleShiftAlpha",
				"Cam_GetAngleTheta",
				"Cam_GetAngleShiftTheta",
				"Cam_GetLinearSpeed",
				"Cam_GetLinearIncreaseSpeed",
				"Cam_GetLinearDecreaseSpeed",
				"Cam_GetAngularSpeed",
				"Cam_GetAngularIncreaseSpeed",
				"Cam_GetAngularDecreaseSpeed",
				"Cam_GetTargetSpeed",
				"Cam_GetTargetIncreaseSpeed",
				"Cam_GetTargetDecreaseSpeed",
				"Cam_GetFocal",
                //"Cam_GetZMin",
                //"Cam_GetZMax",
                "Cam_GetTargetedSuperObject",
				"Cam_GetTypeOfViewport",
				"Cam_GetCameraOfViewport",
				"Cam_GetMainCamera",
				"Cam_ComputeTargetWithTgtPerso",
				"Cam_ComputeCurrentTarget",
				"Cam_GetSectorCameraType",
				"Cam_GetBestPos"
			};
		}
		#endregion

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
				AI_DsgVarType.SuperObject
			};
		}
		#endregion

		#region Node types
		protected override void InitInterpretTypes() {
			InterpretTypes = new AI_InterpretType[] {
				AI_InterpretType.KeyWord, // 0
                AI_InterpretType.Condition,
				AI_InterpretType.Operator,
				AI_InterpretType.Function,
				AI_InterpretType.Procedure,
				AI_InterpretType.MetaAction, // 5
                AI_InterpretType.BeginMacro,
				AI_InterpretType.EndMacro,
				AI_InterpretType.EndTree,
				AI_InterpretType.Field,
				AI_InterpretType.DsgVar, // 10
                AI_InterpretType.DsgVarRef,
				AI_InterpretType.Constant,
				AI_InterpretType.Real,
				AI_InterpretType.Button,
				AI_InterpretType.ConstantVector, // 15
                AI_InterpretType.Vector,
				AI_InterpretType.Mask,
				AI_InterpretType.Module,
				AI_InterpretType.DsgVarId,
				AI_InterpretType.String, // 20
                AI_InterpretType.LipsSynchroRef,
				AI_InterpretType.FamilyRef,
				AI_InterpretType.PersoRef,
				AI_InterpretType.ActionRef,
				AI_InterpretType.SuperObjectRef, // 25
                AI_InterpretType.WayPointRef,
				AI_InterpretType.TextRef,
				AI_InterpretType.ComportRef,
				AI_InterpretType.ModuleRef,
				AI_InterpretType.SoundEventRef, // 30
                AI_InterpretType.ObjectTableRef,
				AI_InterpretType.GameMaterialRef,
				AI_InterpretType.ParticleGenerator,
				AI_InterpretType.Color,
				AI_InterpretType.ModelRef, // 35
                AI_InterpretType.Light,
				AI_InterpretType.Caps,
				AI_InterpretType.Graph,
				AI_InterpretType.MacroRef__Subroutine,
				AI_InterpretType.Null, // 40
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
				AI_Keyword.NoSuperObject,
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
				AI_Condition.XOr,
				AI_Condition.Equal,
				AI_Condition.Different,
				AI_Condition.Lesser,
				AI_Condition.Greater,
				AI_Condition.LesserOrEqual,
				AI_Condition.GreaterOrEqual,
				AI_Condition.CollidePersoZDDNoWithPerso,
				AI_Condition.CollideModuleZDDNoWithPerso,
				AI_Condition.CollidePersoAllZDDWithPerso,
				AI_Condition.CollidePersoZDDWithAnyPerso,
				AI_Condition.CollideModuleZDDWithAnyPerso,
				AI_Condition.CollidePersoZDENoWithPersoZDENo,
				AI_Condition.CollideModuleZDENoWithPersoZDENo,
				AI_Condition.CollidePersoZDENoWithModuleZDENo,
				AI_Condition.CollideModuleZDENoWithModuleZDENo,
				AI_Condition.CollidePersoZDENoWithPersoTypeZDE,
				AI_Condition.CollideModuleZDENoWithPersoTypeZDE,
				AI_Condition.CollidePersoTypeZDEWithPersoTypeZDE,
				AI_Condition.CollidePersoAllZDEWithPersoAllZDE,
				AI_Condition.CollidePersoTypeZDEWithPersoAllZDE,
				AI_Condition.CollidePersoAllZDEWithPersoTypeZDE,
				AI_Condition.CollidePersoZDENoWithTypeZDE,
				AI_Condition.CollideModuleZDENoWithTypeZDE,
				AI_Condition.CollideWithGround,
				AI_Condition.CollideWithWall,
				AI_Condition.CollideWithNothing,
				AI_Condition.CollideWithCeiling,
				AI_Condition.CollideWithPerso,
				AI_Condition.CollideWithWater,
				AI_Condition.CollideWithThisPerso,
				AI_Condition.ZDMCollideWithGround,
				AI_Condition.ZDMCollideWithWall,
				AI_Condition.ZDMCollideWithNothing,
				AI_Condition.ZDMCollideWithCeiling,
				AI_Condition.IsPersoInList,
				AI_Condition.IsModelInList,
				AI_Condition.IsFamilyInList,
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
				AI_Condition.IsValidGMT,
				AI_Condition.IsValidAction,
				AI_Condition.IsValidText,
				AI_Condition.SeePerso,
				AI_Condition.IsActivable,
				AI_Condition.IsAlreadyHandled,
				AI_Condition.Alw_IsMine,
				AI_Condition.IsPersoLightOn,
				AI_Condition.IsPersoLightPulseOn,
				AI_Condition.IsPersoLightGyroPhareOn,
				AI_Condition.IsZDMCollideWithObstacle,
				AI_Condition.IsZDMCollideWithWall,
				AI_Condition.IsZDMCollideWithGround,
				AI_Condition.IsZDMCollideWithCeiling,
				AI_Condition.CmtIdentifierContainsMask,
				AI_Condition.HitByCollider,
				AI_Condition.IsTypeOfGMTCollide,
				AI_Condition.IsInComport,
				AI_Condition.IsInReflexComport,
				AI_Condition.IsInAction,
				AI_Condition.ChangeActionEnable,
				AI_Condition.EngineReinitRequested,
				AI_Condition.IsThereMechEvent,
				AI_Condition.CollisionWP,
				AI_Condition.IsCustomBitSet,
				AI_Condition.IsPersoActive,
				AI_Condition.CheckActionEnd,
				AI_Condition.IsCurrentStateCustomBitSet,

				AI_Condition.IsGiBlock,
				AI_Condition.IsMechanicBlock,
				AI_Condition.IsMechanicAnimation,
				AI_Condition.IsMechanicCollide,
				AI_Condition.IsMechanicGravity,
				AI_Condition.IsMechanicTilt,
				AI_Condition.IsMechanicGi,
				AI_Condition.IsMechanicClimb,
				AI_Condition.IsMechanicOnGround,
				AI_Condition.IsMechanicSpider,
				AI_Condition.IsMechanicShoot,
				AI_Condition.IsMechanicSwim,
				AI_Condition.IsMechanicNeverFall,
				AI_Condition.IsMechanicCollisionControl,
				AI_Condition.IsMechanicKeepSpeedZ,
				AI_Condition.IsMechanicSpeedLimit,
				AI_Condition.IsMechanicInertia,
				AI_Condition.IsMechanicStream,
				AI_Condition.IsMechanicStickOnPlatform,
				AI_Condition.IsMechanicPatformCrash,
				AI_Condition.IsMechanicScale,
				AI_Condition.IsMechanicExec,
				AI_Condition.CanFall,
				AI_Condition.IsMechanicCrash,

				AI_Condition.IsNullVector,
				AI_Condition.HierIsSonOfActor,
				AI_Condition.IsMorphing,
				AI_Condition.CheckAnimEnd,
				AI_Condition.HasTheCapability,
				AI_Condition.HasOneOfTheCapabilities,
				AI_Condition.HasTheCapabilityNumber,
				AI_Condition.PersoHasTheCapability,
				AI_Condition.PersoHasOneOfTheCapabilities,
				AI_Condition.PersoHasTheCapabilityNumber,
				AI_Condition.MagnetIsActivated,
				AI_Condition.NEstPasEnCollisionAvecMap,
				AI_Condition.NEstPasEnCollisionAvecProjectile,
				AI_Condition.NEstPasEnCollisionAvecSecondCharact,
				AI_Condition.NEstPasEnCollisionAvecMainCharact,
				AI_Condition.NEstPasEnCollisionAvecAutresSecteurs,
				AI_Condition.IsInFamily,
				AI_Condition.IsInModel,

				AI_Condition.AJoypadIsConnected,
				AI_Condition.AKeyJustPressed,
				AI_Condition.AButtonPadJustPressed,
				AI_Condition.IsInDemoMode,
				AI_Condition.IsInStereoMode,
				AI_Condition.IsMusicPlaying,
				AI_Condition.IsShapnessMax,

				AI_Condition.Cam_IsActive,
				AI_Condition.Cam_IsViewportOwner,
				AI_Condition.Cam_IsFlagNoDynamicTarget,
				AI_Condition.Cam_IsFlagNoAverageMoveTgtPerso,
				AI_Condition.Cam_IsFlagNoParseCutAngle,
				AI_Condition.Cam_IsFlagNoVisibility,
				AI_Condition.Cam_IsFlagNoVisibilityWithDynHie,
				AI_Condition.Cam_IsFlagNoDynChangeTheta,
				AI_Condition.Cam_IsFlagNoShiftUntilPosReached,
				AI_Condition.Cam_IsFlagNoDynSpeed,
				AI_Condition.Cam_IsFlagNoLinearParsing,
				AI_Condition.Cam_IsFlagNoLinearInertia,
				AI_Condition.Cam_IsFlagNoAngularParsing,
				AI_Condition.Cam_IsFlagNoAngularInertia,
				AI_Condition.Cam_IsFlagNoTargetParsing,
				AI_Condition.Cam_IsFlagNoTargetInertia,
				AI_Condition.Cam_IsFlagNoObstacle,
				AI_Condition.Cam_IsFlagFixedOrientation,
				AI_Condition.Cam_IsFlagForcedPosition,
				AI_Condition.Cam_IsFlagForcedTarget,
				AI_Condition.Cam_IsFlagForcedAxis,
			};
		}
		#endregion

		#region Operators
		protected override void InitOperators() {
			Operators = new AI_Operator[] {
				AI_Operator.ScalarPlusScalar,
				AI_Operator.ScalarMinusScalar,
				AI_Operator.ScalarMulScalar,
				AI_Operator.ScalarDivScalar,
				AI_Operator.ScalarUnaryMinus,
				AI_Operator.PlusAffect,
				AI_Operator.MinusAffect,
				AI_Operator.MulAffect,
				AI_Operator.DivAffect,
				AI_Operator.PlusPlusAffect,
				AI_Operator.MinusMinusAffect,
				AI_Operator.Affect,
				AI_Operator.Dot,
				AI_Operator.GetVectorX,
				AI_Operator.GetVectorY,
				AI_Operator.GetVectorZ,
				AI_Operator.VectorPlusVector,
				AI_Operator.VectorMinusVector,
				AI_Operator.VectorMulScalar,
				AI_Operator.VectorDivScalar,
				AI_Operator.VectorUnaryMinus,
				AI_Operator.SetVectorX,
				AI_Operator.SetVectorY,
				AI_Operator.SetVectorZ,
				AI_Operator.Ultra,
				AI_Operator.ModelCast,
				AI_Operator.Array,
				AI_Operator.AffectArray,
			};
		}
		#endregion

		#region Meta Actions
		protected override void InitMetaActions() {
			MetaActions = new AI_MetaAction[] {
				AI_MetaAction.FrozenWait,
				AI_MetaAction.ExecuteAction,
				AI_MetaAction.WaitEndOfAction,
				AI_MetaAction.WaitEndOfAnim,
				AI_MetaAction.CamCineMoveAToBTgtC,
				AI_MetaAction.CamCineMoveAToBTgtAC,
				AI_MetaAction.CamCinePosATgtB,
				AI_MetaAction.CamCinePosAMoveTgtBToC,
				AI_MetaAction.CamCinePosATgtBTurnPosH,
				AI_MetaAction.CamCinePosATgtBTurnTgtH,
				AI_MetaAction.CamCinePosATgtBTurnPosV,
				AI_MetaAction.CamCinePosATgtBTurnTgtV
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
				AI_Field.PadGlobalVector,
				AI_Field.PadHorizontalAxis,
				AI_Field.PadVerticalAxis,
				AI_Field.PadAnalogForce,
				AI_Field.PadTrueAnalogForce,
				AI_Field.PadRotationAngle,
				AI_Field.PadSector,
			};
		}
		#endregion
	}

}
