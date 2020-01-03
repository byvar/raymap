using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ComportArray : ROMStruct {
		public Reference<Comport>[] comports;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			comports = new Reference<Comport>[length];
			for (int i = 0; i < comports.Length; i++) {
				comports[i] = new Reference<Comport>(reader, true);
			}
        }
    }
}
