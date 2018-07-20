using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.AI {
    public class Behavior {
        public Pointer offset;

        public AIModel aiModel;

        public string name = null;
        public Pointer off_scripts;
        public uint unknown;
        public byte num_scripts;
        public Script[] scripts;

        public Behavior(Pointer offset) {
            this.offset = offset;
        }

        public static Behavior Read(EndianBinaryReader reader, Pointer offset, AIModel aiModel) {
            MapLoader l = MapLoader.Loader;
            Behavior entry = new Behavior(offset);
            entry.aiModel = aiModel;

            if (Settings.s.hasNames) entry.name = new string(reader.ReadChars(0x100)).TrimEnd('\0');
            entry.off_scripts = Pointer.Read(reader);
            entry.unknown = reader.ReadUInt32();
            if(Settings.s.isR2Demo) reader.ReadUInt32();
            entry.num_scripts = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            //if (entry.name != null) l.print(entry.name);

            entry.scripts = new Script[entry.num_scripts];
            if (entry.off_scripts != null && entry.num_scripts > 0) {
                Pointer off_current = Pointer.Goto(ref reader, entry.off_scripts);
                for (int i = 0; i < entry.num_scripts; i++) {
                    entry.scripts[i] = Script.Read(reader, Pointer.Current(reader), entry);
                }
                Pointer.Goto(ref reader, off_current);
            }
            return entry;
        }
    }
}
