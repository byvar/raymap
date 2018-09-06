using OpenSpace.Animation.ComponentMontreal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationMontreal {
        public Pointer offset;
        public Pointer off_frames = null;
        public byte num_frames;
        public byte speed;
        public byte num_channels;
        public byte unkbyte;
        public Pointer off_unk;
        public Matrix speedMatrix;
        public AnimFrameMontreal[] frames;


        public AnimationMontreal(Pointer offset) {
            this.offset = offset;
        }

        public static AnimationMontreal Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimationMontreal ar = new AnimationMontreal(offset);
            ar.off_frames = Pointer.Read(reader);
            ar.num_frames = reader.ReadByte();
            ar.speed = reader.ReadByte();
            ar.num_channels = reader.ReadByte();
            ar.unkbyte = reader.ReadByte();
            ar.off_unk = Pointer.Read(reader);
            reader.ReadUInt32();
            reader.ReadUInt32();
            ar.speedMatrix = Matrix.Read(reader, Pointer.Current(reader));
            reader.ReadUInt32();
            reader.ReadUInt32();
            ar.frames = new AnimFrameMontreal[ar.num_frames];


            Pointer.DoAt(ref reader, ar.off_frames, () => {
                for (int i = 0; i < ar.num_frames; i++) {
                    ar.frames[i] = AnimFrameMontreal.Read(reader, Pointer.Current(reader), ar);
                }
            });
            return ar;
        }

        public static AnimationMontreal FromOffsetOrRead(Pointer offset, Reader reader) {
            if (offset == null) return null;
            AnimationMontreal ar = FromOffset(offset);
            if (ar == null) {
                Pointer.DoAt(ref reader, offset, () => {
                    ar = AnimationMontreal.Read(reader, offset);
                    MapLoader.Loader.animationReferencesMontreal.Add(ar);
                });
            }
            return ar;
        }

        public static AnimationMontreal FromOffset(Pointer offset) {
            if (offset == null) return null;
            MapLoader l = MapLoader.Loader;
            return l.animationReferencesMontreal.FirstOrDefault(ar => ar.offset == offset);
        }
    }
}
