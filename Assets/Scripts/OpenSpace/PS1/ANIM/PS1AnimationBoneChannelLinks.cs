using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1AnimationBoneChannelLinks : OpenSpaceStruct {
		public ushort ind_ntto_channel;
		public ushort num_indices;
		public LegacyPointer off_indices; // channels

		// Parsed
		public ushort[] indices;

		protected override void ReadInternal(Reader reader) {
			ind_ntto_channel = reader.ReadUInt16();
			num_indices = reader.ReadUInt16();
			off_indices = LegacyPointer.Read(reader);

			LegacyPointer.DoAt(ref reader, off_indices, () => {
				//Load.print("Bone: " + ind_ntto_channel + " - " + off_indices + " - " + num_indices);
				indices = new ushort[num_indices];
				for (int i = 0; i < num_indices; i++) {
					indices[i] = reader.ReadUInt16();
				}
			});
		}
	}
}
