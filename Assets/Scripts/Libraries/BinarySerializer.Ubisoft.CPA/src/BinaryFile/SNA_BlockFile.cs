﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarySerializer.Ubisoft.CPA {
	public abstract class SNA_BlockFile : MemoryMappedStreamFile {
		public SNA_RelocationTableBlock RelocationBlock { get; set; }
		public SNA_MemorySnapshot Snapshot { get; set; }

		public SNA_BlockFile(Context context, SNA_MemorySnapshot snapshot, SNA_RelocationTableBlock relocationBlock, string name, long baseAddress, byte[] data)
			: base(context, name, baseAddress, data, endianness: context.GetCPASettings().GetEndian) {
			Snapshot = snapshot;
			RelocationBlock = relocationBlock;
				
		}

		public void OverrideData(long offset, byte[] data) {
			var pos = Stream.Position;
			using (Writer writer = new Writer(Stream, isLittleEndian: Endianness == Endian.Little, leaveOpen: true)) {
				writer.BaseStream.Position = offset;
				writer.Write(data);
			}
			Stream.Position = pos;
		}
		public void OverrideUInt(long offset, uint data) {
			var pos = Stream.Position;
			using (Writer writer = new Writer(Stream, isLittleEndian: Endianness == Endian.Little, leaveOpen: true)) {
				writer.BaseStream.Position = offset;
				writer.Write(data);
			}
			Stream.Position = pos;
		}
		public override bool IsMemoryMapped => false;

		#region Dictionaries
		private Dictionary<uint, SNA_MemoryBlock> PointerBlockDictionary { get; set; }
		private void CreatePointerBlockDictionary() {
			PointerBlockDictionary = new Dictionary<uint, SNA_MemoryBlock>();
			if(RelocationBlock?.Pointers == null) return;
			foreach (var ptr in RelocationBlock.Pointers) {
				PointerBlockDictionary[ptr.Pointer] = Snapshot.Blocks.LastOrDefault(
						x => x.Module == ptr.TargetModule && x.Block == ptr.TargetBlock);
			}
		}
		private SNA_MemoryBlock GetBlockFromDictionary(uint serializedValue) {
			if (PointerBlockDictionary == null) CreatePointerBlockDictionary();
			if (PointerBlockDictionary.TryGetValue(serializedValue, out SNA_MemoryBlock b))
				return b;
			return null;
		}
		private Dictionary<SNA_MemoryBlock, BinaryFile> BlockFileDictionary { get; set; }
		public void InvalidateBlockFileDictionary() {
			BlockFileDictionary = null;
		}
		private void CreateBlockFileDictionary() {
			BlockFileDictionary = new Dictionary<SNA_MemoryBlock, BinaryFile>();
			foreach (var block in Snapshot.Blocks) {
				BlockFileDictionary[block] = Context.MemoryMap.Files.FirstOrDefault(
						x => x is SNA_DataBlockFile b
						&& b.Block.Module == block.Module
						&& b.Block.Block == block.Block);
			}
		}
		private BinaryFile GetFileFromBlock(SNA_MemoryBlock b) {
			if (BlockFileDictionary == null) CreateBlockFileDictionary();
			if (BlockFileDictionary.TryGetValue(b, out BinaryFile f))
				return f;
			return null;
		}
		#endregion

		public override bool TryGetPointer(long value, out Pointer result, Pointer anchor = null, bool allowInvalid = false, PointerSize size = PointerSize.Pointer32) {
			Pointer ptr = null;

			var anchorOffset = anchor?.AbsoluteOffset ?? 0;
			uint absoluteValue = (uint)(value + anchorOffset);
			var block = GetBlockFromDictionary(absoluteValue);

			if (block == null) {
				// Pointer isn't listed in relocation file - try memory mapped
				block = Snapshot?.Blocks?.LastOrDefault(b => 
					b.BeginBlock != SNA_MemoryBlock.InvalidBeginBlock
					&& absoluteValue >= b.BeginBlock
					&& absoluteValue < b.EndBlock + 1);
			}

			if (block != null) {
				BinaryFile file = GetFileFromBlock(block);
				if (file != null) {
					// Use file offset
					ptr = new Pointer(
						absoluteValue - block.BeginBlock,
						file, anchor: anchor,
						offsetType: Pointer.OffsetType.File);

				}
			}

			result = ptr;
			if (ptr == null && value != 0 && !allowInvalid && !AllowInvalidPointer(value, anchor: anchor)) {
				return false;
			}
			return true;
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

			var anchorOffset = anchor?.AbsoluteOffset ?? 0;
			uint absoluteValue = (uint)(serializedValue + anchorOffset);
			var block = GetBlockFromDictionary(absoluteValue);

			if (block == null) {
				// Pointer isn't listed in relocation file - try memory mapped
				block = Snapshot?.Blocks?.LastOrDefault(b =>
					b.BeginBlock != SNA_MemoryBlock.InvalidBeginBlock
					&& absoluteValue >= b.BeginBlock
					&& absoluteValue < b.EndBlock + 1);
			}

			if (block != null) {
				return GetFileFromBlock(block);
			}
			
			return null;
		}
	}
}