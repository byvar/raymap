using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class String : ROMStruct {
		public string str;
		public byte[] bytes;
		public ushort length;
		public bool isBytes = false;

        protected override void ReadInternal(Reader reader) {
			if (!isBytes) {
				str = reader.ReadString(length);
			} else {
				bytes = reader.ReadBytes(length);
			}
        }
    }
}
