using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Comport : U64_Struct {
		public U64_Reference<AI_Rule> Schedule { get; set; }
		public LST_ReferenceList<AI_Rule> Rules { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Schedule = s.SerializeObject<U64_Reference<AI_Rule>>(Schedule, name: nameof(Schedule))?.Resolve(s);
			Rules = s.SerializeObject<LST_ReferenceList<AI_Rule>>(Rules, name: nameof(Rules))?.Resolve(s);
		}
	}
}
