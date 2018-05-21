using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Visual Material definition
    /// </summary>
    public class R3Material {
        public List<R3Pointer> off_textures;
        public List<R3Pointer> off_animTextures;
        public uint flags;
        public List<int> textureTypes;
        public R3Pointer offset;
        public Vector4 ambientCoef;
        public Vector4 diffuseCoef;
        public Vector4 color;
        public byte properties;
        private Material material;
        private Material materialCombined;

        // flags
        public static uint flags_isTransparent = (1 << 3);
        public static uint flags_backfaceCulling = (1 << 10);
        public static uint flags_isMaterialChromed = (1 << 22);

        //properties
        public static uint property_receiveShadows = 2;
        public static uint property_isSpriteGenerator = 4;
        public static uint property_isAnimatedSpriteGenerator = 12;

        // TODO: Split material into material_main and material_light, find how these are stored differently.
        public Material Material {
            get {
                if (material == null) {
                    R3Loader l = R3Loader.Loader;
                    //bool backfaceCulling = ((flags & flags_backfaceCulling) == flags_backfaceCulling); // example: 4DDC43FF
                    bool useAlphaMask = false;
                    R3Texture texMain = null, texSecondary = null;
                    if (off_textures != null && off_textures.Count > 0) {
                        texMain = R3Texture.FromOffset(off_textures[0]);
                        if (off_textures.Count > 1) {
                            texSecondary = R3Texture.FromOffset(off_textures[1]);
                        }
                    }
                    Material baseMaterial = l.baseMaterial;
                    bool transparent = IsTransparent;
                    if (textureTypes.Where(i => (i & 0x20) != 0).Count() > 0) {
                        baseMaterial = l.baseLightMaterial;
                    } else if (texMain != null && texSecondary != null) {
                        if (transparent) {
                            baseMaterial = l.baseBlendTransparentMaterial;
                            if (!texSecondary.texture.HasColor()) useAlphaMask = true;
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
                    material.SetColor("_EmissionColor", new Color(ambientCoef.x / 2f, ambientCoef.y / 2f, ambientCoef.z / 2f, ambientCoef.w));
                    if (color.w > 0) {
                        material.SetColor("_Color", new Color(color.x, color.y, color.z, color.w));
                    } else {
                        material.SetColor("_Color", new Color(diffuseCoef.x, diffuseCoef.y, diffuseCoef.z, diffuseCoef.w));
                    }
                    if (texMain != null) material.SetTexture("_MainTex", texMain.texture);
                    if (texSecondary != null) {
                        if (baseMaterial == l.baseBlendMaterial || baseMaterial == l.baseBlendTransparentMaterial) {
                            material.SetTexture("_MainTex2", texSecondary.texture);
                            if (useAlphaMask) material.SetFloat("_UseAlpha", 1f);
                            //material.SetFloat("_Blend", 1f);
                        } else {
                            material.SetTexture("_DetailAlbedoMap", texSecondary.texture);
                        }
                    }
                    if (texMain == null || texMain.texture == null) {
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

        public bool IsTransparent {
            get {
                if ((flags & flags_isTransparent) != 0) {
                    if (off_textures.Count > 0 && off_textures[0] != null) {
                        R3Texture tex = R3Texture.FromOffset(off_textures[0]);
                        if (tex != null) {
                            return tex.IsTransparent;
                        }
                    }
                    return false;
                } else return true;
            }
        }

        public R3Material(R3Pointer offset) {
            this.offset = offset;
            off_textures = new List<R3Pointer>();
            textureTypes = new List<int>();
            off_animTextures = new List<R3Pointer>();
        }

        public static R3Material Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Material m = new R3Material(offset);
            // Material struct = 0x188
            m.flags = reader.ReadUInt32();
            m.ambientCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            m.diffuseCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            reader.ReadSingle();
            m.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadUInt32();
            R3Pointer off_animTextures = R3Pointer.Read(reader);
            reader.ReadUInt32(); // a repeat of last offset?
            ushort num_animTextures = reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadByte();
            reader.ReadByte();
            m.properties = reader.ReadByte();
            reader.ReadByte();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            R3Pointer off_texture1 = R3Pointer.Read(reader);
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            int type_texture1 = reader.ReadInt32();
            reader.ReadBytes(0x3C);
            R3Pointer off_texture2 = R3Pointer.Read(reader);
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            int type_texture2 = reader.ReadInt32();
            uint num_textures = 0;
            if (off_texture1 != null) {
                m.off_textures.Add(off_texture1);
                m.textureTypes.Add(type_texture1);
            }
            if (off_texture2 != null) {
                m.off_textures.Add(off_texture2);
                m.textureTypes.Add(type_texture2);
            }
            /*if (off_texture2 != null) num_textures++;
            R3Pointer[] off_textures = new R3Pointer[num_textures];
            int[] textureTypes = new int[num_textures];
            if (off_texture1 != null) {
                off_textures[0] = off_texture1;
                textureTypes[0] = type_texture1;
            }
            if (off_texture2 != null) {
                off_textures[num_textures - 1] = off_texture2;
                textureTypes[num_textures - 1] = type_texture2;
            }*/

            /*uint num_textures = Math.Min(reader.ReadUInt32(), 2);
            R3Pointer[] off_textures = new R3Pointer[num_textures];
            int[] textureTypes = new int[num_textures];
            for (uint i = 0; i < num_textures; i++) {
                off_textures[i] = R3Pointer.Read(reader);
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                textureTypes[i] = reader.ReadInt32();
                if (num_textures > i + 1) reader.ReadBytes(0x3C);
            }*/
            if (num_animTextures > 0 && off_animTextures != null) {
                R3Pointer.Goto(ref reader, off_animTextures);
                for (int i = 0; i < num_animTextures; i++) {
                    R3Pointer off_animTexture = R3Pointer.Read(reader);
                    m.off_animTextures.Add(off_animTexture);
                    reader.ReadUInt32();
                    R3Pointer off_nextAnimTexture = R3Pointer.Read(reader);
                    if (off_nextAnimTexture != null) R3Pointer.Goto(ref reader, off_nextAnimTexture);
                }
            }

            return m;
        }

        public static R3Material FromOffset(R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            for (int i = 0; i < l.materials.Length; i++) {
                if (offset == l.materials[i].offset) return l.materials[i];
            }
            l.print("null!");
            return null;
        }
    }
}
