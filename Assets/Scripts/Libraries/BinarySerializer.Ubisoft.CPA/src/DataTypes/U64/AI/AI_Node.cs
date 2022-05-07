using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node : U64_Struct {
		public byte Type { get; set; }
		public byte Depth { get; set; }
		public ushort IdOrValue { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Depth = s.Serialize<byte>(Depth, name: nameof(Depth));
			SerializeValue(s);
		}

		private void SerializeValue(SerializerObject s) {
			// TODO: Serialize differently depending on type
			IdOrValue = s.Serialize<ushort>(IdOrValue, name: nameof(IdOrValue));
		}

		public override bool UseShortLog => true;
		public override string ShortLog => ToString();

		public override string ToString() {
			var aiTypes = Context.GetCPASettings().AITypes;
			var nodeType = aiTypes.GetNodeType(Type);
			string translatedValue = nodeType switch {
				AI_InterpretType.KeyWord => aiTypes.GetKeyword(IdOrValue)?.ToString(),
				AI_InterpretType.Procedure => aiTypes.GetProcedure(IdOrValue),
				AI_InterpretType.Function => aiTypes.GetFunction(IdOrValue)?.ToString(),
				AI_InterpretType.Field => aiTypes.GetField(IdOrValue)?.ToString(),
				AI_InterpretType.Operator => aiTypes.GetOperator(IdOrValue)?.ToString(),
				AI_InterpretType.MetaAction => aiTypes.GetMetaAction(IdOrValue)?.ToString(),
				AI_InterpretType.Condition => aiTypes.GetCondition(IdOrValue)?.ToString(),
				_ => $"{nodeType}_{IdOrValue:X4}"
			};
			return $"({Type:X2},{Depth:X2},{IdOrValue:X4}){new string(' ',4*Depth)}{translatedValue}";
		}
	}
}
