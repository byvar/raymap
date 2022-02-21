using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_MorphData : BinarySerializable
    {
		public byte Target { get; set; }
		public byte MorphingAmount { get; set; }
        public ushort ChannelIndex { get; set; }
		public ushort FrameIndex { get; set; }

		public ushort Align { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			Target = s.Serialize<byte>(Target, name: nameof(Target));
			MorphingAmount = s.Serialize<byte>(MorphingAmount, name: nameof(MorphingAmount));
			ChannelIndex = s.Serialize<ushort>(ChannelIndex, name: nameof(ChannelIndex));
			FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));

			Align = s.Serialize<ushort>(Align, name: nameof(Align));
		}
    }
}