namespace BinarySerializer.Ubisoft.CPA {
	public class GEO_Sprite : BinarySerializable {
		public ushort SpritesCount { get; set; }
		public Pointer<GEO_SpriteDrawMode[]> DrawModes { get; set; }
		public Pointer<float[]> Thresholds { get; set; }
		public Pointer<MTH2D_Vector[]> SpriteSizes { get; set; }
		public Pointer<MTH2D_Vector[]> SpriteDisplacements { get; set; }
		public Pointer<Pointer<GMT_GameMaterial>[]> Material { get; set; }
		public Pointer<Pointer<GLI_Material>[]> VisualMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SpritesCount = s.Serialize<ushort>(SpritesCount, name: nameof(SpritesCount));
			s.Align(4, Offset);
			DrawModes = s.SerializePointer<GEO_SpriteDrawMode[]>(DrawModes, name: nameof(DrawModes))?.ResolveArray(s, SpritesCount);
			Thresholds = s.SerializePointer<float[]>(Thresholds, name: nameof(Thresholds))?.ResolveArray(s, SpritesCount);
			SpriteSizes = s.SerializePointer<MTH2D_Vector[]>(SpriteSizes, name: nameof(SpriteSizes))?.ResolveObjectArray(s, SpritesCount);
			SpriteDisplacements = s.SerializePointer<MTH2D_Vector[]>(SpriteDisplacements, name: nameof(SpriteDisplacements))?.ResolveObjectArray(s, SpritesCount);
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				VisualMaterial = s.SerializePointer<Pointer<GLI_Material>[]>(VisualMaterial, name: nameof(VisualMaterial))?.ResolvePointerArray(s, SpritesCount);
				VisualMaterial?.Value?.ResolveObject(s);
			} else {
				Material = s.SerializePointer<Pointer<GMT_GameMaterial>[]>(Material, name: nameof(Material))?.ResolvePointerArray(s, SpritesCount);
				Material?.Value?.ResolveObject(s);
			}
		}
	}
}
