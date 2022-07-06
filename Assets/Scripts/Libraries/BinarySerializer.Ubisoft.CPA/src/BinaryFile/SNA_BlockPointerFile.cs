using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_BlockPointerFile : MemoryMappedStreamFile {
		public SNA_RelocationTableBlock RelocationBlock { get; set; }

		public SNA_BlockPointerFile(Context context, string name, byte[] data, SNA_RelocationTableBlock relocationBlock)
			: base(context, name, 0, data, endianness: context.GetCPASettings().GetEndian) {
			RelocationBlock = relocationBlock;
		}

		public override bool IsMemoryMapped => false;

		/// <summary>
		/// Retrieves the <see cref="BinaryFile"/> for a serialized <see cref="Pointer"/> value
		/// </summary>
		/// <param name="serializedValue">The serialized pointer value</param>
		/// <param name="anchor">An optional anchor for the pointer</param>
		/// <returns></returns>
		public override BinaryFile GetPointerFile(long serializedValue, Pointer anchor = null) {
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
	}
}