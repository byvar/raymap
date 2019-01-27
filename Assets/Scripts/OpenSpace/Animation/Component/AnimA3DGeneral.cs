using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimA3DGeneral {
        public Pointer offset;

        public ushort speed;
        public ushort num_vectors;
        public ushort num_quaternions;
        public ushort num_hierarchies;
        public ushort num_NTTO;
        public ushort num_numNTTO;
        public ushort num_deformations;
        public ushort num_channels;
        public ushort num_onlyFrames;
        public ushort unk_12;
        public ushort unk_14;
        public ushort num_keyframes;
        public ushort num_events;
        public ushort unk_1A;
        public ushort subtractFramesForSpeed;
        public ushort unk_1E;
        public ushort speed2;
        public ushort unk_22_morphs;
        public ushort start_vectors2;
        public ushort start_quaternions2;
        public ushort num_morphData;
        public ushort start_vectors;
        public ushort start_quaternions;
        public ushort start_hierarchies;
        public ushort start_NTTO;
        public ushort start_deformations;
        public ushort start_onlyFrames;
        public ushort start_channels;
        public ushort start_events;
        public ushort start_morphData;

        public AnimVector[] vectors;
        public AnimQuaternion[] quaternions;
        public AnimHierarchy[] hierarchies;
        public AnimNTTO[] ntto;
        public AnimOnlyFrame[] onlyFrames;
        public AnimChannel[] channels;
        public AnimNumOfNTTO[] numOfNTTO;
        public AnimFramesKFIndex[] framesKFIndex;
        public AnimKeyframe[] keyframes;
        public AnimEvent[] events;
        public AnimMorphData[] morphData;
        public AnimDeformation[] deformations;

        public AnimA3DGeneral(Pointer offset) { this.offset = offset; }

        public static AnimA3DGeneral Read(Reader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimA3DGeneral a3d = new AnimA3DGeneral(offset);
            if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
                /* Each a3d is 0x38 long in R2. 0x34 in TT */
                a3d.speed = reader.ReadUInt16();
                a3d.num_vectors = reader.ReadUInt16();
                a3d.num_quaternions = reader.ReadUInt16();
                a3d.num_hierarchies = reader.ReadUInt16();
                a3d.num_NTTO = reader.ReadUInt16();
                a3d.num_numNTTO = reader.ReadUInt16();
                a3d.num_channels = reader.ReadUInt16();
                a3d.num_onlyFrames = reader.ReadUInt16();
                a3d.num_keyframes = reader.ReadUInt16();
                if (Settings.s.engineVersion >= Settings.EngineVersion.R2 && Settings.s.game != Settings.Game.R2Demo) {
                    a3d.unk_14 = reader.ReadUInt16();
                }
                a3d.num_events = reader.ReadUInt16();
                a3d.unk_1A = reader.ReadUInt16(); // vector related
                a3d.subtractFramesForSpeed = reader.ReadUInt16();
                a3d.unk_1E = reader.ReadUInt16(); // only frames again?
                a3d.speed2 = reader.ReadUInt16(); // field0 again?
                a3d.unk_22_morphs = reader.ReadUInt16(); // morph count ? (GAM_fn_p_stGetMorphData)
                a3d.start_vectors2 = reader.ReadUInt16();
                a3d.start_quaternions2 = reader.ReadUInt16();
                a3d.num_morphData = reader.ReadUInt16();
                a3d.start_vectors = reader.ReadUInt16();
                a3d.start_quaternions = reader.ReadUInt16();
                a3d.start_hierarchies = reader.ReadUInt16();
                a3d.start_NTTO = reader.ReadUInt16();
                a3d.start_onlyFrames = reader.ReadUInt16();
                a3d.start_channels = reader.ReadUInt16();
                a3d.start_events = reader.ReadUInt16();
                a3d.start_morphData = reader.ReadUInt16();
                if (Settings.s.engineVersion >= Settings.EngineVersion.R2 && Settings.s.game != Settings.Game.R2Demo) {
                    reader.ReadUInt16(); // padding?
                }
                if (Settings.s.engineVersion == Settings.EngineVersion.TT) {
                    a3d.start_vectors2 = 0;
                    a3d.start_quaternions2 = 0;
                    a3d.start_vectors = 0;
                    a3d.start_quaternions = 0;
                    a3d.start_hierarchies = 0;
                    a3d.start_NTTO = 0;
                    a3d.start_onlyFrames = 0;
                    a3d.start_channels = 0;
                    a3d.start_events = 0;
                    a3d.start_morphData = 0;
                }
            } else {
                /* Each a3d is 0x3c long */
                a3d.speed = reader.ReadUInt16();
                a3d.num_vectors = reader.ReadUInt16();
                a3d.num_quaternions = reader.ReadUInt16();
                a3d.num_hierarchies = reader.ReadUInt16();
                a3d.num_NTTO = reader.ReadUInt16();
                a3d.num_numNTTO = reader.ReadUInt16();
                a3d.num_deformations = reader.ReadUInt16();
                a3d.num_channels = reader.ReadUInt16();
                a3d.num_onlyFrames = reader.ReadUInt16();
                a3d.unk_12 = reader.ReadUInt16();
                a3d.unk_14 = reader.ReadUInt16();
                a3d.num_keyframes = reader.ReadUInt16();
                a3d.num_events = reader.ReadUInt16();
                a3d.unk_1A = reader.ReadUInt16();
                a3d.subtractFramesForSpeed = reader.ReadUInt16();
                a3d.unk_1E = reader.ReadUInt16();
                a3d.speed2 = reader.ReadUInt16();
                a3d.unk_22_morphs = reader.ReadUInt16();
                a3d.start_vectors2 = reader.ReadUInt16();
                a3d.start_quaternions2 = reader.ReadUInt16();
                a3d.num_morphData = reader.ReadUInt16();
                a3d.start_vectors = reader.ReadUInt16();
                a3d.start_quaternions = reader.ReadUInt16();
                a3d.start_hierarchies = reader.ReadUInt16();
                a3d.start_NTTO = reader.ReadUInt16();
                a3d.start_deformations = reader.ReadUInt16();
                a3d.start_onlyFrames = reader.ReadUInt16();
                a3d.start_channels = reader.ReadUInt16();
                a3d.start_events = reader.ReadUInt16();
                a3d.start_morphData = reader.ReadUInt16();
            }
            return a3d;
        }

        public static AnimA3DGeneral ReadFull(Reader reader, Pointer offset) {
            reader.AutoAligning = true;
            AnimA3DGeneral a3d = AnimA3DGeneral.Read(reader, offset);
            a3d.vectors = new AnimVector[a3d.num_vectors];
            a3d.quaternions = new AnimQuaternion[a3d.num_quaternions];
            a3d.hierarchies = new AnimHierarchy[a3d.num_hierarchies];
            a3d.ntto = new AnimNTTO[a3d.num_NTTO];
            a3d.onlyFrames = new AnimOnlyFrame[a3d.num_onlyFrames];
            a3d.channels = new AnimChannel[a3d.num_channels];
            a3d.numOfNTTO = new AnimNumOfNTTO[a3d.num_numNTTO * a3d.num_channels];
            a3d.framesKFIndex = new AnimFramesKFIndex[a3d.num_onlyFrames * a3d.num_channels];
            a3d.keyframes = new AnimKeyframe[a3d.num_keyframes];
            a3d.events = new AnimEvent[a3d.num_events];
            a3d.morphData = new AnimMorphData[a3d.num_morphData];
            a3d.deformations = new AnimDeformation[a3d.num_deformations];
            if (AnimVector.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.vectors.Length; k++) a3d.vectors[k] = AnimVector.Read(reader);
            if (AnimQuaternion.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.quaternions.Length; k++) a3d.quaternions[k] = AnimQuaternion.Read(reader);
            if (AnimHierarchy.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.hierarchies.Length; k++) a3d.hierarchies[k] = AnimHierarchy.Read(reader);
            if (AnimNTTO.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.ntto.Length; k++) a3d.ntto[k] = AnimNTTO.Read(reader);
            if (AnimOnlyFrame.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.onlyFrames.Length; k++) a3d.onlyFrames[k] = AnimOnlyFrame.Read(reader);
            if (AnimChannel.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.channels.Length; k++) a3d.channels[k] = AnimChannel.Read(reader);
            if (AnimNumOfNTTO.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.numOfNTTO.Length; k++) a3d.numOfNTTO[k] = AnimNumOfNTTO.Read(reader);
            if (AnimFramesKFIndex.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.framesKFIndex.Length; k++) a3d.framesKFIndex[k] = AnimFramesKFIndex.Read(reader);
            if (AnimKeyframe.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.keyframes.Length; k++) a3d.keyframes[k] = AnimKeyframe.Read(reader);
            if (AnimEvent.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.events.Length; k++) a3d.events[k] = AnimEvent.Read(reader);
            if (AnimMorphData.Aligned) reader.AutoAlign(4);
            for (uint k = 0; k < a3d.morphData.Length; k++) a3d.morphData[k] = AnimMorphData.Read(reader);
            /*MapLoader.Loader.print("A3D: " + offset + " - " + Pointer.Current(reader)
                + " - NN:" + a3d.num_numNTTO
                + " - CH:" + a3d.num_channels
                + " - OF:" + a3d.num_onlyFrames
                + " - KF:" + a3d.num_keyframes
                + " - KFI: " + a3d.num_onlyFrames * a3d.num_channels
                + " - V:" + a3d.num_vectors
                + " - Q:" + a3d.num_quaternions);*/
            if (Settings.s.hasDeformations) {
                if (AnimDeformation.Aligned) reader.AutoAlign(4);
                for (uint k = 0; k < a3d.deformations.Length; k++) a3d.deformations[k] = AnimDeformation.Read(reader);
                //reader.Align(AnimDeformation.Size * a3d.deformations.Length, 4);
            }
            reader.AutoAligning = false;
            return a3d;
        }

        public static int Size {
            get {
                switch (Settings.s.engineVersion) {
                    case Settings.EngineVersion.R3: return 0x3C;
                    case Settings.EngineVersion.R2:
                        if (Settings.s.game == Settings.Game.R2Demo) {
                            return 0x34;
                        } else {
                            return 0x38;
                        }
                    case Settings.EngineVersion.TT: return 0x34;
                    default: throw new Exception("Anim A3D size not set for this engine version");
                }
            }
        }

        public static bool Aligned {
            get { return false; }
        }
    }
}
