namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterSound : BinarySerializable {
		public SND_RollOffParam RollOff { get; set; }
		public SND_RollOffParam RollOffLipsSynchro { get; set; }
		public Pointer<HIE_SuperObject> MechaCharacter { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RollOff = s.SerializeObject<SND_RollOffParam>(RollOff, name: nameof(RollOff));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				RollOffLipsSynchro = s.SerializeObject<SND_RollOffParam>(RollOffLipsSynchro, name: nameof(RollOffLipsSynchro));
			}
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				MechaCharacter = s.SerializePointer<HIE_SuperObject>(MechaCharacter, name: nameof(MechaCharacter))?.ResolveObject(s);
			}
		}
	}
}
