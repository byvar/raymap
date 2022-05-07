namespace BinarySerializer.Ubisoft.CPA {
    public enum AI_DsgVarType {
        None,

		[AI_Definition(SCR: "Boolean", FR: "Booleen", EN: "Boolean")] Boolean,
		[AI_Definition(SCR: "_128To127", FR: "Entier_128To127", EN: "_128To127")] SByte,
		[AI_Definition(SCR: "0To255", FR: "Entier0To255", EN: "0To255")] UByte,
		[AI_Definition(SCR: "_32768To32767", FR: "Entier_32768To32767", EN: "_32768To32767")] Short,
		[AI_Definition(SCR: "0To65535", FR: "Entier0To65535", EN: "0To65535")] UShort,
		[AI_Definition(SCR: "Integer", FR: "Entier", EN: "Integer")] Int,
		[AI_Definition(SCR: "PositiveInteger", FR: "EntierPositif", EN: "PositiveInteger")] UInt,
		[AI_Definition(SCR: "Float", FR: "Reel", EN: "Float")] Float,
		[AI_Definition(SCR: "WayPoint", FR: "WayPoint", EN: "WayPoint")] WayPoint,
		[AI_Definition(SCR: "Way", FR: "Way", EN: "Way")] Way,
		[AI_Definition(SCR: "Perso", FR: "Perso", EN: "Perso")] Perso,
		[AI_Definition(SCR: "List", FR: "Liste", EN: "List")] List,
		[AI_Definition(SCR: "Vector", FR: "TypeVecteur", EN: "Vector")] Vector,
		[AI_Definition(SCR: "Comport", FR: "TypeComportement", EN: "Comport")] Comport,
		[AI_Definition(SCR: "Action", FR: "TypeAction", EN: "Action")] Action,
		[AI_Definition(SCR: "Text", FR: "Texte", EN: "Text")] Text,
		[AI_Definition(SCR: "GameMaterial", FR: "TypeMateriauJeu", EN: "GameMaterial")] GameMaterial,
		[AI_Definition(SCR: "Caps", FR: "TypeCapacite", EN: "Capability")] Caps,
		[AI_Definition(SCR: "Graph", FR: "TypeReseau", EN: "Graph")] Graph,
		[AI_Definition(SCR: "PersoArray", FR: "TypeTableauPerso", EN: "PersoArray")] PersoArray,
		[AI_Definition(SCR: "VectorArray", FR: "TypeTableauVecteur", EN: "VectorArray")] VectorArray,
		[AI_Definition(SCR: "FloatArray", FR: "TypeTableauReel", EN: "FloatArray")] FloatArray,
		[AI_Definition(SCR: "IntegerArray", FR: "TypeTableauEntier", EN: "IntegerArray")] IntegerArray,
		[AI_Definition(SCR: "WayPointArray", FR: "TypeTableauWayPoint", EN: "WayPointArray")] WayPointArray,
		[AI_Definition(SCR: "TextArray", FR: "TypeTableauTexte", EN: "TextArray")] TextArray,
		[AI_Definition(SCR: "SuperObject", FR: "TypeSuperObjet", EN: "SuperObject")] SuperObject,

		#region R3 PS2
		Input,
		SoundEvent,
		Light,
		VisualMaterial,
		SOLinks,
		GraphArray,
		SuperObjectArray,
		SOLinksArray,
		SoundEventArray,
		VisualMatArray,
		#endregion

		TextRefArray,
        ActionArray, // Hype

		Placeholder__R2PS2__Type2E,
		Placeholder__UnknownArray,
		Module,
		Placeholder__Largo__Type2F,
		Placeholder__Largo__Type30,
		ObjectTable,
	}
}