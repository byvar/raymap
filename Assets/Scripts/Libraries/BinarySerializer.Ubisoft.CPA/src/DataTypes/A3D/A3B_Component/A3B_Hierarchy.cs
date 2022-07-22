namespace BinarySerializer.Ubisoft.CPA {
	public class A3B_Hierarchy : BinarySerializable {
		public uint CouplesCount { get; set; }
		public Pointer<A3D_Hierarchy[]> Couples { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CouplesCount = s.Serialize<uint>(CouplesCount, name: nameof(CouplesCount));
			Couples = s.SerializePointer<A3D_Hierarchy[]>(Couples, name: nameof(Couples))?.ResolveObjectArray(s, CouplesCount);
		}
	}
}