using OpenSpace.Animation.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation.ComponentMontreal {
    public class AnimFrameMontreal {
        public Pointer offset;
        public AnimationMontreal anim;
        public Pointer off_channels;
        public Pointer off_mat;
        public Pointer off_vec;
        public Pointer off_hierarchies;

        public AnimChannelMontreal[] channels = null;
        public AnimHierarchy[] hierarchies = null;

        public AnimFrameMontreal(Pointer offset, AnimationMontreal anim) {
            this.offset = offset;
            this.anim = anim;
        }

        public static AnimFrameMontreal Read(Reader reader, Pointer offset, AnimationMontreal anim) {
            MapLoader l = MapLoader.Loader;
            AnimFrameMontreal f = new AnimFrameMontreal(offset, anim);
            f.off_channels = Pointer.Read(reader);
            f.off_mat = Pointer.Read(reader);
            f.off_vec = Pointer.Read(reader);
            f.off_hierarchies = Pointer.Read(reader);

            // Read channels
            Pointer.DoAt(ref reader, f.off_channels, () => {
                f.channels = new AnimChannelMontreal[anim.num_channels];
                for (uint i = 0; i < f.channels.Length; i++) {
                    Pointer off_channel = Pointer.Read(reader);
                    Pointer.DoAt(ref reader, off_channel, () => {
                        f.channels[i] = AnimChannelMontreal.Read(reader, off_channel);
                    });
                }
            });


            // Read hierarchies
            Pointer.DoAt(ref reader, f.off_hierarchies, () => {
                uint num_hierarchies = reader.ReadUInt32();
                f.hierarchies = new AnimHierarchy[num_hierarchies];
                Pointer off_hierarchies2 = Pointer.Read(reader);
                Pointer.DoAt(ref reader, off_hierarchies2, () => {
                    for (uint i = 0; i < num_hierarchies; i++) {
                        f.hierarchies[i] = AnimHierarchy.Read(reader);
                    }
                });
            });
            return f;
        }

    }
}
