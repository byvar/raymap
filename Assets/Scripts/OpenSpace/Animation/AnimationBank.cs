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

        public static AnimationBank[] Read(Reader reader, Pointer offset, uint index, uint num_banks, FileFormat.FileWithPointers kfFile, bool append = false) {
            MapLoader l = MapLoader.Loader;
            AnimationBank[] banks = new AnimationBank[num_banks];
            
            for (int i = 0; i < num_banks; i++) {
                // In R3, each animation bank is of size 0x104 = 13 times a stack description of 5 uint32s.
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
                if (Settings.s.hasDeformations) {
                    banks[i].deformations = AnimationStack.Read(reader);
                } else {
                    banks[i].deformations = null;
                }
                banks[i].animations = new AnimA3DGeneral[banks[i].a3d_general.count];
            }
            if (l.mode != MapLoader.Mode.Rayman3GC && !append) {
                if (!Settings.s.loadFromMemory) {
                    for (int i = 0; i < num_banks; i++) {
                        if (banks[i].a3d_general.reservedMemory > 0) banks[i].a3d_general.off_data = Pointer.Read(reader);
                        if (banks[i].vectors.reservedMemory > 0) banks[i].vectors.off_data = Pointer.Read(reader);
                        if (banks[i].quaternions.reservedMemory > 0) banks[i].quaternions.off_data = Pointer.Read(reader);
                        if (banks[i].hierarchies.reservedMemory > 0) banks[i].hierarchies.off_data = Pointer.Read(reader);
                        if (banks[i].NTTO.reservedMemory > 0) banks[i].NTTO.off_data = Pointer.Read(reader);
                        if (banks[i].onlyFrames.reservedMemory > 0) banks[i].onlyFrames.off_data = Pointer.Read(reader);
                        if (banks[i].channels.reservedMemory > 0) banks[i].channels.off_data = Pointer.Read(reader);
                        if (banks[i].framesNumOfNTTO.reservedMemory > 0) banks[i].framesNumOfNTTO.off_data = Pointer.Read(reader);
                        if (banks[i].framesKFIndex.reservedMemory > 0) banks[i].framesKFIndex.off_data = Pointer.Read(reader);
                        if (kfFile == null) {
                            if (banks[i].keyframes.reservedMemory > 0) banks[i].keyframes.off_data = Pointer.Read(reader);
                        } else {
                            banks[i].keyframes.off_data = new Pointer(0, kfFile);
                        }
                        if (banks[i].events.reservedMemory > 0) banks[i].events.off_data = Pointer.Read(reader);
                        if (banks[i].morphData.reservedMemory > 0) banks[i].morphData.off_data = Pointer.Read(reader);
                        if (Settings.s.hasDeformations && banks[i].deformations.reservedMemory > 0) banks[i].deformations.off_data = Pointer.Read(reader);
                    }
                } else {
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_a3d"], offset.file));
                    banks[0].a3d_general.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_vectors"], offset.file));
                    banks[0].vectors.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_quaternions"], offset.file));
                    banks[0].quaternions.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_hierarchies"], offset.file));
                    banks[0].hierarchies.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_NTTO"], offset.file));
                    banks[0].NTTO.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_onlyFrames"], offset.file));
                    banks[0].onlyFrames.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_channels"], offset.file));
                    banks[0].channels.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_framesNumOfNTTO"], offset.file));
                    banks[0].framesNumOfNTTO.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_framesKF"], offset.file));
                    banks[0].framesKFIndex.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_keyframes"], offset.file));
                    banks[0].keyframes.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_events"], offset.file));
                    banks[0].events.off_data = Pointer.Read(reader);
                    Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_morphData"], offset.file));
                    banks[0].morphData.off_data = Pointer.Read(reader);
                    if (Settings.s.hasDeformations) {
                        Pointer.Goto(ref reader, new Pointer(Settings.s.memoryAddresses["anim_deformations"], offset.file));
                        banks[0].deformations.off_data = Pointer.Read(reader);
                    }
                }
            }
            Pointer off_current = Pointer.Current(reader);
            Pointer off_a3d = null;
            uint num_a3d = (uint)banks.Sum(b => b.a3d_general.count);
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
            } else if (l.mode != MapLoader.Mode.Rayman3GC) {
                for (uint i = 0; i < banks.Length; i++) {
                    banks[i].animations = new AnimA3DGeneral[banks[i].a3d_general.Count(append)];
                    banks[i].global_vectors = new AnimVector[banks[i].vectors.Count(append)];
                    banks[i].global_quaternions = new AnimQuaternion[banks[i].quaternions.Count(append)];
                    banks[i].global_hierarchies = new AnimHierarchy[banks[i].hierarchies.Count(append)];
                    banks[i].global_NTTO = new AnimNTTO[banks[i].NTTO.Count(append)];
                    banks[i].global_onlyFrames = new AnimOnlyFrame[banks[i].onlyFrames.Count(append)];
                    banks[i].global_channels = new AnimChannel[banks[i].channels.Count(append)];
                    banks[i].global_numOfNTTO = new AnimNumOfNTTO[banks[i].framesNumOfNTTO.Count(append)];
                    banks[i].global_framesKFIndex = new AnimFramesKFIndex[banks[i].framesKFIndex.Count(append)];
                    banks[i].global_keyframes = new AnimKeyframe[banks[i].keyframes.Count(append)];
                    banks[i].global_events = new AnimEvent[banks[i].events.Count(append)];
                    banks[i].global_morphData = new AnimMorphData[banks[i].morphData.Count(append)];
                    if(Settings.s.hasDeformations) banks[i].global_deformations = new AnimDeformation[banks[i].deformations.Count(append)];
                    if (banks[i].animations.Length > 0) {
                        if (banks[i].a3d_general.off_data != null) Pointer.Goto(ref reader, banks[i].a3d_general.off_data);
                        for (uint j = 0; j < banks[i].animations.Length; j++) banks[i].animations[j] = AnimA3DGeneral.Read(reader, Pointer.Current(reader));
                        if (Settings.s.engineMode == Settings.EngineMode.R2) {
                            if (Settings.s.isR2Demo) {
                                reader.Align(52 * banks[i].animations.Length, 4);
                            } else {
                                reader.Align(56 * banks[i].animations.Length, 4);
                            }
                        }
                    }
                    if (banks[i].global_vectors.Length > 0) {
                        if(banks[i].vectors.off_data != null) Pointer.Goto(ref reader, banks[i].vectors.off_data);
                        for (uint j = 0; j < banks[i].global_vectors.Length; j++) banks[i].global_vectors[j] = AnimVector.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(12 * banks[i].global_vectors.Length, 4);
                    }
                    if (banks[i].global_quaternions.Length > 0) {
                        if (banks[i].quaternions.off_data != null) Pointer.Goto(ref reader, banks[i].quaternions.off_data);
                        for (uint j = 0; j < banks[i].global_quaternions.Length; j++) banks[i].global_quaternions[j] = AnimQuaternion.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(8 * banks[i].global_quaternions.Length, 4);
                    }
                    if (banks[i].global_hierarchies.Length > 0) {
                        if (banks[i].hierarchies.off_data != null) Pointer.Goto(ref reader, banks[i].hierarchies.off_data);
                        for (uint j = 0; j < banks[i].global_hierarchies.Length; j++) banks[i].global_hierarchies[j] = AnimHierarchy.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(4 * banks[i].global_hierarchies.Length, 4);
                    }
                    if (banks[i].global_NTTO.Length > 0) {
                        if (banks[i].NTTO.off_data != null) Pointer.Goto(ref reader, banks[i].NTTO.off_data);
                        for (uint j = 0; j < banks[i].global_NTTO.Length; j++) banks[i].global_NTTO[j] = AnimNTTO.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(6 * banks[i].global_NTTO.Length, 4);
                    }
                    if (banks[i].global_onlyFrames.Length > 0) {
                        if (banks[i].onlyFrames.off_data != null) Pointer.Goto(ref reader, banks[i].onlyFrames.off_data);
                        for (uint j = 0; j < banks[i].global_onlyFrames.Length; j++) banks[i].global_onlyFrames[j] = AnimOnlyFrame.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(10 * banks[i].global_onlyFrames.Length, 4);
                    }
                    if (banks[i].global_channels.Length > 0) {
                        if (banks[i].channels.off_data != null) Pointer.Goto(ref reader, banks[i].channels.off_data);
                        for (uint j = 0; j < banks[i].global_channels.Length; j++) banks[i].global_channels[j] = AnimChannel.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(16 * banks[i].global_channels.Length, 4);
                    }
                    if (banks[i].global_numOfNTTO.Length > 0) {
                        if (banks[i].framesNumOfNTTO.off_data != null) Pointer.Goto(ref reader, banks[i].framesNumOfNTTO.off_data);
                        for (uint j = 0; j < banks[i].global_numOfNTTO.Length; j++) banks[i].global_numOfNTTO[j] = AnimNumOfNTTO.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(2 * banks[i].global_numOfNTTO.Length, 4);
                    }
                    if (banks[i].global_framesKFIndex.Length > 0) {
                        if (banks[i].framesKFIndex.off_data != null) Pointer.Goto(ref reader, banks[i].framesKFIndex.off_data);
                        for (uint j = 0; j < banks[i].global_framesKFIndex.Length; j++) banks[i].global_framesKFIndex[j] = AnimFramesKFIndex.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) {
                            if (Settings.s.isR2Demo) {
                                reader.Align(2 * banks[i].global_framesKFIndex.Length, 4);
                            } else {
                                reader.Align(4 * banks[i].global_framesKFIndex.Length, 4);
                            }
                        }
                    }
                    if (banks[i].global_keyframes.Length > 0) {
                        if (banks[i].keyframes.off_data != null) Pointer.Goto(ref reader, banks[i].keyframes.off_data);
                        if (kfFile != null) {
                            int alignBytes = reader.ReadInt32();
                            if(alignBytes > 0) reader.Align(4, alignBytes);
                        }
                        for (uint j = 0; j < banks[i].global_keyframes.Length; j++) banks[i].global_keyframes[j] = AnimKeyframe.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(36 * banks[i].global_keyframes.Length, 4);
                    }
                    if (banks[i].global_events.Length > 0) {
                        if (banks[i].events.off_data != null) Pointer.Goto(ref reader, banks[i].events.off_data);
                        for (uint j = 0; j < banks[i].global_events.Length; j++) banks[i].global_events[j] = AnimEvent.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(12 * banks[i].global_events.Length, 4);
                    }
                    if (banks[i].global_morphData.Length > 0) {
                        if (banks[i].morphData.off_data != null) Pointer.Goto(ref reader, banks[i].morphData.off_data);
                        for (uint j = 0; j < banks[i].global_morphData.Length; j++) banks[i].global_morphData[j] = AnimMorphData.Read(reader);
                        if (Settings.s.engineMode == Settings.EngineMode.R2) reader.Align(8 * banks[i].global_morphData.Length, 4);
                    }
                    if (banks[i].global_deformations != null && banks[i].global_deformations.Length > 0) {
                        if (banks[i].deformations.off_data != null) Pointer.Goto(ref reader, banks[i].deformations.off_data);
                        for (uint j = 0; j < banks[i].global_deformations.Length; j++) banks[i].global_deformations[j] = AnimDeformation.Read(reader);
                    }
                    if (append) {
                        AnimationBank b = l.animationBanks[index + i];
                        b.a3d_general = banks[i].a3d_general;
                        b.vectors = banks[i].vectors;
                        b.quaternions = banks[i].quaternions;
                        b.hierarchies = banks[i].hierarchies;
                        b.NTTO = banks[i].NTTO;
                        b.onlyFrames = banks[i].onlyFrames;
                        b.channels = banks[i].channels;
                        b.framesNumOfNTTO = banks[i].framesNumOfNTTO;
                        b.framesKFIndex = banks[i].framesKFIndex;
                        b.keyframes = banks[i].keyframes;
                        b.events = banks[i].events;
                        b.morphData = banks[i].morphData;
                        b.deformations = banks[i].deformations;
                        //Util.AppendArray(ref b.animations, ref banks[i].animations);
                        Util.AppendArrayAndMergeReferences(ref b.global_vectors, ref banks[i].global_vectors, (int)banks[i].vectors.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_quaternions, ref banks[i].global_quaternions, (int)banks[i].quaternions.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_hierarchies, ref banks[i].global_hierarchies, (int)banks[i].hierarchies.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_NTTO, ref banks[i].global_NTTO, (int)banks[i].NTTO.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_onlyFrames, ref banks[i].global_onlyFrames, (int)banks[i].onlyFrames.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_channels, ref banks[i].global_channels, (int)banks[i].channels.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_numOfNTTO, ref banks[i].global_numOfNTTO, (int)banks[i].framesNumOfNTTO.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_framesKFIndex, ref banks[i].global_framesKFIndex, (int)banks[i].framesKFIndex.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_keyframes, ref banks[i].global_keyframes, (int)banks[i].keyframes.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_events, ref banks[i].global_events, (int)banks[i].events.countInFix);
                        Util.AppendArrayAndMergeReferences(ref b.global_morphData, ref banks[i].global_morphData, (int)banks[i].morphData.countInFix);
                        if(banks[i].deformations != null) Util.AppendArrayAndMergeReferences(ref b.global_deformations, ref banks[i].global_deformations, (int)banks[i].deformations.countInFix);
                        /*Array.Resize(ref b.animations, b.animations.Length + banks[i].animations.Length);
                        Array.Resize(ref b.global_vectors, b.global_vectors.Length + banks[i].global_vectors.Length);
                        Array.Resize(ref b.global_quaternions, b.global_quaternions.Length + banks[i].global_quaternions.Length);
                        Array.Resize(ref b.global_hierarchies, b.global_hierarchies.Length + banks[i].global_hierarchies.Length);
                        Array.Resize(ref b.global_NTTO, b.global_NTTO.Length + banks[i].global_NTTO.Length);
                        Array.Resize(ref b.global_onlyFrames, b.global_onlyFrames.Length + banks[i].global_onlyFrames.Length);
                        Array.Resize(ref b.global_channels, b.global_channels.Length + banks[i].global_channels.Length);
                        Array.Resize(ref b.global_numOfNTTO, b.global_numOfNTTO.Length + banks[i].global_numOfNTTO.Length);
                        Array.Resize(ref b.global_framesKFIndex, b.global_framesKFIndex.Length + banks[i].global_framesKFIndex.Length);
                        Array.Resize(ref b.global_keyframes, b.global_keyframes.Length + banks[i].global_keyframes.Length);
                        Array.Resize(ref b.global_events, b.global_events.Length + banks[i].global_events.Length);
                        Array.Resize(ref b.global_morphData, b.global_morphData.Length + banks[i].global_morphData.Length);
                        if (banks[i].deformations != null) Array.Resize(ref b.global_deformations, b.global_deformations.Length + banks[i].global_deformations.Length);
                        Array.Copy(banks[i].global_vectors, 0, b.global_vectors, banks[i].vectors.countInFix, banks[i].global_vectors.Length);
                        Array.Copy(banks[i].global_quaternions, 0, b.global_quaternions, banks[i].quaternions.countInFix, banks[i].global_quaternions.Length);
                        Array.Copy(banks[i].global_hierarchies, 0, b.global_hierarchies, banks[i].hierarchies.countInFix, banks[i].global_hierarchies.Length);
                        Array.Copy(banks[i].global_NTTO, 0, b.global_NTTO, banks[i].NTTO.countInFix, banks[i].global_NTTO.Length);
                        Array.Copy(banks[i].global_onlyFrames, 0, b.global_onlyFrames, banks[i].onlyFrames.countInFix, banks[i].global_onlyFrames.Length);
                        Array.Copy(banks[i].global_channels, 0, b.global_channels, banks[i].channels.countInFix, banks[i].global_channels.Length);
                        Array.Copy(banks[i].global_numOfNTTO, 0, b.global_numOfNTTO, banks[i].framesNumOfNTTO.countInFix, banks[i].global_numOfNTTO.Length);
                        Array.Copy(banks[i].global_framesKFIndex, 0, b.global_framesKFIndex, banks[i].framesKFIndex.countInFix, banks[i].global_framesKFIndex.Length);
                        Array.Copy(banks[i].global_keyframes, 0, b.global_keyframes, banks[i].keyframes.countInFix, banks[i].global_keyframes.Length);
                        Array.Copy(banks[i].global_events, 0, b.global_events, banks[i].events.countInFix, banks[i].global_events.Length);
                        Array.Copy(banks[i].global_morphData, 0, b.global_morphData, banks[i].morphData.countInFix, banks[i].global_morphData.Length);
                        if (banks[i].deformations != null) Array.Copy(banks[i].global_deformations, 0, b.global_deformations, banks[i].deformations.countInFix, banks[i].global_deformations.Length);*/
                    }

                    if (banks[i].animations.Length > 0) {
                        AnimationBank srcBank = append ? l.animationBanks[index+i] : banks[i];
                        for (uint j = 0; j < banks[i].animations.Length; j++) {
                            banks[i].animations[j].vectors = srcBank.global_vectors;
                            banks[i].animations[j].quaternions = srcBank.global_quaternions;
                            banks[i].animations[j].hierarchies = srcBank.global_hierarchies;
                            banks[i].animations[j].ntto = srcBank.global_NTTO;
                            banks[i].animations[j].onlyFrames = srcBank.global_onlyFrames;
                            banks[i].animations[j].channels = srcBank.global_channels;
                            banks[i].animations[j].numOfNTTO = srcBank.global_numOfNTTO;
                            banks[i].animations[j].framesKFIndex = srcBank.global_framesKFIndex;
                            banks[i].animations[j].keyframes = srcBank.global_keyframes;
                            banks[i].animations[j].events = srcBank.global_events;
                            banks[i].animations[j].morphData = srcBank.global_morphData;
                            banks[i].animations[j].deformations = srcBank.global_deformations;
                        }
                    }
                    if(append) Util.AppendArrayAndMergeReferences(ref l.animationBanks[index + i].animations, ref banks[i].animations, (int)banks[i].a3d_general.countInFix);
                }
            }
            Pointer.Goto(ref reader, off_current);
            return banks;
        }
    }
}
