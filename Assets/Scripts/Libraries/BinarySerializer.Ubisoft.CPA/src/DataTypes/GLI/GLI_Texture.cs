namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_Texture : BinarySerializable {
		public int IsAvailable { get; set; }
		public uint DinosaurUnknown { get; set; }
		public GLI_TextureQuality TextureQuality { get; set; }
		
		// Texture data
		public Pointer BitMap { get; set; }
		public Pointer ColorTable { get; set; }

		// Parameters specific for graphics card
		public Pointer SpecParam { get; set; }

		public GLI_TextureCaps TextureCaps { get; set; }

		// Texture size after compression
		public ushort Height { get; set; }
		public ushort Width { get; set; }

		// Real size before compression
		public ushort RealHeight { get; set; }
		public ushort RealWidth { get; set; }

		// Scrolling texture
		public MTH2D_Vector AddUV { get; set; }
		public int IncrementIsEnabled { get; set; } // See Material for possible values

		// Chroma key
		public GLI_RGBA8888Color ChromaKeyColor { get; set; }
		public GLI_RGBA8888Color BlendColor { get; set; } // CPA_3 only

		// Mipmapping
		public int LODCount { get; set; }
		public uint CompressionCounter { get; set; }
		public uint CompressionType { get; set; }
		public GLI_MipMappingType MipMappingType { get; set; }
		public Pointer<GLI_Texture> TextureSubstitution { get; set; }

		public GLI_BilinearMode BilinearMode { get; set; }
		public GLI_CyclingMode CyclingMode { get; set; }

		public string FileName { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			IsAvailable = s.Serialize<int>(IsAvailable, name: nameof(IsAvailable));

			if (s.GetCPASettings().EngineVersion == EngineVersion.Dinosaur) 
				DinosaurUnknown = s.Serialize<uint>(DinosaurUnknown, name: nameof(DinosaurUnknown));

			TextureQuality = s.Serialize<GLI_TextureQuality>(TextureQuality, name: nameof(TextureQuality));

			BitMap = s.SerializePointer(BitMap, name: nameof(BitMap));
			ColorTable = s.SerializePointer(ColorTable, name: nameof(ColorTable));
			SpecParam = s.SerializePointer(SpecParam, name: nameof(SpecParam));

			TextureCaps = s.Serialize<GLI_TextureCaps>(TextureCaps, name: nameof(TextureCaps));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			RealHeight = s.Serialize<ushort>(RealHeight, name: nameof(RealHeight));
			RealWidth = s.Serialize<ushort>(RealWidth, name: nameof(RealWidth));

			AddUV = s.SerializeObject<MTH2D_Vector>(AddUV, name: nameof(AddUV));
			IncrementIsEnabled = s.Serialize<int>(IncrementIsEnabled, name: nameof(IncrementIsEnabled));

			ChromaKeyColor = s.SerializeObject<GLI_RGBA8888Color>(ChromaKeyColor, name: nameof(ChromaKeyColor));
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				BlendColor = s.SerializeObject<GLI_RGBA8888Color>(BlendColor, name: nameof(BlendColor));

			LODCount = s.Serialize<int>(LODCount, name: nameof(LODCount));
			CompressionCounter = s.Serialize<uint>(CompressionCounter, name: nameof(CompressionCounter));
			CompressionType = s.Serialize<uint>(CompressionType, name: nameof(CompressionType));
			MipMappingType = s.Serialize<GLI_MipMappingType>(MipMappingType, name: nameof(MipMappingType));
			TextureSubstitution = s.SerializePointer<GLI_Texture>(TextureSubstitution, name: nameof(TextureSubstitution));

			BilinearMode = s.Serialize<GLI_BilinearMode>(BilinearMode, name: nameof(BilinearMode));
			CyclingMode = s.Serialize<GLI_CyclingMode>(CyclingMode, name: nameof(CyclingMode));

			FileName = s.SerializeString(FileName, length: 128, name: nameof(FileName));
			s.Align(4, Offset);
		}
	}
}
