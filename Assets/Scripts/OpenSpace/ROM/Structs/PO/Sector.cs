using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class Sector : ROMStruct {
		// size: 52

		protected override void ReadInternal(Reader reader) {
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("IPO @ " + Offset);
			return gao;
		}
	}
}
