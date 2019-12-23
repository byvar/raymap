using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ShortArray : ROMStruct {
		public ushort length;
		public ushort[] shorts;

		protected override void ReadInternal(Reader reader) {
			shorts = new ushort[length];
			for (int i = 0; i < length; i++) {
				shorts[i] = reader.ReadUInt16();
			}
		}
	}
}
