namespace BinarySerializer.Ubisoft.CPA.PS1
{
	public class CS_ZDXEntry : BinarySerializable
	{
		public uint SpheresCount { get; set; }
		public Pointer SpheresPointer { get; set; }
		public uint BoxesCount { get; set; }
		public Pointer BoxesPointer { get; set; }
		public uint UnknownCount { get; set; }
		public Pointer UnknownPointer { get; set; } // 0x3c large

		// Serialized from pointers
		public CS_ZDXSphere[] Spheres { get; set; }
		public CS_ZDXBox[] Boxes { get; set; }

		public override void SerializeImpl(SerializerObject s)
		{
			SpheresCount = s.Serialize<uint>(SpheresCount, name: nameof(SpheresCount));
			SpheresPointer = s.SerializePointer(SpheresPointer, name: nameof(SpheresPointer));
			BoxesCount = s.Serialize<uint>(BoxesCount, name: nameof(BoxesCount));
			BoxesPointer = s.SerializePointer(BoxesPointer, name: nameof(BoxesPointer));
			UnknownCount = s.Serialize<uint>(UnknownCount, name: nameof(UnknownCount));
			UnknownPointer = s.SerializePointer(UnknownPointer, name: nameof(UnknownPointer));

			// Serialize data from pointers
			s.DoAt(SpheresPointer, () => Spheres = s.SerializeObjectArray<CS_ZDXSphere>(Spheres, SpheresCount, name: nameof(Spheres)));
			s.DoAt(BoxesPointer, () => Boxes = s.SerializeObjectArray<CS_ZDXBox>(Boxes, BoxesCount, name: nameof(Boxes)));
		}
	}
}