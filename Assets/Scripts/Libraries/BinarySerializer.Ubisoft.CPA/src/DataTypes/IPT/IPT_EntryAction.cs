namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_EntryAction : BinarySerializable {
		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
