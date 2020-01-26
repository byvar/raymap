using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class DsgVar : ROMStruct {

		// size: 16
		public ushort buffer_size;
		public Reference<DsgVarInfo> info;
		public ushort ref_111_unk0;
		public ushort ref_111_unk1;
		public ushort num_dsgVars_in_buffer;
		public ushort num_info;
        public ushort num_unk0;
        public ushort num_unk1;

		protected override void ReadInternal(Reader reader) {
            buffer_size = reader.ReadUInt16(); // 0x00
			info = new Reference<DsgVarInfo>(reader); // 0x02
            ref_111_unk0 = reader.ReadUInt16(); // 0x04
            ref_111_unk1 = reader.ReadUInt16(); // 0x06
            num_dsgVars_in_buffer = reader.ReadUInt16(); // 0x08
            num_info = reader.ReadUInt16(); // 0x0A
            num_unk0 = reader.ReadUInt16(); // 0x0C
            num_unk1 = reader.ReadUInt16(); // 0x0E

			info.Resolve(reader, i => i.length = num_info);
		}
	}
}
