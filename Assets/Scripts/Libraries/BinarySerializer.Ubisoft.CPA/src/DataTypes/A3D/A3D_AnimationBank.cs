namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_AnimationBank : BinarySerializable {
		public A3D_StackInfos[] Infos { get; set; }
		public Pointer<A3D_General[]> A3DGeneral { get; set; }
		public Pointer<A3D_Vector[]> Vectors { get; set; }
		public Pointer<A3D_Quaternion[]> Quaternions { get; set; }
		public Pointer<A3D_Hierarchy[]> Hierarchies { get; set; }
		public Pointer<A3D_NTTO[]> NTTO { get; set; }
		public Pointer<A3D_OnlyFrame[]> OnlyFrames { get; set; }
		public Pointer<A3D_Channel[]> Channels { get; set; }
		public Pointer<A3D_Frame[]> Frames { get; set; }
		public Pointer<A3D_FrameKF[]> FramesKF { get; set; }
		public Pointer<A3D_KeyFrame[]> KeyFrames { get; set; }
		public Pointer<A3D_Event[]> Events { get; set; }
		public Pointer<A3D_MorphData[]> MorphData { get; set; }
		public Pointer<A3D_Deformation[]> Deformations { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Infos = s.SerializeObjectArray<A3D_StackInfos>(Infos, s.GetCPASettings().HasDeformations ? 13 : 12, name: nameof(Infos));
		}

		public void SerializeData(SerializerObject s, bool append = false) {
			A3DGeneral = s.SerializePointer<A3D_General[]>(A3DGeneral, name: nameof(A3DGeneral))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.A3dGENERAL].MaxCount(append: append));

			Vectors = s.SerializePointer<A3D_Vector[]>(Vectors, name: nameof(Vectors))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Vectors].MaxCount(append: append));

			Quaternions = s.SerializePointer<A3D_Quaternion[]>(Quaternions, name: nameof(Quaternions))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Quaternions].MaxCount(append: append));

			Hierarchies = s.SerializePointer<A3D_Hierarchy[]>(Hierarchies, name: nameof(Hierarchies))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Hierarchies].MaxCount(append: append));

			NTTO = s.SerializePointer<A3D_NTTO[]>(NTTO, name: nameof(NTTO))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.NTTO].MaxCount(append: append));

			OnlyFrames = s.SerializePointer<A3D_OnlyFrame[]>(OnlyFrames, name: nameof(OnlyFrames))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.OnlyFrames].MaxCount(append: append));

			Channels = s.SerializePointer<A3D_Channel[]>(Channels, name: nameof(Channels))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Channels].MaxCount(append: append));

			Frames = s.SerializePointer<A3D_Frame[]>(Frames, name: nameof(Frames))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Frames].MaxCount(append: append));

			FramesKF = s.SerializePointer<A3D_FrameKF[]>(FramesKF, name: nameof(FramesKF))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.FramesKF].MaxCount(append: append));

			KeyFrames = s.SerializePointer<A3D_KeyFrame[]>(KeyFrames, name: nameof(KeyFrames))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.KeyFrames].MaxCount(append: append));

			Events = s.SerializePointer<A3D_Event[]>(Events, name: nameof(Events))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Events].MaxCount(append: append));

			MorphData = s.SerializePointer<A3D_MorphData[]>(MorphData, name: nameof(MorphData))
				?.ResolveObjectArray(s, Infos[(int)A3D_StackType.MorphData].MaxCount(append: append));

			if (s.GetCPASettings().HasDeformations)
				Deformations = s.SerializePointer<A3D_Deformation[]>(Deformations, name: nameof(Deformations))
					?.ResolveObjectArray(s, Infos[(int)A3D_StackType.Deformations].MaxCount(append: append));
		}
	}
}
