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
		public Dictionary<FATEntry.Type, Dictionary<ushort, FATEntry>> entriesDict = new Dictionary<FATEntry.Type, Dictionary<ushort, FATEntry>>();

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
					t.AddEntryToDict(t.entries[i]);
				}
			});
			return t;
		}

		private void AddEntryToDict(FATEntry entry) {
			FATEntry.Type entryType = entry.EntryType;
			if (!entriesDict.ContainsKey(entryType)) {
				entriesDict[entryType] = new Dictionary<ushort, FATEntry>();
			}
			entriesDict[entryType][entry.index] = entry;
		}

		public FATEntry GetEntry(FATEntry.Type type, ushort index) {
			if (!entriesDict.ContainsKey(type) || !entriesDict[type].ContainsKey(index)) return null;
			return entriesDict[type][index];
		}
	}
}
