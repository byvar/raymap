using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class LanguageTable : ROMStruct {
		public Reference<StringRefArray> textTable;
		public ushort num_txtTable;
		public Reference<BinaryStringRefArray> textTable2;
		public ushort num_txtTable2;
		public ushort unk_08;
		public string name;
		
        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			textTable = new Reference<StringRefArray>(reader);
			num_txtTable = reader.ReadUInt16();
			textTable2 = new Reference<BinaryStringRefArray>(reader);
			num_txtTable2 = reader.ReadUInt16();
			unk_08 = reader.ReadUInt16();
			name = reader.ReadString(0x12);

			// Resolve unresolved references
			textTable.Resolve(reader, (tt) => { tt.length = num_txtTable; });
			textTable2.Resolve(reader, (tt) => { tt.length = num_txtTable2; });
		}
    }
}
