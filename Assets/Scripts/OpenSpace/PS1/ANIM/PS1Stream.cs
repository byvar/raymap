using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Stream : OpenSpaceStruct {
		public List<PS1StreamFrame> frames = new List<PS1StreamFrame>();
		public int num_frames;

		protected override void ReadInternal(Reader reader) {
			while (reader.BaseStream.Position <= reader.BaseStream.Length - 8) {
				PS1StreamFrame f = Load.FromOffsetOrRead<PS1StreamFrame>(reader, Pointer.Current(reader), inline: true);
				if (f.num_frame == -1) {
					num_frames = frames[frames.Count - 1].num_frame;
				}
				frames.Add(f);
				if (f.num_frame > 0 && (f.num_frame + 1) % 30 == 0) {
					reader.Align(4);
				}
			}
			if (num_frames == 0) {
				num_frames = frames.Count;
			}
		}
	}
}
