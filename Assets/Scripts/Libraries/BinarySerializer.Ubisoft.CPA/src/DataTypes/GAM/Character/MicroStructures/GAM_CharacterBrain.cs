namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterBrain : BinarySerializable {
		public Pointer<AI_Mind> Mind { get; set; }
		public Pointer<GMT_GameMaterial> LastCollidedGoThroughMaterial { get; set; }
		public bool WarnMechanicsFlag { get; set; }
		public bool ActiveDuringTransition { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Mind = s.SerializePointer<AI_Mind>(Mind, name: nameof(Mind))?.ResolveObject(s);
			if (s.GetCPASettings().EngineVersion != EngineVersion.Rayman2Revolution) {
				LastCollidedGoThroughMaterial = s.SerializePointer<GMT_GameMaterial>(LastCollidedGoThroughMaterial,
					nullValue: GMT_GameMaterial.InvalidGameMaterial, name: nameof(LastCollidedGoThroughMaterial))?.ResolveObject(s);
			}
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				WarnMechanicsFlag = s.Serialize<bool>(WarnMechanicsFlag, name: nameof(WarnMechanicsFlag));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					ActiveDuringTransition = s.Serialize<bool>(ActiveDuringTransition, name: nameof(ActiveDuringTransition));
				}
			}
		}
	}
}
