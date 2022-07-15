namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_GlobalTextures : BinarySerializable {
		public bool Pre_IsFix { get; set; } = false;

		public uint MaxTexturesCount { get; set; }
		public uint TexturesCount { get; set; }
		public Pointer<GLI_Texture>[] Textures { get; set; }
		public uint TextureToCreateIndex { get; set; } // If a texture is added, it will receive this index
		public uint[] TextureMemoryChannels { get; set; }
		public uint CurrentMemoryChannel { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			MaxTexturesCount = s.Serialize<uint>(MaxTexturesCount, name: nameof(MaxTexturesCount));
			TexturesCount = s.Serialize<uint>(TexturesCount, name: nameof(TexturesCount));
			Textures = s.SerializePointerArray<GLI_Texture>(Textures, TexturesCount, name: nameof(Textures))?.ResolveObject(s);
			TextureToCreateIndex = s.Serialize<uint>(TextureToCreateIndex, name: nameof(TextureToCreateIndex));
			TextureMemoryChannels = s.SerializeArray<uint>(TextureMemoryChannels, Pre_IsFix ? TexturesCount : MaxTexturesCount, name: nameof(TextureMemoryChannels));
			CurrentMemoryChannel = s.Serialize<uint>(CurrentMemoryChannel, name: nameof(CurrentMemoryChannel));
		}

		public bool TextureIsAllocated(int i) => TextureMemoryChannels[i] != 0xC0DE0005;
	}
}
