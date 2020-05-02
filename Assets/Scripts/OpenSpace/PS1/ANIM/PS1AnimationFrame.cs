using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationFrame : OpenSpaceStruct {
		public Pointer off_channels;
		public ushort num_channels;
		public ushort unk;

		public PS1AnimationChannel[] channels;

		protected override void ReadInternal(Reader reader) {
			off_channels = Pointer.Read(reader);
			num_channels = reader.ReadUInt16();
			unk = reader.ReadUInt16();

			channels = Load.ReadArray<PS1AnimationChannel>(num_channels, reader, off_channels);
		}
	}
}
