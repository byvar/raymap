using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class LanguageTable : ROMStruct {
		public Reference<StringRefArray> textTable;
		public ushort num_txtTable;
		public Reference<BinaryStringRefArray> binaryTable;
		public ushort num_binaryTable;
		public ushort unk_08;
		public string name;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			textTable = new Reference<StringRefArray>(reader);
			num_txtTable = reader.ReadUInt16();
			binaryTable = new Reference<BinaryStringRefArray>(reader);
			num_binaryTable = reader.ReadUInt16();
			unk_08 = reader.ReadUInt16();
			name = reader.ReadString(0x12);

			// Resolve unresolved references
			textTable.Resolve(reader, (tt) => { tt.length = num_txtTable; });
			binaryTable.Resolve(reader, (tt) => { tt.length = num_binaryTable; });
		}
    }
}
