using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationChannel : OpenSpaceStruct {
		public LegacyPointer off_frames;
		public ushort num_frames;
		public short id;

		public PS1AnimationKeyframe[] frames;

		protected override void ReadInternal(Reader reader) {
			off_frames = LegacyPointer.Read(reader);
			num_frames = reader.ReadUInt16();
			id = reader.ReadInt16();

			//Load.print("Channel: " + id + " - " + num_frames + " - " + off_frames);

			frames = Load.ReadArray<PS1AnimationKeyframe>(num_frames, reader, off_frames);
		}
	}
}
