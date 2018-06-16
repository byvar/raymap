using OpenSpace.Animation.Component;
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

        public AnimA3DGeneral[] animations;

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
                banks[i].animations = new AnimA3DGeneral[banks[i].a3d_general.count];
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
            Pointer off_current = Pointer.Current(reader);
            Pointer off_a3d = null;
            uint num_a3d = (uint)banks.Sum(b => b.a3d_general.count);
            FileFormat.FileWithPointers kfFile = null;
            if (index == 0 && l.files_array[MapLoader.Mem.FixKeyFrames] != null) {
                kfFile = MapLoader.Loader.files_array[MapLoader.Mem.FixKeyFrames];
            }
            if (index > 0 && l.files_array[MapLoader.Mem.LvlKeyFrames] != null) {
                kfFile = MapLoader.Loader.files_array[MapLoader.Mem.LvlKeyFrames];
            }
            if (kfFile != null && l.mode == MapLoader.Mode.Rayman3GC) {
                kfFile.GotoHeader();
                reader = kfFile.reader;
                uint[] a3d_sizes = new uint[num_a3d];
                for (uint i = 0; i < num_a3d; i++) {
                    a3d_sizes[i] = reader.ReadUInt32();
                }
                off_a3d = Pointer.Current(reader);
                uint current_anim = 0;
                for (uint i = 0; i < banks.Length; i++) {
                    uint num_a3d_in_bank = banks[i].a3d_general.count;
                    for (uint j = 0; j < num_a3d_in_bank; j++) {
                        Pointer.Goto(ref reader, off_a3d);
                        // Read animation data here
                        banks[i].animations[j] = AnimA3DGeneral.Read(reader, off_a3d);
                        AnimA3DGeneral a = banks[i].animations[j];
                        a.vectors       = new AnimVector[a.num_vectors];
                        a.quaternions   = new AnimQuaternion[a.num_quaternions];
                        a.hierarchies   = new AnimHierarchy[a.num_hierarchies];
                        a.ntto          = new AnimNTTO[a.num_NTTO];
                        a.onlyFrames    = new AnimOnlyFrame[a.num_onlyFrames];
                        a.channels      = new AnimChannel[a.num_channels];
                        a.numOfNTTO     = new AnimNumOfNTTO[a.num_numNTTO * a.num_channels];
                        a.framesKFIndex = new AnimFramesKFIndex[a.num_onlyFrames * a.num_channels];
                        a.keyframes     = new AnimKeyframe[a.num_keyframes];
                        a.events        = new AnimEvent[a.num_events];
                        a.morphData     = new AnimMorphData[a.num_morphData];
                        a.deformations  = new AnimDeformation[a.num_deformations];
                        for (uint k = 0; k < a.vectors.Length; k++) a.vectors[k] = AnimVector.Read(reader);
                        for (uint k = 0; k < a.quaternions.Length; k++) a.quaternions[k] = AnimQuaternion.Read(reader);
                        for (uint k = 0; k < a.hierarchies.Length; k++) a.hierarchies[k] = AnimHierarchy.Read(reader);
                        for (uint k = 0; k < a.ntto.Length; k++) a.ntto[k] = AnimNTTO.Read(reader);
                        for (uint k = 0; k < a.onlyFrames.Length; k++) a.onlyFrames[k] = AnimOnlyFrame.Read(reader);
                        reader.Align(4);
                        for (uint k = 0; k < a.channels.Length; k++) a.channels[k] = AnimChannel.Read(reader);
                        for (uint k = 0; k < a.numOfNTTO.Length; k++) a.numOfNTTO[k] = AnimNumOfNTTO.Read(reader);
                        reader.Align(4);
                        for (uint k = 0; k < a.framesKFIndex.Length; k++) a.framesKFIndex[k] = AnimFramesKFIndex.Read(reader);
                        for (uint k = 0; k < a.keyframes.Length; k++) a.keyframes[k] = AnimKeyframe.Read(reader);
                        reader.Align(4);
                        for (uint k = 0; k < a.events.Length; k++) a.events[k] = AnimEvent.Read(reader);
                        for (uint k = 0; k < a.morphData.Length; k++) a.morphData[k] = AnimMorphData.Read(reader);
                        reader.Align(4);
                        for (uint k = 0; k < a.deformations.Length; k++) a.deformations[k] = AnimDeformation.Read(reader);
                        off_a3d += a3d_sizes[current_anim];

                        // Check if read correctly
                        Pointer off_postAnim = Pointer.Current(reader);
                        if (off_postAnim != off_a3d) l.print("Animation block size does not match data size: " +
                            "Current offset: " + off_postAnim + " - Expected offset: " + off_a3d +
                            " - Block start: " + (off_a3d + -(int)(a3d_sizes[current_anim])));

                        current_anim++;
                    }
                }
            }
            Pointer.Goto(ref reader, off_current);
            return banks;
        }
    }
}
