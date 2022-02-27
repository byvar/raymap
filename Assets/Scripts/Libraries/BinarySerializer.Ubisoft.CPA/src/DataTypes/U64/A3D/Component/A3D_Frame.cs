using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_Frame : BinarySerializable
    {
        public ushort NTTOIndex { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			NTTOIndex = s.Serialize<ushort>(NTTOIndex, name: nameof(NTTOIndex));
		}
    }
}