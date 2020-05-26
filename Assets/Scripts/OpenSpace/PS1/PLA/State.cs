using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.PS1 {
	public class State : OpenSpaceStruct { // Animation/state related
		public Pointer off_anim;
		public ushort ushort_04;
		public ushort ushort_06;
		public int int_08;
		public Pointer off_transitions;
		public uint num_transitions;
		public Pointer off_state_auto;
		public Pointer off_18;
		public byte byte_1C;
		public byte speed;
		public ushort ushort_1E;

		// Parsed
		public PS1AnimationIndex anim;
		public State state_auto;
		public StateTransition[] transitions;
		public string name;

		protected override void ReadInternal(Reader reader) {
			//Load.print("State @ " + Offset);
			off_anim = Pointer.Read(reader);
			ushort_04 = reader.ReadUInt16();
			ushort_06 = reader.ReadUInt16();
			int_08 = reader.ReadInt32();
			off_transitions = Pointer.Read(reader); // Points to animation data, incl name
			num_transitions = reader.ReadUInt32();
			off_state_auto = Pointer.Read(reader);
			if (Settings.s.game != Settings.Game.RRush) {
				off_18 = Pointer.Read(reader);
			}
			byte_1C = reader.ReadByte();
			speed = reader.ReadByte(); // Usually 30, but can also be 20, 40, 60, 35
			ushort_1E = reader.ReadUInt16();

			anim = Load.FromOffsetOrRead<PS1AnimationIndex>(reader, off_anim);
			transitions = Load.ReadArray<StateTransition>(num_transitions, reader, off_transitions);
			state_auto = Load.FromOffsetOrRead<State>(reader, off_state_auto);
			/*if (transitions != null) {
				name = transitions.name;
				Load.print(Offset + " - " + name);
			}*/
			Load.print("State " + anim?.index + " - " + Offset + ": " + off_anim + " - " + off_18);
		}
	}
}
