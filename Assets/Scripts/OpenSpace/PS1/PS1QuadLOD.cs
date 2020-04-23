using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1QuadLOD : OpenSpaceStruct {
		public ushort v0;
		public ushort v1;
		public ushort v2;
		public ushort v3;
		public ushort ushort_08;
		public ushort ushort_0A;
		public ushort ushort_0C;
		public ushort ushort_0E;
		public ushort ushort_10;
		public ushort ushort_12;
		public ushort ushort_14;
		public ushort ushort_16;
		public Pointer off_actualQuads;

		public uint length;
		public PS1Quad[] quads;

		protected override void ReadInternal(Reader reader) {
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			v3 = reader.ReadUInt16();
			ushort_08 = reader.ReadUInt16();
			ushort_0A = reader.ReadUInt16();
			ushort_0C = reader.ReadUInt16();
			ushort_0E = reader.ReadUInt16();// palette info?
			ushort_10 = reader.ReadUInt16();
			ushort_12 = reader.ReadUInt16();// page info?
			ushort_14 = reader.ReadUInt16();
			ushort_16 = reader.ReadUInt16();
			off_actualQuads = Pointer.Read(reader);

			Pointer.DoAt(ref reader, off_actualQuads, () => {
				length = reader.ReadUInt32();
				quads = Load.ReadArray<PS1Quad>(length, reader);
			});


			/*R2PS1Loader l = Load as R2PS1Loader;
			PS1VRAM vram = l.vram;
			Texture2D tex = vram.GetTexture(128, 128, ushort_12, ushort_0E, PS1VRAM.PixelMode.Byte, 0, 0);
			Util.ByteArrayToFile(l.gameDataBinFolder + "test_tex/" + Offset.FileOffset + ".png", tex.EncodeToPNG());*/
		}
	}
}
