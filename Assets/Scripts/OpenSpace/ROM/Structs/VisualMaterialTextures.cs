using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class VisualMaterialTextures : ROMStruct {
		public VisualMaterialTexture[] vmTex;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			vmTex = new VisualMaterialTexture[length];
			for (int i = 0; i < vmTex.Length; i++) {
				vmTex[i].texRef = new Reference<TextureInfoRef>(reader, true);
				vmTex[i].uvmap_id = reader.ReadUInt16();
			}
        }

		public struct VisualMaterialTexture {
			public Reference<TextureInfoRef> texRef;
			public ushort uvmap_id;
		}
    }
}
