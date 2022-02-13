using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GameMaterial : OpenSpaceStruct {
		public ushort ushort_00;
		public ushort ushort_02;
		public LegacyPointer off_collideMaterial;
		public int int_08;
		public uint uint_0C;

		// Parsed
		public CollideMaterial collideMaterial;

		protected override void ReadInternal(Reader reader) {
			ushort_00 = reader.ReadUInt16();
			ushort_02 = reader.ReadUInt16();
			off_collideMaterial = LegacyPointer.Read(reader);
			int_08 = reader.ReadInt32();
			uint_0C = reader.ReadUInt32();

			collideMaterial = Load.FromOffsetOrRead<CollideMaterial>(reader, off_collideMaterial);
		}
		public Material CreateMaterial() {
			if (collideMaterial != null) return collideMaterial.CreateMaterial();
			Material mat = new Material(MapLoader.Loader.collideMaterial);
			return mat;
		}
	}
}
