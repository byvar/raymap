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
        public Pointer offset;
        public uint scrollMode; // R2

        public Pointer off_texture;
        public byte type;
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
                return scrollX * Settings.s.textureAnimationSpeedModifier;
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
    }
}
