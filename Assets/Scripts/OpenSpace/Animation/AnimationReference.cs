using Newtonsoft.Json;
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
        public LegacyPointer off_events;
        public float x;
        public float y;
        public float z;
        public LegacyPointer off_morphData;
        public ushort anim_index; // Index of animation within bank
        public byte num_events;
        public byte transition;
        public AnimMorphData[,] morphDataArray; // [channel][frame]
		
        public LegacyPointer off_a3d = null;
        public AnimA3DGeneral a3d = null;
		public AnimA3DLargo a3dLargo = null;

		protected override void ReadInternal(Reader reader) {
			if (CPA_Settings.s.game == CPA_Settings.Game.R2Revolution) {
				off_a3d = LegacyPointer.Read(reader);
				reader.ReadUInt32();
				off_events = LegacyPointer.Read(reader);
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
				if (CPA_Settings.s.hasNames) name = new string(reader.ReadChars(0x50));
				if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.TT) reader.ReadUInt32();
				if (CPA_Settings.s.game == CPA_Settings.Game.LargoWinch) off_a3d = LegacyPointer.Read(reader);
				num_onlyFrames = reader.ReadUInt16();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
				off_events = LegacyPointer.Read(reader);
				if (CPA_Settings.s.engineVersion < CPA_Settings.EngineVersion.R3) {
					x = reader.ReadSingle();
					y = reader.ReadSingle();
					z = reader.ReadSingle();
				}
				off_morphData = LegacyPointer.Read(reader); // Runtime only?
				if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.TT) {
					reader.ReadUInt32();
					reader.ReadUInt32();
				}
				if (CPA_Settings.s.game != CPA_Settings.Game.LargoWinch) {
					anim_index = reader.ReadUInt16();
					num_events = reader.ReadByte();
					transition = reader.ReadByte();
				} else {
					num_events = reader.ReadByte();
					transition = reader.ReadByte();
					anim_index = reader.ReadUInt16();
				}

				if (CPA_Settings.s.engineVersion == CPA_Settings.EngineVersion.R2) reader.ReadUInt32(); // no idea what this is sadly
				if (CPA_Settings.s.engineVersion <= CPA_Settings.EngineVersion.TT) {
					off_a3d = LegacyPointer.Read(reader);
				}
			}
			MapLoader l = MapLoader.Loader;
			if (CPA_Settings.s.game == CPA_Settings.Game.LargoWinch) {
				a3dLargo = l.FromOffsetOrRead<AnimA3DLargo>(reader, off_a3d, (a) => {
					a.num_onlyFrames = num_onlyFrames;
					a.num_channels = num_channels;
				});
			} else {
				LegacyPointer.DoAt(ref reader, off_a3d, () => {
					a3d = l.FromOffsetOrRead<AnimA3DGeneral>(reader, off_a3d, onPreRead: a3d => a3d.readFull = true);
				});
			}
		}
	}
}
