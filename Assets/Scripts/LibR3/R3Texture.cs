using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    /// <summary>
    /// Texture definition
    /// </summary>
    public class R3Texture {
        public R3Pointer offset;
        public Texture2D texture;

        public uint flags;
        public string name;
        public static uint flags_isTransparent = (1 << 3);

        public R3Texture(R3Pointer offset) {
            this.offset = offset;
        }

        public bool IsTransparent {
            get {
                return (flags & flags_isTransparent) != 0;
            }
        }

        public static R3Texture Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Texture tex = new R3Texture(offset);
            reader.ReadUInt32(); // 888 or 8888
            reader.ReadUInt16(); // 20
            reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            tex.flags = reader.ReadUInt32();
            reader.ReadBytes(0x32); // lol, sorry :(
                                    //string name = new string(reader.ReadChars(0x82)); // buffer is way too long and there's probably other stuff in there, but everything's 0
            tex.name = reader.ReadNullDelimitedString();
            return tex;
        }

        public static R3Texture FromOffset(R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            for (int i = 0; i < l.textures.Length; i++) {
                if (offset == l.textures[i].offset) return l.textures[i];
            }
            Debug.LogWarning("Material looked for texture with offset " + String.Format("0x{0:X}", offset.offset) + ", but it wasn't found -- likely it's inline and dummy.");
            return null;
        }
    }
}
