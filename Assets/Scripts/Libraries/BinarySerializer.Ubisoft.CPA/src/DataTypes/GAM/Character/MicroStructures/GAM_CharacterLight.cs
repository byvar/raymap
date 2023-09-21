namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterLight : BinarySerializable {
		public bool LocalLight { get; set; }
		public bool OnlyLocalLight { get; set; }
		public bool GiroPhare { get; set; }
		public bool Pulsing { get; set; }

		public MTH3D_Vector Position { get; set; }
		public MTH3D_Vector Angle { get; set; }

		public float GiroStep { get; set; }
		public float PulseStep { get; set; }
		public float PulseMaxRange { get; set; }
		public float GiroAngle { get; set; }

		public Pointer<GLI_Light> Light { get; set; }
		public Pointer<GEO_GeometricObject> VisualLight { get; set; }
		public Pointer<SCT_DynamicLight> LightInSector { get; set; }

		public byte RLIInUseCount { get; set; } // 0, 1 or 2 (2 = blend)
		public byte FirstRLI { get; set; } // Index of first RLI in visual set
		public byte SecondRLI { get; set; } // Index of second RLI
		public float RLIBlendPercent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				LocalLight = s.Serialize<bool>(LocalLight, name: nameof(LocalLight));
				OnlyLocalLight = s.Serialize<bool>(OnlyLocalLight, name: nameof(OnlyLocalLight));
			}
			GiroPhare = s.Serialize<bool>(GiroPhare, name: nameof(GiroPhare));
			Pulsing = s.Serialize<bool>(Pulsing, name: nameof(Pulsing));
			s.Align(4, Offset);

			Position = s.SerializeObject<MTH3D_Vector>(Position, name: nameof(Position));
			Angle = s.SerializeObject<MTH3D_Vector>(Angle, name: nameof(Angle));

			GiroStep = s.Serialize<float>(GiroStep, name: nameof(GiroStep));
			PulseStep = s.Serialize<float>(PulseStep, name: nameof(PulseStep));
			PulseMaxRange = s.Serialize<float>(PulseMaxRange, name: nameof(PulseMaxRange));
			GiroAngle = s.Serialize<float>(GiroAngle, name: nameof(GiroAngle));

			Light = s.SerializePointer<GLI_Light>(Light, name: nameof(Light))?.ResolveObject(s);
			VisualLight = s.SerializePointer<GEO_GeometricObject>(VisualLight, name: nameof(VisualLight))?.ResolveObject(s);
			LightInSector = s.SerializePointer<SCT_DynamicLight>(LightInSector, name: nameof(LightInSector))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				RLIInUseCount = s.Serialize<byte>(RLIInUseCount, name: nameof(RLIInUseCount));
				FirstRLI = s.Serialize<byte>(FirstRLI, name: nameof(FirstRLI));
				SecondRLI = s.Serialize<byte>(SecondRLI, name: nameof(SecondRLI));
				s.Align(4, Offset);
				RLIBlendPercent = s.Serialize<float>(RLIBlendPercent, name: nameof(RLIBlendPercent));
			}
		}
	}
}
