namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Quaternion : BinarySerializable, ISerializerShortLog {
		public MTH4D_ShortQuaternion Quaternion { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Quaternion = s.SerializeObject<MTH4D_ShortQuaternion>(Quaternion, name: nameof(Quaternion));
		}

		public string ShortLog => ToString();
		public override string ToString() => Quaternion.ToString();
	}
}