namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementSpheres : GEO_Element {
		public Pointer<GEO_IndexedSphere[]> Spheres { get; set; }
		public ushort SpheresCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Spheres = s.SerializePointer<GEO_IndexedSphere[]>(Spheres, name: nameof(Spheres));
				SpheresCount = s.Serialize<ushort>(SpheresCount, name: nameof(SpheresCount));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
			} else {
				SpheresCount = s.Serialize<ushort>(SpheresCount, name: nameof(SpheresCount));
				s.Align(4, Offset);
				Spheres = s.SerializePointer<GEO_IndexedSphere[]>(Spheres, name: nameof(Spheres));
			}
			Spheres?.ResolveObjectArray(s, SpheresCount);
		}
	}
}
