namespace BinarySerializer.Ubisoft.CPA {
	public class A3B_Frame : BinarySerializable {
		public uint Pre_ElementsCount { get; set; }

		public Pointer<Pointer<A3B_Element>[]> Elements { get; set; } // Channels
		public Pointer<MTH3D_Matrix> AngularSpeedMatrix { get; set; }
		public Pointer<MTH3D_Vector> LinearSpeedVector { get; set; }
		public Pointer<A3B_Hierarchy> Hierarchy { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Elements = s.SerializePointer<Pointer<A3B_Element>[]>(Elements, name: nameof(Elements))
				?.ResolvePointerArray<A3B_Element>(s, Pre_ElementsCount);
			Elements?.Value?.ResolveObject(s);

			AngularSpeedMatrix = s.SerializePointer<MTH3D_Matrix>(AngularSpeedMatrix, name: nameof(AngularSpeedMatrix))?.ResolveObject(s);
			LinearSpeedVector = s.SerializePointer<MTH3D_Vector>(LinearSpeedVector, name: nameof(LinearSpeedVector))?.ResolveObject(s);
			Hierarchy = s.SerializePointer<A3B_Hierarchy>(Hierarchy, name: nameof(Hierarchy))?.ResolveObject(s);
		}
	}
}