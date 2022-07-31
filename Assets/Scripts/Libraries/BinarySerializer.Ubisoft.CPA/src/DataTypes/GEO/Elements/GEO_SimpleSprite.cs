namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_SimpleSprite : BinarySerializable {
		// No complex sprite
		public Pointer<GLI_Material> VisualMaterial { get; set; }
		public MTH2D_Vector SpriteSize { get; set; }
		public short ParallelBoxIndex { get; set; }
		public ushort CenterPoint { get; set; }
		public GEO_SpriteDrawMode DrawMode { get; set; }

		public uint LightCookieIndex { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.DC) {
				// No complex sprite
				VisualMaterial = s.SerializePointer<GLI_Material>(VisualMaterial, name: nameof(VisualMaterial))?.ResolveObject(s);
				SpriteSize = s.SerializeObject<MTH2D_Vector>(SpriteSize, name: nameof(SpriteSize));
				ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
				CenterPoint = s.Serialize<ushort>(CenterPoint, name: nameof(CenterPoint));
				DrawMode = s.Serialize<GEO_SpriteDrawMode>(DrawMode, name: nameof(DrawMode));
				s.Align(4, Offset);
			} else if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				DrawMode = s.Serialize<GEO_SpriteDrawMode>(DrawMode, name: nameof(DrawMode));
				s.Align(4, Offset);
				SpriteSize = s.SerializeObject<MTH2D_Vector>(SpriteSize, name: nameof(SpriteSize));
				if (DrawMode == GEO_SpriteDrawMode.RevolutionLightCookie) {
					LightCookieIndex = s.Serialize<uint>(LightCookieIndex, name: nameof(LightCookieIndex));
				} else {
					VisualMaterial = s.SerializePointer<GLI_Material>(VisualMaterial, name: nameof(VisualMaterial))?.ResolveObject(s);
				}
			}
		}
	}
}
