using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class BinaryStringRefArray : ROMStruct {
		public ushort length;
		public Reference<BinaryStringRef>[] strings;


        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			strings = new Reference<BinaryStringRef>[length];
			for (ushort i = 0; i < length; i++) {
				strings[i] = new Reference<BinaryStringRef>(reader, true);
			}
		}
    }
}
