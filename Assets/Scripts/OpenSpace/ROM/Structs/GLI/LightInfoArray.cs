using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class LightInfoArray : ROMStruct {
		public Reference<LightInfo>[] lights;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			lights = new Reference<LightInfo>[length];
			for (int i = 0; i < lights.Length; i++) {
				lights[i] = new Reference<LightInfo>(reader, true);
			}
        }
    }
}
