using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class PersoArray : ROMStruct {
		public Reference<Perso>[] persos;
		
		public ushort length;

        protected override void ReadInternal(Reader reader) {
			persos = new Reference<Perso>[length];
			for (int i = 0; i < persos.Length; i++) {
				persos[i] = new Reference<Perso>(reader, true);
			}
        }
    }
}
