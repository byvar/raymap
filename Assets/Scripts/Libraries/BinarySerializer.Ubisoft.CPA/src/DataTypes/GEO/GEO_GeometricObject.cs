namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_GeometricObject : BinarySerializable {
		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
