namespace BinarySerializer.Ubisoft.CPA {
	// Regex for NP++:
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	//                                       eCond                 , FR                   ,  EN                  , SCR
	// Replace: \[Definition\(SCR: \6, EN: \5, FR: \3\)] \2,
	// Then replace " EN: ," with ""
	public enum AI_Condition {
		Placeholder,

		#region DefCond
		//
		// 1. DefCond
		//
		///*******************************************************************************************************************************************************************************************************************************************************
		// Boolean conditions
		[AI_Definition(SCR: "Cond_And", EN: "And", FR: "Et")] Et,
		[AI_Definition(SCR: "Cond_Or", EN: "Or", FR: "Ou")] Ou,
		[AI_Definition(SCR: "Cond_Not", EN: "Not", FR: "Non")] Not,
		[AI_Definition(SCR: "Cond_XOr", EN: "XOr", FR: "OuExclusif")] XOr,
		///*******************************************************************************************************************************************************************************************************************************************************
		// Comparison conditions
		[AI_Definition(SCR: "Cond_Equal", EN: "=", FR: "=")] Equal,
		[AI_Definition(SCR: "Cond_Different", EN: "<>", FR: "<>")] Different,
		[AI_Definition(SCR: "Cond_Lesser", EN: "<", FR: "<")] Lesser,
		[AI_Definition(SCR: "Cond_Greater", EN: ">", FR: ">")] Greater,
		[AI_Definition(SCR: "Cond_LesserOrEqual", EN: "<=", FR: "<=")] LesserOrEqual,
		[AI_Definition(SCR: "Cond_GreaterOrEqual", EN: ">=", FR: ">=")] GreaterOrEqual,
		///*******************************************************************************************************************************************************************************************************************************************************
		// ZDD conditions
		[AI_Definition(SCR: "Cond_CollidePersoZDDWithPerso", EN: "COL_CollidePersoZDDWithPerso", FR: "Col_CollisionZDDPersoAvecPerso")] CollidePersoZDDNoWithPerso,
		[AI_Definition(SCR: "Cond_CollideModuleZDDWithPerso", EN: "COL_CollideModuleZDDWithPerso", FR: "Col_CollisionZDDModuleAvecPerso")] CollideModuleZDDNoWithPerso,
		[AI_Definition(SCR: "Cond_CollidePersoAllZDDWithPersoAllZDD", EN: "COL_CollidePersoAllZDDWithPersoAllZDD", FR: "Col_CollisionPersoTouteZDDAvecPerso")] CollidePersoAllZDDWithPerso,
		[AI_Definition(SCR: "Cond_CollidePersoZDDWithAnyPErso", EN: "COL_CollidePersoZDDWithAnyPErso", FR: "Col_CollisionZDDPersoAvecNimporteQuelPerso")] CollidePersoZDDWithAnyPerso,
		[AI_Definition(SCR: "Cond_CollideModuleZDDWithAnyPerso", EN: "COL_CollideModuleZDDWithAnyPerso", FR: "Col_CollisionZDDModuleAvecNimporteQuelPerso")] CollideModuleZDDWithAnyPerso,
		// ZDE conditions
		[AI_Definition(SCR: "Cond_CollidePersoZDEWithPersoZDE", EN: "COL_CollidePersoZDEWithPersoZDE", FR: "Col_CollisionZDEPersoAvecZDEPerso")] CollidePersoZDENoWithPersoZDENo,
		[AI_Definition(SCR: "Cond_CollideModuleZDEWithPersoZDE", EN: "COL_CollideModuleZDEWithPersoZDE", FR: "Col_CollisionZDEModuleAvecZDEPerso")] CollideModuleZDENoWithPersoZDENo,
		[AI_Definition(SCR: "Cond_CollidePersoZDEWithModuleZDE", EN: "COL_CollidePersoZDEWithModuleZDE", FR: "Col_CollisionZDEPersoAvecZDEModule")] CollidePersoZDENoWithModuleZDENo,
		[AI_Definition(SCR: "Cond_CollideModuleZDEWithModuleZDE", EN: "COL_CollideModuleZDEWithModuleZDE", FR: "Col_CollisionZDEModuleAvecZDEModule")] CollideModuleZDENoWithModuleZDENo,
		[AI_Definition(SCR: "Cond_CollidePersoZDEWithPersoTypeZDE", EN: "COL_CollidePersoZDEWithPersoTypeZDE", FR: "Col_CollisionZDEPersoAvecPersoTypeZDE")] CollidePersoZDENoWithPersoTypeZDE,
		[AI_Definition(SCR: "Cond_CollideModuleZDEWithPersoTypeZDE", EN: "COL_CollideModuleZDEWithPersoTypeZDE", FR: "Col_CollisionZDEModuleAvecPersoTypeZDE")] CollideModuleZDENoWithPersoTypeZDE,
		[AI_Definition(SCR: "Cond_CollidePersoTypeZDEWithPersoTypeZDE", EN: "COL_CollidePersoTypeZDEWithPersoTypeZDE", FR: "Col_CollisionPersoTypeZDEAvecPersoTypeZDE")] CollidePersoTypeZDEWithPersoTypeZDE,
		[AI_Definition(SCR: "Cond_CollidePersoAllZDEWithPersoAllZDE", EN: "COL_CollidePersoAllZDEWithPersoAllZDE", FR: "Col_CollisionPersoTouteZDEAvecPersoTouteZDE")] CollidePersoAllZDEWithPersoAllZDE,
		[AI_Definition(SCR: "Cond_CollidePersoTypeZDEWithPersoAllZDE", EN: "COL_CollidePersoTypeZDEWithPersoAllZDE", FR: "Col_CollisionPersoTypeZDEAvecPersoTouteZDE")] CollidePersoTypeZDEWithPersoAllZDE,
		[AI_Definition(SCR: "Cond_CollidePersoAllZDEWithPersoTypeZDE", EN: "COL_CollidePersoAllZDEWithPersoTypeZDE", FR: "Col_CollisionPersoTouteZDEAvecPersoTypeZDE")] CollidePersoAllZDEWithPersoTypeZDE,
		[AI_Definition(SCR: "Cond_CollidePersoZDENoWithTypeZDE", EN: "COL_CollidePersoZDENoWithTypeZDE", FR: "Col_CollisionPersoZDEAvecTypeZDE")] CollidePersoZDENoWithTypeZDE,
		[AI_Definition(SCR: "Cond_CollideModuleZDENoWithTypeZDE", EN: "COL_CollideModuleZDENoWithTypeZDE", FR: "Col_CollisionModuleZDEAvecTypeZDE")] CollideModuleZDENoWithTypeZDE,
		// ZDM Collision Test
		[AI_Definition(SCR: "Cond_CollideMovingZDM", FR: "Col_CollisionAvecZDMDeplacee")] CollideMovingZDM,
		// Collision conditions : ULTRA
		[AI_Definition(SCR: "Cond_CollideWithGround", EN: "COL_CollideWithGround", FR: "Col_CollisionAvecSol")] CollideWithGround,
		[AI_Definition(SCR: "Cond_CollideWithWall", EN: "COL_CollideWithWall", FR: "Col_CollisionAvecMur")] CollideWithWall,
		[AI_Definition(SCR: "Cond_CollideWithNothing", EN: "COL_CollideWithNothing", FR: "Col_PasDeCollision")] CollideWithNothing,
		[AI_Definition(SCR: "Cond_CollideWithCeiling", EN: "COL_CollideWithCeiling", FR: "Col_CollisionAvecPlafond")] CollideWithCeiling,
		[AI_Definition(SCR: "Cond_CollideWithPerso", EN: "COL_CollideWithPerso", FR: "Col_CollisionAvecPerso")] CollideWithPerso,
		[AI_Definition(SCR: "Cond_CollideWithWater", EN: "COL_CollideWithWater", FR: "Col_CollisionAvecEau")] CollideWithWater,

		[AI_Definition(SCR: "Cond_CollideWithThisPerso", EN: "COL_CollideWithThisPerso", FR: "Col_CollisionAvecCePerso")] CollideWithThisPerso,

		//ANNECY BBB
		[AI_Definition(SCR: "Cond_CollideWithTrap", FR: "Col_CollisionAvecTrappe")] CollideWithTrap,
		[AI_Definition(SCR: "Cond_CollideWithSlope", FR: "Col_CollisionAvecPente")] CollideWithSlope,
		[AI_Definition(SCR: "Cond_CollideWithAttic", FR: "Col_CollisionAvecMansarde")] CollideWithAttic,

		// ZDM Collision conditions : ULTRA
		[AI_Definition(SCR: "Cond_ZDMCollideWithGround", EN: "COL_ZDMCollideWithGround", FR: "Col_ZDMCollisionAvecSol")] ZDMCollideWithGround,
		[AI_Definition(SCR: "Cond_ZDMCollideWithWall", EN: "COL_ZDMCollideWithWall", FR: "Col_ZDMCollisionAvecMur")] ZDMCollideWithWall,
		[AI_Definition(SCR: "Cond_ZDMCollideWithNothing", EN: "COL_ZDMCollideWithNothing", FR: "Col_ZDMPasDeCollision")] ZDMCollideWithNothing,
		[AI_Definition(SCR: "Cond_ZDMCollideWithCeiling", EN: "COL_ZDMCollideWithCeiling", FR: "Col_ZDMCollisionAvecPlafond")] ZDMCollideWithCeiling,

		[AI_Definition(SCR: "Cond_ZDMCollideWithSlope", FR: "Col_ZDMCollisionAvecPente")] ZDMCollideWithSlope,
		[AI_Definition(SCR: "Cond_ZDMCollideWithAttic", FR: "Col_ZDMCollisionAvecMansarde")] ZDMCollideWithAttic,
		///*******************************************************************************************************************************************************************************************************************************************************
		// List conditions
		[AI_Definition(SCR: "Cond_IsPersoInList", EN: "LST_IsPersoInList", FR: "LST_PersoExisteDansListe")] IsPersoInList,
		[AI_Definition(SCR: "Cond_IsModelInList", EN: "LST_IsModelInList", FR: "LST_ModeleExisteDansListe")] IsModelInList,
		[AI_Definition(SCR: "Cond_IsFamilyInList", EN: "LST_IsFamilyInList", FR: "LST_FamilleExisteDansListe")] IsFamilyInList,
		[AI_Definition(SCR: "Cond_ListEmptyTest", EN: "LST_ListEmptyTest", FR: "LST_ListeVide")] ListEmptyTest,
		///*******************************************************************************************************************************************************************************************************************************************************
		// UserEvent
		[AI_Definition(SCR: "Cond_UserEvent_IsSet", EN: "UserEvent_IsSet", FR: "UserEvent_IsSet")] UserEvent_IsSet,
		[AI_Definition(SCR: "Cond_UserEvent_IsSet2", EN: "UserEvent_IsSet2", FR: "UserEvent_IsSet2")] UserEvent_IsSet2,
		///*******************************************************************************************************************************************************************************************************************************************************
		// NO FAMILY
		[AI_Definition(SCR: "Cond_PressedBut", EN: "BUT_PressedBut", FR: "BoutonAppuye")] PressedBut,
		[AI_Definition(SCR: "Cond_JustPressedBut", EN: "BUT_JustPressedBut", FR: "BoutonJusteAppuye")] JustPressedBut,
		[AI_Definition(SCR: "Cond_ReleasedBut", EN: "BUT_ReleasedBut", FR: "BoutonRelache")] ReleasedBut,
		[AI_Definition(SCR: "Cond_JustReleasedBut", EN: "BUT_JustReleasedBut", FR: "BoutonJusteRelache")] JustReleasedBut,

		///*******************************************************************************************************************************************************************************************************************************************************
		// Time conditions
		[AI_Definition(SCR: "Cond_IsTimeElapsed", EN: "TIME_IsTimeElapsed", FR: "TempsDepasse")] IsTimeElapsed,

		///*******************************************************************************************************************************************************************************************************************************************************
		// Validity condition
		[AI_Definition(SCR: "Cond_IsValidObject", EN: "SPO_IsValidObject", FR: "ObjetValide")] IsValidObject,
		[AI_Definition(SCR: "Cond_IsValidWayPoint", EN: "NET_IsValidWayPoint", FR: "Reseau_WayPointValide")] IsValidWayPoint,
		[AI_Definition(SCR: "Cond_IsValidGMT", EN: "GMT_IsValidGMT", FR: "GMTValide")] IsValidGMT,
		[AI_Definition(SCR: "Cond_IsValidAction", EN: "ACT_IsValidAction", FR: "ActionValide")] IsValidAction,
		[AI_Definition(SCR: "Cond_IsValidText", EN: "TEXT_IsValidText", FR: "TexteValide")] IsValidText,
		///*******************************************************************************************************************************************************************************************************************************************************
		// Lips_Synchro : ULTRA
		[AI_Definition(SCR: "Cond_IsSpeechOver", EN: "SPEEK_IsSpeechOver", FR: "DiscoursTermine")] IsSpeechOver,
		///*******************************************************************************************************************************************************************************************************************************************************
		// Sector : ULTRA for eCond_SeePerso
		[AI_Definition(SCR: "Cond_SeePerso", EN: "ACT_SeePerso", FR: "VoitPerso")] SeePerso,
		///*******************************************************************************************************************************************************************************************************************************************************
		// activation
		[AI_Definition(SCR: "Cond_IsActivable", EN: "ACT_IsActivable", FR: "EstActivable")] IsActivable,
		///*******************************************************************************************************************************************************************************************************************************************************
		// traitement
		[AI_Definition(SCR: "Cond_IsAlreadyHandled", EN: "ACT_IsAlreadyHandled", FR: "ACT_EstDejaTraite")] IsAlreadyHandled,
		///*******************************************************************************************************************************************************************************************************************************************************
		// Always condition
		[AI_Definition(SCR: "Cond_Alw_IsMain", EN: "ALW_IsMain", FR: "Alw_EstAMoi")] Alw_IsMine,
		///*******************************************************************************************************************************************************************************************************************************************************
		// Lights
		[AI_Definition(SCR: "Cond_IsPersoLightOn", EN: "LIGHT_IsPersoLightOn", FR: "LumierePersoAllumee")] IsPersoLightOn,
		[AI_Definition(SCR: "Cond_IsPersoLightPulseOn", EN: "LIGHT_IsPersoLightPulseOn", FR: "PulseLumierePersoAllumee")] IsPersoLightPulseOn,
		[AI_Definition(SCR: "Cond_IsPersoLightGyroPhareOn", EN: "LIGHT_IsPersoLightGyroPhareOn", FR: "GyroPhareLumierePersoAllumee")] IsPersoLightGyroPhareOn,

		///*******************************************************************************************************************************************************************************************************************************************************
		//RRR DS: New AI Functions
		[AI_Definition(SCR: "Cond_DS_StyletJustPressed", EN: "DS_StyletJustPressed", FR: "DS_StyletJustPressed")] StyletJustPressed,
		[AI_Definition(SCR: "Cond_DS_StyletPressed", EN: "DS_StyletPressed", FR: "DS_StyletPressed")] StyletPressed,
		[AI_Definition(SCR: "Cond_DS_StyletJustReleased", EN: "DS_StyletJustReleased", FR: "DS_StyletJustReleased")] StyletJustReleased,
		[AI_Definition(SCR: "Cond_DS_StyletSlice", EN: "DS_StyletSlice", FR: "DS_StyletSlice")] StyletSlice,
		[AI_Definition(SCR: "Cond_DS_CheckStyletRotation", EN: "CheckStyletRotation", FR: "DS_CheckStyletRotation")] StyletRotation,
		[AI_Definition(SCR: "Cond_DS_CheckStyletGratter", EN: "CheckStyletGratter", FR: "DS_CheckStyletGratter")] StyletGratter,
		#endregion

		#region DefCond1
		///*******************************************************************************************************************************************************************************************************************************************************
		//
		// 2. DefCond1
		//
		///*******************************************************************************************************************************************************************************************************************************************************
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_ZDMCollideWithObstacle", EN: "ZON_ZDMCollideWithObstacle", FR: "ZON_ZDMCollisionAvecObstacle")] IsZDMCollideWithObstacle,
		[AI_Definition(SCR: "Cond_IsZDMCollideWithWall", EN: "ZON_IsZDMCollideWithWall", FR: "ZON_ZDMContreMur")] IsZDMCollideWithWall,
		[AI_Definition(SCR: "Cond_IsZDMCollideWithGround", EN: "ZON_IsZDMCollideWithGround", FR: "ZON_ZDMContreSol")] IsZDMCollideWithGround,
		[AI_Definition(SCR: "Cond_IsZDMCollideWithCeiling", EN: "ZON_IsZDMCollideWithCeiling", FR: "ZON_ZDMContrePlafond")] IsZDMCollideWithCeiling,
		//********************************************************************************************************************************************************************************************************************************************************
		// collision communication management : NOT ULTRA 
		[AI_Definition(SCR: "Cond_CmtIdentifierContainsMask", EN: "COL_CmtIdentifierContainsMask", FR: "COL_TesteCmtContreMasque")] CmtIdentifierContainsMask,
		[AI_Definition(SCR: "Cond_HitByCollider", EN: "COL_HitByCollider", FR: "COL_CollisionneurTouche")] HitByCollider,
		[AI_Definition(SCR: "Cond_IsTypeOfGMTCollide", EN: "COL_IsTypeOfGMTCollide", FR: "TypeGMTCollision")] IsTypeOfGMTCollide,

		[AI_Definition(SCR: "Cond_IsInComport", EN: "ACT_IsInComport", FR: "EstDansComportement")] IsInComport,
		[AI_Definition(SCR: "Cond_IsInReflexComport", EN: "ACT_IsInReflexComport", FR: "EstDansComportementReflexe")] IsInReflexComport,
		[AI_Definition(SCR: "Cond_IsInAction", EN: "ACT_IsInAction", FR: "EstDansAction")] IsInAction,
		[AI_Definition(SCR: "Cond_ChangeActionEnable", EN: "ACT_ChangeActionEnable", FR: "ChangeActionPossible")] ChangeActionEnable,
		[AI_Definition(SCR: "Cond_EngineReinitRequested", EN: "ReinitMoteurDemande", FR: "ReinitMoteurDemande")] EngineReinitRequested,
		[AI_Definition(SCR: "Cond_IsThereMechEvent", EN: "MEC__IsThereMechEvent", FR: "EvenementMecanique")] IsThereMechEvent,
		[AI_Definition(SCR: "Cond_CollisionWP", EN: "COL_CollisionWP", FR: "COL_CollisionWP")] CollisionWP,
		//********************************************************************************************************************************************************************************************************************************************************
		// Acteurs 
		[AI_Definition(SCR: "Cond_IsCustomBitSet", EN: "ACT_IsCustomBitSet", FR: "ACT_TestCustomBit")] IsCustomBitSet,
		[AI_Definition(SCR: "Cond_IsPersoActive", EN: "ACT_IsPersoActive", FR: "ACT_EstActif")] IsPersoActive,
		[AI_Definition(SCR: "Cond_CheckActionEnd", EN: "ACT_CheckActionEnd", FR: "ACT_TestFinAction")] CheckActionEnd,
		[AI_Definition(SCR: "Cond_IsCurrentStateCustomBitSet", EN: "ACT_IsCurrentStateCustomBitSet", FR: "ACT_TestCustomBitDEtatCourant")] IsCurrentStateCustomBitSet,

		//********************************************************************************************************************************************************************************************************************************************************
		// Meca 
		[AI_Definition(SCR: "Cond_GiBlock", EN: "MEC_GiBlock", FR: "MEC_GiBloque")] IsGiBlock,
		[AI_Definition(SCR: "Cond_MechanicBlock", EN: "MEC_MechanicBlock", FR: "MEC_MecaniqueBloque")] IsMechanicBlock,
		[AI_Definition(SCR: "Cond_IsMechanicAnimatino", EN: "MEC_IsMechanicAnimatino", FR: "MEC_OptionAnimation")] IsMechanicAnimation,
		[AI_Definition(SCR: "Cond_IsMechanicCollide", EN: "MEC_IsMechanicCollide", FR: "MEC_OptionCollision")] IsMechanicCollide,
		[AI_Definition(SCR: "Cond_IsMechanicGravity", EN: "MEC_IsMechanicGravity", FR: "MEC_OptionGravite")] IsMechanicGravity,
		[AI_Definition(SCR: "Cond_IsMechanicTilt", EN: "MEC_IsMechanicTilt", FR: "MEC_OptionTilt")] IsMechanicTilt,
		[AI_Definition(SCR: "Cond_IsMechanicGi", EN: "MEC_IsMechanicGi", FR: "MEC_OptionGi")] IsMechanicGi,
		[AI_Definition(SCR: "Cond_IsMechanicClimb", EN: "MEC_IsMechanicClimb", FR: "MEC_OptionVarappe")] IsMechanicClimb,
		[AI_Definition(SCR: "Cond_IsMechanicOnGround", EN: "MEC_IsMechanicOnGround", FR: "MEC_OptionPlaqueAuSol")] IsMechanicOnGround,
		[AI_Definition(SCR: "Cond_IsMechanicSpider", EN: "MEC_IsMechanicSpider", FR: "MEC_OptionAraignee")] IsMechanicSpider,
		[AI_Definition(SCR: "Cond_IsMechanicShoot", EN: "MEC_IsMechanicShoot", FR: "MEC_OptionShoot")] IsMechanicShoot,
		[AI_Definition(SCR: "Cond_IsMechanicSwim", EN: "MEC_IsMechanicSwim", FR: "MEC_OptionNage")] IsMechanicSwim,
		[AI_Definition(SCR: "Cond_IsMechanicNeverFall", EN: "MEC_IsMechanicNeverFall", FR: "MEC_OptionNeTombePas")] IsMechanicNeverFall,
		[AI_Definition(SCR: "Cond_IsMechanicCollisionControl", EN: "MEC_IsMechanicCollisionControl", FR: "MEC_OptionControleCollision")] IsMechanicCollisionControl,
		[AI_Definition(SCR: "Cond_IsMechanicKeepSpeedZ", EN: "MEC_IsMechanicKeepSpeedZ", FR: "MEC_OptionConserveVitesseZ")] IsMechanicKeepSpeedZ,
		[AI_Definition(SCR: "Cond_IsMechanicSpeedLimit", EN: "MEC_IsMechanicSpeedLimit", FR: "MEC_OptionLimiteVitesse")] IsMechanicSpeedLimit,
		[AI_Definition(SCR: "Cond_IsMechanicInertia", EN: "MEC_IsMechanicInertia", FR: "MEC_OptionInertie")] IsMechanicInertia,
		[AI_Definition(SCR: "Cond_IsMechanicStream", EN: "MEC_IsMechanicStream", FR: "MEC_OptionFlux")] IsMechanicStream,
		[AI_Definition(SCR: "Cond_IsMechanicStickOnPlatform", EN: "MEC_IsMechanicStickOnPlatform", FR: "MEC_OptionCollerAuxPlateformes")] IsMechanicStickOnPlatform,
		[AI_Definition(SCR: "Cond_MechanicPatformCrash", EN: "MEC_MechanicPatformCrash", FR: "MEC_EcraseParPlateforme")] IsMechanicPatformCrash,
		[AI_Definition(SCR: "Cond_IsMechanicScale", EN: "MEC_IsMechanicScale", FR: "MEC_OptionScale")] IsMechanicScale,
		[AI_Definition(SCR: "Cond_IsMechanicExec", EN: "MEC_IsMechanicExec", FR: "MEC_Execution")] IsMechanicExec,
		[AI_Definition(SCR: "Cond_CanFall", EN: "MEC_CanFall", FR: "MEC_PeutTomber")] CanFall,
		[AI_Definition(SCR: "Cond_MainActorCrashed", EN: "MEC_IsMainActorCrashed", FR: "MEC_ActeurPrincipalEcrase")] IsMechanicCrash,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_NullVector", EN: "VEC_NullVector", FR: "VEC_VecteurNul")] IsNullVector,
		//********************************************************************************************************************************************************************************************************************************************************
		// Hierarchy - Platform 
		[AI_Definition(SCR: "Cond_HierIsSonOfActor", EN: "HIER_HierIsSonOfActor", FR: "HIER_EstFils_acteur")] HierIsSonOfActor,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_IsMorphing", EN: "MOD_IsMorphing", FR: "MOD_MorphingEnCours")] IsMorphing,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_CheckAnimEnd", EN: "ANI_CheckAnimEnd", FR: "ANI_TestFinAnim")] CheckAnimEnd,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_HasTheCapability", EN: "CAPS_HasTheCapability", FR: "Caps_ALaCapacite")] HasTheCapability,
		[AI_Definition(SCR: "Cond_HasOneOfTheCapabilities", EN: "CAPS_HasOneOfTheCapabilities", FR: "Caps_AUneDesCapacites")] HasOneOfTheCapabilities,
		[AI_Definition(SCR: "Cond_HasTheCapabilityNumber", EN: "CAPS_HasTheCapabilityNumber", FR: "Caps_ALaCapaciteNumero")] HasTheCapabilityNumber,
		[AI_Definition(SCR: "Cond_PersoHasTheCapability", EN: "CAPS_PersoHasTheCapability", FR: "Caps_PersoALaCapacite")] PersoHasTheCapability,
		[AI_Definition(SCR: "Cond_PersoHasOneOfTheCapabilities", EN: "CAPS_PersoHasOneOfTheCapabilities", FR: "Caps_PersoAUneDesCapacites")] PersoHasOneOfTheCapabilities,
		[AI_Definition(SCR: "Cond_PersoHasTheCapabilityNumber", EN: "CAPS_PersoHasTheCapabilityNumber", FR: "Caps_PersoALaCapaciteNumero")] PersoHasTheCapabilityNumber,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_IsMagnetActivated", EN: "MAGNET_IsMagnetActivated", FR: "Magnet_EstActif")] MagnetIsActivated,
		//********************************************************************************************************************************************************************************************************************************************************
		// Collision Flags 
		[AI_Definition(SCR: "Cond_IsNotInCollWithMap", EN: "ACT_IsNotInCollWithMap", FR: "ACT_NEstPasEnCollisionAvecMap")] NEstPasEnCollisionAvecMap,
		[AI_Definition(SCR: "Cond_IsNotInCollWithProj", EN: "ACT_IsNotInCollWithProj", FR: "ACT_NEstPasEnCollisionAvecProjectiles")] NEstPasEnCollisionAvecProjectile,
		[AI_Definition(SCR: "Cond_IsNotInColWithSecondCharact", EN: "ACT_IsNotInColWithSecondCharact", FR: "ACT_NEstPasEnCollisionAvecActeursSecondaires")] NEstPasEnCollisionAvecSecondCharact,
		[AI_Definition(SCR: "Cond_IsNotInColWithMainCharact", EN: "ACT_IsNotInColWithMainCharact", FR: "ACT_NEstPasEnCollisionAvecActeurPrincipal")] NEstPasEnCollisionAvecMainCharact,
		[AI_Definition(SCR: "Cond_IsNotInColWithOtherSectors", EN: "ACT_IsNotInColWithOtherSectors", FR: "ACT_NEstPasEnCollisionAvecAutresSecteurs")] NEstPasEnCollisionAvecAutresSecteurs,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_IsOfFamily", EN: "ACT_IsOfFamily", FR: "ACT_EstDeFamille")] IsInFamily,
		[AI_Definition(SCR: "Cond_IsOfModel", EN: "ACT_IsOfModel", FR: "ACT_EstDeModele")] IsInModel,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_AJoypadIsConnected", EN: "IPT_AJoypadIsConnected", FR: "IPT_AJoypadIsConnected")] AJoypadIsConnected,
		[AI_Definition(SCR: "Cond_AKeyJustPressed", EN: "IPT_AKeyJustPressed", FR: "IPT_AKeyJustPressed")] AKeyJustPressed,
		[AI_Definition(SCR: "Cond_AButtonPadJustPressed", EN: "IPT_AButtonPadJustPressed", FR: "IPT_AButtonPadJustPressed")] AButtonPadJustPressed,

		//********************************************************************************************************************************************************************************************************************************************************
		// demos 
		[AI_Definition(SCR: "Cond_IsInDemoMode", EN: "IsInDemoMode", FR: "EstDansDemoMode")] IsInDemoMode,

		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Cond_IsInStereoMode", EN: "SOUND_IsInStereoMode", FR: "SOUND_IsInStereoMode")] IsInStereoMode,
		[AI_Definition(SCR: "Cond_IsMusicPlaying", EN: "SOUND_IsMusicPlaying", FR: "SOUND_IsMusicPlaying")] IsMusicPlaying,

		//********************************************************************************************************************************************************************************************************************************************************

		[AI_Definition(SCR: "Cond_IsShapnessMax", EN: "VID_IsShapnessMax", FR: "VID_IsShapnessMax")] IsShapnessMax,
		[AI_Definition(SCR: "COND_IsDataCorrupt", EN: "SLOT_IsDataCorrupt", FR: "SLOT_IsDataCorrupt")] IsSlotDataCorrupt,
		[AI_Definition(SCR: "COND_IsCheatMenu", EN: "CHEAT_IsCheatMenu", FR: "CHEAT_IsCheatMenu")] IsCheatMenu,
		[AI_Definition(SCR: "COND_IsUSBuild", EN: "COND_IsUSBuild", FR: "COND_IsUSBuild")] IsUSBuild,
		#endregion

		#region DefConCa
		[AI_Definition(SCR: "Cond_Cam_IsActive", EN: "Cam_IsActive", FR: "Cam_IsActive")] Cam_IsActive,
		[AI_Definition(SCR: "Cond_Cam_IsViewportOwner", EN: "Cam_IsViewportOwner", FR: "Cam_IsViewportOwner")] Cam_IsViewportOwner,
		// IA FLAGS 
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoDynamicTarget", EN: "Cam_IsFlagNoDynamicTarget", FR: "Cam_IsFlagNoDynamicTarget")] Cam_IsFlagNoDynamicTarget,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoAverageMoveTgtPerso", EN: "Cam_IsFlagNoAverageMoveTgtPerso", FR: "Cam_IsFlagNoAverageMoveTgtPerso")] Cam_IsFlagNoAverageMoveTgtPerso,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoParseCutAngle", EN: "Cam_IsFlagNoParseCutAngle", FR: "Cam_IsFlagNoParseCutAngle")] Cam_IsFlagNoParseCutAngle,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoVisibility", EN: "Cam_IsFlagNoVisibility", FR: "Cam_IsFlagNoVisibility")] Cam_IsFlagNoVisibility,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoVisibilityWithDynHie", EN: "Cam_IsFlagNoVisibilityWithDynHie", FR: "Cam_IsFlagNoVisibilityWithDynHie")] Cam_IsFlagNoVisibilityWithDynHie,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoDynChangeTheta", EN: "Cam_IsFlagNoDynChangeTheta", FR: "Cam_IsFlagNoDynChangeTheta")] Cam_IsFlagNoDynChangeTheta,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoShiftUntilPosReached", EN: "Cam_IsFlagNoShiftUntilPosReached", FR: "Cam_IsFlagNoShiftUntilPosReached")] Cam_IsFlagNoShiftUntilPosReached,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoDynSpeed", EN: "Cam_IsFlagNoDynSpeed", FR: "Cam_IsFlagNoDynSpeed")] Cam_IsFlagNoDynSpeed,
		// DNM FLAGS 
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoLinearParsing", EN: "Cam_IsFlagNoLinearParsing", FR: "Cam_IsFlagNoLinearParsing")] Cam_IsFlagNoLinearParsing,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoLinearInertia", EN: "Cam_IsFlagNoLinearInertia", FR: "Cam_IsFlagNoLinearInertia")] Cam_IsFlagNoLinearInertia,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoAngularParsing", EN: "Cam_IsFlagNoAngularParsing", FR: "Cam_IsFlagNoAngularParsing")] Cam_IsFlagNoAngularParsing,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoAngularInertia", EN: "Cam_IsFlagNoAngularInertia", FR: "Cam_IsFlagNoAngularInertia")] Cam_IsFlagNoAngularInertia,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoTargetParsing", EN: "Cam_IsFlagNoTargetParsing", FR: "Cam_IsFlagNoTargetParsing")] Cam_IsFlagNoTargetParsing,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoTargetInertia", EN: "Cam_IsFlagNoTargetInertia", FR: "Cam_IsFlagNoTargetInertia")] Cam_IsFlagNoTargetInertia,
		[AI_Definition(SCR: "Cond_Cam_IsFlagNoObstacle", EN: "Cam_IsFlagNoObstacle", FR: "Cam_IsFlagNoObstacle")] Cam_IsFlagNoObstacle,
		[AI_Definition(SCR: "Cond_Cam_IsFlagFixedOrientation", EN: "Cam_IsFlagFixedOrientation", FR: "Cam_IsFlagFixedOrientation")] Cam_IsFlagFixedOrientation,

		[AI_Definition(SCR: "Cond_Cam_IsFlagForcedPosition", EN: "Cam_IsFlagForcedPosition", FR: "Cam_IsFlagForcedPosition")] Cam_IsFlagForcedPosition,
		[AI_Definition(SCR: "Cond_Cam_IsFlagForcedTarget", EN: "Cam_IsFlagForcedTarget", FR: "Cam_IsFlagForcedTarget")] Cam_IsFlagForcedTarget,
		[AI_Definition(SCR: "Cond_Cam_IsFlagForcedAxis", EN: "Cam_IsFlagForcedAxis", FR: "Cam_IsFlagForcedAxis")] Cam_IsFlagForcedAxis,

		#endregion

		#region Hype PC & PS2
		[AI_Definition(SCR: "Cond_CollideWithPoint")] CollideWithPoint,
		[AI_Definition(SCR: "Cond_CollideWithTriangle")] CollideWithTriangle,
		[AI_Definition(SCR: "Cond_CollideWithEdge")] CollideWithEdge,
		[AI_Definition(SCR: "Cond_CollideWithSphere")] CollideWithSphere,
		[AI_Definition(SCR: "Cond_CollideWithAlignedBox")] CollideWithAlignedBox,

		[AI_Definition(SCR: "Cond_InEnvironmentAir")] InEnvironmentAir,
		[AI_Definition(SCR: "Cond_InEnvironmentWater")] InEnvironmentWater,
		[AI_Definition(SCR: "Cond_InEnvironmentFire")] InEnvironmentFire,

		[AI_Definition(SCR: "Cond_IsInReflex")] IsInReflex,
		[AI_Definition(SCR: "Cond_WallIsTypeOfGMTCollide")] WallIsTypeOfGMTCollide,
		[AI_Definition(SCR: "Cond_ObstacleIsTypeOfGMTCollide")] ObstacleIsTypeOfGMTCollide,
		[AI_Definition(SCR: "Cond_IsValidWay")] IsValidWay,
		[AI_Definition(SCR: "Cond_InTopOfJump")] InTopOfJump,
		[AI_Definition(SCR: "Cond_CanSwim")] CanSwim,
		[AI_Definition(SCR: "Cond_CanSwimOnSurface")] CanSwimOnSurface,
		[AI_Definition(SCR: "Cond_CanSwimUnderWater")] CanSwimUnderWater,
		[AI_Definition(SCR: "Cond_IsNotOutOfDepth")] IsNotOutOfDepth,
		[AI_Definition(SCR: "Cond_IsCompletelyOutOfWater")] IsCompletelyOutOfWater,
		[AI_Definition(SCR: "Cond_LSY_IsSpeechOver")] LSY_IsSpeechOver,
		[AI_Definition(SCR: "Cond_SeePosition")] SeePosition,
		[AI_Definition(SCR: "Cond_SeePositionWithOffset")] SeePositionWithOffset,
		[AI_Definition(SCR: "Cond_SeePersoWithOffset")] SeePersoWithOffset,
		[AI_Definition(SCR: "Cond_IsSectorInTranslation")] IsSectorInTranslation,
		[AI_Definition(SCR: "Cond_IsSectorInRotation")] IsSectorInRotation,
		[AI_Definition(SCR: "Condition_Camera_IsInState")] IsCameraInState,
		[AI_Definition(SCR: "Condition_Camera_IsInInitialState")] IsCameraInInitialState,
		[AI_Definition(SCR: "Condition_Camera_IsActive")] IsCameraActive,
		[AI_Definition(SCR: "Condition_Camera_IsViewportOwner")] IsCameraViewportOwner,
		[AI_Definition(SCR: "Condition_Camera_IsCameraTargetVisible")] IsCameraTargetVisible,
		[AI_Definition(SCR: "Condition_Camera_IsCameraTargetMoving")] IsCameraTargetMoving,
		[AI_Definition(SCR: "Cond_Camera_IsCamReachedItsOptPos")] IsCameraReachedItsOptPos,
		[AI_Definition(SCR: "Cond_Camera_IsCamInAlphaOrientation")] IsCameraInAlphaOrientation,
		[AI_Definition(SCR: "Cond_Camera_IsCamInTetaOrientation")] IsCameraInTetaOrientation,
		[AI_Definition(SCR: "Cond_IsSurfaceHeightMoving")] IsSurfaceHeightMoving,
		[AI_Definition(SCR: "Cond_TestPower")] TestPower,
		[AI_Definition(SCR: "Cond_IsAtLeftOfPerso")] HasAtLeft,
		[AI_Definition(SCR: "Cond_IsAtRightOfPerso")] HasAtRight,
		[AI_Definition(SCR: "Cond_IsBehindPerso")] HasBehind,
		[AI_Definition(SCR: "Cond_IsInFrontOfPerso")] HasInFront,
		[AI_Definition(SCR: "Cond_IsAbovePerso")] HasAbove,
		[AI_Definition(SCR: "Cond_IsBelowPerso")] HasBelow,

		[AI_Definition(SCR: "Cond_GetAction")] GetAction,
		[AI_Definition(SCR: "Cond_Inv_InventoryIsFull")] Inv_InventairePlein,
		[AI_Definition(SCR: "Cond_Inv_FindObject")] Inv_TrouverObjet,
		[AI_Definition(SCR: "Cond_GetDialogStatus")] GetDialogStatus,
		[AI_Definition(SCR: "Cond_DLG_IsDialogOver")] DLG_IsDialogOver,
		[AI_Definition(SCR: "Cond_DLG_IsScrollingOver")] DLG_IsScrollingOver,
		[AI_Definition(SCR: "Cond_ActionFinished")] ActionFinished,
		[AI_Definition(SCR: "SCT_ActorInSector")] SCT_ActorInSector,
		[AI_Definition(SCR: "Cond_CollisionSphereSphere")] CollisionSphereSphere,
		[AI_Definition(SCR: "Cond_SectorActive")] SectorActive,
		[AI_Definition(SCR: "Cond_SND_IsSonFinished")] IsSoundFinished,
		#endregion

		#region R3 PS2
		[AI_Definition(SCR: "Cond_UserEvent_IsSet3")] UserEvent_IsSet3,
		[AI_Definition(SCR: "Cond_IsValidVMT")]  IsValidVMT,
		[AI_Definition(SCR: "Cond_IsValidSuperObject")] IsValidSPO,
		[AI_Definition(SCR: "Cond_IsGraphValid")] IsValidGraph,
		[AI_Definition(SCR: "Cond_IsRLITransitionInProgress")] IsRLITransitionInProgress,
		[AI_Definition(SCR: "Cond_ALWACT_IsInAlwaysActiveList")] IsInAlwaysActiveList,
		[AI_Definition(SCR: "Cond_ALWACT_IsAlwaysActive")] IsAlwaysActive,
		[AI_Definition(SCR: "PAD2_IsAnActivePad")] IsAnActivePad,
		[AI_Definition(SCR: "IsMultitap")] IsMultitap,
		[AI_Definition(SCR: "Cond_SAV2_IsValid")] SAV2_IsValid,
		[AI_Definition(SCR: "IsWidescreen")] IsWidescreen,
		EngineIsInPAL,
		CheckAnimSmooth,
		[AI_Definition(SCR: "Cond_IsTooFar")] IsTooFar,
		[AI_Definition(SCR: "Cond_CheckSubAnimEnd")] IsSubAnimPlaying,
		[AI_Definition(SCR: "Cond_CheckCustomBitSubAnim")] TestCBSubAnim,
		[AI_Definition(SCR: "Cond_IsInSubAnim")] IsInSubAnim,
		IsSubAnimNearEnd,
		[AI_Definition(SCR: "Cond_EqualitySPO")] IsSameSPO,
		[AI_Definition(SCR: "Cond_AnalJoyPressedBut")] PressedPadBut,
		[AI_Definition(SCR: "Cond_AnalJoyJustPressedBut")] JustPressedPadBut,
		[AI_Definition(SCR: "Cond_AnalJoyReleasedBut")] ReleasedPadBut,
		[AI_Definition(SCR: "Cond_AnalJoyJustReleasedBut")] JustReleasedPadBut,
		[AI_Definition(SCR: "Func_CineIsPlaying")] IsCinePlaying,
		LoadInProgress,
		[AI_Definition(SCR: "Cond_SAV2_LastError")] SAV2LastError,
		[AI_Definition(SCR: "Cond_SAV2_CheckMCStatus")] CheckMCStatus,
		[AI_Definition(SCR: "Cond_IsInStereoMode")] SND_IsInStereoMode, // Redefinition of version without SND_ from R2
		[AI_Definition(SCR: "Cond_IsMusicPlaying")] SND_IsMusicPlaying, // Redefinition of version without SND_ from R2
		SND_IsVoicePlaying,
		SND_IsEventValid,
		#endregion

		#region RA PS2 (Custom name, SCR from the ELF)
		[AI_Definition(SCR: "Func_NetNeedDataDescription")] NetNeedDataDescription,
		[AI_Definition(SCR: "Func_NetWasUpdated")] NetWasUpdated,
		[AI_Definition(SCR: "Func_NetWasBooleanUpdated")] NetWasBooleanUpdated,
		[AI_Definition(SCR: "Func_NetWasPersoUpdated")] NetWasPersoUpdated,
		[AI_Definition(SCR: "Func_NetWasIntegerUpdated")] NetWasIntegerUpdated,
		[AI_Definition(SCR: "Func_NetWasVectorUpdated")] NetWasVectorUpdated,
		[AI_Definition(SCR: "Func_NetWasRealUpdated")] NetWasRealUpdated,
		[AI_Definition(SCR: "Func_NetGetEvent")] NetGetEvent,
		[AI_Definition(SCR: "Func_NetIsPlayerConnected")] NetIsPlayerConnected,
		#endregion

		#region RA PC (Custom name)
		NetPing1,
		NetPing2,
		NetHasInstalledUbiGameService,
		#endregion

		#region TT:SE PC
		[AI_Definition(SCR: "Cond_CanIAddObjectInInventory")] CanIAddObjectInInventory,
		#endregion

		#region Largo (Custom name)
		AKeyJustPressedAlphanumeric,
		#endregion

		#region R2 iOS & DC
		IsInternetAvailable,
		IsRumblePakInPort1,
		IsRumblePakInPort2,
		IsRumblePakInPort1_Active,
		IsRumblePakInPort2_Active,
		IsStartButtonPressed,
		DC_IsNTSC,
		DC_OptionLoaded1,
		DC_OptionLoaded2,
		DC_Video1,
		DC_Video2,
		#endregion
	}
}