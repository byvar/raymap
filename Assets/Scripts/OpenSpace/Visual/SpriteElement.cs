using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
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
		public Mesh meshUnity = null;
    }

    public class SpriteElement : IGeometricElement {
        public MeshObject mesh;
        public Pointer offset;

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
                    BillboardBehaviour billboard = gao.AddComponent<BillboardBehaviour>();
                    billboard.mode = BillboardBehaviour.LookAtMode.ViewRotation;
                    CreateUnityMesh();
                }
                return gao;
            }
        }

        public SpriteElement(Pointer offset, MeshObject mesh) {
            this.mesh = mesh;
            this.offset = offset;
        }

        private void CreateUnityMesh() {
            for (uint i = 0; i < num_sprites; i++) {
                bool mirrorX = false;
                bool mirrorY = false;
                GameObject spr_gao = new GameObject(name + " - Sprite " + i);
                spr_gao.transform.SetParent(gao.transform);
                MeshFilter mf = spr_gao.AddComponent<MeshFilter>();
                MeshRenderer mr = spr_gao.AddComponent<MeshRenderer>();
                BoxCollider bc = spr_gao.AddComponent<BoxCollider>();
                bc.size = new Vector3(0, sprites[i].info_scale.y * 2, sprites[i].info_scale.x * 2);
                spr_gao.layer = LayerMask.NameToLayer("Visual");
                if (sprites[i].visualMaterial != null) {
                    if (sprites[i].visualMaterial.textures != null && sprites[i].visualMaterial.textures.Count > 0) {
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
                        mtmat.mat = mr.sharedMaterial;
                    }
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

        public static SpriteElement Read(Reader reader, Pointer offset, MeshObject m) {
            MapLoader l = MapLoader.Loader;
            SpriteElement s = new SpriteElement(offset, m);
            s.name = "Sprite @ pos " + offset;
            
            if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                if (Settings.s.platform == Settings.Platform.DC) {
                    s.off_sprites = offset;
                    s.num_sprites = 1;
                } else {
                    s.off_sprites = Pointer.Read(reader);
                    s.num_sprites = reader.ReadUInt16();
                    reader.ReadInt16(); // -1
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }
            } else {
                s.num_sprites = (ushort)reader.ReadUInt32();
                s.off_sprites = Pointer.Read(reader);
                reader.ReadUInt32();
            }
            if (Settings.s.platform == Settings.Platform.DC) {
                s.sprites = new IndexedSprite[1];
                s.sprites[0] = new IndexedSprite();
                s.sprites[0].off_material = Pointer.Read(reader);
                if (s.sprites[0].off_material != null) {
                    s.sprites[0].visualMaterial = VisualMaterial.FromOffsetOrRead(s.sprites[0].off_material, reader);
                }
                s.sprites[0].info_scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                reader.ReadUInt16();
                reader.ReadUInt16();
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
                        if (Settings.s.engineVersion <= Settings.EngineVersion.Montreal) reader.ReadUInt32();
                        s.sprites[i].off_info = Pointer.Read(reader);
                        s.sprites[i].size = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                        if (Settings.s.engineVersion > Settings.EngineVersion.Montreal) {
                            s.sprites[i].constraint = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            s.sprites[i].uv1 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            s.sprites[i].uv2 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            s.sprites[i].centerPoint = reader.ReadUInt16();
                            reader.ReadUInt16();
                            if (Settings.s.engineVersion < Settings.EngineVersion.R3) reader.ReadUInt32();
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
                                    if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
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

        public IGeometricElement Clone(MeshObject mesh) {
            SpriteElement sm = (SpriteElement)MemberwiseClone();
            sm.mesh = mesh;
            sm.Reset();
            return sm;
        }
    }
}
