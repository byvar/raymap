using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GraphNodeArray : ROMStruct {
		public Reference<GraphNode>[] nodes;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			nodes = new Reference<GraphNode>[length];
			for (int i = 0; i < nodes.Length; i++) {
				nodes[i] = new Reference<GraphNode>(reader, true);
			}
        }
    }
}
