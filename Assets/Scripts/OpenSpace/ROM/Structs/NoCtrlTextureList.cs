using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class NoCtrlTextureList : ROMStruct {
		public Reference<TextureInfoRef>[] refs;
		public ushort length;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			refs = new Reference<TextureInfoRef>[length];
			for (int i = 0; i < length; i++) {
				refs[i] = new Reference<TextureInfoRef>(reader, true);
			}
		}
    }
}
