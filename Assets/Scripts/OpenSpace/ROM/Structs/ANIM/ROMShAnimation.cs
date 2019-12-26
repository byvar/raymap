using OpenSpace.ROM.ANIM.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.ROM {
	// Data in shanims.bin
	public class ROMShAnimation : OpenSpaceStruct {
		public ushort num_frames;
		public ushort speed;
		public ushort num_channels;
		public ushort num_events;
		public ushort num_morphData;
		public ushort wordA;
		public float x;
		public float y;
		public float z;
		public ushort word18;
		public ushort word1A;
		public ushort word1C;
		public ushort word1E;

		protected override void ReadInternal(Reader reader) {
			num_frames = reader.ReadUInt16();
			speed = reader.ReadUInt16();
			num_channels = reader.ReadUInt16();
			num_events = reader.ReadUInt16();
			num_morphData = reader.ReadUInt16();
			wordA = reader.ReadUInt16();
			x = reader.ReadSingle();
			y = reader.ReadSingle();
			z = reader.ReadSingle();
			word18 = reader.ReadUInt16();
			word1A = reader.ReadUInt16();
			word1C = reader.ReadUInt16();
			word1E = reader.ReadUInt16();
		}
	}
}
