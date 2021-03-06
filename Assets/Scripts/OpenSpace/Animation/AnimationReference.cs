﻿using Newtonsoft.Json;
using OpenSpace.Animation.Component;
using OpenSpace.Animation.ComponentLargo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OpenSpace.Animation {
    public class AnimationReference : OpenSpaceStruct { // Also known as Anim3d
        public string name = null;
        public ushort num_onlyFrames;
        public byte speed;
        public byte num_channels;
        public Pointer off_events;
        public float x;
        public float y;
        public float z;
        public Pointer off_morphData;
        public ushort anim_index; // Index of animation within bank
        public byte num_events;
        public byte transition;
        public AnimMorphData[,] morphDataArray; // [channel][frame]
		
        public Pointer off_a3d = null;
        public AnimA3DGeneral a3d = null;
		public AnimA3DLargo a3dLargo = null;

		protected override void ReadInternal(Reader reader) {
			if (Settings.s.game == Settings.Game.R2Revolution) {
				off_a3d = Pointer.Read(reader);
				reader.ReadUInt32();
				off_events = Pointer.Read(reader);
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt32();
				num_onlyFrames = reader.ReadUInt16();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
				anim_index = reader.ReadUInt16();
				num_events = reader.ReadByte();
				transition = reader.ReadByte();
			} else {
				if (Settings.s.hasNames) name = new string(reader.ReadChars(0x50));
				if (Settings.s.engineVersion <= Settings.EngineVersion.TT) reader.ReadUInt32();
				if (Settings.s.game == Settings.Game.LargoWinch) off_a3d = Pointer.Read(reader);
				num_onlyFrames = reader.ReadUInt16();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
				off_events = Pointer.Read(reader);
				if (Settings.s.engineVersion < Settings.EngineVersion.R3) {
					x = reader.ReadSingle();
					y = reader.ReadSingle();
					z = reader.ReadSingle();
				}
				off_morphData = Pointer.Read(reader); // Runtime only?
				if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
				if (Settings.s.game != Settings.Game.LargoWinch) {
					anim_index = reader.ReadUInt16();
					num_events = reader.ReadByte();
					transition = reader.ReadByte();
				} else {
					num_events = reader.ReadByte();
					transition = reader.ReadByte();
					anim_index = reader.ReadUInt16();
				}

				if (Settings.s.engineVersion == Settings.EngineVersion.R2) reader.ReadUInt32(); // no idea what this is sadly
				if (Settings.s.engineVersion <= Settings.EngineVersion.TT) {
					off_a3d = Pointer.Read(reader);
				}
			}
			MapLoader l = MapLoader.Loader;
			if (Settings.s.game == Settings.Game.LargoWinch) {
				a3dLargo = l.FromOffsetOrRead<AnimA3DLargo>(reader, off_a3d, (a) => {
					a.num_onlyFrames = num_onlyFrames;
					a.num_channels = num_channels;
				});
			} else {
				Pointer.DoAt(ref reader, off_a3d, () => {
					a3d = l.FromOffsetOrRead<AnimA3DGeneral>(reader, off_a3d, onPreRead: a3d => a3d.readFull = true);
				});
			}
		}
	}
}
