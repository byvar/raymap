using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1AnimationVector : OpenSpaceStruct {
		public short x;
		public short y;
		public short z;
		public Vector3 vector;

		protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			vector = GetVector();
		}
		public Vector3 GetVector(float factor = R2PS1Loader.CoordinateFactor, bool switchAxes = true) {
			if (switchAxes) {
				return new Vector3(x / factor, z / factor, y / factor);
			} else {
				return new Vector3(x / factor, y / factor, z / factor);
			}
		}
	}
}
