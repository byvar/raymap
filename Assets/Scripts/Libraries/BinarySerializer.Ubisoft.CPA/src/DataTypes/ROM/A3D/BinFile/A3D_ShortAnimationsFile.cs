using System;

namespace BinarySerializer.Ubisoft.CPA.ROM {
	public class A3D_ShortAnimationsFile : BinarySerializable {
		public A3D_ShortAnimation[] ShortAnimations { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// TODO: Get actual count
			ShortAnimations = s.SerializeObjectArrayUntil<A3D_ShortAnimation>(ShortAnimations, _ => s.CurrentFileOffset >= s.CurrentLength, name: nameof(ShortAnimations));
		}
	}
}