using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class DsgMemInfoArray : ROMStruct {
		public Reference<DsgMemInfo>[] info;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			info = new Reference<DsgMemInfo>[length];
			for (int i = 0; i < info.Length; i++) {
				info[i] = new Reference<DsgMemInfo>(reader, true);
			}
        }
    }
}
