namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Runtime_Frame : BinarySerializable { // Same struct as A3B_Frame, but for later engine versions
		public uint Pre_ElementsCount { get; set; }

		public Pointer<Pointer<A3D_Runtime_Element>[]> Elements { get; set; } // Channels
		public Pointer<MTH3D_Matrix> AngularSpeedMatrix { get; set; }
		public Pointer<MTH3D_Vector> LinearSpeedVector { get; set; }
		public A3B_Hierarchy Hierarchy { get; set; }
		public A3D_Runtime_Deformation Deformation { get; set; }

		public Pointer<A3D_Animation_A3I> Animation { get; set; }
		public ushort FrameIndex { get; set; }
		public bool MatrixUpdated { get; set; }
		public byte CacheEntry { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Elements = s.SerializePointer<Pointer<A3D_Runtime_Element>[]>(Elements, name: nameof(Elements))
				?.ResolvePointerArray<A3D_Runtime_Element>(s, Pre_ElementsCount);
			Elements?.Value?.ResolveObject(s);

			AngularSpeedMatrix = s.SerializePointer<MTH3D_Matrix>(AngularSpeedMatrix, name: nameof(AngularSpeedMatrix))?.ResolveObject(s);
			LinearSpeedVector = s.SerializePointer<MTH3D_Vector>(LinearSpeedVector, name: nameof(LinearSpeedVector))?.ResolveObject(s);
			Hierarchy = s.SerializeObject<A3B_Hierarchy>(Hierarchy, name: nameof(Hierarchy));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Deformation = s.SerializeObject<A3D_Runtime_Deformation>(Deformation, name: nameof(Deformation));
			}
			Animation = s.SerializePointer<A3D_Animation_A3I>(Animation, name: nameof(Animation))?.ResolveObject(s);
			FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
			MatrixUpdated = s.Serialize<bool>(MatrixUpdated, name: nameof(MatrixUpdated));
			CacheEntry = s.Serialize<byte>(CacheEntry, name: nameof(CacheEntry));
		}
	}
}