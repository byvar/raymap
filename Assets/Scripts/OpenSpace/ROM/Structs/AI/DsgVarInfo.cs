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
			for (int i = 0; i < entries.Length; i++) {
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
			entry = entries.FirstOrDefault(e => e.index == ind);
			return entry;
		}

		public class Entry {
			public ushort type;
			public ushort param;
			public ushort offsetInBuffer;

			// Custom
			public DsgVarType dsgVarType;
			public Pointer offset;
			public int index;

			// Parsed param
			public short paramShort;
			public byte paramByte;
			public Reference<DsgVarEntry> paramEntry;
			public Reference<Dsg_Int> paramInt;
			public Reference<Dsg_UInt> paramUInt;
			public Reference<Dsg_Real> paramFloat;
			public Reference<Dsg_Vector3> paramVector3;
			public Reference<WayPoint> paramWaypoint;
			public Reference<Graph> paramGraph;
			public Reference<Comport> paramComport;
			public Reference<State> paramState;
			public Reference<GameMaterial> paramGameMaterial;
			public Reference<Perso> paramPerso;

			public Entry(Reader reader, int index) {
				offset = Pointer.Current(reader);
				this.index = index;

				type = reader.ReadUInt16();
				param = reader.ReadUInt16();

				// Read different types of param
				Pointer.Goto(ref reader, offset + 2); paramByte = reader.ReadByte();
				Pointer.Goto(ref reader, offset + 2); paramShort = reader.ReadInt16();

				offsetInBuffer = reader.ReadUInt16();

				Parse(reader);
			}

			private void Parse(Reader reader) {
				ushort typeNumber = type;
				ushort usedParam = param;
				dsgVarType = DsgVarType.None;
				if ((type & ((ushort)FATEntry.Flag.Fix)) == (ushort)FATEntry.Flag.Fix) {
					paramEntry = new Reference<DsgVarEntry>(param, reader, true);
					typeNumber = (ushort)(typeNumber & 0x7FFF);
					index = paramEntry.Value.index_of_entry;
					usedParam = paramEntry.Value.param;
				}
				if (Settings.s.aiTypes != null) dsgVarType = Settings.s.aiTypes.GetDsgVarType(typeNumber);
				if (paramEntry == null) {
					// If paramEntry exists, all params <= 4 bytes, so until float, should be read from paramEntry.
					// eg. instead of using paramUInt, you use paramEntry.Value.paramUInt
					// If paramEntry doesn't exist, all params <= 2 bytes should be read from the dsgvar info
					switch (dsgVarType) {
						case DsgVarType.UInt:
							paramUInt = new Reference<Dsg_UInt>(usedParam, reader, true);
							break;
						case DsgVarType.Int:
							paramInt = new Reference<Dsg_Int>(usedParam, reader, true);
							break;
						case DsgVarType.Float:
							paramFloat = new Reference<Dsg_Real>(usedParam, reader, true);
							break;
					}
				}
				switch (dsgVarType) {
					case DsgVarType.Vector:
						paramVector3 = new Reference<Dsg_Vector3>(usedParam, reader, true);
						break;
					case DsgVarType.Graph:
						paramGraph = new Reference<Graph>(usedParam, reader, true);
						break;
					case DsgVarType.WayPoint:
						paramWaypoint = new Reference<WayPoint>(usedParam, reader, true);
						break;
					case DsgVarType.Comport:
						paramComport = new Reference<Comport>(usedParam, reader, true);
						break;
					case DsgVarType.Action:
						paramState = new Reference<State>(usedParam, reader, true);
						break;
					case DsgVarType.GameMaterial:
						paramGameMaterial = new Reference<GameMaterial>(usedParam, reader, true);
						break;
					case DsgVarType.Perso:
						paramPerso = new Reference<Perso>(usedParam, reader, true);
						break;
				}
			}
		}
	}
}
