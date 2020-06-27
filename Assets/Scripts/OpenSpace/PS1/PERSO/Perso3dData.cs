using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Perso3dData : OpenSpaceStruct { // Animation/state related
		public uint flags;
		public Pointer off_family;
		public Pointer off_58;
		public ushort ushort_5C;
		public short short_5E;
		public Pointer off_animationBuffer;
		public Pointer off_64;
		public Pointer off_68;
		public Pointer off_6C;
		public int num_collisionObjects;
		public uint uint_74;
		public uint stateIndex;

		// Rush
		public Pointer off_collisionObjects;

		// Parsed
		public Family family;


		protected override void ReadInternal(Reader reader) {
			//Load.print(Offset);
			flags = reader.ReadUInt32();
			off_family = Pointer.Read(reader);
			if (Settings.s.game == Settings.Game.R2 || Settings.s.game == Settings.Game.RRush) {
				reader.ReadBytes(0x50); // TODO
				off_58 = Pointer.Read(reader);
				ushort_5C = reader.ReadUInt16();
				short_5E = reader.ReadInt16();
				off_animationBuffer = Pointer.Read(reader);
				off_64 = Pointer.Read(reader);
				off_68 = Pointer.Read(reader);
				off_6C = Pointer.Read(reader); // same as col
				num_collisionObjects = reader.ReadInt32();
				if (num_collisionObjects > 0) {
					off_collisionObjects = Pointer.Read(reader); // mapping: off_collisionObject, off_poListEntry
				} else {
					uint_74 = reader.ReadUInt32();
				}
				stateIndex = reader.ReadUInt32();
				reader.ReadBytes(0x28); // TODO
			}
			//Load.print(off_family + " - " + off_animation);
			/*Pointer.DoAt(ref reader, off_68, () => {
				for (int i = 0; i < ushort_5C; i++) {
					reader.ReadUInt16();
				}
				reader.ReadByte();
				byte unk = reader.ReadByte();
				if (unk != 0xFF) {
					string familyName = reader.ReadString(0x20);
					Load.print(familyName);
				}
			});*/

			family = Load.FromOffsetOrRead<Family>(reader, off_family);
		}


		[Flags]
		public enum Perso3dDataFlags : uint {
			None = 0,
			Actor1 = 0x8000,
			Actor2 = 0x10000
		}

		public bool HasFlag(Perso3dDataFlags flags) {
			return (this.flags & (uint)flags) == (uint)flags;
		}
	}
}
