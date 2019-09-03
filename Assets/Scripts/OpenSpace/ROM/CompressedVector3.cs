using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.ROM {
	public struct CompressedVector3 {
		public short x;
		public short y;
		public short z;

		public CompressedVector3(short x, short y, short z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public CompressedVector3(Reader reader) {
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
		}
		public Vector3 GetVector(float factor, bool switchAxes = true) {
			if (switchAxes) {
				return new Vector3(x / factor, z / factor, y / factor);
			} else {
				return new Vector3(x / factor, y / factor, z / factor);
			}
		}
	}
}
