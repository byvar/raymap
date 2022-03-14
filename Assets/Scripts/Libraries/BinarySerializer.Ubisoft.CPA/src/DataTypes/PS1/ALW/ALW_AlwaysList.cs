namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ALW_AlwaysList : BinarySerializable
	{
		public int Index { get; set; }
		public int Count { get; set; }
		public Pointer ItemsPointer { get; set; }
		public uint InvalidPointer { get; set; }

		// Serialized from pointers
		public ALW_AlwaysItem[] Items { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			Index = s.Serialize<int>(Index, name: nameof(Index));
			Count = s.Serialize<int>(Count, name: nameof(Count));
			ItemsPointer = s.SerializePointer(ItemsPointer, name: nameof(ItemsPointer));
			InvalidPointer = s.Serialize<uint>(InvalidPointer, name: nameof(InvalidPointer));

			// Serialize data from pointers
			s.DoAt(ItemsPointer, () => Items = s.SerializeObjectArray<ALW_AlwaysItem>(Items, Count, name: nameof(Items)));
		}
	}
}