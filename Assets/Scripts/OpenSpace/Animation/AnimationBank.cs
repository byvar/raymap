using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationBank : IEquatable<AnimationBank> {
        public Pointer off_header;
        public Pointer off_data;
        public AnimationStack a3d_general;
        public AnimationStack vectors;
        public AnimationStack quaternions;
        public AnimationStack hierarchies;
        public AnimationStack NTTO;
        public AnimationStack onlyFrames;
        public AnimationStack channels;
        public AnimationStack framesNumOfNTTO;
        public AnimationStack framesKFIndex;
        public AnimationStack keyframes;
        public AnimationStack events;
        public AnimationStack morphData;
        public AnimationStack deformations;

        public AnimationBank(Pointer off_header) {
            this.off_header = off_header;
        }
        public override bool Equals(System.Object obj) {
            return obj is AnimationBank && this == (AnimationBank)obj;
        }
        public override int GetHashCode() {
            return off_header.GetHashCode();
        }

        public bool Equals(AnimationBank other) {
            return this == (AnimationBank)other;
        }

        public static bool operator ==(AnimationBank x, AnimationBank y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.off_header == y.off_header;
        }
        public static bool operator !=(AnimationBank x, AnimationBank y) {
            return !(x == y);
        }

        public static AnimationBank[] Read(EndianBinaryReader reader, Pointer offset, uint index, uint num_banks) {
            MapLoader l = MapLoader.Loader;
            AnimationBank[] banks = new AnimationBank[num_banks];

            for (int i = 0; i < num_banks; i++) {
                // Each animation bank is of size 0x104 = 13 times a stack description of 5 uint32s.
                banks[i] = new AnimationBank(Pointer.Current(reader));
                banks[i].a3d_general = AnimationStack.Read(reader);
                banks[i].vectors = AnimationStack.Read(reader);
                banks[i].quaternions = AnimationStack.Read(reader);
                banks[i].hierarchies = AnimationStack.Read(reader);
                banks[i].NTTO = AnimationStack.Read(reader);
                banks[i].onlyFrames = AnimationStack.Read(reader);
                banks[i].channels = AnimationStack.Read(reader);
                banks[i].framesNumOfNTTO = AnimationStack.Read(reader);
                banks[i].framesKFIndex = AnimationStack.Read(reader);
                banks[i].keyframes = AnimationStack.Read(reader);
                banks[i].events = AnimationStack.Read(reader);
                banks[i].morphData = AnimationStack.Read(reader);
                if (l.mode != MapLoader.Mode.Rayman2PC) {
                    banks[i].deformations = AnimationStack.Read(reader);
                } else {
                    banks[i].deformations = null;
                }
            }
            if (l.mode == MapLoader.Mode.Rayman3PC || l.mode == MapLoader.Mode.Rayman2PC) {
                for (int i = 0; i < num_banks; i++) {
                    if (banks[i].a3d_general.count > 0) banks[i].a3d_general.off_data = Pointer.Read(reader);
                    if (banks[i].vectors.count > 0) banks[i].vectors.off_data = Pointer.Read(reader);
                    if (banks[i].quaternions.count > 0) banks[i].quaternions.off_data = Pointer.Read(reader);
                    if (banks[i].hierarchies.count > 0) banks[i].hierarchies.off_data = Pointer.Read(reader);
                    if (banks[i].NTTO.count > 0) banks[i].NTTO.off_data = Pointer.Read(reader);
                    if (banks[i].onlyFrames.count > 0) banks[i].onlyFrames.off_data = Pointer.Read(reader);
                    if (banks[i].channels.count > 0) banks[i].channels.off_data = Pointer.Read(reader);
                    if (banks[i].framesNumOfNTTO.count > 0) banks[i].framesNumOfNTTO.off_data = Pointer.Read(reader);
                    if (banks[i].framesKFIndex.count > 0) banks[i].framesKFIndex.off_data = Pointer.Read(reader);
                    if (banks[i].keyframes.count > 0) banks[i].keyframes.off_data = Pointer.Read(reader);
                    if (banks[i].events.count > 0) banks[i].events.off_data = Pointer.Read(reader);
                    if (banks[i].morphData.count > 0) banks[i].morphData.off_data = Pointer.Read(reader);
                    if (l.mode != MapLoader.Mode.Rayman2PC && banks[i].deformations.count > 0) banks[i].deformations.off_data = Pointer.Read(reader);
                }
            }
            return banks;
        }
    }
}
