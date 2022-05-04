namespace BinarySerializer.Ubisoft.CPA {
	// Regex for NP++:
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	//                                       eCond                 , FR                   ,  EN                  , SCR
	// Replace: \[AI_Definition\(SCR: \6, EN: \5, FR: \3\)] \2,
	// Then replace " EN: ," with ""
	public enum AI_Function {
		#region DefFunc
		//********************************************************************************************************************************************************************************************************************************************************
		// Acthor 
		[AI_Definition(SCR: "Func_GetPersoAbsolutePosition", EN: "ACT_GetPersoAbsolutePosition", FR: "ACT_PositionAbsoluePerso")] GetPersoAbsolutePosition,
		[AI_Definition(SCR: "Func_GetMyAbsolutePosition", EN: "ACT_GetMyAbsolutePosition", FR: "ACT_MaPositionAbsolue")] GetMyAbsolutePosition,
		[AI_Definition(SCR: "Func_GetAngleAroundZToPerso", EN: "ACT_GetAngleAroundZToPerso", FR: "ACT_AngleAutourZVersPerso")] GetAngleAroundZToPerso,
		[AI_Definition(SCR: "Func_DistanceToPerso", EN: "ACT_DistanceToPerso", FR: "ACT_DistanceAuPerso")] DistanceToPerso,
		[AI_Definition(SCR: "Func_DistanceXToPerso", EN: "ACT_DistanceXToPerso", FR: "ACT_DistanceXAuPerso")] DistanceXToPerso,
		[AI_Definition(SCR: "Func_DistanceYToPerso", EN: "ACT_DistanceYToPerso", FR: "ACT_DistanceYAuPerso")] DistanceYToPerso,
		[AI_Definition(SCR: "Func_DistanceZToPerso", EN: "ACT_DistanceZToPerso", FR: "ACT_DistanceZAuPerso")] DistanceZToPerso,
		[AI_Definition(SCR: "Func_DistanceXYToPerso", EN: "ACT_DistanceXYToPerso", FR: "ACT_DistanceXYAuPerso")] DistanceXYToPerso,
		[AI_Definition(SCR: "Func_DistanceXZToPerso", EN: "ACT_DistanceXZToPerso", FR: "ACT_DistanceXZAuPerso")] DistanceXZToPerso,
		[AI_Definition(SCR: "Func_DistanceYZToPerso", EN: "ACT_DistanceYZToPerso", FR: "ACT_DistanceYZAuPerso")] DistanceYZToPerso,
		[AI_Definition(SCR: "Func_DistanceToPersoCenter", EN: "ACT_DistanceToPersoCenter", FR: "ACT_DistanceAuCentrePerso")] DistanceToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceXToPersoCenter", EN: "ACT_DistanceXToPersoCenter", FR: "ACT_DistanceXAuCentrePerso")] DistanceXToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceYToPersoCenter", EN: "ACT_DistanceYToPersoCenter", FR: "ACT_DistanceYAuCentrePerso")] DistanceYToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceZToPersoCenter", EN: "ACT_DistanceZToPersoCenter", FR: "ACT_DistanceZAuCentrePerso")] DistanceZToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceXYToPersoCenter", EN: "ACT_DistanceXYToPersoCenter", FR: "ACT_DistanceXYAuCentrePerso")] DistanceXYToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceXZToPersoCenter", EN: "ACT_DistanceXZToPersoCenter", FR: "ACT_DistanceXZAuCentrePerso")] DistanceXZToPersoCenter,
		[AI_Definition(SCR: "Func_DistanceYZToPersoCenter", EN: "ACT_DistanceYZToPersoCenter", FR: "ACT_DistanceYZAuCentrePerso")] DistanceYZToPersoCenter,
		//********************************************************************************************************************************************************************************************************************************************************
		// Reseau 
		[AI_Definition(SCR: "Func_DistanceToWP", EN: "NETWORK_DistanceToWP", FR: "Reseau_DistanceAuWP")] DistanceToWP,
		[AI_Definition(SCR: "Func_GetWPAbsolutePosition", EN: "NETWORK_GetWPAbsolutePosition", FR: "Reseau_PositionAbsolueWP")] GetWPAbsolutePosition,
		//********************************************************************************************************************************************************************************************************************************************************
		// Math Functions 
		[AI_Definition(SCR: "Func_Int", EN: "MATH_Int", FR: "Math_ConversionEnEntier")] Int,
		[AI_Definition(SCR: "Func_RandomInt", EN: "MATH_RandomInt", FR: "Math_EntierAuHasard")] RandomInt,
		[AI_Definition(SCR: "Func_Real", EN: "MATH_Real", FR: "Math_ConversionEnReel")] Real,
		[AI_Definition(SCR: "Func_Sinus", EN: "MATH_Sinus", FR: "Math_Sinus")] Sinus,
		[AI_Definition(SCR: "Func_Cosinus", EN: "MATH_Cosinus", FR: "Math_Cosinus")] Cosinus,
		[AI_Definition(SCR: "Func_Square", EN: "MATH_Square", FR: "Math_Carre")] Square,
		[AI_Definition(SCR: "Func_SquareRoot", EN: "MATH_SquareRoot", FR: "Math_Racine")] SquareRoot,
		[AI_Definition(SCR: "Func_RandomReal", EN: "MATH_RandomReal", FR: "Math_ReelAuHasard")] RandomReal,
		[AI_Definition(SCR: "Func_MinimumReal", EN: "MATH_MinimumReal", FR: "Math_MinimumReel")] MinimumReal,
		[AI_Definition(SCR: "Func_MaximumReal", EN: "MATH_MaximumReal", FR: "Math_MaximumReel")] MaximumReal,
		[AI_Definition(SCR: "Func_DegreeToRadian", EN: "MATH_DegreeToRadian", FR: "Math_ConversionDegreEnRadian")] DegreeToRadian,
		[AI_Definition(SCR: "Func_RadianToDegree", EN: "MATH_RadianToDegree", FR: "Math_ConversionRadianEnDegre")] RadianToDegree,
		[AI_Definition(SCR: "Func_AbsoluteValue", EN: "MATH_AbsoluteValue", FR: "Math_ValeurAbsolue")] AbsoluteValue,
		[AI_Definition(SCR: "Func_LimitRealInRange", EN: "MATH_LimitRealInRange", FR: "Math_ReelBorne")] LimitRealInRange,
		[AI_Definition(SCR: "Func_Sign", EN: "MATH_Sign", FR: "Math_Signe")] Sign,
		[AI_Definition(SCR: "Func_Cube", EN: "MATH_Cube", FR: "Math_Cube")] Cube,
		[AI_Definition(SCR: "Func_Modulo", EN: "MATH_Modulo", FR: "Math_Modulo")] Modulo,
		[AI_Definition(SCR: "Func_TernInf", EN: "MATH_TernInf", FR: "Math_TernInf")] TernInf,
		[AI_Definition(SCR: "Func_TernSup", EN: "MATH_TernSup", FR: "Math_TernSup")] TernSup,
		[AI_Definition(SCR: "Func_TernEq", EN: "MATH_TernEq", FR: "Math_TernEq")] TernEq,
		[AI_Definition(SCR: "Func_TernInfEq", EN: "MATH_TernInfEq", FR: "Math_TernInfEq")] TernInfEq,
		[AI_Definition(SCR: "Func_TernSupEq", EN: "MATH_TernSupEq", FR: "Math_TernSupEq")] TernSupEq,
		[AI_Definition(SCR: "Func_TernOp", EN: "MATH_TernOp", FR: "Math_TernOp")] TernOp,
		[AI_Definition(SCR: "Func_TemporalRealCombination", EN: "MATH_TemporalRealCombination", FR: "Math_CombinaisonTemporelleReel")] TemporalRealCombination,

		//********************************************************************************************************************************************************************************************************************************************************
		// HitPoints & HitPointsMax Functions 
		[AI_Definition(SCR: "Func_GetHitPoints", EN: "ACT_GetHitPoints", FR: "ACT_LitPointsDeVie")] GetHitPoints,
		[AI_Definition(SCR: "Func_AddAndGetHitPoints", EN: "ACT_AddAndGetHitPoints", FR: "ACT_AjouteEtLitPointsDeVie")] AddAndGetHitPoints,
		[AI_Definition(SCR: "Func_SubAndGetHitPoints", EN: "ACT_SubAndGetHitPoints", FR: "ACT_EnleveEtLitPointsDeVie")] SubAndGetHitPoints,
		[AI_Definition(SCR: "Func_GetHitPointsMax", EN: "ACT_GetHitPointsMax", FR: "ACT_LitPointsDeVieMax")] GetHitPointsMax,
		[AI_Definition(SCR: "Func_AddAndGetHitPointsMax", EN: "ACT_AddAndGetHitPointsMax", FR: "ACT_AjouteEtLitPointsDeVieMax")] AddAndGetHitPointsMax,
		[AI_Definition(SCR: "Func_SubAndGetHitPointsMax", EN: "ACT_SubAndGetHitPointsMax", FR: "ACT_EnleveEtLitPointsDeVieMax")] SubAndGetHitPointsMax,
		//********************************************************************************************************************************************************************************************************************************************************
		// List Functions 
		[AI_Definition(SCR: "Func_ListSize", EN: "LST_ListSize", FR: "LST_TailleDeListe")] ListSize,
		[AI_Definition(SCR: "Func_GivePersoInList", EN: "LST_GivePersoInList", FR: "LST_PrendPersoDansListe")] GivePersoInList,
		//********************************************************************************************************************************************************************************************************************************************************
		// Vector 
		[AI_Definition(SCR: "Func_AbsoluteVector", EN: "VEC_AbsoluteVector", FR: "VEC_VecteurAbsolu")] AbsoluteVector,
		[AI_Definition(SCR: "Func_RelativeVector", EN: "VEC_RelativeVector", FR: "VEC_VecteurRelatif")] RelativeVector,
		[AI_Definition(SCR: "Func_VectorLocalToGlobal", EN: "VEC_VectorLocalToGlobal", FR: "VEC_VecteurLocalToGlobal")] VecteurLocalToGlobal,
		[AI_Definition(SCR: "Func_VectorGlobalToLocal", EN: "VEC_VectorGlobalToLocal", FR: "VEC_VecteurGlobalToLocal")] VecteurGlobalToLocal,
		//********************************************************************************************************************************************************************************************************************************************************
		// Magnet MGT functions 
		[AI_Definition(SCR: "Func_MAGNETGetStrength", EN: "MAGNET_GetStrength", FR: "Magnet_GetStrength")] GetMagnetStrength,
		[AI_Definition(SCR: "Func_MAGNETGetFar", EN: "MAGNET_GetFar", FR: "Magnet_GetFar")] GetMagnetFar,
		[AI_Definition(SCR: "Func_MAGNETGetNear", EN: "MAGNET_GetNear", FR: "Magnet_GetNear")] GetMagnetNear,
		[AI_Definition(SCR: "Func_MAGNETGetDuration", EN: "MAGNET_GetDuration", FR: "Magnet_GetDuration")] GetMagnetDuration,
		//********************************************************************************************************************************************************************************************************************************************************
		// to change SPO draw mask 
		[AI_Definition(SCR: "Func_SPO_GetDrawFlag", EN: "SPO_GetDrawFlag", FR: "SPO_GetDrawFlag")] SPO_GetDrawFlag,
		//********************************************************************************************************************************************************************************************************************************************************
		// Time Functions 
		[AI_Definition(SCR: "Func_GetTime", EN: "TIME_GetTime", FR: "Temps_Obtenir")] GetTime,
		[AI_Definition(SCR: "Func_ElapsedTime", EN: "TIME_ElapsedTime", FR: "Temps_Ecoule")] GetElapsedTime,
		[AI_Definition(SCR: "Func_GetDT", EN: "TIME_GetDT", FR: "Temps_LitDT")] GetDeltaT,
		[AI_Definition(SCR: "Func_GetFrameLength", EN: "TIME_GetFrameLength", FR: "Temps_DureeTrame")] GetFrameLength,

		//********************************************************************************************************************************************************************************************************************************************************
		// Input analogic value Functions 
		[AI_Definition(SCR: "Func_GetInputAnalogicValue", EN: "PAD_GetInputAnalogicValue", FR: "PAD_LitValeurAnalogiqueEntree")] InputAnalogicValue,
		[AI_Definition(SCR: "Func_VitessePadAnalogique", EN: "PAD_VitessePadAnalogique", FR: "PAD_VitessePadAnalogique")] VitessePadAnalogique,
		//********************************************************************************************************************************************************************************************************************************************************
		// Misc 
		[AI_Definition(SCR: "Func_GenerateObject", EN: "ACT_GenerateObject", FR: "GenereObjet")] GenerateObject,
		[AI_Definition(SCR: "Func_CountGeneratedObjects", EN: "ACT_CountGeneratedObjects", FR: "ACT_CompteObjetsGeneres")] CountGeneratedObjects,
		[AI_Definition(SCR: "Func_GetGlobalCounter", EN: "MAP_GetGlobalCounter", FR: "MAP_LitCompteurGlobal")] GetGlobalCounter,
		[AI_Definition(SCR: "Func_GetSubMapId", EN: "MAP_GetSubMapId", FR: "MAP_LitNumeroDeSousMap")] GetSubMapId,
		//********************************************************************************************************************************************************************************************************************************************************
		// Color functions 
		[AI_Definition(SCR: "Func_AddColor", EN: "COLOR_AddColor", FR: "AjouteCouleur")] AddColor,
		[AI_Definition(SCR: "Func_AddRed", EN: "COLOR_AddRed", FR: "AjouteRouge")] AddRed,
		[AI_Definition(SCR: "Func_AddGreen", EN: "COLOR_AddGreen", FR: "AjouteVert")] AddGreen,
		[AI_Definition(SCR: "Func_AddBlue", EN: "COLOR_AddBlue", FR: "AjouteBleu")] AddBlue,
		[AI_Definition(SCR: "Func_AddAlpha", EN: "COLOR_AddAlpha", FR: "AjouteAlpha")] AddAlpha,
		[AI_Definition(SCR: "Func_ColorRGBA", EN: "COLOR_ColorRGBA", FR: "CouleurRVBA")] ColorRedGreenBlueAlpha,
		[AI_Definition(SCR: "Func_ColorRGB", EN: "COLOR_ColorRGB", FR: "CouleurRVB")] ColorRedGreenBlue,
		[AI_Definition(SCR: "Func_ColorRed", EN: "COLOR_ColorRed", FR: "ComposanteRouge")] ColorRed,
		[AI_Definition(SCR: "Func_ColorGreen", EN: "COLOR_ColorGreen", FR: "ComposanteVerte")] ColorGreen,
		[AI_Definition(SCR: "Func_ColorBlue", EN: "COLOR_ColorBlue", FR: "ComposanteBleue")] ColorBlue,
		[AI_Definition(SCR: "Func_ColorAlpha", EN: "COLOR_ColorAlpha", FR: "ComposanteAlpha")] ColorAlpha,
		//********************************************************************************************************************************************************************************************************************************************************
		// Visual GMT functions 
		[AI_Definition(SCR: "Func_GetVisualGMTColor", EN: "GMT_GetVisualGMTColor", FR: "LitVisuelGMTCouleur")] GetVisualGMTColor,
		[AI_Definition(SCR: "Func_GetVisualGMTSpecularCoef", EN: "GMT_GetVisualGMTSpecularCoef", FR: "LitVisuelGMTSpeculaireCoef")] GetVisualGMTSpecularCoef,
		[AI_Definition(SCR: "Func_GetVisualGMTSpecularExponant", EN: "GMT_GetVisualGMTSpecularExponant", FR: "LitVisuelGMTSpeculaireCoef")] GetVisualGMTSpecularExponent,
		[AI_Definition(SCR: "Func_GetVisualGMTDiffuseCoef", EN: "GMT_GetVisualGMTDiffuseCoef", FR: "LitVisuelGMTDiffusionCoef")] GetVisualGMTDiffuseCoef,
		[AI_Definition(SCR: "Func_GetVisualGMTAmbientCoef", EN: "GMT_GetVisualGMTAmbientCoef", FR: "LitVisuelGMTAmbientCoef")] GetVisualGMTAmbientCoef,
		[AI_Definition(SCR: "Func_GetVisualGMTTextureScrollingCoefU", EN: "GMT_GetVisualGMTTextureScrollCoefU", FR: "LitVisuelGMTTextureScrollingCoefU")] GetVisualGMTTextureScrollingCoefU,
		[AI_Definition(SCR: "Func_GetVisualGMTTextureScrollingCoefV", EN: "GMT_GetVisualGMTTextureScrollCoefV", FR: "LitVisuelGMTTextureScrollingCoefV")] GetVisualGMTTextureScrollingCoefV,
		[AI_Definition(SCR: "Func_GetVisualGMTFrame", EN: "GMT_GetVisualGMTFrame", FR: "LitNoImageVisuelGMT")] GetVisualGMTFrame,
		[AI_Definition(SCR: "Func_GetVisualGMTNumberOfFrames", EN: "GMT_GetVisualGMTNumberOfFrames", FR: "LitNbImagesVisuelGMT")] GetVisualGMTNumberOfFrames,
		//********************************************************************************************************************************************************************************************************************************************************
		// Function for savegames 
		[AI_Definition(SCR: "Func_SaveGame", EN: "GAME_SaveGame", FR: "SauvePartie")] SaveGame,
		[AI_Definition(SCR: "Func_LoadGame", EN: "GAME_LoadGame", FR: "ChargePartie")] LoadGame,
		[AI_Definition(SCR: "Func_EraseGame", EN: "GAME_EraseGame", FR: "EffacePartie")] EraseGame,
		[AI_Definition(SCR: "Func_GetCurrentSlotNumber", EN: "GAME_GetCurrentSlotNumber", FR: "LitNumeroSlotCourant")] GetCurrentSlot,
		[AI_Definition(SCR: "Func_IsAValidSlotName", EN: "OPTION_IsAValidSlotName", FR: "OPTION_IsAValidSlotName")] IsAValidSlotName,
		[AI_Definition(SCR: "Func_RepairCorruption", EN: "GAME_RepairCorruption", FR: "GAME_RepairCorruption")] RepairCorruptSlot,

		//********************************************************************************************************************************************************************************************************************************************************
		//KWN : Add a New AI Function
		[AI_Definition(SCR: "Func_GetScreenCoordinates", EN: "ACT_ScreenCoordinates", FR: "ACT_GetScreenCoordinates")] GetScreenCoordinates,
		[AI_Definition(SCR: "Func_GetScreenDSActif", EN: "ACT_GetScreenDSActif", FR: "ACT_GetScreenDSActif")] GetScreenDSActif,
		[AI_Definition(SCR: "Func_GetStyletCordinates", EN: "ACT_GetStyletCordinates", FR: "ACT_GetStyletCordinates")] GetStyletCordinates,
		//********************************************************************************************************************************************************************************************************************************************************

		//MICRO
		[AI_Definition(SCR: "Func_MICRO_GetSoufleValue", EN: "MICRO_GetSoufleValue", FR: "MICRO_GetSoufleValue")] MICRO_GetSoufleValue,

		#endregion

		#region DefFunc1
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_VitesseHorizontaleDuPerso", EN: "ACT_HorizontalPersoSpeed", FR: "ACT_VitesseHorizontaleDuPerso")] VitesseHorizontaleDuPerso,
		[AI_Definition(SCR: "Func_VitesseVerticaleDuPerso", EN: "ACT_VerticalPersoSpeed", FR: "ACT_VitesseVerticaleDuPerso")] VitesseVerticaleDuPerso,
		[AI_Definition(SCR: "Func_GetPersoZoomFactor", EN: "ACT_GetPersoZoomFactor", FR: "ACT_DeformationPerso")] GetPersoZoomFactor,
		[AI_Definition(SCR: "Func_GetPersoSighting", EN: "ACT_GetPersoSighting", FR: "ACT_ViseePerso")] GetPersoSighting,
		[AI_Definition(SCR: "Func_GetPersoHorizon", EN: "ACT_GetPersoHorizon", FR: "ACT_HorizonPerso")] GetPersoHorizon,
		[AI_Definition(SCR: "Func_GetPersoBanking", EN: "ACT_GetPersoBanking", FR: "ACT_AssiettePerso")] GetPersoBanking,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_LitPositionZDM", EN: "ZON_GetZDMPosition", FR: "ZON_LitPositionZDM")] LitPositionZDM,
		[AI_Definition(SCR: "Func_LitPositionZDE", EN: "ZON_GetZDEPosition", FR: "ZON_LitPositionZDE")] LitPositionZDE,
		[AI_Definition(SCR: "Func_LitPositionZDD", EN: "ZON_GetZDDPosition", FR: "ZON_LitPositionZDD")] LitPositionZDD,
		[AI_Definition(SCR: "Func_LitCentreZDM", EN: "ZON_GetZDMCenter", FR: "ZON_LitCentreZDM")] LitCentreZDM,
		[AI_Definition(SCR: "Func_LitCentreZDE", EN: "ZON_GetZDECenter", FR: "ZON_LitCentreZDE")] LitCentreZDE,
		[AI_Definition(SCR: "Func_LitCentreZDD", EN: "ZON_GetZDDCenter", FR: "ZON_LitCentreZDD")] LitCentreZDD,
		[AI_Definition(SCR: "Func_LitAxeZDM", EN: "ZON_GetZDMAxis", FR: "ZON_LitAxeZDM")] LitAxeZDM,
		[AI_Definition(SCR: "Func_LitAxeZDE", EN: "ZON_GetZDEAxis", FR: "ZON_LitAxeZDE")] LitAxeZDE,
		[AI_Definition(SCR: "Func_LitAxeZDD", EN: "ZON_GetZDDAxis", FR: "ZON_LitAxeZDD")] LitAxeZDD,
		[AI_Definition(SCR: "Func_LitDimensionZDM", EN: "ZON_GetZDMDimension", FR: "ZON_LitDimensionZDM")] LitDimensionZDM,
		[AI_Definition(SCR: "Func_LitDimensionZDE", EN: "ZON_GetZDEDimension", FR: "ZON_LitDimensionZDE")] LitDimensionZDE,
		[AI_Definition(SCR: "Func_LitDimensionZDD", EN: "ZON_GetZDDDimension", FR: "ZON_LitDimensionZDD")] LitDimensionZDD,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_VecteurPointAxe", EN: "VEC_PointAxisVector", FR: "VEC_VecteurPointAxe")] VecteurPointAxe,
		[AI_Definition(SCR: "Func_VecteurPointSegment", EN: "VEC_PointSegmentVector", FR: "VEC_VecteurPointSegment")] VecteurPointSegment,
		[AI_Definition(SCR: "Func_VectorContribution", EN: "VEC_VectorContribution", FR: "VEC_ContributionVecteur")] VectorContribution,
		[AI_Definition(SCR: "Func_VectorCombination", EN: "VEC_VectorCombination", FR: "VEC_CombinaisonVecteur")] VectorCombination,
		[AI_Definition(SCR: "Func_TemporalVectorCombination", EN: "VEC_TemporalVectorCombination", FR: "VEC_CombinaisonTemporelleVecteur")] TemporalVectorCombination,
		[AI_Definition(SCR: "Func_ScaledVector", EN: "VEC_ScaledVector", FR: "VEC_MulVecteurScalaire")] ScaledVector,
		[AI_Definition(SCR: "Func_GetVectorNorm", EN: "VEC_GetVectorNorm", FR: "VEC_VecteurNorme")] GetVectorNorm,
		[AI_Definition(SCR: "Func_RotateVector", EN: "VEC_RotateVector", FR: "VEC_TourneVecteur")] RotateVector,
		[AI_Definition(SCR: "Func_VecteurAngle", EN: "VEC_AngleVector", FR: "VEC_VecteurAngle")] VectorAngle,
		[AI_Definition(SCR: "Func_VecteurCos", EN: "VEC_CosVector", FR: "VEC_VecteurCos")] VectorCos,
		[AI_Definition(SCR: "Func_VecteurSin", EN: "VEC_SinVector", FR: "VEC_VecteurSin")] VectorSin,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetNormalCollideVector", EN: "COL_GetNormalCollideVector", FR: "COL_VecteurNormalCollision")] GetNormalCollideVector,
		[AI_Definition(SCR: "Func_GetNormalCollideVector2", EN: "COL_GetNormalCollideVector2", FR: "COL_VecteurNormalCollision2")] GetNormalCollideVector2,
		[AI_Definition(SCR: "Func_GetCollidePoint", EN: "COL_GetCollidePoint", FR: "COL_LitPointCollision")] GetCollidePoint,
		[AI_Definition(SCR: "Func_GetCollidePoint2", EN: "COL_GetCollidePoint2", FR: "COL_LitPointCollision2")] GetCollidePoint2,
		[AI_Definition(SCR: "Func_GetHandsCollidePoint", EN: "COL_GetHandsCollidePoint", FR: "COL_LitPointCollisionMains")] GetHandsCollidePoint,
		[AI_Definition(SCR: "eFunc_GetCollideRate", EN: "COL_GetCollideRate", FR: "COL_LitTauxDePenetrationCollision")] GetCollideRate,
		[AI_Definition(SCR: "Func_GetCollideRate2", EN: "COL_GetCollideRate2", FR: "COL_LitTauxDePenetrationCollision2")] GetCollideRate2,
		[AI_Definition(SCR: "Func_GetCollideMaterialType", EN: "COL_GetCollideMaterialType", FR: "COL_LitTypeMateriauCollision")] GetCollideMaterialType,
		[AI_Definition(SCR: "eFunc_GetCollideMaterialType2", EN: "COL_GetCollideMaterialType2", FR: "COL_LitTypeMateriauCollision2")] GetCollideMaterialType2,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_CollisionPoint", EN: "COL_CollisionPoint", FR: "COL_PointCollision")] GetCollisionPoint,
		[AI_Definition(SCR: "Func_CollisionNormalVector", EN: "COL_CollisionNormalVector", FR: "COL_NormaleCollision")] GetCollisionVector,
		[AI_Definition(SCR: "Func_PersoCollisionne", EN: "COL_PersoCollisionne", FR: "COL_PersoCollisionne")] GetCollisionPerso,
		[AI_Definition(SCR: "Func_CollisionPointMaterial", EN: "COL_CollisionPointMaterial", FR: "COL_PointCollisionAvecMateriau")] GetCollisionPointMaterial,
		// GMT functions 
		[AI_Definition(SCR: "Func_GetLastTraversedMaterialType", EN: "COL_GetLastTraversedMaterialType", FR: "COL_TypeDernierMateriauTraverse")] GetLastTraversedMaterialType,
		[AI_Definition(SCR: "Func_GetLastTraversedMaterial", EN: "COL_GetLastTraversedMaterial", FR: "COL_DernierMateriauTraverse")] GetLastTraversedMaterial,
		[AI_Definition(SCR: "Func_GetCurrentCollidedGMT", EN: "COL_GetCurrentCollidedGMT", FR: "LitGMTCourant")] GetCurrentCollidedGMT,
		// collision communication management : NOT ULTRA, because only the executed actor can read his information 
		[AI_Definition(SCR: "Func_GetColliderType", EN: "COL_GetColliderType", FR: "COL_LitTypeDeCollisionneur")] GetColliderType,
		[AI_Definition(SCR: "Func_GetColliderVector", EN: "COL_GetColliderVector", FR: "COL_LitVecteurDeCollisionneur")] GetColliderVector,
		[AI_Definition(SCR: "Func_GetColliderReal", EN: "COL_GetColliderReal", FR: "COL_LitReelDeCollisionneur")] GetColliderReal,
		[AI_Definition(SCR: "Func_LitDernierPersoCollisione", EN: "COL_LitDernierPersoCollisione", FR: "COL_LitDernierPersoCollisione")] GetLastCollisionActor,
		[AI_Definition(SCR: "Func_CalculVecteurRebond", EN: "COL_CalculVecteurRebond", FR: "COL_CalculVecteurRebond")] ComputeRebondVector,
		//********************************************************************************************************************************************************************************************************************************************************
		// position management : ULTRA 
		[AI_Definition(SCR: "Func_GetModuleAbsolutePosition", EN: "MOD_GetModuleAbsolutePosition", FR: "MOD_PositionAbsolueModule")] GetModuleAbsolutePosition,
		[AI_Definition(SCR: "Func_GetModuleRelativePosition", EN: "MOD_GetModuleRelativePosition", FR: "MOD_PositionRelativeModule")] GetModuleRelativePosition,
		[AI_Definition(SCR: "Func_GetModuleZoomFactor", EN: "MOD_GetModuleZoomFactor", FR: "MOD_DeformationModule")] GetModuleZoomFactor,
		[AI_Definition(SCR: "Func_GetModuleSighting", EN: "MOD_GetModuleSighting", FR: "MOD_ViseeModule")] GetModuleSighting,
		[AI_Definition(SCR: "Func_CastIntegerToChannel", EN: "MOD_CastIntegerToChannel", FR: "MOD_ConversionEnModule")] CastIntegerToChannel,
		//********************************************************************************************************************************************************************************************************************************************************
		// names and strings management 
		[AI_Definition(SCR: "Func_GetSlotDate", EN: "TEXT_GetSlotDate", FR: "TEXT_LitDateDuSlot")] GetSlotDate,
		[AI_Definition(SCR: "Func_GetSlotName", EN: "TEXT_GetSlotName", FR: "TEXT_LitNomDuSlot")] GetSlotName,
		[AI_Definition(SCR: "Func_GetSlotScore", EN: "TEXT_GetSlotScore", FR: "TEXT_LitScoreDuSlot")] GetSlotScore,
		[AI_Definition(SCR: "Func_GetStringCharAt", EN: "TEXT_GetStringCharAt", FR: "TEXT_LettreDuTexteALaPosition")] GetStringCharAt,
		[AI_Definition(SCR: "Func_GetFormattedTextInfo", EN: "TEXT_GetFormattedTextInfo", FR: "TEXT_LitInfoSurTexteFormate")] GetFormattedTextInfo,
		[AI_Definition(SCR: "Func_GetInputEntryName", EN: "TEXT_GetInputEntryName", FR: "TEXT_LitNomDeLaTouche")] GetInputEntryName,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_GetMechanicGravityFactor", EN: "MEC_GetGravityFactor", FR: "MEC_LitGravite")] GetMechanicGravityFactor,
		[AI_Definition(SCR: "Proc_GetMechanicSlide", EN: "MEC_GetSlide", FR: "MEC_LitGlissement")] GetMechanicSlide,
		[AI_Definition(SCR: "Proc_GetMechanicRebound", EN: "MEC_GetRebound", FR: "MEC_LitRebond")] GetMechanicRebound,
		[AI_Definition(SCR: "Proc_GetMechanicSlopeLimit", EN: "MEC_GetSlopeLimit", FR: "MEC_LitLimitePente")] GetMechanicSlopeLimit,
		[AI_Definition(SCR: "Proc_GetMechanicInertiaX", EN: "MEC_GetInertiaX", FR: "MEC_LitInertieX")] GetMechanicInertiaX,
		[AI_Definition(SCR: "Proc_GetMechanicInertiaY", EN: "MEC_GetInertiaY", FR: "MEC_LitInertieY")] GetMechanicInertiaY,
		[AI_Definition(SCR: "Proc_GetMechanicInertiaZ", EN: "MEC_GetInertiaZ", FR: "MEC_LitInertieZ")] GetMechanicInertiaZ,
		[AI_Definition(SCR: "Proc_GetMechanicTiltIntensity", EN: "MEC_GetTiltIntensity", FR: "MEC_LitIntensiteTilt")] GetMechanicTiltIntensity,
		[AI_Definition(SCR: "Proc_GetMechanicTiltInertia", EN: "MEC_GetTiltInertia", FR: "MEC_LitInertieTilt")] GetMechanicTiltInertia,
		[AI_Definition(SCR: "Proc_GetMechanicTiltOrigin", EN: "MEC_GetTiltOrigin", FR: "MEC_LitOrigineTilt")] GetMechanicTiltOrigin,
		[AI_Definition(SCR: "Proc_GetMechanicMaxSpeed", EN: "MEC_GetMaxSpeed", FR: "MEC_LitVitesseMax")] GetMechanicMaxSpeed,
		[AI_Definition(SCR: "Proc_GetMechanicStreamPriority", EN: "MEC_GetStreamPriority", FR: "MEC_LitPrioriteFlux")] GetMechanicStreamPriority,
		[AI_Definition(SCR: "Proc_GetMechanicStreamSpeed", EN: "MEC_GetStreamSpeed", FR: "MEC_LitVitesseFlux")] GetMechanicStreamSpeed,
		[AI_Definition(SCR: "Proc_GetMechanicStreamFactor", EN: "MEC_GetStreamFactor", FR: "MEC_LitFacteurDeFlux")] GetMechanicStreamFactor,
		// slide coef 
		[AI_Definition(SCR: "Proc_GetSlideFactorX", EN: "MEC_GetSlideFactorX", FR: "MEC_LitCoefDeGlisseX")] GetSlideFactorX,
		[AI_Definition(SCR: "Proc_GetSlideFactorY", EN: "MEC_GetSlideFactorY", FR: "MEC_LitCoefDeGlisseY")] GetSlideFactorY,
		[AI_Definition(SCR: "Proc_GetSlideFactorZ", EN: "MEC_GetSlideFactorZ", FR: "MEC_LitCoefDeGlisseZ")] GetSlideFactorZ,
		[AI_Definition(SCR: "Proc_JumpImpulsion", EN: "MEC_JumpImpulsion", FR: "MEC_ImpulsionSaut")] JumpImpulsion,
		// anim speed 
		[AI_Definition(SCR: "Proc_GetSpeedAnim", EN: "MEC_GetSpeedAnim", FR: "MEC_LitVitesseAnimation")] GetSpeedAnim,
		//********************************************************************************************************************************************************************************************************************************************************
		// Hierarchy - Platform 
		[AI_Definition(SCR: "proc_HIERGetFather", EN: "HIER_GetFather", FR: "HIER_MonPere")] HierGetFather,
		//********************************************************************************************************************************************************************************************************************************************************
		// Zone Activation Privileged 
		[AI_Definition(SCR: "Func_LitActivationZDD", EN: "ZON_GetZDDActivation", FR: "ZON_LitActivationZDD")] GetActivationZDD,
		[AI_Definition(SCR: "Func_LitActivationZDM", EN: "ZON_GetZDMActivation", FR: "ZON_LitActivationZDM")] GetActivationZDM,
		[AI_Definition(SCR: "Func_LitActivationZDE", EN: "ZON_GetZDEActivation", FR: "ZON_LitActivationZDE")] GetActivationZDE,
		[AI_Definition(SCR: "Func_LitActivationZDR", EN: "ZON_GetZDRActivation", FR: "ZON_LitActivationZDR")] GetActivationZDR,
		//********************************************************************************************************************************************************************************************************************************************************
		// Computing collision frequency 
		// Computing Brain frequency 
		// Computing Light frequency 
		[AI_Definition(SCR: "Func_GetCollComputationFrequency", EN: "ACT_GetCollComputationFrequency", FR: "ACT_LitFrequenceCalculCollisions")] GetCollisionFrequency,
		[AI_Definition(SCR: "Func_GetBrainComputationFrequency", EN: "ACT_GetBrainComputationFrequency", FR: "ACT_LitFrequenceCalculIA")] GetBrainFrequency,
		[AI_Definition(SCR: "Func_GetLightComputationFrequency", EN: "ACT_GetLightComputationFrequency", FR: "ACT_LitFrequenceCalculLumieres")] GetLightFrequency,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetBooleanInArray", EN: "ACT_GetBooleanInArray", FR: "ACT_LitBooleenDansTableau")] GetBooleanInArray,
		[AI_Definition(SCR: "Func_GetNumberOfBooleanInArray", EN: "ACT_GetNumberOfBooleanInArray", FR: "ACT_LitNombreDeBooleensDansTableau")] GetNumberOfBooleanInArray,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetButtonName", EN: "BUT_GetButtonName", FR: "BUT_GetButtonName")] GetButtonName,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetDriversAvailable", EN: "VID_GetDriversAvailable", FR: "VID_GetDriversAvailable")] GetDriversAvailable,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetCurrentLanguageId", EN: "TEXT_GetCurrentLanguageId", FR: "TEXT_GetCurrentLanguageId")] GetCurrentLanguageId,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetNbLanguages", EN: "TEXT_GetNbLanguages", FR: "TEXT_GetNbLanguages")] GetNbLanguages,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetLanguageText", EN: "TEXT_GetLanguageText", FR: "TEXT_GetLanguageText")] GetLanguageText,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_TextToInt", EN: "TEXT_TextToInt", FR: "TEXT_TexteEnEntier")] TextToInt,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetMusicVolume", EN: "OPTION_GetMusicVolume", FR: "Option_GetVolumeMusical")] GetMusicVolume,
		[AI_Definition(SCR: "Func_GetSfxVolume", EN: "OPTION_GetSfxVolume", FR: "Option_GetVolumeEffets")] GetSfxVolume,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_SlotIsValid", EN: "OPTION_SlotIsValid", FR: "Option_SlotIsValid")] SlotIsValid,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_NbAvailableResolution", EN: "VID_NbAvailableResolution", FR: "VID_NbAvailableResolution")] NbAvailableResolution,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_CurrentResolution", EN: "VID_CurrentResolution", FR: "VID_CurrentResolution")] CurrentResolution,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetBrightness", EN: "VID_GetBrightness", FR: "VID_GetBrightness")] GetBrightness,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_NameResolution", EN: "VID_NameResolution", FR: "VID_NameResolution")] NameResolution,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetNbSlotsAvailable", EN: "OPTION_GetNbSlotsAvailable", FR: "OPTION_GetNbSlotsAvailable")] GetNbSlotsAvailable,
		[AI_Definition(SCR: "Func_GetTextureFiltering", EN: "VID_GetTextureFiltering", FR: "VID_GetTextureFiltering")] GetTextureFiltering,
		[AI_Definition(SCR: "Func_GetAntiAliasing", EN: "VID_GetAntiAliasing", FR: "VID_GetAntiAliasing")] GetAntiAliasing,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetSaturationDistance", EN: "ACT_GetSaturationDistance", FR: "ACT_LitDistanceDeSaturation")] GetSaturationDistance,
		[AI_Definition(SCR: "Func_GetBackGroundDistance", EN: "ACT_GetBackGroundDistance", FR: "ACT_LitDistanceDeBackground")] GetBackgroundDistance,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetTooFarLimit", EN: "ACT_GetTooFarLimit", FR: "ACT_LitLimiteEloignement")] GetTooFarLimit,
		[AI_Definition(SCR: "Func_GetTransparencyZoneMin", EN: "ACT_GetTransparencyZoneMin", FR: "ACT_LitZoneMinTransparence")] GetTransparencyZoneMin,
		[AI_Definition(SCR: "Func_GetTransparencyZoneMax", EN: "ACT_GetTransparencyZoneMax", FR: "ACT_LitZoneMaxTransparence")] GetTransparencyZoneMax,
		//********************************************************************************************************************************************************************************************************************************************************
		// ANNECY MT - 30/03/99 { PC Protection Code 
		[AI_Definition(SCR: "Func_ExecuteVariable", EN: "PRO_ExecuteVariable", FR: "PRO_ExecuteVariable")] ExecuteVariable,
		[AI_Definition(SCR: "Func_ComputeProtectKey", EN: "ACT_ComputeProtectKey", FR: "ACT_CalculeClefProtection")] ComputeProtectKey,
		[AI_Definition(SCR: "Func_Xor", EN: "MATH_Xor", FR: "MATH_Xor")] Xor,
		[AI_Definition(SCR: "Func_DivU", EN: "MATH_DivU", FR: "MATH_DivU")] DivUnsigned,
		[AI_Definition(SCR: "Func_MulU", EN: "MATH_MulU", FR: "MATH_MulU")] MulUnsigned,
		[AI_Definition(SCR: "Func_AddU", EN: "MATH_AddU", FR: "MATH_AddU")] AddUnsigned,
		[AI_Definition(SCR: "Func_SubU", EN: "MATH_SubU", FR: "MATH_SubU")] SubUnsigned,
		[AI_Definition(SCR: "Func_MemoryValue", EN: "MATH_MemoryValue", FR: "MATH_ValeurMemoire")] GetMemoryValue,

		[AI_Definition(SCR: "Func_GetCheats", EN: "FUNC_GetCheats", FR: "FUNC_GetCheats")] GetCheats,
		[AI_Definition(SCR: "Func_GetBacklight", EN: "FUNC_GetBacklight", FR: "FUNC_GetBacklight")] GetBacklight,

		[AI_Definition(SCR: "Func_DoneAnalogCalibration", EN: "FUNC_DoneAnalogCalibration", FR: "FUNC_DoneAnalogCalibration")] DoneAnalogCalibration,
		#endregion

		#region DefFunCa
		[AI_Definition(SCR: "Proc_Cam_GetShiftTarget", EN: "Cam_GetShiftTarget", FR: "Cam_GetShiftTarget")] Cam_GetShiftTarget,
		[AI_Definition(SCR: "Proc_Cam_GetShiftPos", EN: "Cam_GetShiftPos", FR: "Cam_GetShiftPos")] Cam_GetShiftPos,

		[AI_Definition(SCR: "Fct_Cam_GetDistMin", EN: "Cam_GetDistMin", FR: "Cam_GetDistMin")] Cam_GetDistMin,
		[AI_Definition(SCR: "Fct_Cam_GetDistMax", EN: "Cam_GetDistMax", FR: "Cam_GetDistMax")] Cam_GetDistMax,
		[AI_Definition(SCR: "Fct_Cam_GetBoundDistMin", EN: "Cam_GetBoundDistMin", FR: "Cam_GetBoundDistMin")] Cam_GetBoundDistMin,
		[AI_Definition(SCR: "Fct_Cam_GetBoundDistMax", EN: "Cam_GetBoundDistMax", FR: "Cam_GetBoundDistMax")] Cam_GetBoundDistMax,
		[AI_Definition(SCR: "Fct_Cam_GetAngleAlpha", EN: "Cam_GetAngleAlpha", FR: "Cam_GetAngleAlpha")] Cam_GetAngleAlpha,
		[AI_Definition(SCR: "Fct_Cam_GetAngleShiftAlpha", EN: "Cam_GetAngleShiftAlpha", FR: "Cam_GetAngleShiftAlpha")] Cam_GetAngleShiftAlpha,
		[AI_Definition(SCR: "Fct_Cam_GetAngleTheta", EN: "Cam_GetAngleTheta", FR: "Cam_GetAngleTheta")] Cam_GetAngleTheta,
		[AI_Definition(SCR: "Fct_Cam_GetAngleShiftTheta", EN: "Cam_GetAngleShiftTheta", FR: "Cam_GetAngleShiftTheta")] Cam_GetAngleShiftTheta,
		[AI_Definition(SCR: "Fct_Cam_GetLinearSpeed", EN: "Cam_GetLinearSpeed", FR: "Cam_GetLinearSpeed")] Cam_GetLinearSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetLinearIncreaseSpeed", EN: "Cam_GetLinearIncreaseSpeed", FR: "Cam_GetLinearIncreaseSpeed")] Cam_GetLinearIncreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetLinearDecreaseSpeed", EN: "Cam_GetLinearDecreaseSpeed", FR: "Cam_GetLinearDecreaseSpeed")] Cam_GetLinearDecreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetAngularSpeed", EN: "Cam_GetAngularSpeed", FR: "Cam_GetAngularSpeed")] Cam_GetAngularSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetAngularIncreaseSpeed", EN: "Cam_GetAngularIncreaseSpeed", FR: "Cam_GetAngularIncreaseSpeed")] Cam_GetAngularIncreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetAngularDecreaseSpeed", EN: "Cam_GetAngularDecreaseSpeed", FR: "Cam_GetAngularDecreaseSpeed")] Cam_GetAngularDecreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetTargetSpeed", EN: "Cam_GetTargetSpeed", FR: "Cam_GetTargetSpeed")] Cam_GetTargetSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetTargetIncreaseSpeed", EN: "Cam_GetTargetIncreaseSpeed", FR: "Cam_GetTargetIncreaseSpeed")] Cam_GetTargetIncreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetTargetDecreaseSpeed", EN: "Cam_GetTargetDecreaseSpeed", FR: "Cam_GetTargetDecreaseSpeed")] Cam_GetTargetDecreaseSpeed,
		[AI_Definition(SCR: "Fct_Cam_GetFocal", EN: "Cam_GetFocal", FR: "Cam_GetFocal")] Cam_GetFocal,
		[AI_Definition(SCR: "Fct_Cam_GetZMin", EN: "Cam_GetZMin", FR: "Cam_GetZMin")] Cam_GetZMin,
		[AI_Definition(SCR: "Fct_Cam_GetZMax", EN: "Cam_GetZMax", FR: "Cam_GetZMax")] Cam_GetZMax,

		[AI_Definition(SCR: "Fct_Cam_GetTargetedSuperObject", EN: "Cam_GetTargetedSuperObject", FR: "Cam_GetTargetedSuperObject")] Cam_GetTargetedSuperObject,
		[AI_Definition(SCR: "Fct_Cam_GetTypeOfViewport", EN: "Cam_GetTypeOfViewport", FR: "Cam_GetTypeOfViewport")] Cam_GetTypeOfViewport,
		[AI_Definition(SCR: "Fct_Cam_GetCameraOfViewport", EN: "Cam_GetCameraOfViewport", FR: "Cam_GetCameraOfViewport")] Cam_GetCameraOfViewport,
		[AI_Definition(SCR: "Fct_Cam_GetMainCamera", EN: "Cam_GetMainCamera", FR: "Cam_GetMainCamera")] Cam_GetMainCamera,

		[AI_Definition(SCR: "Fct_Cam_ComputeTargetWithTgtPerso", EN: "Cam_ComputeTargetWithTgtPerso", FR: "Cam_ComputeTargetWithTgtPerso")] Cam_ComputeTargetWithTgtPerso,
		[AI_Definition(SCR: "Fct_Cam_ComputeCurrentTarget", EN: "Cam_ComputeCurrentTarget", FR: "Cam_ComputeCurrentTarget")] Cam_GetCurrentTargetPosition,


		[AI_Definition(SCR: "Fct_Cam_GetSectorCameraType", EN: "Cam_GetSectorCameraType", FR: "Cam_GetSectorCameraType")] Cam_GetSectorCameraType,
		[AI_Definition(SCR: "Fct_Cam_GetBestPos", EN: "Cam_GetBestPos", FR: "Cam_GetBestPos")] Cam_GetBestPos,
		#endregion

		#region FuncRay2
		//********************************************************************************************************************************************************************************************************************************************************
		// Management of magic points for Rayman 
		[AI_Definition(SCR: "Func_LitPointsDeMagie", EN: "RAY_GetMagicPoints", FR: "RAY_LitPointsDeMagie")] LitPointsDeMagie,
		[AI_Definition(SCR: "Func_LitPointsDeMagieMax", EN: "RAY_GetMagicPointsMax", FR: "RAY_LitPointsDeMagieMax")] LitPointsDeMagieMax,
		[AI_Definition(SCR: "Func_AjouteEtLitPointsDeMagie", EN: "RAY_AddAndGetMagicPoints", FR: "RAY_AjouteEtLitPointsDeMagie")] AjouteEtLitPointsDeMagie,
		[AI_Definition(SCR: "Func_AjouteEtLitPointsDeMagieMax", EN: "RAY_AddAndGetMagicPointsMax", FR: "RAY_AjouteEtLitPointsDeMagieMax")] AjouteEtLitPointsDeMagieMax,
		[AI_Definition(SCR: "Func_EnleveEtLitPointsDeMagie", EN: "RAY_RemoveAndGetMagicPoints", FR: "RAY_EnleveEtLitPointsDeMagie")] EnleveEtLitPointsDeMagie,
		[AI_Definition(SCR: "Func_EnleveEtLitPointsDeMagieMax", EN: "RAY_RemoveAndGetMagicPointsMax", FR: "RAY_EnleveEtLitPointsDeMagieMax")] EnleveEtLitPointsDeMagieMax,
		// Management of air points for Rayman 
		[AI_Definition(SCR: "Func_LitPointsDair", EN: "RAY_GetAirPoints", FR: "RAY_LitPointsDair")] LitPointsDair,
		[AI_Definition(SCR: "Func_LitPointsDairMax", EN: "RAY_GetAirPointsMax", FR: "RAY_LitPointsDairMax")] LitPointsDairMax,
		[AI_Definition(SCR: "Func_AjouteEtLitPointsDair", EN: "RAY_AddAndGetAirPoints", FR: "RAY_AjouteEtLitPointsDair")] AjouteEtLitPointsDair,
		[AI_Definition(SCR: "Func_AjouteEtLitPointsDairMax", EN: "RAY_AddAndGetAirPointsMax", FR: "RAY_AjouteEtLitPointsDairMax")] AjouteEtLitPointsDairMax,
		[AI_Definition(SCR: "Func_EnleveEtLitPointsDair", EN: "RAY_RemoveAndGetAirPoints", FR: "RAY_EnleveEtLitPointsDair")] EnleveEtLitPointsDair,
		[AI_Definition(SCR: "Func_EnleveEtLitPointsDairMax", EN: "RAY_RemoveAndGetAirPointsMax", FR: "RAY_EnleveEtLitPointsDairMax")] EnleveEtLitPointsDairMax,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_PersoLePlusProche", EN: "ACT_NearestActor", FR: "ACT_PersoLePlusProche")] PersoLePlusProche,
		[AI_Definition(SCR: "Func_NearestActorInCurrentSector", EN: "ACT_NearestActorInCurrentSector", FR: "ACT_PersoLePlusProcheDansSecteurCourant")] PersoLePlusProcheDansSecteurCourant,
		[AI_Definition(SCR: "Func_NearerActorInFieldOfVision", EN: "ACT_NearerActorInFieldOfVision", FR: "ACT_PersoLePlusProcheDansChampsDeVision")] NearerActorInFieldOfVision,
		[AI_Definition(SCR: "Func_GetNbActivePerso", EN: "ACT_GetNbActivePerso", FR: "ACT_LitNbPersoActifs")] GetNbActivePerso,
		[AI_Definition(SCR: "Func_CibleLaPlusProche", EN: "ACT_CibleLaPlusProche", FR: "ACT_CibleLaPlusProche")] CibleLaPlusProche,
		[AI_Definition(SCR: "Func_CibleLaPlusProcheavecAngles", EN: "ACT_CibleLaPlusProcheAvecAngles", FR: "ACT_CibleLaPlusProcheAvecAngles")] CibleLaPlusProcheAvecAngles,
		//********************************************************************************************************************************************************************************************************************************************************
		// Graph function management 

		// To reach a graph before building a way (or to travel on a graph without building a way) 
		[AI_Definition(SCR: "Func_ReseauWPnLePlusProche", EN: "NETWORK_CloserWPn", FR: "Reseau_WPLePlusProche")] ReseauWPLePlusProche,
		[AI_Definition(SCR: "Func_NetworkCloserWPOfType", EN: "NETWORK_CloserWPOfType", FR: "Reseau_WPDeTelTypeLePlusProche")] NetworkCloserWPOfType,
		[AI_Definition(SCR: "Func_ReseauWPnLePlusDansLAxe", EN: "NETWORK_CloserWPnInAxis", FR: "Reseau_WPLePlusDansLAxe")] ReseauWPLePlusDansLAxe,
		[AI_Definition(SCR: "Func_ReseauWPnLePlusDansLAxe2", EN: "NETWORK_CloserWPnInAxis2", FR: "Reseau_WPDeTelTypeLePlusDansLAxe")] ReseauWPLePlusDansLAxe2,

		// To travel on a graph without building a way 
		[AI_Definition(SCR: "eFunc_NetworkNextWPWithCapa", EN: "NETWORK_NextWPWithCapa", FR: "Reseau_ProchainWPJoignableAvecCapacite")] NetworkNextWPWithCapa,
		[AI_Definition(SCR: "Func_NetworkAffectTypeOfConnectedWP", EN: "NETWORK_AffectTypeOfConnectedWP", FR: "Reseau_ChangeTypeDesWPJoignables")] NetworkAffectTypeOfConnectedWP,
		[AI_Definition(SCR: "Func_NetworkAffectTypeOfConnectedWP2", EN: "NETWORK_AffectTypeOfConnectedWP2", FR: "Reseau_ChangeTypeDesWPJoignablesAvecCapacite")] NetworkAffectTypeOfConnectedWPWithCapa,

		// To build a way from a graph 
		[AI_Definition(SCR: "Func_ReseauCheminLePlusCourt", EN: "NETWORK_ShorterWay", FR: "Reseau_CheminLePlusCourt")] ReseauCheminLePlusCourt,
		[AI_Definition(SCR: "Func_NetworkBuildOrderedPath", EN: "NETWORK_BuildOrderedPath", FR: "Reseau_ConstruitCheminOrdonne")] NetworkBuildOrderedPath,
		[AI_Definition(SCR: "Func_NetworkBuildOrderedPathCircular", EN: "NETWORK_BuildOrderedPathCircular", FR: "Reseau_ConstruitCheminCirculaireOrdonne")] NetworkBuildOrderedPathCircular,
		[AI_Definition(SCR: "Func_NetworkAllocateGraphToMSWay", EN: "NETWORK_AllocateGraphToMSWay", FR: "Reseau_AffecteGraphAChemin")] NetworkAllocateGraphToMSWay,
		[AI_Definition(SCR: "Func_NetworkAllocateGraphToMSWayCircular", EN: "NETWORK_AllocateGraphToMSWay", FR: "Reseau_AffecteGraphACheminCirculaire")] NetworkAllocateGraphToMSWayCircular,

		// To travel on the way built (no graph in parameter, bicose everything is tested with the MsWay) 
		[AI_Definition(SCR: "Func_ReseauLitIndexCourant", EN: "NETWORK_GetCurrentIndex", FR: "Reseau_LitIndexCourant")] ReseauLitIndexCourant,
		[AI_Definition(SCR: "Func_ReseauForceIndexCourant", EN: "NETWORK_SetCurrentIndex", FR: "Reseau_ForceIndexCourant")] ReseauForceIndexCourant,
		[AI_Definition(SCR: "Func_ReseauLitPremierIndex", EN: "NETWORK_GetFirstIndex", FR: "Reseau_LitPremierIndex")] ReseauLitPremierIndex,
		[AI_Definition(SCR: "Func_ReseauLitDernierIndex", EN: "NETWORK_GetLastIndex", FR: "Reseau_LitDernierIndex")] ReseauLitDernierIndex,
		[AI_Definition(SCR: "Func_ReseauIncrementIndex", EN: "NETWORK_IncrementIndex", FR: "Reseau_IncrementIndex")] ReseauIncrementIndex,
		[AI_Definition(SCR: "Func_ReseauDecrementIndex", EN: "NETWORK_DecrementIndex", FR: "Reseau_DecrementIndex")] ReseauDecrementIndex,
		[AI_Definition(SCR: "Func_ReseauLitWPAIndex", EN: "NETWORK_GetWPAIndex", FR: "Reseau_LitWPAIndex")] ReseauLitWPAIndex,

		[AI_Definition(SCR: "Func_ReseauLitCapaciteLiaisonAIndex", EN: "NETWORK_GetLinkCapacityToIndex", FR: "Reseau_LitCapaciteLiaisonAIndex")] ReseauLitCapaciteLiaisonAIndex,
		[AI_Definition(SCR: "Func_ReseauChangeCapaciteLiaisonAIndex", EN: "NETWORK_ChangeLinkCapacityToIndex", FR: "Reseau_ChangeCapaciteLiaisonAIndex")] ReseauChangeCapaciteLiaisonAIndex,
		[AI_Definition(SCR: "Func_ReseauLitPoidsLiaisonAIndex", EN: "NETWORK_GetLinkWeightToIndex", FR: "Reseau_LitPoidsLiaisonAIndex")] ReseauLitPoidsLiaisonAIndex,
		[AI_Definition(SCR: "Func_ReseauChangePoidsLiaisonAIndex", EN: "NETWORK_ChangeLinkWeightToIndex", FR: "Reseau_ChangePoidsLiaisonAIndex")] ReseauChangePoidsLiaisonAIndex,

		[AI_Definition(SCR: "Func_NetworkGetIndexOfWPInMSWay", EN: "NETWORK_GetIndexOfWPInMSWay", FR: "Reseau_ChercheIndiceDuWP")] NetworkGetIndexOfWPInMSWay,
		[AI_Definition(SCR: "Func_NetworkForceWPToCurrent", EN: "NETWORK_ForceWPToCurrent", FR: "Reseau_ForceWPCourant")] NetworkForceWPToCurrent,

		[AI_Definition(SCR: "Func_ReseauTestExtremite", EN: "NETWORK_TestExtremite", FR: "Reseau_TestExtremities")] NetworkTestTheEnds,


		// miscellaneous 
		[AI_Definition(SCR: "Func_ReseauLitCapaciteLiaisonDansGraph", EN: "NETWORK_GetLinkCapInGraph", FR: "Reseau_LitCapaciteLiaisonDansGraph")] ReseauLitCapaciteLiaisonDansGraph,
		[AI_Definition(SCR: "Func_ReseauChangeCapaciteLiaisonDansGraph", EN: "NETWORK_SetLinkCapInGraph", FR: "Reseau_ChangeCapaciteLiaisonDansGraph")] ReseauChangeCapaciteLiaisonDansGraph,
		[AI_Definition(SCR: "Func_ReseauLitPoidsLiaisonDansGraph", EN: "NETWORK_GetLinkWeightInGraph", FR: "Reseau_LitPoidsLiaisonDansGraph")] ReseauLitPoidsLiaisonDansGraph,
		[AI_Definition(SCR: "Func_ReseauChangePoidsLiaisonDansGraph", EN: "NETWORK_SetLinkWeightInGraph", FR: "Reseau_ChangePoidsLiaisonDansGraph")] ReseauChangePoidsLiaisonDansGraph,

		[AI_Definition(SCR: "Func_NetworkGetTypeOfWP", EN: "NETWORK_GetTypeOfWP", FR: "Reseau_LitTypeDuWP")] NetworkGetTypeOfWP,
		// Capability functions 
		[AI_Definition(SCR: "Func_CapsGetCapabilities", EN: "CAPS_GetCapabilities", FR: "Caps_LitCapacites")] GetCapabilities,
		[AI_Definition(SCR: "Func_CapabilityAtBitNumber", EN: "CAPS_CapabilityAtBitNumber", FR: "Caps_CapaciteAuBitNumero")] CapabilityAtBitNumber,

		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetScrollSpeed", EN: "MAT_GetScrollSpeed", FR: "MAT_VitesseTexture")] GetScrollSpeed,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetNbFrame", EN: "MAT_GetNbFrame", FR: "ANI_LitNbFrame")] GetNbFrame,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_DotProduct", EN: "VEC_DotProduct", FR: "VEC_ProduitScalaire")] DotProduct,
		[AI_Definition(SCR: "Func_CrossProduct", EN: "VEC_CrossProduct", FR: "VEC_ProduitVectoriel")] CrossProduct,
		[AI_Definition(SCR: "Func_Normalize", EN: "VEC_Normalize", FR: "VEC_Normer")] Normalize,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetSPOCoordinates", EN: "SPO_GetSPOCoordinates", FR: "SPO_LitCoordonnees")] GetSPOCoordinates,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_ACTGetTractionFactor", EN: "ACT_GetTractionFactor", FR: "ACT_LitFacteurDeTraction")] GetTractionFactor,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetCenterZDEType", EN: "ZON_GetCenterZDEType", FR: "ZON_LitCentreZDEType")] GetCenterZDEType,
		[AI_Definition(SCR: "Func_GetCenterZDMType", EN: "ZON_GetCenterZDMType", FR: "ZON_LitCentreZDMType")] GetCenterZDMType,
		[AI_Definition(SCR: "Func_GetCenterZDRType", EN: "ZON_GetCenterZDRType", FR: "ZON_LitCentreZDRType")] GetCenterZDRType,
		[AI_Definition(SCR: "Func_GetCenterZDDType", EN: "ZON_GetCenterZDDType", FR: "ZON_LitCentreZDDType")] GetCenterZDDType,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_TextAffiche", EN: "TEXT_DisplayText", FR: "TEXT_Affiche")] TextAffiche,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_GetCPUCounter", EN: "DEBUG_GetCPUCounter", FR: "DEBUG_LitCompteurCPU")] GetCPUCounter,
		#endregion
	}
}