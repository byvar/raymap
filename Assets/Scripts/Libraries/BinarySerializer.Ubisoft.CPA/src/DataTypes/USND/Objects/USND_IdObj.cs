namespace BinarySerializer.Ubisoft.CPA {
	public abstract class USND_IdObj : BinarySerializable {
		// The ID of this object
		public USND_UUID Id { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Id = s.SerializeObject<USND_UUID>(Id, name: nameof(Id));
		}
	}
}