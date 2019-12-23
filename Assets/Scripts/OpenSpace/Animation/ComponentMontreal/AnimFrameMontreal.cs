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
        public Pointer off_channels;
        public Pointer off_mat;
        public Pointer off_vec;
        public Pointer off_hierarchies;

        public AnimChannelMontreal[] channels = null;
        public AnimHierarchy[] hierarchies = null;

		protected override void ReadInternal(Reader reader) {
			MapLoader l = MapLoader.Loader;

			off_channels = Pointer.Read(reader);
			off_mat = Pointer.Read(reader);
			off_vec = Pointer.Read(reader);
			off_hierarchies = Pointer.Read(reader);

			// Read channels
			Pointer.DoAt(ref reader, off_channels, () => {
				channels = new AnimChannelMontreal[anim.num_channels];
				for (uint i = 0; i < channels.Length; i++) {
					Pointer off_channel = Pointer.Read(reader);
					channels[i] = l.FromOffsetOrRead<AnimChannelMontreal>(reader, off_channel);
				}
			});

			// Read hierarchies
			Pointer.DoAt(ref reader, off_hierarchies, () => {
				uint num_hierarchies = reader.ReadUInt32();
				Pointer off_hierarchies2 = Pointer.Read(reader);
				hierarchies = l.ReadArray<AnimHierarchy>((int)num_hierarchies, reader, off_hierarchies2);
			});
		}
	}
}
