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
		public short short_00;
		public short short_02;
		public short short_04;
		public short short_06;

		// Parsed

		protected override void ReadInternal(Reader reader) {
			short_00 = reader.ReadInt16();
			short_02 = reader.ReadInt16();
			short_04 = reader.ReadInt16();
			short_06 = reader.ReadInt16();
		}
	}
}
