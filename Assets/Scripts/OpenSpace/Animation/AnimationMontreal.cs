using Newtonsoft.Json;
using OpenSpace.Animation.ComponentMontreal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationMontreal : OpenSpaceStruct {
        public Pointer off_frames = null;
        public byte num_frames;
        public byte speed;
        public byte num_channels;
        public byte unkbyte;
        public Pointer off_unk;
        public Matrix speedMatrix;
        public AnimFrameMontreal[] frames;

		protected override void ReadInternal(Reader reader) {
			off_frames = Pointer.Read(reader);
			num_frames = reader.ReadByte();
			speed = reader.ReadByte();
			num_channels = reader.ReadByte();
			unkbyte = reader.ReadByte();
			off_unk = Pointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			speedMatrix = Matrix.Read(reader, Pointer.Current(reader));
			reader.ReadUInt32();
			reader.ReadUInt32();

			frames = MapLoader.Loader.ReadArray<AnimFrameMontreal>(num_frames, reader, off_frames, onPreRead: (afm) => {
				afm.anim = this;
			});
		}
	}
}
