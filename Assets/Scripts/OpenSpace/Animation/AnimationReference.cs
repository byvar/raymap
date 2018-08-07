using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationReference {
        public Pointer offset;
        public string name = null;
        public ushort num_onlyFrames;
        public byte speed;
        public byte num_channels;
        public Pointer off_events;
        public float x;
        public float y;
        public float z;
        public Pointer off_morphData;
        public ushort anim_index; // Index of animation within bank
        public byte num_events;
        public byte transition;

        public AnimationReference(Pointer offset) {
            this.offset = offset;
        }

        public static AnimationReference Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimationReference ar = new AnimationReference(offset);
            if (l.mode == MapLoader.Mode.Rayman3GC) ar.name = new string(reader.ReadChars(0x50));
            ar.num_onlyFrames = reader.ReadUInt16();
            ar.speed = reader.ReadByte();
            ar.num_channels = reader.ReadByte();
            ar.off_events = Pointer.Read(reader);
            if (Settings.s.engineMode == Settings.EngineMode.R2) {
                ar.x = reader.ReadSingle();
                ar.y = reader.ReadSingle();
                ar.z = reader.ReadSingle();
            }
            ar.off_morphData = Pointer.Read(reader);
            ar.anim_index = reader.ReadUInt16();
            ar.num_events = reader.ReadByte();
            ar.transition = reader.ReadByte();
            if (Settings.s.engineMode == Settings.EngineMode.R2) reader.ReadUInt32(); // no idea what this is sadly
            return ar;
        }
    }
}
