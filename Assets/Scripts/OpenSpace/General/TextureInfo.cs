using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace {
    /// <summary>
    /// Texture definition
    /// </summary>
    public class TextureInfo {
        public Pointer offset;
        private Texture2D texture;

        public uint field0;
        public ushort field4;
        public ushort field6;
        public Pointer off_buffer;     // field8
        public uint fieldC;           //
        public uint field10;         //
        public uint flags;          // field14
        public ushort height_;     // field18
        public ushort width_;     // field1A
        public ushort height;    // field1C
        public ushort width;    // field1E
        public uint field20;
        public uint field24;
        public uint field28;
        public uint alphaMask; // field2C
        public uint field30;
        public uint numMipmaps = 1;
        public uint field38; // from here on, do -4 for R2. r2 doesn't have mipmaps
        public uint field3C;
        public uint field40;
        public uint field44;
        public byte field48;
        public byte flagsByte;
        public string name;


        public static uint flags_isTransparent = (1 << 3);

        public TextureInfo(Pointer offset) {
            this.offset = offset;
        }

        public bool IsTransparent {
            get {
                if (Settings.s.engineMode == Settings.EngineMode.R2) {
                    return (flags & (1 << 1)) != 0 || (flags & flags_isTransparent) != 0;
                } else {
                    return (flags & flags_isTransparent) != 0;
                }
            }
        }

        public bool IsLight {
            get {
                //if (Settings.s.engineMode == Settings.EngineMode.R2) return false;
                return (flags & (1 << 5)) != 0;
            }
        }

        public Texture2D Texture {
            get { return texture; }
            set {
                texture = value;
                if (IsMirrorX) {
                    texture.wrapModeU = TextureWrapMode.Mirror;
                }
                if (IsMirrorY) {
                    texture.wrapModeV = TextureWrapMode.Mirror;
                }
            }
        }

        public bool IsMirrorX {
            get { return (flagsByte & 4) != 0; }
        }

        public bool IsMirrorY {
            get { return (flagsByte & 8) != 0; }
        }

        public static TextureInfo Read(EndianBinaryReader reader, Pointer offset) {
            TextureInfo tex = new TextureInfo(offset);
            MapLoader.Loader.print("Tex off: " + offset);
            tex.field0 = reader.ReadUInt32(); // 888 or 8888
            tex.field4 = reader.ReadUInt16(); // 20
            tex.field6 = reader.ReadUInt16();
            tex.off_buffer = Pointer.Read(reader); // always null because it's stored here dynamically
            tex.fieldC = reader.ReadUInt32();
            tex.field10 = reader.ReadUInt32();
            tex.flags = reader.ReadUInt32();
            tex.height_ = reader.ReadUInt16();
            tex.width_ = reader.ReadUInt16();
            tex.height = reader.ReadUInt16();
            tex.width = reader.ReadUInt16();
            tex.field20 = reader.ReadUInt32();
            tex.field24 = reader.ReadUInt32();
            tex.field28 = reader.ReadUInt32();
            tex.alphaMask = reader.ReadUInt32();
            tex.field30 = reader.ReadUInt32();
            if (Settings.s.engineMode == Settings.EngineMode.R3) tex.numMipmaps = reader.ReadUInt32();
            tex.field38 = reader.ReadUInt32();
            tex.field3C = reader.ReadUInt32();
            tex.field40 = reader.ReadUInt32();
            tex.field44 = reader.ReadUInt32();
            tex.field48 = reader.ReadByte();
            tex.flagsByte = reader.ReadByte(); // contains stuff like tiling mode
            tex.name = reader.ReadNullDelimitedString();
            return tex;
        }

        public static TextureInfo FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            for (int i = 0; i < l.textures.Length; i++) {
                if (l.textures[i] == null) continue;
                if (offset == l.textures[i].offset) return l.textures[i];
            }
            Debug.LogWarning("Material looked for texture with offset " + String.Format("0x{0:X}", offset.offset) + ", but it wasn't found -- likely it's inline and dummy.");
            return null;
        }
    }
}
