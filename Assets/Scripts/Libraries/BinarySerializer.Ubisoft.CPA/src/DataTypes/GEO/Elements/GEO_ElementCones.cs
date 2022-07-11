namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementCones : GEO_Element {
		public Pointer<GEO_IndexedCone[]> Cones { get; set; }
		public ushort ConesCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Cones = s.SerializePointer<GEO_IndexedCone[]>(Cones, name: nameof(Cones));
				ConesCount = s.Serialize<ushort>(ConesCount, name: nameof(ConesCount));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
			} else {
				ConesCount = s.Serialize<ushort>(ConesCount, name: nameof(ConesCount));
				s.Align(4, Offset);
				Cones = s.SerializePointer<GEO_IndexedCone[]>(Cones, name: nameof(Cones));
			}
			Cones?.ResolveObjectArray(s, ConesCount);
		}
	}
}
