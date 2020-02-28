using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenSpace.Exporter;
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
        public uint num_textures_in_material;
		
        public Pointer off_animTextures_first;
        public Pointer off_animTextures_current;
        public ushort num_animTextures;

        public byte properties;
        private Material material;
        //private Material materialBillboard;

        // UV scrolling
        public int currentAnimTexture = 0;

        // flags
        public static uint flags_backfaceCulling = (1 << 10);
        public static uint flags_isMaterialChromed = (1 << 22);
        public static uint flags_isBillboard = (1 << 9);
		public static uint Flags_IsTransparent {
			get {
				if (Settings.s.game == Settings.Game.LargoWinch) {
					return (1 << 10);
				} else {
					return (1 << 3);
				}
			}
		}

        //properties
        public static uint property_receiveShadows = 2;
        public static uint property_isSpriteGenerator = 4;
        public static uint property_isAnimatedSpriteGenerator = 12;
        public static uint property_isGrass = 0x2000;
        public static uint property_isWater = 0x1000;

        public enum Hint {
            None = 0,
            Transparent = 1,
            Billboard = 2
        }

        [JsonIgnore]
        public Hint receivedHints = Hint.None;

        public bool ScrollingEnabled {
            get {
                return textures.Where(t => t != null && t.ScrollingEnabled).Count() > 0;
            }
        }

        // TODO: Split material into material_main and material_light, find how these are stored differently.
        public Material GetMaterial(Hint hints = Hint.None) {
            if (material == null || hints != receivedHints) {
                bool billboard = (hints & Hint.Billboard) == Hint.Billboard;// || (flags & flags_isBillboard) == flags_isBillboard;
                MapLoader l = MapLoader.Loader;
                receivedHints = hints;
                //bool backfaceCulling = ((flags & flags_backfaceCulling) == flags_backfaceCulling); // example: 4DDC43FF
                Material baseMaterial = l.baseMaterial;
                bool transparent = IsTransparent || ((hints & Hint.Transparent) == Hint.Transparent) || textures.Count == 0;
                if (textures.Where(t => ((t.properties & 0x20) != 0 && (t.properties & 0x80000000) == 0)).Count() > 0
					|| IsLight//) {
					|| (textures.Count > 0 && textures[0].textureOp == 1)) {
                    baseMaterial = l.baseLightMaterial;
                } else if (transparent) {
                    baseMaterial = l.baseTransparentMaterial;
                }
                //if (textureTypes.Where(i => ((i & 1) != 0)).Count() > 0) baseMaterial = loader.baseLightMaterial;
                material = new Material(baseMaterial);
                if (textures != null) {
                    material.SetFloat("_NumTextures", num_textures);
                    if (num_textures == 0) {
                        // Zero textures? Can only happen in R3 mode. Make it fully transparent.
                        Texture2D tex = new Texture2D(1, 1);
                        tex.SetPixel(0, 0, new Color(0,0,0,0));
                        tex.Apply();
                        material.SetTexture("_Tex0", tex);
                    }
                    for (int i = 0; i < num_textures; i++) {
                        string textureName = "_Tex" + i;
                        if (textures[i].Texture != null) {
                            material.SetTexture(textureName, textures[i].Texture);
                            material.SetVector(textureName + "Params", new Vector4(textures[i].textureOp,
                                textures[i].ScrollingEnabled ? 1f : (textures[i].IsRotate ? 2f : 0f),
                                0f, textures[i].Format));
                            material.SetVector(textureName + "Params2", new Vector4(
                                textures[i].currentScrollX, textures[i].currentScrollY,
                                textures[i].ScrollX, textures[i].ScrollY));
                            //material.SetTextureOffset(textureName, new Vector2(textures[i].texture.currentScrollX, textures[i].texture.currentScrollY));
                        } else {
                            // No texture = just color. So create white texture and let that be colored by other properties.
                            Texture2D tex = new Texture2D(1, 1);
                            tex.SetPixel(0, 0, new Color(1, 1, 1, 1));
                            tex.Apply();
                            material.SetTexture(textureName, tex);
                        }
                    }
                }
                material.SetVector("_AmbientCoef", ambientCoef);
                material.SetVector("_DiffuseCoef", diffuseCoef);
                if (billboard) material.SetFloat("_Billboard", 1f);
                /* if (baseMaterial == l.baseMaterial || baseMaterial == l.baseTransparentMaterial) {
                        material.SetVector("_AmbientCoef", ambientCoef);
                        //material.SetVector("_SpecularCoef", specularCoef);
                        material.SetVector("_DiffuseCoef", diffuseCoef);
                        //material.SetVector("_Color", color);
                        //if (IsPixelShaded) material.SetFloat("_ShadingMode", 1f);
                    }*/
            }
            return material;
        }

        public string ToJSON()
        {
            JsonSerializerSettings settings = MapExporter.JsonExportSettings;
            return JsonConvert.SerializeObject(this, settings);
        }

        [JsonRequired]
        public bool IsTransparent {
            get {
                bool transparent = false;
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 &&
                    ((flags & Flags_IsTransparent) != 0 || (receivedHints & Hint.Transparent) == Hint.Transparent)) transparent = true;
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

        [JsonRequired]
        public bool IsLight {
            get {
				//if (R3Loader.Loader.mode == R3Loader.Mode.Rayman2PC) R3Loader.Loader.print("Flags: " + flags + "Transparent flag: " + flags_isTransparent);
				/*if ((flags & Flags_IsTransparent) != 0 || (receivedHints & Hint.Transparent) == Hint.Transparent
					|| Settings.s.engineVersion < Settings.EngineVersion.R3
					|| Settings.s.game == Settings.Game.LargoWinch) {
					if (textures.Count > 0 && textures[0] != null && textures[0].texture != null) {
						return textures[0].texture.IsLight;
					}
					return false;
				} else {
					if (Settings.s.game == Settings.Game.LargoWinch) return false;
					return true;
				}*/
				if (textures.Count > 0 && textures[0] != null && textures[0].texture != null) {
					return textures[0].texture.IsLight;
				}
				return false;
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
			//l.print("Material @ " + offset);
            m.flags = reader.ReadUInt32(); // After this: 0x4
			if (Settings.s.game != Settings.Game.R2Revolution && Settings.s.game != Settings.Game.LargoWinch) {
				if (Settings.s.platform == Settings.Platform.DC) reader.ReadUInt32();
				m.ambientCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				m.diffuseCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				m.specularCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				m.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()); // 0x44
			} else if (Settings.s.game == Settings.Game.R2Revolution) {
				// Fill in light info for Revolution
				m.ambientCoef = new Vector4(0, 0, 0, 1f);
				m.diffuseCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				//m.diffuseCoef = new Vector4(1, 1, 1, 1);
				reader.ReadInt32(); // current refresh number for scrolling/animated textures
				m.off_animTextures_first = Pointer.Read(reader);
				m.off_animTextures_current = Pointer.Read(reader);
				reader.ReadInt32();
				m.num_animTextures = reader.ReadUInt16();
				reader.ReadUInt16(); // 0x70
			} else if (Settings.s.game == Settings.Game.LargoWinch) {
				m.ambientCoef = new Vector4(0, 0, 0, 1f);
				m.color = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()); // 0x44
				m.diffuseCoef = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				//m.ambientCoef = m.diffuseCoef;
				reader.ReadInt32(); // current refresh number for scrolling/animated textures
				m.off_animTextures_first = Pointer.Read(reader);
				m.off_animTextures_current = Pointer.Read(reader);
				reader.ReadInt32();
				m.num_animTextures = reader.ReadUInt16();
				reader.ReadUInt16();
			}
			if (Settings.s.game == Settings.Game.LargoWinch) {
				m.num_textures = 1;
				VisualMaterialTexture t = new VisualMaterialTexture();
				t.offset = Pointer.Current(reader);
				t.off_texture = Pointer.Read(reader); // 0x4c
				t.texture = TextureInfo.FromOffset(t.off_texture);
				t.textureOp = reader.ReadByte();
				t.shadingMode = reader.ReadByte();
				t.uvFunction = reader.ReadByte();
				t.scrollByte = reader.ReadByte();
				t.scrollX = reader.ReadSingle();
				t.scrollY = reader.ReadSingle();
				reader.ReadSingle();

				new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
				m.textures.Add(t);
			} else if (Settings.s.game == Settings.Game.R2Revolution) {
				m.num_textures = 1;
				VisualMaterialTexture t = new VisualMaterialTexture();
				t.offset = Pointer.Current(reader);
				t.off_texture = Pointer.Read(reader); // 0x4c
				t.texture = TextureInfo.FromOffset(t.off_texture);
				t.scrollMode = reader.ReadUInt32();
				t.scrollX = reader.ReadSingle();
				t.scrollY = reader.ReadSingle();
				t.currentScrollX = reader.ReadSingle();
				t.currentScrollY = reader.ReadSingle();
				m.textures.Add(t);
				/*reader.ReadInt32(); // current refresh number for scrolling/animated textures, 0x64
				m.off_animTextures_first = Pointer.Read(reader); // 0x68
				m.off_animTextures_current = Pointer.Read(reader); // 0x6c
				m.num_animTextures = reader.ReadUInt16();
				reader.ReadUInt16(); // 0x70
				reader.ReadUInt32();
				reader.ReadByte();
				reader.ReadByte();
				m.properties = reader.ReadByte();
				reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadUInt32();*/
			} else if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
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
                    if (Settings.s.platform == Settings.Platform.DC) {
                        // For some reason there's a huge gap here
                        reader.ReadBytes(0xD0);
                    }
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
                reader.ReadByte(); // padding, not in DC
                reader.ReadByte(); // padding, not in DC
            } else { // EngineVersion >= R3
                reader.ReadUInt32(); // current refresh number for scrolling/animated textures, 0x48
				if (Settings.s.game == Settings.Game.Dinosaur) {
					reader.ReadBytes(0x1C);
                }
                if (Settings.s.platform == Settings.Platform.PS2) {
                    reader.ReadBytes(0x20);
                }
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
                m.num_textures_in_material = reader.ReadUInt32();
                for (int i = 0; i < 4; i++) {
                    VisualMaterialTexture t = new VisualMaterialTexture();
                    t.offset = Pointer.Current(reader);
                    //l.print(t.offset);
                    t.off_texture = Pointer.Read(reader);
                    if (t.off_texture == null) break;
					/*if (Settings.s.game == Settings.Game.Dinosaur) {
						Pointer.DoAt(ref reader, t.off_texture, () => {
							Pointer off_tex = Pointer.Read(reader);
							t.texture = TextureInfo.FromOffset(off_tex);
						});
					} else {*/
					t.texture = TextureInfo.FromOffset(t.off_texture);
					//}

                    t.textureOp = reader.ReadByte();
                    t.shadingMode = reader.ReadByte();
                    t.uvFunction = reader.ReadByte();
                    t.scrollByte = reader.ReadByte();

					if (Settings.s.game == Settings.Game.Dinosaur) {
						t.properties = reader.ReadInt32();
						new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
						t.currentScrollX = reader.ReadSingle();
						t.currentScrollY = reader.ReadSingle();
						t.scrollX = reader.ReadSingle();
						t.scrollY = reader.ReadSingle();
						new Vector2(reader.ReadSingle(), reader.ReadSingle());
						new Vector2(reader.ReadSingle(), reader.ReadSingle());
						new Vector2(reader.ReadSingle(), reader.ReadSingle());
						new Vector2(reader.ReadSingle(), reader.ReadSingle());
						new Vector2(reader.ReadSingle(), reader.ReadSingle());
					} else if (Settings.s.platform == Settings.Platform.PS2) {
                        t.properties = reader.ReadInt32();
                        new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        new Vector2(reader.ReadSingle(), reader.ReadSingle());

                        t.currentScrollX = reader.ReadSingle();
                        t.currentScrollY = reader.ReadSingle();
                        t.scrollX = reader.ReadSingle();
                        t.scrollY = reader.ReadSingle();
                        t.rotateSpeed = reader.ReadSingle();
                        t.rotateDirection = reader.ReadSingle();
                        reader.ReadInt32();
                        new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    } else {
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
					}
                    
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
            if (offset == null) return null;
            VisualMaterial vm = FromOffset(offset);
            if (vm == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    vm = VisualMaterial.Read(reader, offset);
                    MapLoader.Loader.visualMaterials.Add(vm);
                });
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


		// Call after clone
		public void Reset() {
			material = null;
			//materialBillboard = null;
		}
		public VisualMaterial Clone() {
			VisualMaterial vm = (VisualMaterial)MemberwiseClone();
			vm.textures = new List<VisualMaterialTexture>(textures);
			vm.animTextures = new List<AnimatedTexture>(animTextures);
			vm.Reset();
			return vm;
		}


        public class VisualMaterialReferenceJsonConverter : JsonConverter {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(VisualMaterial);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                VisualMaterial vmt = (VisualMaterial)value;
                string hash = HashUtils.MD5Hash(vmt.ToJSON());
                VisualMaterialReference reference = new VisualMaterialReference() { Hash = hash };

                var jt = JToken.FromObject(reference);
                jt.WriteTo(writer);
            }
        }
    }
}
