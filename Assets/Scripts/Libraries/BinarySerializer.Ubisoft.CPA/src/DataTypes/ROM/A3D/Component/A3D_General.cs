using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_General : BinarySerializable
    {
        public ushort AnimationSpeed { get; set; }

        public ushort VectorsCount { get; set; }
        public ushort QuaternionsCount { get; set; }
        public ushort HierarchiesCount { get; set; }
        public ushort NTTOCount { get; set; }
        public ushort SavedFramesCount { get; set; } // NumNTTO

        public ushort ChannelsCount { get; set; }
        public ushort FramesCount { get; set; }
        public ushort KeyFramesCount { get; set; }
        public ushort EventsCount { get; set; }
        public ushort FirstPositionCount { get; set; }

        public ushort StartFrame { get; set; }
        public ushort EndFrame { get; set; }
        public ushort FakeAnimSpeed { get; set; }
        public ushort Flags { get; set; }

        public ushort NumOfAnimationTranslationOffset { get; set; }
        public ushort NumOfAnimationRotationOffset { get; set; }

        public ushort MorphDataCount { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
			AnimationSpeed = s.Serialize<ushort>(AnimationSpeed, name: nameof(AnimationSpeed));

			VectorsCount = s.Serialize<ushort>(VectorsCount, name: nameof(VectorsCount));
			QuaternionsCount = s.Serialize<ushort>(QuaternionsCount, name: nameof(QuaternionsCount));
			HierarchiesCount = s.Serialize<ushort>(HierarchiesCount, name: nameof(HierarchiesCount));
			NTTOCount = s.Serialize<ushort>(NTTOCount, name: nameof(NTTOCount));
			SavedFramesCount = s.Serialize<ushort>(SavedFramesCount, name: nameof(SavedFramesCount));

			ChannelsCount = s.Serialize<ushort>(ChannelsCount, name: nameof(ChannelsCount));
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			KeyFramesCount = s.Serialize<ushort>(KeyFramesCount, name: nameof(KeyFramesCount));
			EventsCount = s.Serialize<ushort>(EventsCount, name: nameof(EventsCount));
			FirstPositionCount = s.Serialize<ushort>(FirstPositionCount, name: nameof(FirstPositionCount));

			StartFrame = s.Serialize<ushort>(StartFrame, name: nameof(StartFrame));
			EndFrame = s.Serialize<ushort>(EndFrame, name: nameof(EndFrame));
			FakeAnimSpeed = s.Serialize<ushort>(FakeAnimSpeed, name: nameof(FakeAnimSpeed));
			Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));

			NumOfAnimationTranslationOffset = s.Serialize<ushort>(NumOfAnimationTranslationOffset, name: nameof(NumOfAnimationTranslationOffset));
			NumOfAnimationRotationOffset = s.Serialize<ushort>(NumOfAnimationRotationOffset, name: nameof(NumOfAnimationRotationOffset));
			
            MorphDataCount = s.Serialize<ushort>(MorphDataCount, name: nameof(MorphDataCount));
		}
    }
}