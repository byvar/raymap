using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.ROM.ANIM.Component {
    public class AnimA3DGeneral {
        public ushort speed;
        public ushort num_vectors;
        public ushort num_quaternions;
        public ushort num_hierarchies;
        public ushort num_NTTO;
		public ushort num_numNTTO;
		public ushort num_channels;
		public ushort total_num_onlyFrames;
		public ushort num_keyframes;
		public ushort num_events;
		public ushort num_unk2;
		public ushort this_start_onlyFrames;
		public ushort this_num_onlyFrames;
		public ushort num_unk5;
		public ushort num_unk6;
		public ushort num_unk7;
		public ushort num_unk8;
		public ushort num_morphData;

		public AnimA3DGeneral(Reader reader) {
			ReadInternal(reader);
		}

		protected void ReadInternal(Reader reader) {
			speed = reader.ReadUInt16();
			num_vectors = reader.ReadUInt16();
			num_quaternions = reader.ReadUInt16();
			num_hierarchies = reader.ReadUInt16();
			num_NTTO = reader.ReadUInt16();
			num_numNTTO = reader.ReadUInt16();
			num_channels = reader.ReadUInt16();
			total_num_onlyFrames = reader.ReadUInt16();
			num_keyframes = reader.ReadUInt16();
			num_events = reader.ReadUInt16();
			num_unk2 = reader.ReadUInt16();
			this_start_onlyFrames = reader.ReadUInt16();
			this_num_onlyFrames = reader.ReadUInt16();
			num_unk5 = reader.ReadUInt16();
			num_unk6 = reader.ReadUInt16();
			num_unk7 = reader.ReadUInt16();
			num_unk8 = reader.ReadUInt16();
			num_morphData = reader.ReadUInt16();
		}

        public static bool Aligned {
            get { return false; }
        }
    }
}
