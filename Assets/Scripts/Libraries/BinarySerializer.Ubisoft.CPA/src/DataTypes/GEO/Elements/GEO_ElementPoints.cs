namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementPoints : GEO_Element {
		public float Fatness { get; set; }
		public Pointer<ushort[]> PointIndices { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }
		public ushort PointsCount { get; set; }
		public short ParallelBoxIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Fatness = s.Serialize<float>(Fatness, name: nameof(Fatness));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				PointIndices = s.SerializePointer<ushort[]>(PointIndices, name: nameof(PointIndices));
				GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
			} else {
				PointsCount = s.Serialize<ushort>(PointsCount, name: nameof(PointsCount));
				s.Align(4, Offset);
				PointIndices = s.SerializePointer<ushort[]>(PointIndices, name: nameof(PointIndices));
				GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
			}
			PointIndices?.ResolveValueArray(s, PointsCount);
		}
	}
}
