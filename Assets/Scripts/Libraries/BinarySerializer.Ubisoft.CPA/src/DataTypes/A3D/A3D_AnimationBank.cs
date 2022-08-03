namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_AnimationBank : BinarySerializable {
		public A3D_StackInfos[] Infos { get; set; }
		public Pointer[] StackPointers { get; set; }

		public A3D_General[] A3DGeneral { get; set; }
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
			Infos = s.SerializeObjectArray<A3D_StackInfos>(Infos, s.GetCPASettings().HasDeformations ? 13 : 12, name: nameof(Infos));
		}

		public void SerializeData(SerializerObject s, SerializeMode mode, bool append = false) {
			if(mode == SerializeMode.Pointer)
				StackPointers = s.SerializePointerArray(StackPointers, Infos.Length, name: nameof(StackPointers));

			bool align = !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3);
			var alignOffset = s.CurrentPointer;

			T[] SerializeFromInfo<T>(T[] obj, A3D_StackType type, bool align, string name = null) where T : BinarySerializable, new() {
				int stackIndex = (int)type;
				T[] newObj = obj;
				if (mode == SerializeMode.Pointer) {
					s.DoAt(StackPointers[stackIndex], () => {
						newObj = s.SerializeObjectArray<T>(obj, Infos[stackIndex].Count(append: append), name: name);
					});
				} else {
					if(align) s.Align(4, alignOffset);
					newObj = s.SerializeObjectArray<T>(obj, Infos[stackIndex].Count(append: append), name: name);
				}
				return newObj;
			}

			A3DGeneral = SerializeFromInfo<A3D_General>(A3DGeneral, A3D_StackType.A3dGENERAL, align, name: nameof(A3DGeneral));
			Vectors = SerializeFromInfo<A3D_Vector>(Vectors, A3D_StackType.Vectors, align, name: nameof(Vectors));
			Quaternions = SerializeFromInfo<A3D_Quaternion>(Quaternions, A3D_StackType.Quaternions, align, name: nameof(Quaternions));
			Hierarchies = SerializeFromInfo<A3D_Hierarchy>(Hierarchies, A3D_StackType.Hierarchies, align, name: nameof(Hierarchies));
			NTTO = SerializeFromInfo<A3D_NTTO>(NTTO, A3D_StackType.NTTO, align, name: nameof(NTTO));
			OnlyFrames = SerializeFromInfo<A3D_OnlyFrame>(OnlyFrames, A3D_StackType.OnlyFrames, align, name: nameof(OnlyFrames));
			Channels = SerializeFromInfo<A3D_Channel>(Channels, A3D_StackType.Channels, align, name: nameof(Channels));
			Frames = SerializeFromInfo<A3D_Frame>(Frames, A3D_StackType.Frames, align, name: nameof(Frames));
			FramesKF = SerializeFromInfo<A3D_FrameKF>(FramesKF, A3D_StackType.FramesKF, align, name: nameof(FramesKF));
			KeyFrames = SerializeFromInfo<A3D_KeyFrame>(KeyFrames, A3D_StackType.KeyFrames, align, name: nameof(KeyFrames));
			Events = SerializeFromInfo<A3D_Event>(Events, A3D_StackType.Events, align, name: nameof(Events));
			MorphData = SerializeFromInfo<A3D_MorphData>(MorphData, A3D_StackType.MorphData, align, name: nameof(MorphData));
			
			if (s.GetCPASettings().HasDeformations)
				Deformations = SerializeFromInfo<A3D_Deformation>(Deformations, A3D_StackType.Deformations, align, name: nameof(Deformations));
		}

		public enum SerializeMode {
			Pointer,
			Inline
		}
	}
}
