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

        public AnimVector[]        global_vectors;
        public AnimQuaternion[]    global_quaternions;
        public AnimHierarchy[]     global_hierarchies;
        public AnimNTTO[]          global_NTTO;
        public AnimOnlyFrame[]     global_onlyFrames;
        public AnimChannel[]       global_channels;
        public AnimNumOfNTTO[]     global_numOfNTTO;
        public AnimFramesKFIndex[] global_framesKFIndex;
        public AnimKeyframe[]      global_keyframes;
        public AnimEvent[]         global_events;
        public AnimMorphData[]     global_morphData;
        public AnimDeformation[]   global_deformations;

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
                    if (banks[i].a3d_general.countInFile > 0) banks[i].a3d_general.off_data = Pointer.Read(reader);
                    if (banks[i].vectors.countInFile > 0) banks[i].vectors.off_data = Pointer.Read(reader);
                    if (banks[i].quaternions.countInFile > 0) banks[i].quaternions.off_data = Pointer.Read(reader);
                    if (banks[i].hierarchies.countInFile > 0) banks[i].hierarchies.off_data = Pointer.Read(reader);
                    if (banks[i].NTTO.countInFile > 0) banks[i].NTTO.off_data = Pointer.Read(reader);
                    if (banks[i].onlyFrames.countInFile > 0) banks[i].onlyFrames.off_data = Pointer.Read(reader);
                    if (banks[i].channels.countInFile > 0) banks[i].channels.off_data = Pointer.Read(reader);
                    if (banks[i].framesNumOfNTTO.countInFile > 0) banks[i].framesNumOfNTTO.off_data = Pointer.Read(reader);
                    if (banks[i].framesKFIndex.countInFile > 0) banks[i].framesKFIndex.off_data = Pointer.Read(reader);
                    if (banks[i].keyframes.countInFile > 0) banks[i].keyframes.off_data = Pointer.Read(reader);
                    if (banks[i].events.countInFile > 0) banks[i].events.off_data = Pointer.Read(reader);
                    if (banks[i].morphData.countInFile > 0) banks[i].morphData.off_data = Pointer.Read(reader);
                    if (l.mode != MapLoader.Mode.Rayman2PC && banks[i].deformations.countInFile > 0) banks[i].deformations.off_data = Pointer.Read(reader);
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
                        a.vectors = new AnimVector[a.num_vectors];
                        a.quaternions = new AnimQuaternion[a.num_quaternions];
                        a.hierarchies = new AnimHierarchy[a.num_hierarchies];
                        a.ntto = new AnimNTTO[a.num_NTTO];
                        a.onlyFrames = new AnimOnlyFrame[a.num_onlyFrames];
                        a.channels = new AnimChannel[a.num_channels];
                        a.numOfNTTO = new AnimNumOfNTTO[a.num_numNTTO * a.num_channels];
                        a.framesKFIndex = new AnimFramesKFIndex[a.num_onlyFrames * a.num_channels];
                        a.keyframes = new AnimKeyframe[a.num_keyframes];
                        a.events = new AnimEvent[a.num_events];
                        a.morphData = new AnimMorphData[a.num_morphData];
                        a.deformations = new AnimDeformation[a.num_deformations];
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
            } else if (kfFile == null && l.mode == MapLoader.Mode.Rayman3PC) {
                for (uint i = 0; i < banks.Length; i++) {
                    banks[i].animations = new AnimA3DGeneral[banks[i].a3d_general.count];
                    banks[i].global_vectors = new AnimVector[banks[i].vectors.count];
                    banks[i].global_quaternions = new AnimQuaternion[banks[i].quaternions.count];
                    banks[i].global_hierarchies = new AnimHierarchy[banks[i].hierarchies.count];
                    banks[i].global_NTTO = new AnimNTTO[banks[i].NTTO.count];
                    banks[i].global_onlyFrames = new AnimOnlyFrame[banks[i].onlyFrames.count];
                    banks[i].global_channels = new AnimChannel[banks[i].channels.count];
                    banks[i].global_numOfNTTO = new AnimNumOfNTTO[banks[i].framesNumOfNTTO.count];
                    banks[i].global_framesKFIndex = new AnimFramesKFIndex[banks[i].framesKFIndex.count];
                    banks[i].global_keyframes = new AnimKeyframe[banks[i].keyframes.count];
                    banks[i].global_events = new AnimEvent[banks[i].events.count];
                    banks[i].global_morphData = new AnimMorphData[banks[i].morphData.count];
                    banks[i].global_deformations = new AnimDeformation[banks[i].deformations.count];

                    if (banks[i].vectors.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].vectors.off_data);
                        for (uint j = 0; j < banks[i].global_vectors.Length; j++) banks[i].global_vectors[j] = AnimVector.Read(reader);
                    }
                    if (banks[i].quaternions.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].quaternions.off_data);
                        for (uint j = 0; j < banks[i].global_quaternions.Length; j++) banks[i].global_quaternions[j] = AnimQuaternion.Read(reader);
                    }
                    if (banks[i].hierarchies.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].hierarchies.off_data);
                        for (uint j = 0; j < banks[i].global_hierarchies.Length; j++) banks[i].global_hierarchies[j] = AnimHierarchy.Read(reader);
                    }
                    if (banks[i].NTTO.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].NTTO.off_data);
                        for (uint j = 0; j < banks[i].global_NTTO.Length; j++) banks[i].global_NTTO[j] = AnimNTTO.Read(reader);
                    }
                    if (banks[i].onlyFrames.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].onlyFrames.off_data);
                        for (uint j = 0; j < banks[i].global_onlyFrames.Length; j++) banks[i].global_onlyFrames[j] = AnimOnlyFrame.Read(reader);
                    }
                    if (banks[i].channels.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].channels.off_data);
                        for (uint j = 0; j < banks[i].global_channels.Length; j++) banks[i].global_channels[j] = AnimChannel.Read(reader);
                    }
                    if (banks[i].framesNumOfNTTO.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].framesNumOfNTTO.off_data);
                        for (uint j = 0; j < banks[i].global_numOfNTTO.Length; j++) banks[i].global_numOfNTTO[j] = AnimNumOfNTTO.Read(reader);
                    }
                    if (banks[i].framesKFIndex.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].framesKFIndex.off_data);
                        for (uint j = 0; j < banks[i].global_framesKFIndex.Length; j++) banks[i].global_framesKFIndex[j] = AnimFramesKFIndex.Read(reader);
                    }
                    if (banks[i].keyframes.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].keyframes.off_data);
                        for (uint j = 0; j < banks[i].global_keyframes.Length; j++) banks[i].global_keyframes[j] = AnimKeyframe.Read(reader);
                    }
                    if (banks[i].events.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].events.off_data);
                        for (uint j = 0; j < banks[i].global_events.Length; j++) banks[i].global_events[j] = AnimEvent.Read(reader);
                    }
                    if (banks[i].morphData.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].morphData.off_data);
                        for (uint j = 0; j < banks[i].global_morphData.Length; j++) banks[i].global_morphData[j] = AnimMorphData.Read(reader);
                    }
                    if (banks[i].deformations.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].deformations.off_data);
                        for (uint j = 0; j < banks[i].global_deformations.Length; j++) banks[i].global_deformations[j] = AnimDeformation.Read(reader);
                    }

                    if (banks[i].a3d_general.off_data != null) {
                        Pointer.Goto(ref reader, banks[i].a3d_general.off_data);
                        for (uint j = 0; j < banks[i].animations.Length; j++) {
                            banks[i].animations[j] = AnimA3DGeneral.Read(reader, Pointer.Current(reader));
                            banks[i].animations[j].vectors = banks[i].global_vectors;
                            banks[i].animations[j].quaternions = banks[i].global_quaternions;
                            banks[i].animations[j].hierarchies = banks[i].global_hierarchies;
                            banks[i].animations[j].ntto = banks[i].global_NTTO;
                            banks[i].animations[j].onlyFrames = banks[i].global_onlyFrames;
                            banks[i].animations[j].channels = banks[i].global_channels;
                            banks[i].animations[j].numOfNTTO = banks[i].global_numOfNTTO;
                            banks[i].animations[j].framesKFIndex = banks[i].global_framesKFIndex;
                            banks[i].animations[j].keyframes = banks[i].global_keyframes;
                            banks[i].animations[j].events = banks[i].global_events;
                            banks[i].animations[j].morphData = banks[i].global_morphData;
                            banks[i].animations[j].deformations = banks[i].global_deformations;
                        }
                    }
                }
            }
            Pointer.Goto(ref reader, off_current);
            return banks;
        }
    }
}
