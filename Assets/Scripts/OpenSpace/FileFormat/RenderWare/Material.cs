using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 material format
	// https://gtamods.com/wiki/RpMaterial
	public class Material {
		public uint flags;
		public Color color;
		public uint unused; // maybe this is really color?
		public bool isTextured;
		public float ambient;
		public float specular;
		public float diffuse;

		public static Material Read(Reader reader) {
			Material m = new Material();
			m.flags = reader.ReadUInt32();
			m.color = new Color(reader.ReadByte() / 255f, reader.ReadByte() / 255f, reader.ReadByte() / 255f, reader.ReadByte() / 255f);
			m.unused = reader.ReadUInt32();
			m.isTextured = reader.ReadInt32() != 0;
			m.ambient = reader.ReadSingle();
			m.specular = reader.ReadSingle();
			m.diffuse = reader.ReadSingle();
			return m;
		}
	}
}