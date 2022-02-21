using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_Channel : BinarySerializable
    {
        public ushort KeysCount { get; set; }
		public ushort ChannelNumber { get; set; }
		public ushort LocalPivotPosIndex { get; set; }
		public ushort Align { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			KeysCount = s.Serialize<ushort>(KeysCount, name: nameof(KeysCount));
			ChannelNumber = s.Serialize<ushort>(ChannelNumber, name: nameof(ChannelNumber));
			LocalPivotPosIndex = s.Serialize<ushort>(LocalPivotPosIndex, name: nameof(LocalPivotPosIndex));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
		}
    }
}