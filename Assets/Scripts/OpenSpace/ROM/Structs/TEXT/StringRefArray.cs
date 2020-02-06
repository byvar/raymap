using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class StringRefArray : ROMStruct {
		public ushort length;
		public Reference<StringRef>[] strings;


        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			strings = new Reference<StringRef>[length];
			for (ushort i = 0; i < length; i++) {
				strings[i] = new Reference<StringRef>(reader, true);
			}
		}
    }
}
