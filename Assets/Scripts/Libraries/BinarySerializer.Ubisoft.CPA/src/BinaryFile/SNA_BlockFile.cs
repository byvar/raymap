using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_BlockFile : MemoryMappedStreamFile {
		public SNA_MemoryBlock Block { get; set; }
		public SNA_RelocationTableBlock RelocationBlock { get; set; }
		public PointerMode Mode { get; set; }

		public SNA_BlockFile(Context context, SNA_MemoryBlock block, SNA_RelocationTableBlock relocationBlock, PointerMode mode = PointerMode.MemoryMapped)
			: base(context, block.BlockName, block.BeginBlock, block.ExpandedData, endianness: context.GetCPASettings().GetEndian) {
			Block = block;
			Mode = mode;
			RelocationBlock = relocationBlock;

			foreach (var f in Context.MemoryMap.Files) {
				if(f is SNA_BlockFile bf)
					bf.InvalidatePointerFileDictionary();
				else if(f is SNA_BlockPointerFile bpf)
					bpf.InvalidatePointerFileDictionary();
			}
				
		}

		/*public SNA_BlockFile(Context context, SNA_MemoryBlock block, SNA_RelocationTableBlock relocationBlock, PointerMode mode = PointerMode.MemoryMapped)
			: base(context, block.BlockName, block.BeginBlock, block.Data, endianness: context.GetCPASettings().GetEndian) {
			Block = block;
			Mode = mode;
			RelocationBlock = relocationBlock;
		}*/

		public override bool IsMemoryMapped => Mode == PointerMode.MemoryMapped;

		private Dictionary<uint, BinaryFile> PointerFileDictionary { get; set; }
		public void InvalidatePointerFileDictionary() {
			PointerFileDictionary = null;
		}
		private void CreatePointerFileDictionary() {
			PointerFileDictionary = new Dictionary<uint, BinaryFile>();
			foreach (var ptr in RelocationBlock.Pointers) {
				PointerFileDictionary[ptr.Pointer] = Context.MemoryMap.Files.FirstOrDefault(
						x => x is SNA_BlockFile b
						&& b.Block.Module == ptr.TargetModule
						&& b.Block.Block == ptr.TargetBlock);
			}
		}
		private BinaryFile GetFileFromDictionary(uint serializedValue) {
			if (PointerFileDictionary == null) CreatePointerFileDictionary();
			if (PointerFileDictionary.TryGetValue(serializedValue, out BinaryFile f))
				return f;
			return null;
		}


		/// <summary>
		/// Retrieves the <see cref="BinaryFile"/> for a serialized <see cref="Pointer"/> value
		/// </summary>
		/// <param name="serializedValue">The serialized pointer value</param>
		/// <param name="anchor">An optional anchor for the pointer</param>
		/// <returns></returns>
		public override BinaryFile GetPointerFile(long serializedValue, Pointer anchor = null) {
			if(IsMemoryMapped)
				return base.GetPointerFile(serializedValue, anchor: anchor);

			var f = GetFileFromDictionary((uint)serializedValue);
			if(f != null) return f;

			// Fallback: memory mapped

			// Get all memory mapped files
			var files = Context.MemoryMap.Files.Where(x => x is SNA_BlockFile b).Select(x => (SNA_BlockFile)x);

			// Sort based on the base address
			files = files.OrderByDescending(file => file.BaseAddress)
				.Where(f => serializedValue <= f.Block.MaxMem && serializedValue >= f.BaseAddress);

			// Return the first pointer within the range
			return files.FirstOrDefault();
		}

		public enum PointerMode {
			MemoryMapped,
			Relocation,
		}
	}
}