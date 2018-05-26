using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3Behavior {
        public R3Pointer offset;

        public string name = null;
        public R3Pointer off_scripts;
        public uint unknown;
        public byte num_scripts;
        public R3Script[] scripts;

        public R3Behavior(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3Behavior Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3Behavior entry = new R3Behavior(offset);
            if (l.mode == R3Loader.Mode.Rayman3GC) entry.name = new string(reader.ReadChars(0x100));
            entry.off_scripts = R3Pointer.Read(reader);
            entry.unknown = reader.ReadUInt32();
            entry.num_scripts = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //if (entry.name != null) l.print(entry.name);
            
            if (entry.off_scripts != null && entry.num_scripts > 0) {
                entry.scripts = new R3Script[entry.num_scripts];
                R3Pointer off_current = R3Pointer.Goto(ref reader, entry.off_scripts);
                for (int i = 0; i < entry.num_scripts; i++) {
                    entry.scripts[i] = R3Script.Read(reader, R3Pointer.Current(reader));
                }
                R3Pointer.Goto(ref reader, off_current);
            }
            return entry;
        }
    }
}
