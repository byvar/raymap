using OpenSpace.Loader;
using System;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class MeshElementTriangles : ROMStruct {
		public ushort length;
		public ushort num_vertices;
		public Triangle[] triangles;
		public Vector2[] uvs;
		public CompressedVector3[] verts;
		public CompressedVector3[] normals;

		protected override void ReadInternal(Reader reader) {
			MapLoader.Loader.print("Triangles: " + Pointer.Current(reader) + " - " + string.Format("{0:X4}", length) + " - " + string.Format("{0:X4}", num_vertices));
			// For DS: https://github.com/scurest/apicula

			// For N64: http://www.shootersforever.com/forums_message_boards/viewtopic.php?t=6920
			// Or RSP commands sheet
			// maybe this can help https://github.com/ricrpi/mupen64plus-video-gles2rice/blob/master/src/RSP_Parser.cpp
			// 8 bytes per command, 1st byte is RSP byte

			if (Settings.s.platform == Settings.Platform._3DS) {
				triangles = new Triangle[length];
				for (int i = 0; i < length; i++) {
					/*if (i % 2 == 0) {
						triangles[i].v2 = reader.ReadUInt16();
						triangles[i].v1 = reader.ReadUInt16();
						triangles[i].v3 = reader.ReadUInt16();
					} else {*/
						triangles[i].v1 = reader.ReadUInt16();
						triangles[i].v2 = reader.ReadUInt16();
						triangles[i].v3 = reader.ReadUInt16();
					//}
				}
				uvs = new Vector2[num_vertices];
				for (int i = 0; i < num_vertices; i++) {
					uvs[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				}
				verts = new CompressedVector3[num_vertices];
				normals = new CompressedVector3[num_vertices];
				for (int i = 0; i < num_vertices; i++) {
					verts[i] = new CompressedVector3(reader);
				}
				for (int i = 0; i < num_vertices; i++) {
					normals[i] = new CompressedVector3(reader);
				}
			}
		}


		public struct Triangle {
			public ushort v1;
			public ushort v2;
			public ushort v3;
		}
	}
}
