using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class NoCtrlTextureList : ROMStruct {
		public Reference<TextureInfoRef>[] texRefs;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			ushort length = 5;
			texRefs = new Reference<TextureInfoRef>[length];
			for (int i = 0; i < length; i++) {
				texRefs[i] = new Reference<TextureInfoRef>(reader, true);
			}
		}
    }
}
