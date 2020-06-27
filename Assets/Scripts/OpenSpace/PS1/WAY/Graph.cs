using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Graph : OpenSpaceStruct {
		public uint num_arcs;
		public Pointer off_firstNode;
		public Pointer off_arcs;
		public uint num_nodes;
		public byte[] unkBytes;

		// Parsed
		public Arc[] arcs;

		protected override void ReadInternal(Reader reader) {
			//Load.print("Graph: " + Offset);
			num_arcs = reader.ReadUInt32();
			if (num_arcs == 0) {
				reader.ReadUInt32();
				reader.ReadUInt32();
			} else {
				off_firstNode = Pointer.Read(reader);
				off_arcs = Pointer.Read(reader);
			}
			num_nodes = reader.ReadUInt32();
			unkBytes = reader.ReadBytes(0x58);

			arcs = Load.ReadArray<Arc>(num_arcs, reader, off_arcs);
		}
	}
}
