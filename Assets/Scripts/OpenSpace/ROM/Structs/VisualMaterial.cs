using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public Reference<VisualMaterialTextures> textures;
		public ushort num_textures;
		public ushort flags;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt32();
			reader.ReadUInt32();
			textures = new Reference<VisualMaterialTextures>(reader);
			num_textures = reader.ReadUInt16();
			textures.Resolve(reader, (vmt) => { vmt.length = num_textures; });
			reader.ReadUInt16();
			flags = reader.ReadUInt16();
        }
    }
}
