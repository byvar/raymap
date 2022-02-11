using Newtonsoft.Json;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    public class GeometricObjectElementSprites : IGeometricObjectElement {
        public class IndexedSprite {
            public Pointer off_info;
            public Vector2 size;
            public Vector3 constraint;
            public Vector2 uv1;
            public Vector2 uv2;
            public ushort centerPoint;

            public Vector2 info_scale;
            public Vector2 info_unknown;
            public Pointer off_material_pointer;
            public Pointer off_material;
            public GameMaterial gameMaterial;
            public VisualMaterial visualMaterial = null;
            [JsonIgnore] public Mesh meshUnity = null;
        }

        [JsonIgnore] public GeometricObject geo;
        public Pointer offset;

        [JsonIgnore]
        public string name;
        public Pointer off_sprites; // called IndexedSprites in the game code
        public ushort num_sprites;
        public IndexedSprite[] sprites;
		
        private GameObject gao = null;
        public GameObject Gao {
            get {
                if (gao == null) {
                    gao = new GameObject(name);// Create object and read triangle data
                    gao.layer = LayerMask.NameToLayer("Visual");
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public GeometricObjectElementSprites(Pointer offset, GeometricObject geo) {
            this.geo = geo;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            for (uint i = 0; i < num_sprites; i++) {
                bool mirrorX = false;
                bool mirrorY = false;
                GameObject spr_gao = new GameObject(name + " - Sprite " + i);
                spr_gao.transform.SetParent(gao.transform);
				spr_gao.transform.localPosition = geo.vertices[sprites[i].centerPoint];
				BillboardBehaviour billboard = spr_gao.AddComponent<BillboardBehaviour>();
				billboard.mode = BillboardBehaviour.LookAtMode.ViewRotation;
				MeshFilter mf = spr_gao.AddComponent<MeshFilter>();
                MeshRenderer mr = spr_gao.AddComponent<MeshRenderer>();
                BoxCollider bc = spr_gao.AddComponent<BoxCollider>();
                bc.size = new Vector3(0, sprites[i].info_scale.y * 2, sprites[i].info_scale.x * 2);
                spr_gao.layer = LayerMask.NameToLayer("Visual");
				if (sprites[i].visualMaterial != null) {
					if (CPA_Settings.s.game != CPA_Settings.Game.R2Revolution &&
						sprites[i].visualMaterial.textures != null &&
						sprites[i].visualMaterial.textures.Count > 0) {
						TextureInfo mainTex = sprites[i].visualMaterial.textures[0].texture;

						if (mainTex != null && mainTex.IsMirrorX) mirrorX = true;
						if (mainTex != null && mainTex.IsMirrorY) mirrorY = true;
					}
					//Material unityMat = sprites[i].visualMaterial.MaterialBillboard;
					Material unityMat = sprites[i].visualMaterial.GetMaterial(VisualMaterial.Hint.Billboard);
					bool receiveShadows = (sprites[i].visualMaterial.properties & VisualMaterial.property_receiveShadows) != 0;
					//if (num_uvMaps > 1) unityMat.SetFloat("_UVSec", 50f);
					//if (r3mat.Material.GetColor("_EmissionColor") != Color.black) print("Mesh with emission: " + name);
					mr.sharedMaterial = unityMat;
					/*mr.material.SetFloat("_ScaleX", sprites[i].info_scale.x);
                    mr.material.SetFloat("_ScaleY", sprites[i].info_scale.y);*/
					if (!receiveShadows) mr.receiveShadows = false;
					if (sprites[i].visualMaterial.animTextures.Count > 0) {
						MultiTextureMaterial mtmat = mr.gameObject.AddComponent<MultiTextureMaterial>();
						mtmat.visMat = sprites[i].visualMaterial;
						mtmat.mat = mr.material;
					}
				} else {
					Material transMat = new Material(MapLoader.Loader.baseTransparentMaterial);
					Texture2D tex = new Texture2D(1, 1);
					tex.SetPixel(0, 0, new Color(0, 0, 0, 0));
					transMat.SetTexture("_Tex0", tex);
					mr.sharedMaterial = transMat;
				}
				if (sprites[i].meshUnity == null) {
					sprites[i].meshUnity = new Mesh();
					Vector3[] vertices = new Vector3[4];
					vertices[0] = new Vector3(0, -sprites[i].info_scale.y, -sprites[i].info_scale.x);
					vertices[1] = new Vector3(0, -sprites[i].info_scale.y, sprites[i].info_scale.x);
					vertices[2] = new Vector3(0, sprites[i].info_scale.y, -sprites[i].info_scale.x);
					vertices[3] = new Vector3(0, sprites[i].info_scale.y, sprites[i].info_scale.x);
					Vector3[] normals = new Vector3[4];
					normals[0] = Vector3.forward;
					normals[1] = Vector3.forward;
					normals[2] = Vector3.forward;
					normals[3] = Vector3.forward;
					Vector3[] uvs = new Vector3[4];
					uvs[0] = new Vector3(0, 0 - (mirrorY ? 1 : 0), 1);
					uvs[1] = new Vector3(1 + (mirrorX ? 1 : 0), 0 - (mirrorY ? 1 : 0), 1);
					uvs[2] = new Vector3(0, 1, 1);
					uvs[3] = new Vector3(1 + (mirrorX ? 1 : 0), 1, 1);
					int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

					sprites[i].meshUnity.vertices = vertices;
					sprites[i].meshUnity.normals = normals;
					sprites[i].meshUnity.triangles = triangles;
					sprites[i].meshUnity.SetUVs(0, uvs.ToList());
				}

                
                mf.sharedMesh = sprites[i].meshUnity;
            }
        }

        public static GeometricObjectElementSprites Read(Reader reader, Pointer offset, GeometricObject m) {
            MapLoader l = MapLoader.Loader;
            GeometricObjectElementSprites s = new GeometricObjectElementSprites(offset, m);
            s.name = "Sprite @ pos " + offset;
			//l.print(s.name);
            
            if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
                if (CPA_Settings.s.platform == CPA_Settings.Platform.DC) {
                    s.off_sprites = offset;
                    s.num_sprites = 1;
                } else {
                    s.off_sprites = Pointer.Read(reader);
                    s.num_sprites = reader.ReadUInt16();
                    reader.ReadInt16(); // -1
					if (CPA_Settings.s.game != CPA_Settings.Game.R2Revolution) {
						reader.ReadUInt32();
						if(CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) reader.ReadUInt32();
					}
                }
            } else {
                s.num_sprites = (ushort)reader.ReadUInt32();
                s.off_sprites = Pointer.Read(reader);
                reader.ReadUInt32();
            }
			if (CPA_Settings.s.game == CPA_Settings.Game.R2Revolution) {
				Pointer.DoAt(ref reader, s.off_sprites, () => {
					s.sprites = new IndexedSprite[s.num_sprites];
					for (uint i = 0; i < s.num_sprites; i++) {
						s.sprites[i] = new IndexedSprite();
						uint type = reader.ReadUInt32();
						s.sprites[i].info_scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
						if (type == 0x20) {
							// Light cookie sprite
							uint index = reader.ReadUInt32();
							R2PS2Loader ps2l = MapLoader.Loader as R2PS2Loader;
							s.sprites[i].visualMaterial = ps2l.lightCookieMaterial.Clone();
							s.sprites[i].visualMaterial.diffuseCoef = ps2l.lightCookieColors[index];
						} else {
							s.sprites[i].off_material = Pointer.Read(reader);
							if (s.sprites[i].off_material != null) {
								s.sprites[i].visualMaterial = VisualMaterial.FromOffsetOrRead(s.sprites[0].off_material, reader);
							}
						}
					}
				});
			} else if (CPA_Settings.s.platform == CPA_Settings.Platform.DC) {
                s.sprites = new IndexedSprite[1];
                s.sprites[0] = new IndexedSprite();
                s.sprites[0].off_material = Pointer.Read(reader);
                if (s.sprites[0].off_material != null) {
                    s.sprites[0].visualMaterial = VisualMaterial.FromOffsetOrRead(s.sprites[0].off_material, reader);
                }
                s.sprites[0].info_scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                reader.ReadUInt16();
                s.sprites[0].centerPoint = reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt16();
            } else {
                if (s.off_sprites != null) {
                    Pointer.Goto(ref reader, s.off_sprites);
                    s.sprites = new IndexedSprite[s.num_sprites];
                    for (uint i = 0; i < s.num_sprites; i++) {
                        s.sprites[i] = new IndexedSprite();
						if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.Montreal) reader.ReadUInt32();
						s.sprites[i].off_info = Pointer.Read(reader);
						s.sprites[i].size = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                        if (CPA_Settings.s.engineVersion > CPA_Settings.EngineVersion.Montreal) {
							if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
								s.sprites[i].constraint = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
								s.sprites[i].uv1 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
								s.sprites[i].uv2 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
							}
                            s.sprites[i].centerPoint = reader.ReadUInt16();
                            reader.ReadUInt16();
                            if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) reader.ReadUInt32();
                        }

                        if (s.sprites[i].off_info != null) {
                            Pointer off_current = Pointer.Goto(ref reader, s.sprites[i].off_info);
                            reader.ReadUInt32();
                            Pointer.Read(reader);
                            Pointer.Read(reader);
                            Pointer off_info_scale = Pointer.Read(reader);
                            Pointer off_info_unknown = Pointer.Read(reader);
                            s.sprites[i].off_material_pointer = Pointer.Read(reader);
                            Pointer.Goto(ref reader, off_current);

                            Pointer.DoAt(ref reader, off_info_scale, () => {
                                s.sprites[i].info_scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            });
                            Pointer.DoAt(ref reader, off_info_unknown, () => {
                                s.sprites[i].info_unknown = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            });
                            if (s.sprites[i].off_material_pointer != null) {
                                off_current = Pointer.Goto(ref reader, s.sprites[i].off_material_pointer);
                                s.sprites[i].off_material = Pointer.Read(reader);
                                if (s.sprites[i].off_material != null) {
                                    if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
                                        s.sprites[i].gameMaterial = GameMaterial.FromOffsetOrRead(s.sprites[i].off_material, reader);
                                        s.sprites[i].visualMaterial = s.sprites[i].gameMaterial.visualMaterial;
                                    } else {
                                        s.sprites[i].visualMaterial = VisualMaterial.FromOffsetOrRead(s.sprites[i].off_material, reader);
                                    }
                                }
                                Pointer.Goto(ref reader, off_current);
                            }
                        }
                    }
                }
            }

            return s;
        }

        // Call after clone
        public void Reset() {
            gao = null;
        }

        public IGeometricObjectElement Clone(GeometricObject mesh) {
            GeometricObjectElementSprites sm = (GeometricObjectElementSprites)MemberwiseClone();
            sm.geo = mesh;
            sm.Reset();
            return sm;
        }
    }
}
