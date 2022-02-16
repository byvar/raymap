using System;

namespace BinarySerializer.Ubisoft.CPA.ROM {
	public class A3D_Quaternion : BinarySerializable {
		public CPA_ShortQuaternion Quaternion { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Quaternion = s.SerializeObject<CPA_ShortQuaternion>(Quaternion, name: nameof(Quaternion));
		}
	}
}