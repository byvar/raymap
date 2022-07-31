namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_IndexedSprite : BinarySerializable {
		public Pointer<GEO_Sprite> Sprite { get; set; }
		public MTH2D_Vector SpriteSize { get; set; }
		public MTH3D_Vector Axis { get; set; }
		public GEO_UV UVPosition { get; set; }
		public GEO_UV UVSize { get; set; }
		public ushort CenterPoint { get; set; }

		// No complex sprite
		public GEO_SimpleSprite SimpleSprite { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				// No complex sprite
				SimpleSprite = s.SerializeObject<GEO_SimpleSprite>(SimpleSprite, name: nameof(SimpleSprite));
			} else {
				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					CenterPoint = s.Serialize<ushort>(CenterPoint, name: nameof(CenterPoint));
					s.Align(4, Offset);
				}
				Sprite = s.SerializePointer<GEO_Sprite>(Sprite, name: nameof(Sprite))?.ResolveObject(s);
				SpriteSize = s.SerializeObject<MTH2D_Vector>(SpriteSize, name: nameof(SpriteSize));
				Axis = s.SerializeObject<MTH3D_Vector>(Axis, name: nameof(Axis));
				UVPosition = s.SerializeObject<GEO_UV>(UVPosition, name: nameof(UVPosition));
				UVSize = s.SerializeObject<GEO_UV>(UVSize, name: nameof(UVSize));
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					CenterPoint = s.Serialize<ushort>(CenterPoint, name: nameof(CenterPoint));
					s.Align(4, Offset);
				}
			}
		}
	}
}
