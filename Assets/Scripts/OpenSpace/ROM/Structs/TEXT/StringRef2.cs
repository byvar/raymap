using OpenSpace.Loader;
using UnityEngine;

namespace OpenSpace.ROM {
	public class StringRef2 : ROMStruct {
		public Reference<String> str;
		public ushort sz_str;

        protected override void ReadInternal(Reader reader) {
			R2ROMLoader l = MapLoader.Loader as R2ROMLoader;
			sz_str = reader.ReadUInt16();
			str = new Reference<String>(reader, resolve: true, onPreRead: (s) => { s.length = sz_str; });
            Debug.Log("Loaded string " + str.Value.str);
        }

        public override string ToString()
        {
            return "stringTable["+str.index+"]/*"+str.Value.str+"*/";
        }
    }
}
