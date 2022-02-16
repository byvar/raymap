using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_Vector : BinarySerializable
    {
		public int IntX { get; set; } // Divide by 4096 to get float
		public int IntY { get; set; }
		public int IntZ { get; set; }

		public float FloatX { get; set; }
		public float FloatY { get; set; }
		public float FloatZ { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			if (s.GetCPASettings().Platform == Platform.N64) {
				FloatX = s.Serialize<float>(FloatX, name: nameof(FloatX));
				FloatY = s.Serialize<float>(FloatY, name: nameof(FloatY));
				FloatZ = s.Serialize<float>(FloatZ, name: nameof(FloatZ));
			} else {
				IntX = s.Serialize<int>(IntX, name: nameof(IntX));
				IntY = s.Serialize<int>(IntY, name: nameof(IntY));
				IntZ = s.Serialize<int>(IntZ, name: nameof(IntZ));
			}
		}
    }
}