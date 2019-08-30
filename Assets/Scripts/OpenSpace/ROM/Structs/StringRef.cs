using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class StringRef : ROMStruct {
		public String str;
		public ushort ind_str;
		public ushort sz_str;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			sz_str = reader.ReadUInt16();
			ind_str = reader.ReadUInt16();
			str = l.GetOrRead<String>(reader, ind_str, (s) => { s.length = sz_str; });
        }
    }
}
