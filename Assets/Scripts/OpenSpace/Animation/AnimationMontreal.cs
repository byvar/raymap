﻿using Newtonsoft.Json;
using OpenSpace.Animation.ComponentMontreal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationMontreal : OpenSpaceStruct {
        public LegacyPointer off_frames = null;
        public ushort num_frames;
        public byte speed;
        public byte num_channels;
        public byte unkbyte;
        public LegacyPointer off_unk;
        public Matrix speedMatrix;
        public AnimFrameMontreal[] frames;

		protected override void ReadInternal(Reader reader) {
			//MapLoader.Loader.print($"Anim: {Offset}");
			off_frames = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.game == Legacy_Settings.Game.R2Beta) {
				num_frames = reader.ReadUInt16();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
			} else {
				num_frames = reader.ReadByte();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
				unkbyte = reader.ReadByte();
			}
			off_unk = LegacyPointer.Read(reader);
			reader.ReadUInt32();
			reader.ReadUInt32();
			if(Legacy_Settings.s.game == Legacy_Settings.Game.R2Beta) reader.ReadUInt32();
			speedMatrix = Matrix.Read(reader, LegacyPointer.Current(reader));
			reader.ReadUInt32();
			reader.ReadUInt32();

			frames = MapLoader.Loader.ReadArray<AnimFrameMontreal>(num_frames, reader, off_frames, onPreRead: (afm) => {
				afm.anim = this;
			});
		}
	}
}
