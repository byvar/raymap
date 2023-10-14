namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ScriptAI : BinarySerializable { // Model for AI_Intelligence
		public Pointer<AI_Comport[]> Comports { get; set; }
		public uint ComportsCount { get; set; }
		public Pointer<AI_Comport> DefaultInitComport { get; set; }
		public byte ActionTableEntriesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Comports = s.SerializePointer<AI_Comport[]>(Comports, name: nameof(Comports));
			ComportsCount = s.Serialize<uint>(ComportsCount, name: nameof(ComportsCount));
			DefaultInitComport = s.SerializePointer<AI_Comport>(DefaultInitComport, name: nameof(DefaultInitComport))?.ResolveObject(s);
			ActionTableEntriesCount = s.Serialize<byte>(ActionTableEntriesCount, name: nameof(ActionTableEntriesCount));
			
			Comports?.ResolveObjectArray(s, ComportsCount);
		}
	}
}
