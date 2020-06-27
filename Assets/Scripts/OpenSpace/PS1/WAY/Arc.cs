using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Arc : OpenSpaceStruct {
		public Pointer off_node1;
		public Pointer off_node2;
		public uint uint_08;
		public ushort ushort_0C;
		public ushort ushort_0E;

		public WayPoint node1;
		public WayPoint node2;

		protected override void ReadInternal(Reader reader) {
			off_node1 = Pointer.Read(reader);
			off_node2 = Pointer.Read(reader);
			uint_08 = reader.ReadUInt32();
			ushort_0C = reader.ReadUInt16();
			ushort_0E = reader.ReadUInt16();

			node1 = Load.FromOffsetOrRead<WayPoint>(reader, off_node1);
			node2 = Load.FromOffsetOrRead<WayPoint>(reader, off_node2);
		}
	}
}
