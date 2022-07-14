using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_BlockPointerFile : MemoryMappedStreamFile {
		public SNA_RelocationTableBlock RelocationBlock { get; set; }

		public SNA_BlockPointerFile(Context context, string name, byte[] data, SNA_RelocationTableBlock relocationBlock)
			: base(context, name, 0, data, endianness: context.GetCPASettings().GetEndian) {
			RelocationBlock = relocationBlock;
		}

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
			if(PointerFileDictionary == null) CreatePointerFileDictionary();
			if(PointerFileDictionary.TryGetValue(serializedValue, out BinaryFile f))
				return f;
			return null;
		}

		public override bool IsMemoryMapped => false;

		/// <summary>
		/// Retrieves the <see cref="BinaryFile"/> for a serialized <see cref="Pointer"/> value
		/// </summary>
		/// <param name="serializedValue">The serialized pointer value</param>
		/// <param name="anchor">An optional anchor for the pointer</param>
		/// <returns></returns>
		public override BinaryFile GetPointerFile(long serializedValue, Pointer anchor = null) {
			return GetFileFromDictionary((uint)serializedValue);
		}
	}
}