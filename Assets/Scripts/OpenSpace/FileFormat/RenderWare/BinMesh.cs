using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 optimized mesh format
	// https://gtamods.com/wiki/Bin_Mesh_PLG_(RW_Section)
	public class BinMesh {
		public uint flags;
		public uint numMeshes;
		public uint numTotalIndices;
		public BinMeshMesh[] meshes;

		public static BinMesh Read(Reader reader) {
			BinMesh b = new BinMesh();
			b.flags = reader.ReadUInt32();
			b.numMeshes = reader.ReadUInt32();
			b.numTotalIndices = reader.ReadUInt32();
			b.meshes = new BinMeshMesh[b.numMeshes];
			for (int i = 0; i < b.numMeshes; i++) {
				BinMeshMesh m = new BinMeshMesh();
				m.numIndices = reader.ReadUInt32();
				m.materialIndex = reader.ReadUInt32();
				m.indices = new uint[m.numIndices];
				for (int j = 0; j < m.numIndices; j++) {
					m.indices[j] = reader.ReadUInt32();
				}
				b.meshes[i] = m;
			}
			return b;
		}

		public class BinMeshMesh {
			public uint numIndices;
			public uint materialIndex;
			public uint[] indices;
		}
	}
}