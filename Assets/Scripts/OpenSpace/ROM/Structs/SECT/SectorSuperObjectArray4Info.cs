using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class SectorSuperObjectArray4Info : ROMStruct {
		public uint[] infos;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			infos = new uint[length];
			for (int i = 0; i < infos.Length; i++) {
				infos[i] = reader.ReadUInt32();
			}
        }
    }
}
