using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class BinaryStringRef : ROMStruct {
		public Reference<String> str;
		public ushort sz_str;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			sz_str = reader.ReadUInt16();
			str = new Reference<String>(reader, resolve: true, onPreRead: (s) => { s.length = sz_str; s.isBytes = true; });
			//l.print(sz_str + " - " + string.Join(", ", str.Value.bytes));
        }

		public override string ToString() {
			return string.Join(", ", str.Value.bytes);
		}
	}
}
