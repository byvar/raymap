using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1QuadNoTexture : OpenSpaceStruct {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort v3;
		public ushort ushort_08;
		public ushort ushort_0A;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			v3 = reader.ReadUInt16();
			ushort_08 = reader.ReadUInt16();
			ushort_0A = reader.ReadUInt16();


			/*R2PS1Loader l = Load as R2PS1Loader;
			PS1VRAM vram = l.vram;
			Texture2D tex = vram.GetTexture(128, 128, ushort_12, ushort_0E, PS1VRAM.PixelMode.Byte, 0, 0);
			Util.ByteArrayToFile(l.gameDataBinFolder + "test_tex/" + Offset.FileOffset + ".png", tex.EncodeToPNG());*/
		}
	}
}
