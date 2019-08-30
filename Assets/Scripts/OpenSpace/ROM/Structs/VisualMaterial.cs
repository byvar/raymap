using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class VisualMaterial : ROMStruct {
		public ushort ind_vmTex;
		public ushort num_vmTex;
		public VisualMaterialTextures vmTex;
		public ushort flags;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt32();
			reader.ReadUInt32();
			ind_vmTex = reader.ReadUInt16();
			num_vmTex = reader.ReadUInt16();
			vmTex = l.GetOrRead<VisualMaterialTextures>(reader, ind_vmTex, (vt) => {
				vt.length = num_vmTex;
			});
			reader.ReadUInt16();
			flags = reader.ReadUInt16();
        }
    }
}
