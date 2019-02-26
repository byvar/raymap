using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Visual {
    /// <summary>
    /// Visual Material definition
    /// </summary>
    public class VisualMaterialTexture {
        [JsonIgnore]
        public Pointer offset;
        public uint scrollMode; // R2

        [JsonIgnore]
        public Pointer off_texture;
        public byte textureOp;
        /*
        3: Trans
        4: Mul
        5: Add
        6: LM
        7: Overlight

		Custom for Renderware support:
		50 = lightmap r
		51 = lightmap g
		52 = lightmap b
        */


        public byte shadingMode;
        public byte uvFunction;
        public byte scrollByte;
        public int properties;
        // ...
        public float scrollX;
        public float scrollY;
        public float rotateSpeed;
        public float rotateDirection;
        // ...
        public float currentScrollX;
        public float currentScrollY;
        // ...
        public uint blendIndex;

        // Derived
        public TextureInfo texture;
        private Texture2D texture2D;

        public Texture2D Texture {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    if (texture == null) return null;
                    return texture.Texture;
                }
                if (texture2D == null && texture != null && texture.Texture != null) {
                    texture2D = new Texture2D(texture.Texture.width, texture.Texture.height);
                    texture2D.SetPixels(texture.Texture.GetPixels());
                    texture2D.Apply();
                    if (!IsRepeatU) {
                        texture2D.wrapModeU = TextureWrapMode.Clamp;
                    }
                    if (!IsRepeatV) {
                        texture2D.wrapModeV = TextureWrapMode.Clamp;
                    }
                    if (IsMirrorX) {
                        texture2D.wrapModeU = TextureWrapMode.Mirror;
                    }
                    if (IsMirrorY) {
                        texture2D.wrapModeV = TextureWrapMode.Mirror;
                    }
                }
                return texture2D;
            }
        }

        public bool ScrollingEnabled {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return scrollMode != 0;
                } else {
                    return ((scrollByte & 6) != 0);
                }
            }
        }

        public bool IsScrollX {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return scrollMode != 0 && scrollX != 0;
                } else {
                    return ((scrollByte & 2) != 0) && scrollX != 0;
                }
            }
        }
        public bool IsScrollY {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return scrollMode != 0 && scrollY != 0;
                } else {
                    return ((scrollByte & 4) != 0) && scrollY != 0;
                }
            }
        }
        public bool IsRotate {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return false;
                } else {
                    return ((scrollByte & 8) != 0) || ((scrollByte & 16) != 0);
                }
            }
        }
        public float RotateSpeed {
            get {
                if (!IsRotate) return 0f;
                float spd = Mathf.Acos(rotateSpeed) * Mathf.Sign(rotateDirection);
                if ((scrollByte & 8) != 0) {
                    // Dependent on animation speed
                    spd *= 60f * 318.30988f; // We force 60fps
                } else if ((scrollByte & 16) != 0) {
                    // Not dependent on animation speed
                    spd *= 0.31830987f;
                }
                return spd;
            }
        }
        public float ScrollX {
            get {
                if (!IsScrollX) return 0f;
                return scrollX * Mathf.Abs(Settings.s.textureAnimationSpeedModifier);
            }
        }
        public float ScrollY {
            get {
                if (!IsScrollY) return 0f;
                return scrollY * Settings.s.textureAnimationSpeedModifier;
            }
        }

        public bool IsPixelShaded {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    return false;
                } else {
                    return shadingMode != 2;
                }
            }
        }

        public bool IsMirrorX {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3 && texture != null) return texture.IsMirrorX;
                return (properties & 4) != 0;
            }
        }

        public bool IsMirrorY {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3 && texture != null) return texture.IsMirrorY;
                return (properties & 8) != 0;
            }
        }
        public bool IsRepeatU {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3 && texture != null) return texture.IsRepeatU;
                return (properties & 1) != 0;
            }
        }
        public bool IsRepeatV {
            get {
                if (Settings.s.engineVersion < Settings.EngineVersion.R3 && texture != null) return texture.IsRepeatV;
                return (properties & 2) != 0;
            }
        }
        public uint Format {
            get {
                if (Settings.s.engineVersion == Settings.EngineVersion.R3 && texture != null) {
                    return texture.field0;
                } else return 0;
            }
        }
    }
}
