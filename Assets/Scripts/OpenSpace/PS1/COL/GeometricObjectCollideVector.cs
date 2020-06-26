using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObjectCollideVector : OpenSpaceStruct {
		public short x;
		public short y;
		public short z;
		public short short_06;

		// Parsed

		protected override void ReadInternal(Reader reader) {
			x = reader.ReadInt16();
			y = reader.ReadInt16();
			z = reader.ReadInt16();
			short_06 = reader.ReadInt16();
		}
	}
}
