using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_BlockFile : MemoryMappedStreamFile {
		public SNA_MemoryBlock Block { get; set; }
		public SNA_RelocationTableBlock RelocationBlock { get; set; }
		public PointerMode Mode { get; set; }

		public SNA_BlockFile(Context context, SNA_MemoryBlock block, SNA_RelocationTableBlock relocationBlock, PointerMode mode = PointerMode.MemoryMapped)
			: base(context, block.BlockName, block.BeginBlock, block.Data, endianness: context.GetCPASettings().GetEndian) {
			Block = block;
			Mode = mode;
			RelocationBlock = relocationBlock;
		}

		/*public SNA_BlockFile(Context context, SNA_MemoryBlock block, SNA_RelocationTableBlock relocationBlock, PointerMode mode = PointerMode.MemoryMapped)
			: base(context, block.BlockName, block.BeginBlock, block.Data, endianness: context.GetCPASettings().GetEndian) {
			Block = block;
			Mode = mode;
			RelocationBlock = relocationBlock;
		}*/

		public override bool IsMemoryMapped => Mode == PointerMode.MemoryMapped;

		/// <summary>
		/// Retrieves the <see cref="BinaryFile"/> for a serialized <see cref="Pointer"/> value
		/// </summary>
		/// <param name="serializedValue">The serialized pointer value</param>
		/// <param name="anchor">An optional anchor for the pointer</param>
		/// <returns></returns>
		public override BinaryFile GetPointerFile(long serializedValue, Pointer anchor = null) {
			if(IsMemoryMapped)
				return base.GetPointerFile(serializedValue, anchor: anchor);

			foreach (var ptr in RelocationBlock.Pointers) {
				if (ptr.Pointer == serializedValue) {
					// Get file this points to
					return Context.MemoryMap.Files.FirstOrDefault(
						x => x is SNA_BlockFile b
						&& b.Block.Module == ptr.TargetModule
						&& b.Block.Block == ptr.TargetBlock);
				}
			}

			return null;
		}

		public enum PointerMode {
			MemoryMapped,
			Relocation,
		}
	}
}