namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterDynamics : BinarySerializable { // Original struct name: Dynam
		public Pointer<DNM_Dynamics> Dynamics { get; set; }
		public Pointer<DNM_ParsingData> ParsingData { get; set; }
		public MEC_MechanicsId UsedMechanics { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Dynamics = s.SerializePointer<DNM_Dynamics>(Dynamics, name: nameof(Dynamics))?.ResolveObject(s);
			ParsingData = s.SerializePointer<DNM_ParsingData>(ParsingData, name: nameof(ParsingData))?.ResolveObject(s);
			UsedMechanics = s.Serialize<MEC_MechanicsId>(UsedMechanics, name: nameof(UsedMechanics));
		}
	}
}
