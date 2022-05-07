﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_Hype_PS2 : AI_Types {
		#region Procedures
		protected override void InitProcedures() {
			procedureTable = new string[] {
				"Proc_PlayerIsDead",
				"Proc_ChangeInitPosition",
				"Proc_SetMainActor",
				"Proc_ActivateObject",
				"Proc_DesactivateObject",
				"Proc_ChangeMap",
				"Proc_SetActionReturn",
				"Proc_FactorAnimationFrameRate",
				"Proc_PlayerIsDeadWithOption",
				"Proc_BecomesSonOfPerso",
				"Proc_BecomesFatherOfPerso",
				"Proc_FillListWithSons",
				"Proc_ActivateObjectOnPosition",
				"Proc_ChangeMapAtPosition",
				"Proc_PlayerIsDeadWithPlacement",
				"Proc_AllowDynamLOD",
				"Proc_ForbidDynamLOD",
				"Proc_ResetSpeed",
				"Proc_ResetOrientation",
				"Proc_SetRotationAxe",
				"Proc_SetAlphaAngle",
				"Proc_SetRotationAngleStep",
				"Proc_SetThetaAngle",
				"Proc_SetImpulse",
				"Proc_SetNormSpeed",
				"Proc_AddNormSpeed",
				"Proc_MulNormSpeed",
				"Proc_SetDirectionSpeed",
				"Proc_AddDirectionSpeed",
				"Proc_SetVectorSpeed",
				"Proc_AddWind",
				"Proc_SetDynamScalar",
				"Proc_SetTarget",
				"Proc_TurnAround",
				"Proc_GoRelative",
				"Proc_GoInDirection",
				"Proc_TurnLeft",
				"Proc_TurnRight",
				"Proc_TurnUp",
				"Proc_LateralLeft",
				"Proc_TurnLateralright",
				"Proc_Brake",
				"Proc_AccelTurbo",
				"Proc_TurnDown",
				"Proc_Pulse",
				"Proc_StonePulse",
				"Proc_Jump",
				"Proc_JumpAbsolute",
				"Proc_JumpWithoutAddingSpeed",
				"Proc_Fire",
				"Proc_GoTarget",
				"Proc_ReachTarget",
				"Proc_SetHitPoints",
				"Proc_SetHitPointsToInit",
				"Proc_SetHitPointsToMax",
				"Proc_AddHitPoints",
				"Proc_SubHitPoints",
				"Proc_SetHitPointsMax",
				"Proc_SetHitPointsMaxToInit",
				"Proc_SetHitPointsMaxToMax",
				"Proc_AddHitPointsMax",
				"Proc_SubHitPointsMax",
				"Proc_ListAffectWithPersoZDD",
				"Proc_ListAffectWithModuleZDD",
				"Proc_ListAffectWithPersoZDE",
				"Proc_ListAffectWithModuleZDE",
				"Proc_ListAffectWithPersoTypeZDE",
				"Proc_ListAffectWithModuleTypeZDE",
				"Proc_ListAffectTypeZDEWithTypeZDE",
				"Proc_AddPersoInList",
				"Proc_AddPersoInListAt",
				"Proc_DeletePersoAtInList",
				"Proc_FindPersoAndDeleteInList",
				"Proc_Select",
				"Proc_UnSelect",
				"Proc_ListSort",
				"Proc_ListSortByFamily",
				"Proc_FillListWithAllPersoOfAFamily",
				"Proc_ListUnion",
				"Proc_ListInter",
				"Proc_ListDiff",
				"Proc_ListAdd",
				"Proc_SwapLinkTableObjects",
				"Proc_ChangeCurrentObjectTable",
				"Proc_CopyObjectFromTableToTable",
				"Proc_LSY_StartSpeech",
				"Proc_LSY_StopSpeech",
				"Proc_TakeModuleControl",
				"Proc_ReleaseModuleControl",
				"Proc_InitModuleCtrlWithAnimTranslation",
				"Proc_InitModuleCtrlWithAnimRotation",
				"Proc_RotateSector",
				"Proc_RotateSectorLocalX",
				"Proc_RotateSectorLocalY",
				"Proc_RotateSectorLocalZ",
				"Proc_LevelSaveRotationSector",
				"Proc_PlayerSaveRotationSector",
				"Proc_TranslateSector",
				"Proc_TranslateLocalSector",
				"Proc_SaveGame",
				"Proc_IncHistoricAndSaveGame",
				"Proc_SaveAllGameValues",
				"Proc_EnableEscape",
				"Proc_ResetButtonState",
				"Proc_ActivateChannel",
				"Proc_DeactivateChannel",
				"Proc_FogOn",
				"Proc_FogOff",
				"Proc_SetFogColor",
				"Proc_SetFogNearFarInf",
				"Proc_PersoLightOn",
				"Proc_PersoLightOff",
				"Proc_SetPersoLightColor",
				"Proc_SetPersoLightNearFar",
				"Proc_SetPersoLightLittleBigAlpha",
				"Proc_SetPersoLightGyrophare",
				"Proc_SetPersoLightPulse",
				"Proc_SetPersoLightParalleleType",
				"Proc_SetPersoLightSphericalType",
				"Proc_SetPersoLightHotSpotType",
				"Proc_SetPersoLightAmbientType",
				"Proc_DYL_ChangeGraduallyIntensity",
				"Proc_DYL_CopyStaticToDynamic",
				"Proc_AddSurfaceHeight",
				"Proc_MoveSurfaceHeight",
				"Proc_LevelSaveMovingSurface",
				"Proc_PlayerSaveMovingSurface",
				"Proc_SendSoundRequest",
				"Proc_SendVoiceRequest",
				"Proc_SendMusicRequest",
				"Proc_DisplaylValue",
				"Proc_DisplayChrono",
				"Proc_DisplayString",
				"Proc_ActivateString",
				"Proc_EraseString",
				"Proc_Camera_UpdatePosition",
				"Procedure_Camera_Reset",
				"Procedure_Camera_ActivateSctUpdPatch",
				"Procedure_Camera_RestoreAfterTrame",
				"Procedure_Camera_CancelRestoreTrame",
				"Procedure_Camera_ShowInfo",
				"Procedure_Camera_ChangeCamera",
				"Procedure_Camera_ActiveCamera",
				"Procedure_Camera_ChangeDistance",
				"Procedure_Camera_BoundChangeDist",
				"Procedure_Camera_ChangeCamLinearSpeed",
				"Procedure_Camera_ChangeCamLinearInertia",
				"Procedure_Camera_ChangeCamAngularSpeed",
				"Procedure_Camera_ChangeCamAngularInertia",
				"Procedure_Camera_ChangeTgtLinearSpeed",
				"Procedure_Camera_MoveTarget",
				"Procedure_Camera_MoveLookTarget",
				"Procedure_Camera_MoveShift",
				"Procedure_Camera_ChangeAngle",
				"Procedure_Camera_Move",
				"Procedure_Camera_Turn",
				"Procedure_Camera_ManageSavedPos",
				"Procedure_Camera_ChangeState",
				"Procedure_Camera_ChangeFocal",
				"Procedure_Camera_ActivateFocalParsing",
				"Procedure_Camera_ChangeTargetedPerso",
				"Proc_Cam_ChangeTgtChannel",
				"Proc_Cam_StopTargettingChannel",
				"Procedure_Camera_ResetTargetedPerso",
				"Procedure_Camera_Shake",
				"Procedure_Camera_SetPosition",
				"Procedure_Camera_ChangeFlag",
				"Procedure_Camera_SavePositionAsOptimal",
				"Procedure_Camera_SetAdditionnalTarget",
				"Procedure_Camera_SetApexParameters",
				"Procedure_Camera_SetCameraZMax",
				"Procedure_Camera_ResetCameraZMax",
				"Procedure_Camera_SetCameraZMin",
				"Procedure_Camera_ResetCameraZMin",
				"Procedure_Camera_SetCameraOrientation",
				"Procedure_Camera_SetCameraPosition",
				"Procedure_Camera_SetTargetPosition",
				"Procedure_Cam_ChangeOrientAngleMax",
				"Procedure_Cam_ChangeOrientAngularSpeed",
				"Procedure_Cam_ChangeChannel",
				"Procedure_Camera_ChangeHard",
				"Procedure_Camera_RepositionHard",
				"Proc_SetPower",
				"Proc_SubPower",
				"Proc_AddPower",
				"Proc_TransparentDisplay",
				"Proc_DefautDisplay",
				"Proc_SetTransparency",
				"Proc_SetDisplayFixFlag",
				"Proc_StartMenuWithPauseGame",
				"Proc_StartMenuWithoutPauseGame",
				"Proc_DisplayVignetteDuring",
				"Proc_SetParticleGeneratorOn",
				"Proc_SetParticleGeneratorOff",
				"Proc_SetParticleGenerator",
				"Proc_SetGenerationModeNone",
				"Proc_SetGenerationModeContinuous",
				"Proc_SetGenerationModeCrenel",
				"Proc_SetGenerationModeProbability",
				"Proc_SetGenerationNumberConstant",
				"Proc_SetGenerationNumberProbabilist",
				"Proc_SetGenerationInfiniteLifeTime",
				"Proc_SetGenerationConstantLifeTime",
				"Proc_SetGenerationProbabilistLifeTime",
				"Proc_DeactivateBut",
				"Proc_ActivateBut",
				"Proc_None",
				"Proc_ChangeComport",
				"Proc_ChangeComportReflex",
				"Proc_ChangeMyComport",
				"Proc_ChangeMyComportReflex",
				"Proc_TurnAbsoluteDirection",
				"Proc_GoAbsoluteDirection",
				"Proc_Accelerate",
				"Proc_Turn",
				"Proc_Turn2",
				"Proc_DeltaTurnPerso",
				"Proc_TurnPerso",
				"Proc_ChangeAction",
				"Proc_ChangeActionRandom",
				"Proc_ReInitWay",
				"Proc_ReInitWayBack",
				"Proc_InitWayWithWp",
				"Proc_SwimPulse",
				"Proc_SkiTurnLeft",
				"Proc_SkiTurnRight",
				"Proc_KillPerso",
				"Proc_SetMechanicalGMTAdhesionCoef",
				"Proc_SetMechanicalGMTAbsorptionCoef",
				"Proc_SetMechanicalGMTFrictionCoef",
				"Proc_SetMechanicalGMTSlideCoef",
				"Proc_SetMechanicalGMTProgressionCoef",
				"Proc_SetMechanicalGMTPenetrationCoef",
				"Proc_SetMechanicalGMTPenetrationMaxCoef",
				"Proc_SetVisualGMTColor",
				"Proc_SetVisualGMTSpecularExponant",
				"Proc_SetVisualGMTSpecularCoef",
				"Proc_SetVisualGMTDiffuseCoef",
				"Proc_SetVisualGMTAmbientCoef",
				"Proc_SetVisualGMTAsChromed",
				"Proc_SetVisualGMTTextureScrollingCoef",
				"Proc_LockVisualGMT",
				"Proc_UnlockVisualGMT",
				"Proc_SetVisualGMTFrame",
				"Proc_FootPath_AddFootPrint",
				"Proc_FootPath_Clear",
				"Proc_ReleaseAllModulesControl",
				"Proc_ConfirmModuleControl",
				"Proc_OptionChangeScreenSize",
				"Proc_OptionChangeDetailsValue",
				"Proc_OptionChangeMusicVolume",
				"Proc_OptionChangeSfxVolume",
				"Proc_OptionChangeVoiceVolume",
				"Proc_SetPersoAbsolutePosition",
				"Proc_RelativeMovePerso",
				"Proc_SetModuleAbsolutePosition",
				"Proc_RelativeMoveModule",
				"Proc_ChangeModuleSighting",
				"Proc_RotatePersoAroundX",
				"Proc_RotatePersoAroundY",
				"Proc_RotatePersoAroundZ",
				"Proc_RotateModuleAroundX",
				"Proc_RotateModuleAroundY",
				"Proc_RotateModuleAroundZ",
				"Proc_ZoomPerso",
				"Proc_ZoomModule",
				"Proc_InvisibleDisplay",
				"Proc_VisibleDisplay",
				"Proc_AddChildToChannel",
				"Proc_RemoveChildFromChannel",
				"Map_vEnableObject",
				"Map_vSetDestination",
				"Map_vResetDestination",
				"Proc_ChangeOneCustomBit",
				"Proc_ChangeManyCustomBits",
				"Proc_StringAddChar",
				"Proc_StringReplaceChar",
				"Proc_StringRemoveChar",
				"Proc_SetWeightlessness",
				"Proc_OrientWallNormal",
				"Proc_SetAnimationGravity",
				"Proc_FixePositionZDM",
				"Proc_FixePositionZDD",
				"Proc_FixePositionZDE",
				"Proc_ChangeLightIntensity",
				"Proc_GiveImpulsionRope",
				"Proc_ChangeSpringConstantForChannel",
				"Proc_RopeHitAWall",
				"Proc_FixePointsDeMagie",
				"Proc_FixePointsDeMagieMax",
				"Proc_InitPointsDeMagie",
				"Proc_InitPointsDeMagieMax",
				"Proc_AddMagicPoints",
				"Proc_AddMagicPointsMax",
				"Proc_SubMagicPoints",
				"Proc_SubMagicPointsMax",
				"Proc_CapsSetCapabilities",
				"Proc_CapsAddCapabilities",
				"Proc_CapsSubCapabilities",
				"SCT_ActivateSector",
				"SCT_DesactivateSector",
				"SCT_ActivateSectorVisibility",
				"SCT_DesactivateSectorVisibility",
				"SCT_ActivateSectorCollisions",
				"SCT_DesactivateSectorCollisions",
				"QuitGame",
				"ReturnToMainMenu",
				"DisplayVignetteAndReturnToMainMenu",
				"DisplayVignetteAndQuitGame",
				"Proc_ActivateOptionnalSectors",
				"Proc_DeactivateOptionnalSectors",
				"Proc_SetGameState",
				"Proc_SetMouseCursorTexture",
				"Proc_RestoreComport",
				"Proc_RestoreMyComport",
				"Proc_SCT_SetTransparence",
				"Proc_setFadeValue",
				"Proc_setFadeTexture",
				"Proc_ActivateFade",
				"Proc_DeactivateFade",
				"Proc_ExecuteFade",
				"Proc_ExecuteFadeMS",
				"Proc_ToggleWideScreen",
				"Proc_EndDialog",
				"Proc_RunDialog",
				"Proc_ScrollDialog",
				"Proc_SelectDialog",
				"Proc_PositionDialog",
				"Proc_DLG_ChangeAction",
				"Proc_DLG_ChangeComport",
				"Proc_DLG_ActivateChannel",
				"Proc_SND_SetSoundEventActivation",
				"Proc_ActivateCreditsMotorMode",
				"Proc_ActivateInventoryMotorMode",
				"Proc_ActivateBinocularMotorMode",
				"Proc_ActivateSaveGameMotorMode",
				"Proc_ActivateLaserPointer",
				"Proc_HideLaserPointer",
				"Proc_SetLaserPointerDistance",
				"Proc_SetLaserPointerTexture",
				"Proc_SetLaserPointerDetailLevel",
				"Proc_SetLaserPointerSniperMode",
				"Proc_SetLaserPointerFlyMode",
				"Proc_FadeRandom",
				"Proc_RopeInitialisation",
				"InterpolationReset",
				"InterpolationOff",
				"InterpolationOn",
				"Proc_GPI_SetDraw",
				"Proc_GPI_SetImage",
				"Proc_GPI_SetToNextImage",
				"Proc_GPI_FadeImage",
				"Proc_GPI_MovingUV",
				"Proc_GPI_Rotation",
				"Proc_GPI_SetText",
				"Proc_GPI_SetTextNumber",
				"Proc_GPI_SlideGamePlayInterface",
				"Proc_HilitePerso",
				"Proc_ResetPersoToInitialState",
				"Proc_SetFlashingSpeed",
				"Proc_SetFlashingTransparent",
				"Proc_SetFlashingNotTransparent",
				"Proc_SetFlashingWithDefaultGMT",
				"Proc_SetFlashingNotWithDefaultGMT",
				"Proc_SetInitialGameFloors",
				"Proc_SetStandardFloor",
				"Proc_ActivateFloorGame",
				"Proc_SetColorPathGoal",
				"Proc_LGT_ChangeColor",
				"Proc_LGT_ChangeIntensity",
				"Proc_LGT_Switch",
				"Proc_LGT_SwitchAll",
				"SetStableEnvironnement",
				"Proc_ActivateMapMotorMode",
				"Proc_ActivateEpoque",
				"Proc_ActivateName",
				"Proc_RopeCareTaker",
				"Proc_ChangeChannelMassForPolySys",
				"Proc_FixNormReelSpeed",
				"Proc_SND_LoadSoundBank",
				"Proc_SND_UnLoadSoundBank",
				"Proc_SND_KillMusic",
				"Proc_SND_KillSoundEffect",
				"Proc_SND_KillVoice",
				"Proc_SND_KillAllSounds",
				"Proc_SND_ChangeMyVolumeEffects",
				"Proc_SND_ChangeVolumeEffects",
				"Proc_SND_ChangeMyVolumeVoice",
				"Proc_SND_ChangeVolumeVoice",
				"Proc_SND_ChangeMyVolumeMusic",
				"Proc_SND_ChangeVolumeMusic",
				"Proc_SND_ChangeAllVolume",
				"Proc_SND_ResetMyVolumeEffects",
				"Proc_SND_ResetMyVolumeVoice",
				"Proc_SND_ResetMyVolumeMusique",
				"Proc_SND_ResetVolumeEffects",
				"Proc_SND_ResetVolumeVoice",
				"Proc_SND_ResetVolumeMusic",
				"Proc_SND_ResetAllVolume",
				"Proc_SND_SetEffectSound",
				"Proc_SND_SwitchMicroPerso",
				"Proc_SND_SwitchOldMicroPerso",
				"SetVignetteIndex",
				"Proc_SND_SaveMusic",
				"Proc_SND_LoadMusic",
				"Proc_SND_FadeMusic",
				"Proc_SND_FadeVoice",
				"Proc_SND_FadeSound",
				"Proc_SND_FadeAll",
				"Proc_SND_FadeObjectAndStop",
				"Proc_SND_FadeInObject",
				"Proc_SND_KillObject",
				"Proc_SetEconomicPersoAbsolutePosition",
				"ScaleReallyModule",
				"Inv_UnselectSelectedObject",
				"Inv_ChangeObjectMaxCapacity",
				"Inv_ChangeJewelState",
				"Inv_ChangeStockLstNumber",
				"Inv_DisplayAnObject",
				"Stats_DisplayMainActorLife",
				"Stats_DisplayMainActorArmor",
				"Stats_DisplayMainActorMagic",
				"Stats_DisplayMainActorSelectedObject",
				"Stats_DisplayMainActorWeapon",
				"Stats_DisplayDragon",
				"Stats_DisplayCash",
				"Fight_ChangeArmor",
				"Fight_ChangeArmorResistance",
				"Fight_RepairArmor",
				"Fight_SubHitPointsEnemy",
				"Magic_ChangeInitialMagicPoints",
				"Magic_ChangeMaxMagicPoints",
				"Magic_InitMagicPointsToInit",
				"Magic_InitMagicPointsToMax",
				"Magic_SelectPrevSpellType",
				"Magic_SelectNextSpellType",
				"Dragon_ChangeInitialNbrDiamonds",
				"Dragon_ChangeMaxNbrDiamonds",
				"Dragon_InitDiamondsToInit",
				"Dragon_InitDiamondsToMax",
				"Dragon_ChangeDragonActor",
				"Demo_ValidateMap",
				"Proc_ChangePersoSighting"
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
				AI_DsgVarType.Way,
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
				AI_DsgVarType.ActionArray,
				AI_DsgVarType.None
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
				AI_InterpretType.Way,
				AI_InterpretType.PersoRef,
				AI_InterpretType.ActionRef, // 25
                AI_InterpretType.EnvironmentRef,
				AI_InterpretType.SuperObjectRef,
				AI_InterpretType.SurfaceRef,
				AI_InterpretType.WayPointRef,
				AI_InterpretType.TextRef, // 30
                AI_InterpretType.FontRef,
				AI_InterpretType.ComportRef,
				AI_InterpretType.ModuleRef,
				AI_InterpretType.SoundEventRef,
				AI_InterpretType.ObjectTableRef, // 35
                AI_InterpretType.GameMaterialRef,
				AI_InterpretType.ParticleGenerator,
				AI_InterpretType.Color,
				AI_InterpretType.ModelRef,
				AI_InterpretType.Caps,
				AI_InterpretType.GraphRef
			};
		}
		#endregion

		#region Conditions
		protected override void InitConditions() {
			Conditions = new AI_Condition[] {
				AI_Condition.Et,
				AI_Condition.Ou,
				AI_Condition.Not,
				AI_Condition.Equal,
				AI_Condition.Different,
				AI_Condition.Lesser,
				AI_Condition.Greater,
				AI_Condition.LesserOrEqual,
				AI_Condition.GreaterOrEqual,
				AI_Condition.PressedBut,
				AI_Condition.JustPressedBut,
				AI_Condition.ReleasedBut,
				AI_Condition.JustReleasedBut,
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
				AI_Condition.CollideMovingZDM,
				AI_Condition.CollideWithGround,
				AI_Condition.CollideWithWall,
				AI_Condition.CollideWithNothing,
				AI_Condition.CollideWithSlope,
				AI_Condition.CollideWithAttic,
				AI_Condition.CollideWithCeiling,
				AI_Condition.CollideWithPerso,
				AI_Condition.CollideWithTrap,
				AI_Condition.CollideWithPoint,
				AI_Condition.CollideWithTriangle,
				AI_Condition.CollideWithEdge,
				AI_Condition.CollideWithSphere,
				AI_Condition.CollideWithAlignedBox,
				AI_Condition.ZDMCollideWithGround,
				AI_Condition.ZDMCollideWithWall,
				AI_Condition.ZDMCollideWithNothing,
				AI_Condition.ZDMCollideWithSlope,
				AI_Condition.ZDMCollideWithAttic,
				AI_Condition.ZDMCollideWithCeiling,
				AI_Condition.InEnvironmentAir,
				AI_Condition.InEnvironmentWater,
				AI_Condition.InEnvironmentFire,
				AI_Condition.IsPersoInList,
				AI_Condition.ListEmptyTest,
				AI_Condition.IsTimeElapsed,
				AI_Condition.IsInComport,
				AI_Condition.IsInReflex,
				AI_Condition.ChangeActionEnable,
				AI_Condition.IsInAction,
				AI_Condition.IsTypeOfGMTCollide,
				AI_Condition.WallIsTypeOfGMTCollide, // New on PS2
				AI_Condition.ObstacleIsTypeOfGMTCollide, // New on PS2
				AI_Condition.IsThereMechEvent,
				AI_Condition.IsValidObject,
				AI_Condition.IsValidWayPoint,
				AI_Condition.IsValidGMT,
				AI_Condition.IsValidAction,
				AI_Condition.IsValidWay,
				AI_Condition.InTopOfJump,
				AI_Condition.CanSwim,
				AI_Condition.CanSwimOnSurface,
				AI_Condition.CanSwimUnderWater,
				AI_Condition.IsNotOutOfDepth,
				AI_Condition.IsCompletelyOutOfWater,
				AI_Condition.LSY_IsSpeechOver,
				AI_Condition.SeePerso,
				AI_Condition.SeePosition,
				AI_Condition.SeePositionWithOffset,
				AI_Condition.SeePersoWithOffset,
				AI_Condition.IsSectorInTranslation,
				AI_Condition.IsSectorInRotation,
				AI_Condition.IsCameraInState,
				AI_Condition.IsCameraInInitialState,
				AI_Condition.IsCameraActive,
				AI_Condition.IsCameraViewportOwner,
				AI_Condition.IsCameraTargetVisible,
				AI_Condition.IsCameraTargetMoving,
				AI_Condition.IsCameraReachedItsOptPos,
				AI_Condition.IsCameraInAlphaOrientation,
				AI_Condition.IsCameraInTetaOrientation,
				AI_Condition.IsSurfaceHeightMoving,
				AI_Condition.TestPower,
				AI_Condition.IsActivable,
				AI_Condition.HasAtLeft,
				AI_Condition.HasAtRight,
				AI_Condition.HasBehind,
				AI_Condition.HasInFront,
				AI_Condition.HasAbove,
				AI_Condition.HasBelow,
				AI_Condition.UserEvent_IsSet,
				AI_Condition.IsPersoLightOn,
				AI_Condition.IsPersoLightPulseOn,
				AI_Condition.IsPersoLightGyroPhareOn,
				AI_Condition.IsZDMCollideWithObstacle,
				AI_Condition.IsZDMCollideWithWall,
				AI_Condition.IsZDMCollideWithGround,
				AI_Condition.IsCustomBitSet,
				AI_Condition.CollisionWP,
				AI_Condition.IsGiBlock,
				AI_Condition.IsNullVector,
				AI_Condition.HasTheCapability,
				AI_Condition.HasTheCapabilityNumber,
				AI_Condition.PersoHasTheCapability,
				AI_Condition.PersoHasTheCapabilityNumber,
				AI_Condition.HasOneOfTheCapabilities,
				AI_Condition.PersoHasOneOfTheCapabilities,
				AI_Condition.GetAction,
				AI_Condition.Inv_InventairePlein,
				AI_Condition.Inv_TrouverObjet,
				AI_Condition.GetDialogStatus,
				AI_Condition.DLG_IsDialogOver,
				AI_Condition.DLG_IsScrollingOver,
				AI_Condition.ActionFinished,
				AI_Condition.SCT_ActorInSector,
				AI_Condition.CollisionSphereSphere,
				AI_Condition.SectorActive,
				AI_Condition.IsSoundFinished,
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

		#region Keywords
		protected override void InitKeywords() {
			Keywords = new AI_Keyword[] {
				AI_Keyword.If,
				AI_Keyword.Then,
				AI_Keyword.Else,
				AI_Keyword.EngineGoto,
				AI_Keyword.Me,
				AI_Keyword.MainActor,
				AI_Keyword.World,
				AI_Keyword.Nobody,
				AI_Keyword.Nowhere,
				AI_Keyword.Noway,
				AI_Keyword.CapsNull,
				AI_Keyword.NoGraph,
			};
		}
		#endregion

		#region Meta Actions
		protected override void InitMetaActions() {
			MetaActions = new AI_MetaAction[] {
				AI_MetaAction.IncrementalTurn2,
				AI_MetaAction.IncrementalTurn,
				AI_MetaAction.Pursuit,
				AI_MetaAction.DeltaPursuit,
				AI_MetaAction.ExecuteAction,
				AI_MetaAction.ExecuteActionPerso,
				AI_MetaAction.WaitEndOfAction,
				AI_MetaAction.WaitEndOfActionPerso,
				AI_MetaAction.WaitEndOfAnim,
				AI_MetaAction.WaitEndOfAnimPerso,
				AI_MetaAction.SpeakAndWaitEnd,
				AI_MetaAction.LSY_SpeakPersoAndWaitEnd,
				AI_MetaAction.ExecuteActionAndTakeObject,
				AI_MetaAction.ExecuteActionAndPutObject,
				AI_MetaAction.ExecuteActionAndLaunchObject,
				AI_MetaAction.ExecuteActionAndGenerateObject,
				AI_MetaAction.AdjustPersoOnObjectAxes,
				AI_MetaAction.AdjustPersoOnObject,
				AI_MetaAction.RunAway,
				AI_MetaAction.IncrementalTurnPosition,
				AI_MetaAction.IncrementalDeltaTurnPerso2,
				AI_MetaAction.IncrementalTurnPerso2,
				AI_MetaAction.IncrementalDeltaTurnPerso,
				AI_MetaAction.IncrementalTurnPerso,
				AI_MetaAction.RunAwayWP,
				AI_MetaAction.GoToWP,
				AI_MetaAction.GoToWPXY,
				AI_MetaAction.GoOverWay,
				AI_MetaAction.GoOverWay3D,
				AI_MetaAction.GoOverWayBack,
				AI_MetaAction.GoToNextWPOfWay,
				AI_MetaAction.GoToNextWPOfWayBack,
				AI_MetaAction.GoToPosition,
				AI_MetaAction.GoToPositionXY,
				AI_MetaAction.GoOverShortestPath,
				AI_MetaAction.GoOverShortestPathBezier,
				AI_MetaAction.MoveLift,
				AI_MetaAction.Wait,
				AI_MetaAction.WaitBoolean,
				AI_MetaAction.WaitNextTime,
				AI_MetaAction.WaitInteger,
				AI_MetaAction.WaitReal,
				AI_MetaAction.IncrementalTurnPositionW,
				AI_MetaAction.IncrementalTurnPositionWPerso,
				AI_MetaAction.IncrementalTurnPersoW,
				AI_MetaAction.IncrementalTurnPersoWPerso,
				AI_MetaAction.IncrementalTurnPersoWPersoW,
				AI_MetaAction.IncrementalTurnPersoWPersoWPerso,
				AI_MetaAction.Camera_GoToPosition,
				AI_MetaAction.Camera_GoToWP,
				AI_MetaAction.Camera_GoOverWay,
				AI_MetaAction.Camera_GoOverWayBack,
				AI_MetaAction.Camera_GoToNextWPOfWay,
				AI_MetaAction.Camera_GoToNextWPOfWayBack,
				AI_MetaAction.Camera_StayInWay,
				AI_MetaAction.Camera_GoToFromTarget,
				AI_MetaAction.Camera_Shake,
				AI_MetaAction.Camera_WaitUntil,
				AI_MetaAction.RotateModule,
				AI_MetaAction.RotateModuleToPerso,
				AI_MetaAction.SwimToSurface,
				AI_MetaAction.WaitEndOfFade,
				AI_MetaAction.SendSoundRequestAndWait,
				AI_MetaAction.SendVoiceRequestAndWait,
				AI_MetaAction.SendVoiceRequestPersoAndWait,
				AI_MetaAction.GoToPosition2,
				AI_MetaAction.IncrementalTurnAbsDirection,
				AI_MetaAction.IncTurnPersoAbsDirection,
				AI_MetaAction.WaitNbFrames,
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
				AI_Field.CollisionFlag,
				AI_Field.ShadowScaleX,
				AI_Field.ShadowScaleY,
			};
		}
		#endregion

		#region Functions
		protected override void InitFunctions() {
			Functions = new AI_Function[] {
				AI_Function.GetPersoAbsolutePosition,
				AI_Function.GetAngleAroundZToPerso,
				AI_Function.GetAngleAroundZToPosition,
				AI_Function.GetWPAbsolutePosition,
				AI_Function.DistanceToPerso,
				AI_Function.DistanceXToPerso,
				AI_Function.DistanceYToPerso,
				AI_Function.DistanceZToPerso,
				AI_Function.DistanceXYToPerso,
				AI_Function.DistanceXZToPerso,
				AI_Function.DistanceYZToPerso,
				AI_Function.DistanceToWP,
				AI_Function.CloserWP,
				AI_Function.DistanceToPosition,
				AI_Function.DistanceToPersoCenter,
				AI_Function.DistanceXToPersoCenter,
				AI_Function.DistanceYToPersoCenter,
				AI_Function.DistanceZToPersoCenter,
				AI_Function.DistanceXYToPersoCenter,
				AI_Function.DistanceXZToPersoCenter,
				AI_Function.DistanceYZToPersoCenter,
				AI_Function.Real,
				AI_Function.Int,
				AI_Function.Sinus,
				AI_Function.Cosinus,
				AI_Function.Square,
				AI_Function.SquareRoot,
				AI_Function.RandomInt,
				AI_Function.RandomReal,
				AI_Function.MinimumReal,
				AI_Function.MaximumReal,
				AI_Function.DegreeToRadian,
				AI_Function.RadianToDegree,
				AI_Function.AbsoluteValue,
				AI_Function.AbsoluteInteger,
				AI_Function.GetHitPoints,
				AI_Function.AddAndGetHitPoints,
				AI_Function.SubAndGetHitPoints,
				AI_Function.GetHitPointsMax,
				AI_Function.AddAndGetHitPointsMax,
				AI_Function.SubAndGetHitPointsMax,
				AI_Function.GetTime,
				AI_Function.GetElapsedTime,
				AI_Function.ListSize,
				AI_Function.GivePersoInList,
				AI_Function.InputAnalogicValue,
				AI_Function.GenerateObject,
				AI_Function.DepthEnv,
				AI_Function.AltitudeEnv,
				AI_Function.GetFather,
				AI_Function.GetCamera,
				AI_Function.GetMainCamera,
				AI_Function.GetDistanceCameraTarget,
				AI_Function.GetTargetCamera,
				AI_Function.GetDistMinCamera,
				AI_Function.GetDistMaxCamera,
				AI_Function.GetBoundDistMin,
				AI_Function.GetBoundDistMax,
				AI_Function.GetAlphaCamera,
				AI_Function.GetShiftAlphaCamera,
				AI_Function.GetTetaCamera,
				AI_Function.GetShiftTetaCamera,
				AI_Function.GetOrientationXCamera,
				AI_Function.GetOrientationYCamera,
				AI_Function.GetOrientationZCamera,
				AI_Function.GetCamLinearSpeedMin,
				AI_Function.GetCamLinearSpeedMax,
				AI_Function.GetCamAngularSpeed,
				AI_Function.GetTgtLinearSpeed,
				AI_Function.GetFocalCamera,
				AI_Function.GetStateCamera,
				AI_Function.GetFlagCamera,
				AI_Function.GetActivationCamera,
				AI_Function.GetShiftPersoCamera,
				AI_Function.GetShiftVertexCamera,
				AI_Function.GetLookVertexCamera,
				AI_Function.ComputeAlphaAngleCameraTarget,
				AI_Function.ComputeTetaAngleCameraTarget,
				AI_Function.GetOrientationSpeed,
				AI_Function.GetOrientationLimitX,
				AI_Function.GetOrientationLimitY,
				AI_Function.GetOrientationLimitZ,
				AI_Function.ComputeTargetPosition,
				AI_Function.GetNormSpeed,
				AI_Function.GetAlphaPas,
				AI_Function.GetThetaPas,
				AI_Function.GetAlpha,
				AI_Function.GetTheta,
				AI_Function.GetVectorNorm,
				AI_Function.AbsoluteVector,
				AI_Function.RelativeVector,
				AI_Function.GetEnvironmentToxicity,
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
				AI_Function.GetCurrentCollidedGMT,
				AI_Function.GetMechanicalGMTAdhesionCoef,
				AI_Function.GetMechanicalGMTAbsorptionCoef,
				AI_Function.GetMechanicalGMTFrictionCoef,
				AI_Function.GetMechanicalGMTSlideCoef,
				AI_Function.GetMechanicalGMTProgressionCoef,
				AI_Function.GetMechanicalGMTPenetrationCoef,
				AI_Function.GetMechanicalGMTPenetrationMaxCoef,
				AI_Function.GetVisualGMTColor,
				AI_Function.GetVisualGMTSpecularCoef,
				AI_Function.GetVisualGMTSpecularExponent,
				AI_Function.GetVisualGMTDiffuseCoef,
				AI_Function.GetVisualGMTAmbientCoef,
				AI_Function.GetVisualGMTTextureScrollingCoefU,
				AI_Function.GetVisualGMTTextureScrollingCoefV,
				AI_Function.GetVisualGMTFrame,
				AI_Function.GetVisualGMTNumberOfFrames,
				AI_Function.LitPointsDeMagie,
				AI_Function.LitPointsDeMagieMax,
				AI_Function.AjouteEtLitPointsDeMagie,
				AI_Function.AjouteEtLitPointsDeMagieMax,
				AI_Function.EnleveEtLitPointsDeMagie,
				AI_Function.EnleveEtLitPointsDeMagieMax,
				AI_Function.PersoLePlusProche,
				AI_Function.PersoLePlusProcheAvecAngle,
				AI_Function.ReseauLitIndexCourant,
				AI_Function.ReseauForceIndexCourant,
				AI_Function.ReseauLitPremierIndex,
				AI_Function.ReseauLitDernierIndex,
				AI_Function.ReseauIncrementIndex,
				AI_Function.ReseauDecrementIndex,
				AI_Function.ReseauLitWPAIndex,
				AI_Function.ReseauLitCapaciteLiaison,
				AI_Function.ReseauChangeCapaciteLiaison,
				AI_Function.ReseauCheminLePlusCourt,
				AI_Function.NetworkAllocateGraphToMSWay,
				AI_Function.NetworkGetIndexOfWPInMSWay,
				AI_Function.NetworkForceWPToCurrent,
				AI_Function.ReseauWPLePlusProche,
				AI_Function.NetworkWPCloserOrientation,
				AI_Function.GetCapabilities,
				AI_Function.CapabilityAtBitNumber,
				AI_Function.VitesseHorizontaleDuPerso,
				AI_Function.VitesseVerticaleDuPerso,
				AI_Function.LitPositionZDM,
				AI_Function.LitPositionZDE,
				AI_Function.LitPositionZDD,
				AI_Function.GetDeltaT,
				AI_Function.VecteurLocalToGlobal,
				AI_Function.VecteurGlobalToLocal,
				AI_Function.GetNormalCollideVector,
				AI_Function.GetCollidePoint,
				AI_Function.GetCollisionPoint,
				AI_Function.GetCollisionVector,
				AI_Function.CrossProduct,
				AI_Function.NormalizeVector,
				AI_Function.GetLastTraversedMaterial,
				AI_Function.GetModuleAbsolutePosition,
				AI_Function.GetModuleRelativePosition,
				AI_Function.GetModuleZoomFactor,
				AI_Function.GetPersoZoomFactor,
				AI_Function.GetPersoSighting,
				AI_Function.GetModuleSighting,
				AI_Function.GetPlayerName,
				AI_Function.GetStringCharAt,
				AI_Function.GetLastCollisionActor,
				AI_Function.ComputeRebondVector,
				AI_Function.ComputeRebondVector2,
				AI_Function.VectorAngle,
				AI_Function.VectorCos,
				AI_Function.VectorSin,
				AI_Function.GetNormalGroundVector,
				AI_Function.PositionAbsolueCanal,
				AI_Function.DistancePersoToCanal,
				AI_Function.CollisionRopeSphere,
				AI_Function.TBL_WayPointResearchInTable,
				AI_Function.TBL_PersoResearchInTable,
				AI_Function.MTH_DotProduct,
				AI_Function.GetRopeSpeedVector,
				AI_Function.GetRopeChannelPosition,
				AI_Function.MTH_RoundRealToInteger,
				AI_Function.GetCurrectAnimFrame,
				AI_Function.PersoUnderCursor,
				AI_Function.XCoordOfCursor,
				AI_Function.YCoordOfCursor,
				AI_Function.MTH_Puissance,
				AI_Function.MTH_Modulo,
				AI_Function.RelativeXPositionOfPerso,
				AI_Function.RelativeYPositionOfPerso,
				AI_Function.RelativeZPositionOfPerso,
				AI_Function.RelativePositionOfPerso,
				AI_Function.DistanceCaracToWP,
				AI_Function.GetGameState,
				AI_Function.Inv_AjouterObjet,
				AI_Function.Inv_EnleverObjet,
				AI_Function.Inv_LireOr,
				AI_Function.Inv_AjouterOr,
				AI_Function.Inv_EnleverOr,
				AI_Function.Inv_LireArme,
				AI_Function.Inv_ChangerArme,
				AI_Function.Inv_LireMagie,
				AI_Function.Inv_ChangerMagie,
				AI_Function.Inv_LireObjet,
				AI_Function.Inv_SelectionnerObjet,
				AI_Function.Inv_UtiliserItem,
				AI_Function.Inv_LireQuantiteObjet,
				AI_Function.Inv_EnleverQuantiteObjet,
				AI_Function.Inv_LireCapaciteMaximumObjet,
				AI_Function.GetLaserPointerDirection,
				AI_Function.GetLaserPointerDistance,
				AI_Function.GetLaserPointerDistaneToStaticMap,
				AI_Function.SpeedChannel,
				AI_Function.ExecuteFloorGame,
				AI_Function.DifferentFromCurrentImage,
				AI_Function.IsDrawn,
				AI_Function.DLG_PersoParle,
				AI_Function.WP_IndexOfClosestWPinArray,
				AI_Function.eReduceIndexOfVectorArray,
				AI_Function.TBL_InitArray,
				AI_Function.SND_GetMicroPerso,
				AI_Function.SND_GetMicroOrientationPerso,
				AI_Function.FightRemoveHitPointsMainActorWithArmor,
				AI_Function.FightRemoveAndReadHitPointsEnemy,
				AI_Function.FightReadArmorResistance,
				AI_Function.FightGetNumberOfArmors,
				AI_Function.Magic_ReadMagicPoints,
				AI_Function.Magic_AddMagicPoints,
				AI_Function.Magic_RemoveMagicPoints,
				AI_Function.Magic_ReadMagicPointsMax,
				AI_Function.Magic_ReadCurrentSpell,
				AI_Function.Dragon_ReadNbrDiamonds,
				AI_Function.Dragon_AddDiamonds,
				AI_Function.Dragon_RemoveDiamonds,
				AI_Function.Dragon_ReadNbrDiamondsMax,
				AI_Function.POS_GetChannelPosition,
				AI_Function.SendSoundRequest,
				AI_Function.SendVoiceRequest,
				AI_Function.SendMusicRequest,
				AI_Function.GiveMeTheGround,
			};
		}
		#endregion

	}
}
