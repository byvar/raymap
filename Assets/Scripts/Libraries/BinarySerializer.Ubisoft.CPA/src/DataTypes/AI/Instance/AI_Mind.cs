namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Mind : BinarySerializable {
		public Pointer AIModel { get; set; }
		public Pointer<AI_Intelligence> Intelligence { get; set; }
		public Pointer<AI_Intelligence> Reflex { get; set; }
		public Pointer<AI_DsgMem> DsgMem { get; set; }

		public Pointer<string> PersoName { get; set; }

		public bool DoingIntel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AIModel = s.SerializePointer(AIModel, name: nameof(AIModel));
			Intelligence = s.SerializePointer<AI_Intelligence>(Intelligence, name: nameof(Intelligence))?.ResolveObject(s);
			if (!s.GetCPASettings().IsPressDemo) {
				Reflex = s.SerializePointer<AI_Intelligence>(Reflex, name: nameof(Reflex))?.ResolveObject(s);
				DsgMem = s.SerializePointer<AI_DsgMem>(DsgMem, name: nameof(DsgMem))?.ResolveObject(s);
			} else {
				DsgMem = s.SerializePointer<AI_DsgMem>(DsgMem, name: nameof(DsgMem))?.ResolveObject(s);
				Reflex = s.SerializePointer<AI_Intelligence>(Reflex, name: nameof(Reflex))?.ResolveObject(s);
			}

			if (s.GetCPASettings().Defines.HasFlag(CPA_EngineDefines.DebugAI)) {
				PersoName = s.SerializePointer<string>(PersoName, name: nameof(PersoName))?.ResolveString(s);
			}

			DoingIntel = s.Serialize<bool>(DoingIntel, name: nameof(DoingIntel));
		}
	}
}
