using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	public class FATTable {
		public LegacyPointer offset;
		public LegacyPointer off_table;
		public uint num_entries;
		public FATEntry[] entries = new FATEntry[0];
		public Dictionary<FATEntry.Type, Dictionary<ushort, FATEntry>> entriesDict = new Dictionary<FATEntry.Type, Dictionary<ushort, FATEntry>>();

		public static FATTable Read(Reader reader, LegacyPointer offset, bool readEntries = true) {
			FATTable t = new FATTable();
			t.offset = offset;
			t.off_table = LegacyPointer.Read(reader);
			t.num_entries = reader.ReadUInt32();
			if (readEntries) {
				t.ReadEntries(reader);
			}
			return t;
		}

		public void ReadEntries(Reader reader) {
			entries = new FATEntry[num_entries];
			LegacyPointer.DoAt(ref reader, off_table, () => {
				for (int i = 0; i < entries.Length; i++) {
					entries[i] = FATEntry.Read(reader, LegacyPointer.Current(reader));
					entries[i].entryIndexWithinTable = (uint)i;
					AddEntryToDict(entries[i]);
				}
			});
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
