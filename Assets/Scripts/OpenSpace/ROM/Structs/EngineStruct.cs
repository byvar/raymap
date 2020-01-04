using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class EngineStruct : ROMStruct {
		public Reference<TextureInfoRef> shadowTexRef;
		public Reference<VisualMaterial> characterMaterial;
		public Reference<NoCtrlTextureList> noCtrlTextureList;
		public Reference<Vector3Array> vectors_0;
		public Reference<Vector3Array> vectors_1_poScales;
		public ushort num_vectors_0;
		public ushort num_vectors_1;
		public Reference<Short3Array> indices_0;
		public Reference<Short3Array> indices_1;
		public ushort num_indices_0;
		public ushort num_indices_1;

		protected override void ReadInternal(Reader reader) {
			reader.ReadUInt16();
			reader.ReadUInt16();
			reader.ReadUInt16();
			vectors_0 = new Reference<Vector3Array>(reader);
			indices_0 = new Reference<Short3Array>(reader);
			reader.ReadUInt16();
			shadowTexRef = new Reference<TextureInfoRef>(reader, true);
			reader.ReadUInt16();
			characterMaterial = new Reference<VisualMaterial>(reader, true);
			noCtrlTextureList = new Reference<NoCtrlTextureList>(reader, true, l => l.length = 5);
			num_vectors_0 = reader.ReadUInt16();
			num_indices_0 = reader.ReadUInt16();
			reader.ReadUInt16();
			vectors_1_poScales = new Reference<Vector3Array>(reader, forceFix: true);
			indices_1 = new Reference<Short3Array>(reader);
			num_vectors_1 = reader.ReadUInt16();
			num_indices_1 = reader.ReadUInt16();
			reader.ReadUInt16();

			vectors_0.Resolve(reader, v => v.length = num_vectors_0);
			vectors_1_poScales.Resolve(reader, v => v.length = num_vectors_1);
			indices_0.Resolve(reader, i => i.length = num_indices_0);
			indices_1.Resolve(reader, i => i.length = num_indices_1);
		}
    }
}
