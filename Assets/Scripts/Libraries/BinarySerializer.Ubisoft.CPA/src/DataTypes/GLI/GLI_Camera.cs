namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_Camera : BinarySerializable {
		public int SizeOfThis { get; set; }
		public int TypeOfThis { get; set; }
		public GLI_CameraMode CameraMode { get; set; }
		public MAT_Transformation Matrix { get; set; }
		public float AlphaX { get; set; }
		public float AlphaY { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }
		public float Screen { get; set; }
		public GLI_2DVertex Scale { get; set; }
		public GLI_2DVertex Translation { get; set; }
		public int ViewportInitialHeight { get; set; }
		public int ViewportInitialWidth { get; set; }
		public float XProjectionRatio { get; set; }
		public float YProjectionRatio { get; set; }
		public Plane PlaneLeft { get; set; }
		public Plane PlaneRight { get; set; }
		public Plane PlaneUp { get; set; }
		public Plane PlaneDown { get; set; }
		public float Ratio { get; set; }

		public byte CameraTransparency { get; set; }
		public float TransparencyDistance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2))
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				SizeOfThis = s.Serialize<int>(SizeOfThis, name: nameof(SizeOfThis));
				TypeOfThis = s.Serialize<int>(TypeOfThis, name: nameof(TypeOfThis));
			}
			CameraMode = s.Serialize<GLI_CameraMode>(CameraMode, name: nameof(CameraMode));

			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");

			Matrix = s.SerializeObject<MAT_Transformation>(Matrix, name: nameof(Matrix));
			AlphaX = s.Serialize<float>(AlphaX, name: nameof(AlphaX));
			AlphaY = s.Serialize<float>(AlphaY, name: nameof(AlphaY));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Screen = s.Serialize<float>(Screen, name: nameof(Screen));
			Scale = s.SerializeObject<GLI_2DVertex>(Scale, name: nameof(Scale));
			Translation = s.SerializeObject<GLI_2DVertex>(Translation, name: nameof(Translation));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				XProjectionRatio = s.Serialize<float>(XProjectionRatio, name: nameof(XProjectionRatio));
				YProjectionRatio = s.Serialize<float>(YProjectionRatio, name: nameof(YProjectionRatio));
			} else {
				ViewportInitialHeight = s.Serialize<int>(ViewportInitialHeight, name: nameof(ViewportInitialHeight));
				ViewportInitialWidth = s.Serialize<int>(ViewportInitialWidth, name: nameof(ViewportInitialWidth));
			}
			PlaneLeft = s.SerializeObject<Plane>(PlaneLeft, name: nameof(PlaneLeft));
			PlaneRight = s.SerializeObject<Plane>(PlaneRight, name: nameof(PlaneRight));
			PlaneUp = s.SerializeObject<Plane>(PlaneUp, name: nameof(PlaneUp));
			PlaneDown = s.SerializeObject<Plane>(PlaneDown, name: nameof(PlaneDown));
			Ratio = s.Serialize<float>(Ratio, name: nameof(Ratio));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				CameraTransparency = s.Serialize<byte>(CameraTransparency, name: nameof(CameraTransparency));
				s.Align(4, Offset);
				TransparencyDistance = s.Serialize<float>(TransparencyDistance, name: nameof(TransparencyDistance));
				throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
			}
		}

		public class Plane : BinarySerializable {
			public MTH3D_Vector Norm { get; set; }
			public float Distance { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Norm = s.SerializeObject<MTH3D_Vector>(Norm, name: nameof(Norm));
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
			}
		}
	}
}
