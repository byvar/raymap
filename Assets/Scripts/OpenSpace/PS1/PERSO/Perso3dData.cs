using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class Perso3dData : OpenSpaceStruct { // Animation/state related
		public uint flags;
		public LegacyPointer off_family;
		public LegacyPointer off_58;
		public ushort ushort_5C;
		public short short_5E;
		public LegacyPointer off_animationBuffer;
		public LegacyPointer off_64;
		public LegacyPointer off_68;
		public LegacyPointer off_6C;
		public int num_collisionObjects;
		public LegacyPointer off_collisionObjects; public uint uint_74;
		public uint stateIndex;
		public LegacyPointer off_currentState;

		// Parsed
		public Family family;
		public PhysicalObjectCollisionMapping[] collisionMapping;


		protected override void ReadInternal(Reader reader) {
			//Load.print(Offset);
			flags = reader.ReadUInt32();
			off_family = LegacyPointer.Read(reader);
			reader.ReadBytes(0x50); // TODO
			off_58 = LegacyPointer.Read(reader); // 3 structs of 0x8 (no pointers)
			ushort_5C = reader.ReadUInt16();
			short_5E = reader.ReadInt16();
			off_animationBuffer = LegacyPointer.Read(reader);
			if (ushort_5C > 0) {
				off_64 = LegacyPointer.Read(reader); // short_5e * 0x4
				off_68 = LegacyPointer.Read(reader); // short_5e * 0x2
			} else {
				reader.ReadUInt32();
				reader.ReadUInt32();
			}
			off_6C = LegacyPointer.Read(reader); // same as col
			num_collisionObjects = reader.ReadInt32();
			if (num_collisionObjects > 0) {
				off_collisionObjects = LegacyPointer.Read(reader); // mapping: off_collisionObject, off_poListEntry
				collisionMapping = Load.ReadArray<PhysicalObjectCollisionMapping>(num_collisionObjects, reader, off_collisionObjects);
			} else {
				uint_74 = reader.ReadUInt32();
			}
			stateIndex = reader.ReadUInt32();
			off_currentState = LegacyPointer.Read(reader); // only at runtime
			reader.ReadBytes(0x24); // TODO

			// at 0x92: scale shorts

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
			(Load as Loader.R2PS1Loader).familyStates[GetFileIndex()].Add(new Tuple<Perso3dData, uint>(this, stateIndex));
		}


		[Flags]
		public enum Perso3dDataFlags : uint {
			None = 0,
			Actor1 = 0x8000,
			Actor2 = 0x10000
		}

		[Flags]
		public enum Perso3dDataJBFlags : uint {
			None = 0,
			Actor1 = 0x4000,
			Actor2 = 0x8000
		}

		public bool HasFlag(Perso3dDataFlags flags) {
			return (this.flags & (uint)flags) == (uint)flags;
		}

		public bool HasFlag(Perso3dDataJBFlags flags) {
			return (this.flags & (uint)flags) == (uint)flags;
		}

		public uint GetFileIndex() {
			if (CPA_Settings.s.game == CPA_Settings.Game.RRush) {
				if (HasFlag(Perso3dDataFlags.Actor1)) {
					return 1;
				} else if (HasFlag(Perso3dDataFlags.Actor2)) {
					return 2;
				}
			} else if (CPA_Settings.s.game == CPA_Settings.Game.JungleBook) {
				if (HasFlag(Perso3dDataJBFlags.Actor1)) {
					return 1;
				} else if (HasFlag(Perso3dDataJBFlags.Actor2)) {
					return 2;
				}
			}
			return 0;
		}
	}
}
