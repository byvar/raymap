using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SectorSuperObjectArray1Info : ROMStruct {
		public NeighborSectorInfo[] infos;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			infos = new NeighborSectorInfo[length];
			for (int i = 0; i < infos.Length; i++) {
				infos[i] = new NeighborSectorInfo(reader);
			}
        }

		public class NeighborSectorInfo {
			public ushort word0;
			public ushort word2;
			public NeighborSectorInfo(Reader reader) {
				word0 = reader.ReadUInt16();
				word2 = reader.ReadUInt16();
			}
		}
    }
}
