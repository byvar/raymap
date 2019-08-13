using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class FATTable {
		public Pointer offset;
		public Pointer off_table;
		public uint num_entries;
		public FATEntry[] entries;

		public static FATTable Read(Reader reader, Pointer offset) {
			FATTable t = new FATTable();
			t.offset = offset;
			t.off_table = Pointer.Read(reader);
			t.num_entries = reader.ReadUInt32();
			t.entries = new FATEntry[t.num_entries];
			Pointer.DoAt(ref reader, t.off_table, () => {
				for (int i = 0; i < t.entries.Length; i++) {
					t.entries[i] = FATEntry.Read(reader, Pointer.Current(reader));
					t.entries[i].entryIndexWithinTable = (uint)i;
				}
			});
			return t;
		}
	}
}
