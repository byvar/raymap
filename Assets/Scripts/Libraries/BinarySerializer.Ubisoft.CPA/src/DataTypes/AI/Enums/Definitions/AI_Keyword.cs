namespace BinarySerializer.Ubisoft.CPA {
	// Find: (NU_)?M_DEFINE_[^ \t\(]*[ \t]*\([ \t]*e[^_]*_([^ \t,]*)[ \t]*,[ \t]*([^ \t,]*)[ \t]*,([ \t]*([^ \t,]*)[ \t]*,)?[ \t]*([^ \t,]*)[ \t]*,[ \t]*fn_.*
	// Replace: \[Definition\(SCR: \6, EN: \5, FR: \3\)] \2,
	public enum AI_Keyword {
		#region DefKey
		[AI_Definition(SCR: "If", EN: "If", FR: "Si")] If,
		[AI_Definition(SCR: "IfNot", EN: "IfNot", FR: "SiPas")] IfNot,
		[AI_Definition(SCR: "If2", EN: "If2", FR: "Si2")] If2,
		[AI_Definition(SCR: "If4", EN: "If4", FR: "Si4")] If4,
		[AI_Definition(SCR: "If8", EN: "If8", FR: "Si8")] If8,
		[AI_Definition(SCR: "If16", EN: "If16", FR: "Si16")] If16,
		[AI_Definition(SCR: "IfDebug", EN: "IfDebug", FR: "SiDebug")] IfDebug,
		[AI_Definition(SCR: "IfNotU64", EN: "IfNotU64", FR: "SiPasU64")] IfNotU64,
		[AI_Definition(SCR: "Then", EN: "Then", FR: "Alors")] Then,
		[AI_Definition(SCR: "Else", EN: "Else", FR: "Sinon")] Else,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "_EngineGoto", EN: "_EngineGoto", FR: "_EngineGoto")] EngineGoto,
		//********************************************************************************************************************************************************************************************************************************************************
		[AI_Definition(SCR: "Me", EN: "Me", FR: "Moi")] Me,
		[AI_Definition(SCR: "MainActor", EN: "MainActor", FR: "ActeurPrincipal")] MainActor,
		[AI_Definition(SCR: "World", FR: "Monde")] World,
		[AI_Definition(SCR: "Nobody", EN: "Nobody", FR: "Personne")] Nobody,
		[AI_Definition(SCR: "NoSuperObject", EN: "NoSuperObject", FR: "SansSuperObjet")] NoSuperObject,

		[AI_Definition(SCR: "Nowhere", EN: "Nowhere", FR: "Nullepart")] Nowhere,
		[AI_Definition(SCR: "EmptyText", EN: "EmptyText", FR: "TexteVide")] EmptyText,

		[AI_Definition(SCR: "CapaNull", EN: "CapaNull", FR: "CapaZero")] CapsNull,
		[AI_Definition(SCR: "NoGraph", EN: "NoGraph", FR: "SansGraph")] NoGraph,

		[AI_Definition(SCR: "NoAction", EN: "NoAction", FR: "SansAction")] NoAction,

		// Editor
		[AI_Definition(SCR: "NoComport", EN: "NoComport", FR: "SansComportement")] NoComport,
		[AI_Definition(SCR: "Schedule", EN: "Schedule", FR: "EmploiTemps")] Schedule,
		[AI_Definition(SCR: "InterruptRule", EN: "InterruptRule", FR: "RegleInterruption")] InterruptRule,
		[AI_Definition(SCR: "Vector", EN: "Vector", FR: "Vecteur")] Vector,
		[AI_Definition(SCR: "ConstantVector", EN: "ConstantVector", FR: "VecteurConstant")] ConstVector,
		[AI_Definition(SCR: "Endif", EN: "Endif", FR: "FinSi")] Endif,
		[AI_Definition(SCR: "Macro", EN: "Macro", FR: "Macro")] Macro,
		[AI_Definition(SCR: "True", EN: "True", FR: "Vrai")] True,
		[AI_Definition(SCR: "False", EN: "False", FR: "Faux")] False,
		[AI_Definition(SCR: "StopEngine", EN: "StopEngine", FR: "BloquantMoteur")] StopEngine,
		[AI_Definition(SCR: "StopRule", EN: "StopRule", FR: "BloquantRegle")] StopRule,
		[AI_Definition(SCR: "NonStop", EN: "NonStop", FR: "NonBloquant")] NonStop,
		#endregion

		#region R3 PS2 (No definitions)
		[AI_Definition(SCR: "If32", EN: "If32", FR: "Si32")] If32,
		[AI_Definition(SCR: "If64", EN: "If64", FR: "Si64")] If64,
		[AI_Definition(SCR: "IfNot2", EN: "IfNot2", FR: "SiPas2")] IfNot2,
		[AI_Definition(SCR: "IfNot4", EN: "IfNot4", FR: "SiPas4")] IfNot4,
		[AI_Definition(SCR: "IfNot8", EN: "IfNot8", FR: "SiPas8")] IfNot8,
		[AI_Definition(SCR: "IfNot16", EN: "IfNot16", FR: "SiPas16")] IfNot16,
		[AI_Definition(SCR: "IfNot32", EN: "IfNot32", FR: "SiPas32")] IfNot32,
		[AI_Definition(SCR: "IfNot64", EN: "IfNot64", FR: "SiPas64")] IfNot64,

		NoInput,
		NoSoundEvent,
		NoLight,

		NoSOL,

		NoGameMaterial,
		NoVisualMaterial,
		While,
		BeginWhile,
		EndWhile,
		#endregion
	}
}