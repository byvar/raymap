using System;
using System.IO;

namespace BinarySerializer.Ubisoft.CPA {
    /// <summary>
    /// The encoder for Rayman 2's LEVELS0.DAT
    /// </summary>
    public class PTC_BigFileEncoder : IStreamEncoder
    {
		public PTC_BigFileEncoder(uint length) {
			Length = length;
		}

        public string Name => "PTC_BigFileEncoding";
		public uint Length { get; protected set; }
		public static uint KeysSize => 16;

        public void DecodeStream(Stream input, Stream output) {
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
			using Writer writer = new Writer(output, isLittleEndian: true, leaveOpen: true);

			// Read keys
			long[] keys = new long[4];
			keys[0] = reader.ReadUInt32();
			keys[1] = reader.ReadUInt32();
			keys[2] = reader.ReadUInt32();
			keys[3] = reader.ReadUInt32();


			// Round length up to multiple of 4
			uint rest = Length % 4;
			if (rest > 0) {
				Length += (4 - rest);
			}
			for (int i = 0; i < Length / 4; i++) {
				long value = reader.ReadUInt32();

				// Make sure value wraps around
				value = (value - keys[0]) ^ keys[1];
				while(value < 0) value += 0x100000000;

				keys[0] = (keys[0] + keys[2]) % 0x100000000;
				keys[1] = (keys[1] + keys[3]) % 0x100000000;

				writer.Write((uint)value);
			}
        }

        public void EncodeStream(Stream input, Stream output) {
			using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
			using Writer writer = new Writer(output, isLittleEndian: true, leaveOpen: true);

			writer.Write((uint)0); // Write 0 as keys
			writer.Write((uint)0);
			writer.Write((uint)0);
			writer.Write((uint)0);

			writer.Write(reader.ReadBytes((int)Length)); // no need to XOR now

			uint rest = Length % 4;
			if (rest > 0) {
				// Pad out length to multiple of 4
				for (int i = 0; i < (4 - rest); i++) {
					writer.Write((byte)0);
				}
			}
		}
    }
}