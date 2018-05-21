using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3AnimationReference {
        public R3Pointer offset;
        public string name = null;
        public ushort num_onlyFrames;
        public byte field0_of_a3d;
        public byte num_channels;
        public R3Pointer off_events;
        public R3Pointer off_morphData;
        public ushort anim_index; // Index of animation within bank
        public byte num_events;
        public byte transition;

        public R3AnimationReference(R3Pointer offset) {
            this.offset = offset;
        }

        public static R3AnimationReference Read(EndianBinaryReader reader, R3Pointer offset) {
            R3Loader l = R3Loader.Loader;
            R3AnimationReference ar = new R3AnimationReference(offset);
            if (l.mode == R3Loader.Mode.Rayman3GC) ar.name = new string(reader.ReadChars(0x50));
            ar.num_onlyFrames = reader.ReadUInt16();
            ar.field0_of_a3d = reader.ReadByte();
            ar.num_channels = reader.ReadByte();
            ar.off_events = R3Pointer.Read(reader);
            ar.off_morphData = R3Pointer.Read(reader);
            ar.anim_index = reader.ReadUInt16();
            ar.num_events = reader.ReadByte();
            ar.transition = reader.ReadByte();
            return ar;
        }
    }
}
