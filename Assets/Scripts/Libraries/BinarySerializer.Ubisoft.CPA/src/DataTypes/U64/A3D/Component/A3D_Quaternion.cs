﻿using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class A3D_Quaternion : BinarySerializable {
		public MTH4D_ShortQuaternion Quaternion { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Quaternion = s.SerializeObject<MTH4D_ShortQuaternion>(Quaternion, name: nameof(Quaternion));
		}
	}
}