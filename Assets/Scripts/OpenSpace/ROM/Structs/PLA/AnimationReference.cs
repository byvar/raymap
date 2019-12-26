using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class AnimationReference : ROMStruct {
		// Size: 8
		public ushort word0;
		public ushort word2;
		public ushort word4;
		public ushort animIndex;

		protected override void ReadInternal(Reader reader) {
			word0 = reader.ReadUInt16();
			word2 = reader.ReadUInt16();
			word4 = reader.ReadUInt16();
			animIndex = reader.ReadUInt16();
			//MapLoader.Loader.print(word0 + " - " + word2 + " - " + word4);
			//MapLoader.Loader.print(animIndex);
			if (animIndex != 0xFFFF && Loader.cutTable != null) {
				Loader.cutTable.ReadData(animIndex);
			}
		}
	}
}
