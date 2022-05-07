namespace BinarySerializer.Ubisoft.CPA {
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*eFieldType_([^ \t,]*)[ \t]*,[ \t]*fn_.*
	// Replace: \[Definition\(SCR: \6, EN: \5, FR: \3, Type: "\7"\)] \2,
	public enum AI_Field {
		#region DefFild
		//********************************************************************************************************************************************************************************************************************************************************
		// Affect the Perso 
		[AI_Definition(SCR: "Position", EN: "Position", FR: "Position", Type: "Vector")] Position,

		[AI_Definition(SCR: "Orientation", EN: "Orientation", FR: "Orientation", Type: "Vector")] Orientation,
		[AI_Definition(SCR: "Speed", EN: "Speed", FR: "Speed", Type: "Vector")] Speed,
		[AI_Definition(SCR: "NormSpeed", EN: "NormSpeed", FR: "NormSpeed", Type: "Float")] NormSpeed,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "SightAxis", FR: "AxeDeVue", Type: "Vector")] SightAxis,
		[AI_Definition(SCR: "FirstCompAxis", FR: "AxeComplementaire1", Type: "Vector")] FirstCompAxis,
		[AI_Definition(SCR: "SecondCompAxis", FR: "AxeComplementaire2", Type: "Vector")] SecondCompAxis,

		[AI_Definition(SCR: "AbsoluteAxisX", EN: "AbsoluteAxisX", FR: "AxeXEnAbsolu", Type: "Vector")] AbsoluteAxisX,
		[AI_Definition(SCR: "AbsoluteAxisY", EN: "AbsoluteAxisY", FR: "AxeYEnAbsolu", Type: "Vector")] AbsoluteAxisY,
		[AI_Definition(SCR: "AbsoluteAxisZ", EN: "AbsoluteAxisZ", FR: "AxeZEnAbsolu", Type: "Vector")] AbsoluteAxisZ,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "PrevComportIntell", EN: "PrevComportIntell", FR: "ComportIntelPrec", Type: "Comport")] PrevComportIntell,
		[AI_Definition(SCR: "PrevComportReflex", EN: "PrevComportReflex", FR: "ComportReflexePrec", Type: "Comport")] PrevComportReflex,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "CollisionFlag", FR: "CollisionFlag", Type: "0To255")] CollisionFlag,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "ShadowScaleX", EN: "ShadowScaleX", FR: "ShadowScaleX", Type: "Float")] ShadowScaleX,
		[AI_Definition(SCR: "ShadowScaleY", EN: "ShadowScaleY", FR: "ShadowScaleY", Type: "Float")] ShadowScaleY,
		#endregion

		#region R3 PS2
		PadGlobalVector,
		PadHorizontalAxis,
		PadVerticalAxis,
		PadAnalogForce,
		PadTrueAnalogForce,
		PadRotationAngle,
		PadSector,
		SystemData,
		SystemTime,
		#endregion

		#region R2 iOS
		CameraOffsetX,
		MenuIndex,
		MenuItem_Index,
		HoldItem_Index,
		SoundEffectVolume,
		MusicVolume,
		bGotoIGM,
		TempFunction,
		bMotionSensor,
		bCameraLook,
		bHoldCamera,
		CheatEnable,
		#endregion
	}
}