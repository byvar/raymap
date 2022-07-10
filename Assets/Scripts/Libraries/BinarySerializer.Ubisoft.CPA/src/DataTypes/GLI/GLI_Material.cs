namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_Material : BinarySerializable {
		public GLI_DrawMask MaterialType { get; set; }
		public GLI_FloatColor Ambient { get; set; }
		public GLI_FloatColor Diffuse { get; set; }
		public GLI_FloatColor Specular { get; set; }
		public GLI_FloatColor Color { get; set; }
		public uint MaterialAdditionalType { get; set; }
		public int SpecularExponent { get; set; }
		public Pointer<GLI_Texture> Texture { get; set; }
		
		// Scrolling material
		public MTH2D_Vector AddUV { get; set; }
		public MTH2D_Vector ConstantAddUV { get; set; }
		public int IncrementIsEnabled { get; set; } //0: No scroll, 1: Continue scrolling, 2: set value, 3: Pause scroll at current value,

		// Animated textures
		public uint ActualRefreshNumber { get; set; }
		public Pointer<GLI_AnimatedTextureNode> FirstAnimatedTextureNode { get; set; }
		public Pointer<GLI_AnimatedTextureNode> CurrentAnimatedTextureNode { get; set; }
		public int DisplayNodesCount { get; set; }
		public float CurrentDisplayTimeSinceStartOfLastTexture { get; set; }
		public bool IsLocked { get; set; }

		// Multitexture
		public uint Flags { get; set; }
		public uint MultiTextureType { get; set; }
		public uint TextureStagesCount { get; set; }
		public GLI_MultiTextureMaterial[] MultiMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MaterialType = s.Serialize<GLI_DrawMask>(MaterialType, name: nameof(MaterialType));
			Ambient = s.SerializeObject<GLI_FloatColor>(Ambient, name: nameof(Ambient));
			Diffuse = s.SerializeObject<GLI_FloatColor>(Diffuse, name: nameof(Diffuse));
			Specular = s.SerializeObject<GLI_FloatColor>(Specular, name: nameof(Specular));
			Color = s.SerializeObject<GLI_FloatColor>(Color, name: nameof(Color));

			if(s.GetCPASettings().EngineVersion == EngineVersion.Rayman3 && s.GetCPASettings().Platform == Platform.PS2)
				MaterialAdditionalType = s.Serialize<uint>(MaterialAdditionalType, name: nameof(MaterialAdditionalType));

			SpecularExponent = s.Serialize<int>(SpecularExponent, name: nameof(SpecularExponent));

			Texture = s.SerializePointer<GLI_Texture>(Texture, name: nameof(Texture))?.ResolveObject(s);

			// Scrolling material
			AddUV = s.SerializeObject<MTH2D_Vector>(AddUV, name: nameof(AddUV));
			ConstantAddUV = s.SerializeObject<MTH2D_Vector>(ConstantAddUV, name: nameof(ConstantAddUV));
			IncrementIsEnabled = s.Serialize<int>(IncrementIsEnabled, name: nameof(IncrementIsEnabled));

			// Animated textures
			ActualRefreshNumber = s.Serialize<uint>(ActualRefreshNumber, name: nameof(ActualRefreshNumber));
			FirstAnimatedTextureNode = s.SerializePointer<GLI_AnimatedTextureNode>(FirstAnimatedTextureNode, name: nameof(FirstAnimatedTextureNode))?.ResolveObject(s);
			CurrentAnimatedTextureNode = s.SerializePointer<GLI_AnimatedTextureNode>(CurrentAnimatedTextureNode, name: nameof(CurrentAnimatedTextureNode))?.ResolveObject(s);
			DisplayNodesCount = s.Serialize<int>(DisplayNodesCount, name: nameof(DisplayNodesCount));
			CurrentDisplayTimeSinceStartOfLastTexture = s.Serialize<float>(CurrentDisplayTimeSinceStartOfLastTexture, name: nameof(CurrentDisplayTimeSinceStartOfLastTexture));
			IsLocked = s.Serialize<bool>(IsLocked, name: nameof(IsLocked));
			s.Align(4, Offset);

			// Multitexture
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
				MultiTextureType = s.Serialize<uint>(MultiTextureType, name: nameof(MultiTextureType));
				TextureStagesCount = s.Serialize<uint>(TextureStagesCount, name: nameof(TextureStagesCount));
				MultiMaterial = s.SerializeObjectArray<GLI_MultiTextureMaterial>(MultiMaterial, 4, name: nameof(MultiMaterial));
			}
		}
	}
}
