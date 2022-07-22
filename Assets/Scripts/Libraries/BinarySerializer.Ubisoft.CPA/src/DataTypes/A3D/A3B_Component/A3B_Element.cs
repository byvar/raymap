namespace BinarySerializer.Ubisoft.CPA {
	public class A3B_Element : BinarySerializable {

		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}