using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObjectCollide : OpenSpaceStruct {
		public ushort num_vertices;
		public ushort num_normals;
		public ushort num_triangles;
		public ushort ushort_06;
		public byte[] unknownBytes;
		public Pointer off_vertices;
		public Pointer off_normals;
		public Pointer off_triangles;
		public uint uint_34;
		public uint uint_38;

		// Parsed
		public GeometricObjectCollideVector[] vertices;
		public GeometricObjectCollideVector[] normals;
		public GeometricObjectCollideTriangle[] triangles;

		protected override void ReadInternal(Reader reader) {
			//Load.print(Offset);
			num_vertices = reader.ReadUInt16();
			num_normals = reader.ReadUInt16();
			num_triangles = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
			unknownBytes = reader.ReadBytes(0x20);
			off_vertices = Pointer.Read(reader);
			off_normals = Pointer.Read(reader);
			off_triangles = Pointer.Read(reader);
			uint_34 = reader.ReadUInt32();
			uint_38 = reader.ReadUInt32();

			vertices = Load.ReadArray<GeometricObjectCollideVector>(num_vertices, reader, off_vertices);
			normals = Load.ReadArray<GeometricObjectCollideVector>(num_normals, reader, off_normals);
			triangles = Load.ReadArray<GeometricObjectCollideTriangle>(num_triangles, reader, off_triangles);
		}
	}
}
