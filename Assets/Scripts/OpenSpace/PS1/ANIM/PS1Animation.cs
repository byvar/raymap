using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class PS1Animation : OpenSpaceStruct { // Animation/state related
		public uint speed;
		public Pointer off_anim;
		public uint num_frames;
		public ushort num_channels;
		public ushort ushort_0E;
		public uint uint_10;
		public Pointer off_14;
		public uint num_18;

		// Parsed
		public PS1AnimationFrame[] frames;
		public string name;

		protected override void ReadInternal(Reader reader) {
			speed = reader.ReadUInt32();
			off_anim = Pointer.Read(reader);
			num_frames = reader.ReadUInt32();
			num_channels = reader.ReadUInt16();
			ushort_0E = reader.ReadUInt16();
			uint_10 = reader.ReadUInt32();
			off_14 = Pointer.Read(reader); // Points to uint_10 structs of 0x8 (2 uints. some kind of hierarchy?)
			num_18 = reader.ReadUInt32();

			
			frames = Load.ReadArray<PS1AnimationFrame>(num_frames, reader, off_anim);
			Pointer.DoAt(ref reader, off_14 - 0x10, () => {
				name = reader.ReadString(0x10);
				Load.print(name);
			});
			/*if (transitions != null) {
				name = transitions.name;
				Load.print(Offset + " - " + name);
			}*/
		}
	}
}
