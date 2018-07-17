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

        public Pointer off_texture;

        public TextureInfo texture;
        public int properties;

        // UV scrolling
        public int currentAnimTexture = 0;
        public float scrollX, scrollY;
        public float currentScrollX, currentScrollY;
        public uint scrollMode; // R2

        public byte type;
        public byte shadingMode;
        public byte uvFunction;
        public byte scrollByte; // R3

        public bool ScrollingEnabled {
            get {
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
                    return scrollMode != 0;
                } else {
                    return ((scrollByte & 6) != 0);
                }
            }
        }

        public bool IsScrollX {
            get {
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
                    return scrollMode != 0 && scrollX != 0;
                } else {
                    return ((scrollByte & 2) != 0) && scrollX != 0;
                }
            }
        }
        public bool IsScrollY {
            get {
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
                    return scrollMode != 0 && scrollY != 0;
                } else {
                    return ((scrollByte & 4) != 0) && scrollY != 0;
                }
            }
        }
    }
}
