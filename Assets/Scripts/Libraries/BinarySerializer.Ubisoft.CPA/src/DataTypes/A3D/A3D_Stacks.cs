namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Stacks : BinarySerializable {
		public A3D_StackInfos[] Infos { get; set; }
		public Pointer A3DGeneral { get; set; }
		public Pointer Vectors { get; set; }
		public Pointer Quaternions { get; set; }
		public Pointer Hierarchies { get; set; }
		public Pointer NTTO { get; set; }
		public Pointer OnlyFrames { get; set; }
		public Pointer Channels { get; set; }
		public Pointer Frames { get; set; }
		public Pointer FramesKF { get; set; }
		public Pointer KeyFrames { get; set; }
		public Pointer Events { get; set; }
		public Pointer MorphData { get; set; }
		public Pointer Deformations { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Infos = s.SerializeObjectArray<A3D_StackInfos>(Infos, s.GetCPASettings().HasDeformations ? 13 : 12, name: nameof(Infos));
			A3DGeneral = s.SerializePointer(A3DGeneral, name: nameof(A3DGeneral));
			Vectors = s.SerializePointer(Vectors, name: nameof(Vectors));
			Quaternions = s.SerializePointer(Quaternions, name: nameof(Quaternions));
			Hierarchies = s.SerializePointer(Hierarchies, name: nameof(Hierarchies));
			NTTO = s.SerializePointer(NTTO, name: nameof(NTTO));
			OnlyFrames = s.SerializePointer(OnlyFrames, name: nameof(OnlyFrames));
			Channels = s.SerializePointer(Channels, name: nameof(Channels));
			Frames = s.SerializePointer(Frames, name: nameof(Frames));
			FramesKF = s.SerializePointer(FramesKF, name: nameof(FramesKF));
			KeyFrames = s.SerializePointer(KeyFrames, name: nameof(KeyFrames));
			Events = s.SerializePointer(Events, name: nameof(Events));
			MorphData = s.SerializePointer(MorphData, name: nameof(MorphData));
			if(s.GetCPASettings().HasDeformations)
				Deformations = s.SerializePointer(Deformations, name: nameof(Deformations));
		}
	}
}
