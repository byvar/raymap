using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class WayPoint : OpenSpaceStruct {
		public int x;
		public int y;
		public int z;
		public short short_0C;
		public short short_0E;
		public short radius;
		public short short_12;

		protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt32();
			y = reader.ReadInt32();
			z = reader.ReadInt32();
			short_0C = reader.ReadInt16();
			short_0E = reader.ReadInt16();
			radius = reader.ReadInt16();
			short_12 = reader.ReadInt16();
		}



		public WayPointBehaviour GetGameObject() {
			GameObject gao = new GameObject("WayPoint (" + Offset + ")");
			//if (perso.Value != null) gao.name += " - Perso: " + perso.Value.Offset;
			float factor = 100f;
			Vector3 pos = Position;
			float radius_conv = radius / factor;

			gao.transform.position = pos;
			WayPointBehaviour wpBehaviour = gao.AddComponent<WayPointBehaviour>();
			gao.layer = LayerMask.NameToLayer("Graph");
			wpBehaviour.wpPS1 = this;
			wpBehaviour.radius = radius_conv;
			return wpBehaviour;
		}
		public Vector3 Position {
			get {
				float factor = 256f;
				Vector3 pos = new Vector3(x / factor, z / factor, y / factor);
				return pos;
			}
		}
	}
}
