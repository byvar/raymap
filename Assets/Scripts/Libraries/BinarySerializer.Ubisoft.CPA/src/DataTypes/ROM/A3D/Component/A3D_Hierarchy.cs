using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_Hierarchy : BinarySerializable
    {
        public ushort Child { get; set; }
		public ushort Father { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			Child = s.Serialize<ushort>(Child, name: nameof(Child));
			Father = s.Serialize<ushort>(Father, name: nameof(Father));
		}
    }
}