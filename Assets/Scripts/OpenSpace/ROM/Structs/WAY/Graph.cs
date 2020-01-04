using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Graph : ROMStruct {
		// Size: 4
		public Reference<GraphNodeArray> nodes;
		public ushort num_nodes;

		protected override void ReadInternal(Reader reader) {
			nodes = new Reference<GraphNodeArray>(reader);
			num_nodes = reader.ReadUInt16();
			nodes.Resolve(reader, n => n.length = num_nodes);
			Loader.graphsROM.Add(this);
		}
	}
}
