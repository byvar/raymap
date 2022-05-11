﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_Hype_PC : AI_Types_Hype_PS2 {
		// Re-checked

		// Fields list: same as PS2

		#region Conditions
		// Slightly less than PS2
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
				// AI_Condition.WallIsTypeOfGMTCollide, // Not on PC
				// AI_Condition.ObstacleIsTypeOfGMTCollide, // Not on PC
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

		#region Functions
		protected override void InitFunctions() {
			base.InitFunctions();
			var funcList = Functions.ToList();
			// These two functions were added on PS2
			funcList.Remove(AI_Function.GetLaserPointerDistaneToStaticMap);
			funcList.Remove(AI_Function.SND_GetMicroOrientationPerso);
			Functions = funcList.ToArray();
		}
		#endregion

		#region Procedures
		protected override void InitProcedures() {
			Procedures = new AI_Procedure[] {
				AI_Procedure.PlayerIsDead,
				AI_Procedure.ChangeInitPosition,
				AI_Procedure.SetMainActor,
				AI_Procedure.ActivateObject,
				AI_Procedure.DesactivateObject,
				AI_Procedure.ChangeMap,
				AI_Procedure.SetActionReturn,
				AI_Procedure.FactorAnimationFrameRate,
				AI_Procedure.PlayerIsDeadWithOption,
				AI_Procedure.BecomesSonOfPerso,
				AI_Procedure.BecomesFatherOfPerso,
				AI_Procedure.FillListWithSons,
				AI_Procedure.ActivateObjectOnPosition,
				AI_Procedure.ChangeMapAtPosition,
				AI_Procedure.PlayerIsDeadWithPlacement,
				AI_Procedure.AllowDynamLOD,
				AI_Procedure.ForbidDynamLOD,
				AI_Procedure.ResetSpeed,
				AI_Procedure.ResetOrientation,
				AI_Procedure.SetRotationAxe,
				AI_Procedure.SetAlphaAngle,
				AI_Procedure.SetRotationAngleStep,
				AI_Procedure.SetThetaAngle,
				AI_Procedure.SetImpulse,
				AI_Procedure.SetNormSpeed,
				AI_Procedure.AddNormSpeed,
				AI_Procedure.MulNormSpeed,
				AI_Procedure.SetDirectionSpeed,
				AI_Procedure.AddDirectionSpeed,
				AI_Procedure.SetVectorSpeed,
				AI_Procedure.AddWind,
				AI_Procedure.SetDynamScalar,
				AI_Procedure.SetTarget,
				AI_Procedure.TurnAround,
				AI_Procedure.GoRelative,
				AI_Procedure.GoInDirection,
				AI_Procedure.TurnLeft,
				AI_Procedure.TurnRight,
				AI_Procedure.TurnUp,
				AI_Procedure.TurnLateralLeft,
				AI_Procedure.TurnLateralright,
				AI_Procedure.Brake,
				AI_Procedure.AccelTurbo,
				AI_Procedure.TurnDown,
				AI_Procedure.Pulse,
				AI_Procedure.StonePulse,
				AI_Procedure.Jump,
				AI_Procedure.JumpAbsolute,
				AI_Procedure.JumpWithoutAddingSpeed,
				AI_Procedure.Fire,
				AI_Procedure.GoTarget,
				AI_Procedure.ReachTarget,
				AI_Procedure.SetHitPoints,
				AI_Procedure.SetHitPointsToInitValue,
				AI_Procedure.SetHitPointsToMaxValue,
				AI_Procedure.AddHitPoints,
				AI_Procedure.SubHitPoints,
				AI_Procedure.SetHitPointsMax,
				AI_Procedure.SetHitPointsMaxToInitValue,
				AI_Procedure.SetHitPointsMaxToMaxValue,
				AI_Procedure.AddHitPointsMax,
				AI_Procedure.SubHitPointsMax,
				AI_Procedure.ListAffectPersoZDD,
				AI_Procedure.ListAffectModuleZDD,
				AI_Procedure.ListAffectPersoZDE,
				AI_Procedure.ListAffectModuleZDE,
				AI_Procedure.ListAffectPersoTypeZDE,
				AI_Procedure.ListAffectModuleTypeZDE,
				AI_Procedure.ListAffectTypeZDEWithTypeZDE,
				AI_Procedure.AddPersoInList,
				AI_Procedure.AddPersoInListAt,
				AI_Procedure.DeletePersoAtInList,
				AI_Procedure.FindPersoAndDeleteInList,
				AI_Procedure.ListSelect,
				AI_Procedure.ListUnSelect,
				AI_Procedure.ListSort,
				AI_Procedure.ListSortByFamily,
				AI_Procedure.FillListWithAllPersoOfAFamily,
				AI_Procedure.ListUnion,
				AI_Procedure.ListInter,
				AI_Procedure.ListDiff,
				AI_Procedure.ListAdd,
				AI_Procedure.SwapLinkTableObjects,
				AI_Procedure.ChangeCurrentObjectTable,
				AI_Procedure.BuildObjectTableFromTableAndString,
				AI_Procedure.LSY_StartSpeech,
				AI_Procedure.LSY_StopSpeech,
				AI_Procedure.TakeModuleControl,
				AI_Procedure.ReleaseModuleControl,
				AI_Procedure.InitModuleControlWithAnimTranslation,
				AI_Procedure.InitModuleControlWithAnimRotation,
				AI_Procedure.RotateSector,
				AI_Procedure.RotateSectorLocalX,
				AI_Procedure.RotateSectorLocalY,
				AI_Procedure.RotateSectorLocalZ,
				AI_Procedure.LevelSaveRotationSector,
				AI_Procedure.PlayerSaveRotationSector,
				AI_Procedure.TranslateSector,
				AI_Procedure.TranslateLocalSector,
				AI_Procedure.SaveGame,
				AI_Procedure.IncHistoryAndSaveGame,
				AI_Procedure.SaveAllGameValues,
				AI_Procedure.EnableEscape,
				AI_Procedure.ResetButtonState,
				AI_Procedure.ActivateChannel,
				AI_Procedure.DeactivateChannel,
				AI_Procedure.FogOn,
				AI_Procedure.FogOff,
				AI_Procedure.SetFogColor,
				AI_Procedure.SetFogNearFarInf,
				AI_Procedure.PersoLightOn,
				AI_Procedure.PersoLightOff,
				AI_Procedure.SetPersoLightColor,
				AI_Procedure.SetPersoLightNearFar,
				AI_Procedure.SetPersoLightLittleBigAlpha,
				AI_Procedure.SetPersoLightGyrophare,
				AI_Procedure.SetPersoLightPulse,
				AI_Procedure.SetPersoLightParalleleType,
				AI_Procedure.SetPersoLightSphericalType,
				AI_Procedure.SetPersoLightHotSpotType,
				AI_Procedure.SetPersoLightAmbientType,
				AI_Procedure.DYL_ChangeGraduallyIntensity,
				AI_Procedure.DYL_CopyStaticToDynamic,
				AI_Procedure.AddSurfaceHeight,
				AI_Procedure.MoveSurfaceHeight,
				AI_Procedure.LevelSaveMovingSurface,
				AI_Procedure.PlayerSaveMovingSurface,
				AI_Procedure.SendSoundRequest,
				AI_Procedure.SendVoiceRequest,
				AI_Procedure.SendMusicRequest,
				AI_Procedure.DisplaylValue,
				AI_Procedure.DisplaylChrono,
				AI_Procedure.DisplayString,
				AI_Procedure.ActivateString,
				AI_Procedure.EraseString,
				AI_Procedure.Camera_UpdatePosition,
				AI_Procedure.Camera_Reset,
				AI_Procedure.Camera_ActSctUpdPatch,
				AI_Procedure.Camera_RestoreAfterTrame,
				AI_Procedure.Camera_CancelRestoreTrame,
				AI_Procedure.Camera_ShowInfo,
				AI_Procedure.Camera_ChangeCamera,
				AI_Procedure.Camera_ActiveCamera,
				AI_Procedure.Camera_ChangeDistance,
				AI_Procedure.Camera_ChangeBoundDist,
				AI_Procedure.Camera_ChangeCamLinearSpeed,
				AI_Procedure.Camera_ChangeCamLinearInertia,
				AI_Procedure.Camera_ChangeCamAngularSpeed,
				AI_Procedure.Camera_ChangeCamAngularInertia,
				AI_Procedure.Camera_ChangeTgtLinearSpeed,
				AI_Procedure.Camera_MoveTarget,
				AI_Procedure.Camera_MoveLookTarget,
				AI_Procedure.Camera_MoveShift,
				AI_Procedure.Camera_ChangeAngle,
				AI_Procedure.Camera_Move,
				AI_Procedure.Camera_Turn,
				AI_Procedure.Camera_ManageSavePos,
				AI_Procedure.Camera_ChangeState,
				AI_Procedure.Camera_ChangeFocal,
				AI_Procedure.Camera_ActivateFocalParsing,
				AI_Procedure.Camera_ChangeTargetedPerso,
				AI_Procedure.Camera_ChangeTargetedChannel,
				AI_Procedure.Camera_StopTargettingChannel,
				AI_Procedure.Camera_RestoreInitialTargetedPerso,
				AI_Procedure.Camera_Shake,
				AI_Procedure.Camera_SetPosition,
				AI_Procedure.Camera_ChangeFlag,
				AI_Procedure.Camera_SavePositionAsOptimal,
				AI_Procedure.Camera_SetAdditionnalTarget,
				AI_Procedure.Camera_SetApexParameters,
				AI_Procedure.Camera_SetCameraZMax,
				AI_Procedure.Camera_ResetCameraZMax,
				AI_Procedure.Camera_SetCameraZMin,
				AI_Procedure.Camera_ResetCameraZMin,
				AI_Procedure.Camera_SetCameraOrientation,
				AI_Procedure.Camera_SetCameraPosition,
				AI_Procedure.Camera_SetTargetPosition,
				AI_Procedure.Camera_ChangeOrientationAngleMax,
				AI_Procedure.Camera_ChangeOrientationAngularSpeed,
				AI_Procedure.Camera_ChangeChannel,
				AI_Procedure.Camera_ChangeHard,
				AI_Procedure.Camera_RepositionHard,
				AI_Procedure.SetPower,
				AI_Procedure.SubPower,
				AI_Procedure.AddPower,
				AI_Procedure.TransparentDisplay,
				AI_Procedure.DefaultDisplay,
				AI_Procedure.SetTransparency,
				AI_Procedure.DisplayFixFlag,
				AI_Procedure.StartMenuWithPauseGame,
				AI_Procedure.StartMenuWithoutPauseGame,
				AI_Procedure.DisplayVignetteDuringTime,
				AI_Procedure.SetParticleGeneratorOn,
				AI_Procedure.SetParticleGeneratorOff,
				AI_Procedure.SetParticleGenerator,
				AI_Procedure.SetGenerationModeNone,
				AI_Procedure.SetGenerationModeContinuous,
				AI_Procedure.SetGenerationModeCrenel,
				AI_Procedure.SetGenerationModeProbability,
				AI_Procedure.SetGenerationNumberConstant,
				AI_Procedure.SetGenerationNumberProbabilist,
				AI_Procedure.SetGenerationInfiniteLifeTime,
				AI_Procedure.SetGenerationConstantLifeTime,
				AI_Procedure.SetGenerationProbabilistLifeTime,
				AI_Procedure.DeactivateBut,
				AI_Procedure.ActivateBut,
				AI_Procedure.None,
				AI_Procedure.ChangeComport,
				AI_Procedure.ChangeComportReflex,
				AI_Procedure.ChangeMyComport,
				AI_Procedure.ChangeMyComportReflex,
				AI_Procedure.TurnAbsoluteDirection,
				AI_Procedure.GoAbsoluteDirection,
				AI_Procedure.Accelerate,
				AI_Procedure.Turn,
				AI_Procedure.Turn2,
				AI_Procedure.DeltaTurnPerso,
				AI_Procedure.TurnPerso,
				AI_Procedure.ChangeAction,
				AI_Procedure.ChangeActionRandom,
				AI_Procedure.ReInitWay,
				AI_Procedure.ReInitWayBack,
				AI_Procedure.InitWayWithWp,
				AI_Procedure.SwimPulse,
				AI_Procedure.Ski_TurnLeft,
				AI_Procedure.Ski_TurnRight,
				AI_Procedure.KillPerso,
				AI_Procedure.SetMechanicalGMTAdhesionCoef,
				AI_Procedure.SetMechanicalGMTAbsorptionCoef,
				AI_Procedure.SetMechanicalGMTFrictionCoef,
				AI_Procedure.SetMechanicalGMTSlideCoef,
				AI_Procedure.SetMechanicalGMTProgressionCoef,
				AI_Procedure.SetMechanicalGMTPenetrationCoef,
				AI_Procedure.SetMechanicalGMTPenetrationMaxCoef,
				AI_Procedure.SetVisualGMTColor,
				AI_Procedure.SetVisualGMTSpecularExponent,
				AI_Procedure.SetVisualGMTSpecularCoef,
				AI_Procedure.SetVisualGMTDiffuseCoef,
				AI_Procedure.SetVisualGMTAmbientCoef,
				AI_Procedure.SetVisualGMTAsChromed,
				AI_Procedure.SetVisualGMTTextureScrollingCoef,
				AI_Procedure.LockVisualGMT,
				AI_Procedure.UnlockVisualGMT,
				AI_Procedure.SetVisualGMTFrame,
				AI_Procedure.FootPath_AddFootPrint,
				AI_Procedure.FootPath_Clear,
				AI_Procedure.ReleaseAllModulesControl,
				AI_Procedure.ConfirmModuleControl,
				AI_Procedure.OptionChangeScreenSize,
				AI_Procedure.OptionChangeDetailsValue,
				AI_Procedure.OptionChangeMusicVolume,
				AI_Procedure.OptionChangeSfxVolume,
				AI_Procedure.OptionChangeVoiceVolume,
				AI_Procedure.SetPersoAbsolutePosition,
				AI_Procedure.RelativeMovePerso,
				AI_Procedure.SetModuleAbsolutePosition,
				AI_Procedure.RelativeMoveModule,
				AI_Procedure.ChangeModuleSighting,
				AI_Procedure.RotatePersoAroundX,
				AI_Procedure.RotatePersoAroundY,
				AI_Procedure.RotatePersoAroundZ,
				AI_Procedure.RotateModuleAroundX,
				AI_Procedure.RotateModuleAroundY,
				AI_Procedure.RotateModuleAroundZ,
				AI_Procedure.ZoomPerso,
				AI_Procedure.ZoomModule,
				AI_Procedure.Invisible,
				AI_Procedure.Visible,
				AI_Procedure.AddChildToChannelOfActor,
				AI_Procedure.RemoveChildFromChannelOfActor,
				AI_Procedure.vEnableMapObject,
				AI_Procedure.vSetDestination,
				AI_Procedure.vResetDestination,
				AI_Procedure.ChangeOneCustomBit,
				AI_Procedure.ChangeManyCustomBits,
				AI_Procedure.StringAddChar,
				AI_Procedure.StringReplaceChar,
				AI_Procedure.StringRemoveChar,
				AI_Procedure.Weightlessness,
				AI_Procedure.OrientWallNormal,
				AI_Procedure.SetAnimationGravity,
				AI_Procedure.FixePositionZDM,
				AI_Procedure.FixePositionZDD,
				AI_Procedure.FixePositionZDE,
				AI_Procedure.ChangeLightIntensity,
				AI_Procedure.GiveImpulsionRope,
				AI_Procedure.ChangeSpringConstantForChannel,
				AI_Procedure.RopeHitAWall,
				AI_Procedure.FixePointsDeMagie,
				AI_Procedure.FixePointsDeMagieMax,
				AI_Procedure.InitPointsDeMagie,
				AI_Procedure.InitPointsDeMagieMax,
				AI_Procedure.AjoutePointsDeMagie,
				AI_Procedure.AjoutePointsDeMagieMax,
				AI_Procedure.EnlevePointsDeMagie,
				AI_Procedure.EnlevePointsDeMagieMax,
				AI_Procedure.SetCapabilities,
				AI_Procedure.AddCapabilities,
				AI_Procedure.SubCapabilities,
				AI_Procedure.ActiveSector,
				AI_Procedure.DesactiveSector,
				AI_Procedure.ActiveSectorVisibility,
				AI_Procedure.DesactiveSectorVisibility,
				AI_Procedure.ActiveSectorCollisions,
				AI_Procedure.DesactiveSectorCollisions,
				AI_Procedure.QuitGame,
				AI_Procedure.ReturnToMainMenu,
				AI_Procedure.DisplayVignetteAndReturnToMainMenu,
				AI_Procedure.DisplayVignetteAndQuitGame,

				AI_Procedure.ActivateOptSectors,
				AI_Procedure.DeactivateOptSectors,
				AI_Procedure.SetGameState,
				AI_Procedure.SetMouseCursorTexture,
				AI_Procedure.RestoreComport,
				AI_Procedure.RestoreMyComport,
				AI_Procedure.SCT_SetTransparence,
				AI_Procedure.setFadeValue,
				AI_Procedure.setFadeTexture,
				AI_Procedure.setFadeOn,
				AI_Procedure.setFadeOff,
				AI_Procedure.StartFade,
				AI_Procedure.StartFadeMS,
				AI_Procedure.ToggleWideScreen,
				AI_Procedure.EndDialog,
				AI_Procedure.RunDialog,
				AI_Procedure.ScrollDialog,
				AI_Procedure.SelectDialog,
				AI_Procedure.PositionDialog,
				AI_Procedure.DLG_ChangeAction,
				AI_Procedure.DLG_ChangeComport,
				AI_Procedure.DLG_ActivateChannel,
				AI_Procedure.SND_SetSoundEventActivation,
				AI_Procedure.ActivateCreditsMotorMode,
				AI_Procedure.ActivateInventoryMotorMode,
				AI_Procedure.ActivateBinocularMotorMode,
				AI_Procedure.ActivateSaveGameMotorMode,
				AI_Procedure.ActivateLaserPointer,
				AI_Procedure.HideLaserPointer,
				AI_Procedure.SetLaserPointerDistance,
				AI_Procedure.SetLaserPointerTexture,
				AI_Procedure.SetLaserPointerDetails,
				AI_Procedure.LaserPointerSniper,
				AI_Procedure.LaserPointerFly,
				AI_Procedure.FadeRandom,
				AI_Procedure.RopeInitialisation,
				AI_Procedure.TurnInterpolationReset,
				AI_Procedure.TurnInterpolationOff,
				AI_Procedure.TurnInterpolationOn,
				AI_Procedure.SetDraw,
				AI_Procedure.SetImage,
				AI_Procedure.SetToNextImage,
				AI_Procedure.FadeImage,
				AI_Procedure.MovingUV,
				AI_Procedure.Rotation,
				AI_Procedure.SetTextString,
				AI_Procedure.SetTextNumber,
				AI_Procedure.SlideGamePlayInterface,
				AI_Procedure.HilitePerso,
				AI_Procedure.ResetPersoToInitialState,
				AI_Procedure.SetFlashingSpeed,
				AI_Procedure.SetFlashingTransparent,
				AI_Procedure.SetFlashingNotTransparent,
				AI_Procedure.SetFlashingWithDefaultGMT,
				AI_Procedure.SetFlashingNotWithDefaultGMT,
				AI_Procedure.SetInitialGameFloors,
				AI_Procedure.SetStandardFloor,
				AI_Procedure.ActivateFloorGame,
				AI_Procedure.SetColorPathGoal,
				AI_Procedure.ChangeColor,
				AI_Procedure.LGT_ChangeIntensity,
				AI_Procedure.LGT_Switch,
				AI_Procedure.LGT_SwitchAll,
				AI_Procedure.SetStableEnvironnement,
				AI_Procedure.ActivateMapMotorMode,
				AI_Procedure.SetEpoque,
				AI_Procedure.SetName,
				AI_Procedure.RopeCareTaker,
				AI_Procedure.ChangeChannelMassForPolySys,
				AI_Procedure.FixNormReelSpeed,
				AI_Procedure.LoadSoundBank,
				AI_Procedure.UnLoadSoundBank,
				AI_Procedure.KillMusic,
				AI_Procedure.KillSoundEffect,
				AI_Procedure.KillVoice,
				AI_Procedure.KillAllSounds,
				AI_Procedure.SND_ChangeMyVolumeEffects,

				AI_Procedure.SND_ChangeVolumeEffects,
				AI_Procedure.SND_ChangeMyVolumeVoice,
				AI_Procedure.SND_ChangeVolumeVoice,
				AI_Procedure.SND_ChangeMyVolumeMusic,
				AI_Procedure.SND_ChangeVolumeMusic,
				AI_Procedure.SND_ChangeAllVolume,
				AI_Procedure.SND_ResetMyVolumeEffects,
				AI_Procedure.SND_ResetMyVolumeVoice,
				AI_Procedure.SND_ResetMyVolumeMusic,
				AI_Procedure.SND_ResetVolumeEffects,
				AI_Procedure.SND_ResetVolumeVoice,
				AI_Procedure.SND_ResetVolumeMusic,
				AI_Procedure.SND_ResetAllVolume,
				AI_Procedure.SND_SetEffectSound,
				AI_Procedure.SND_SwitchMicroPerso,
				AI_Procedure.SND_SwitchOldMicroPerso,

				AI_Procedure.SetVignetteIndex,
				AI_Procedure.SND_SaveMusic,
				AI_Procedure.SND_LoadMusic,
				AI_Procedure.SND_FadeMusic,
				AI_Procedure.SND_FadeVoice,
				AI_Procedure.SND_FadeSound,
				AI_Procedure.SND_FadeAll,
				AI_Procedure.SND_FadeObjectAndStop,
				AI_Procedure.SND_FadeInObject,
				AI_Procedure.SND_KillSoundObject,
				AI_Procedure.SetEconomicPersoAbsolutePosition,
				AI_Procedure.ScaleModule,
				AI_Procedure.Inv_DeselectionnerObjetSelectionne,
				AI_Procedure.Inv_ChangerCapaciteMaximumObjet,
				AI_Procedure.Inv_ChangerEtatJoyau,
				AI_Procedure.Inv_ChangerLeNombreDuStockLst,
				AI_Procedure.Inv_AfficherUnObjet,
				AI_Procedure.Stats_DisplayMainActorLife,
				AI_Procedure.Stats_DisplayMainActorArmor,
				AI_Procedure.Stats_DisplayMainActorMagic,
				AI_Procedure.Stats_DisplayMainActorSelectedObject,
				AI_Procedure.Stats_DisplayMainActorWeapon,
				AI_Procedure.Stats_DisplayDragon,
				AI_Procedure.Stats_DisplayCash,
				AI_Procedure.Fight_ChangeArmor,
				AI_Procedure.Fight_ChangeArmorResistance,
				AI_Procedure.Fight_RepairArmor,
				AI_Procedure.Fight_SubHitPointsEnemy,
				AI_Procedure.Magic_ChangeInitialMagicPoints,
				AI_Procedure.Magic_ChangeMaxMagicPoints,
				AI_Procedure.Magic_InitMagicPointsToInit,
				AI_Procedure.Magic_InitMagicPointsToMax,
				AI_Procedure.Magic_SelectPrevSpellType,
				AI_Procedure.Magic_SelectNextSpellType,
				AI_Procedure.Dragon_ChangeInitialNbrDiamonds,
				AI_Procedure.Dragon_ChangeMaxNbrDiamonds,
				AI_Procedure.Dragon_InitDiamondsToInit,
				AI_Procedure.Dragon_InitDiamondsToMax,
				AI_Procedure.Dragon_SetDragonActor,
				AI_Procedure.Demo_ValidateMap,
				AI_Procedure.ChangeViseePerso,
			};
		}
		#endregion
	}
}
