namespace BinarySerializer.Ubisoft.CPA {
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	// Replace: \[Definition\(SCR: \6, EN: \5, FR: \3\)] \2,
	public enum AI_MetaAction {
		#region DefAct
		[AI_Definition(SCR: "Action_FrozenWait", EN: "TIME_FrozenWait", FR: "Temps_AttenteFigee")] FrozenWait,
		[AI_Definition(SCR: "Action_ExecuteAction", EN: "ACTION_ExecuteAction", FR: "ExecuteAction")] ExecuteAction,
		[AI_Definition(SCR: "Action_WaitEndOfAction", EN: "ACTION_WaitEndOfAction", FR: "AttendFinAction")] WaitEndOfAction,
		[AI_Definition(SCR: "Action_WaitEndOfAnim", EN: "ACTION_WaitEndOfAnim", FR: "AttendFinAnim")] WaitEndOfAnim,

		[AI_Definition(SCR: "Action_SpeakAndWaitEnd", EN: "ACTION_SpeakAndWaitEnd", FR: "ParleEtAttendLaFin")] SpeakAndWaitEnd,
		#endregion

		#region DefActCa
		[AI_Definition(SCR: "Action_CAM_CineMoveAToBTgtC", EN: "CAM_CineMoveAToBTgtC", FR: "CAM_CineMoveAToBTgtC")] CamCineMoveAToBTgtC,
		[AI_Definition(SCR: "Action_CAM_CineMoveAToBTgtAC", EN: "CAM_CineMoveAToBTgtAC", FR: "CAM_CineMoveAToBTgtAC")] CamCineMoveAToBTgtAC,
		[AI_Definition(SCR: "Action_CAM_CinePosATgtB", EN: "CAM_CinePosATgtB", FR: "CAM_CinePosATgtB")] CamCinePosATgtB,
		[AI_Definition(SCR: "Action_CAM_CinePosAMoveTgtBToC", EN: "CAM_CinePosAMoveTgtBToC", FR: "CAM_CinePosAMoveTgtBToC")] CamCinePosAMoveTgtBToC,

		[AI_Definition(SCR: "Action_CAM_CinePosATgtBTurnPosH", EN: "CAM_CinePosATgtBTurnPosH", FR: "CAM_CinePosATgtBTurnPosH")] CamCinePosATgtBTurnPosH,
		[AI_Definition(SCR: "Action_CAM_CinePosATgtBTurnTgtH", EN: "CAM_CinePosATgtBTurnTgtH", FR: "CAM_CinePosATgtBTurnTgtH")] CamCinePosATgtBTurnTgtH,
		[AI_Definition(SCR: "Action_CAM_CinePosATgtBTurnPosV", EN: "CAM_CinePosATgtBTurnPosV", FR: "CAM_CinePosATgtBTurnPosV")] CamCinePosATgtBTurnPosV,
		[AI_Definition(SCR: "Action_CAM_CinePosATgtBTurnTgtV", EN: "CAM_CinePosATgtBTurnTgtV", FR: "CAM_CinePosATgtBTurnTgtV")] CamCinePosATgtBTurnTgtV,
		#endregion
	}
}