using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_PointerBlockFile : SNA_BlockFile {

		public SNA_PointerBlockFile(Context context, SNA_MemorySnapshot snapshot, string name, byte[] data, SNA_RelocationTableBlock relocationBlock)
			: base(context, snapshot, relocationBlock, name, 0, data) { }
	}
}