using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class LanguageTable : ROMStruct {
		public ushort ind_txtTable;
		public ushort num_txtTable;
		public ushort ind_136Table;
		public ushort num_136Table;
		public string name;
		public TextTable textTable;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			ind_txtTable = reader.ReadUInt16();
			num_txtTable = reader.ReadUInt16();
			ind_136Table = reader.ReadUInt16();
			num_136Table = reader.ReadUInt16();
			reader.ReadUInt16();
			name = reader.ReadString(0x12);
			textTable = l.GetOrRead<TextTable>(reader, ind_txtTable, (tt) => { tt.length = num_txtTable; });
		}
    }
}
