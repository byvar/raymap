using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class TextureInfoRef : ROMStruct {
		public Reference<TextureInfo> texInfo;

		protected override void ReadInternal(Reader reader) {
			texInfo = new Reference<TextureInfo>(reader, true);
		}
	}
}
