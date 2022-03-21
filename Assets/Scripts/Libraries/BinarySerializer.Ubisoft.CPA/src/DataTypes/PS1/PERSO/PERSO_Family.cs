namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class PERSO_Family : BinarySerializable
	{
		public string Name { get; set; }

		public uint Uint_00 { get; set; }
		public uint Uint_04 { get; set; }
		public Pointer AnimationsPointer { get; set; }
		public uint AnimationsCount { get; set; }
		public uint Uint_10 { get; set; }
		public uint Uint_14 { get; set; }
		public int Int_18 { get; set; }
		public int Int_1C { get; set; }

		// Serialized from pointers
		public ANIM_Animation[] Animations { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			// TODO: Find better solution to this since this will cause issues when writing. Perhaps start the class at the
			//       name and do Pointer - 0x24 whenever serializing this object?
			s.DoAt(s.CurrentPointer - 0x24, () => Name = s.SerializeString(Name, 0x24, name: nameof(Name)));

			Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
			Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
			AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
			AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));
			Uint_10 = s.Serialize<uint>(Uint_10, name: nameof(Uint_10));
			Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
			Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
			Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));

			// Serialize data from pointers
			s.DoAt(AnimationsPointer, () => 
				Animations = s.SerializeObjectArray<ANIM_Animation>(Animations, AnimationsCount, name: nameof(Animations)));
		}
	}
}