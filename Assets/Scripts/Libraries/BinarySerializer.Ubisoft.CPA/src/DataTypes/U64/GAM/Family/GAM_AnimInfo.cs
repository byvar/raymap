using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GAM_AnimInfo : U64_Struct {
        public U64_Reference<U64_Placeholder> AnimListTable { get; set; }
        public ushort ChannelsCount { get; set; }
        public ushort FramesCount { get; set; }
        public ushort InterpolatedAnimationIndex { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            AnimListTable = s.SerializeObject<U64_Reference<U64_Placeholder>>(AnimListTable, name: nameof(AnimListTable));
            ChannelsCount = s.Serialize<ushort>(ChannelsCount, name: nameof(ChannelsCount));
            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            InterpolatedAnimationIndex = s.Serialize<ushort>(InterpolatedAnimationIndex, name: nameof(InterpolatedAnimationIndex)); // Index in CutTable

            s.GetLoader().LoadInterpolatedAnimation(InterpolatedAnimationIndex);
        }
    }
}
