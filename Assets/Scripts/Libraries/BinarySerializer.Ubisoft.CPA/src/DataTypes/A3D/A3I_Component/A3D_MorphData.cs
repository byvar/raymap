using System;

namespace BinarySerializer.Ubisoft.CPA
{
    public class A3D_MorphData : BinarySerializable
    {
		public byte Target { get; set; }
		public byte MorphingAmount { get; set; }
        public ushort ChannelIndex { get; set; }
		public ushort FrameIndex { get; set; }

		// CPA_3
		public byte TargetsCount { get; set; }
		public short[] Targets { get; set; }
		public byte[] MorphingAmounts { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Target = s.Serialize<byte>(Target, name: nameof(Target));
				MorphingAmount = s.Serialize<byte>(MorphingAmount, name: nameof(MorphingAmount));
			}
			ChannelIndex = s.Serialize<ushort>(ChannelIndex, name: nameof(ChannelIndex));
			FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				s.Align(4, Offset);
			} else {
				TargetsCount = s.Serialize<byte>(TargetsCount, name: nameof(TargetsCount));
				s.SerializePadding(3, logIfNotNull: true);
				Targets = s.SerializeArray<short>(Targets, 10, name: nameof(Targets));
				MorphingAmounts = s.SerializeArray<byte>(MorphingAmounts, 10, name: nameof(MorphingAmounts));
			}
		}
		public float MorphingAmountFloat => ((float)MorphingAmount) / 100.0f;
		public float GetMorphProgressFloat(int i) {
			if (Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3) && i < MorphingAmounts.Length) {
				return ((float)MorphingAmounts[i]) / 100.0f;
			} else {
				return MorphingAmountFloat;
			}
		}
	}
}