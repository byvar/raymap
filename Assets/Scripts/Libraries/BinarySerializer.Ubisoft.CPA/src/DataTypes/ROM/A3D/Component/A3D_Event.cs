using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_Event : BinarySerializable
    {
        public ushort EventIndexInTBL { get; set; }
		public ushort FrameIndex { get; set; }
		public ushort ChannelIndex { get; set; }
		public ushort Align { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			EventIndexInTBL = s.Serialize<ushort>(EventIndexInTBL, name: nameof(EventIndexInTBL));
			FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
			ChannelIndex = s.Serialize<ushort>(ChannelIndex, name: nameof(ChannelIndex));
			Align = s.Serialize<ushort>(Align, name: nameof(Align));
		}
    }
}