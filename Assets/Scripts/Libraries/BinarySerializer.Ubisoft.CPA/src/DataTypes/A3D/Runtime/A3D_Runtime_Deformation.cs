namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Runtime_Deformation : BinarySerializable {
		public uint DeformationLinksCount { get; set; }
		public Pointer<A3D_Deformation[]> DeformationLinks { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DeformationLinksCount = s.Serialize<uint>(DeformationLinksCount, name: nameof(DeformationLinksCount));
			DeformationLinks = s.SerializePointer<A3D_Deformation[]>(DeformationLinks, name: nameof(DeformationLinks))
				?.ResolveObjectArray(s, DeformationLinksCount);
		}
	}
}