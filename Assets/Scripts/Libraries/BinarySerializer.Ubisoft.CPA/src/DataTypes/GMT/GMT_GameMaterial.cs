namespace BinarySerializer.Ubisoft.CPA {
	public class GMT_GameMaterial : BinarySerializable {
		public Pointer<GLI_Material> VisualMaterial { get; set; }
		public Pointer<MEC_MaterialCharacteristics> MechanicsMaterial { get; set; }
		public int SoundMaterial { get; set; }
		public Pointer<GMT_CollideMaterial> CollideMaterial { get; set; }

		public const uint InvalidCollideMaterial = 0xFFFFFFFF;

		public override void SerializeImpl(SerializerObject s) {
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)
				&& !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VisualMaterial = s.SerializePointer<GLI_Material>(VisualMaterial, name: nameof(VisualMaterial))?.ResolveObject(s);
				MechanicsMaterial = s.SerializePointer<MEC_MaterialCharacteristics>(MechanicsMaterial, name: nameof(MechanicsMaterial))?.ResolveObject(s);
			}
			SoundMaterial = s.Serialize<int>(SoundMaterial, name: nameof(SoundMaterial));
			CollideMaterial = s.SerializePointer<GMT_CollideMaterial>(CollideMaterial, nullValue: InvalidCollideMaterial, name: nameof(CollideMaterial))?.ResolveObject(s);
		}
	}
}
