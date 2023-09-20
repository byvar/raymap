namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterSound : BinarySerializable {
		public SND_RollOffParam RollOff { get; set; }
		public Pointer<HIE_SuperObject> MechaCharacter { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RollOff = s.SerializeObject<SND_RollOffParam>(RollOff, name: nameof(RollOff));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				MechaCharacter = s.SerializePointer<HIE_SuperObject>(MechaCharacter, name: nameof(MechaCharacter))?.ResolveObject(s);
			}
		}
	}
}
