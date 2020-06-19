using OpenSpace.FileFormat.Texture;
using OpenSpace.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OpenSpace.PS1 {
	public class ActorFileHeader : OpenSpaceStruct {
		public Pointer off_superObject;
		public byte[] copiedActorData;
		public Pointer off_states;
		public ushort num_states;
		public ushort ushort_22;

		public Pointer off_animPositions;
		public Pointer off_animRotations;
		public Pointer off_animScales;
		public Pointer off_geometricObjects_dynamic;
		public uint num_geometricObjects_dynamic;
		public ushort ushort_38;
		public ushort ushort_3A;
		public Pointer off_state_indices;


		// Parsed
		public ushort file_index;
		public ObjectsTable geometricObjectsDynamic;
		public PS1AnimationVector[] animPositions;
		public PS1AnimationQuaternion[] animRotations;
		public PS1AnimationVector[] animScales;
		public PointerList<State> states;

		protected override void ReadInternal(Reader reader) {
			R2PS1Loader l = Load as R2PS1Loader;
			if (Settings.s.game == Settings.Game.RRush) {
				off_superObject = Pointer.Read(reader);
			} else if (Settings.s.game == Settings.Game.JungleBook) {
				reader.ReadBytes(0x98);
			}
			copiedActorData = reader.ReadBytes(0x18);
			off_states = Pointer.Read(reader);
			num_states = reader.ReadUInt16();
			ushort_22 = reader.ReadUInt16();
			off_animPositions = Pointer.Read(reader); // 0x6 size
			off_animRotations = Pointer.Read(reader); // big array of structs of 0x8 size. 4 ushorts per struct
			off_animScales = Pointer.Read(reader);
			off_geometricObjects_dynamic = Pointer.Read(reader);
			num_geometricObjects_dynamic = reader.ReadUInt32();
			if (Settings.s.game == Settings.Game.RRush) {
				ushort_38 = reader.ReadUInt16();
				ushort_3A = reader.ReadUInt16();
				off_state_indices = Pointer.Read(reader);
			}
			// Parse
			states = Load.FromOffsetOrRead<PointerList<State>>(reader, off_states, s => s.length = num_states);
			geometricObjectsDynamic = Load.FromOffsetOrRead<ObjectsTable>(reader, off_geometricObjects_dynamic, onPreRead: t => t.length = num_geometricObjects_dynamic - 2);
			animPositions = Load.ReadArray<PS1AnimationVector>((off_animRotations.offset - off_animPositions.offset) / 6, reader, off_animPositions);
			animRotations = Load.ReadArray<PS1AnimationQuaternion>((off_animScales.offset - off_animRotations.offset) / 8, reader, off_animRotations);
			animScales = Load.ReadArray<PS1AnimationVector>(l.maxScaleVector[file_index] + 1, reader, off_animScales);
		}
	}
}
