using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class LanguageTable : ROMStruct {
		public Reference<TextTable> textTable;
		public ushort num_txtTable;
		public ushort ind_136Table;
		public ushort num_136Table;
		public string name;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			textTable = new Reference<TextTable>(reader);
			num_txtTable = reader.ReadUInt16();
			ind_136Table = reader.ReadUInt16();
			num_136Table = reader.ReadUInt16();
			reader.ReadUInt16();
			name = reader.ReadString(0x12);

			// Resolve unresolved references
			textTable.Resolve(reader, (tt) => { tt.length = num_txtTable; });
		}
    }
}
