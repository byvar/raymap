using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Collide {
    public class GeometricObjectElementCollideTriangles : IGeometricObjectElementCollide {
        [JsonIgnore] public GeometricObjectCollide geo;
        public LegacyPointer offset;
		
        public LegacyPointer off_material;
        public LegacyPointer off_triangles; // num_triangles * 3 * 0x2
        public LegacyPointer off_mapping; // num_triangles * 3 * 0x2. Max: num_uvs-1
        public LegacyPointer off_normals; // num_triangles * 3 * 0x4. 1 normal per face, kinda logical for collision I guess
        public LegacyPointer off_uvs;
        public ushort num_triangles;
        public ushort num_mapping;
        public LegacyPointer off_unk;
        public LegacyPointer off_unk2;
        public ushort num_mapping_entries;
        public short ind_parallelBox;

        public GameMaterial gameMaterial;
        public int[] triangles = null;
        public Vector3[] normals = null;
        public int[] mapping = null;
        public Vector2[] uvs = null;
		
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject("Collide Submesh @ " + offset);// Create object and read triangle data
                    gao.layer = LayerMask.NameToLayer("Collide");
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public GeometricObjectElementCollideTriangles(LegacyPointer offset, GeometricObjectCollide geo) {
            this.geo = geo;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            if(num_triangles > 0) {
                Vector3[] new_vertices = new Vector3[num_triangles * 3];
                Vector3[] new_normals = null;
                if(normals != null) new_normals = new Vector3[num_triangles * 3];
                Vector2[] new_uvs = new Vector2[num_triangles * 3];

                for (int j = 0; j < num_triangles * 3; j++) {
                    new_vertices[j] = geo.vertices[triangles[j]];
                    if(normals != null) new_normals[j] = normals[j/3];
                    if (uvs != null) {
                        new_uvs[j] = uvs[mapping[j] % uvs.Length];
                    }
                }
                int[] new_triangles = new int[num_triangles * 3];
                for (int j = 0; j < num_triangles; j++) {
                    new_triangles[(j * 3) + 0] = (j * 3) + 0;
                    new_triangles[(j * 3) + 1] = (j * 3) + 2;
                    new_triangles[(j * 3) + 2] = (j * 3) + 1;
                }
                Mesh meshUnity = new Mesh();
                meshUnity.vertices = new_vertices;
                if(normals != null) meshUnity.normals = new_normals;
                meshUnity.triangles = new_triangles;
				if (normals == null) meshUnity.RecalculateNormals();

                // If no UVs exist for collide mesh, generate simple UVs (basically a box projection)
                /*System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
                w.Start();*/
                if (uvs == null) {
                    if (normals == null) {
                        List<Vector3> norm = new List<Vector3>();
                        meshUnity.GetNormals(norm);
                        new_normals = norm.ToArray();
                    }
                    for (int j = 0; j < new_uvs.Length; j++) {
                        Vector3 normal = new_normals[j];
                        normal = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));
                        float biggestNorm = Mathf.Max(normal.x, normal.y, normal.z);
                        
                        float uvX = (new_vertices[j].x / 20.0f);
                        float uvY = (new_vertices[j].y / 20.0f);
                        float uvZ = (new_vertices[j].z / 20.0f);

                        if (biggestNorm == Mathf.Abs(normal.x)) {
                            new_uvs[j] = new Vector2(uvY, uvZ);
                        } else if (biggestNorm == Mathf.Abs(normal.y)) {
                            new_uvs[j] = new Vector2(uvX, uvZ);
                        } else {
                            new_uvs[j] = new Vector2(uvX, uvY);
                        }
                    }
                }
                /*MapLoader.Loader.print("UV Code: " + offset + " - " + num_triangles + " - " + w.ElapsedMilliseconds);
                w.Stop();*/

                meshUnity.uv = new_uvs;

                MeshFilter mf = gao.AddComponent<MeshFilter>();
                MeshRenderer mr = gao.AddComponent<MeshRenderer>();
                mf.mesh = meshUnity;
                try {
                    MeshCollider mc = gao.AddComponent<MeshCollider>();
                    //mc.cookingOptions = MeshColliderCookingOptions.None;
                    //mc.sharedMesh = mf.sharedMesh;
                } catch (Exception) { }

                CollideComponent cc = gao.AddComponent<CollideComponent>();
                cc.collide = this;
                cc.type = geo.type;

                mr.material = MapLoader.Loader.collideMaterial;
                if (gameMaterial != null && gameMaterial.collideMaterial != null) {
                    gameMaterial.collideMaterial.SetMaterial(mr);
                }
                if (geo.type != CollideType.None) {
                    Color col = mr.material.color;
                    mr.material = new Material(MapLoader.Loader.collideTransparentMaterial);
                    mr.material.color = new Color(col.r, col.g, col.b, col.a * 0.7f);
                    switch (geo.type) {
                        case CollideType.ZDD:
                            mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdd")); break;
                        case CollideType.ZDE:
                            mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zde")); break;
                        case CollideType.ZDM:
                            mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdm")); break;
                        case CollideType.ZDR:
                            mr.material.SetTexture("_MainTex", Resources.Load<Texture2D>("Textures/zdr")); break;
                    }
                }
            }
        }

        public static GeometricObjectElementCollideTriangles Read(Reader reader, LegacyPointer offset, GeometricObjectCollide geo) {
            MapLoader l = MapLoader.Loader;
            GeometricObjectElementCollideTriangles sm = new GeometricObjectElementCollideTriangles(offset, geo);
            sm.off_material = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Revolution || Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
				sm.num_triangles = reader.ReadUInt16();
				reader.ReadUInt16();
				sm.off_triangles = LegacyPointer.Read(reader);
				if (Legacy_Settings.s.game == Legacy_Settings.Game.LargoWinch) {
					sm.off_normals = LegacyPointer.Read(reader);
					sm.off_unk = LegacyPointer.Read(reader);
				}
			} else {
				if (Legacy_Settings.s.engineVersion < Legacy_Settings.EngineVersion.R3) {
					sm.num_triangles = reader.ReadUInt16();
					sm.num_mapping = reader.ReadUInt16();
					sm.off_triangles = LegacyPointer.Read(reader);
					sm.off_mapping = LegacyPointer.Read(reader);
					sm.off_normals = LegacyPointer.Read(reader);
					sm.off_uvs = LegacyPointer.Read(reader);
					if (Legacy_Settings.s.engineVersion == Legacy_Settings.EngineVersion.Montreal) {
						reader.ReadUInt32();
					}
					if (Legacy_Settings.s.game != Legacy_Settings.Game.TTSE && Legacy_Settings.s.game != Legacy_Settings.Game.R2Beta) {
						LegacyPointer.Read(reader); // table of num_unk vertex indices (vertices, because max = num_vertices - 1)
						reader.ReadUInt16(); // num_unk
						sm.ind_parallelBox = reader.ReadInt16();
					}
				} else {
					sm.off_triangles = LegacyPointer.Read(reader);
					sm.off_normals = LegacyPointer.Read(reader);
					sm.num_triangles = reader.ReadUInt16();
					sm.ind_parallelBox = reader.ReadInt16();
					reader.ReadUInt32();
					if (Legacy_Settings.s.game != Legacy_Settings.Game.Dinosaur) {
						sm.off_mapping = LegacyPointer.Read(reader);
						sm.off_unk = LegacyPointer.Read(reader); // num_mapping_entries * 3 floats 
						sm.off_unk2 = LegacyPointer.Read(reader); // num_mapping_entries * 1 float
						sm.num_mapping = reader.ReadUInt16();
						reader.ReadUInt16();
					}
				}
			}
            if (!geo.isBoundingVolume) {
                if (sm.off_material != null) sm.gameMaterial = GameMaterial.FromOffsetOrRead(sm.off_material, reader);
            } else {
                // Sector superobject
            }
            LegacyPointer.Goto(ref reader, sm.off_triangles);
            sm.triangles = new int[sm.num_triangles * 3];
            for (int j = 0; j < sm.num_triangles; j++) {
                sm.triangles[(j * 3) + 0] = reader.ReadInt16();
                sm.triangles[(j * 3) + 1] = reader.ReadInt16();
                sm.triangles[(j * 3) + 2] = reader.ReadInt16();
            }
			LegacyPointer.DoAt(ref reader, sm.off_normals, () => {
				sm.normals = new Vector3[sm.num_triangles];
				for (int j = 0; j < sm.num_triangles; j++) {
					float x = reader.ReadSingle();
					float z = reader.ReadSingle();
					float y = reader.ReadSingle();
					sm.normals[j] = new Vector3(x, y, z);
				}
			});

            if (sm.num_mapping > 0 && sm.off_mapping != null) {
                LegacyPointer.Goto(ref reader, sm.off_mapping);
                sm.mapping = new int[sm.num_triangles * 3];
                for (int i = 0; i < sm.num_triangles; i++) {
                    sm.mapping[(i * 3) + 0] = reader.ReadInt16();
                    sm.mapping[(i * 3) + 1] = reader.ReadInt16();
                    sm.mapping[(i * 3) + 2] = reader.ReadInt16();
                }
                if (sm.off_uvs != null) {
                    LegacyPointer.Goto(ref reader, sm.off_uvs);
                    sm.uvs = new Vector2[sm.num_mapping];
                    for (int i = 0; i < sm.num_mapping; i++) {
                        sm.uvs[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    }
                }
            }

            /*R3Pointer.Goto(ref reader, sm.off_mapping);
            sm.mapping = new int[sm.num_triangles * 3];
            for (int j = 0; j < sm.num_triangles; j++) {
                sm.mapping[(j * 3) + 0] = reader.ReadInt16();
                sm.mapping[(j * 3) + 1] = reader.ReadInt16();
                sm.mapping[(j * 3) + 2] = reader.ReadInt16();
            }
            R3Pointer.Goto(ref reader, sm.off_unk);
            sm.normals = new Vector3[sm.num_mapping_entries];
            for (int j = 0; j < sm.num_mapping_entries; j++) {
                float x = reader.ReadSingle();
                float z = reader.ReadSingle();
                float y = reader.ReadSingle();
                sm.normals[j] = new Vector3(x, y, z);
            }*/
            return sm;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public IGeometricObjectElementCollide Clone(GeometricObjectCollide mesh) {
            GeometricObjectElementCollideTriangles sm = (GeometricObjectElementCollideTriangles)MemberwiseClone();
            sm.geo = mesh;
            sm.Reset();
            return sm;
        }

        public GameMaterial GetMaterial(int? index) {
            return gameMaterial;
        }
    }
}
