using OpenSpace.Animation.Component;
using OpenSpace.Object.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationBank : IEquatable<AnimationBank> {
        public LegacyPointer off_header;
        public LegacyPointer off_data;
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

        public AnimationBank(LegacyPointer off_header) {
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

        public static AnimationBank[] Read(Reader reader, LegacyPointer offset, uint index, uint num_banks, FileFormat.FileWithPointers kfFile, bool append = false) {
            MapLoader l = MapLoader.Loader;
            AnimationBank[] banks = new AnimationBank[num_banks];

            for (int i = 0; i < num_banks; i++) {
                // In R3, each animation bank is of size 0x104 = 13 times a stack description of 5 uint32s.
                banks[i] = new AnimationBank(LegacyPointer.Current(reader));
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
                if (CPA_Settings.s.hasDeformations) {
                    banks[i].deformations = AnimationStack.Read(reader);
                } else {
                    banks[i].deformations = null;
                }
                banks[i].animations = new AnimA3DGeneral[banks[i].a3d_general.count];
            }
            if (CPA_Settings.s.mode != CPA_Settings.Mode.Rayman3GC && !append) {
                if (!CPA_Settings.s.loadFromMemory) {
                    for (int i = 0; i < num_banks; i++) {
                        if (banks[i].a3d_general.reservedMemory > 0) banks[i].a3d_general.off_data = LegacyPointer.Read(reader);
                        if (banks[i].vectors.reservedMemory > 0) banks[i].vectors.off_data = LegacyPointer.Read(reader);
                        if (banks[i].quaternions.reservedMemory > 0) banks[i].quaternions.off_data = LegacyPointer.Read(reader);
                        if (banks[i].hierarchies.reservedMemory > 0) banks[i].hierarchies.off_data = LegacyPointer.Read(reader);
                        if (banks[i].NTTO.reservedMemory > 0) banks[i].NTTO.off_data = LegacyPointer.Read(reader);
                        if (banks[i].onlyFrames.reservedMemory > 0) banks[i].onlyFrames.off_data = LegacyPointer.Read(reader);
                        if (banks[i].channels.reservedMemory > 0) banks[i].channels.off_data = LegacyPointer.Read(reader);
                        if (banks[i].framesNumOfNTTO.reservedMemory > 0) banks[i].framesNumOfNTTO.off_data = LegacyPointer.Read(reader);
                        if (banks[i].framesKFIndex.reservedMemory > 0) banks[i].framesKFIndex.off_data = LegacyPointer.Read(reader);
                        if (kfFile == null) {
                            if (banks[i].keyframes.reservedMemory > 0) banks[i].keyframes.off_data = LegacyPointer.Read(reader);
                        } else {
                            banks[i].keyframes.off_data = new LegacyPointer(0, kfFile);
                        }
                        if (banks[i].events.reservedMemory > 0) banks[i].events.off_data = LegacyPointer.Read(reader);
                        if (banks[i].morphData.reservedMemory > 0) banks[i].morphData.off_data = LegacyPointer.Read(reader);
                        if (CPA_Settings.s.hasDeformations && banks[i].deformations.reservedMemory > 0) banks[i].deformations.off_data = LegacyPointer.Read(reader);
                    }
                } else {
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_a3d"], offset.file));
                    for(int i = 0; i < num_banks; i++) banks[i].a3d_general.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_vectors"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].vectors.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_quaternions"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].quaternions.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_hierarchies"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].hierarchies.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_NTTO"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].NTTO.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_onlyFrames"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].onlyFrames.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_channels"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].channels.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_framesNumOfNTTO"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].framesNumOfNTTO.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_framesKF"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].framesKFIndex.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_keyframes"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].keyframes.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_events"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].events.off_data = LegacyPointer.Read(reader);
                    LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_morphData"], offset.file));
                    for (int i = 0; i < num_banks; i++) banks[i].morphData.off_data = LegacyPointer.Read(reader);
                    if (CPA_Settings.s.hasDeformations) {
                        LegacyPointer.Goto(ref reader, new LegacyPointer(CPA_Settings.s.memoryAddresses["anim_deformations"], offset.file));
                        for (int i = 0; i < num_banks; i++) banks[i].deformations.off_data = LegacyPointer.Read(reader);
                    }
                }
            }
            LegacyPointer off_current = LegacyPointer.Current(reader);
            LegacyPointer off_a3d = null;
            uint num_a3d = (uint)banks.Sum(b => b.a3d_general.count);
            if (kfFile != null && CPA_Settings.s.mode == CPA_Settings.Mode.Rayman3GC) {
                kfFile.GotoHeader();
                reader = kfFile.reader;
                uint[] a3d_sizes = new uint[num_a3d];
                for (uint i = 0; i < num_a3d; i++) {
                    a3d_sizes[i] = reader.ReadUInt32();
                }
                off_a3d = LegacyPointer.Current(reader);
                uint current_anim = 0;
                for (uint i = 0; i < banks.Length; i++) {
                    uint num_a3d_in_bank = banks[i].a3d_general.count;
                    for (uint j = 0; j < num_a3d_in_bank; j++) {
                        LegacyPointer.Goto(ref reader, off_a3d);
						// Read animation data here
						banks[i].animations[j] = l.FromOffsetOrRead<AnimA3DGeneral>(reader, off_a3d, onPreRead: (a3d) => {
							a3d.readFull = true;
						}, inline: true);
                        off_a3d += a3d_sizes[current_anim];

                        // Check if read correctly
                        LegacyPointer off_postAnim = LegacyPointer.Current(reader);
                        if (off_postAnim != off_a3d) l.print("Animation block size does not match data size: " +
                            "Current offset: " + off_postAnim + " - Expected offset: " + off_a3d +
                            " - Block start: " + (off_a3d + -(int)(a3d_sizes[current_anim])));

                        current_anim++;
                    }
                }
            } else if (CPA_Settings.s.mode != CPA_Settings.Mode.Rayman3GC) {
                for (uint i = 0; i < banks.Length; i++) {
                    if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) reader.AutoAligning = true;
                    
					if (banks[i].a3d_general.off_data != null) LegacyPointer.Goto(ref reader, banks[i].a3d_general.off_data);
					banks[i].animations = l.ReadArray<AnimA3DGeneral>(banks[i].a3d_general.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
					if (banks[i].vectors.off_data != null) LegacyPointer.Goto(ref reader, banks[i].vectors.off_data);
					banks[i].global_vectors = l.ReadArray<AnimVector>(banks[i].vectors.Count(append), reader);

					if (reader.AutoAligning) reader.AutoAlign(4);
					if (banks[i].quaternions.off_data != null) LegacyPointer.Goto(ref reader, banks[i].quaternions.off_data);
					banks[i].global_quaternions = l.ReadArray<AnimQuaternion>(banks[i].quaternions.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].hierarchies.off_data != null) LegacyPointer.Goto(ref reader, banks[i].hierarchies.off_data);
					banks[i].global_hierarchies = l.ReadArray<AnimHierarchy>(banks[i].hierarchies.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
					if (banks[i].NTTO.off_data != null) LegacyPointer.Goto(ref reader, banks[i].NTTO.off_data);
					banks[i].global_NTTO = l.ReadArray<AnimNTTO>(banks[i].NTTO.Count(append), reader);

					if (reader.AutoAligning) reader.AutoAlign(4);
					if (banks[i].onlyFrames.off_data != null) LegacyPointer.Goto(ref reader, banks[i].onlyFrames.off_data);
					banks[i].global_onlyFrames = l.ReadArray<AnimOnlyFrame>(banks[i].onlyFrames.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].channels.off_data != null) LegacyPointer.Goto(ref reader, banks[i].channels.off_data);
                    banks[i].global_channels = l.ReadArray<AnimChannel>(banks[i].channels.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].framesNumOfNTTO.off_data != null) LegacyPointer.Goto(ref reader, banks[i].framesNumOfNTTO.off_data);
                    banks[i].global_numOfNTTO = l.ReadArray<AnimNumOfNTTO>(banks[i].framesNumOfNTTO.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].framesKFIndex.off_data != null) LegacyPointer.Goto(ref reader, banks[i].framesKFIndex.off_data);
					banks[i].global_framesKFIndex = l.ReadArray<AnimFramesKFIndex>(banks[i].framesKFIndex.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].keyframes.off_data != null) LegacyPointer.Goto(ref reader, banks[i].keyframes.off_data);
                    if (banks[i].keyframes.Count(append) > 0 && kfFile != null) {
                        int alignBytes = reader.ReadInt32();
                        if(alignBytes > 0) reader.Align(4, alignBytes);
                    }
                    banks[i].global_keyframes = l.ReadArray<AnimKeyframe>(banks[i].keyframes.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].events.off_data != null) LegacyPointer.Goto(ref reader, banks[i].events.off_data);
					banks[i].global_events = l.ReadArray<AnimEvent>(banks[i].events.Count(append), reader);

                    if (reader.AutoAligning) reader.AutoAlign(4);
                    if (banks[i].morphData.off_data != null) LegacyPointer.Goto(ref reader, banks[i].morphData.off_data);
					banks[i].global_morphData = l.ReadArray<AnimMorphData>(banks[i].morphData.Count(append), reader);

					if (CPA_Settings.s.hasDeformations) {
						if (reader.AutoAligning) reader.AutoAlign(4);
						if (banks[i].deformations.off_data != null) LegacyPointer.Goto(ref reader, banks[i].deformations.off_data);
						banks[i].global_deformations = l.ReadArray<AnimDeformation>(banks[i].deformations.Count(append), reader);
					}
                    reader.AutoAligning = false;
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
            LegacyPointer.Goto(ref reader, off_current);
            //Debug.LogError("Bank here: " + offset);
            return banks;
        }

        // A completely separate Read function for the Dreamcast version, since it's so different
        public static AnimationBank ReadDreamcast(Reader reader, LegacyPointer offset, LegacyPointer off_events_fix, uint num_events_fix) {
            MapLoader l = MapLoader.Loader;
            AnimationBank bank = new AnimationBank(offset);
            bank.a3d_general = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.vectors = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.quaternions = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.hierarchies = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.NTTO = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.onlyFrames = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.channels = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.framesNumOfNTTO = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.framesKFIndex = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.keyframes = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            bank.events = new AnimationStack() { off_data = LegacyPointer.Read(reader), countInFix = num_events_fix };
            bank.events.count = bank.events.countInFix + reader.ReadUInt32();
            bank.morphData = new AnimationStack() { off_data = LegacyPointer.Read(reader) };
            LegacyPointer off_current = LegacyPointer.Current(reader);

            uint max_a3d_ind = 0, num_vectors = 0, num_quaternions = 0, num_hierarchies = 0, num_NTTO = 0,
                num_numNTTO = 0, num_channels = 0, num_onlyFrames = 0, num_framesKF = 0, num_keyframes = 0, num_morphData = 0;
            foreach (State s in l.states) {
                if (s.anim_ref != null && s.anim_ref.anim_index > max_a3d_ind) max_a3d_ind = s.anim_ref.anim_index;
            }


            bank.a3d_general.count = max_a3d_ind + 1;
			bank.animations = l.ReadArray<AnimA3DGeneral>(bank.a3d_general.count, reader, bank.a3d_general.off_data);
			for (int i = 0; i < bank.a3d_general.count; i++) {
				AnimA3DGeneral a = bank.animations[i];
				if (a.start_vectors + a.num_vectors > num_vectors) num_vectors = (uint)a.start_vectors + a.num_vectors;
				if (a.start_quaternions + a.num_quaternions > num_quaternions) num_quaternions = (uint)a.start_quaternions + a.num_quaternions;
				//if (a.start_hierarchies + a.num_hierarchies > num_hierarchies) num_hierarchies = (uint)a.start_hierarchies + a.num_hierarchies;
				if (a.start_NTTO + a.num_NTTO > num_NTTO) num_NTTO = (uint)a.start_NTTO + a.num_NTTO;
				if (a.start_onlyFrames + a.num_onlyFrames > num_onlyFrames) num_onlyFrames = (uint)a.start_onlyFrames + a.num_onlyFrames;
				if (a.start_channels + a.num_channels > num_channels) num_channels = (uint)a.start_channels + a.num_channels;
				if (a.start_morphData + a.num_morphData > num_morphData) num_morphData = (uint)a.start_morphData + a.num_morphData;
			}

            bank.vectors.count = num_vectors;
            bank.quaternions.count = num_quaternions;
            bank.NTTO.count = num_NTTO;
            bank.onlyFrames.count = num_onlyFrames;
            bank.channels.count = num_channels;
            bank.morphData.count = num_morphData;
            
			bank.global_vectors = l.ReadArray<AnimVector>(bank.vectors.Count(false), reader, bank.vectors.off_data);
			bank.global_quaternions = l.ReadArray<AnimQuaternion>(bank.quaternions.Count(false), reader, bank.quaternions.off_data);
			bank.global_NTTO = l.ReadArray<AnimNTTO>(bank.NTTO.Count(false), reader, bank.NTTO.off_data);
			bank.global_onlyFrames = l.ReadArray<AnimOnlyFrame>(bank.onlyFrames.Count(false), reader, bank.onlyFrames.off_data);
			bank.global_channels = l.ReadArray<AnimChannel>(bank.channels.Count(false), reader, bank.channels.off_data);

			AnimEvent[] eventsFix = l.ReadArray<AnimEvent>(num_events_fix, reader, off_events_fix);
			AnimEvent[] eventsLvl = l.ReadArray<AnimEvent>(bank.events.Count(false) - num_events_fix, reader, bank.events.off_data);
			bank.global_events = new AnimEvent[bank.events.Count(false)];
			Array.Copy(eventsFix, 0, bank.global_events, 0, num_events_fix);
			Array.Copy(eventsLvl, 0, bank.global_events, num_events_fix, bank.global_events.Length - num_events_fix);

			bank.global_morphData = l.ReadArray<AnimMorphData>(bank.morphData.Count(false), reader, bank.morphData.off_data);

            for (int i = 0; i < bank.a3d_general.count; i++) {
                AnimA3DGeneral a = bank.animations[i];
                for (int j = a.start_channels; j < a.start_channels + a.num_channels; j++) {
                    AnimChannel ch = bank.global_channels[j];
                    if (ch.framesKF + a.num_onlyFrames > num_framesKF) num_framesKF = (uint)ch.framesKF + a.num_onlyFrames;

                    for (int k = a.start_onlyFrames; k < a.start_onlyFrames + a.num_onlyFrames; k++) {
                        AnimOnlyFrame of = bank.global_onlyFrames[k];
                        if (ch.numOfNTTO + of.numOfNTTO + 1 > num_numNTTO) num_numNTTO = (uint)ch.numOfNTTO + of.numOfNTTO + 1;
                    }
                }
                for (int k = a.start_onlyFrames; k < a.start_onlyFrames + a.num_onlyFrames; k++) {
                    AnimOnlyFrame of = bank.global_onlyFrames[k];
                    if (of.start_hierarchies_for_frame + of.num_hierarchies_for_frame > num_hierarchies) num_hierarchies = (uint)of.start_hierarchies_for_frame + of.num_hierarchies_for_frame;
                }
            }
            bank.hierarchies.count = num_hierarchies;
            bank.framesNumOfNTTO.count = num_numNTTO;
            bank.framesKFIndex.count = num_framesKF;
   
			bank.global_hierarchies = l.ReadArray<AnimHierarchy>(bank.hierarchies.Count(false), reader, bank.hierarchies.off_data);
			bank.global_numOfNTTO = l.ReadArray<AnimNumOfNTTO>(bank.framesNumOfNTTO.Count(false), reader, bank.framesNumOfNTTO.off_data);
			bank.global_framesKFIndex = l.ReadArray<AnimFramesKFIndex>(bank.framesKFIndex.Count(false), reader, bank.framesKFIndex.off_data);
			for (uint j = 0; j < bank.global_framesKFIndex.Length; j++) {
				if (bank.global_framesKFIndex[j].kf + 1 > num_keyframes) num_keyframes = bank.global_framesKFIndex[j].kf + 1;
			}
			bank.keyframes.count = num_keyframes;
			bank.global_keyframes = l.ReadArray<AnimKeyframe>(bank.keyframes.Count(false), reader, bank.keyframes.off_data);

            LegacyPointer.Goto(ref reader, off_current);
            for (uint j = 0; j < bank.animations.Length; j++) {
                bank.animations[j].vectors = bank.global_vectors;
                bank.animations[j].quaternions = bank.global_quaternions;
                bank.animations[j].hierarchies = bank.global_hierarchies;
                bank.animations[j].ntto = bank.global_NTTO;
                bank.animations[j].onlyFrames = bank.global_onlyFrames;
                bank.animations[j].channels = bank.global_channels;
                bank.animations[j].numOfNTTO = bank.global_numOfNTTO;
                bank.animations[j].framesKFIndex = bank.global_framesKFIndex;
                bank.animations[j].keyframes = bank.global_keyframes;
                bank.animations[j].events = bank.global_events;
                bank.animations[j].morphData = bank.global_morphData;
                bank.animations[j].deformations = bank.global_deformations;
            }

            return bank;
        }
    }
}
