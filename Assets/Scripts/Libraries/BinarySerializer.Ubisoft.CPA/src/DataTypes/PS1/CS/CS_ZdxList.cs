namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ZDXList : BinarySerializable
	{
		public uint EntriesCount { get; set; }
		public Pointer EntriesPointer { get; set; }

		// Serialized from pointers
		public CS_ZDXEntry[] Entries { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			EntriesCount = s.Serialize<uint>(EntriesCount, name: nameof(EntriesCount));
			EntriesPointer = s.SerializePointer(EntriesPointer, name: nameof(EntriesPointer));

			// Serialize data from pointers
			s.DoAt(EntriesPointer, () => Entries = s.SerializeObjectArray<CS_ZDXEntry>(Entries, EntriesCount, name: nameof(Entries)));
		}
	}
}