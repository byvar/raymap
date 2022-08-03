namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_Light : BinarySerializable {
		public bool IsOn { get; set; }
		public bool IsZBuffered { get; set; }
		public GLI_LightType Type { get; set; }
		public float Far { get; set; }
		public float Near { get; set; }
		public float LittleAlpha { get; set; }
		public float BigAlpha { get; set; }
		public float LittleTangent { get; set; }
		public float BigTangent { get; set; }
		public MAT_Transformation Matrix { get; set; }
		public GLI_LightZBuffer ZBuffer { get; set; }
		public GLI_FloatColor_RGBA Color { get; set; }
		public bool IsValid { get; set; }
		public GLI_ObjectLighted ObjectLighted { get; set; }
		public bool IsPainting { get; set; }
		public GLI_LightAlpha AlphaType { get; set; }
		public MTH3D_Vector InterMinPos { get; set; }
		public MTH3D_Vector ExterMinPos { get; set; }
		public MTH3D_Vector InterMaxPos { get; set; }
		public MTH3D_Vector ExterMaxPos { get; set; }
		public MTH3D_Vector CenterBox { get; set; }
		public float Radius { get; set; }
		public float IntensityMin { get; set; }
		public float IntensityMax { get; set; }
		public GLI_FloatColor_RGBA BackgroundColor { get; set; }

		// CPA_3
		public float SquareNear { get; set; }
		public float SquareFar { get; set; }
		public float OOSquareDiv { get; set; }
		public bool CreatesShadows { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<uint>(b => {
				IsOn = b.SerializeBits<bool>(IsOn, 1, name: nameof(IsOn));
				b.SerializePadding(31, logIfNotNull: true);
			});
			s.DoBits<uint>(b => {
				IsZBuffered = b.SerializeBits<bool>(IsZBuffered, 1, name: nameof(IsZBuffered));
				b.SerializePadding(31, logIfNotNull: true);
			});
			Type = s.Serialize<GLI_LightType>(Type, name: nameof(Type));
			Far = s.Serialize<float>(Far, name: nameof(Far));
			Near = s.Serialize<float>(Near, name: nameof(Near));
			LittleAlpha = s.Serialize<float>(LittleAlpha, name: nameof(LittleAlpha));
			BigAlpha = s.Serialize<float>(BigAlpha, name: nameof(BigAlpha));
			LittleTangent = s.Serialize<float>(LittleTangent, name: nameof(LittleTangent));
			BigTangent = s.Serialize<float>(BigTangent, name: nameof(BigTangent));
			Matrix = s.SerializeObject<MAT_Transformation>(Matrix, name: nameof(Matrix));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Color = s.SerializeObject<GLI_FloatColor_RGBA>(Color, name: nameof(Color));
				SquareNear = s.Serialize<float>(SquareNear, name: nameof(SquareNear));
				SquareFar = s.Serialize<float>(SquareFar, name: nameof(SquareFar));
				OOSquareDiv = s.Serialize<float>(OOSquareDiv, name: nameof(OOSquareDiv));
				ZBuffer = s.SerializeObject<GLI_LightZBuffer>(ZBuffer, name: nameof(ZBuffer));
			} else {
				ZBuffer = s.SerializeObject<GLI_LightZBuffer>(ZBuffer, name: nameof(ZBuffer));
				Color = s.SerializeObject<GLI_FloatColor_RGBA>(Color, name: nameof(Color));
			}

			IsValid = s.Serialize<bool>(IsValid, name: nameof(IsValid));
			ObjectLighted = s.Serialize<GLI_ObjectLighted>(ObjectLighted, name: nameof(ObjectLighted));
			IsPainting = s.Serialize<bool>(IsPainting, name: nameof(IsPainting));
			AlphaType = s.Serialize<GLI_LightAlpha>(AlphaType, name: nameof(AlphaType));
			InterMinPos = s.SerializeObject<MTH3D_Vector>(InterMinPos, name: nameof(InterMinPos));
			ExterMinPos = s.SerializeObject<MTH3D_Vector>(ExterMinPos, name: nameof(ExterMinPos));
			InterMaxPos = s.SerializeObject<MTH3D_Vector>(InterMaxPos, name: nameof(InterMaxPos));
			ExterMaxPos = s.SerializeObject<MTH3D_Vector>(ExterMaxPos, name: nameof(ExterMaxPos));
			CenterBox = s.SerializeObject<MTH3D_Vector>(CenterBox, name: nameof(CenterBox));
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
			IntensityMin = s.Serialize<float>(IntensityMin, name: nameof(IntensityMin));
			IntensityMax = s.Serialize<float>(IntensityMax, name: nameof(IntensityMax));
			BackgroundColor = s.SerializeObject<GLI_FloatColor_RGBA>(BackgroundColor, name: nameof(BackgroundColor));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				s.DoBits<uint>(b => {
					CreatesShadows = b.SerializeBits<bool>(CreatesShadows, 1, name: nameof(CreatesShadows));
					b.SerializePadding(31, logIfNotNull: true);
				});
				if(s.GetCPASettings().Platform == Platform.PS2)
					s.Align(16, Offset);
			}

		}
	}
}
