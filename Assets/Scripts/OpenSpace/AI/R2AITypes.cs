using OpenSpace.EngineObject;
using OpenSpace.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public static class R2AITypes {

        #region Function Types
        public static List<string> functionTypes = new List<string>(new string[] {
            "evalKeyword",
            "evalCondition",
            "evalOperator",
            "evalFunction",
            "evalProcedure",
            "evalMetaAction",
            "evalBeginMacro",
            "evalBeginMacro",
            "evalEndMacro",
            "evalField",
            "evalDsgVarRef",
            "evalDsgVarRef",
            "evalConstant",
            "evalReal",
            "evalButton",
            "evalConstantVector",
            "evalVector",
            "evalMask",
            "evalModuleRef",
            "evalDsgVarId",
            "evalString",
            "evalLipsSynchroRef",
            "evalFamilyRef",
            "evalPersoRef",
            "evalActionRef",
            "evalSuperObjectRef",
            "evalWayPointRef",
            "evalTextRef",
            "evalComportRef",
            "evalModuleRef",
            "evalSoundEventRef",
            "evalObjectTableRef",
            "evalGameMaterialRef",
            "evalParticleGenerator",
            "evalVisualMaterial",
            "evalColor",
            "eval_42",
            "evalLight",
            "evalCaps",
            "evalSubRoutine",
            "evalNULL"
        });
        #endregion

        #region Keywords
        public static List<string> keywordTable = new List<string>(new string[] {
            "If",
            "IfNot",
            "If2",
            "If4",
            "If8",
            "If16",
            "IfDebug",
            "IfNotU64",
            "Then",
            "Else",
            "Goto",
            "Me",
            "MainActor",
            "Nobody",
            "NoInput",
            "Nowhere",
            "EmptyText",
            "CapsNull",
            "NoGraph",
            "NoAction"
        });
        #endregion

        #region Operators
        public static List<string> operatorTable = new List<string>(new string[] {
            "Scalar",
            "Scalar",
            "Scalar",
            "Scalar",
            "Scalar",
            "Affect",
            "Affect",
            "Affect",
            "Affect",
            "Affect",
            "Affect",
            "Affect",
            "Dot", // 12
            "VectorDot",
            "VectorDot",
            "VectorDot",
            "Vector",
            "Vector",
            "Vector",
            "Vector",
            "Vector",
            "Affect",
            "Affect",
            "Affect",
            "Ultra",
            "ModelCast",
            "Array",
            "Affect"
        });
        #endregion

        #region Functions
        public static List<string> functionTable = new List<string>(new string[] {
            "Func_GetPersoAbsolutePosition",
            "Func_GetMyAbsolutePosition",
            "Func_GetAngleAroundZToPerso",
            "Func_DistanceToPerso",
            "Func_DistanceXToPerso",
            "Func_DistanceYToPerso",
            "Func_DistanceZToPerso",
            "Func_DistanceXYToPerso",
            "Func_DistanceXZToPerso",
            "Func_DistanceYZToPerso",
            "Func_DistanceToPersoCenter",
            "Func_DistanceXToPersoCenter",
            "Func_DistanceYToPersoCenter",
            "Func_DistanceZToPersoCenter",
            "Func_DistanceXYToPersoCenter",
            "Func_DistanceXZToPersoCenter",
            "Func_DistanceYZToPersoCenters",
            "Func_DistanceToWP",
            "fn_p_stGetWpAbsolutePosition",
            "Func_Int",
            "Func_RandomInt",
            "Func_Real",
            "Func_Sinus",
            "Func_Cosinus",
            "Func_Square",
            "Func_SquareRoot",
            "Func_RandomReal",
            "Func_MinimumReal",
            "Func_MaximumReal",
            "Func_DegreeToRadian",
            "Func_RadianToDegree",
            "Func_AbsoluteValue",
            "Func_LimitRealInRange",
            "Func_Sign",
            "Func_Cube",
            "Func_Modulo",
            "Func_TernInf",
            "Func_TernSup",
            "Func_TernEq",
            "Func_TernInfEq",
            "Func_TernSupEq",
            "Func_TernOp",
            "fn_p_stRealFunction",
            "Func_GetHitPoints",
            "Func_AddAndGetHitPoints",
            "Func_SubAndGetHitPoints",
            "Func_GetHitPointsMax",
            "Func_AddAndGetHitPointsMax",
            "Func_SubAndGetHitPointsMax",
            "Func_ListSize",
            "Func_GivePersoInList",
            "Func_AbsoluteVector",
            "Func_RelativeVector",
            "Func_VectorLocalToGlobal",
            "Func_VectorGlobalToLocal",
            "Func_MAGNETGetStrength",
            "Func_MAGNETGetFar",
            "Func_MAGNETGetNear",
            "Func_MAGNETGetDuration",
            "Func_SPO_GetDrawFlag",
            "Func_GetTime",
            "Func_ElapsedTime",
            "Func_GetDeltaTime",
            "Func_GetFrameLength",
            "Func_GetInputAnalogicValue",
            "Func_VitessePadAnalogique",
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
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
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
            "Cam_GetZMin",
            "Cam_GetZMax",
            "Cam_GetTargetedSuperObject",
            "Cam_GetTypeOfViewport",
            "Cam_GetCameraOfViewport",
            "Cam_GetMainCamera",
            "Cam_ComputeTargetWithTgtPerso",
            "Cam_ComputeCurrentTarget",
            "Cam_GetSectorCameraType",
            "Cam_GetBestPos"
        });
        #endregion

        #region Procedures
        public static List<string> procedureTable = new List<string>(new string[] {
            "Proc_SetHitPoints",
            "Proc_SetHitPointsInit",
            "Proc_SetHitPointsToInit",
            "Proc_SetHitPointsToMax",
            "Proc_AddHitPoints",
            "Proc_SubHitPoints",
            "Proc_SetHitPointsMax",
            "Proc_SetHitPointsMaxToInit",
            "Proc_SetHitPointsMaxToMax",
            "Proc_AddHitPointsMax",
            "Proc_SubHitPointsMax",
            "Proc_TransparentDisplay",
            "Proc_SetTransparency",
            "Proc_ACT_SetDrawFlag",
            "Proc_ModuleTransparentDisplay",
            "Proc_ModuleTransparentDisplay2",
            "Proc_SetModuleTransparency",
            "Proc_SetModuleTransparency2",
            "Proc_ListAffectWithPersoZDD",
            "Proc_ListAffectWithModuleZDD",
            "Proc_ListAffectWithPersoZDE",
            "Proc_ListAffectWithModuleZDE",
            "Proc_ListAffectWithPersoTypeZDE",
            "Proc_ListAffectWithModuleTypeZDE",
            "Proc_ListAffectTypeZDEWithTypeZDE",
            // list procedure:
            "Proc_AddPersoInList",
            "Proc_AddPersoInListAt",
            "Proc_DeletePersoAtInList",
            "Proc_FindPersoAndDeleteInList",
            "Proc_Select", 
            "Proc_UnSelect",
            "Proc_ListSort",
            "Proc_ListSortByFamily",
            "Proc_ListSortByModele",
            "Proc_FillListWithAllPersoOfAFamily",
            "Proc_FillListWithAllPersoOfAModel",
            "Proc_DeleteFamilyInList",
            "Proc_DeleteModelInList",
            // list ensemble procedure
            "Proc_ListUnion",
            "Proc_ListInter",
            "Proc_ListDiff",
            "Proc_ListAdd",
            "FOG_Proc_Activate",
            "FOG_Proc_SetColor",
            "FOG_Proc_SetNearFarInf",
            "FOG_Proc_SetBlend",
            "FOG_Proc_RestoreFog",
            "FOG_Proc_SaveFog",
            "Procedure_Magnet_ActiveMagnet",
            "Procedure_Magnet_DeactiveMagnet",
            "Procedure_Magnet_SetStrength",
            "Procedure_Magnet_SetFar",
            "Procedure_Magnet_SetNear",
            "Procedure_Magnet_SetDuration",
            "Proc_FootPath_AddFootPrint",
            "Proc_FootPath_Clear",
            "SinusEffect_SetFreq",
            "SinusEffect_SetAmplitude",
            "SinusEffect_SetState",
            "SinusEffect_SetFreq",
            "SinusEffect_SetRLIParams",
            "SinusEffect_SetRLIParams",
            "Proc_SPO_SetDrawFlag",
            "Proc_SPO_SetEngineDisplayModeFlag",
            "Proc_DeactivateBut",
            "Proc_ActivateBut",
            "Proc_None",
            "Proc_ChangeComport",
            "Proc_ChangeComportReflex",
            "Proc_ChangeMyComport",
            "Proc_ChangeMyComportReflex",
            "Proc_ChangeMyComportAndMyReflex",
            "Proc_ChangeAction",
            "Proc_ChangeActionForce",
            "Proc_ChangeActionRandom",
            "Proc_ChangeActionWithEvents",
            "Proc_Loop",
            "Proc_EndLoop",
            "Proc_Break",
            // MiscNoProcedure
            "Proc_PlayerIsDead",
            "Proc_RestoreCardParameters",
            "Proc_BreakAI",
            "Proc_IgnoreTraceFlagForNextPicking",
            // MiscProcedure
            "Proc_SetMainActor",
            "Proc_ActivateObject",
            "Proc_DesactivateObject",
            "Proc_ChangeMap",
            "Proc_SetActionReturn",
            "Proc_FactorAnimationFrameRate",
            // MiscUltraProcedure
            "Proc_ForcePersoHandling",
            "Proc_PlayerIsDeadWithPlacement",
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
        });
        #endregion

        #region Conditions
        public static List<string> conditionTable = new List<string>(new string[] {
            "fn_p_stBooleanCondition",
            "fn_p_stBooleanCondition",
            "fn_p_stBooleanCondition",
            "fn_p_stBooleanCondition",
            "fn_p_stComparisonCondition",
            "fn_p_stComparisonCondition",
            "fn_p_stComparisonCondition",
            "fn_p_stComparisonCondition",
            "fn_p_stComparisonCondition",
            "fn_p_stComparisonCondition",
            "Cond_CollidePersoZDDWithPerso",
            "Cond_CollideModuleZDDWithPerso",
            "Cond_CollidePersoAllZDDWithPersoAllZDD",
            "Cond_CollidePersoZDDWithAnyPErso",
            "Cond_CollideModuleZDDWithAnyPerso",
            "Cond_CollidePersoZDEWithPersoZDE",
            "Cond_CollideModuleZDEWithPersoZDE",
            "Cond_CollidePersoZDEWithModuleZDE",
            "Cond_CollideModuleZDEWithModuleZDE",
            "Cond_CollidePersoZDEWithPersoTypeZDE",
            "Cond_CollideModuleZDEWithPersoTypeZDE",
            "Cond_CollidePersoTypeZDEWithPersoTypeZDE",
            "Cond_CollidePersoAllZDEWithPersoAllZDE",
            "Cond_CollidePersoTypeZDEWithPersoAllZDE",
            "Cond_CollidePersoAllZDEWithPersoTypeZDE",
            "Cond_CollidePersoZDENoWithTypeZDE",
            "Cond_CollideModuleZDENoWithTypeZDE",
            "Cond_CollideWithGround",
            "Cond_CollideWithWall",
            "Cond_CollideWithNothing",
            "Cond_CollideWithCeiling",
            "Cond_CollideWithPerso",
            "Cond_CollideWithWater",
            "Cond_CollideWithThisPerso",
            "Cond_ZDMCollideWithGround",
            "Cond_ZDMCollideWithWall",
            "Cond_ZDMCollideWithNothing",
            "Cond_ZDMCollideWithCeiling",
            "Cond_IsPersoInList",
            "Cond_IsModelInList",
            "Cond_IsFamilyInList",
            "Cond_ListEmptyTest",
            "fn_p_st_UserEvent_IsSet",
            "fn_p_st_UserEvent_IsSet",
            "fn_p_stButtonCondition", // 44
            "fn_p_stButtonCondition", // 45
            "fn_p_stButtonCondition", // 46
            "fn_p_stButtonCondition", // 47
            "Cond_IsTimeElapsed",
            "Cond_IsValidObject",
            "Cond_IsValidWayPoint",
            "Cond_IsValidGMT",
            "Cond_IsValidAction",
            "Cond_IsValidText",
            "Cond_SeePerso", // SectorCondition
            "Cond_IsActivable", // ActivationCondition
            "Cond_IsAlreadyHandled", // HandledCondition
            "Cond_Alw_IsMain",
            "Cond_IsPersoLightOn",
            "Cond_IsPersoLightPulseOn",
            "Cond_IsPersoLightGyroPhareOn",
            "Cond_ZDMCollideWithObstacle",
            "Cond_IsZDMCollideWithWall",
            "Cond_IsZDMCollideWithGround",
            "Cond_IsZDMCollideWithCeiling",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "Cond_CheckActionEnd", // fn_p_stCheckAnimEnd
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stCollisionWP",
            "Cond_IsCustomBitSet",
            "Cond_IsPersoActive",
            "Cond_CheckActionEnd",
            "Cond_IsCurrentStateCustomBitSet",
            "Cond_GiBlock",
            "Cond_MechanicBlock",
            "Cond_IsMechanicAnimatino",
            "Cond_IsMechanicCollide",
            "Cond_IsMechanicGravity",
            "Cond_IsMechanicTilt",
            "Cond_IsMechanicGi",
            "Cond_IsMechanicClimb",
            "Cond_IsMechanicOnGround",
            "Cond_IsMechanicSpider",
            "Cond_IsMechanicShoot",
            "Cond_IsMechanicSwim",
            "Cond_IsMechanicNeverFall",
            "Cond_IsMechanicCollisionControl",
            "Cond_IsMechanicKeepSpeedZ",
            "Cond_IsMechanicSpeedLimit",
            "Cond_IsMechanicInertia",
            "Cond_IsMechanicStream",
            "Cond_IsMechanicStickOnPlatform",
            "Cond_MechanicPatformCrash",
            "fn_p_stMechanicOption",
            "Cond_IsMechanicScale",
            "Cond_IsMechanicExec",
            "Cond_CanFall",
            "fn_p_stNullVector",
            "Cond_HierIsSonOfActor",
            "fn_p_stIsMorphing",
            "fn_p_stCheckAnimEnd",
            "Cond_HasTheCapability",
            "Cond_HasOneOfTheCapabilities",
            "Cond_HasTheCapabilityNumber",
            "Cond_PersoHasTheCapability",
            "Cond_PersoHasOneOfTheCapabilities",
            "Cond_PersoHasTheCapabilityNumber",
            "fn_p_stMagnetActivated",
            "Cond_IsNotInCollWithMap",
            "Cond_IsNotInCollWithProj",
            "Cond_IsNotInColWithSecondCharact",
            "Cond_IsNotInColWithMainCharact",
            "Cond_IsNotInColWithOtherSectors",
            "Cond_IsOfFamily",
            "Cond_IsOfModel",
            "Cond_AJoypadIsConnected",
            "Cond_AKeyJustPressed",
            "Cond_AButtonPadJustPressed",
            "Cond_IsInDemoMode",
            "fn_p_stIsInStereoMode_unsure",
            "fn_p_stIsMusicPlaying_unsure",
            "fn_p_stDummyCondition",
            "Cond_Cam_IsActive",
            "Cond_Cam_IsViewportOwner",
            "Cond_Cam_IsFlagNoDynamicTarget",
            "Cond_Cam_IsFlagNoAverageMoveTgtPerso",
            "Cond_Cam_IsFlagNoParseCutAngle",
            "Cond_Cam_IsFlagNoVisibility",
            "Cond_Cam_IsFlagNoVisibilityWithDynHie",
            "Cond_Cam_IsFlagNoDynChangeTheta",
            "Cond_Cam_IsFlagNoShiftUntilPosReached",
            "Cond_Cam_IsFlagNoDynSpeed",
            "Cond_Cam_IsFlagNoLinearParsing",
            "Cond_Cam_IsFlagNoLinearInertia",
            "Cond_Cam_IsFlagNoAngularParsing",
            "Cond_Cam_IsFlagNoAngularInertia",
            "Cond_Cam_IsFlagNoTargetParsing",
            "Cond_Cam_IsFlagNoTargetInertia",
            "Cond_Cam_IsFlagNoObstacle",
            "Cond_Cam_IsFlagFixedOrientation",
            "Cond_Cam_IsFlagForcedPosition",
            "Cond_Cam_IsFlagForcedTarget",
            "Cond_Cam_IsFlagForcedAxis"
        });
        #endregion

        #region DsgVar Types
        public static List<DsgVarInfoEntry.DsgVarType> dsgVarTypeTable = new List<DsgVarInfoEntry.DsgVarType>(new DsgVarInfoEntry.DsgVarType[] {
            DsgVarInfoEntry.DsgVarType.Boolean,
            DsgVarInfoEntry.DsgVarType.Byte,
            DsgVarInfoEntry.DsgVarType.UByte, // Unsigned
            DsgVarInfoEntry.DsgVarType.Short,
            DsgVarInfoEntry.DsgVarType.UShort, // Unsigned
            DsgVarInfoEntry.DsgVarType.Int,
            DsgVarInfoEntry.DsgVarType.UInt, // Unsigned
            DsgVarInfoEntry.DsgVarType.Float,
            DsgVarInfoEntry.DsgVarType.Waypoint,
            DsgVarInfoEntry.DsgVarType.Perso,
            DsgVarInfoEntry.DsgVarType.List,
            DsgVarInfoEntry.DsgVarType.Vector,
            DsgVarInfoEntry.DsgVarType.Comport,
            DsgVarInfoEntry.DsgVarType.Action,
            DsgVarInfoEntry.DsgVarType.Text,
            DsgVarInfoEntry.DsgVarType.GameMaterial,
            DsgVarInfoEntry.DsgVarType.Caps, // Capabilities
            DsgVarInfoEntry.DsgVarType.Graph,
            DsgVarInfoEntry.DsgVarType.Array1,
            DsgVarInfoEntry.DsgVarType.Array2,
            DsgVarInfoEntry.DsgVarType.Array3,
            DsgVarInfoEntry.DsgVarType.Array4,
            DsgVarInfoEntry.DsgVarType.Array5,
            DsgVarInfoEntry.DsgVarType.Array6,
            DsgVarInfoEntry.DsgVarType.Input
        });
        #endregion

        #region Fields
        public static List<string> fieldTable = new List<string>(new string[] {
            "GetFieldPosition",
            "SetFieldPosition",
            "GetFieldOrientation",
            "SetFieldOrientation",
            "GetFieldSpeed",
            "SetFieldSpeed",
            "GetFieldNormSpeed",
            "SetFieldNormSpeed",
            "GetFieldAbsoluteAxisX",
            "SetFieldAbsoluteAxisX",
            "GetFieldAbsoluteAxisY",
            "SetFieldAbsoluteAxisY",
            "GetFieldAbsoluteAxisZ",
            "SetFieldAbsoluteAxisZ",
            "GetFieldPrevComportIntell",
            "SetFieldPrevComportIntell",
            "GetFieldPrevComportReflex",
            "SetFieldPrevComportReflex",
            "GetFieldShadowScaleX",
            "SetFieldShadowScaleX",
            "GetFieldShadowScaleY",
            "SetFieldShadowScaleY",
            "GetFieldPadGlobalVector",
            "SetFieldPadGlobalVector",
            "GetFieldPadHorizontalAxis",
            "SetFieldPadHorizontalAxis",
            "GetFieldPadVerticalAxis",
            "SetFieldPadVerticalAxis",
            "GetFieldPadAnalogForce",
            "SetFieldPadAnalogForce",
            "GetFieldPadTrueAnalogForce",
            "SetFieldPadTrueAnalogForce",
            "GetFieldPadRotationAngle",
            "SetFieldPadRotationAngle",
            "GetFieldPadSector",
            "SetFieldPadSector"
        });
        #endregion

        public static ScriptNode.NodeType getNodeType(byte functionType) {

            switch (functionType) {
                case 0: // KeyWordFunctionPtr
                    return ScriptNode.NodeType.KeyWord;
                case 1: // GetConditionFunctionPtr
                    return ScriptNode.NodeType.Condition;
                case 2: // GetOperatorFunctionPtr
                    return ScriptNode.NodeType.Operator;
                case 3: // GetFunctionFunctionPtr
                    return ScriptNode.NodeType.Function;
                case 4: // ProcedureFunctionReturn
                    return ScriptNode.NodeType.Procedure;
                case 5: // meta action
                    return ScriptNode.NodeType.MetaAction;
                case 6:
                    return ScriptNode.NodeType.BeginMacro;
                case 7:
                    return ScriptNode.NodeType.BeginMacro;
                case 8:
                    return ScriptNode.NodeType.EndMacro;
                case 9:
                    return ScriptNode.NodeType.Field;
                case 10:
                case 11:
                    return ScriptNode.NodeType.DsgVarRef;
                case 12:
                    return ScriptNode.NodeType.Constant;
                case 13:
                    return ScriptNode.NodeType.Real;
                case 14:
                    return ScriptNode.NodeType.Button;
                case 15:
                    return ScriptNode.NodeType.ConstantVector;
                case 16:
                    return ScriptNode.NodeType.Vector;
                case 17:
                    return ScriptNode.NodeType.Mask;
                case 18:
                    return ScriptNode.NodeType.ModuleRef;
                case 19:
                    return ScriptNode.NodeType.DsgVarId;
                case 20:
                    return ScriptNode.NodeType.String;
                case 21:
                    return ScriptNode.NodeType.LipsSynchroRef;
                case 22:
                    return ScriptNode.NodeType.FamilyRef;
                case 23:
                    return ScriptNode.NodeType.PersoRef;
                case 24:
                    return ScriptNode.NodeType.ActionRef;
                case 25:
                    return ScriptNode.NodeType.SuperObjectRef;
                case 26:
                    return ScriptNode.NodeType.WayPointRef;
                case 27:
                    return ScriptNode.NodeType.TextRef;
                case 28:
                    return ScriptNode.NodeType.ComportRef;
                case 29:
                    return ScriptNode.NodeType.ModuleRef;
                case 30:
                    return ScriptNode.NodeType.SoundEventRef;
                case 31:
                    return ScriptNode.NodeType.ObjectTableRef;
                case 32:
                    return ScriptNode.NodeType.GameMaterialRef;
                case 33:
                    return ScriptNode.NodeType.ParticleGenerator;
                case 34:
                    return ScriptNode.NodeType.VisualMaterial;
                case 35:
                    return ScriptNode.NodeType.Color;
                case 36:
                    return ScriptNode.NodeType.DataType42;
                case 37:
                    return ScriptNode.NodeType.Light;
                case 38:
                    return ScriptNode.NodeType.Caps;
                case 39:
                    return ScriptNode.NodeType.SubRoutine;
                case 44:
                    return ScriptNode.NodeType.GraphRef;
            }

            return ScriptNode.NodeType.Unknown;
        }

        public static string readableFunctionSubTypeBasic(ScriptNode sn, Perso perso)
        {
            MapLoader l = MapLoader.Loader;
            byte functionType = sn.type;
            int param = (int)sn.param;
            short mask = 0;

            Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };

            switch (functionType)
            {
                case 0: // KeyWordFunctionPtr
                    if (param < keywordTable.Count) { return keywordTable[param]; }
                    return "Unknown Keyword (" + param + ")";
                case 1: // GetConditionFunctionPtr
                    if (param < conditionTable.Count) { return conditionTable[param]; }
                    return "Unknown Condition (" + param + ")";
                case 2: // GetOperatorFunctionPtr
                    if (param < operatorTable.Count) { return operatorTable[param]; }
                    return "Unknown Operator (" + param + ")";
                case 3: // GetFunctionFunctionPtr
                    if (param < functionTable.Count) { return functionTable[param]; }
                    return "Unknown Function (" + param + ")";
                case 4: // ProcedureFunctionReturn
                    if (param < procedureTable.Count) { return procedureTable[param]; }
                    return "Unknown Procedure (" + param + ")";
                case 5: // meta action
                    return "Meta Action";
                case 6:
                    return "Begin Macro";
                case 7:
                    return "Begin Macro";
                case 8:
                    return "End Macro";
                case 9:
                    if (param < fieldTable.Count) { return fieldTable[param]; }
                    return "Unknown Field (" + param + ")";
                case 10:
                case 11:
                    string dsgVarString = "";
                    return "dsgVar_" + param;
                case 12:

                    return param.ToString();
                case 13:

                    return BitConverter.ToSingle(BitConverter.GetBytes(param), 0).ToString();
                case 14: // Button/entryaction
                    EntryAction ea = EntryAction.FromOffset(sn.param_ptr);
                    string eaName = ea == null ? "ERR_ENTRYACTION_NOTFOUND" : ea.ToBasicString();
                    return eaName;
                case 15:
                    return "Constant Vector: " + "0x" + param.ToString("x8"); // TODO: get from address
                case 16:
                    return "Vector: " + "0x" + param.ToString("x8"); // TODO: same
                case 17:
                    mask = (short)param; // TODO: as short
                    return "Mask: " + (mask).ToString("x4");
                case 18:
                    return "ModuleRef: " + "0x" + (param).ToString("x8");
                case 19:
                    return "DsgVarId: " + "0x" + (param).ToString("x8");
                case 20:
                    string str = "ERR_STRING_NOTFOUND";
                    if (l.strings.ContainsKey(sn.param_ptr)) str = l.strings[sn.param_ptr];
                    return "\"" + str + "\"";
                case 21:
                    return "LipsSynchroRef: " + sn.param_ptr;
                case 22:
                    return "FamilyRef: " + sn.param_ptr;
                case 23:
                    Perso p = Perso.FromOffset(sn.param_ptr);
                    string persoName = p == null ? "ERR_PERSO_NOTFOUND" : p.fullName;
                    return "(" + persoName + "@" + sn.param_ptr + ")";
                case 24:
                    State state = State.FromOffset(sn.param_ptr);
                    string stateName = state == null ? "ERR_STATE_NOTFOUND" : state.name;
                    return "ActionRef: " + sn.param_ptr + " (" + stateName + ")";
                case 25:
                    return "SuperObjectRef: " + sn.param_ptr;
                case 26:
                    return "WayPointRef: " + sn.param_ptr;
                case 27:
                    return "TextRef: " + sn.param_textRefString;
                case 28:
                    return "ComportRef: " + sn.param_ptr;
                case 29:
                    return "ModuleRef: " + sn.param_ptr;
                case 30:
                    return "SoundEventRef: " + sn.param_ptr;
                case 31:
                    return "ObjectTableRef: " + sn.param_ptr;
                case 32:
                    return "GameMaterialRef: " + sn.param_ptr;
                case 33:
                    return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                case 34:
                    return "VisualMaterial: " + sn.param_ptr;
                case 35:
                    return "ModelCastType: " + "0x" + (param).ToString("x8");
                case 36:
                    return "EvalDataType42: " + "0x" + (param).ToString("x8");
                case 37:
                    return "CustomBits: " + "0x" + (param).ToString("x8");
                case 38:
                    return "Caps: " + "0x" + (param).ToString("x8");
                case 39:
                    return "Eval SubRoutine: " + sn.param_ptr;
                case 40:
                    return "NULL";
                case 44:
                    return "Graph: " + "0x" + (param).ToString("x8");
            }

            return "unknown";
        }

        public static string readableFunctionSubType(ScriptNode sn, Perso perso) {
            MapLoader l = MapLoader.Loader;
            byte functionType = sn.type;
            int param = (int)sn.param;
            short mask = 0;

            Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };

            switch (functionType) {
                case 0: // KeyWordFunctionPtr
                    if (param < keywordTable.Count) { return keywordTable[param]; }
                    return "Unknown Keyword (" + param + ")";
                case 1: // GetConditionFunctionPtr
                    if (param < conditionTable.Count) { return conditionTable[param]; }
                    return "Unknown Condition (" + param + ")";
                case 2: // GetOperatorFunctionPtr
                    if (param < operatorTable.Count) { return operatorTable[param] + " (" + param + ")"; }
                    return "Unknown Operator (" + param + ")";
                case 3: // GetFunctionFunctionPtr
                    if (param < functionTable.Count) { return functionTable[param]; }
                    return "Unknown Function (" + param + ")";
                case 4: // ProcedureFunctionReturn
                    if (param < procedureTable.Count) { return procedureTable[param]; }
                    return "Unknown Procedure (" + param + ")";
                case 5: // meta action
                    return "Meta Action";
                case 6:
                    return "Begin Macro";
                case 7:
                    return "Begin Macro";
                case 8:
                    return "End Macro";
                case 9:
                    return "EvalField";
                case 10:
                case 11:
                    string dsgVarString = "";
                    if (perso.brain.mind.dsgMem != null && perso.brain.mind.dsgMem.dsgVar != null
                        && perso.brain.mind.dsgMem.dsgVar.dsgVarInfos != null
                        && param < perso.brain.mind.dsgMem.dsgVar.dsgVarInfos.Length) {
                        DsgVarInfoEntry info = perso.brain.mind.dsgMem.dsgVar.dsgVarInfos[param];
                        if (info != null) {
                            dsgVarString += " (type " + info.type + ", value " + info.value + ")";
                        } else {
                            dsgVarString += " (not found)";
                        }
                    }
                    return "DsgVarRef: " + param + dsgVarString;
                case 12:

                    return "Constant: " + param;
                case 13:

                    return "Real: " + BitConverter.ToSingle(BitConverter.GetBytes(param), 0);
                case 14:
                    EntryAction ea = EntryAction.FromOffset(sn.param_ptr);
                    string eaName = ea == null ? "ERR_ENTRYACTION_NOTFOUND" : ea.ToString();
                    return "Button: " + eaName + "("+sn.param_ptr+")";
                case 15:
                    return "Constant Vector: " + "0x" + param.ToString("x8"); // TODO: get from address
                case 16:
                    return "Vector: " + "0x" + param.ToString("x8"); // TODO: same
                case 17:
                    mask = (short)param; // TODO: as short
                    return "Mask: " + (mask).ToString("x4");
                case 18:
                    return "ModuleRef: " + "0x" + (param).ToString("x8");
                case 19:
                    return "DsgVarId: " + "0x" + (param).ToString("x8");
                case 20:
                    string str = "ERR_STRING_NOTFOUND";
                    if (l.strings.ContainsKey(sn.param_ptr)) str = l.strings[sn.param_ptr];
                    return "String: " + sn.param_ptr + " (" + str + ")";
                case 21:
                    return "LipsSynchroRef: " + sn.param_ptr;
                case 22:
                    return "FamilyRef: " + sn.param_ptr;
                case 23:
                    Perso p = Perso.FromOffset(sn.param_ptr);
                    string persoName = p == null ? "ERR_PERSO_NOTFOUND" : p.fullName;
                    return "PersoRef: " + sn.param_ptr + " (" + persoName + ")";
                case 24:
                    State state = State.FromOffset(sn.param_ptr);
                    string stateName = state == null ? "ERR_STATE_NOTFOUND" : state.name;
                    return "ActionRef: " + sn.param_ptr + " (" + stateName + ")";
                case 25:
                    return "SuperObjectRef: " + sn.param_ptr;
                case 26:
                    return "WayPointRef: " + sn.param_ptr;
                case 27:
                    return "TextRef: " + sn.param + " ("+sn.param_textRefString+")";
                case 28:
                    return "ComportRef: " + sn.param_ptr;
                case 29:
                    return "ModuleRef: " + sn.param_ptr;
                case 30:
                    return "SoundEventRef: " + sn.param_ptr;
                case 31:
                    return "ObjectTableRef: " + sn.param_ptr;
                case 32:
                    return "GameMaterialRef: " + sn.param_ptr;
                case 33:
                    return "ParticleGenerator: " + "0x" + (param).ToString("x8");
                case 34:
                    return "VisualMaterial: " + sn.param_ptr;
                case 35:
                    return "Color: " + "0x" + (param).ToString("x8");
                case 36:
                    return "EvalDataType42: " + "0x" + (param).ToString("x8");
                case 37:
                    return "Light: " + "0x" + (param).ToString("x8");
                case 38:
                    return "Caps: " + "0x" + (param).ToString("x8");
                case 39:
                    return "Eval SubRoutine: " + sn.param_ptr;
                case 40:
                    return "NULL";
                case 44:
                    return "Graph: " + "0x" + (param).ToString("x8");
            }

            return "unknown";
        }
    }
}
