﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Types_TTSE : AI_Types {
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
				AI_DsgVarType.PersoArray,
				AI_DsgVarType.VectorArray,
				AI_DsgVarType.FloatArray,
				AI_DsgVarType.IntegerArray,
				AI_DsgVarType.WayPointArray,
				AI_DsgVarType.TextArray,
			};
		}
		#endregion

		#region Node types
		protected override void InitKeywords() {
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

		#region Keywords
		protected override void InitInterpretTypes() {
			Keywords = new AI_Keyword[] {
				AI_Keyword.If,
				AI_Keyword.Then,
				AI_Keyword.Else,
				AI_Keyword.EngineGoto,
				AI_Keyword.Me,
				AI_Keyword.MainActor,
				AI_Keyword.World,
				AI_Keyword.Nobody,
				AI_Keyword.Nowhere
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
				AI_MetaAction.IncrementalTurn2,
				AI_MetaAction.IncrementalTurn,
				AI_MetaAction.Pursuit,
				AI_MetaAction.DeltaPursuit,
				AI_MetaAction.ExecuteAction,
				AI_MetaAction.WaitEndOfAction,
				AI_MetaAction.WaitEndOfAnim,
				AI_MetaAction.SpeakAndWaitEnd,
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
				AI_MetaAction.GoOverWay,
				AI_MetaAction.GoOverWay3D,
				AI_MetaAction.GoOverWayBack,
				AI_MetaAction.GoToNextWPOfWay,
				AI_MetaAction.GoToNextWPOfWayBack,
				AI_MetaAction.GoToPosition,
				AI_MetaAction.MoveLift,
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
				AI_MetaAction.SwimToSurface
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
				AI_Condition.ChangeActionEnable,
				AI_Condition.IsInAction,
				AI_Condition.IsTypeOfGMTCollide,
				AI_Condition.IsThereMechEvent,
				AI_Condition.IsValidObject,
				AI_Condition.IsValidWayPoint,
				AI_Condition.IsValidGMT,
				AI_Condition.IsValidAction,
				AI_Condition.InTopOfJump,
				AI_Condition.CanSwim,
				AI_Condition.CanSwimOnSurface,
				AI_Condition.CanSwimUnderWater,
				AI_Condition.IsNotOutOfDepth,
				AI_Condition.IsCompletelyOutOfWater,
				AI_Condition.IsSpeechOver,
				AI_Condition.SeePerso,
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
				AI_Condition.CanIAddObjectInInventory,
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
				AI_Field.SightAxis,
				AI_Field.FirstCompAxis,
				AI_Field.SecondCompAxis,
				AI_Field.AbsoluteAxisX,
				AI_Field.AbsoluteAxisY,
				AI_Field.AbsoluteAxisZ,
				AI_Field.PrevComportIntell,
				AI_Field.PrevComportReflex,
				AI_Field.CollisionFlag,
				AI_Field.ShadowScaleX,
				AI_Field.ShadowScaleY
			};
		}
		#endregion

		#region Functions
		protected override void InitFunctions() {
			Functions = new AI_Function[] {
				AI_Function.GetPersoAbsolutePosition,
				AI_Function.GetAngleAroundZToPerso,
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
				AI_Function.GetCurrentObjectInInventory,
				AI_Function.GetObjectNumberInInventory,
				AI_Function.UseObjectFromInventory,
				AI_Function.UseNObjectsFromInventory,
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
			};
		}
		#endregion

		#region Procedures
		protected override void InitProcedures() {
			Procedures = new AI_Procedure[] {
				AI_Procedure.PlayerIsDead,
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
				AI_Procedure.SetThetaAngle,
				AI_Procedure.SetImpulse,
				AI_Procedure.SetNormSpeed,
				AI_Procedure.AddNormSpeed,
				AI_Procedure.MulNormSpeed,
				AI_Procedure.SetDirectionSpeed,
				AI_Procedure.AddDirectionSpeed,
				AI_Procedure.SetVectorSpeed,
				AI_Procedure.SetDynamScalar,
				AI_Procedure.SetTarget,
				AI_Procedure.TurnAround,
				AI_Procedure.GoRelative,
				AI_Procedure.GoInDirection,
				AI_Procedure.TurnLeft,
				AI_Procedure.TurnRight,
				AI_Procedure.TurnUp,
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
				AI_Procedure.CopyObjectFromTableToTable,
				AI_Procedure.StartSpeech,
				AI_Procedure.StopSpeech,
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
				AI_Procedure.AddSurfaceHeight,
				AI_Procedure.MoveSurfaceHeight,
				AI_Procedure.AddObjectInInventory,
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
				AI_Procedure.PRT_SetGeneratorOn,
				AI_Procedure.PRT_SetGeneratorOff,
				AI_Procedure.PRT_SetGenerator,
				AI_Procedure.PRT_SetModeNone,
				AI_Procedure.PRT_SetModeContinuous,
				AI_Procedure.PRT_SetModeCrenel,
				AI_Procedure.PRT_SetModeProbability,
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
			};
		}
		#endregion

	}

}
