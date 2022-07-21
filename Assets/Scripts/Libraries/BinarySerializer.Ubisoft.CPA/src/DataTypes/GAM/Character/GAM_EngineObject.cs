namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_EngineObject : BinarySerializable {

		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}
