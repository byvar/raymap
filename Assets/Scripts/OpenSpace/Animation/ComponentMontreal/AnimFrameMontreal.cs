using Newtonsoft.Json;
using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentMontreal {
    public class AnimFrameMontreal : OpenSpaceStruct {
        public AnimationMontreal anim;
        public LegacyPointer off_channels;
        public LegacyPointer off_mat;
        public LegacyPointer off_vec;
        public LegacyPointer off_hierarchies;

        public AnimChannelMontreal[] channels = null;
        public AnimHierarchy[] hierarchies = null;

		protected override void ReadInternal(Reader reader) {
			MapLoader l = MapLoader.Loader;

			off_channels = LegacyPointer.Read(reader);
			off_mat = LegacyPointer.Read(reader);
			off_vec = LegacyPointer.Read(reader);
			off_hierarchies = LegacyPointer.Read(reader);

			// Read channels
			LegacyPointer.DoAt(ref reader, off_channels, () => {
				channels = new AnimChannelMontreal[anim.num_channels];
				for (uint i = 0; i < channels.Length; i++) {
					LegacyPointer off_channel = LegacyPointer.Read(reader);
					channels[i] = l.FromOffsetOrRead<AnimChannelMontreal>(reader, off_channel);
				}
			});

			// Read hierarchies
			LegacyPointer.DoAt(ref reader, off_hierarchies, () => {
				uint num_hierarchies = reader.ReadUInt32();
				LegacyPointer off_hierarchies2 = LegacyPointer.Read(reader);
				hierarchies = l.ReadArray<AnimHierarchy>((int)num_hierarchies, reader, off_hierarchies2);
			});
		}
	}
}
