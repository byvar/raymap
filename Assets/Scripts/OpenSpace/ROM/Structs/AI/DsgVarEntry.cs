using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class DsgVarEntry : ROMStruct {

		// size: 8
		public ushort param;
		public uint paramUInt;
		public float paramFloat;
		public int paramInt;
		public short paramShort;
		public sbyte paramByte;
		public byte paramUByte;

		public ushort index_of_entry;
		public ushort index_in_array;

		protected override void ReadInternal(Reader reader) {
            param = reader.ReadUInt16(); // 0x00

			// Read different types of param
			LegacyPointer.Goto(ref reader, Offset); paramByte = reader.ReadSByte();
			LegacyPointer.Goto(ref reader, Offset); paramUByte = reader.ReadByte();
			LegacyPointer.Goto(ref reader, Offset); paramShort = reader.ReadInt16();
			LegacyPointer.Goto(ref reader, Offset); paramUInt = reader.ReadUInt32();
			LegacyPointer.Goto(ref reader, Offset); paramInt = reader.ReadInt32();
			LegacyPointer.Goto(ref reader, Offset); paramFloat = reader.ReadSingle();

			index_of_entry = reader.ReadUInt16(); // 0x04
            index_in_array = reader.ReadUInt16(); // 0x06
		}
	}
}
