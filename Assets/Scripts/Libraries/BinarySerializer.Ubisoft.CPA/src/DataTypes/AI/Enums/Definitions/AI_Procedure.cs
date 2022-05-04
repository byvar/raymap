namespace BinarySerializer.Ubisoft.CPA {
	// Regex for NP++:
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	//                                       eCond                 , FR                   ,  EN                  , SCR
	// Replace: \[AI_Definition\(SCR: \6, EN: \5, FR: \3\)] \2,
	// Then replace " EN: ," with ""
	public enum AI_Procedure {
		#region DefProc
		//********************************************************************************************************************************************************************************************************************************************************
		// HitPoints procedures 
		[AI_Definition(SCR: "Proc_SetHitPoints", EN: "ACT_SetHitPoints", FR: "ACT_FixePointsDeVie")] SetHitPoints,
		[AI_Definition(SCR: "Proc_SetHitPointsInit", EN: "ACT_SetHitPointsInit", FR: "ACT_FixePointsDeVieInit")] SetHitPointsInit,
		[AI_Definition(SCR: "Proc_SetHitPointsToInit", EN: "ACT_SetHitPointsToInit", FR: "ACT_ReinitPointsDeVie")] SetHitPointsToInitValue,
		[AI_Definition(SCR: "Proc_SetHitPointsToMax", EN: "ACT_SetHitPointsToMax", FR: "ACT_ReinitPointsDeVieAMax")] SetHitPointsToMaxValue,
		[AI_Definition(SCR: "Proc_AddHitPoints", EN: "ACT_AddHitPoints", FR: "ACT_AjoutePointsDeVie")] AddHitPoints,
		[AI_Definition(SCR: "Proc_SubHitPoints", EN: "ACT_SubHitPoints", FR: "ACT_EnlevePointsDeVie")] SubHitPoints,
		// HitPointsMax procedures 
		[AI_Definition(SCR: "Proc_SetHitPointsMax", EN: "ACT_SetHitPointsMax", FR: "ACT_FixePointsDeVieMax")] SetHitPointsMax,
		[AI_Definition(SCR: "Proc_SetHitPointsMaxToInit", EN: "ACT_SetHitPointsMaxToInit", FR: "ACT_ReinitPointsDeVieMax")] SetHitPointsMaxToInitValue,
		[AI_Definition(SCR: "Proc_SetHitPointsMaxToMax", EN: "ACT_SetHitPointsMaxToMax", FR: "ACT_ReinitPointsDeVieMaxAMax")] SetHitPointsMaxToMaxValue,
		[AI_Definition(SCR: "Proc_AddHitPointsMax", EN: "ACT_AddHitPointsMax", FR: "ACT_AjoutePointsDeVieMax")] AddHitPointsMax,
		[AI_Definition(SCR: "Proc_SubHitPointsMax", EN: "ACT_SubHitPointsMax", FR: "ACT_EnlevePointsDeVieMax")] SubHitPointsMax,
		// Transparence 
		[AI_Definition(SCR: "Proc_TransparentDisplay", EN: "ACT_TransparentDisplay", FR: "ACT_ActivationTransparence")] TransparentDisplay,
		[AI_Definition(SCR: "Proc_SetTransparency", EN: "ACT_SetTransparency", FR: "ACT_NiveauTransparence")] SetTransparency,
		// draw mask 
		[AI_Definition(SCR: "Proc_ACT_SetDrawFlag", EN: "ACT_SetDrawFlag", FR: "ACT_SetDrawFlag")] ACT_SetDrawFlag,
		//*********************************************************************************************************************************************************************************************************************************************************
		// Transparence 
		[AI_Definition(SCR: "Proc_ModuleTransparentDisplay", EN: "MOD_ModuleTransparentDisplay", FR: "MOD_ActivationTransparence")] ModuleTransparentDisplay,
		[AI_Definition(SCR: "Proc_ModuleTransparentDisplay2", EN: "MOD_ModuleTransparentDisplay2", FR: "MOD_ActivationTransparence2")] ModuleTransparentDisplay2,
		[AI_Definition(SCR: "Proc_SetModuleTransparency", EN: "MOD_SetModuleTransparency", FR: "MOD_NiveauTransparence")] SetModuleTransparency,
		[AI_Definition(SCR: "Proc_SetModuleTransparency2", EN: "MOD_SetModuleTransparency2", FR: "MOD_NiveauTransparence2")] SetModuleTransparency2,
		//*********************************************************************************************************************************************************************************************************************************************************
		// List ZDD ZDE procedures 
		[AI_Definition(SCR: "Proc_ListAffectWithPersoZDD", EN: "LST_ListAffectWithPersoZDD", FR: "LST_AffecteListeAvecZDDPerso")] ListAffectPersoZDD,
		[AI_Definition(SCR: "Proc_ListAffectWithModuleZDD", EN: "LST_ListAffectWithModuleZDD", FR: "LST_AffecteListeAvecZDDModule")] ListAffectModuleZDD,
		[AI_Definition(SCR: "Proc_ListAffectWithPersoZDE", EN: "LST_ListAffectWithPersoZDE", FR: "LST_AffecteListeAvecZDEPerso")] ListAffectPersoZDE,
		[AI_Definition(SCR: "Proc_ListAffectWithModuleZDE", EN: "LST_ListAffectWithModuleZDE", FR: "LST_AffecteListeAvecZDEModule")] ListAffectModuleZDE,
		[AI_Definition(SCR: "Proc_ListAffectWithPersoTypeZDE", EN: "LST_ListAffectWithPersoTypeZDE", FR: "LST_AffecteListeAvecTypeZDEPerso")] ListAffectPersoTypeZDE,
		[AI_Definition(SCR: "Proc_ListAffectWithModuleTypeZDE", EN: "LST_ListAffectWithModuleTypeZDE", FR: "LST_AffecteListeAvecTypeZDEModule")] ListAffectModuleTypeZDE,
		[AI_Definition(SCR: "Proc_ListAffectTypeZDEWithTypeZDE", EN: "LST_ListAffectTypeZDEWithTypeZDE", FR: "LST_AffecteListeTypeZDEAvecTypeZDE")] ListAffectTypeZDEWithTypeZDE,
		// List Misc procedures 
		[AI_Definition(SCR: "Proc_AddPersoInList", EN: "LST_AddPersoInList", FR: "LST_AjoutePersoDansListe")] AddPersoInList,
		[AI_Definition(SCR: "Proc_AddPersoInListAt", EN: "LST_AddPersoInListAt", FR: "LST_AjoutePersoDansListeA")] AddPersoInListAt,
		[AI_Definition(SCR: "Proc_DeletePersoAtInList", EN: "LST_DeletePersoAtInList", FR: "LST_DetruitPersoDansListe")] DeletePersoAtInList,
		[AI_Definition(SCR: "Proc_FindPersoAndDeleteInList", EN: "LST_FindPersoAndDeleteInList", FR: "LST_TrouveEtDetruitPersoDansListe")] FindPersoAndDeleteInList,
		[AI_Definition(SCR: "Proc_Select", EN: "LST_Select", FR: "LST_SelectDansListe")] ListSelect,
		[AI_Definition(SCR: "Proc_UnSelect", EN: "LST_UnSelect", FR: "LST_UnSelectDansListe")] ListUnSelect,
		[AI_Definition(SCR: "Proc_ListSort", EN: "LST_ListSort", FR: "LST_TrieListe")] ListSort,
		[AI_Definition(SCR: "Proc_ListSortByFamily", EN: "LST_ListSortByFamily", FR: "LST_TrieListeParFamille")] ListSortByFamily,
		[AI_Definition(SCR: "Proc_ListSortByModele", EN: "LST_ListSortByModele", FR: "LST_TrieListeParModele")] ListSortByModel,
		[AI_Definition(SCR: "Proc_FillListWithAllPersoOfAFamily", EN: "LST_ListSortByModele", FR: "LST_RemplirListeAvecTouteUneFamille")] FillListWithAllPersoOfAFamily,
		[AI_Definition(SCR: "Proc_FillListWithAllPersoOfAModel", EN: "LST_FillListWithAllPersoOfAModel", FR: "LST_RemplirListeAvecToutUnModele")] FillListWithAllPersoOfAModel,
		[AI_Definition(SCR: "Proc_DeleteFamilyInList", EN: "LST_DeleteFamilyInList", FR: "LST_DetruitFamilleDansListe")] DeleteFamilyInList,
		[AI_Definition(SCR: "Proc_DeleteModelInList", EN: "LST_DeleteModelInList", FR: "LST_DetruitModeleDansListe")] DeleteModelInList,
		// List Ensemble procedures 
		[AI_Definition(SCR: "Proc_ListUnion", EN: "LST_DeleteModelInList", FR: "LST_ListeUnion")] ListUnion,
		[AI_Definition(SCR: "Proc_ListInter", EN: "LST_ListInter", FR: "LST_ListeInter")] ListInter,
		[AI_Definition(SCR: "Proc_ListDiff", EN: "LST_ListDiff", FR: "LST_ListeDiff")] ListDiff,
		[AI_Definition(SCR: "Proc_ListAdd", EN: "LST_ListAdd", FR: "LST_ListeAjoute")] ListAdd,
		//********************************************************************************************************************************************************************************************************************************************************
		// Brouillards 
		[AI_Definition(SCR: "FOG_Proc_Activate", EN: "FOG_Activate", FR: "LUM_BrouillardActive")] FogActivate,
		[AI_Definition(SCR: "FOG_Proc_SetColor", EN: "FOG_SetColor", FR: "LUM_BrouillardFixeCouleur")] SetFogColor,
		[AI_Definition(SCR: "FOG_Proc_SetNearFarInf", EN: "FOG_SetNearFarInf", FR: "LUM_BrouillardFixeDistances")] SetFogNearFarInf,
		[AI_Definition(SCR: "FOG_Proc_SetBlend", EN: "FOG_SetBlend", FR: "LUM_BrouillardFixeBlend")] SetFogBlend,
		[AI_Definition(SCR: "FOG_Proc_RestoreFog", EN: "FOG_RestoreFog", FR: "LUM_RestaurerBrouillard")] RestoreFog,
		[AI_Definition(SCR: "FOG_Proc_SaveFog", EN: "FOG_SaveFog", FR: "LUM_SauverBrouillard")] SaveFog,
		//***********************************************************************************************************************************
		// Magnet MGT procedure 
		[AI_Definition(SCR: "Procedure_Magnet_ActiveMagnet", EN: "MAGNET_ON", FR: "Magnet_ON")] Magnet_ActiveMagnet,
		[AI_Definition(SCR: "Procedure_Magnet_DeactiveMagnet", EN: "MAGNET_OFF", FR: "Magnet_OFF")] Magnet_DeactiveMagnet,
		[AI_Definition(SCR: "Procedure_Magnet_SetStrength", EN: "MAGNET_SetStrength", FR: "Magnet_SetStrength")] Magnet_SetStrength,
		[AI_Definition(SCR: "Procedure_Magnet_SetFar", EN: "MAGNET_SetFar", FR: "Magnet_SetFar")] Magnet_SetFar,
		[AI_Definition(SCR: "Procedure_Magnet_SetNear", EN: "MAGNET_SetNear", FR: "Magnet_SetNear")] Magnet_SetNear,
		[AI_Definition(SCR: "Procedure_Magnet_SetDuration", EN: "MAGNET_SetDuration", FR: "Magnet_SetDuration")] Magnet_SetDuration,
		//********************************************************************************************************************************************************************************************************************************************************
		// FootPath 
		[AI_Definition(SCR: "Proc_FootPath_AddFootPrint", EN: "FootPath_AddFootPrint", FR: "FootPath_AddFootPrint")] FootPath_AddFootPrint,
		[AI_Definition(SCR: "Proc_FootPath_Clear", EN: "FootPath_Clear", FR: "FootPath_Clear")] FootPath_Clear,
		//********************************************************************************************************************************************************************************************************************************************************
		// Effect 
		[AI_Definition(SCR: "Proc_SinusEffect_SetFreq", EN: "EFFECT_SetSinusFreq", FR: "Effect_SetSinusFreq")] SinEffect_SetFreq,
		[AI_Definition(SCR: "Proc_SinusEffect_SetAmplitude", EN: "EFFECT_SetSinusAmplitude", FR: "Effect_SetSinusAmplitude")] SinEffect_SetAmplitude,
		[AI_Definition(SCR: "Proc_SinusEffect_SetState", EN: "EFFECT_SetSinusState", FR: "Effect_SetSinusState")] SinEffect_SetState,
		[AI_Definition(SCR: "Proc_SinusEffect_SetFreq3D", EN: "EFFECT_SetSinusFreq3D", FR: "Effect_SetSinusFreq3D")] SinEffect_SetFreq3D,
		[AI_Definition(SCR: "Proc_SinusEffect_SetRLIAmplitude", EN: "EFFECT_SetRLISinusAmplitude", FR: "Effect_SetRLISinusAmplitude")] SinEffect_SetRLIAmplitude,
		[AI_Definition(SCR: "Proc_SinusEffect_SetRLIBase", EN: "EFFECT_SetRLISinusBase", FR: "Effect_SetRLISinusBase")] SinEffect_SetRLIBase,

		//********************************************************************************************************************************************************************************************************************************************************
		// SuperObject Draw mask 
		[AI_Definition(SCR: "Proc_SPO_SetDrawFlag", EN: "SPO_SetDrawFlag", FR: "SPO_SetDrawFlag")] SPO_SetDrawFlag,
		[AI_Definition(SCR: "Proc_SPO_SetEngineDisplayModeFlag", EN: "SPO_SetEngineDisplayModeFlag", FR: "SPO_SetEngineDisplayModeFlag")] SPO_SetEngineDisplayModeFlag,
		//********************************************************************************************************************************************************************************************************************************************************
		// NO FAMILY 
		[AI_Definition(SCR: "Proc_DeactivateBut", EN: "DeactivateBut", FR: "BoutonDesactive")] DeactivateBut,
		[AI_Definition(SCR: "Proc_ActivateBut", EN: "ActivateBut", FR: "BoutonActive")] ActivateBut,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_None", EN: "None", FR: "NeRienFaire")] None,
		[AI_Definition(SCR: "Proc_ChangeComport", EN: "ChangeComport", FR: "ChangeComportement")] ChangeComport,
		[AI_Definition(SCR: "Proc_ChangeComportReflex", EN: "ChangeComportReflex", FR: "ChangeComportementReflexe")] ChangeComportReflex,
		[AI_Definition(SCR: "Proc_ChangeMyComport", EN: "ChangeMyComport", FR: "ChangeMonComportement")] ChangeMyComport,
		[AI_Definition(SCR: "Proc_ChangeMyComportReflex", EN: "ChangeMyComportReflex", FR: "ChangeMonComportementReflexe")] ChangeMyComportReflex,
		[AI_Definition(SCR: "Proc_ChangeMyComportAndMyReflex", EN: "ChangeMyComportAndMyReflex", FR: "ChangeMonComportementEtMonReflexe")] ChangeMyComportAndMyReflex,
		[AI_Definition(SCR: "Proc_ChangeAction", EN: "ChangeAction", FR: "ChangeAction")] ChangeAction,
		[AI_Definition(SCR: "Proc_ChangeActionForce", EN: "ChangeActionForce", FR: "ChangeActionForce")] ChangeActionForce,
		[AI_Definition(SCR: "Proc_ChangeActionRandom", EN: "ChangeActionRandom", FR: "ChangeActionAleatoire")] ChangeActionRandom,
		[AI_Definition(SCR: "Proc_ChangeActionWithEvents", EN: "ChangeActionWithEvents", FR: "ChangeActionAvecEvenements")] ChangeActionWithEvents,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_Loop", EN: "Loop", FR: "Boucle")] Boucle,
		[AI_Definition(SCR: "Proc_EndLoop", EN: "EndLoop", FR: "FinBoucle")] FinBoucle,
		[AI_Definition(SCR: "Proc_Break", EN: "Break", FR: "Break")] Break,

		//********************************************************************************************************************************************************************************************************************************************************
		// Misc procedures with no parameter 
		[AI_Definition(SCR: "Proc_PlayerIsDead", EN: "ACT_PlayerIsDead", FR: "MortDuJoueur")] PlayerIsDead,
		[AI_Definition(SCR: "Proc_RestoreCardParameters", EN: "MEC_RestoreCardParameters", FR: "MEC_RestaureParametresCarte")] ResetDynamicsWithCard,
		[AI_Definition(SCR: "Proc_BreakAI", EN: "BreakAI", FR: "BreakAI")] BreakAI,
		[AI_Definition(SCR: "Proc_IgnoreTraceFlagForNextPicking", EN: "Proc_IgnoreTraceFlagForNextPicking", FR: "COL_IgnoreFlagRayTraceAuProchainPicking")] IgnoreTraceFlagForNextPicking,

		//********************************************************************************************************************************************************************************************************************************************************
		// Misc procedures with one parameter 
		[AI_Definition(SCR: "Proc_SetMainActor", EN: "ACT_SetMainActor", FR: "FixeActeurPrincipal")] SetMainActor,
		[AI_Definition(SCR: "Proc_ActivateObject", EN: "SPO_ActivateObject", FR: "ActiveObjet")] ActivateObject,
		[AI_Definition(SCR: "Proc_DesactivateObject", EN: "SPO_DesactivateObject", FR: "DesactiveObjet")] DesactivateObject,
		[AI_Definition(SCR: "Proc_ChangeMap", EN: "MAP_ChangeMap", FR: "ChangeMap")] ChangeMap,
		[AI_Definition(SCR: "Proc_ChangeMapNoAutosave", EN: "MAP_ChangeMapNoAutosave", FR: "ChangeMapSansSauver")] ChangeMapNoAutosave,
		[AI_Definition(SCR: "Proc_SetActionReturn", EN: "SetActionReturn", FR: "ForceValeurDeRetourMetaAction")] SetActionReturn,
		[AI_Definition(SCR: "Proc_FactorAnimationFrameRate", EN: "FactorAnimationFrameRate", FR: "FixeFacteurVitesseAnimation")] FactorAnimationFrameRate,
		[AI_Definition(SCR: "Proc_SetCharacterPriority", EN: "ACT_SetCharacterPriority", FR: "ACT_FixePriorite")] SetCharacterPriority,
		[AI_Definition(SCR: "Proc_ForcePersoHandling", EN: "ACT_ForcePersoHandling", FR: "ACT_ForceTraitementPerso")] ForcePersoHandling,
		[AI_Definition(SCR: "Proc_PlayerIsDeadWithOption", FR: "MortDuJoueurAvecOption")] PlayerIsDeadWithOption,
		//********************************************************************************************************************************************************************************************************************************************************
		// Misc procedures with two or more parameters 
		[AI_Definition(SCR: "Proc_ChangeMapAtPosition", FR: "ChangeMapALaPosition")] ChangeMapAtPosition,
		[AI_Definition(SCR: "Proc_PlayerIsDeadWithPlacement", EN: "ACT_PlayerIsDeadWithPlacement", FR: "MortDuJoueurAvecRepositionnement")] PlayerIsDeadWithPlacement,
		//********************************************************************************************************************************************************************************************************************************************************

		//********************************************************************************************************************************************************************************************************************************************************
		// Table procedures 
		[AI_Definition(SCR: "Proc_SwapLinkTableObjects", EN: "SwapLinkTableObjects", FR: "EchangeObjetsDansTable")] SwapLinkTableObjects,
		[AI_Definition(SCR: "Proc_ChangeCurrentObjectTable", EN: "ChangeCurrentObjectTable", FR: "ChangeTableObjetCourante")] ChangeCurrentObjectTable,
		[AI_Definition(SCR: "Proc_BuildObjectTableFromTableAndString", EN: "BuildObjectTableFromTableAndString", FR: "ModifieTableCouranteAvecTableEtTexte")] BuildObjectTableFromTableAndString,
		[AI_Definition(SCR: "Proc_BuildObjectTableFromFormattedString", EN: "BuildObjectTableFromFormattedString", FR: "ModifieTableCouranteAvecTexteFormate")] BuildObjectTableFromFormattedString,
		//********************************************************************************************************************************************************************************************************************************************************
		// LipsSynchro 
		[AI_Definition(SCR: "Proc_StartSpeech", EN: "StartSpeech", FR: "Parle")] StartSpeech,
		[AI_Definition(SCR: "Proc_StopSpeech", EN: "StopSpeech", FR: "ArreteDeParler")] StopSpeech,
		//********************************************************************************************************************************************************************************************************************************************************
		// Module Control 
		[AI_Definition(SCR: "Proc_TakeModuleControl", EN: "MOD_TakeModuleControl", FR: "ControlerModule")] TakeModuleControl,
		[AI_Definition(SCR: "Proc_TakeManyModulesControl", EN: "MOD_TakeManyModulesControl", FR: "ControlerPlusieursModules")] TakeManyModulesControl,
		[AI_Definition(SCR: "Proc_ReleaseModuleControl", EN: "MOD_ReleaseModuleControl", FR: "LibererModule")] ReleaseModuleControl,

		//********************************************************************************************************************************************************************************************************************************************************
		// Saved game 
		[AI_Definition(SCR: "Proc_SaveGame", EN: "GAME_SaveGame", FR: "SauvePartie")] SaveGame,
		[AI_Definition(SCR: "Proc_LoadGame", EN: "GAME_LoadGame", FR: "ChargePartie")] LoadGame,
		[AI_Definition(SCR: "Proc_EraseGame", EN: "GAME_EraseGame", FR: "EffacePartie")] EraseGame,
		[AI_Definition(SCR: "Proc_CopyGame", EN: "GAME_CopyGame", FR: "CopiePartie")] CopyGame,
		[AI_Definition(SCR: "Proc_QuitGame", EN: "GAME_QuitGame", FR: "TerminePartie")] QuitGame,
		//********************************************************************************************************************************************************************************************************************************************************
		// Channel activation : ULTRA 
		[AI_Definition(SCR: "Proc_ActivateChannel", EN: "ActivateChannel", FR: "ActiveCanal")] ActivateChannel,
		[AI_Definition(SCR: "Proc_DeactivateChannel", EN: "DeactivateChannel", FR: "DesactiveCanal")] DeactivateChannel,
		//********************************************************************************************************************************************************************************************************************************************************
		// Light 
		[AI_Definition(SCR: "Proc_PersoLightOn", EN: "ACT_PersoLightOn", FR: "AllumeLumierePerso")] PersoLightOn,
		[AI_Definition(SCR: "Proc_PersoLightOff", EN: "ACT_PersoLightOff", FR: "EteintLumierePerso")] PersoLightOff,
		[AI_Definition(SCR: "Proc_SetPersoLightColor", EN: "ACT_SetPersoLightColor", FR: "FixeCouleurLumierePerso")] SetPersoLightColor,
		[AI_Definition(SCR: "Proc_SetPersoLightNearFar", EN: "ACT_SetPersoLightNearFar", FR: "FixeDistancesLumierePerso")] SetPersoLightNearFar,
		[AI_Definition(SCR: "Proc_SetPersoLightLittleBigAlpha", EN: "ACT_SetPersoLightLittleBigAlpha", FR: "FixeAlphasLumierePerso")] SetPersoLightLittleBigAlpha,
		[AI_Definition(SCR: "Proc_SetPersoLightGyrophare", EN: "ACT_SetPersoLightGyrophare", FR: "FixeGyrophareLumierePerso")] SetPersoLightGyrophare,
		[AI_Definition(SCR: "Proc_SetPersoLightPulse", EN: "ACT_SetPersoLightPulse", FR: "FixePulseLumierePerso")] SetPersoLightPulse,
		[AI_Definition(SCR: "Proc_SetPersoLightParalleleType", EN: "ACT_SetPersoLightParalleleType", FR: "FixeTypeParalleleLumierePerso")] SetPersoLightParalleleType,
		[AI_Definition(SCR: "Proc_SetPersoLightSphericalType", EN: "ACT_SetPersoLightSphericalType", FR: "FixeTypeSpheriqueLumierePerso")] SetPersoLightSphericalType,
		[AI_Definition(SCR: "Proc_SetPersoLightHotSpotType", EN: "ACT_SetPersoLightHotSpotType", FR: "FixeTypeHotSpotLumierePerso")] SetPersoLightHotSpotType,
		[AI_Definition(SCR: "Proc_SetPersoLightAmbientType", EN: "ACT_SetPersoLightAmbientType", FR: "FixeTypeAmbientLumierePerso")] SetPersoLightAmbientType,
		//********************************************************************************************************************************************************************************************************************************************************
		// Sound 
		[AI_Definition(SCR: "Proc_SendSoundRequest", EN: "SOUND_SendSoundRequest", FR: "EnvoieRequeteSon")] SendSoundRequest,
		[AI_Definition(SCR: "Proc_SendVoiceRequest", EN: "SOUND_SendVoiceRequest", FR: "EnvoieRequeteVoix")] SendVoiceRequest,
		[AI_Definition(SCR: "Proc_SendMusicRequest", EN: "SOUND_SendMusicRequest", FR: "EnvoieRequeteMusique")] SendMusicRequest,
		[AI_Definition(SCR: "Proc_SendAmbianceRequest", EN: "SOUND_SendAmbianceRequest", FR: "EnvoieRequeteAmbiance")] SendAmbianceRequest,
		[AI_Definition(SCR: "Proc_SendMenuSndRequest", EN: "SOUND_SendMenuSndRequest", FR: "EnvoieRequeteMenuSon")] SendMenuSndRequest,
		//********************************************************************************************************************************************************************************************************************************************************
		// display 
		[AI_Definition(SCR: "Proc_DefautDisplay", EN: "DefautDisplay", FR: "AfficheDefaut")] DefaultDisplay,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_DisplayVignette", EN: "VIG_DisplayVignette", FR: "VIG_AfficheVignette")] DisplayVignette,
		[AI_Definition(SCR: "Proc_DeleteVignette", EN: "VIG_DeleteVignette", FR: "VIG_SupprimeVignette")] DeleteVignette,
		//********************************************************************************************************************************************************************************************************************************************************
		// ParticleGenerator 
		// diverse
		[AI_Definition(SCR: "Proc_SetParticleGeneratorOn", EN: "PRT_SetParticleGeneratorOn", FR: "PRT_DeclencheGenerateur")] PRT_SetGeneratorOn,
		[AI_Definition(SCR: "Proc_SetParticleGeneratorOff", EN: "PRT_SetParticleGeneratorOff", FR: "PRT_ArreteGenerateur")] PRT_SetGeneratorOff,
		[AI_Definition(SCR: "Proc_SetParticleGenerator", EN: "PRT_SetParticleGenerator", FR: "PRT_InstalleGenerateur")] PRT_SetGenerator,
		// Generation mode
		[AI_Definition(SCR: "Proc_SetGenerationModeNone", EN: "PRT_SetGenerationModeNone", FR: "PRT_PasDeGeneration")] PRT_SetModeNone,
		[AI_Definition(SCR: "Proc_SetGenerationModeContinuous", EN: "PRT_SetGenerationModeContinuous", FR: "PRT_GenerationContinue")] PRT_SetModeContinuous,
		[AI_Definition(SCR: "Proc_SetGenerationModeCrenel", EN: "PRT_SetGenerationModeCrenel", FR: "PRT_GenerationEnCreneau")] PRT_SetModeCrenel,
		[AI_Definition(SCR: "Proc_SetGenerationModeProbability", EN: "PRT_SetGenerationModeProbability", FR: "PRT_GenerationProbabiliste")] PRT_SetModeProbability,
		// Generation number
		[AI_Definition(SCR: "Proc_SetGenerationNbConstant", EN: "PRT_SetGenerationNbConstant", FR: "PRT_GenereNombreConstant")] PRT_SetNumberConstant,
		[AI_Definition(SCR: "Proc_SetGenerationNbProbabilist", EN: "PRT_SetGenerationNbProbabilist", FR: "PRT_GenereNombreProbabiliste")] PRT_SetNumberProbabilist,
		// Particles LifeTime
		[AI_Definition(SCR: "Proc_SetInfiniteLifeTime", EN: "PRT_SetInfiniteLifeTime", FR: "PRT_TempsDeVieInfini")] PRT_SetInfiniteLifeTime,
		[AI_Definition(SCR: "Proc_SetConstantLifeTime", EN: "PRT_SetConstantLifeTime", FR: "PRT_TempsDeVieConstant")] PRT_SetConstantLifeTime,
		[AI_Definition(SCR: "Proc_SetProbabilistLifeTime", EN: "PRT_SetProbabilistLifeTime", FR: "PRT_TempsDeVieProbabiliste")] PRT_SetPobabilistLifeTime,
		
		//********************************************************************************************************************************************************************************************************************************************************

		//********************************************************************************************************************************************************************************************************************************************************
		// Old metactions
		[AI_Definition(SCR: "Proc_TurnAbsoluteDirection", FR: "TourneDirectionAbsolue")] TurnAbsoluteDirection,
		[AI_Definition(SCR: "Proc_GoAbsoluteDirection", FR: "VaToutDroitDirectionAbsolue")] GoAbsoluteDirection,
		[AI_Definition(SCR: "Proc_Turn", EN: "ACT_Turn", FR: "ACT_TourneVitesse")] Turn,
		[AI_Definition(SCR: "Proc_Turn2", EN: "ACT_Turn2", FR: "ACT_TourneAngle")] Turn2,
		[AI_Definition(SCR: "Proc_DeltaTurnPerso", EN: "ACT_DeltaTurnPerso", FR: "OrienteVersPersoAvecDelta")] DeltaTurnPerso,
		[AI_Definition(SCR: "Proc_TurnPerso", EN: "ACT_TurnPerso", FR: "OrienteVersPerso")] TurnPerso,
		[AI_Definition(SCR: "Proc_KillPerso", EN: "ACT_KillPerso", FR: "DetruitPerso")] KillPerso,
		[AI_Definition(SCR: "Proc_KillPersoAndClearVariable", EN: "ACT_KillPersoAndClearVariable", FR: "DetruitPersoEtEffaceVariable")] KillPersoAndClearVariable,


		//***********************************************************************************************************************************
		// Visual GMT procedure
		[AI_Definition(SCR: "Proc_SetVisualGMTColor", EN: "GMT_SetVisualGMTColor", FR: "FixeVisuelGMTCouleur")] SetVisualGMTColor,
		[AI_Definition(SCR: "Proc_SetVisualGMTSpecularExponant", EN: "GMT_SetVisualGMTSpecularExponant", FR: "FixeVisuelGMTSpeculaireExposant")] SetVisualGMTSpecularExponent,
		[AI_Definition(SCR: "Proc_SetVisualGMTSpecularCoef", EN: "GMT_SetVisualGMTSpecularCoef", FR: "FixeVisuelGMTSpeculaireCoef")] SetVisualGMTSpecularCoef,
		[AI_Definition(SCR: "Proc_SetVisualGMTDiffuseCoef", EN: "GMT_SetVisualGMTDiffuseCoef", FR: "FixeVisuelGMTDiffusionCoef")] SetVisualGMTDiffuseCoef,
		[AI_Definition(SCR: "Proc_SetVisualGMTAmbientCoef", EN: "GMT_SetVisualGMTAmbientCoef", FR: "FixeVisuelGMTAmbientCoef")] SetVisualGMTAmbientCoef,
		[AI_Definition(SCR: "Proc_SetVisualGMTAsChromed", EN: "GMT_SetVisualGMTAsChromed", FR: "FixeVisuelGMTChrome")] SetVisualGMTAsChromed,
		[AI_Definition(SCR: "Proc_SetVisualGMTTextureScrollingCoef", EN: "GMT_SetVisualGMTTextureScrollingCoef", FR: "FixeVisuelGMTTextureScrollingCoef")] SetVisualGMTTextureScrollingCoef,
		[AI_Definition(SCR: "Proc_LockVisualGMT", EN: "GMT_LockVisualGMT", FR: "BloqueVisuelGMT")] LockVisualGMT,
		[AI_Definition(SCR: "Proc_UnlockVisualGMT", EN: "GMT_UnlockVisualGMT", FR: "DebloqueVisuelGMT")] UnlockVisualGMT,
		[AI_Definition(SCR: "Proc_SetVisualGMTFrame", EN: "GMT_SetVisualGMTFrame", FR: "FixeNoImageVisuelGMT")] SetVisualGMTFrame,

		//***********************************************************************************************************************************
		//KWN's New AI Function
		[AI_Definition(SCR: "Func_SetScreenDSActif", EN: "ACT_SetScreenDSActif", FR: "ACT_SetScreenDSActif")] SetScreenDSActif,
		//***********************************************************************************************************************************

		//MICRO
		[AI_Definition(SCR: "Func_MICRO_Enable", EN: "MICRO_Enable", FR: "MICRO_Enable")] MICRO_Enable,

		#endregion

		#region DefProc1
		//********************************************************************************************************************************************************************************************************************************************************
		// Option procedures 
		[AI_Definition(SCR: "Proc_OptionChangeDetailsValue", EN: "OPTION_ChangeDetailsValue", FR: "Option_ChangeNiveauDeDetails")] OptionChangeDetailsValue,
		[AI_Definition(SCR: "Proc_OptionChangeMusicVolume", EN: "OPTION_ChangeMusicVolume", FR: "Option_ChangeVolumeMusical")] OptionChangeMusicVolume,
		[AI_Definition(SCR: "Proc_OptionChangeSfxVolume", EN: "OPTION_ChangeSfxVolume", FR: "Option_ChangeVolumeEffets")] OptionChangeSfxVolume,
		[AI_Definition(SCR: "Proc_OptionChangeVoiceVolume", EN: "OPTION_ChangeVoiceVolume", FR: "Option_ChangeVolumeVoix")] OptionChangeVoiceVolume,
		[AI_Definition(SCR: "Proc_OptionChangeActionKey", EN: "OPTION_ChangeActionKey", FR: "Option_ChangeConfigDeTouche")] OptionChangeActionKey,
		[AI_Definition(SCR: "Proc_ConfigureKey", EN: "OPTION_ConfigureKey", FR: "Option_ConfigureTouche")] OptionConfigureKey,
		//********************************************************************************************************************************************************************************************************************************************************
		// 3D procedures 
		[AI_Definition(SCR: "Proc_SetPersoAbsolutePosition", EN: "ACT_SetPersoAbsolutePosition", FR: "ACT_ChangePositionAbsoluePerso")] SetPersoAbsolutePosition,
		[AI_Definition(SCR: "Proc_SetPersoAtModulePosition", EN: "ACT_SetPersoAtModulePosition", FR: "ACT_PlaceSurModuleDePerso")] SetPersoAtModulePosition,
		[AI_Definition(SCR: "Proc_ForcePersoAveragePosition", EN: "ACT_ForcePersoAveragePosition", FR: "ACT_ForcePositionMoyennePerso")] ForcePersoAveragePosition,
		[AI_Definition(SCR: "Proc_RelativeMovePerso", EN: "ACT_RelativeMovePerso", FR: "ACT_DeplacePerso")] RelativeMovePerso,
		[AI_Definition(SCR: "Proc_ChangePersoAnySighting", EN: "ACT_SetPersoAnySighting", FR: "ACT_ChangeViseeQuelconquePerso")] ChangePersoAnySighting,
		[AI_Definition(SCR: "Proc_ChangePersoSightingWithOffset", EN: "ACT_SetPersoSightingWithOffset", FR: "ACT_ChangeViseePersoAvecOffset")] ChangePersoSightingWithOffset,
		[AI_Definition(SCR: "Proc_RotatePersoAroundX", EN: "ACT_RotatePersoAroundX", FR: "ACT_TournePersoAutourDeX")] RotatePersoAroundX,
		[AI_Definition(SCR: "Proc_RotatePersoAroundY", EN: "ACT_RotatePersoAroundY", FR: "ACT_TournePersoAutourDeY")] RotatePersoAroundY,
		[AI_Definition(SCR: "Proc_RotatePersoAroundZ", EN: "ACT_RotatePersoAroundZ", FR: "ACT_TournePersoAutourDeZ")] RotatePersoAroundZ,
		[AI_Definition(SCR: "Proc_RotatePersoAroundVector", EN: "ACT_RotatePersoAroundVector", FR: "ACT_TournePersoAutourDeVecteur")] RotatePersoAroundVector,
		[AI_Definition(SCR: "Proc_RotatePersoAroundVectorWithOffset", EN: "ACT_RotatePersoAroundVectorOffset", FR: "ACT_TournePersoAutourDeVecteurAvecOffset")] RotatePersoAroundVectorWithOffset,
		[AI_Definition(SCR: "Proc_RotatePersoAroundXwithOffset", EN: "ACT_RotatePersoAroundXwithOffset", FR: "ACT_TournePersoAutourDeXavecOffset")] RotatePersoAroundXwithOffset,
		[AI_Definition(SCR: "Proc_RotatePersoAroundYwithOffset", EN: "ACT_RotatePersoAroundYwithOffset", FR: "ACT_TournePersoAutourDeYavecOffset")] RotatePersoAroundYwithOffset,
		[AI_Definition(SCR: "Proc_RotatePersoAroundZwithOffset", EN: "ACT_RotatePersoAroundZwithOffset", FR: "ACT_TournePersoAutourDeZavecOffset")] RotatePersoAroundZwithOffset,
		[AI_Definition(SCR: "Proc_SetFullPersoOrientation", EN: "ACT_SetFullPersoOrientation", FR: "ACT_ReorientePerso")] SetFullPersoOrientation,
		[AI_Definition(SCR: "Proc_SetFullPersoOrientationWithOffset", EN: "ACT_SetFullPersoOrientWithOffset", FR: "ACT_ReorientePersoAvecOffset")] SetFullPersoOrientationWithOffset,

		[AI_Definition(SCR: "Proc_ZoomPerso", EN: "ACT_ZoomPerso", FR: "ACT_DeformePerso")] ZoomPerso,
		[AI_Definition(SCR: "Proc_ChangeOneCustomBit", EN: "ACT_ChangeOneCustomBit", FR: "ACT_ChangeCustomBit")] ChangeOneCustomBit,
		[AI_Definition(SCR: "Proc_ChangeManyCustomBits", EN: "ACT_ChangeManyCustomBits", FR: "ACT_ChangeCustomBits")] ChangeManyCustomBits,
		// impose relative 
		[AI_Definition(SCR: "Proc_ImposeSpeed", EN: "ACT_ImposeSpeed", FR: "ACT_ImposeVecteurVitesse")] ImposeSpeed,
		[AI_Definition(SCR: "Proc_ImposeSpeedX", EN: "ACT_ImposeSpeedX", FR: "ACT_ImposeVitesseX")] ImposeSpeedX,
		[AI_Definition(SCR: "Proc_ImposeSpeedY", EN: "ACT_ImposeSpeedY", FR: "ACT_ImposeVitesseY")] ImposeSpeedY,
		[AI_Definition(SCR: "Proc_ImposeSpeedZ", EN: "ACT_ImposeSpeedZ", FR: "ACT_ImposeVitesseZ")] ImposeSpeedZ,
		[AI_Definition(SCR: "Proc_ImposeSpeedXY", EN: "ACT_ImposeSpeedXY", FR: "ACT_ImposeVitesseXY")] ImposeSpeedXY,
		[AI_Definition(SCR: "Proc_ImposeSpeedXYZ", EN: "ACT_ImposeSpeedXYZ", FR: "ACT_ImposeVitesseXYZ")] ImposeSpeedXYZ,
		// impose absolute 
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeed", EN: "ACT_ImposeAbsoluteSpeed", FR: "ACT_ImposeVecteurVitesseAbsolu")] ImposeAbsoluteSpeed,
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeedX", EN: "ACT_ImposeAbsoluteSpeedX", FR: "ACT_ImposeVitesseAbsoluX")] ImposeAbsoluteSpeedX,
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeedY", EN: "ACT_ImposeAbsoluteSpeedY", FR: "ACT_ImposeVitesseAbsoluY")] ImposeAbsoluteSpeedY,
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeedZ", EN: "ACT_ImposeAbsoluteSpeedZ", FR: "ACT_ImposeVitesseAbsoluZ")] ImposeAbsoluteSpeedZ,
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeedXY", EN: "ACT_ImposeAbsoluteSpeedXY", FR: "ACT_ImposeVitesseAbsoluXY")] ImposeAbsoluteSpeedXY,
		[AI_Definition(SCR: "Proc_ImposeAbsoluteSpeedXYZ", EN: "ACT_ImposeAbsoluteSpeedXYZ", FR: "ACT_ImposeVitesseAbsoluXYZ")] ImposeAbsoluteSpeedXYZ,
		// propose relative 
		[AI_Definition(SCR: "Proc_ProposeSpeed", EN: "ACT_ProposeSpeed", FR: "ACT_ProposeVecteurVitesse")] ProposeSpeed,
		[AI_Definition(SCR: "Proc_ProposeSpeedX", EN: "ACT_ProposeSpeedX", FR: "ACT_ProposeVitesseX")] ProposeSpeedX,
		[AI_Definition(SCR: "Proc_ProposeSpeedY", EN: "ACT_ProposeSpeedY", FR: "ACT_ProposeVitesseY")] ProposeSpeedY,
		[AI_Definition(SCR: "Proc_ProposeSpeedZ", EN: "ACT_ProposeSpeedZ", FR: "ACT_ProposeVitesseZ")] ProposeSpeedZ,
		[AI_Definition(SCR: "Proc_ProposeSpeedXY", EN: "ACT_ProposeSpeedXY", FR: "ACT_ProposeVitesseXY")] ProposeSpeedXY,
		[AI_Definition(SCR: "Proc_ProposeSpeedXYZ", EN: "ACT_ProposeSpeedXYZ", FR: "ACT_ProposeVitesseXYZ")] ProposeSpeedXYZ,
		// propose absolute 
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeed", EN: "ACT_ProposeAbsoluteSpeed", FR: "ACT_ProposeVecteurVitesseAbsolu")] ProposeAbsoluteSpeed,
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeedX", EN: "ACT_ProposeAbsoluteSpeedX", FR: "ACT_ProposeVitesseAbsoluX")] ProposeAbsoluteSpeedX,
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeedY", EN: "ACT_ProposeAbsoluteSpeedY", FR: "ACT_ProposeVitesseAbsoluY")] ProposeAbsoluteSpeedY,
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeedZ", EN: "ACT_ProposeAbsoluteSpeedZ", FR: "ACT_ProposeVitesseAbsoluZ")] ProposeAbsoluteSpeedZ,
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeedXY", EN: "ACT_ProposeAbsoluteSpeedXY", FR: "ACT_ProposeVitesseAbsoluXY")] ProposeAbsoluteSpeedXY,
		[AI_Definition(SCR: "Proc_ProposeAbsoluteSpeedXYZ", EN: "ACT_ProposeAbsoluteSpeedXYZ", FR: "ACT_ProposeVitesseAbsoluXYZ")] ProposeAbsoluteSpeedXYZ,
		[AI_Definition(SCR: "Proc_FixePositionPerso", EN: "ACT_FixePositionPerso", FR: "ACT_FixePositionPerso")] FixePositionPerso,
		// add speed relative 
		[AI_Definition(SCR: "Proc_AddSpeed", EN: "ACT_AddSpeed", FR: "ACT_AddVecteurVitesse")] AddSpeed,
		[AI_Definition(SCR: "Proc_AddSpeedX", EN: "ACT_AddSpeedX", FR: "ACT_AddVitesseX")] AddSpeedX,
		[AI_Definition(SCR: "Proc_AddSpeedY", EN: "ACT_AddSpeedY", FR: "ACT_AddVitesseY")] AddSpeedY,
		[AI_Definition(SCR: "Proc_AddSpeedZ", EN: "ACT_AddSpeedZ", FR: "ACT_AddVitesseZ")] AddSpeedZ,
		[AI_Definition(SCR: "Proc_AddSpeedXY", EN: "ACT_AddSpeedXY", FR: "ACT_AddVitesseXY")] AddSpeedXY,
		[AI_Definition(SCR: "Proc_AddSpeedXYZ", EN: "ACT_AddSpeedXYZ", FR: "ACT_AddVitesseXYZ")] AddSpeedXYZ,
		// propose absolute 
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeed", EN: "ACT_AddAbsoluteSpeed", FR: "ACT_AddVecteurVitesseAbsolu")] AddAbsoluteSpeed,
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeedX", EN: "ACT_AddAbsoluteSpeedX", FR: "ACT_AddVitesseAbsoluX")] AddAbsoluteSpeedX,
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeedY", EN: "ACT_AddAbsoluteSpeedY", FR: "ACT_AddVitesseAbsoluY")] AddAbsoluteSpeedY,
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeedZ", EN: "ACT_AddAbsoluteSpeedZ", FR: "ACT_AddVitesseAbsoluZ")] AddAbsoluteSpeedZ,
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeedXY", EN: "ACT_AddAbsoluteSpeedXY", FR: "ACT_AddVitesseAbsoluXY")] AddAbsoluteSpeedXY,
		[AI_Definition(SCR: "Proc_AddAbsoluteSpeedXYZ", EN: "ACT_AddAbsoluteSpeedXYZ", FR: "ACT_AddVitesseAbsoluXYZ")] AddAbsoluteSpeedXYZ,
		//********************************************************************************************************************************************************************************************************************************************************
		// Morphing management procedures 
		[AI_Definition(SCR: "Proc_PrepareMorph", EN: "MOD_PrepareMorph", FR: "MOD_PrepareMorph")] PrepareMorph,
		[AI_Definition(SCR: "Proc_StopMorph", EN: "MOD_StopMorph", FR: "MOD_StopMorph")] StopMorph,
		[AI_Definition(SCR: "Proc_Morphing", EN: "MOD_Morphing", FR: "MOD_Morphing")] Morphing,
		[AI_Definition(SCR: "Proc_ReleaseAllModulesControl", EN: "MOD_ReleaseAllModulesControl", FR: "MOD_LibererTousLesModules")] ReleaseAllModulesControl,
		[AI_Definition(SCR: "Proc_ChangeModuleSighting", EN: "MOD_ChangeModuleSighting", FR: "MOD_ChangeViseeModule")] ChangeModuleSighting,
		[AI_Definition(SCR: "Proc_SetModuleAbsolutePosition", EN: "MOD_SetModuleAbsolutePosition", FR: "MOD_ChangePositionModule")] SetModuleAbsolutePosition,
		[AI_Definition(SCR: "Proc_RelativeMoveModule", EN: "MOD_RelativeMoveModule", FR: "MOD_DeplaceModule")] RelativeMoveModule,
		[AI_Definition(SCR: "Proc_ChangeModuleSightingWithOffset", EN: "MOD_ChangeModuleSightingWithOffset", FR: "MOD_ChangeViseeModuleAvecOffset")] ChangeModuleSightingWithOffset,
		[AI_Definition(SCR: "Proc_RotateModuleAroundX", EN: "MOD_RotateModuleAroundX", FR: "MOD_TourneModuleAutourDeX")] RotateModuleAroundX,
		[AI_Definition(SCR: "Proc_RotateModuleAroundY", EN: "MOD_RotateModuleAroundY", FR: "MOD_TourneModuleAutourDeY")] RotateModuleAroundY,
		[AI_Definition(SCR: "Proc_RotateModuleAroundZ", EN: "MOD_RotateModuleAroundZ", FR: "MOD_TourneModuleAutourDeZ")] RotateModuleAroundZ,
		[AI_Definition(SCR: "Proc_RotateModuleAroundVector", EN: "MOD_RotateModuleAroundVector", FR: "MOD_TourneModuleAutourDeVecteur")] RotateModuleAroundVector,
		[AI_Definition(SCR: "Proc_RotateModuleAroundVectorOffset", EN: "MOD_RotateModuleAroundVectorOffset", FR: "MOD_TourneModuleAutourDeVecteurAvecOffset")] RotateModuleAroundVectorWithOffset,
		[AI_Definition(SCR: "Proc_RotateModuleAroundXwithOffset", EN: "MOD_RotateModuleAroundXwithOffset", FR: "MOD_TourneModuleAutourDeXavecOffset")] RotateModuleAroundXwithOffset,
		[AI_Definition(SCR: "Proc_RotateModuleAroundYwithOffset", EN: "MOD_RotateModuleAroundYwithOffset", FR: "MOD_TourneModuleAutourDeYavecOffset")] RotateModuleAroundYwithOffset,
		[AI_Definition(SCR: "Proc_RotateModuleAroundZwithOffset", EN: "MOD_RotateModuleAroundZwithOffset", FR: "MOD_TourneModuleAutourDeZavecOffset")] RotateModuleAroundZwithOffset,
		[AI_Definition(SCR: "Proc_SetFullModuleOrientation", EN: "MOD_SetFullModuleOrientation", FR: "MOD_ReorienteModule")] SetFullModuleOrientation,
		[AI_Definition(SCR: "Proc_SetFullModuleOrientationOffset", EN: "MOD_SetFullModuleOrientationOffset", FR: "MOD_ReorienteModuleAvecOffset")] SetFullModuleOrientationWithOffset,
		[AI_Definition(SCR: "Proc_ZoomModule", EN: "MOD_ZoomModule", FR: "MOD_DeformeModule")] ZoomModule,
		//********************************************************************************************************************************************************************************************************************************************************
		// collision communication management : ULTRA 
		[AI_Definition(SCR: "Proc_SetColliderType", EN: "COL_SetColliderType", FR: "COL_ChangeTypeDeCollisionneur")] SetColliderType,
		[AI_Definition(SCR: "Proc_SetColliderVector", EN: "COL_SetColliderVector", FR: "COL_ChangeVecteurDeCollisionneur")] SetColliderVector,
		[AI_Definition(SCR: "Proc_SetColliderReal", EN: "COL_SetColliderReal", FR: "COL_ChangeReelDeCollisionneur")] SetColliderReal,
		[AI_Definition(SCR: "Proc_ResetLastCollisionActor", EN: "COL_ResetLastCollisionActor", FR: "COL_AnnuleDernierPersoCollisionne")] ResetLastCollisionActor,
		[AI_Definition(SCR: "Proc_ClearCollisionReport", EN: "COL_ClearCollisionReport", FR: "COL_VideRapportDeCollision")] ClearCollisionReport,
		[AI_Definition(SCR: "Proc_SetGoThroughMechanicsHandling", EN: "COL_SetGoThroughMechanicsHandling", FR: "COL_TraiteCollisionTraversee")] SetGoThroughMechanicsHandling,
		[AI_Definition(SCR: "Proc_EraseLastGoThroughMaterial", EN: "COL_EraseLastGoThroughMaterial", FR: "COL_EffaceDernierMateriauTraverse")] EraseLastGoThroughMaterial,
		//********************************************************************************************************************************************************************************************************************************************************
		// String procedures 
		[AI_Definition(SCR: "Proc_StringAddChar", EN: "TEXT_AddChar", FR: "TEXT_AjouteCaractere")] StringAddChar,
		[AI_Definition(SCR: "Proc_StringReplaceChar", EN: "TEXT_ReplaceChar", FR: "TEXT_RemplaceCaractere")] StringReplaceChar,
		[AI_Definition(SCR: "Proc_StringRemoveChar", EN: "TEXT_RemoveChar", FR: "TEXT_RetireCaractere")] StringRemoveChar,
		[AI_Definition(SCR: "Proc_ChangeLanguage", EN: "TEXT_ChangeLanguage", FR: "TEXT_ChangeLangueCourante")] ChangeLanguage,
		[AI_Definition(SCR: "Proc_IntToText", EN: "TEXT_IntToText", FR: "TEXT_EntierEnTexte")] IntToText,


		//********************************************************************************************************************************************************************************************************************************************************
		// Meca 
		[AI_Definition(SCR: "Proc_SetMechanicAnimation", EN: "MEC_SetMechanicAnimation", FR: "MEC_ActiveAnimation")] SetMechanicAnimation,
		[AI_Definition(SCR: "Proc_SetMechanicCollide", EN: "MEC_SetMechanicCollide", FR: "MEC_ActiveCollision")] SetMechanicCollide,
		[AI_Definition(SCR: "Proc_SetMechanicGravity", EN: "MEC_SetMechanicGravity", FR: "MEC_ActiveGravite")] SetMechanicGravity,
		[AI_Definition(SCR: "Proc_SetMechanicTilt", EN: "MEC_SetMechanicTilt", FR: "MEC_ActiveTilt")] SetMechanicTilt,
		[AI_Definition(SCR: "Proc_SetMechanicGI", EN: "MEC_SetMechanicGI", FR: "MEC_ActiveGi")] SetMechanicGi,
		[AI_Definition(SCR: "Proc_SetMechanicClimb", EN: "MEC_SetMechanicClimb", FR: "MEC_ActiveVarappe")] SetMechanicClimb,
		[AI_Definition(SCR: "Proc_SetMechanicOnGround", EN: "MEC_SetMechanicOnGround", FR: "MEC_ActivePlaqueAuSol")] SetMechanicOnGround,
		[AI_Definition(SCR: "Proc_SetMechanicSpider", EN: "MEC_SetMechanicSpider", FR: "MEC_ActiveAraignee")] SetMechanicSpider,
		[AI_Definition(SCR: "Proc_SetMechanicShoot", EN: "MEC_SetMechanicShoot", FR: "MEC_ActiveShoot")] SetMechanicShoot,
		[AI_Definition(SCR: "Proc_SetMechanicSwim", EN: "MEC_SetMechanicSwim", FR: "MEC_ActiveNage")] SetMechanicSwim,
		[AI_Definition(SCR: "Proc_SetMechanicNeverFall", EN: "MEC_SetMechanicNeverFall", FR: "MEC_ActiveNeTombePas")] SetMechanicNeverFall,
		[AI_Definition(SCR: "Proc_ResetMechanicNewMechanic", EN: "MEC_ResetMechanicNewMechanic", FR: "MEC_DesactiveNouvelleMecanique")] SetMechanicNewMechanic,
		[AI_Definition(SCR: "Proc_SetMechanicCollisionControl", EN: "MEC_SetMechanicCollisionControl", FR: "MEC_ControleCollision")] SetMechanicCollisionControl,
		[AI_Definition(SCR: "Proc_SetMechanicKeepSpeedZ", EN: "MEC_SetMechanicKeepSpeedZ", FR: "MEC_ConserveVitesseZ")] SetMechanicKeepSpeedZ,
		[AI_Definition(SCR: "Proc_SetMechanicSpeedLimit", EN: "MEC_SetMechanicSpeedLimit", FR: "MEC_ActiveLimiteVitesse")] SetMechanicSpeedLimit,
		[AI_Definition(SCR: "Proc_SetMechanicInertia", EN: "MEC_SetMechanicInertia", FR: "MEC_ActiveInertie")] SetMechanicInertia,
		[AI_Definition(SCR: "Proc_SetMechanicStream", EN: "MEC_SetMechanicStream", FR: "MEC_ActiveFlux")] SetMechanicStream,
		[AI_Definition(SCR: "Proc_SetMechanicStickOnPlatform", EN: "MEC_SetMechanicStickOnPlatform", FR: "MEC_ActiveCollerAuxPlateformes")] SetMechanicStickOnPlatform,
		[AI_Definition(SCR: "Proc_SetMechanicScale", EN: "MEC_SetMechanicScale", FR: "MEC_ActiveScale")] SetMechanicScale,
		[AI_Definition(SCR: "Proc_SetMechanicGravityFactor", EN: "MEC_SetMechanicGravityFactor", FR: "MEC_FixeGravite")] SetMechanicGravityFactor,
		[AI_Definition(SCR: "Proc_SetMechanicSlide", EN: "MEC_SetMechanicSlide", FR: "MEC_FixeGlissement")] SetMechanicSlide,
		[AI_Definition(SCR: "Proc_SetMechanicMaxRebound", EN: "MEC_SetMechanicMaxRebound", FR: "MEC_FixeRebond")] SetMechanicRebound,
		[AI_Definition(SCR: "Proc_SetMechanicSlopeLimit", EN: "MEC_SetMechanicSlopeLimit", FR: "MEC_FixeLimitePente")] SetMechanicSlopeLimit,
		[AI_Definition(SCR: "Proc_SetMechanicInertiaX", EN: "MEC_SetMechanicInertiaX", FR: "MEC_FixeInertieX")] SetMechanicInertiaX,
		[AI_Definition(SCR: "Proc_SetMechanicInertiaY", EN: "MEC_SetMechanicInertiaY", FR: "MEC_FixeInertieY")] SetMechanicInertiaY,
		[AI_Definition(SCR: "Proc_SetMechanicInertiaZ", EN: "MEC_SetMechanicInertiaZ", FR: "MEC_FixeInertieZ")] SetMechanicInertiaZ,
		[AI_Definition(SCR: "Proc_SetMechanicInertiaXYZ", EN: "MEC_SetMechanicInertiaXYZ", FR: "MEC_FixeInertie")] SetMechanicInertiaXYZ,
		[AI_Definition(SCR: "Proc_SetMechanicTiltIntensity", EN: "MEC_SetMechanicTiltIntensity", FR: "MEC_FixeIntensiteTilt")] SetMechanicTiltIntensity,
		[AI_Definition(SCR: "Proc_SetMechanicTiltInertia", EN: "MEC_SetMechanicTiltInertia", FR: "MEC_FixeInertieTilt")] SetMechanicTiltInertia,
		[AI_Definition(SCR: "Proc_SetMechanicTiltOrigin", EN: "MEC_SetMechanicTiltOrigin", FR: "MEC_FixeOrigineTilt")] SetMechanicTiltOrigin,
		[AI_Definition(SCR: "Proc_SetMechanicSpeedMax", EN: "MEC_SetMechanicSpeedMax", FR: "MEC_FixeVitesseMax")] SetMechanicSpeedMax,
		[AI_Definition(SCR: "Proc_SetMechanicStreamPriority", EN: "MEC_SetMechanicStreamPriority", FR: "MEC_FixePrioriteFlux")] SetMechanicStreamPriority,
		[AI_Definition(SCR: "Proc_SetMechanicStreamSpeed", EN: "MEC_SetMechanicStreamSpeed", FR: "MEC_FixeVitesseFlux")] SetMechanicStreamSpeed,
		[AI_Definition(SCR: "Proc_SetMechanicStreamFactor", EN: "MEC_SetMechanicStreamFactor", FR: "MEC_FixeFacteurDeFlux")] SetMechanicStreamFactor,
		[AI_Definition(SCR: "Proc_AddMechanicStreamSpeed", EN: "MEC_AddMechanicStreamSpeed", FR: "MEC_AjouteVitesseFlux")] AddMechanicStreamSpeed,
		[AI_Definition(SCR: "Proc_AddMechanicStreamSpeedList", EN: "MEC_AddMechanicStreamSpeedList", FR: "MEC_AjouteVitesseFluxListe")] AddMechanicStreamSpeedList,
		// limit 
		[AI_Definition(SCR: "Proc_MoveLimit", EN: "MEC_MoveLimit", FR: "MEC_LimiteDeplacement")] MoveLimit,
		[AI_Definition(SCR: "Proc_MoveLimitX", EN: "MEC_MoveLimitX", FR: "MEC_LimiteDeplacementX")] MoveLimitX,
		[AI_Definition(SCR: "Proc_MoveLimitY", EN: "MEC_MoveLimitY", FR: "MEC_LimiteDeplacementY")] MoveLimitY,
		[AI_Definition(SCR: "Proc_MoveLimitZ", EN: "MEC_MoveLimitZ", FR: "MEC_LimiteDeplacementZ")] MoveLimitZ,
		[AI_Definition(SCR: "Proc_MoveLimitXYZ", EN: "MEC_MoveLimitXYZ", FR: "MEC_LimiteDeplacementXYZ")] MoveLimitXYZ,
		// stop limit 
		[AI_Definition(SCR: "Proc_StopMoveLimitX", EN: "MEC_StopMoveLimitX", FR: "MEC_StopLimiteDeplacementX")] StopMoveLimitX,
		[AI_Definition(SCR: "Proc_StopMoveLimitY", EN: "MEC_StopMoveLimitY", FR: "MEC_StopLimiteDeplacementY")] StopMoveLimitY,
		[AI_Definition(SCR: "Proc_StopMoveLimitZ", EN: "MEC_StopMoveLimitZ", FR: "MEC_StopLimiteDeplacementZ")] StopMoveLimitZ,
		[AI_Definition(SCR: "Proc_StopMoveLimitXYZ", EN: "MEC_StopMoveLimitXYZ", FR: "MEC_StopLimiteDeplacementXYZ")] StopMoveLimitXYZ,
		// platform 
		[AI_Definition(SCR: "Proc_SetPlatformLink", EN: "MEC_SetPlatformLink", FR: "MEC_ForceLienPlateforme")] SetPlatformLink,
		[AI_Definition(SCR: "Proc_FreePlatformLink", EN: "MEC_FreePlatformLink", FR: "MEC_LibereLienPlateforme")] FreePlatformLink,
		// Scale 
		[AI_Definition(SCR: "Proc_SetScale", EN: "MEC_SetScale", FR: "MEC_FixeScale")] MecSetScale,
		// Slide coeff 
		[AI_Definition(SCR: "Proc_SetSlideFactorXYZ", EN: "MEC_SetSlideFactorXYZ", FR: "MEC_FixeCoefDeGlisseXYZ")] MecSetSlideFactorXYZ,
		[AI_Definition(SCR: "Proc_SetSlideFactorX", EN: "MEC_SetSlideFactorX", FR: "MEC_FixeCoefDeGlisseX")] MecSetSlideFactorX,
		[AI_Definition(SCR: "Proc_SetSlideFactorY", EN: "MEC_SetSlideFactorY", FR: "MEC_FixeCoefDeGlisseY")] MecSetSlideFactorY,
		[AI_Definition(SCR: "Proc_SetSlideFactorZ", EN: "MEC_SetSlideFactorZ", FR: "MEC_FixeCoefDeGlisseZ")] MecSetSlideFactorZ,
		[AI_Definition(SCR: "Proc_SetClimbSpeedLimit", EN: "MEC_SetClimbSpeedLimit", FR: "MEC_FixeLimiteVitesseVarappe")] MecSetClimbSpeedLimit,

		[AI_Definition(SCR: "Proc_SetHangingLimit", EN: "MEC_SetHangingLimit", FR: "MEC_ActiveMecaAccrochage")] MecSetHangingLimit,
		[AI_Definition(SCR: "Proc_SetHangingOff", EN: "MEC_SetHangingOff", FR: "MEC_DesactiveMecaAccrochage")] MecSetHangingOff,


		//********************************************************************************************************************************************************************************************************************************************************
		// Hierarchy - Platform 
		[AI_Definition(SCR: "Proc_HierSetFather", EN: "HIER_SetFather", FR: "HIER_ForcePere")] HierSetFather,
		[AI_Definition(SCR: "Proc_HierFreeFather", EN: "HIER_FreeFather", FR: "HIER_LiberePere")] HierFreeFather,
		[AI_Definition(SCR: "Proc_HierListOfSon", EN: "HIER_ListOfSon", FR: "HIER_MesFils")] HierListOfSon,
		[AI_Definition(SCR: "Proc_HierSetPlatformType", EN: "HIER_SetPlatformType", FR: "HIER_ForceTypePlateforme")] HierSetPlatformType,
		[AI_Definition(SCR: "Proc_HierLinkControl", EN: "HIER_LinkControl", FR: "HIER_ControlePlatforme")] HierControlLink,
		[AI_Definition(SCR: "Proc_HierFreezeEngine", EN: "HIER_FreezeEngine", FR: "HIER_GeleMoteur")] HierFreezeEngine,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_FixePositionZDM", EN: "ZON_SetPositionZDM", FR: "ZON_FixePositionZDM")] FixePositionZDM,
		[AI_Definition(SCR: "Proc_FixePositionZDD", EN: "ZON_SetPositionZDD", FR: "ZON_FixePositionZDD")] FixePositionZDD,
		[AI_Definition(SCR: "Proc_FixePositionZDE", EN: "ZON_SetPositionZDE", FR: "ZON_FixePositionZDE")] FixePositionZDE,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ChangeLightIntensity", EN: "LIGHT_ChangeLightIntensity", FR: "LUM_ChangerIntensiteLumineuse")] ChangeLightIntensity,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SPO_ChangeFlag", EN: "SPO_ChangeFlag", FR: "SPO_ChangeFlag")] SPO_ChangeFlag,
		[AI_Definition(SCR: "ACT_ChangeSpoFlag", EN: "ACT_ChangeSpoFlag", FR: "ACT_ChangeSpoFlag")] ActChangeSpoFlag,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_EcranChangeTaille", EN: "SCREEN_ChangeSize", FR: "Ecran_ChangeTaille")] ChangeScreenSize,
		[AI_Definition(SCR: "Proc_EcranChangeClip", EN: "SCREEN_ChangeClip", FR: "Ecran_ChangeClip")] ChangeScreenClip,
		[AI_Definition(SCR: "Proc_EcranChangePos", EN: "SCREEN_ChangePos", FR: "Ecran_ChangePos")] ChangeScreenPos,
		[AI_Definition(SCR: "Proc_EcranChangePosPercent", EN: "SCREEN_ChangePosPercent", FR: "Ecran_ChangePosPercent")] ChangeScreenPosPercent,
		[AI_Definition(SCR: "Proc_EcranChangeRatio", EN: "SCREEN_ChangeProportion", FR: "Ecran_ChangeRatio")] ChangeScreenRatio,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_OMBREAffichage", EN: "SHADOW_Display", FR: "OMBRE_Affichage")] DisplayShadow,
		[AI_Definition(SCR: "Proc_OMBREChangeHauteur", EN: "SHADOW_ChangeHeight", FR: "OMBRE_ChangeHauteur")] ChangeShadowHeight,
		[AI_Definition(SCR: "Proc_OMBREChangeVecteurProjection", EN: "SHADOW_ChangeVectorProjection", FR: "OMBRE_ChangeVecteurProjection")] ChangeShadowVector,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ForceActivationZDD", EN: "ZON_ForceActivationZDD", FR: "ZON_ForceActivationZDD")] ForceActivationZDD,
		[AI_Definition(SCR: "Proc_ForceActivationZDM", EN: "ZON_ForceActivationZDM", FR: "ZON_ForceActivationZDM")] ForceActivationZDM,
		[AI_Definition(SCR: "Proc_ForceActivationZDE", EN: "ZON_ForceActivationZDE", FR: "ZON_ForceActivationZDE")] ForceActivationZDE,
		[AI_Definition(SCR: "Proc_ForceActivationZDR", EN: "ZON_ForceActivationZDR", FR: "ZON_ForceActivationZDR")] ForceActivationZDR,
		[AI_Definition(SCR: "Proc_ForceDesactivationZDD", EN: "ZON_ForceDesactivationZDD", FR: "ZON_ForceDesactivationZDD")] ForceDesactivationZDD,
		[AI_Definition(SCR: "Proc_ForceDesactivationZDM", EN: "ZON_ForceDesactivationZDM", FR: "ZON_ForceDesactivationZDM")] ForceDesactivationZDM,
		[AI_Definition(SCR: "Proc_ForceDesactivationZDE", EN: "ZON_ForceDesactivationZDE", FR: "ZON_ForceDesactivationZDE")] ForceDesactivationZDE,
		[AI_Definition(SCR: "Proc_ForceDesactivationZDR", EN: "ZON_ForceDesactivationZDR", FR: "ZON_ForceDesactivationZDR")] ForceDesactivationZDR,
		[AI_Definition(SCR: "Proc_LibereActivationZDD", EN: "ZON_ReleaseActivationZDD", FR: "ZON_LibereActivationZDD")] LibereActivationZDD,
		[AI_Definition(SCR: "Proc_LibereActivationZDM", EN: "ZON_ReleaseActivationZDM", FR: "ZON_LibereActivationZDM")] LibereActivationZDM,
		[AI_Definition(SCR: "Proc_LibereActivationZDE", EN: "ZON_ReleaseActivationZDE", FR: "ZON_LibereActivationZDE")] LibereActivationZDE,
		[AI_Definition(SCR: "Proc_LibereActivationZDR", EN: "ZON_ReleaseActivationZDR", FR: "ZON_LibereActivationZDR")] LibereActivationZDR,
		[AI_Definition(SCR: "Proc_ActiveZDR", EN: "ZON_ActiveZDR", FR: "ZON_ActiveZDR")] ActiveZDR,
		//********************************************************************************************************************************************************************************************************************************************************
		// Collision Flags and Computing collisions frequency 
		[AI_Definition(SCR: "Proc_SetNoColWithMap", EN: "ACT_SetNoColWithMap", FR: "ACT_FixePasDeCollisionAvecMap")] SetNoCollisionWithMap,
		[AI_Definition(SCR: "Proc_SetNoColWithProjectile", EN: "ACT_SetNoColWithProjectile", FR: "ACT_FixePasDeCollisionAvecProjectiles")] SetNoCollisionWithProjectile,
		[AI_Definition(SCR: "Proc_SetNoColWithSeconfCharact", EN: "ACT_SetNoColWithSeconfCharact", FR: "ACT_FixePasDeCollisionAvecActeursSecondaires")] SetNoCollisionWithSecondCharact,
		[AI_Definition(SCR: "Proc_SetNoColWithMainCharact", EN: "ACT_SetNoColWithMainCharact", FR: "ACT_FixePasDeCollisionAvecActeurPrincipal")] SetNoCollisionWithMainCharact,
		[AI_Definition(SCR: "Proc_SetNoColWithOtherSectors", EN: "ACT_SetNoColWithOtherSectors", FR: "ACT_FixePasDeCollisionAvecAutresSecteurs")] SetNoCollisionWithOtherSectors,
		[AI_Definition(SCR: "Proc_SetNoColZdeWithProjectile", EN: "ACT_SetNoColZdeWithProjectile", FR: "ACT_FixePasDeCollisionZdeAvecProjectiles")] SetNoCollisionZdeWithProjectile,
		[AI_Definition(SCR: "Proc_SetCollComputeFreq", EN: "ACT_SetCollComputeFreq", FR: "ACT_FixeFrequenceCalculCollisions")] SetCollComputeFrequency,
		[AI_Definition(SCR: "Proc_SetBrainComputeFreq", EN: "ACT_SetBrainComputeFreq", FR: "ACT_FixeFrequenceCalculIA")] SetBrainComputeFrequency,
		[AI_Definition(SCR: "Proc_SetLightComputeFreq", EN: "ACT_SetLightComputeFreq", FR: "ACT_FixeFrequenceCalculLumieres")] SetLightComputeFrequency,
		//********************************************************************************************************************************************************************************************************************************************************
		// map exits management for world map update 
		[AI_Definition(SCR: "Proc_SetUsedExitIdentifier", EN: "MAP_SetUsedExitIdentifier", FR: "MAP_IndiqueLaSortieUtilisee")] SetUsedExitIdentifier,
		[AI_Definition(SCR: "Proc_GetUsedExitIdentifier", EN: "MAP_GetUsedExitIdentifier", FR: "MAP_LitLaSortieUtilisee")] GetUsedExitIdentifier,
		[AI_Definition(SCR: "eProc_SetGlobalCounter", EN: "MAP_SetGlobalCounter", FR: "MAP_ChangeCompteurGlobal")] SetGlobalCounter,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "PAD_ReadAnalogJoystickMarioMode", EN: "PAD_ReadAnalogJoystickMarioMode", FR: "PAD_LitJoyAnalogiqueModeMario")] ReadAnalogJoystick,
		[AI_Definition(SCR: "PAD_InitAnalogJoystickAxes", EN: "PAD_InitAnalogJoystickAxes", FR: "PAD_JoyAnalogiqueFixeAxes")] SetAnalogJoystickAxes,
		[AI_Definition(SCR: "Proc_SetPadReadingDsgvars", EN: "PAD_SetPadReadingDsgvars", FR: "PAD_FixeVariablesDeLecture")] SetPadReadingDsgvars,
		[AI_Definition(SCR: "Proc_CancelKeyboardInertia", EN: "PAD_CancelKeyboardInertia", FR: "PAD_AnnuleInertieClavier")] CancelKeyboardInertia,
		[AI_Definition(SCR: "Proc_GetPadCalibration", EN: "PAD_GetPadCalibration", FR: "PAD_LitCalibration")] GetPadCalibration,
		[AI_Definition(SCR: "Proc_SetPadCalibration", EN: "PAD_SetPadCalibration", FR: "PAD_ChangeCalibration")] SetPadCalibration,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "ACT_SetBooleanInArray", EN: "ACT_SetBooleanInArray", FR: "ACT_FixeBooleenDansTableau")] SetBooleanInArray,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "TEXT_SetDefaultFormatCharacter", EN: "TEXT_SetDefaultFormatCharacter", FR: "TEXT_SetDefaultFormatCharacter")] SetDefaultFormatCharacter,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "VID_SelectDriver", EN: "VID_SelectDriver", FR: "VID_SelectDriver")] SelectDriver,

		[AI_Definition(SCR: "Proc_SelectShapnessMax", EN: "VID_SelectShapnessMax", FR: "VID_SelectShapnessMax")] SelectShapnessMax,
		[AI_Definition(SCR: "Proc_CenterScreen", EN: "VID_CenterScreen", FR: "VID_CenterScreen")] CenterScreen,


		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "VID_SelectResolution", EN: "VID_SelectResolution", FR: "VID_SelectResolution")] SelectResolution,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "VID_ChangeBrightness", EN: "VID_ChangeBrightness", FR: "VID_ChangeBrightness")] ChangeBrightness,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "TEXT_IntegerParameter", EN: "TEXT_IntegerParameter", FR: "TEXT_ParametreEntier")] TexteEntier,
		[AI_Definition(SCR: "TEXT_RealParameter", EN: "TEXT_RealParameter", FR: "TEXT_ParametreReel")] TexteReel,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "VID_SetTextureFiltering", EN: "VID_SetTextureFiltering", FR: "VID_SetTextureFiltering")] SetTextureFiltering,
		[AI_Definition(SCR: "VID_SetAntiAliasing", EN: "VID_SetAntiAliasing", FR: "VID_SetAntiAliasing")] SetAntiAliasing,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "SOUND_SetInStereoMode", EN: "SOUND_SetInStereoMode", FR: "SOUND_SetInStereoMode")] SetInStereoMode,
		[AI_Definition(SCR: "Proc_SetSaturationBackGroundDistance", EN: "ACT_SetSaturationBackGroundDistance", FR: "ACT_FixeDistanceSaturationBackGround")] SetSaturationBackGroundDistance,
		[AI_Definition(SCR: "Proc_SaveCurrentRequest", EN: "SOUND_SaveCurrentRequest", FR: "SOUND_SauveRequeteCourante")] SaveSoundEventInSlotMemory,
		[AI_Definition(SCR: "Proc_RestoreRequest", EN: "SOUND_RestoreRequest", FR: "SOUND_RestaureRequete")] RestoreSoundEventInSlotMemory,

		[AI_Definition(SCR: "Proc_DrawSegment", EN: "DEBUG_DrawSegment", FR: "DEBUG_DessineSegment")] DrawSegment,
		[AI_Definition(SCR: "Proc_UpdateChecksum", EN: "ACT_UpdateChecksum", FR: "ACT_MetAJourChecksum")] UpdateChecksum,
		[AI_Definition(SCR: "Proc_ShellExecute", EN: "ShellExecute", FR: "ShellExecute")] ShellExecute,

		[AI_Definition(SCR: "Proc_ForceVerticalUsingX", EN: "ACT_ForceVerticalUsingX", FR: "ACT_ForceVerticalUsingX")] ForceVerticalUsingX,
		[AI_Definition(SCR: "Proc_ForceVerticalUsingY", EN: "ACT_ForceVerticalUsingY", FR: "ACT_ForceVerticalUsingY")] ForceVerticalUsingY,
		#endregion

		#region DefProCa
		[AI_Definition(SCR: "Proc_Cam_UpdatePosition", EN: "Cam_UpdatePosition", FR: "Cam_UpdatePosition")] Cam_UpdatePosition,
		[AI_Definition(SCR: "Proc_Cam_ChangeShiftTarget", EN: "Cam_ChangeShiftTarget", FR: "Cam_ChangeShiftTarget")] Cam_ChangeShiftTarget,
		[AI_Definition(SCR: "Proc_Cam_ChangeShiftPos", EN: "Cam_ChangeShiftPos", FR: "Cam_ChangeShiftPos")] Cam_ChangeShiftPos,
		[AI_Definition(SCR: "Proc_Cam_ChangeDistMin", EN: "Cam_ChangeDistMin", FR: "Cam_ChangeDistMin")] Cam_ChangeDistMin,
		[AI_Definition(SCR: "Proc_Cam_ChangeDistMax", EN: "Cam_ChangeDistMax", FR: "Cam_ChangeDistMax")] Cam_ChangeDistMax,
		[AI_Definition(SCR: "Proc_Cam_ChangeBoundDistMin", EN: "Cam_ChangeBoundDistMin", FR: "Cam_ChangeBoundDistMin")] Cam_ChangeBoundDistMin,
		[AI_Definition(SCR: "Proc_Cam_ChangeBoundDistMax", EN: "Cam_ChangeBoundDistMax", FR: "Cam_ChangeBoundDistMax")] Cam_ChangeBoundDistMax,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngleAlpha", EN: "Cam_ChangeAngleAlpha", FR: "Cam_ChangeAngleAlpha")] Cam_ChangeAngleAlpha,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngleShiftAlpha", EN: "Cam_ChangeAngleShiftAlpha", FR: "Cam_ChangeAngleShiftAlpha")] Cam_ChangeAngleShiftAlpha,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngleTheta", EN: "Cam_ChangeAngleTheta", FR: "Cam_ChangeAngleTheta")] Cam_ChangeAngleTheta,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngleShiftTheta", EN: "Cam_ChangeAngleShiftTheta", FR: "Cam_ChangeAngleShiftTheta")] Cam_ChangeAngleShiftTheta,
		[AI_Definition(SCR: "Proc_Cam_ChangeLinearSpeed", EN: "Cam_ChangeLinearSpeed", FR: "Cam_ChangeLinearSpeed")] Cam_ChangeLinearSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeLinearIncreaseSpeed", EN: "Cam_ChangeLinearIncreaseSpeed", FR: "Cam_ChangeLinearIncreaseSpeed")] Cam_ChangeLinearIncreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeLinearDecreaseSpeed", EN: "Cam_ChangeLinearDecreaseSpeed", FR: "Cam_ChangeLinearDecreaseSpeed")] Cam_ChangeLinearDecreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngularSpeed", EN: "Cam_ChangeAngularSpeed", FR: "Cam_ChangeAngularSpeed")] Cam_ChangeAngularSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngularIncreaseSpeed", EN: "Cam_ChangeAngularIncreaseSpeed", FR: "Cam_ChangeAngularIncreaseSpeed")] Cam_ChangeAngularIncreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeAngularDecreaseSpeed", EN: "Cam_ChangeAngularDecreaseSpeed", FR: "Cam_ChangeAngularDecreaseSpeed")] Cam_ChangeAngularDecreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeTargetSpeed", EN: "Cam_ChangeTargetSpeed", FR: "Cam_ChangeTargetSpeed")] Cam_ChangeTargetSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeTargetIncreaseSpeed", EN: "Cam_ChangeTargetIncreaseSpeed", FR: "Cam_ChangeTargetIncreaseSpeed")] Cam_ChangeTargetIncreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeTargetDecreaseSpeed", EN: "Cam_ChangeTargetDecreaseSpeed", FR: "Cam_ChangeTargetDecreaseSpeed")] Cam_ChangeTargetDecreaseSpeed,
		[AI_Definition(SCR: "Proc_Cam_ChangeFocal", EN: "Cam_ChangeFocal", FR: "Cam_ChangeFocal")] Cam_ChangeFocal,
		[AI_Definition(SCR: "Proc_Cam_ChangeZMin", EN: "Cam_ChangeZMin", FR: "Cam_ChangeZMin")] Cam_ChangeZMin,
		[AI_Definition(SCR: "Proc_Cam_ChangeZMax", EN: "Cam_ChangeZMax", FR: "Cam_ChangeZMax")] Cam_ChangeZMax,

		[AI_Definition(SCR: "Proc_Cam_ChangeTgtPerso", EN: "Cam_ChangeTgtPerso", FR: "Cam_ChangeTgtPerso")] Cam_ChangeTgtPerso,
		[AI_Definition(SCR: "Proc_Cam_ChangeSecondTgtPerso", EN: "Cam_ChangeSecondTgtPerso", FR: "Cam_ChangeSecondTgtPerso")] Cam_ChangeSecondTgtPerso,
		[AI_Definition(SCR: "Proc_Cam_ChangeChannel", EN: "Cam_ChangeChannel", FR: "Cam_ChangeChannel")] Cam_ChangeChannel,

		[AI_Definition(SCR: "Proc_Cam_Activate", EN: "Cam_Activate", FR: "Cam_Activate")] Cam_Activate,
		[AI_Definition(SCR: "Proc_Cam_AssociateViewport", EN: "Cam_AssociateViewport", FR: "Cam_AssociateViewport")] Cam_AssociateViewport,

		[AI_Definition(SCR: "Proc_Cam_ResetIAFlags", EN: "Cam_ResetIAFlags", FR: "Cam_ResetIAFlags")] Cam_ResetIAFlags,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoDynamicTarget", EN: "Cam_SetFlagNoDynamicTarget", FR: "Cam_SetFlagNoDynamicTarget")] Cam_SetFlagNoDynamicTarget,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoAverageMoveTgtPerso", EN: "Cam_SetFlagNoAverageMoveTgtPerso", FR: "Cam_SetFlagNoAverageMoveTgtPerso")] Cam_SetFlagNoAverageMoveTgtPerso,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoParseCutAngle", EN: "Cam_SetFlagNoParseCutAngle", FR: "Cam_SetFlagNoParseCutAngle")] Cam_SetFlagNoParseCutAngle,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoVisibility", EN: "Cam_SetFlagNoVisibility", FR: "Cam_SetFlagNoVisibility")] Cam_SetFlagNoVisibility,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoVisibilityWithDynHie", EN: "Cam_SetFlagNoVisibilityWithDynHie", FR: "Cam_SetFlagNoVisibilityWithDynHie")] Cam_SetFlagNoVisibilityWithDynHie,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoDynChangeTheta", EN: "Cam_SetFlagNoDynChangeTheta", FR: "Cam_SetFlagNoDynChangeTheta")] Cam_SetFlagNoDynChangeTheta,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoShiftUntilPosReached", EN: "Cam_SetFlagNoShiftUntilPosReached", FR: "Cam_SetFlagNoShiftUntilPosReached")] Cam_SetFlagNoShiftUntilPosReached,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoDynSpeed", EN: "Cam_SetFlagNoDynSpeed", FR: "Cam_SetFlagNoDynSpeed")] Cam_SetFlagNoDynSpeed,

		[AI_Definition(SCR: "Proc_Cam_ResetDNMFlags", EN: "Cam_ResetDNMFlags", FR: "Cam_ResetDNMFlags")] Cam_ResetDNMFlags,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoLinearParsing", EN: "Cam_SetFlagNoLinearParsing", FR: "Cam_SetFlagNoLinearParsing")] Cam_SetFlagNoLinearParsing,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoLinearInertia", EN: "Cam_SetFlagNoLinearInertia", FR: "Cam_SetFlagNoLinearInertia")] Cam_SetFlagNoLinearInertia,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoAngularParsing", EN: "Cam_SetFlagNoAngularParsing", FR: "Cam_SetFlagNoAngularParsing")] Cam_SetFlagNoAngularParsing,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoAngularInertia", EN: "Cam_SetFlagNoAngularInertia", FR: "Cam_SetFlagNoAngularInertia")] Cam_SetFlagNoAngularInertia,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoTargetParsing", EN: "Cam_SetFlagNoTargetParsing", FR: "Cam_SetFlagNoTargetParsing")] Cam_SetFlagNoTargetParsing,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoTargetInertia", EN: "Cam_SetFlagNoTargetInertia", FR: "Cam_SetFlagNoTargetInertia")] Cam_SetFlagNoTargetInertia,
		[AI_Definition(SCR: "Proc_Cam_SetFlagFixedOrientation", EN: "Cam_SetFlagFixedOrientation", FR: "Cam_SetFlagFixedOrientation")] Cam_SetFlagFixedOrientation,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoObstacle", EN: "Cam_SetFlagNoObstacle", FR: "Cam_SetFlagNoObstacle")] Cam_SetFlagNoObstacle,
		[AI_Definition(SCR: "Proc_Cam_SetFlagNoCollisionWhenNotMoving", EN: "Cam_SetFlagNoCollisionWhenNotMoving", FR: "Cam_SetFlagNoCollisionWhenNotMoving")] Cam_SetFlagNoCollisionWhenNotMoving,

		[AI_Definition(SCR: "Proc_Cam_ChangeConstants", EN: "Cam_ChangeConstants", FR: "Cam_ChangeConstants")] Cam_ChangeConstants,
		[AI_Definition(SCR: "Proc_Cam_SaveConstants", EN: "Cam_SaveConstants", FR: "Cam_SaveConstants")] Cam_SaveConstants,
		[AI_Definition(SCR: "Proc_Cam_RestoreConstants", EN: "Cam_RestoreConstants", FR: "Cam_RestoreConstants")] Cam_RestoreConstants,

		[AI_Definition(SCR: "Proc_Cam_ShowInfo", EN: "Cam_ShowInfo", FR: "Cam_ShowInfo")] Cam_ShowInfo,

		[AI_Definition(SCR: "Proc_Cam_ForceTarget", EN: "Cam_ForceTarget", FR: "Cam_ForceTarget")] Cam_ForceTarget,
		[AI_Definition(SCR: "Proc_Cam_ForcePosition", EN: "Cam_ForcePosition", FR: "Cam_ForcePosition")] Cam_ForcePosition,
		[AI_Definition(SCR: "Proc_Cam_ForceRefAxis", EN: "Cam_ForceRefAxis", FR: "Cam_ForceRefAxis")] Cam_ForceRefAxis,

		[AI_Definition(SCR: "Proc_Cam_Reset", EN: "Cam_Reset", FR: "Cam_Reset")] Cam_Reset,
		[AI_Definition(SCR: "Proc_Cam_ForceBestPos", EN: "Cam_ForceBestPos", FR: "Cam_ForceBestPos")] Cam_ForceBestPos,
		[AI_Definition(SCR: "Proc_Cam_ForceNormalState", EN: "Cam_ForceNormalState", FR: "Cam_ForceNormalState")] Cam_ForceNormalState,


		[AI_Definition(SCR: "Proc_Cam_ForceMovingOnRail", EN: "Cam_ForceMovingOnRail", FR: "Cam_ForceMovingOnRail")] Cam_ForceMovingOnRail,

		[AI_Definition(SCR: "Proc_Cam_SetCameraModeForEngine", EN: "Cam_SetCameraModeForEngine", FR: "Cam_ChangeModePourMoteur")] Cam_SetCameraModeForEngine,
		#endregion

		#region DefProc part 2
		/////////////////////////////////////////////////////////////////////////////////////////
		// Animations Graphiques Optimisées
		/////////////////////////////////////////////////////////////////////////////////////////
		// Generates a simple particle
		// Arg 1 = Particle type (int)
		// Arg 2 = Position
		// Arg 3 = Initial Direction
		// Arg 4 = Game Material
		// Arg 5 = Optional Real Parameter
		[AI_Definition(SCR: "Proc_SetAGO", EN: "SetAGO", FR: "LanceAGO")] LaunchAGO,
		//
		///////////////////////////////////////////////////////////////////////////////////////////

		[AI_Definition(SCR: "Proc_ActivateMenuMap", EN: "ActivateMenuMap", FR: "ActiveMenuMap")] SetInGameMenu,

		///////////////////////////////////////////////////////////////////////////////////////////
		// Texts without always                                                                  
		///////////////////////////////////////////////////////////////////////////////////////////

		[AI_Definition(SCR: "Proc_JffTxt_Affiche", EN: "JFFTXT_Affiche", FR: "JFFTXT_Affiche")] JffTxt_Affiche,
		[AI_Definition(SCR: "Proc_JffTxt_Extend", EN: "JFFTXT_Extend", FR: "JFFTXT_Extend")] JffTxt_Extend,
		[AI_Definition(SCR: "Proc_JffTxt_Modify", EN: "JFFTXT_Modify", FR: "JFFTXT_Modify")] JffTxt_Modify,

		[AI_Definition(SCR: "Proc_FadeIn", EN: "FADE_FadeIn", FR: "FadeIn")] FadeIn,
		[AI_Definition(SCR: "Proc_FadeOut", EN: "FADE_FadeOut", FR: "FadeOut")] FadeOut,

		[AI_Definition(SCR: "Proc_HUD_SetLumsCount", EN: "HUD_SetLumsCount", FR: "HUD_SetLumsCount")] HUD_SetLumsCount,
		[AI_Definition(SCR: "Proc_HUD_SetCagesCount", EN: "HUD_SetCagesCount", FR: "HUD_SetCagesCount")] HUD_SetCagesCount,
		[AI_Definition(SCR: "Proc_HUD_SetSwimGauge", EN: "HUD_SetSwimGauge", FR: "HUD_SetSwimGauge")] HUD_SetSwimGauge,
		[AI_Definition(SCR: "Proc_HUD_SetHealthGauge", EN: "HUD_SetHealthGauge", FR: "HUD_SetHealthGauge")] HUD_SetHealthGauge,
		[AI_Definition(SCR: "Proc_HUD_SetEnemyHealthGauge", EN: "HUD_SetEnemyHealthGauge", FR: "HUD_SetEnemyHealthGauge")] HUD_SetEnemyHealthGauge,

		[AI_Definition(SCR: "Proc_DoCheatMenu", EN: "PROC_DoCheatMenu", FR: "PROC_DoCheatMenu")] DoCheatMenu,
		[AI_Definition(SCR: "Proc_SetBacklight", EN: "PROC_SetBacklight", FR: "PROC_SetBacklight")] SetBacklight,

		[AI_Definition(SCR: "Proc_BeginAnalogCalibration", EN: "PROC_BeginAnalogCalibration", FR: "PROC_BeginAnalogCalibration")] BeginAnalogCalibration,
		[AI_Definition(SCR: "Proc_EndAnalogCalibration", EN: "PROC_EndAnalogCalibration", FR: "PROC_EndAnalogCalibration")] EndAnalogCalibration,
		#endregion

		#region ProcRay2
		//********************************************************************************************************************************************************************************************************************************************************
		// Magic points management 
		[AI_Definition(SCR: "Proc_FixePointsDeMagie", EN: "RAY_SetMagicPoints", FR: "RAY_FixePointsDeMagie")] FixePointsDeMagie,
		[AI_Definition(SCR: "Proc_FixePointsDeMagieMax", EN: "RAY_SetMagicPointsMax", FR: "RAY_FixePointsDeMagieMax")] FixePointsDeMagieMax,
		[AI_Definition(SCR: "Proc_InitPointsDeMagie", EN: "RAY_InitMagicPoints", FR: "RAY_InitPointsDeMagie")] InitPointsDeMagie,
		[AI_Definition(SCR: "Proc_InitPointsDeMagieMax", EN: "RAY_InitMagicPointsMax", FR: "RAY_InitPointsDeMagieMax")] InitPointsDeMagieMax,
		[AI_Definition(SCR: "Proc_AddMagicPoints", EN: "RAY_AddMagicPoints", FR: "RAY_AjoutePointsDeMagie")] AjoutePointsDeMagie,
		[AI_Definition(SCR: "Proc_AddMagicPointsMax", EN: "RAY_AddMagicPointsMax", FR: "RAY_AjoutePointsDeMagieMax")] AjoutePointsDeMagieMax,
		[AI_Definition(SCR: "Proc_SubMagicPoints", EN: "RAY_SubMagicPoints", FR: "RAY_EnlevePointsDeMagie")] EnlevePointsDeMagie,
		[AI_Definition(SCR: "Proc_SubMagicPointsMax", EN: "RAY_SubMagicPointsMax", FR: "RAY_EnlevePointsDeMagieMax")] EnlevePointsDeMagieMax,
		// Air points management 
		[AI_Definition(SCR: "Proc_FixePointsDair", EN: "RAY_SetAirPoints", FR: "RAY_FixePointsDair")] FixePointsDair,
		[AI_Definition(SCR: "Proc_FixePointsDairMax", EN: "RAY_SetAirPointsMax", FR: "RAY_FixePointsDairMax")] FixePointsDairMax,
		[AI_Definition(SCR: "Proc_InitPointsDair", EN: "RAY_InitAirPoints", FR: "RAY_InitPointsDair")] InitPointsDair,
		[AI_Definition(SCR: "Proc_InitPointsDairMax", EN: "RAY_InitAirPointsMax", FR: "RAY_InitPointsDairMax")] InitPointsDairMax,
		[AI_Definition(SCR: "Proc_AddAirPoints", EN: "RAY_AddAirPoints", FR: "RAY_AjoutePointsDair")] AjoutePointsDair,
		[AI_Definition(SCR: "Proc_AddAirPointsMax", EN: "RAY_AddAirPointsMax", FR: "RAY_AjoutePointsDairMax")] AjoutePointsDairMax,
		[AI_Definition(SCR: "Proc_SubAirPoints", EN: "RAY_SubAirPoints", FR: "RAY_EnlevePointsDair")] EnlevePointsDair,
		[AI_Definition(SCR: "Proc_SubAirPointsMax", EN: "RAY_SubAirPointsMax", FR: "RAY_EnlevePointsDairMax")] EnlevePointsDairMax,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_FixePositionFade", EN: "FAD_FixePositionFade", FR: "FAD_FixePositionFade")] FixePositionFade,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SetLocalPersoLight", EN: "LIGHT_SetLocalPersoLight", FR: "LUM_FixeLumierePersoLocal")] FixeLumierePersoLocal,
		[AI_Definition(SCR: "Proc_SetStaticLightOnOff", EN: "LIGHT_SetStaticLightOnOff", FR: "LUM_FixeLumiereStaticOnOff")] FixeLumiereStaticOnOff,
		[AI_Definition(SCR: "Proc_SetStaticLightNearFar", EN: "LIGHT_SetStaticLightNearFar", FR: "LUM_FixeLumiereStaticNearFar")] FixeLumiereStaticNearFar,
		[AI_Definition(SCR: "Proc_SetStaticLightColor", EN: "LIGHT_SetStaticLightColor", FR: "LUM_FixeLumiereStaticCouleur")] FixeLumiereStaticColor,
		[AI_Definition(SCR: "Proc_ComputeLightEffect", EN: "LIGHT_ComputeLightEffect", FR: "LUM_CalculeLumiereEffet")] CalculeLumiereEffet,
		// Blend RLI 
		[AI_Definition(SCR: "Proc_RLIPasDeRLI", EN: "LIGHT_RLIPasDeRLI", FR: "LUM_RLIDesactive")] NoRLI,
		[AI_Definition(SCR: "Proc_RLIFixe", EN: "LIGHT_RLIFixe", FR: "LUM_RLIFixe")] FixeRLI,
		[AI_Definition(SCR: "Proc_RLIBlend", EN: "LIGHT_RLIBlend", FR: "LUM_RLIBlend")] BlendRLI,
		//********************************************************************************************************************************************************************************************************************************************************
		// Type Of WP 
		[AI_Definition(SCR: "Proc_ChangeTypeOfWP", EN: "NETWORK_ChangeWPType", FR: "Reseau_ChangeTypeDuWP")] ChangeTypeOfWP,
		// Capability procedures 
		[AI_Definition(SCR: "Proc_CAPSSetCapabilities", EN: "CAPS_SetCapabilities", FR: "Caps_FixeCapacites")] SetCapabilities,
		[AI_Definition(SCR: "Proc_CAPSAddCapabilities", EN: "CAPS_AddCapabilities", FR: "Caps_AjouteCapacites")] AddCapabilities,
		[AI_Definition(SCR: "Proc_CAPSSubCapabilities", EN: "CAPS_SubCapabilities", FR: "Caps_EnleveCapacites")] SubCapabilities,
		[AI_Definition(SCR: "Proc_CAPSChangeCapabilities", EN: "CAPS_ChangeCapabilities", FR: "Caps_ChangeCapacites")] ChangeCapabilities,
		// Reinit Graph 
		[AI_Definition(SCR: "Proc_ReinitGraph", EN: "NETWORK_ReinitGraph", FR: "Reseau_ReinitGraph")] ReinitGraph,

		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SetScrollSpeed", EN: "MAT_SetScrollSpeed", FR: "MAT_FixeVitesseTexture")] FixeVitesseTexture,
		[AI_Definition(SCR: "Proc_SetScrollOnOff", EN: "MAT_SetScrollOnOff", FR: "MAT_ScrollingOnOff")] ScrollingOnOff,
		[AI_Definition(SCR: "Proc_SetTextureOffset", EN: "MAT_SetTextureOffset", FR: "MAT_FixeDecalageTexture")] FixeDecalageTexture,
		[AI_Definition(SCR: "Proc_SetScrollPause", EN: "MAT_SetScrollPause", FR: "MAT_ScrollingPause")] ScrollingPause,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ChangeCurrFrame", EN: "ANI_ChangeCurrFrame", FR: "ANI_ChangeFrameCourante")] ChangeCurrFrame,
		[AI_Definition(SCR: "Proc_ChangeRandomFrame", EN: "ANI_ChangeRandomFrame", FR: "ANI_ChangeFrameAleatoire")] ChangeRandomFrame,
		[AI_Definition(SCR: "Proc_FreezeAnim", EN: "ANI_FreezeAnim", FR: "ANI_GelerPerso")] FreezeAnim,
		[AI_Definition(SCR: "Proc_UnFreezeAnim", EN: "ANI_UnFreezeAnim", FR: "ANI_DegelerPerso")] UnFreezeAnim,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SetSuperimposed", EN: "SPO_SetSuperimposed", FR: "SPO_SetSuperimposed")] SetSuperimposed,
		[AI_Definition(SCR: "Proc_SetSuperimposed2", EN: "SPO_SetSuperimposed2", FR: "SPO_SetSuperimposed2")] SetSuperimposed2,
		[AI_Definition(SCR: "Proc_ReleaseSuperimposed", EN: "SPO_ReleaseSuperimposed", FR: "SPO_ReleaseSuperimposed")] ReleaseSuperimposed,
		[AI_Definition(SCR: "Proc_SetCoordinates", EN: "SPO_SetCoordinates", FR: "SPO_FixeCoordonnees")] SetSPOCoordinates,
		[AI_Definition(SCR: "Proc_SwitchSuperimposedTab", EN: "SPO_SwitchSuperimposedTab", FR: "SPO_SwitchSuperimposedTab")] SwitchSuperimposedTab,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ACTsetTractionFactor", EN: "ACT_SetTractionFactor", FR: "ACT_FixeFacteurDeTraction")] SetTractionFactor,
		[AI_Definition(SCR: "Proc_TurnToPositionAngle", EN: "ACT_TurnToPositionAngle", FR: "ACT_OrienteVersPositionAngle")] TurnToPositionAngle,
		[AI_Definition(SCR: "Proc_TurnToPositionSpeed", EN: "ACT_TurnToPositionSpeed", FR: "ACT_OrienteVersPositionVitesse")] TurnToPositionSpeed,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_TABSortArray", EN: "ARRAY_Sort", FR: "TAB_Trie")] SortArray,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ActiveJoystickAnal", EN: "OPTION_ActiveJoystickAnal", FR: "Option_ActiveJoystickAnalogique")] ActiveJoystickAnal,
		[AI_Definition(SCR: "Proc_UnActiveJoystickAnal", EN: "OPTION_UnActiveJoystickAnal", FR: "Option_DesactiveJoystickAnalogique")] UnActiveJoystickAnal,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SetZDMSizeSphere", EN: "ZON_SetZDMSizeSphere", FR: "ZON_FixeTailleZDMSphere")] SetZDMSizeSphere,
		[AI_Definition(SCR: "Proc_SetZDDSizeCone", EN: "ZON_SetZDDSizeCone", FR: "ZON_FixeTailleZDDCone")] SetZDDSizeCone,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_SetVolumeAnim", EN: "SOUND_SetVolumeAnim", FR: "SON_FixeAnimVolume")] SetSoundVolumeAnim,
		[AI_Definition(SCR: "Proc_SetVolumeMusic", EN: "SOUND_SetVolumeMusic", FR: "SON_FixeMusicVolume")] SetSoundVolumeMusic,
		[AI_Definition(SCR: "Proc_SetVolumeAmbiance", EN: "SOUND_SetVolumeAmbiance", FR: "SON_FixeAmbianceVolume")] SetSoundVolumeAmbiance,
		[AI_Definition(SCR: "Proc_SetDopplerEffect", EN: "SOUND_SetDopplerEffect", FR: "SON_ChangeEffetDoppler")] SetDopplerEffect,
		[AI_Definition(SCR: "Proc_PauseSound", EN: "SOUND_PauseSound", FR: "SON_Pause")] PauseSound,
		[AI_Definition(SCR: "Proc_ResumeSound", EN: "SOUND_ResumeSound", FR: "SON_Reprendre")] ResumeSound,
		[AI_Definition(SCR: "Proc_StopMenuSound", EN: "SOUND_StopMenuSound", FR: "SON_ArretSonMenu")] StopMenuSound,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ChangeCaractere", EN: "TEXT_ChangeCharactere", FR: "TEXT_ChangeCaractere")] ChangeCaractere,
		[AI_Definition(SCR: "Proc_FormateTexte", EN: "TEXT_FormatText", FR: "TEXT_FormateTexte")] FormateTexte,
		[AI_Definition(SCR: "Proc_ValideNomDuSlot", EN: "MNU_ValidateSlotName", FR: "MNU_ValideNomDuSlot")] ValideNomDuSlot,
		[AI_Definition(SCR: "Proc_RestoreNomDuSlot", EN: "MNU_RestoreSlotName", FR: "MNU_RestoreNomDuSlot")] RestoreNomDuSlot,
		[AI_Definition(SCR: "Proc_ChangePersoSighting", EN: "ACT_ChangeActorSighting", FR: "ACT_ChangeViseePerso")] ChangePersoSighting,
		[AI_Definition(SCR: "Proc_PlayDemo", EN: "DEM_PlayDemo", FR: "DEM_JoueDemo")] PlayDemo,

		// Specific PC IA procs
		[AI_Definition(SCR: "PAD_InitKeyBoardDirections", EN: "PAD_InitKeyBoardDirections", FR: "PAD_InitKeyBoardDirections")] InitKeyBoardDirections,
		[AI_Definition(SCR: "PAD_SetCenterPosition", EN: "PAD_SetCenterPosition", FR: "PAD_SetCenterPosition")] PadSetCenterposition,
		[AI_Definition(SCR: "PAD_SetMaximalValues", EN: "PAD_SetMaximalValues", FR: "PAD_SetMaximalValues")] PadSetMaximalValues,
		// End Specific PC IA procs

		// ANNECY MT - 22/10/98 {
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Proc_ChangeTooFarLimit", EN: "ACT_ChangeTooFarLimit", FR: "ACT_FixeLimiteEloignement")] ChangeTooFarLimit,
		[AI_Definition(SCR: "Proc_ChangeTransparencyZone", EN: "ACT_ChangeTransparencyZone", FR: "ACT_FixeZoneTransparence")] ChangeTransparencyZone,
		// END ANNECY MT }

		[AI_Definition(SCR: "Proc_SetBaseFrequenceForMenu", EN: "Effect_SetBaseFrequenceForMenu", FR: "Effect_SetBaseFrequenceForMenu")] SetBaseFrequenceForMenu,
		[AI_Definition(SCR: "Proc_SaveSinusContext", EN: "Effect_SaveSinusContext", FR: "Effect_SaveSinusContext")] SaveSinusContext,
		[AI_Definition(SCR: "Proc_RestoreSinusContext", EN: "Effect_RestoreSinusContext", FR: "Effect_RestoreSinusContext")] RestoreSinusContext,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Func_TextSuperImposed", EN: "TEXT_SuperImposed", FR: "TEXT_SuperImposed")] TextSuperImposed,
		#endregion

	}
}