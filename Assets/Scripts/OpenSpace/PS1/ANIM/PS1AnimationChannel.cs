using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationChannel : OpenSpaceStruct {
		public Pointer off_frames;
		public ushort num_frames;
		public short id;

		public PS1AnimationChannelFrame[] frames;

		protected override void ReadInternal(Reader reader) {
			off_frames = Pointer.Read(reader);
			num_frames = reader.ReadUInt16();
			id = reader.ReadInt16();

			frames = Load.ReadArray<PS1AnimationChannelFrame>(num_frames, reader, off_frames);
		}
	}
}
