namespace BinarySerializer.Ubisoft.CPA {
	public class AI_AIModel : BinarySerializable {
		public Pointer<AI_ScriptAI> Intelligence { get; set; }
		public Pointer<AI_ScriptAI> Reflex { get; set; }
		public Pointer<AI_DsgVar> DsgVar { get; set; }
		public Pointer<AI_ListOfMacros> Macros { get; set; }
		public bool IsSecondPassDone { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Intelligence = s.SerializePointer<AI_ScriptAI>(Intelligence, name: nameof(Intelligence))?.ResolveObject(s);
			Reflex = s.SerializePointer<AI_ScriptAI>(Reflex, name: nameof(Reflex))?.ResolveObject(s);
			DsgVar = s.SerializePointer<AI_DsgVar>(DsgVar, name: nameof(DsgVar))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Macros = s.SerializePointer<AI_ListOfMacros>(Macros, name: nameof(Macros))?.ResolveObject(s);
			}

			IsSecondPassDone = s.Serialize<bool>(IsSecondPassDone, name: nameof(IsSecondPassDone));
			s.Align(4, Offset);
		}
	}
}
