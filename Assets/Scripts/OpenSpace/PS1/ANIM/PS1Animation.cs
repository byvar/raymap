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
		public uint uint_10;
		public Pointer off_hierarchies;
		public uint num_hierarchies;

		// Parsed
		public PS1AnimationChannel[] channels;
		public PS1AnimationHierarchy[] hierarchies;
		public string name;

		protected override void ReadInternal(Reader reader) {
			speed = reader.ReadUInt32();
			off_channels = Pointer.Read(reader);
			num_channels = reader.ReadUInt32();
			num_frames = reader.ReadUInt16();
			ushort_0E = reader.ReadUInt16();
			uint_10 = reader.ReadUInt32();
			off_hierarchies = Pointer.Read(reader); // Points to uint_10 structs of 0x8 (2 uints. some kind of hierarchy?)
			num_hierarchies = reader.ReadUInt32();

			
			channels = Load.ReadArray<PS1AnimationChannel>(num_channels, reader, off_channels);
			hierarchies = Load.ReadArray<PS1AnimationHierarchy>(num_hierarchies, reader, off_hierarchies);
			//Load.print(channels.Max(c => c.frames.Length == 0 ? 0 : c.frames.Max(f => f.ntto >= 1 ? f.frameNumber.GetValueOrDefault(0) : 0)) + " - " + num_frames + " - " + num_channels);
			Pointer.DoAt(ref reader, off_hierarchies - 0x10, () => {
				name = reader.ReadString(0x10);
				//Load.print(Offset + " - " + name);
			});
			/*if (transitions != null) {
				name = transitions.name;
				Load.print(Offset + " - " + name);
			}*/
		}
	}
}
