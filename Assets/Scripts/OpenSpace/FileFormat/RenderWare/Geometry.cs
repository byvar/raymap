using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 geometry format
	// https://gtamods.com/wiki/RpGeometry
	public class Geometry {
		public uint formatNumber;
		public GeometryFormat format;
		public uint numTriangles;
		public uint numVertices;
		public uint numMorphTargets;
		public float ambient;
		public float specular;
		public float diffuse;
		public uint numTexSets;
		public Vector2[][] uvs;
		public Triangle[] triangles;
		public MorphTarget[] morphTargets;

		[Flags]
		public enum GeometryFormat {
			TRISTRIP = 0x00000001, //Is triangle strip (if disabled it will be an triangle list)
			POSITIONS = 0x00000002, //Vertex translation
			TEXTURED = 0x00000004, //Texture coordinates
			PRELIT = 0x00000008, //Vertex colors
			NORMALS = 0x00000010, //Store normals
			LIGHT = 0x00000020, //Geometry is lit (dynamic and static)
			MODULATEMATERIALCOLOR = 0x00000040, //Modulate material color
			TEXTURED2 = 0x00000080, //Texture coordinates 2
			NATIVE = 0x01000000 //Native Geometry
		}

		public struct Triangle {
			public ushort vertex2;
			public ushort vertex1;
			public ushort materialId;
			public ushort vertex3;
		}

		public struct Sphere {
			public Vector3 pos;
			public float radius;
		}

		public class MorphTarget {
			public Sphere boundingSphere;
			public bool hasVertices;
			public bool hasNormals;
			public Vector3[] vertices;
			public Vector3[] normals;
		}

		public static Geometry Read(Reader reader) {
			MapLoader l = MapLoader.Loader;
			Geometry g = new Geometry();
			g.formatNumber = reader.ReadUInt32();
			g.format = (GeometryFormat)g.formatNumber;
			g.numTriangles = reader.ReadUInt32();
			g.numVertices = reader.ReadUInt32();
			g.numMorphTargets = reader.ReadUInt32();
			g.ambient = reader.ReadSingle();
			g.specular = reader.ReadSingle();
			g.diffuse = reader.ReadSingle();
			g.numTexSets = (g.formatNumber & 0x00FF0000) << 16;
			if (g.numTexSets == 0) {
				g.numTexSets = (uint)(g.format.HasFlag(GeometryFormat.TEXTURED2) ? 2 : (g.format.HasFlag(GeometryFormat.TEXTURED) ? 1 : 0));
			}
			g.uvs = new Vector2[g.numTexSets][];
			if (!g.format.HasFlag(GeometryFormat.NATIVE)) {
				if (g.format.HasFlag(GeometryFormat.PRELIT)) {
					l.print("Unsupported: GeometryFormat Prelit");
				}
				if (g.format.HasFlag(GeometryFormat.TEXTURED) || g.format.HasFlag(GeometryFormat.TEXTURED2)) {
					for (int i = 0; i < g.numTexSets; i++) {
						g.uvs[i] = new Vector2[g.numVertices];
						for (int j = 0; j < g.numVertices; j++) {
							g.uvs[i][j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
						}
					}
				}
				g.triangles = new Triangle[g.numTriangles];
				for (int i = 0; i < g.numTriangles; i++) {
					g.triangles[i] = new Triangle() {
						vertex2 = reader.ReadUInt16(),
						vertex1 = reader.ReadUInt16(),
						materialId = reader.ReadUInt16(),
						vertex3 = reader.ReadUInt16()
					};
				}
			}
			g.morphTargets = new MorphTarget[g.numMorphTargets];
			for (int i = 0; i < g.numMorphTargets; i++) {
				g.morphTargets[i] = new MorphTarget();
				g.morphTargets[i].boundingSphere = new Sphere() {
					pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
					radius = reader.ReadSingle()
				};
				g.morphTargets[i].hasVertices = reader.ReadUInt32() != 0;
				g.morphTargets[i].hasNormals = reader.ReadUInt32() != 0;
				if (g.morphTargets[i].hasVertices) {
					g.morphTargets[i].vertices = new Vector3[g.numVertices];
					for (int j = 0; j < g.numVertices; j++) {
						g.morphTargets[i].vertices[j] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
					}
				}
				if (g.morphTargets[i].hasNormals) {
					g.morphTargets[i].normals = new Vector3[g.numVertices];
					for (int j = 0; j < g.numVertices; j++) {
						g.morphTargets[i].normals[j] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
					}
				}
			}

			return g;
		}
	}
}