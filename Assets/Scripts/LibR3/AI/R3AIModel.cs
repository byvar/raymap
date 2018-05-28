using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3AIModel {
        public R3Pointer offset;

        public R3Pointer off_behaviors_normal;
        public R3Pointer off_behaviors_reflex;
        public R3Pointer off_dsgMem;
        public R3Pointer off_macros;
        public uint flags;

        public R3Behavior[] behaviors_normal = null;
        public R3Behavior[] behaviors_reflex = null;
        public R3Macro[] macros = null;

        public R3AIModel(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3AIModel Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3AIModel ai = new R3AIModel(offset);
            ai.off_behaviors_normal = R3Pointer.Read(reader);
            ai.off_behaviors_reflex = R3Pointer.Read(reader);
            ai.off_dsgMem = R3Pointer.Read(reader);
            ai.off_macros = R3Pointer.Read(reader);
            ai.flags = reader.ReadUInt32();
            
            if (ai.off_behaviors_normal != null) {
                R3Pointer.Goto(ref reader, ai.off_behaviors_normal);
                R3Pointer off_entries = R3Pointer.Read(reader);
                uint num_entries = reader.ReadUInt32();
                ai.behaviors_normal = new R3Behavior[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    R3Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.behaviors_normal[i] = R3Behavior.Read(reader, R3Pointer.Current(reader));
                    }
                }
            }
            if (ai.off_behaviors_reflex != null) {
                R3Pointer.Goto(ref reader, ai.off_behaviors_reflex);
                R3Pointer off_entries = R3Pointer.Read(reader);
                uint num_entries = reader.ReadUInt32();
                ai.behaviors_reflex = new R3Behavior[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    R3Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.behaviors_reflex[i] = R3Behavior.Read(reader, R3Pointer.Current(reader));
                    }
                }
            }
            if (ai.off_macros != null) {
                R3Pointer.Goto(ref reader, ai.off_macros);
                R3Pointer off_entries = R3Pointer.Read(reader);
                byte num_entries = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                ai.macros = new R3Macro[num_entries];
                if (num_entries > 0 && off_entries != null) {
                    R3Pointer.Goto(ref reader, off_entries);
                    for (int i = 0; i < num_entries; i++) {
                        ai.macros[i] = R3Macro.Read(reader, R3Pointer.Current(reader));
                    }
                }
            }
            l.aiModels.Add(ai);
            return ai;
        }


        public static R3AIModel FromOffset(R3Pointer offset) {
            if (offset == null) return null;
            R3Loader l = R3Loader.Loader;
            return l.aiModels.FirstOrDefault(f => f.offset == offset);
        }
    }
}
