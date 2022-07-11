namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_IndexedSphere : BinarySerializable {
		public float Radius { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }
		public ushort CenterPoint { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Radius = s.Serialize<float>(Radius, name: nameof(Radius));
				GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
				CenterPoint = s.Serialize<ushort>(CenterPoint, name: nameof(CenterPoint));
				s.Align(4, Offset);
			} else {
				CenterPoint = s.Serialize<ushort>(CenterPoint, name: nameof(CenterPoint));
				s.Align(4, Offset);
				Radius = s.Serialize<float>(Radius, name: nameof(Radius));
				GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
			}
		}
	}
}
