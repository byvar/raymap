using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_DataBlockFile : SNA_BlockFile {
		public SNA_MemoryBlock Block { get; set; }

		public SNA_DataBlockFile(Context context, SNA_MemorySnapshot snapshot, SNA_MemoryBlock block, SNA_RelocationTableBlock relocationBlock)
			: base(context, snapshot, relocationBlock, block.BlockName, block.BeginBlock, block.ExpandedData) {
			Block = block;

			foreach (var f in Context.MemoryMap.Files) {
				if(f is SNA_BlockFile bf) bf.InvalidateBlockFileDictionary();
			}
				
		}
	}
}