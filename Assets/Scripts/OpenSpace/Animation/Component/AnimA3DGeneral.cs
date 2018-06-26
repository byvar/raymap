using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.Component {
    public class AnimA3DGeneral {
        public Pointer offset;

        public ushort unk_0;
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
        public ushort unk_1C;
        public ushort unk_1E;
        public ushort unk_20;
        public ushort unk_22;
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

        public static AnimA3DGeneral Read(EndianBinaryReader reader, Pointer offset) {
            MapLoader l = MapLoader.Loader;
            AnimA3DGeneral a3d = new AnimA3DGeneral(offset);
            if (l.mode == MapLoader.Mode.Rayman2PC) {
                /* Each a3d is 0x38 long */
                a3d.unk_0 = reader.ReadUInt16();
                a3d.num_vectors = reader.ReadUInt16();
                a3d.num_quaternions = reader.ReadUInt16();
                a3d.num_hierarchies = reader.ReadUInt16();
                a3d.num_NTTO = reader.ReadUInt16();
                a3d.num_numNTTO = reader.ReadUInt16();
                a3d.num_channels = reader.ReadUInt16();
                a3d.num_onlyFrames = reader.ReadUInt16();
                a3d.num_keyframes = reader.ReadUInt16();
                a3d.unk_14 = reader.ReadUInt16();
                a3d.num_events = reader.ReadUInt16();
                a3d.unk_1A = reader.ReadUInt16(); // vector related
                a3d.unk_1C = reader.ReadUInt16();
                a3d.unk_1E = reader.ReadUInt16(); // only frames again?
                a3d.unk_20 = reader.ReadUInt16(); // field0 again?
                a3d.unk_22 = reader.ReadUInt16();
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
                reader.ReadUInt16(); // padding?
            } else {
                /* Each a3d is 0x3c long */
                a3d.unk_0 = reader.ReadUInt16();
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
                a3d.unk_1C = reader.ReadUInt16();
                a3d.unk_1E = reader.ReadUInt16();
                a3d.unk_20 = reader.ReadUInt16();
                a3d.unk_22 = reader.ReadUInt16();
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
    }
}
