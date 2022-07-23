namespace BinarySerializer.Ubisoft.CPA {
	public class MEC_MechanicsIdCard : BinarySerializable {
		public MEC_MechanicsId Identity { get; set; }
		public MEC_MechanicsIdCardData Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Identity = s.Serialize<MEC_MechanicsId>(Identity, name: nameof(Identity));
			Data = Identity switch {
				MEC_MechanicsId.Base => SerializeData<MEC_BaseMechanicsIdCard>(s),
				MEC_MechanicsId.Camera => SerializeData<MEC_CameraMechanicsIdCard>(s),
				_ => throw new BinarySerializableException(this, $"Encountered unknown MechanicsId {Identity}")
			};
		}

		private T SerializeData<T>(SerializerObject s) where T : MEC_MechanicsIdCardData, new() {
			return s.SerializeObject<T>((T)Data, onPreSerialize: t => t.Pre_Identity = Identity, name: nameof(Data));
		}
	}
}
