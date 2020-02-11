using OpenSpace.Loader;
using System.Linq;
using UnityEngine;
using DsgVarType = OpenSpace.AI.DsgVarInfoEntry.DsgVarType;

namespace OpenSpace.ROM {
	public class DsgVarInfo : ROMStruct {
		// Size: 6 * length
		public Entry[] entries;
		public ushort length;

		protected override void ReadInternal(Reader reader) {
			entries = new Entry[length];
			for (ushort i = 0; i < entries.Length; i++) {
				entries[i] = new Entry(reader, i);
				/*Loader.print("DsgVarInfoEntry[" + i + "]"
					+ " - " + entries[i].dsgVarType
					+ " - " + entries[i].offsetInBuffer
					+ (entries[i].paramEntry?.Value != null ? "[" + entries[i].paramEntry.Value.index_in_array + "]" : "")
					+ " - " + entries[i].param);*/
			}
		}

		public Entry GetEntryFromIndex(int ind) {
			Entry entry = null;
			entry = entries.FirstOrDefault(e => e.value.index == ind);
			return entry;
		}

		public class Entry {
			public ushort type;
			public ushort offsetInBuffer;

			// Custom
			public Pointer offset;
			public DsgVarValue value;

			public Entry(Reader reader, ushort index_of_info) {
				offset = Pointer.Current(reader);

				type = reader.ReadUInt16();
				value = new DsgVarValue(index_of_info, type);
				value.Read(reader);

				offsetInBuffer = reader.ReadUInt16();

			}
			public string NiceVariableName {
				get {
					return value.dsgVarType + "_" + value.index_of_info;
				}
			}
		}
	}
}
