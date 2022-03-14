namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class ALW_AlwaysItem : BinarySerializable
	{
		public Pointer SuperObjectPointer { get; set; }
		public uint Uint_04 { get; set; }
		public uint Uint_08 { get; set; }
		public uint Uint_0C { get; set; }

		// Serialized from pointers
		public HIE_SuperObject SuperObject { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			SuperObjectPointer = s.SerializePointer(SuperObjectPointer, name: nameof(SuperObjectPointer));
			Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
			Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
			Uint_0C = s.Serialize<uint>(Uint_0C, name: nameof(Uint_0C));

			s.DoAt(SuperObjectPointer, () => SuperObject = s.SerializeObject<HIE_SuperObject>(SuperObject, name: nameof(SuperObject)));
		}
	}
}