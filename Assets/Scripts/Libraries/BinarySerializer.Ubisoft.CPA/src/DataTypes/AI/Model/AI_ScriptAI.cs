namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ScriptAI : BinarySerializable { // Model for AI_Intelligence
		public Pointer<AI_Comport[]> Comports { get; set; }
		public uint ComportsCount { get; set; }
		public uint DefaultInitComportIndex { get; set; }
		public byte ActionTableEntriesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Comports = s.SerializePointer<AI_Comport[]>(Comports, name: nameof(Comports));
			ComportsCount = s.Serialize<uint>(ComportsCount, name: nameof(ComportsCount));
			DefaultInitComportIndex = s.Serialize<uint>(DefaultInitComportIndex, name: nameof(DefaultInitComportIndex));
			ActionTableEntriesCount = s.Serialize<byte>(ActionTableEntriesCount, name: nameof(ActionTableEntriesCount));
			
			Comports?.ResolveObjectArray(s, ComportsCount);
		}
	}
}
