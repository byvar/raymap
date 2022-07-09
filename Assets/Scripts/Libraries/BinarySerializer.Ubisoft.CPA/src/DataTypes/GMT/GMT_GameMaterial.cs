namespace BinarySerializer.Ubisoft.CPA {
	public class GMT_GameMaterial : BinarySerializable {
		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
