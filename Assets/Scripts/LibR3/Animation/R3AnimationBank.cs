using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LibR3 {
    public class R3AnimationBank : IEquatable<R3AnimationBank> {
        public R3Pointer off_header;
        public R3Pointer off_data;
        public R3AnimationStack a3d_general;
        public R3AnimationStack vectors;
        public R3AnimationStack quaternions;
        public R3AnimationStack hierarchies;
        public R3AnimationStack NTTO;
        public R3AnimationStack onlyFrames;
        public R3AnimationStack channels;
        public R3AnimationStack framesNumOfNTTO;
        public R3AnimationStack framesKFIndex;
        public R3AnimationStack keyframes;
        public R3AnimationStack events;
        public R3AnimationStack morphData;
        public R3AnimationStack deformations;

        public R3AnimationBank(R3Pointer off_header) {
            this.off_header = off_header;
        }
        public override bool Equals(System.Object obj) {
            return obj is R3AnimationBank && this == (R3AnimationBank)obj;
        }
        public override int GetHashCode() {
            return off_header.GetHashCode();
        }

        public bool Equals(R3AnimationBank other) {
            return this == (R3AnimationBank)other;
        }

        public static bool operator ==(R3AnimationBank x, R3AnimationBank y) {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            return x.off_header == y.off_header;
        }
        public static bool operator !=(R3AnimationBank x, R3AnimationBank y) {
            return !(x == y);
        }

        public static R3AnimationBank[] Read(EndianBinaryReader reader, R3Pointer offset, uint index, uint num_banks) {
            R3Loader l = R3Loader.Loader;
            R3AnimationBank[] banks = new R3AnimationBank[num_banks];

            for (int i = 0; i < num_banks; i++) {
                // Each animation bank is of size 0x104 = 13 times a stack description of 5 uint32s.
                banks[i] = new R3AnimationBank(R3Pointer.Current(reader));
                banks[i].a3d_general = R3AnimationStack.Read(reader);
                banks[i].vectors = R3AnimationStack.Read(reader);
                banks[i].quaternions = R3AnimationStack.Read(reader);
                banks[i].hierarchies = R3AnimationStack.Read(reader);
                banks[i].NTTO = R3AnimationStack.Read(reader);
                banks[i].onlyFrames = R3AnimationStack.Read(reader);
                banks[i].channels = R3AnimationStack.Read(reader);
                banks[i].framesNumOfNTTO = R3AnimationStack.Read(reader);
                banks[i].framesKFIndex = R3AnimationStack.Read(reader);
                banks[i].keyframes = R3AnimationStack.Read(reader);
                banks[i].events = R3AnimationStack.Read(reader);
                banks[i].morphData = R3AnimationStack.Read(reader);
                banks[i].deformations = R3AnimationStack.Read(reader);
            }
            if (l.mode == R3Loader.Mode.Rayman3PC) {
                for (int i = 0; i < num_banks; i++) {
                    if (banks[i].a3d_general.count > 0) banks[i].a3d_general.off_data = R3Pointer.Read(reader);
                    if (banks[i].vectors.count > 0) banks[i].vectors.off_data = R3Pointer.Read(reader);
                    if (banks[i].quaternions.count > 0) banks[i].quaternions.off_data = R3Pointer.Read(reader);
                    if (banks[i].hierarchies.count > 0) banks[i].hierarchies.off_data = R3Pointer.Read(reader);
                    if (banks[i].NTTO.count > 0) banks[i].NTTO.off_data = R3Pointer.Read(reader);
                    if (banks[i].onlyFrames.count > 0) banks[i].onlyFrames.off_data = R3Pointer.Read(reader);
                    if (banks[i].channels.count > 0) banks[i].channels.off_data = R3Pointer.Read(reader);
                    if (banks[i].framesNumOfNTTO.count > 0) banks[i].framesNumOfNTTO.off_data = R3Pointer.Read(reader);
                    if (banks[i].framesKFIndex.count > 0) banks[i].framesKFIndex.off_data = R3Pointer.Read(reader);
                    if (banks[i].keyframes.count > 0) banks[i].keyframes.off_data = R3Pointer.Read(reader);
                    if (banks[i].events.count > 0) banks[i].events.off_data = R3Pointer.Read(reader);
                    if (banks[i].morphData.count > 0) banks[i].morphData.off_data = R3Pointer.Read(reader);
                    if (banks[i].deformations.count > 0) banks[i].deformations.off_data = R3Pointer.Read(reader);
                }
            }
            return banks;
        }
    }
}
