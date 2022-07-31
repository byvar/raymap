namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_ElementSprites : GEO_Element {
		// Complex sprite
		public Pointer<GEO_IndexedSprite[]> Sprites { get; set; }
		public ushort SpritesCount { get; set; }
		public short ParallelBoxIndex { get; set; }
		public bool FastDraw { get; set; }

		// No complex sprite
		public GEO_SimpleSprite SimpleSprite { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Platform == Platform.DC) {
				// No complex sprite
				SimpleSprite = s.SerializeObject<GEO_SimpleSprite>(SimpleSprite, name: nameof(SimpleSprite));
			} else {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
					Sprites = s.SerializePointer<GEO_IndexedSprite[]>(Sprites, name: nameof(Sprites));
					SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
					ParallelBoxIndex = s.Serialize<short>(ParallelBoxIndex, name: nameof(ParallelBoxIndex));
					if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
						FastDraw = s.Serialize<bool>(FastDraw, name: nameof(FastDraw));
					}
				} else {
					SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
					s.Align(4, Offset);
					Sprites = s.SerializePointer<GEO_IndexedSprite[]>(Sprites, name: nameof(Sprites));
				}
				s.Align(4, Offset);
				Sprites?.ResolveObjectArray(s, SpritesCount);
			}
		}
	}
}
