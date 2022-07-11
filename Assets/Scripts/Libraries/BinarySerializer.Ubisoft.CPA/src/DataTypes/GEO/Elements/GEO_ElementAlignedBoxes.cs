namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementAlignedBoxes : GEO_Element {
		public Pointer<GEO_IndexedAlignedBox[]> Boxes { get; set; }
		public ushort BoxesCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Boxes = s.SerializePointer<GEO_IndexedAlignedBox[]>(Boxes, name: nameof(Boxes));
				BoxesCount = s.Serialize<ushort>(BoxesCount, name: nameof(BoxesCount));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
			} else {
				BoxesCount = s.Serialize<ushort>(BoxesCount, name: nameof(BoxesCount));
				s.Align(4, Offset);
				Boxes = s.SerializePointer<GEO_IndexedAlignedBox[]>(Boxes, name: nameof(Boxes));
			}
			Boxes?.ResolveObjectArray(s, BoxesCount);
		}
	}
}
