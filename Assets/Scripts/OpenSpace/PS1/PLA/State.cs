using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollideType = OpenSpace.Collide.CollideType;

namespace OpenSpace.PS1 {
	public class State : OpenSpaceStruct { // Animation/state related
		public LegacyPointer off_anim;
		public Dictionary<CollideType, short> zoneZdx = new Dictionary<CollideType, short>();
		public LegacyPointer off_transitions;
		public uint num_transitions;
		public LegacyPointer off_state_auto;
		public LegacyPointer off_18;
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
			off_anim = LegacyPointer.Read(reader);
			zoneZdx[CollideType.ZDM] = reader.ReadInt16();
			zoneZdx[CollideType.ZDE] = reader.ReadInt16();
			zoneZdx[CollideType.ZDD] = reader.ReadInt16();
			zoneZdx[CollideType.ZDR] = reader.ReadInt16();
			off_transitions = LegacyPointer.Read(reader); // Points to animation data, incl name
			num_transitions = reader.ReadUInt32();
			off_state_auto = LegacyPointer.Read(reader);
			if (Legacy_Settings.s.game != Legacy_Settings.Game.RRush) {
				off_18 = LegacyPointer.Read(reader);
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
			//Load.print("State " + anim?.index + " - " + Offset + ": " + off_anim + " - " + off_18);
		}
	}
}
