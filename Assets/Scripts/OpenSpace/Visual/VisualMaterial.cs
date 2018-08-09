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
                return textures.Where(t => t.ScrollingEnabled).Count() > 0;
            }
        }

        // TODO: Split material into material_main and material_light, find how these are stored differently.
        public Material Material {
            get {
                if (material == null) {
                    MapLoader l = MapLoader.Loader;
                    //bool backfaceCulling = ((flags & flags_backfaceCulling) == flags_backfaceCulling); // example: 4DDC43FF
                    bool useAlphaMask = false;
                    TextureInfo texMain = null, texSecondary = null;
                    if (textures != null && textures.Count > 0) {
                        texMain = textures[0].texture;
                        if (textures.Count > 1) {
                            texSecondary = textures[1].texture;
                        }
                    }
                    Material baseMaterial = l.baseMaterial;
                    bool transparent = IsTransparent;
                    if (textures.Where(t => (t.properties & 0x20) != 0).Count() > 0 || IsLight) {
                        baseMaterial = l.baseLightMaterial;
                    } else if (texMain != null && texSecondary != null) {
                        if (transparent) {
                            baseMaterial = l.baseBlendTransparentMaterial;
                            if (!texSecondary.Texture.HasColor()) useAlphaMask = true;
                        } /*else if (!texSecondary.texture.HasColor()) {
                            useAlphaMask = true;
                            baseMaterial = l.baseBlendTransparentMaterial;
                        }*/ else {
                            baseMaterial = l.baseBlendMaterial;
                        }
                    } else if (texMain != null && transparent) {
                        baseMaterial = l.baseTransparentMaterial;
                    }
                    //if (textureTypes.Where(i => ((i & 1) != 0)).Count() > 0) baseMaterial = loader.baseLightMaterial;
                    material = new Material(baseMaterial);
                    //material.SetColor("_EmissionColor", new Color(1f, 1f, 1f, (1f - ambientCoef.w)*5f));
                    /*if (color.w > 0) {
                        material.SetColor("_Color", new Color(color.x, color.y, color.z, color.w));
                    } else {
                        material.SetColor("_Color", new Color(diffuseCoef.x, diffuseCoef.y, diffuseCoef.z, diffuseCoef.w));
                    }*/
                    if (texMain != null) {
                        material.SetTexture("_MainTex", texMain.Texture);
                        material.SetTextureOffset("_MainTex", new Vector2(texMain.currentScrollX, texMain.currentScrollY));
                    }
                    if (texSecondary != null) {
                        if (baseMaterial == l.baseBlendMaterial || baseMaterial == l.baseBlendTransparentMaterial) {
                            material.SetTexture("_MainTex2", texSecondary.Texture);
                            material.SetTextureOffset("_MainTex2", new Vector2(texSecondary.currentScrollX, texSecondary.currentScrollY));
                            if (useAlphaMask) material.SetFloat("_UseAlpha", 1f);
                            material.SetFloat("_Blend", 1f);
                        } else {
                            material.SetTexture("_DetailAlbedoMap", texSecondary.Texture);
                        }
                    }
                    if (baseMaterial == l.baseMaterial || baseMaterial == l.baseTransparentMaterial) {
                        material.SetVector("_AmbientCoef", ambientCoef);
                        material.SetVector("_SpecularCoef", specularCoef);
                        material.SetVector("_DiffuseCoef", diffuseCoef);
                        material.SetVector("_Color", color);
                    }
                    if (texMain == null || texMain.Texture == null) {
                        // Don't want to see all those textureless planes, so create transparent texture and use that
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, new Color(0, 0, 0, 0));
                        tex.Apply();
                        material.SetTexture("_MainTex", tex);
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
                    bool useAlphaMask = false;
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
                    materialBillboard.SetColor("_EmissionColor", new Color(ambientCoef.x / 2f, ambientCoef.y / 2f, ambientCoef.z / 2f, ambientCoef.w));
                    if (color.w > 0) {
                        materialBillboard.SetColor("_Color", new Color(color.x, color.y, color.z, color.w));
                    } else {
                        materialBillboard.SetColor("_Color", new Color(diffuseCoef.x, diffuseCoef.y, diffuseCoef.z, diffuseCoef.w));
                    }
                    if (texMain != null) materialBillboard.SetTexture("_MainTex", texMain.Texture);
                    if (texSecondary != null) {
                        if (baseMaterial == l.baseBlendMaterial || baseMaterial == l.baseBlendTransparentMaterial) {
                            materialBillboard.SetTexture("_MainTex2", texSecondary.Texture);
                            if (useAlphaMask) materialBillboard.SetFloat("_UseAlpha", 1f);
                            //material.SetFloat("_Blend", 1f);
                        } else {
                            materialBillboard.SetTexture("_DetailAlbedoMap", texSecondary.Texture);
                        }
                    }
                    if (texMain == null || texMain.Texture == null) {
                        // Don't want to see all those textureless planes, so create transparent texture and use that
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, new Color(0, 0, 0, 0));
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
                if (Settings.s.engineMode == Settings.EngineMode.R3 && (flags & flags_isTransparent) != 0) transparent = true;
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
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
                if ((flags & flags_isTransparent) != 0 || Settings.s.engineMode == Settings.EngineMode.R2) {
                    if (textures.Count > 0 && textures[0] != null && textures[0].texture != null) {
                        return textures[0].texture.IsLight;
                    }
                    return false;
                } else return true;
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
            
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                reader.ReadUInt32(); // 0x48
                VisualMaterialTexture t = new VisualMaterialTexture();
                t.off_texture = Pointer.Read(reader); // 0x4c
                t.texture = TextureInfo.FromOffset(t.off_texture);
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
                reader.ReadUInt32();
                for (int i = 0; i < 4; i++) {
                    Pointer off_texture = Pointer.Read(reader);
                    if (off_texture == null) break;

                    VisualMaterialTexture t = new VisualMaterialTexture();
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

                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    t.currentScrollX = reader.ReadSingle();
                    t.currentScrollY = reader.ReadSingle();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    
                    m.textures.Add(t);
                }
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
