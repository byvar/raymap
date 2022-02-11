using OpenSpace.Loader;
using OpenSpace.ROM.RSP;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class GeometricObjectElementTrianglesData : ROMStruct {
		public ushort length;
		public ushort compressedLength;
		// 3DS
		public ushort num_vertices;
		public Triangle[] triangles;
		public Vector2[] uvs;
		public CompressedVector3[] verts;
		public CompressedVector3[] colors;

		// N64
		public byte[] data;
		public RSPCommand[] rspCommands;

		// DS
		public DS3D.GeometryCommand[] ds3dCommands;

		protected override void ReadInternal(Reader reader) {
			//MapLoader.Loader.print("Triangles: " + Pointer.Current(reader) + " - " + string.Format("{0:X4}", length) + " - " + string.Format("{0:X4}", num_vertices));
			// For DS: https://github.com/scurest/apicula
			// http://problemkaputt.de/gbatek.htm#ds3dvideo check under Geometry Commands

			// For N64: http://www.shootersforever.com/forums_message_boards/viewtopic.php?t=6920
			// Or RSP commands sheet
			// maybe this can help https://github.com/ricrpi/mupen64plus-video-gles2rice/blob/master/src/RSP_Parser.cpp
			// 8 bytes per command, 1st byte is RSP byte

			if (CPA_Settings.s.platform == CPA_Settings.Platform._3DS) {
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
				colors = new CompressedVector3[num_vertices];
				for (int i = 0; i < num_vertices; i++) {
					verts[i] = new CompressedVector3(reader);
				}
				for (int i = 0; i < num_vertices; i++) {
					colors[i] = new CompressedVector3(reader);
				}
			} else {
				if (CPA_Settings.s.platform == CPA_Settings.Platform.N64) {
					data = reader.ReadBytes(length);
					using (MemoryStream str = new MemoryStream(data)) {
						using (Reader dataReader = new Reader(str, CPA_Settings.s.IsLittleEndian)) {
							rspCommands = new RSPCommand[length / 8];
							for (int i = 0; i < rspCommands.Length; i++) {
								rspCommands[i] = new RSPCommand(dataReader);
							}
						}
					}
				} else if (CPA_Settings.s.platform == CPA_Settings.Platform.DS) {
					if (CPA_Settings.s.game == CPA_Settings.Game.RRR) {
						data = reader.ReadBytes(compressedLength);
						data = DS3D.GeometryParser.Decompress(data);
						//data = DS3D.GeometryParser.ReadCompressed(reader);
					} else {
						data = reader.ReadBytes(length);
					}
					ds3dCommands = DS3D.GeometryParser.ReadCommands(data);
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
