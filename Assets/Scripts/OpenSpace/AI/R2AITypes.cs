using OpenSpace.EngineObject;
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
            "If",
            "If",
            "If",
            "If",
            "If",
            "Else1",
            "Else2",
            "Then",
            "Else3",
            "Else4",
            "Goto",
            "Me",
            "MainActor",
            "Nobody",
            "NoInput",
            "Nowhere",
            "EmptyText",
            "NULL",
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
            "Dot",
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
            "fn_p_stGetPersoAbsolutePosition",
            "fn_p_stGetMyAbsolutePosition",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceToPerso",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceToPersoCenter",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stDistanceFunction",
            "fn_p_stGetWpAbsolutePosition",
            "fn_p_stMathFunctionInt",
            "fn_p_stMathFunctionInt",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathAbsoluteValue",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathFunctionReal",
            "fn_p_stMathTernarReal",
            "fn_p_stMathTernarReal",
            "fn_p_stMathTernarReal",
            "fn_p_stMathTernarReal",
            "fn_p_stMathTernarReal",
            "fn_p_stMathTernarReal",
            "fn_p_stRealFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stHitPointsAndHitPointsMaxFunction",
            "fn_p_stListFunction",
            "fn_p_stListFunction",
            "fn_p_stLocalToGlobal",
            "fn_p_stGlobalToLocal",
            "fn_p_stLocalToGlobal",
            "fn_p_stGlobalToLocal",
            "MGT_fn_p_stGetInfo",
            "MGT_fn_p_stGetInfo",
            "MGT_fn_p_stGetInfo",
            "MGT_fn_p_stGetInfo",
            "fn_p_st_SPO_GetDrawFlag",
            "fn_p_stTimeFunction",
            "fn_p_stTimeFunction",
            "fn_p_stGetDTFunction",
            "fn_p_stGetFrameLength",
            "fn_p_stInputFunction",
            "fn_p_stCode4VitessePadAnalogique",
            "fn_p_stMiscFunction",
            "fn_p_stMiscFunction",
            "fn_p_stMiscFunction",
            "fn_p_stMiscFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stColorFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stVisualGameMaterialFunction",
            "fn_p_stSaveGameFunction",
            "fn_p_stSaveGameFunction",
            "fn_p_stSaveGameFunction",
            "fn_p_stSaveGameFunction",
            "fn_p_stSaveGameFunction",
            "fn_p_stIsAValidSlotName",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDeMagie",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PointsDair",
            "fn_p_stCode4PersoLePlusProche",
            "fn_p_stGetNearestActorInCurrentSector",
            "fn_p_stCode4PersoLePlusProche",
            "fn_p_stGetNbActivePerso",
            "fn_p_stCode4PersoLePlusProche",
            "fn_p_stCode4PersoLePlusProche",
            "fn_p_stCloserWPInGraph",
            "fn_p_stCloserWPInGraph",
            "fn_p_stCloserWPInGraph",
            "fn_p_stCloserWPInGraph",
            "fn_p_stTravelOnAGraph",
            "fn_p_stTravelOnAGraph",
            "fn_p_stTravelOnAGraph",
            "fn_p_stGraphToWayFunction",
            "fn_p_stGraphToWayFunction",
            "fn_p_stGraphToWayFunction",
            "fn_p_stGraphToWayFunction",
            "fn_p_stGraphToWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stWayFunction",
            "fn_p_stGetSituationOnRail",
            "fn_p_stGraphFunction",
            "fn_p_stGraphFunction",
            "fn_p_stGraphFunction",
            "fn_p_stGraphFunction",
            "fn_p_stMiscFuncOnTypeOfWP",
            "fn_p_stPersoCapabilityFunction",
            "fn_p_stCapabilityFunction",
            "fn_p_stGetScrollSpeed",
            "fn_p_stGetNbFrame",
            "fn_p_stDotProduct",
            "fn_p_stVectorOperations",
            "fn_p_stVectorOperations",
            "fn_p_stGetSPOCoordinates",
            "fn_p_stTractionFactorFunction",
            "fn_p_stGetZDxCenter",
            "fn_p_stGetZDxCenter",
            "fn_p_stGetZDxCenter",
            "fn_p_stGetZDxCenter",
            "fn_p_stCode4TextAffiche",
            "fn_p_stGetCPUCounter",
            "fn_p_stCode4VitesseHorizontaleDuPerso",
            "fn_p_stCode4VitesseHorizontaleDuPerso",
            "fn_p_st3DFunc",
            "fn_p_stGetPersoSighting",
            "fn_p_st3DFunc",
            "fn_p_st3DFunc",
            "fn_p_stCode4LitPositionZDx",
            "fn_p_stCode4LitPositionZDx",
            "fn_p_stCode4LitPositionZDx",
            "fn_p_stCode4LitCentreZDx",
            "fn_p_stCode4LitCentreZDx",
            "fn_p_stCode4LitCentreZDx",
            "fn_p_stCode4LitAxeZDx",
            "fn_p_stCode4LitAxeZDx",
            "fn_p_stCode4LitAxeZDx",
            "fn_p_stCode4LitDimensionZDx",
            "fn_p_stCode4LitDimensionZDx",
            "fn_p_stCode4LitDimensionZDx",
            "fn_p_stCode4VecteurPointAxe",
            "fn_p_stCode4VecteurPointAxe",
            "fn_p_stVectorFunction",
            "fn_p_stVectorFunction",
            "fn_p_stVectorFunction",
            "fn_p_stVectorFunction",
            "fn_p_stVectorFunction",
            "fn_p_stVectorFunction",
            "fn_p_stVectorAndAngle",
            "fn_p_stVectorAndAngle",
            "fn_p_stVectorAndAngle",
            "fn_p_stGetNormalCollideVector",
            "fn_p_stGetNormalCollideVector",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollidePoint",
            "fn_p_stGetCollision",
            "fn_p_stGetCollision",
            "fn_p_stGetCollision",
            "fn_p_stGetCollisionWithCollideMaterial",
            "fn_p_stGameMaterialFunction",
            "fn_p_stGameMaterialFunction",
            "fn_p_stGameMaterialFunction",
            "fn_p_stCollideCommunicationFunc",
            "fn_p_stCollideCommunicationFunc",
            "fn_p_stCollideCommunicationFunc",
            "fn_p_stGetLastCollisionActor",
            "fn_p_stComputeRebondVector",
            "fn_p_st3DFunc",
            "fn_p_st3DFunc",
            "fn_p_st3DFunc",
            "fn_p_st3DFunc",
            "fn_p_stMiscFunction",
            "fn_p_stNamesFunc",
            "fn_p_stNamesFunc",
            "fn_p_stNamesFunc",
            "fn_p_stNamesFunc",
            "fn_p_stNamesFunc",
            "fn_p_stNamesFunc",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_GetMechanicParameter",
            "fn_p_ComputeJumpImpulsion",
            "fn_p_GetMechanicParameter",
            "fn_p_GetHierarchyLink",
            "fn_p_GetPriviligedActivationZdx",
            "fn_p_GetPriviligedActivationZdx",
            "fn_p_GetPriviligedActivationZdx",
            "fn_p_GetPriviligedActivationZdx",
            "fn_p_GetComputationFrequency",
            "fn_p_GetComputationFrequency",
            "fn_p_GetComputationFrequency",
            "fn_p_GetBooleanInArray",
            "fn_p_GetBooleanInArray",
            "fn_p_stGetButtonName",
            "fn_p_stGetDriversAvailable",
            "fn_p_stGetCurrentLanguageId",
            "fn_p_stGetNbLanguages",
            "fn_p_stGetLanguageText",
            "fn_p_stTextToInt",
            "fn_p_stOptionRecupererFunc",
            "fn_p_stOptionRecupererFunc",
            "fn_p_stOptionSlotIsValid",
            "fn_p_stGetNbAvailableResolution",
            "fn_p_stGetCurrentResolution",
            "fn_p_stSaveCurrentResolution",
            "fn_p_stIsDisplayModeAvailable",
            "fn_p_stGetBrightness",
            "fn_p_stGetNameResolution",
            "fn_p_stGetNbSlotsAvailable",
            "fn_p_stGetVideoOptions",
            "fn_p_stGetVideoOptions",
            "fn_p_stGetMSSoundValues",
            "fn_p_stGetMSSoundValues",
            "fn_p_stGetStdGameLimit",
            "fn_p_stGetStdGameLimit",
            "fn_p_stGetStdGameLimit",
            "fn_p_ExecuteVariable",
            "fn_p_stComputeProtectKey",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "fn_p_stComputeNot",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "fn_p_stComputeXor",
            "CAM_fn_p_stGetVectorParameter",
            "CAM_fn_p_stGetVectorParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetRealParameter",
            "CAM_fn_p_stGetTargetedSuperObject",
            "CAM_fn_p_stGetTypeOfViewport",
            "CAM_fn_p_stGetCameraOfViewport",
            "CAM_fn_p_stGetCameraOfViewport",
            "CAM_fn_p_stComputeTargetWithTgtPerso",
            "CAM_fn_p_stComputeCurrentTarget",
            "CAM_fn_p_stGetSectorCameraType",
            "CAM_fn_p_stGetBestPos"
        });
        #endregion

        #region Procedures
        public static List<string> procedureTable = new List<string>(new string[] {
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListZDDZDEProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListProcedure",
            "fn_p_stListEnsembleProcedure",
            "fn_p_stListEnsembleProcedure",
            "fn_p_stListEnsembleProcedure",
            "fn_p_stListEnsembleProcedure",
            "fn_p_stFogProcedure",
            "fn_p_stFogProcedure",
            "fn_p_stFogProcedure",
            "fn_p_stFogProcedure",
            "fn_p_stFogProcedure",
            "fn_p_stFogProcedure",
            "MGT_fn_p_stActiveMagnet",
            "MGT_fn_p_stDeactiveMagnet",
            "MGT_fn_p_stSetParam",
            "MGT_fn_p_stSetParam",
            "MGT_fn_p_stSetParam",
            "MGT_fn_p_stSetParam",
            "fn_p_st_FootPath_AddFootPrint",
            "fn_p_st_FootPath_Clear",
            "fn_p_st_SinusEffect_SetFreq",
            "fn_p_st_SinusEffect_SetAmplitude",
            "fn_p_st_SinusEffect_SetState",
            "fn_p_st_SinusEffect_SetFreq",
            "fn_p_st_SinusEffect_SetRLIParams",
            "fn_p_st_SinusEffect_SetRLIParams",
            "fn_p_st_SPO_SetDrawFlag",
            "fn_p_st_SPO_SetEngineDisplayModeFlag",
            "fn_p_stKeyboardProcedure",
            "fn_p_stKeyboardProcedure",
            "fn_p_stDummy",
            "fn_p_stChangeComportIntelligenceProcedure",
            "fn_p_stChangeComportReflexProcedure",
            "fn_p_stChangeMyComportIntelligenceProcedure",
            "fn_p_stChangeMyComportReflexProcedure",
            "fn_p_stChangeMyComportIntelligenceAndReflexProcedure",
            "fn_p_stChangeActionProcedure",
            "fn_p_stChangeActionProcedure",
            "fn_p_stChangeActionRandomProcedure",
            "fn_p_stChangeActionProcedure",
            "fn_p_stLoopKeyWord",
            "fn_p_stEndLoopKeyWord",
            "fn_p_stBreakKeyword",
            "fn_p_stMiscNoProcedure",
            "fn_p_stMiscNoProcedure",
            "fn_p_stMiscNoProcedure",
            "fn_p_stMiscNoProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscUltraProcedure",
            "fn_p_stMiscUltraProcedure",
            "fn_p_stMiscProcedure",
            "fn_p_stMiscMoreProcedure",
            "fn_p_stLinkTableProcedure",
            "fn_p_stLinkTableProcedure",
            "fn_p_stBuildObjectTableFromTableAndStringProcedure",
            "fn_p_stBuildObjectTableFromTableAndStringProcedure",
            "fn_p_stModuleControlProcedure",
            "fn_p_stModuleControlProcedure",
            "fn_p_stModuleControlProcedure",
            "fn_p_stSaveGameProcedure",
            "fn_p_stSaveGameProcedure",
            "fn_p_stChangeActivationChannelProcedure",
            "fn_p_stChangeActivationChannelProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stPersoLightProcedure",
            "fn_p_stSoundProcedure",
            "fn_p_stSoundProcedure",
            "fn_p_stSoundProcedure",
            "fn_p_stSoundProcedure",
            "fn_p_stSoundProcedure",
            "fn_p_stDisplayProcedure",
            "fn_p_stMenuAndVignetteProcedure",
            "fn_p_stMenuAndVignetteProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stParticleGeneratorProcedure",
            "fn_p_stTurnProcedure",
            "fn_p_stTurnProcedure",
            "fn_p_stTurnPersoProcedure",
            "fn_p_stTurnPersoProcedure",
            "fn_p_stKillPersoAndClearVariableProcedure",
            "fn_p_stKillPersoAndClearVariableProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stVisualGameMaterialProcedure",
            "fn_p_stOptionChangerProc",
            "fn_p_stOptionChangerProc",
            "fn_p_stOptionChangerProc",
            "fn_p_stOptionChangerProc",
            "fn_p_stOptionChangerProc",
            "fn_p_stOptionChangerProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_stDynamProcedure",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc_ForReorientation",
            "fn_p_st3DProc_ForReorientation",
            "fn_p_st3DProc",
            "fn_p_stCustomBitsProc",
            "fn_p_stCustomBitsProc",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_stFixePositionPerso",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_stPrepareMorph",
            "fn_p_stStopMorph",
            "fn_p_stMorphing",
            "fn_p_stModuleControlProcedure",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc",
            "fn_p_st3DProc_ForReorientation",
            "fn_p_st3DProc_ForReorientation",
            "fn_p_st3DProc",
            "fn_p_stCollideCommunicationProc",
            "fn_p_stCollideCommunicationProc",
            "fn_p_stCollideCommunicationProc",
            "fn_p_stResetLastCollisionActor",
            "fn_p_stResetLastCollisionActor",
            "fn_p_stMiscUltraProcedure",
            "fn_p_stMiscUltraProcedure",
            "fn_p_stStringProc",
            "fn_p_stStringProc",
            "fn_p_stStringProc",
            "fn_p_stStringProc",
            "fn_p_stIntToText",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicOption",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanic3RealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicVectorParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicVectorParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicVectorParameter",
            "fn_p_SetMechanicListParameter",
            "fn_p_SetMechanicSpeedVector",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_PlatformLink",
            "fn_p_PlatformLink",
            "fn_p_SetMechanic3Real",
            "fn_p_SetMechanic3Real",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicClimbSpeedLimit",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicSpeedReal",
            "fn_p_SetMechanicRealParameter",
            "fn_p_SetMechanicRealParameter",
            "fn_p_HierFatherLink",
            "fn_p_HierFatherLink",
            "fn_p_HierFatherLink",
            "fn_p_stMiscProcedure",
            "fn_p_LinkControl",
            "fn_p_stMiscProcedure",
            "fn_p_stFixePositionZDx",
            "fn_p_stFixePositionZDx",
            "fn_p_stFixePositionZDx",
            "fn_p_stChangeLigthIntensity",
            "fn_SPO_p_stChangeFlag",
            "fn_p_stChangeActorSpoFlag",
            "fn_p_stChangeScreen",
            "fn_p_stChangeScreen",
            "fn_p_stChangeScreen",
            "fn_p_stChangeScreen",
            "fn_p_stChangeScreen",
            "fn_p_stShadow",
            "fn_p_stShadow",
            "fn_p_stShadow",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stSetPrivilegeActivationZDx",
            "fn_p_stActivationZDR",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetCollSetCollisionFlag",
            "fn_p_stSetComputationFrequency",
            "fn_p_stSetComputationFrequency",
            "fn_p_stSetComputationFrequency",
            "fn_p_stWorldMapProc",
            "fn_p_stWorldMapProc",
            "fn_p_stWorldMapProc",
            "fn_p_stReadAnalogJoystickMario",
            "fn_p_stSetAnalogJoystickAxes",
            "fn_p_stSetAnalogJoystickAxes",
            "fn_p_stSetAnalogJoystickAxes",
            "fn_p_stSetAnalogJoystickAxes",
            "fn_p_stSetBooleanInArray",
            "fn_p_stSetDefaultFormatCharacter",
            "fn_p_stSetVideoOptions",
            "fn_p_stDummy",
            "fn_p_stDummy",
            "fn_p_stSelectResolution",
            "fn_p_stChangeBrightness",
            "fn_p_stTextIntegerParameter",
            "fn_p_stTextRealParameter",
            "fn_p_stSetVideoOptions",
            "fn_p_stSetVideoOptions",
            "fn_p_stSetInStereoMode",
            "fn_p_stSetPrevMusicFadeOut",
            "fn_p_stSomethingDynamics",
            "fn_p_stSaveCurrentSoundRequest",
            "fn_p_stSaveCurrentSoundRequest",
            "fn_p_stDummy",
            "script_proc_sub_470000",
            "script_proc_sub_470210",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4MagicPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stCode4AirPointsProcedures",
            "fn_p_stFixePositionFade",
            "fn_p_stSetLocalLight",
            "fn_p_stSetLight",
            "fn_p_stSetLight",
            "fn_p_stSetLight",
            "fn_p_stSetLight",
            "fn_p_stRLIProcedure",
            "fn_p_stRLIProcedure",
            "fn_p_stRLIProcedure",
            "fn_p_stMiscProcOnTypeOfWP",
            "fn_p_stCapabilityProcedure",
            "fn_p_stCapabilityProcedure",
            "fn_p_stCapabilityProcedure",
            "fn_p_stCapabilityProcedure",
            "fn_p_stReinitGraphProcedure",
            "fn_p_stSetScrollSpeed",
            "fn_p_stSetScrollSpeed",
            "fn_p_stSetScrollSpeed",
            "fn_p_stSetScrollSpeed",
            "fn_p_stManageFrame",
            "fn_p_stManageFrame",
            "fn_p_stFreezeAnim",
            "fn_p_stFreezeAnim",
            "fn_p_stSPOSuperimpoed",
            "fn_p_stSPOSuperimpoed",
            "fn_p_stSPOSuperimpoed",
            "fn_p_stSetSPOCoordinates",
            "fn_p_stSPOSuperimpoed",
            "fn_p_stTractionFactorProcedure",
            "fn_p_stTurnToPosition",
            "fn_p_stTurnToPosition",
            "fn_p_stSortArray",
            "fn_p_stActiveJoystickAnal",
            "fn_p_stActiveJoystickAnal",
            "fn_p_stSetZDxSize",
            "fn_p_stSetZDxSize",
            "fn_p_stSetSoundVolume",
            "fn_p_stSetSoundVolume",
            "fn_p_stSetSoundVolume",
            "fn_p_stSetDopplerEffect",
            "fn_p_stPauseSound",
            "fn_p_stPauseSound",
            "fn_p_stPauseSound",
            "fn_p_stCode4ChangeCaractere",
            "fn_p_stCode4FormateTexte",
            "fn_p_stCode4ValideNomDuSlot",
            "fn_p_stCode4RestoreNomDuSlot",
            "fn_p_stChangePersoHorizSighting",
            "fn_p_stPlayDemo",
            "fn_p_stInitKeyBoardDirections",
            "fn_p_stSetCenterPosition",
            "fn_p_stSetMaximalValues",
            "fn_p_stSetTooFarLimit",
            "fn_p_stSetTransparencyZone",
            "fn_p_stBaseFrequenceForMenu",
            "fn_p_stSaveSinusEffectContext",
            "fn_p_stSaveSinusEffectContext",
            "fn_p_stTextSuperImposed",
            "CAM_fn_p_stUpdatePosition",
            "CAM_fn_p_stChangeVectorParameter",
            "CAM_fn_p_stChangeVectorParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeRealParameter",
            "CAM_fn_p_stChangeTgtPerso",
            "CAM_fn_p_stChangeSecondTgtPerso",
            "CAM_fn_p_stChangeChannel",
            "CAM_fn_p_stActivate",
            "CAM_fn_p_stAssociateViewport",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetIAFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stSetDNMFlags",
            "CAM_fn_p_stChangeConstants",
            "CAM_fn_p_stConstants",
            "CAM_fn_p_stConstants",
            "CAM_fn_p_stShowInfo",
            "CAM_fn_p_stSetForce",
            "CAM_fn_p_stSetForce",
            "CAM_fn_p_stSetForce",
            "CAM_fn_p_stReset",
            "CAM_fn_p_stForceBestPos",
            "CAM_fn_p_stForceNormalState",
            "CAM_fn_p_stForceMovingOnRail",
            "CAM_fn_p_stCameraMode",
            "fn_p_stAGOProcedure",
            "fn_p_stInGameMenuProcedure",
            "fn_p_stJFFTXTProcedure",
            "fn_p_stJFFTXTProcedure",
            "fn_p_stJFFTXTProcedure",
            "fn_p_stDummy",
            "fn_p_stDummy",
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
            "fn_p_stZDDCondition",
            "fn_p_stZDDCondition",
            "fn_p_stZDDCondition",
            "fn_p_stZDDCondition",
            "fn_p_stZDDCondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stZDECondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportOnSurfaceCondition",
            "fn_p_stReportThisPerso",
            "fn_p_stReportOnZDMSurfaceCondition",
            "fn_p_stReportOnZDMSurfaceCondition",
            "fn_p_stReportOnZDMSurfaceCondition",
            "fn_p_stReportOnZDMSurfaceCondition",
            "fn_p_stListCondition",
            "fn_p_stListCondition",
            "fn_p_stListCondition",
            "fn_p_stListCondition",
            "fn_p_st_UserEvent_IsSet",
            "fn_p_st_UserEvent_IsSet",
            "fn_p_stButtonCondition",
            "fn_p_stButtonCondition",
            "fn_p_stButtonCondition",
            "fn_p_stButtonCondition",
            "fn_p_stTimeCondition",
            "fn_p_stValidityCondition",
            "fn_p_stValidityCondition",
            "fn_p_stValidityCondition",
            "fn_p_stValidityCondition",
            "fn_p_stValidityCondition",
            "fn_p_stSectorCondition",
            "fn_p_st_ActivationCondition",
            "fn_p_st_HandledCondition",
            "fn_p_stAlw_IsMine",
            "fn_p_stLightCondition",
            "fn_p_stLightCondition",
            "fn_p_stLightCondition",
            "fn_p_stZDMCollideWithObstacle",
            "fn_p_stZDMCollideWithObstacle",
            "fn_p_stZDMCollideWithObstacle",
            "fn_p_stZDMCollideWithObstacle",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stCheckAnimEnd",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stMiscCondition",
            "fn_p_stCollisionWP",
            "fn_p_stCustomBitsCond",
            "fn_p_stIsPersoActive",
            "fn_p_stCheckAnimEnd",
            "fn_p_stCustomBitsCond",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stMechanicOption",
            "fn_p_stNullVector",
            "fn_p_stHierarchySon",
            "fn_p_stIsMorphing",
            "fn_p_stCheckAnimEnd",
            "fn_p_stCheckCapabilities",
            "fn_p_stCheckCapabilities",
            "fn_p_stCheckCapabilities",
            "fn_p_stCheckCapabilities",
            "fn_p_stCheckCapabilities",
            "fn_p_stCheckCapabilities",
            "fn_p_stMagnetActivated",
            "fn_p_stIsCollisionFlag",
            "fn_p_stIsCollisionFlag",
            "fn_p_stIsCollisionFlag",
            "fn_p_stIsCollisionFlag",
            "fn_p_stIsCollisionFlag",
            "fn_p_stIsOf",
            "fn_p_stIsOf",
            "fn_p_stAJoypadIsJoystickControlAvailable",
            "fn_p_stAKeyJustPressed_unsure",
            "fn_p_stAButtonPadJustPressed_unsure",
            "fn_p_stDemoCondition_unsure",
            "fn_p_stIsInStereoMode_unsure",
            "fn_p_stIsMusicPlaying_unsure",
            "fn_p_stDummyCondition",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs",
            "CAM_fn_p_stIs"
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

        public static string readableFunctionSubType(ScriptNode sn, Perso perso) {
            MapLoader l = MapLoader.Loader;
            byte functionType = sn.type;
            int param = (int)sn.param;
            short mask = 0;

            Vector3 vector3 = new Vector3 { x = 0, y = 0, z = 0 };

            switch (functionType) {
                case 0: // KeyWordFunctionPtr
                    if (param < keywordTable.Count) { return keywordTable[param] + "(" + param + ")"; }
                    return "Unknown Keyword";
                case 1: // GetConditionFunctionPtr
                    if (param < conditionTable.Count) { return conditionTable[param] + "(" + param + ")"; }
                    return "Unknown Condition";
                case 2: // GetOperatorFunctionPtr
                    if (param < operatorTable.Count) { return operatorTable[param] + "(" + param + ")"; }
                    return "Unknown Operator";
                case 3: // GetFunctionFunctionPtr
                    if (param < functionTable.Count) { return functionTable[param] + "(" + param + ")"; }
                    return "Unknown Function";
                case 4: // ProcedureFunctionReturn
                    if (param < procedureTable.Count) { return procedureTable[param] + "(" + param + ")"; }
                    return "Unknown Procedure";
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
                    return "Button: " + "0x" + param.ToString("x8");
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
                    return "TextRef: " + sn.param_ptr;
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
                    return "Eval SubRoutine";
                case 40:
                    return "NULL";
                case 44:
                    return "Graph: " + "0x" + (param).ToString("x8");
            }

            return "unknown";
        }
    }
}
