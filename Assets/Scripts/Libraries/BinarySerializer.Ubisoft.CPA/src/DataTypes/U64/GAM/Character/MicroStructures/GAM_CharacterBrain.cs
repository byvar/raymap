using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterBrain : U64_Struct {
		public U64_Reference<AI_AIModel> AIModel { get; set; }
		public U64_Reference<AI_Comport> InitComportIntelligence { get; set; }
		public U64_Reference<AI_Comport> InitComportReflex { get; set; }
		public U64_Reference<U64_Placeholder> InitVariables { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			AIModel = s.SerializeObject<U64_Reference<AI_AIModel>>(AIModel, name: nameof(AIModel))?.Resolve(s);
			InitComportIntelligence = s.SerializeObject<U64_Reference<AI_Comport>>(InitComportIntelligence, name: nameof(InitComportIntelligence))?.Resolve(s);
			InitComportReflex = s.SerializeObject<U64_Reference<AI_Comport>>(InitComportReflex, name: nameof(InitComportReflex))?.Resolve(s);
			InitVariables = s.SerializeObject<U64_Reference<U64_Placeholder>>(InitVariables, name: nameof(InitVariables));
		}
	}
}
