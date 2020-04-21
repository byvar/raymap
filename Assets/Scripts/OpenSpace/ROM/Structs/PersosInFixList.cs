using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PersosInFixList : ROMStruct {
		public Reference<PersoArray> persos;
		public ushort num_persos;

        protected override void ReadInternal(Reader reader) {
			persos = new Reference<PersoArray>(reader);
			num_persos = reader.ReadUInt16();
			persos.Resolve(reader, onPreRead: a => a.length = num_persos);
        }
    }
}
