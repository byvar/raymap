namespace BinarySerializer.Ubisoft.CPA {
	/// <summary>
	/// A3B: Binary A3D, only frames with no keyframes
	/// </summary>
	public class A3D_Animation_A3B : A3D_Animation {
		public Pointer<A3B_Frame[]> Frames { get; set; }

		public MAT_Transformation OffsetMatrix { get; set; }
		public float Pad { get; set; }
		public ushort MorphsCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.PS2) {
				OffsetMatrix = s.SerializeObject<MAT_Transformation>(OffsetMatrix, name: nameof(OffsetMatrix));
				Pad = s.Serialize<float>(Pad, name: nameof(Pad));
			}

			if (s.GetCPASettings().HasNames)
				AnimationName = s.SerializeString(AnimationName, length: 80, name: nameof(AnimationName));

			Frames = s.SerializePointer<A3B_Frame[]>(Frames, name: nameof(Frames));

			FramesCount = s.Serialize<byte>((byte)FramesCount, name: nameof(FramesCount));
			FrameRate = s.Serialize<byte>(FrameRate, name: nameof(FrameRate));
			MaxElementsCount = s.Serialize<byte>(MaxElementsCount, name: nameof(MaxElementsCount));
			s.SerializePadding(1, logIfNotNull: true);

			Events = s.SerializePointer<A3D_Event[]>(Events, name: nameof(Events));
			MorphData = s.SerializePointer<A3D_MorphData[]>(MorphData, name: nameof(MorphData));
			EventsCount = s.Serialize<byte>(EventsCount, name: nameof(EventsCount));
			s.Align(2, Offset);
			MorphsCount = s.Serialize<ushort>(MorphsCount, name: nameof(MorphsCount));

			if (s.GetCPASettings().Platform != Platform.PS2) {
				OffsetMatrix = s.SerializeObject<MAT_Transformation>(OffsetMatrix, name: nameof(OffsetMatrix));
			}

			IsCompressedAnimation = s.Serialize<int>(IsCompressedAnimation, name: nameof(IsCompressedAnimation));
			AnimationLights = s.SerializePointer(AnimationLights, name: nameof(AnimationLights));

			Frames?.ResolveObjectArray(s, FramesCount, onPreSerialize: (f,i) => f.Pre_ElementsCount = MaxElementsCount);
			Events?.ResolveObjectArray(s, EventsCount);
			MorphData?.ResolveObjectArray(s, MorphsCount);
		}
	}
}
