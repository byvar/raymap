using OpenSpace.Animation.Component;
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

        public Pointer off_a3d = null;
        public AnimA3DGeneral a3d = null;

        public AnimationReference(Pointer offset) {
            this.offset = offset;
        }

        public static AnimationReference Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimationReference ar = new AnimationReference(offset);
            if (Settings.s.hasNames) ar.name = new string(reader.ReadChars(0x50));
            if (Settings.s.engineVersion == Settings.EngineVersion.Montreal) {
                l.print(offset);
                //ar.off_a3d = Pointer.Read(reader);
            } else {
                if (Settings.s.engineVersion <= Settings.EngineVersion.TT) reader.ReadUInt32();
                ar.num_onlyFrames = reader.ReadUInt16();
                ar.speed = reader.ReadByte();
                ar.num_channels = reader.ReadByte();
                ar.off_events = Pointer.Read(reader);
                if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                    ar.x = reader.ReadSingle();
                    ar.y = reader.ReadSingle();
                    ar.z = reader.ReadSingle();
                }
                ar.off_morphData = Pointer.Read(reader);
                if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }
                ar.anim_index = reader.ReadUInt16();
                ar.num_events = reader.ReadByte();
                ar.transition = reader.ReadByte();

                if (Settings.s.engineVersion == Settings.EngineVersion.R2) reader.ReadUInt32(); // no idea what this is sadly
                if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
                    ar.off_a3d = Pointer.Read(reader);
                }
            }
            Pointer.DoAt(ref reader, ar.off_a3d, () => {
                ar.a3d = AnimA3DGeneral.ReadFull(reader, ar.off_a3d);
            });
            return ar;
        }

        public static AnimationReference FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            AnimationReference ar = FromOffset(offset);
            if (ar == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    ar = AnimationReference.Read(reader, offset);
                    MapLoader.Loader.animationReferences.Add(ar);
                });
            }
            return ar;
        }

        public static AnimationReference FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.animationReferences.FirstOrDefault(ar => ar.offset == offset);
        }
    }
}
