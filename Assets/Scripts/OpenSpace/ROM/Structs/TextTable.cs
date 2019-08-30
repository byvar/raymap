using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class TextTable : ROMStruct {
		public ushort length;
		public ushort[] ind_strings;
		public StringRef[] strings;


        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			ind_strings = new ushort[length];
			strings = new StringRef[length];
			for (ushort i = 0; i < length; i++) {
				ind_strings[i] = reader.ReadUInt16();
				strings[i] = l.GetOrRead<StringRef>(reader, ind_strings[i]);
			}
		}
    }
}
