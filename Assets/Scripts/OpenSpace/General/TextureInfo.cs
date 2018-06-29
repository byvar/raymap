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
        public Texture2D texture;

        public uint flags;
        public uint flags2;
        public string name;
        public static uint flags_isTransparent = (1 << 3);

        public TextureInfo(Pointer offset) {
            this.offset = offset;
        }

        public bool IsTransparent {
            get {
                if (MapLoader.Loader.mode == MapLoader.Mode.Rayman2PC) {
                    return (flags & (1 << 1)) != 0;
                } else {
                    return (flags & flags_isTransparent) != 0;
                }
            }
        }

        public bool IsLight {
            get {
                return (flags & (1 << 5)) != 0;
            }
        }

        public static TextureInfo Read(EndianBinaryReader reader, Pointer offset) {
            TextureInfo tex = new TextureInfo(offset);
            reader.ReadUInt32(); // 888 or 8888
            reader.ReadUInt16(); // 20
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            tex.flags = reader.ReadUInt32();
            reader.ReadBytes(0x2E);
            if (MapLoader.Loader.mode != MapLoader.Mode.Rayman2PC) tex.flags2 = reader.ReadUInt32(); // contains flags such as tiling mode
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
