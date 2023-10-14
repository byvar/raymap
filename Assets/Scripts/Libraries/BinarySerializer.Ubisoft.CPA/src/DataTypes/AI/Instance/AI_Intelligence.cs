namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Intelligence : BinarySerializable {
		public Pointer<Pointer<AI_ScriptAI>> ScriptAI { get; set; } // Pointer toeither the Intelligence or Reflex pointer from the AI model. NOT a pointer to the start of a struct!
		public Pointer<AI_NodeInterpret> CurrentSchedule { get; set; } // 
		public Pointer<AI_Comport> CurrentComport { get; set; }
		public Pointer<AI_Comport> PreviousComport { get; set; }
		public Pointer<AI_ActionTable> ActionTable { get; set; }
		public Pointer<AI_Comport> InitComport { get; set; } // If 0, it takes the default index from the ScriptAI

		public override void SerializeImpl(SerializerObject s) {
			ScriptAI = s.SerializePointer<Pointer<AI_ScriptAI>>(ScriptAI, name: nameof(ScriptAI))?.ResolvePointer<AI_ScriptAI>(s);
			ScriptAI?.Value?.ResolveObject(s);

			CurrentSchedule = s.SerializePointer<AI_NodeInterpret>(CurrentSchedule, name: nameof(CurrentSchedule))?.ResolveObject(s);
			CurrentComport = s.SerializePointer<AI_Comport>(CurrentComport, name: nameof(CurrentComport))?.ResolveObject(s);
			PreviousComport = s.SerializePointer<AI_Comport>(PreviousComport, name: nameof(PreviousComport))?.ResolveObject(s);
			ActionTable = s.SerializePointer<AI_ActionTable>(ActionTable, name: nameof(ActionTable))?.ResolveObject(s);
			InitComport = s.SerializePointer<AI_Comport>(InitComport, name: nameof(InitComport))?.ResolveObject(s);
		}
	}
}
