using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1StreamFrame : OpenSpaceStruct {
		public int num_frame;
		public int num_channels;

		public PS1StreamFrameChannel[] channels;

		protected override void ReadInternal(Reader reader) {
			num_frame = reader.ReadInt32();
			num_channels = reader.ReadInt32();

			channels = Load.ReadArray<PS1StreamFrameChannel>(num_channels, reader);
			//Load.print(num_frame + " - " + num_channels + " - " + Pointer.Current(reader));
		}
	}
}
