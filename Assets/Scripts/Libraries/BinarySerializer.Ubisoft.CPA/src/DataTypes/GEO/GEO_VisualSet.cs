namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_VisualSet : BinarySerializable {
		public float LastDistance { get; set; }
		public ushort LodDefinitionsCount { get; set; }
		public ushort Type { get; set; } // TODO: What is this?
		public Pointer<float[]> Thresholds { get; set; }
		public Pointer<Pointer<GEO_GeometricObject>[]> LodDefinitions { get; set; }
		public Pointer<Pointer<ISI_Radiosity>[]> RLI { get; set; }
		public uint RLICount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LastDistance = s.Serialize<float>(LastDistance, name: nameof(LastDistance));
			LodDefinitionsCount = s.Serialize<ushort>(LodDefinitionsCount, name: nameof(LodDefinitionsCount));
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				Type = s.Serialize<ushort>(Type, name: nameof(Type));
			s.Align(4, Offset);
			Thresholds = s.SerializePointer<float[]>(Thresholds, name: nameof(Thresholds))?.ResolveArray(s, LodDefinitionsCount);
			LodDefinitions = s.SerializePointer<Pointer<GEO_GeometricObject>[]>(LodDefinitions, name: nameof(LodDefinitions))
				?.ResolvePointerArray(s, LodDefinitionsCount);
			LodDefinitions?.Value?.ResolveObject(s);
			RLI = s.SerializePointer<Pointer<ISI_Radiosity>[]>(RLI, name: nameof(RLI));
			RLICount = s.Serialize<uint>(RLICount, name: nameof(RLICount));

			RLI?.ResolvePointerArray(s, RLICount);
			RLI?.Value?.ResolveObject(s);
		}
	}
}
