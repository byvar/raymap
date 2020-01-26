using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class DsgMem : ROMStruct {

		// size: 4
		public Reference<DsgMemInfoArray> info;
        public ushort num_info;

		protected override void ReadInternal(Reader reader) {
			info = new Reference<DsgMemInfoArray>(reader);
            num_info = reader.ReadUInt16();

			info.Resolve(reader, i => i.length = num_info);
		}
	}
}
