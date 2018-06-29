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
        public VisualMaterial r3mat;
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
                Mesh meshUnity = new Mesh();
                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(-sprites[i].info_scale.x, -sprites[i].info_scale.y, 0);
                vertices[1] = new Vector3( sprites[i].info_scale.x, -sprites[i].info_scale.y, 0);
                vertices[2] = new Vector3(-sprites[i].info_scale.x,  sprites[i].info_scale.y, 0);
                vertices[3] = new Vector3( sprites[i].info_scale.x,  sprites[i].info_scale.y, 0);
                Vector3[] normals = new Vector3[4];
                normals[0] = Vector3.forward;
                normals[1] = Vector3.forward;
                normals[2] = Vector3.forward;
                normals[3] = Vector3.forward;
                Vector2[] uvs = new Vector2[4];
                uvs[0] = new Vector2(0, 0);
                uvs[1] = new Vector2(1, 0);
                uvs[2] = new Vector2(0, 1);
                uvs[3] = new Vector2(1, 1);
                int[] triangles = new int[] { 0, 2, 1, 1, 2, 3 };

                meshUnity.vertices = vertices;
                meshUnity.normals = normals;
                meshUnity.triangles = triangles;
                meshUnity.uv = uvs;


                GameObject spr_gao = new GameObject(name + " - Sprite " + i);
                spr_gao.transform.SetParent(gao.transform);
                MeshFilter mf = spr_gao.AddComponent<MeshFilter>();
                mf.mesh = meshUnity;
                MeshRenderer mr = spr_gao.AddComponent<MeshRenderer>();
                if (sprites[i].r3mat != null) {
                    Material unityMat = sprites[i].r3mat.Material;
                    bool receiveShadows = (sprites[i].r3mat.properties & VisualMaterial.property_receiveShadows) != 0;
                    //if (num_uvMaps > 1) unityMat.SetFloat("_UVSec", 50f);
                    //if (r3mat.Material.GetColor("_EmissionColor") != Color.black) print("Mesh with emission: " + name);
                    mr.material = unityMat;
                    if (!receiveShadows) mr.receiveShadows = false;
                    if (sprites[i].r3mat.off_animTextures.Count > 0) {
                        MultiTextureMaterial mtmat = mr.gameObject.AddComponent<MultiTextureMaterial>();
                        mtmat.r3mat = sprites[i].r3mat;
                        mtmat.mat = mr.material;
                    }
                }
                if (sprites[i].r3mat != null) {
                    mr.material = sprites[i].r3mat.Material;
                }
            }
        }

        public static SpriteElement Read(EndianBinaryReader reader, Pointer offset, MeshObject m) {
            MapLoader l = MapLoader.Loader;
            SpriteElement s = new SpriteElement(offset, m);
            s.name = "Sprite @ pos " + offset;

            s.off_sprites = Pointer.Read(reader);
            s.num_sprites = reader.ReadUInt16();
            reader.ReadInt16(); // -1
            reader.ReadUInt32();
            reader.ReadUInt32();

            if (s.off_sprites != null) {
                Pointer.Goto(ref reader, s.off_sprites);
                s.sprites = new IndexedSprite[s.num_sprites];
                for (uint i = 0; i < s.num_sprites; i++) {
                    s.sprites[i] = new IndexedSprite();
                    s.sprites[i].off_info = Pointer.Read(reader);
                    s.sprites[i].size = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    s.sprites[i].constraint = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    s.sprites[i].uv1 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    s.sprites[i].uv2 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    s.sprites[i].centerPoint = reader.ReadUInt16();
                    reader.ReadUInt16();
                    if (l.mode == MapLoader.Mode.Rayman2PC) reader.ReadUInt32();

                    if (s.sprites[i].off_info != null) {
                        Pointer off_current = Pointer.Goto(ref reader, s.sprites[i].off_info);
                        reader.ReadUInt32();
                        Pointer.Read(reader);
                        Pointer.Read(reader);
                        Pointer off_info_scale = Pointer.Read(reader);
                        Pointer off_info_unknown = Pointer.Read(reader);
                        s.sprites[i].off_material_pointer = Pointer.Read(reader);
                        Pointer.Goto(ref reader, off_current);
                        
                        if (off_info_scale != null) {
                            off_current = Pointer.Goto(ref reader, off_info_scale);
                            s.sprites[i].info_scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            Pointer.Goto(ref reader, off_current);
                        }
                        if (off_info_unknown != null) {
                            off_current = Pointer.Goto(ref reader, off_info_unknown);
                            s.sprites[i].info_unknown = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            Pointer.Goto(ref reader, off_current);
                        }
                        if (s.sprites[i].off_material_pointer != null) {
                            off_current = Pointer.Goto(ref reader, s.sprites[i].off_material_pointer);
                            s.sprites[i].off_material = Pointer.Read(reader);
                            if (l.mode == MapLoader.Mode.Rayman2PC && s.sprites[i].off_material != null) {
                                Pointer.Goto(ref reader, s.sprites[i].off_material);
                                s.sprites[i].off_material = Pointer.Read(reader);
                            }
                            if (s.sprites[i].off_material != null) {
                                Pointer.Goto(ref reader, s.sprites[i].off_material);
                                s.sprites[i].r3mat = VisualMaterial.FromOffset(s.sprites[i].off_material, createIfNull: true);
                            } else s.sprites[i].r3mat = null;
                            Pointer.Goto(ref reader, off_current);
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
