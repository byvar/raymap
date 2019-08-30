using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class VisualMaterialTextures : ROMStruct {
		public VisualMaterialTexture[] vmTex;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			vmTex = new VisualMaterialTexture[length];
			for (int i = 0; i < vmTex.Length; i++) {
				vmTex[i].ind_tex = reader.ReadUInt16();
				vmTex[i].uvmap_id = reader.ReadUInt16();
				vmTex[i].texRef = l.GetOrRead<TextureInfoRef>(reader, vmTex[i].ind_tex);
			}
        }

		public struct VisualMaterialTexture {
			public ushort ind_tex;
			public ushort uvmap_id;
			public TextureInfoRef texRef;
		}
    }
}
