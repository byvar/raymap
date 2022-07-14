using System;

namespace BinarySerializer.Ubisoft.CPA
{
    public class A3D_General : BinarySerializable
    {
        public ushort AnimationSpeed { get; set; }

        public ushort VectorsCount { get; set; }
        public ushort QuaternionsCount { get; set; }
        public ushort HierarchiesCount { get; set; }
        public ushort NTTOCount { get; set; }
        public ushort SavedFramesCount { get; set; } // NumNTTO
		public ushort DeformationsCount { get; set; }

        public ushort ChannelsCount { get; set; }
        public ushort FramesCount { get; set; }
        public uint KeyFramesCount { get; set; }
        public ushort EventsCount { get; set; }
        public ushort FirstPositionIndex { get; set; } // Vectors[FirstPositionIndex] to Vectors[FirstVectorIndex + VectorsCount] are positions and should be normalized

        public ushort StartFrame { get; set; }
        public ushort EndFrame { get; set; }
        public ushort FakeAnimSpeed { get; set; }
        public ushort Flags { get; set; }

        public ushort AnimationTranslationOffsetIndex { get; set; } // Vector index
        public ushort AnimationRotationOffsetIndex { get; set; } // Quaternion index

        public ushort MorphDataCount { get; set; }

		public ushort FirstVectorIndex { get; set; }
		public ushort FirstQuaternionIndex { get; set; }
		public ushort FirstHierarchyIndex { get; set; }
		public ushort FirstNTTOIndex { get; set; }
		public ushort FirstDeformationIndex { get; set; }
		public ushort FirstOnlyFrameIndex { get; set; }
		public ushort FirstChannelIndex { get; set; }
		public ushort FirstEventIndex { get; set; }
		public ushort FirstMorphDataIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AnimationSpeed = s.Serialize<ushort>(AnimationSpeed, name: nameof(AnimationSpeed));

			VectorsCount = s.Serialize<ushort>(VectorsCount, name: nameof(VectorsCount));
			QuaternionsCount = s.Serialize<ushort>(QuaternionsCount, name: nameof(QuaternionsCount));
			HierarchiesCount = s.Serialize<ushort>(HierarchiesCount, name: nameof(HierarchiesCount));
			NTTOCount = s.Serialize<ushort>(NTTOCount, name: nameof(NTTOCount));
			SavedFramesCount = s.Serialize<ushort>(SavedFramesCount, name: nameof(SavedFramesCount));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				DeformationsCount = s.Serialize<ushort>(DeformationsCount, name: nameof(DeformationsCount));
			}

			ChannelsCount = s.Serialize<ushort>(ChannelsCount, name: nameof(ChannelsCount));
			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2) && s.GetCPASettings().Branch != EngineBranch.U64) {
				KeyFramesCount = s.Serialize<uint>(KeyFramesCount, name: nameof(KeyFramesCount));
			} else {
				KeyFramesCount = s.Serialize<ushort>((ushort)KeyFramesCount, name: nameof(KeyFramesCount));
			}
			EventsCount = s.Serialize<ushort>(EventsCount, name: nameof(EventsCount));
			FirstPositionIndex = s.Serialize<ushort>(FirstPositionIndex, name: nameof(FirstPositionIndex));

			StartFrame = s.Serialize<ushort>(StartFrame, name: nameof(StartFrame));
			EndFrame = s.Serialize<ushort>(EndFrame, name: nameof(EndFrame));
			FakeAnimSpeed = s.Serialize<ushort>(FakeAnimSpeed, name: nameof(FakeAnimSpeed));
			Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));

			AnimationTranslationOffsetIndex = s.Serialize<ushort>(AnimationTranslationOffsetIndex, name: nameof(AnimationTranslationOffsetIndex));
			AnimationRotationOffsetIndex = s.Serialize<ushort>(AnimationRotationOffsetIndex, name: nameof(AnimationRotationOffsetIndex));

			MorphDataCount = s.Serialize<ushort>(MorphDataCount, name: nameof(MorphDataCount));

			if (s.GetCPASettings().Branch != EngineBranch.U64) {
				FirstVectorIndex = s.Serialize<ushort>(FirstVectorIndex, name: nameof(FirstVectorIndex));
				FirstQuaternionIndex = s.Serialize<ushort>(FirstQuaternionIndex, name: nameof(FirstQuaternionIndex));
				FirstHierarchyIndex = s.Serialize<ushort>(FirstHierarchyIndex, name: nameof(FirstHierarchyIndex));
				FirstNTTOIndex = s.Serialize<ushort>(FirstNTTOIndex, name: nameof(FirstNTTOIndex));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					FirstDeformationIndex = s.Serialize<ushort>(FirstDeformationIndex, name: nameof(FirstDeformationIndex));
				}
				FirstOnlyFrameIndex = s.Serialize<ushort>(FirstOnlyFrameIndex, name: nameof(FirstOnlyFrameIndex));
				FirstChannelIndex = s.Serialize<ushort>(FirstChannelIndex, name: nameof(FirstChannelIndex));
				FirstEventIndex = s.Serialize<ushort>(FirstEventIndex, name: nameof(FirstEventIndex));
				FirstMorphDataIndex = s.Serialize<ushort>(FirstMorphDataIndex, name: nameof(FirstMorphDataIndex));

				s.Align(4, Offset);
			}
		}
    }
}