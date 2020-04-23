using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class PS1TriangleList : OpenSpaceStruct { // TextureTable
		public ushort type;
		public ushort ushort_02;
		public Pointer off_triangles;
		public uint length;

		public PS1Triangle[] triangles;
		public PS1TriangleNoTexture[] trianglesNoTex;
		public PS1QuadNoTexture[] quadsNoTex;
		public PS1Quad[] quads;
		public PS1QuadLOD[] quadsLOD;

		protected override void ReadInternal(Reader reader) {
			type = reader.ReadUInt16();
			ushort_02 = reader.ReadUInt16();
			off_triangles = Pointer.Read(reader);
			Pointer.DoAt(ref reader, off_triangles, () => {
				length = reader.ReadUInt32();
				if (type == 5) {
					triangles = Load.ReadArray<PS1Triangle>(length, reader);
				} else if (type == 6) {
					quads = Load.ReadArray<PS1Quad>(length, reader);
				} else if(type == 4) {
					quadsNoTex = Load.ReadArray<PS1QuadNoTexture>(length, reader);
				} else if(type == 3) {
					trianglesNoTex = Load.ReadArray<PS1TriangleNoTexture>(length, reader);
				} else if (type == 1) {
					quadsLOD = Load.ReadArray<PS1QuadLOD>(length, reader);
				} else {
					UnityEngine.Debug.LogWarning(type + " - " + Offset);
				}
			});
		}
	}
}
