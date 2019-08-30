using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class String : ROMStruct {
		public string str;
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			str = reader.ReadString(length);
        }
    }
}
