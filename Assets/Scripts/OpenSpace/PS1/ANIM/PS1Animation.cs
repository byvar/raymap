using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Animation : OpenSpaceStruct { // Animation/state related
		public uint speed;
		public Pointer off_channels;
		public uint num_channels;
		public ushort num_frames;
		public ushort ushort_0E;
		public uint num_hierarchies;
		public Pointer off_hierarchies;
		public uint uint_18;

		// Non R2
		public uint flags;
		public ushort file_index;
		public ushort ushort_18;
		public ushort ushort_1A;
		public Pointer off_1C;

		// Parsed
		public PS1AnimationChannel[] channels;
		public PS1AnimationHierarchy[] hierarchies;
		public string name;

		protected override void ReadInternal(Reader reader) {
			//Load.print("Anim " + Pointer.Current(reader));
			if (Settings.s.game == Settings.Game.R2) {
				speed = reader.ReadUInt32();
				off_channels = Pointer.Read(reader);
				num_channels = reader.ReadUInt32();
				num_frames = reader.ReadUInt16();
				ushort_0E = reader.ReadUInt16();
				num_hierarchies = reader.ReadUInt32();
				off_hierarchies = Pointer.Read(reader); // Points to uint_10 structs of 0x8 (2 uints. some kind of hierarchy?)
				uint_18 = reader.ReadUInt32();
			} else if(Settings.s.game == Settings.Game.RRush) {
				off_channels = Pointer.Read(reader);
				off_hierarchies = Pointer.Read(reader);
				num_hierarchies = reader.ReadUInt16();
				file_index = reader.ReadUInt16();
				speed = reader.ReadByte();
				num_channels = reader.ReadByte();
				num_frames = reader.ReadUInt16();
			} else {
				flags = reader.ReadUInt32();
				off_channels = Pointer.Read(reader);
				num_channels = reader.ReadUInt32();
				num_frames = reader.ReadUInt16();
				ushort_0E = reader.ReadUInt16();
				num_hierarchies = reader.ReadUInt32();
				if (Settings.s.game == Settings.Game.VIP) {
					off_hierarchies = Pointer.Read(reader); // Points to uint_10 structs of 0x8 (2 uints. some kind of hierarchy?)
					ushort_18 = reader.ReadUInt16();
					ushort_1A = reader.ReadUInt16();
					off_1C = Pointer.Read(reader);
					uint_18 = reader.ReadUInt32();
					speed = 30;
				} else {
					speed = reader.ReadUInt16();
					ushort_1A = reader.ReadUInt16();
					off_hierarchies = Pointer.Read(reader); // Points to uint_10 structs of 0x8 (2 uints. some kind of hierarchy?)
					uint_18 = reader.ReadUInt32();
					uint_18 = reader.ReadUInt32();
					uint_18 = reader.ReadUInt32();
				}
			}

			channels = Load.ReadArray<PS1AnimationChannel>(num_channels, reader, off_channels);
			hierarchies = Load.ReadArray<PS1AnimationHierarchy>(num_hierarchies, reader, off_hierarchies);
			//Load.print(channels.Max(c => c.frames.Length == 0 ? 0 : c.frames.Max(f => f.ntto >= 1 ? f.frameNumber.GetValueOrDefault(0) : 0)) + " - " + num_frames + " - " + num_channels);
			Pointer.DoAt(ref reader, off_hierarchies - 0x10, () => {
				name = reader.ReadString(0x10);
				//Load.print(Offset + " - " + name);
			});


			R2PS1Loader l = Load as R2PS1Loader;
			foreach (PS1AnimationChannel c in channels) {
				foreach (PS1AnimationKeyframe f in c.frames) {
					if (f.scl.HasValue) {
						if (f.scl.Value > l.maxScaleVector[file_index]) l.maxScaleVector[file_index] = f.scl.Value;
					}
				}
			}
			/*if (transitions != null) {
				name = transitions.name;
				Load.print(Offset + " - " + name);
			}*/
		}
	}
}
