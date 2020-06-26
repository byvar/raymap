using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using OpenSpace.PS1.GLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class GeometricObjectCollideTriangle : OpenSpaceStruct, IPS1PolygonCollide {
		public byte flag0;
		public byte gameMaterial;
		public ushort normal;

		// Vertices
		public ushort v0;
		public ushort v1;
		public ushort v2;

		// Unknown
		public short x0;
		public short y0;
		public short z0;
		public short x1;
		public short y1;
		public short z1;
		public short x2;
		public short y2;
		public short z2;

		protected override void ReadInternal(Reader reader) {
			flag0 = reader.ReadByte();
			gameMaterial = reader.ReadByte();
			normal = reader.ReadUInt16();
			v0 = reader.ReadUInt16();
			v1 = reader.ReadUInt16();
			v2 = reader.ReadUInt16();
			x0 = reader.ReadInt16();
			y0 = reader.ReadInt16();
			z0 = reader.ReadInt16();
			x1 = reader.ReadInt16();
			y1 = reader.ReadInt16();
			z1 = reader.ReadInt16();
			x2 = reader.ReadInt16();
			y2 = reader.ReadInt16();
			z2 = reader.ReadInt16();
		}

		public byte MaterialIndex => gameMaterial;
	}
}
