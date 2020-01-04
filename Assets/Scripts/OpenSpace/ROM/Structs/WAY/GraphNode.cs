using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GraphNode : ROMStruct {
		// Size: 12
		public Reference<WayPoint> waypoint;
		public Reference<GraphNodeArray> arcs_nodes;
		public Reference<ArcCapsArray> arcs_caps;
		public Reference<ArcWeightArray> arcs_weights;
		public ushort num_arcs;
		public Reference<ArcCapsArray> unk;

		protected override void ReadInternal(Reader reader) {
			waypoint = new Reference<WayPoint>(reader, true);
			arcs_nodes = new Reference<GraphNodeArray>(reader);
			arcs_caps = new Reference<ArcCapsArray>(reader);
			arcs_weights = new Reference<ArcWeightArray>(reader);
			num_arcs = reader.ReadUInt16();
			unk = new Reference<ArcCapsArray>(reader, true, a => a.length = 1);
			if (num_arcs > 0) {
				arcs_caps.Resolve(reader, a => a.length = num_arcs);
				arcs_weights.Resolve(reader, a => a.length = num_arcs);
				arcs_nodes.Resolve(reader, a => a.length = num_arcs);
			}
		}

		public Vector3 Position {
			get {
				if (waypoint.Value == null) return Vector3.zero;
				return waypoint.Value.Position;
			}
		}
	}
}
