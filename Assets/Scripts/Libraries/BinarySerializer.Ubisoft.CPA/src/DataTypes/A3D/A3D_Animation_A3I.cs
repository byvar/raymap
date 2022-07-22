namespace BinarySerializer.Ubisoft.CPA {
	/// <summary>
	/// A3I: Interpolated keyframes
	/// </summary>
	public class A3D_Animation_A3I : A3D_Animation {
		public Pointer<A3D_AnimationData> AnimationData { get; set; }

		public MTH3D_Vector OffsetVector { get; set; }

		public ushort A3DGeneralIndex { get; set; }
		public byte FlagMergeAnimation { get; set; }

		public override void SerializeImpl(SerializerObject s) {

			if (s.GetCPASettings().HasNames)
				AnimationName = s.SerializeString(AnimationName, length: 80, name: nameof(AnimationName));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution))
				AnimationData = s.SerializePointer<A3D_AnimationData>(AnimationData, name: nameof(AnimationData))?.ResolveObject(s);

			FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
			FrameRate = s.Serialize<byte>(FrameRate, name: nameof(FrameRate));
			MaxElementsCount = s.Serialize<byte>(MaxElementsCount, name: nameof(MaxElementsCount));

			Events = s.SerializePointer<A3D_Event[]>(Events, name: nameof(Events));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				&& !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.LargoWinch)) {
				OffsetVector = s.SerializeObject<MTH3D_Vector>(OffsetVector, name: nameof(OffsetVector));
			}
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				IsCompressedAnimation = s.Serialize<int>(IsCompressedAnimation, name: nameof(IsCompressedAnimation));
				AnimationLights = s.SerializePointer(AnimationLights, name: nameof(AnimationLights));
			}

			MorphData = s.SerializePointer<A3D_MorphData[]>(MorphData, name: nameof(MorphData)); // Can't resolve, the count is in the A3DGeneral
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.LargoWinch)) {
				EventsCount = s.Serialize<byte>(EventsCount, name: nameof(EventsCount));
				s.Align(2, Offset);
				A3DGeneralIndex = s.Serialize<ushort>(A3DGeneralIndex, name: nameof(A3DGeneralIndex));
			} else {
				A3DGeneralIndex = s.Serialize<ushort>(A3DGeneralIndex, name: nameof(A3DGeneralIndex));
				EventsCount = s.Serialize<byte>(EventsCount, name: nameof(EventsCount));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					FlagMergeAnimation = s.Serialize<byte>(FlagMergeAnimation, name: nameof(FlagMergeAnimation));
				}
			}
			s.Align(4, Offset);
			
			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				AnimationData = s.SerializePointer<A3D_AnimationData>(AnimationData, name: nameof(AnimationData))?.ResolveObject(s);

			Events?.ResolveObjectArray(s, EventsCount);
		}
	}
}
