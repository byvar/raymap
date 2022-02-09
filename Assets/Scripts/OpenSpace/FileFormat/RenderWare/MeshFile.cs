using BinarySerializer.Unity;
using OpenSpace.Visual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VrSharp.PvrTexture;

namespace OpenSpace.FileFormat.RenderWare {
	// R2 PS2 mesh file
	// https://gtamods.com/wiki/RenderWare_binary_stream_file
	public class MeshFile {
		public Section root;

		public GeometricObject[] meshes;
		public uint numAtomics;
		public uint numMeshes;
		public uint numFrames;

		public MeshFile(string path) {
			Stream fs = FileSystem.GetFileReadStream(path);
			using (Reader reader = new Reader(fs, Settings.s.IsLittleEndian)) {
				root = Section.Read(reader);
			}
			if (root != null && root.type == Section.Type.Clump) {
				numAtomics = (uint)root[0]["numAtomics"];
				numFrames = (uint)root[1][0]["numFrames"];
				Frame[] frames = (Frame[])root[1][0]["frames"];
				numMeshes = (uint)root[2][0]["numMeshes"];
				Section[] meshSections = new Section[numMeshes];
				Array.Copy(root[2].children.ToArray(), 1, meshSections, 0, (int)numMeshes);
				/*for (int i = 3; i < numAtomics + 3; i++) {
					MapLoader.Loader.print("Atomic " + (i - 3) + ": " + root[i][0]["frameIndex"]);
				}*/
				meshes = meshSections.Select(s => ConvertMeshSection(s)).ToArray();
				/*for (int i = 0; i < meshes.Length; i++) {
					MapLoader.Loader.print("ATO Mesh " + i + ": " + meshes[i].num_vertices + " - " + ((MeshElement)meshes[i].subblocks[0]).num_disconnected_triangles);
				}*/
			}
		}

		private GeometricObject ConvertMeshSection(Section s) {
			Geometry g = (Geometry)s[0]["geometry"];
			uint numMaterials = (uint)s[1][0]["numMaterials"];
			uint numUniqueMaterials = (uint)s[1][0]["numUniqueMaterials"];
			int[] materialIndices = (int[])s[1][0]["materialIndices"];

			Section[] materialSections = new Section[numUniqueMaterials];
			VisualMaterial[] materials = new VisualMaterial[numUniqueMaterials];
			Array.Copy(s[1].children.ToArray(), 1, materialSections, 0, (int)numUniqueMaterials);
			materials = materialSections.Select(vms => ConvertMaterialSection(vms)).ToArray();
			//if (numMaterials > 1) MapLoader.Loader.print("NUM MATERIALS " + numMaterials);

			GeometricObject m = new GeometricObject(null);
			m.num_vertices = (ushort)g.numVertices;
			m.normals = g.morphTargets[0].normals;
			m.vertices = g.morphTargets[0].vertices;
			m.num_elements = (ushort)numMaterials;
			m.elements = new IGeometricObjectElement[numMaterials];
			uint currentUniqueMaterial = 0;
			for (int i = 0; i < numMaterials; i++) {
				GeometricObjectElementTriangles e = new GeometricObjectElementTriangles(null, m);
				List<Geometry.Triangle> triangles = new List<Geometry.Triangle>();
				for (int j = 0; j < g.numTriangles; j++) {
					if (g.triangles[j].materialId == i) {
						triangles.Add(g.triangles[j]);
					}
				}
				e.OPT_num_mapping_entries = m.num_vertices;
				e.num_uvMaps = (ushort)g.numTexSets;
				e.OPT_mapping_uvs = new int[e.num_uvMaps][];
				e.uvs = new Vector2[0];
				e.vertexColors = g.vertexColors;
				for (int j = 0; j < g.numTexSets; j++) {
					e.OPT_mapping_uvs[j] = Enumerable.Range(e.num_uvs, m.num_vertices).ToArray();
					Array.Resize(ref e.uvs, e.num_uvs + m.num_vertices);
					Array.Copy(g.uvs[j], 0, e.uvs, e.num_uvs, m.num_vertices);
					e.num_uvs += m.num_vertices;

				}
				e.OPT_mapping_vertices = Enumerable.Range(0, m.num_vertices).ToArray();
				e.OPT_num_disconnectedTriangles = (ushort)triangles.Count;
				e.OPT_disconnectedTriangles = new int[triangles.Count*3];
				for (int j = 0; j < triangles.Count; j++) {
					e.OPT_disconnectedTriangles[j * 3] = triangles[j].vertex1;
					e.OPT_disconnectedTriangles[j * 3+1] = triangles[j].vertex2;
					e.OPT_disconnectedTriangles[j * 3+2] = triangles[j].vertex3;
				}

				e.visualMaterial = materialIndices[i] == -1 ? materials[currentUniqueMaterial] : materials[materialIndices[i]];
				e.visualMaterialOG = e.visualMaterial;
				if (materialIndices[i] == -1) currentUniqueMaterial++;
				m.elements[i] = e;
			}
			m.name = "Mesh";
			//GameObject gao = m.Gao;
			return m;
		}

		private VisualMaterial ConvertMaterialSection(Section s) {
			VisualMaterial vm = new VisualMaterial(null);
			Material m = (Material)s[0]["material"];
			vm.ambientCoef = m.color * m.ambient;
			vm.diffuseCoef = new Color(m.diffuse, m.diffuse, m.diffuse);
			vm.specularCoef = new Color(m.specular, m.specular, m.specular);
			vm.num_textures = 1;
			if (m.isTextured) {
				MaterialTexture mt = (MaterialTexture)s[1][0]["texture"];
				byte[] texName = s[1][1].data;
				byte[] alphaName = s[1][2].data;
				VisualMaterialTexture vmt = new VisualMaterialTexture();
				/*foreach (TextureDictionary txd in txds) {
					Texture2D tex = txd.Lookup(texName, TextureDictionary.Type.Texture);
					if (tex != null) {
						vmt.texture = new TextureInfo(null) {
							width = (ushort)tex.width,
							height = (ushort)tex.height,
							Texture = tex
						};
						break;
					}
				}*/
				vm.textures.Add(vmt);
			} else {
				vm.textures.Add(new VisualMaterialTexture() {
					texture = new TextureInfo(null) {
						width = 1,
						height = 1,
						Texture = Util.CreateDummyTexture()
					}
				});
			}
			
			return vm;
		}
	}
}