using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class TextureInfoRef : ROMStruct {
		public TextureInfo texInfo;
		public ushort ind_texInfo;

		protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			ind_texInfo = reader.ReadUInt16();
			texInfo = l.Get<TextureInfo>(ind_texInfo);
		}
	}
}
