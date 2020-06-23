using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class DeformVertexUnknown : OpenSpaceStruct {
		public ushort ind_vertex;
		public ushort unk;
		public short unk0;
		public short unk1;
		public short unk2;
		public short unk3;

		protected override void ReadInternal(Reader reader) {
			ind_vertex = reader.ReadUInt16();
			unk = reader.ReadUInt16();

			unk0 = reader.ReadInt16();
			unk1 = reader.ReadInt16();
			unk2 = reader.ReadInt16();
			unk3 = reader.ReadInt16();
		}
	}
}
