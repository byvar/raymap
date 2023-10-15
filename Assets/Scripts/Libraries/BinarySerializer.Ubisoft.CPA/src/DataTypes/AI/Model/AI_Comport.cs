namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Comport : BinarySerializable {
		public string Name { get; set; }
		public Pointer<AI_TreeInterpret[]> Rules { get; set; } // Double pointer to scripts. Actually a pointer to AI model?
		public Pointer<AI_TreeInterpret> CurrentSchedule { get; set; }
		public AI_TreeInterpret CurrentSchedule_Revolution { get; set; }

		public uint CFastIndex { get; set; }
		public byte RulesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Defines.HasFlag(CPA_EngineDefines.DebugAI) && s.GetCPASettings().Platform != Platform.PS2) {
				Name = s.SerializeString(Name, length: 0x100, name: nameof(Name));
			}
			Rules = s.SerializePointer<AI_TreeInterpret[]>(Rules, name: nameof(Rules));
			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				CurrentSchedule_Revolution = s.SerializeObject<AI_TreeInterpret>(CurrentSchedule_Revolution, name: nameof(CurrentSchedule_Revolution));
			} else {
				CurrentSchedule = s.SerializePointer(CurrentSchedule, name: nameof(CurrentSchedule))?.ResolveObject(s);
			}
			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Demo
				|| s.GetCPASettings().EngineVersion == EngineVersion.RedPlanet
				|| s.GetCPASettings().Platform == Platform.DC) {
				CFastIndex = s.Serialize<uint>(CFastIndex, name: nameof(CFastIndex)); // TODO: Is this also what this is in those versions?
			}
			RulesCount = s.Serialize<byte>(RulesCount, name: nameof(RulesCount));
			s.Align(4, Offset);

			Rules?.ResolveObjectArray(s, RulesCount);
		}
	}
}
