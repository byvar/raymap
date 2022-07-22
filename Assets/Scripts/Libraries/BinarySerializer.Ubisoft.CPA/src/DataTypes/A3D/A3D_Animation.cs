namespace BinarySerializer.Ubisoft.CPA {
	public abstract class A3D_Animation : BinarySerializable {
		public string AnimationName { get; set; }

		public ushort FramesCount { get; set; }
		public byte FrameRate { get; set; }
		public byte MaxElementsCount { get; set; } // Total channels count for this animation

		public Pointer<A3D_Event[]> Events { get; set; }
		public Pointer<A3D_MorphData[]> MorphData { get; set; }
		public byte EventsCount { get; set; }


		public int IsCompressedAnimation { get; set; }
		public Pointer AnimationLights { get; set; } // TODO: AnimLights
	}
}
