using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SectorSuperObjectArray5Info : ROMStruct {
		public SectorSuperObject5Info[] infos;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			infos = new SectorSuperObject5Info[length];
			for (int i = 0; i < infos.Length; i++) {
				infos[i] = new SectorSuperObject5Info(reader);
			}
        }

		public class SectorSuperObject5Info {
			public ushort word0;
			public ushort word2;
			public SectorSuperObject5Info(Reader reader) {
				word0 = reader.ReadUInt16();
				word2 = reader.ReadUInt16();
			}
		}
    }
}
