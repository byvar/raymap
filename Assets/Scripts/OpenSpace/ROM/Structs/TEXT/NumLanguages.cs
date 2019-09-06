using OpenSpace.Loader;

namespace OpenSpace.ROM {
	public class NumLanguages : ROMStruct {
		public ushort num_languages;
		
        protected override void ReadInternal(Reader reader) {
			num_languages = reader.ReadUInt16();
		}
    }
}
