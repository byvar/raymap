namespace BinarySerializer.Ubisoft.CPA {
	public class AI_ActionTable : BinarySerializable {
		public Pointer<AI_ActionTableEntry[]> Entries { get; set; }
		public byte EntriesCount { get; set; }
		public byte UsedEntriesCount { get; set; }
		public byte CurrentEntry { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Entries = s.SerializePointer(Entries, name: nameof(Entries));
			EntriesCount = s.Serialize<byte>(EntriesCount, name: nameof(EntriesCount));
			UsedEntriesCount = s.Serialize<byte>(UsedEntriesCount, name: nameof(UsedEntriesCount));
			CurrentEntry = s.Serialize<byte>(CurrentEntry, name: nameof(CurrentEntry));

			Entries?.ResolveObjectArray(s, EntriesCount);
		}
	}
}
