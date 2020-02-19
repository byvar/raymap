using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Visual.PS2Optimized {
	public class PS2OptimizedGeometricObjectElementTriangles : OpenSpaceStruct {
		public uint unk0;
		public uint unk1;
		public uint unk2;
		public uint num_vertices;
		public Vertex[] vertices;
		public Unknown0[] unknowns0;
		public Unknown1[] unknowns1;

		protected override void ReadInternal(Reader reader) {
			Load.print(Offset);
			unk0 = reader.ReadUInt32();
			unk1 = reader.ReadUInt32();
			unk2 = reader.ReadUInt32();
			num_vertices = reader.ReadUInt32();
			vertices = new Vertex[num_vertices];
			for (int i = 0; i < num_vertices; i++) {
				vertices[i] = new Vertex(reader);
			}
			unknowns0 = new Unknown0[unk2];
			for (int i = 0; i < unknowns0.Length; i++) {
				unknowns0[i] = new Unknown0(reader);
			}
			unknowns1 = new Unknown1[unk2];
			for (int i = 0; i < unknowns1.Length; i++) {
				unknowns1[i] = new Unknown1(reader);
			}
		}

		public class Vertex {
			public float x;
			public float y;
			public float z;
			public float w;

			public Vertex(Reader reader) {
				x = reader.ReadSingle();
				y = reader.ReadSingle();
				z = reader.ReadSingle();
				w = reader.ReadSingle();
			}
		}
		public class Unknown0 {
			public uint unk0;
			public uint unk1;
			public uint unk2;
			public uint unk3;

			public Unknown0(Reader reader) {
				unk0 = reader.ReadUInt32();
				unk1 = reader.ReadUInt32();
				unk2 = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			}
		}
		public class Unknown1 {
			public uint unk0;
			public uint unk1;
			public uint unk2;
			public uint unk3;

			public Unknown1(Reader reader) {
				unk0 = reader.ReadUInt32();
				unk1 = reader.ReadUInt32();
				unk2 = reader.ReadUInt32();
				unk3 = reader.ReadUInt32();
			}
		}
	}
}
