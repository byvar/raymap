using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MechanicsMaterial : ROMStruct {
		public float flt0;
		public float flt1;

		protected override void ReadInternal(Reader reader) {
			flt0 = reader.ReadSingle();
			flt1 = reader.ReadSingle();
		}
	}
}
