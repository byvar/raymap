using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ComportList : ROMStruct {
		// Size: 8
		public Reference<ComportArray> comports;
		public Reference<Comport> currentComport;
		public ushort num_comports;
		public ushort word6;

		protected override void ReadInternal(Reader reader) {
			comports = new Reference<ComportArray>(reader);
			currentComport = new Reference<Comport>(reader, true);
			num_comports = reader.ReadUInt16();
			word6 = reader.ReadUInt16();
			comports.Resolve(reader, ca => ca.length = num_comports);
		}
	}
}