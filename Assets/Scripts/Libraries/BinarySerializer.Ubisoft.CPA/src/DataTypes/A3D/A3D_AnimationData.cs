namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_AnimationData : BinarySerializable {
		public A3D_General A3DGeneral { get; set; }
		public A3D_Vector[] Vectors { get; set; }
		public A3D_Quaternion[] Quaternions { get; set; }
		public A3D_Hierarchy[] Hierarchies { get; set; }
		public A3D_NTTO[] NTTO { get; set; }
		public A3D_OnlyFrame[] OnlyFrames { get; set; }
		public A3D_Channel[] Channels { get; set; }
		public A3D_Frame[] Frames { get; set; }
		public A3D_FrameKF[] FramesKF { get; set; }
		public A3D_KeyFrame[] KeyFrames { get; set; }
		public A3D_Event[] Events { get; set; }
		public A3D_MorphData[] MorphData { get; set; }
		public A3D_Deformation[] Deformations { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// TODO: Align before each element if necessary
			A3DGeneral = s.SerializeObject<A3D_General>(A3DGeneral, name: nameof(A3DGeneral));
			Vectors = s.SerializeObjectArray<A3D_Vector>(Vectors, A3DGeneral.VectorsCount, name: nameof(Vectors));
			Quaternions = s.SerializeObjectArray<A3D_Quaternion>(Quaternions, A3DGeneral.QuaternionsCount, name: nameof(Quaternions));
			Hierarchies = s.SerializeObjectArray<A3D_Hierarchy>(Hierarchies, A3DGeneral.HierarchiesCount, name: nameof(Hierarchies));
			NTTO = s.SerializeObjectArray<A3D_NTTO>(NTTO, A3DGeneral.NTTOCount, name: nameof(NTTO));
			OnlyFrames = s.SerializeObjectArray<A3D_OnlyFrame>(OnlyFrames, A3DGeneral.FramesCount, name: nameof(OnlyFrames));
			Channels = s.SerializeObjectArray<A3D_Channel>(Channels, A3DGeneral.ChannelsCount, name: nameof(Channels));
			Frames = s.SerializeObjectArray<A3D_Frame>(Frames, A3DGeneral.SavedFramesCount * A3DGeneral.ChannelsCount, name: nameof(Frames));
			FramesKF = s.SerializeObjectArray<A3D_FrameKF>(FramesKF, A3DGeneral.FramesCount * A3DGeneral.ChannelsCount, name: nameof(FramesKF));
			KeyFrames = s.SerializeObjectArray<A3D_KeyFrame>(KeyFrames, A3DGeneral.KeyFramesCount, name: nameof(KeyFrames));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				Events = s.SerializeObjectArray<A3D_Event>(Events, A3DGeneral.EventsCount, name: nameof(Events));
				MorphData = s.SerializeObjectArray<A3D_MorphData>(MorphData, A3DGeneral.MorphDataCount, name: nameof(MorphData));
				
				if (s.GetCPASettings().HasDeformations) {
					Deformations = s.SerializeObjectArray<A3D_Deformation>(Deformations, A3DGeneral.DeformationsCount, name: nameof(Deformations));
				}
			}
		}
	}
}
