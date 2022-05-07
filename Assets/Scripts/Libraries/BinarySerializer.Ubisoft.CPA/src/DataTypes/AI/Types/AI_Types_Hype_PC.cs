using System;
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
	}
}
