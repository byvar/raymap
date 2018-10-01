using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Visual Material definition
    /// </summary>
    public class VisualMaterial {
        public List<VisualMaterialTexture> textures;
        public List<AnimatedTexture> animTextures;
        public uint flags;
        public Pointer offset;
        public Vector4 ambientCoef;
        public Vector4 diffuseCoef;
        public Vector4 specularCoef;
        public Vector4 color;
        public uint num_textures;

        public Pointer off_animTextures_first;
        public Pointer off_animTextures_current;
        public ushort num_animTextures;

        public byte properties;
        private Material material;
        private Material materialBillboard;

        // UV scrolling
        public int currentAnimTexture = 0;

        // flags
        public static uint flags_isTransparent = (1 << 3);
        public static uint flags_backfaceCulling = (1 << 10);
        public static uint flags_isMaterialChromed = (1 << 22);

        //properties
        public static uint property_receiveShadows = 2;
        public static uint property_isSpriteGenerator = 4;
        public static uint property_isAnimatedSpriteGenerator = 12;
        public static uint property_isGrass = 0x2000;
        public static uint property_isWater = 0x1000;

        public bool ScrollingEnabled {
            get {
                return textures.Where(t => t != null && t.ScrollingEnabled).Count() > 0;
            }
        }

        // TODO: Split material into material_main and material_light, find how these are stored differently.
        public Material Material {
            get {
                if (material == null) {
                    MapLoader l = MapLoader.Loader;
                    //bool backfaceCulling = ((flags & flags_backfaceCulling) == flags_backfaceCulling); // example: 4DDC43FF
                    Material baseMaterial = l.baseMaterial;
                    bool transparent = IsTransparent;
                    if (textures.Where(t => (t.properties & 0x20) != 0).Count() > 0 || IsLight) {
                        baseMaterial = l.baseLightMaterial;
                    } else if (transparent) {
                        baseMaterial = l.baseTransparentMaterial;
                    }
                    //if (textureTypes.Where(i => ((i & 1) != 0)).Count() > 0) baseMaterial = loader.baseLightMaterial;
                    material = new Material(baseMaterial);
                    if (textures != null) {
                        material.SetFloat("_NumTextures", num_textures);
                        for (int i = 0; i < num_textures; i++) {
                            string textureName = "_MainTex" + (i == 0 ? "" : (i + 1).ToString());
                            if (textures[i].texture != null) {
                                material.SetTexture(textureName, textures[i].texture.Texture);
                                material.SetTextureOffset(textureName, new Vector2(textures[i].texture.currentScrollX, textures[i].texture.currentScrollY));
                            } else {
                                // No texture = just color. So create white texture and let that be colored by other properties.
                                Texture2D tex = new Texture2D(1, 1);
                                tex.SetPixel(0, 0, new Color(1, 1, 1, 1));
                                tex.Apply();
                                material.SetTexture(textureName, tex);
                            }
                        }
                    }
                    if (baseMaterial == l.baseMaterial || baseMaterial == l.baseTransparentMaterial) {
                        material.SetVector("_AmbientCoef", ambientCoef);
                        material.SetVector("_SpecularCoef", specularCoef);
                        material.SetVector("_DiffuseCoef", diffuseCoef);
                        material.SetVector("_Color", color);
                        if (IsPixelShaded) material.SetFloat("_ShadingMode", 1f);
                    }
                }
                return material;
            }
        }

        public Material MaterialBillboard {
            get {
                if (materialBillboard == null) {
                    MapLoader l = MapLoader.Loader;
                    //bool backfaceCulling = ((flags & flags_backfaceCulling) == flags_backfaceCulling); // example: 4DDC43FF
                    TextureInfo texMain = null, texSecondary = null;
                    if (textures != null && textures.Count > 0) {
                        texMain = textures[0].texture;
                        if (textures.Count > 1) {
                            texSecondary = textures[1].texture;
                        }
                    }
                    Material baseMaterial = l.billboardMaterial;
                    if (textures.Where(t => (t.properties & 0x20) != 0).Count() > 0 || IsLight) {
                        if (l.billboardAdditiveMaterial != null) {
                            baseMaterial = l.billboardAdditiveMaterial;
                        }
                    }
                    bool transparent = IsTransparent;
                    materialBillboard = new Material(baseMaterial);
                    //materialBillboard.SetColor("_EmissionColor", new Color(ambientCoef.x / 2f, ambientCoef.y / 2f, ambientCoef.z / 2f, ambientCoef.w));
                    if (color.w > 0) {
                        materialBillboard.SetColor("_Color", new Color(color.x, color.y, color.z, color.w));
                    } else {
                        materialBillboard.SetColor("_Color", new Color(diffuseCoef.x, diffuseCoef.y, diffuseCoef.z, diffuseCoef.w));
                    }
                    //materialBillboard.SetColor("_Color", new Color(ambientCoef.x, ambientCoef.y, ambientCoef.z));
                    if (texMain != null) materialBillboard.SetTexture("_MainTex", texMain.Texture);
                    if (texMain == null || texMain.Texture == null) {
                        // No texture = just color. So create white texture and let that be colored by other properties.
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, new Color(1, 1, 1, 1));
                        tex.Apply();
                        materialBillboard.SetTexture("_MainTex", tex);
                    }
                }
                return materialBillboard;
            }
        }

        public bool IsTransparent {
            get {
                bool transparent = false;
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 && (flags & flags_isTransparent) != 0) transparent = true;
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    if ((flags & 0x4000000) != 0) transparent = true;
                }
                if (transparent) {
                    if (textures.Count > 0 && textures[0] != null && textures[0].texture != null) {
                        return textures[0].texture.IsTransparent;
                    }
                    return false;
                } else return true;
            }
        }

        public bool IsLight {
            get {
                //if (R3Loader.Loader.mode == R3Loader.Mode.Rayman2PC) R3Loader.Loader.print("Flags: " + flags + "Transparent flag: " + flags_isTransparent);
                if ((flags & flags_isTransparent) != 0 || Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    if (textures.Count > 0 && textures[0] != null && textures[0].texture != null) {
                        return textures[0].texture.IsLight;
                    }
                    return false;
                } else return true;
            }
        }

        public bool IsPixelShaded {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return false;
                } else {
                    if (textures.Count > 0) {
                        return textures.Where(t => t.IsPixelShaded).Count() > 0;
                    } else {
                        return false;
                    }
                }
            }
        }

        public bool IsLockedAnimatedTexture {
            get { return (properties & 1) == 1; }
        }

        public VisualMaterial(Pointer offset) {
            this.offset = offset;
            textures = new List<VisualMaterialTexture>();
            animTextures = new List<AnimatedTexture>();
        }

        public static VisualMaterial Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            VisualMaterial m = new VisualMaterial(offset);
            // Material struct = 0x188
            m.flags = reader.ReadUInt32(); // After this: 0x4
            m.ambientCoef  = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            m.diffuseCoef  = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            m.specularCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            m.color        = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()); // 0x44
            
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                m.num_textures = 1;
                reader.ReadUInt32(); // 0x48
                VisualMaterialTexture t = new VisualMaterialTexture();
                t.offset = Pointer.Current(reader);
                t.off_texture = Pointer.Read(reader); // 0x4c
                t.texture = TextureInfo.FromOffset(t.off_texture);
                if (Settings.s.game == Settings.Game.TT) {
                    /*m.off_animTextures_first = Pointer.Read(reader); // 0x68
                    m.off_animTextures_current = Pointer.Read(reader); // 0x6c
                    m.num_animTextures = reader.ReadUInt16();*/
                    Pointer.Read(reader); // detail texture
                    t.currentScrollX = reader.ReadSingle();
                    t.currentScrollY = reader.ReadSingle();
                    t.scrollX = reader.ReadSingle(); // 0x58
                    t.scrollY = reader.ReadSingle(); // 0x5c
                    t.scrollMode = reader.ReadUInt32(); //0x60
                    m.textures.Add(t);

                    reader.ReadInt32(); // current refresh number for scrolling/animated textures, 0x64
                } else {
                    t.currentScrollX = reader.ReadSingle();
                    t.currentScrollY = reader.ReadSingle();
                    t.scrollX = reader.ReadSingle(); // 0x58
                    t.scrollY = reader.ReadSingle(); // 0x5c
                    t.scrollMode = reader.ReadUInt32(); //0x60
                    m.textures.Add(t);

                    reader.ReadInt32(); // current refresh number for scrolling/animated textures, 0x64
                    m.off_animTextures_first = Pointer.Read(reader); // 0x68
                    m.off_animTextures_current = Pointer.Read(reader); // 0x6c
                    m.num_animTextures = reader.ReadUInt16();
                    reader.ReadUInt16(); // 0x70
                }
                reader.ReadUInt32(); // 0x74
                m.properties = reader.ReadByte(); // whole byte for texture scroll lock in R2, no bitmasks
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            } else {
                reader.ReadUInt32(); // current refresh number for scrolling/animated textures, 0x48
                m.off_animTextures_first = Pointer.Read(reader);
                m.off_animTextures_current = Pointer.Read(reader);
                m.num_animTextures = reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadByte();
                reader.ReadByte();
                m.properties = reader.ReadByte();
                reader.ReadByte();
                reader.ReadUInt32();
                reader.ReadUInt32();
               /* m.num_textures = */reader.ReadUInt32();
                for (int i = 0; i < 4; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    if (off_texture == null) break;

                    VisualMaterialTexture t = new VisualMaterialTexture();
                    t.offset = Pointer.Current(reader);
                    t.off_texture = off_texture;
                    t.texture = TextureInfo.FromOffset(t.off_texture);

                    t.type = reader.ReadByte();
                    t.shadingMode = reader.ReadByte();
                    t.uvFunction = reader.ReadByte();
                    t.scrollByte = reader.ReadByte();

                    t.properties = reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    t.scrollX = reader.ReadSingle();
                    t.scrollY = reader.ReadSingle();
                    t.rotateSpeed = reader.ReadSingle();
                    t.rotateDirection = reader.ReadSingle();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    t.currentScrollX = reader.ReadSingle();
                    t.currentScrollY = reader.ReadSingle();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    t.blendIndex = reader.ReadUInt32();
                    
                    m.textures.Add(t);
                }
                m.num_textures = (uint)m.textures.Count;
            }
            if (m.num_animTextures > 0 && m.off_animTextures_first != null) {
                Pointer off_currentAnimTexture = m.off_animTextures_first;
                Pointer.Goto(ref reader, m.off_animTextures_first);
                for (int i = 0; i < m.num_animTextures; i++) {
                    if (off_currentAnimTexture == m.off_animTextures_current) m.currentAnimTexture = i;
                    Pointer off_animTexture = Pointer.Read(reader);
                    float time = reader.ReadSingle();
                    m.animTextures.Add(new AnimatedTexture(off_animTexture, time));
                    Pointer off_nextAnimTexture = Pointer.Read(reader);
                    if (off_nextAnimTexture != null) {
                        off_currentAnimTexture = off_nextAnimTexture;
                        Pointer.Goto(ref reader, off_nextAnimTexture);
                    }
                }
            }

            return m;
        }

        public static VisualMaterial FromOffsetOrRead(Pointer offset, Reader reader) {
            VisualMaterial vm = FromOffset(offset);
            if (vm == null) {
                Pointer off_current = Pointer.Goto(ref reader, offset);
                vm = VisualMaterial.Read(reader, offset);
                Pointer.Goto(ref reader, off_current);
                MapLoader.Loader.visualMaterials.Add(vm);
            }
            return vm;
        }

        public static VisualMaterial FromOffset(Pointer offset) {
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.visualMaterials.Count; i++) {
                if (offset == l.visualMaterials[i].offset) return l.visualMaterials[i];
            }
            return null;
        }
    }
}
