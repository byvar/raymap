using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class EngineStruct : ROMStruct {
		public Reference<TextureInfoRef> shadowTexRef;
		public Reference<VisualMaterial> characterMaterial;
		public Reference<NoCtrlTextureList> noCtrlTextureList;
		
        protected override void ReadInternal(Reader reader) {
			reader.ReadBytes(12);
			shadowTexRef = new Reference<TextureInfoRef>(reader, true);
			reader.ReadUInt16();
			characterMaterial = new Reference<VisualMaterial>(reader, true);
			noCtrlTextureList = new Reference<NoCtrlTextureList>(reader, true);
		}
    }
}
