using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class LightInfo : ROMStruct {
		// size: 104

		protected override void ReadInternal(Reader reader) {
		}

		public GameObject GetGameObject() {
			GameObject gao = new GameObject("LightInfo @ " + Offset);
			return gao;
		}
	}
}
