using OpenSpace.Loader;
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
			Vector3 pos = Position;
			//UnityEngine.Debug.Log(Offset + " - " + radius + " - " + short_0C + " - " + short_0E + " - " + short_12);
			float radius_conv = radius / 100f; // Radius is usually 100, matching PC's default value of 1

			gao.transform.position = pos;
			WayPointBehaviour wpBehaviour = gao.AddComponent<WayPointBehaviour>();
			gao.layer = LayerMask.NameToLayer("Graph");
			wpBehaviour.wpPS1 = this;
			wpBehaviour.radius = radius_conv;
			return wpBehaviour;
		}
		public Vector3 Position {
			get {
				Vector3 pos = new Vector3(x, z, y) / R2PS1Loader.CoordinateFactor;
				return pos;
			}
		}
	}
}
