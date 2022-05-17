using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Node : U64_Struct {
		public byte Type { get; set; }
		public byte Depth { get; set; }
		public ushort IdOrValue { get; set; }

		public U64_Reference<AI_Node_Long> ValueConstant { get; set; }
		public U64_Reference<AI_Node_Float> ValueFloat { get; set; }
		public U64_Reference<AI_Node_Vector3D> ValueVector { get; set; }
		public U64_Reference<AI_AIModel> ValueModelRef { get; set; }
		public U64_Reference<WAY_Graph> ValueGraph { get; set; }
		public U64_Reference<AI_Declaration_UnsignedLong> ValueCaps { get; set; }
		public U64_Reference<AI_Rule> ValueMacroRef { get; set; }
		public U64_Reference<GAM_Character> ValuePersoRef { get; set; }
		public U64_Reference<GAM_State> ValueActionRef { get; set; }
		public U64_Reference<WAY_WayPoint> ValueWayPointRef { get; set; }
		public U64_Reference<AI_Comport> ValueComportRef { get; set; }
		public U64_Reference<IPT_InputAction> ValueButton { get; set; }
		public U64_Reference<AI_Node_String> ValueString { get; set; }
		public U64_Reference<HIE_SuperObject> ValueSuperObject { get; set; }
		public U64_Reference<GAM_Family> ValueFamilyRef { get; set; }
		public U64_Reference<AI_Node_Long> ValueSoundEventRef { get; set; }
		public U64_Reference<GAM_ObjectsTable> ValueObjectTableRef { get; set; }
		public U64_Reference<GMT_GameMaterial> ValueGameMaterialRef { get; set; }
		public U64_Reference<GLI_Light> ValueLight { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Depth = s.Serialize<byte>(Depth, name: nameof(Depth));
			SerializeValue(s);
		}

		private void SerializeValue(SerializerObject s) {
			IdOrValue = s.Serialize<ushort>(IdOrValue, name: nameof(IdOrValue));
			s.DoAt(Offset + 2, () => {
				// Serialize differently depending on type
				var aiTypes = Context.GetCPASettings().AITypes;
				var nodeType = aiTypes.GetNodeType(Type);
				switch (nodeType) {
					case AI_InterpretType.Constant:
						ValueConstant = s.SerializeObject<U64_Reference<AI_Node_Long>>(ValueConstant, name: nameof(ValueConstant))?.Resolve(s);
						break;
					case AI_InterpretType.Real:
						ValueFloat = s.SerializeObject<U64_Reference<AI_Node_Float>>(ValueFloat, name: nameof(ValueFloat))?.Resolve(s);
						break;
					case AI_InterpretType.ConstantVector:
						ValueVector = s.SerializeObject<U64_Reference<AI_Node_Vector3D>>(ValueVector, name: nameof(ValueVector))?.Resolve(s);
						break;
					case AI_InterpretType.ModelRef:
						ValueModelRef = s.SerializeObject<U64_Reference<AI_AIModel>>(ValueModelRef, name: nameof(ValueModelRef))?.Resolve(s);
						break;
					case AI_InterpretType.Graph:
						ValueGraph = s.SerializeObject<U64_Reference<WAY_Graph>>(ValueGraph, name: nameof(ValueGraph))?.Resolve(s);
						break;
					case AI_InterpretType.Caps:
						ValueCaps = s.SerializeObject<U64_Reference<AI_Declaration_UnsignedLong>>(ValueCaps, name: nameof(ValueCaps))?.Resolve(s);
						break;
					case AI_InterpretType.MacroRef__Subroutine:
						ValueMacroRef = s.SerializeObject<U64_Reference<AI_Rule>>(ValueMacroRef, name: nameof(ValueMacroRef))?.Resolve(s);
						break;
					case AI_InterpretType.PersoRef:
						ValuePersoRef = s.SerializeObject<U64_Reference<GAM_Character>>(ValuePersoRef, name: nameof(ValuePersoRef))?.Resolve(s);
						break;
					case AI_InterpretType.ActionRef:
						ValueActionRef = s.SerializeObject<U64_Reference<GAM_State>>(ValueActionRef, name: nameof(ValueActionRef))?.Resolve(s);
						break;
					case AI_InterpretType.WayPointRef:
						ValueWayPointRef = s.SerializeObject<U64_Reference<WAY_WayPoint>>(ValueWayPointRef, name: nameof(ValueWayPointRef))?.Resolve(s);
						break;
					case AI_InterpretType.ComportRef:
						ValueComportRef = s.SerializeObject<U64_Reference<AI_Comport>>(ValueComportRef, name: nameof(ValueComportRef))?.Resolve(s);
						break;
					case AI_InterpretType.Button:
						ValueButton = s.SerializeObject<U64_Reference<IPT_InputAction>>(ValueButton, name: nameof(ValueButton))?.Resolve(s);
						break;
					case AI_InterpretType.String:
						ValueString = s.SerializeObject<U64_Reference<AI_Node_String>>(ValueString, name: nameof(ValueString))?.Resolve(s);
						break;
					case AI_InterpretType.SuperObjectRef:
						ValueSuperObject = s.SerializeObject<U64_Reference<HIE_SuperObject>>(ValueSuperObject, name: nameof(ValueSuperObject))?.Resolve(s);
						break;
					case AI_InterpretType.FamilyRef:
						ValueFamilyRef = s.SerializeObject<U64_Reference<GAM_Family>>(ValueFamilyRef, name: nameof(ValueFamilyRef))?.Resolve(s);
						break;
					case AI_InterpretType.SoundEventRef:
						ValueSoundEventRef = s.SerializeObject<U64_Reference<AI_Node_Long>>(ValueSoundEventRef, name: nameof(ValueSoundEventRef))?.Resolve(s);
						break;
					case AI_InterpretType.ObjectTableRef:
						ValueObjectTableRef = s.SerializeObject<U64_Reference<GAM_ObjectsTable>>(ValueObjectTableRef, name: nameof(ValueObjectTableRef))?.Resolve(s);
						break;
					case AI_InterpretType.GameMaterialRef:
						ValueGameMaterialRef = s.SerializeObject<U64_Reference<GMT_GameMaterial>>(ValueGameMaterialRef, name: nameof(ValueGameMaterialRef))?.Resolve(s);
						break;
					case AI_InterpretType.Light:
						ValueLight = s.SerializeObject<U64_Reference<GLI_Light>>(ValueLight, name: nameof(ValueLight))?.Resolve(s);
						break;
					default:
						break;
				}
			});
		}

		public override bool UseShortLog => true;
		public override string ShortLog => ToString();

		public override string ToString() {
			var aiTypes = Context.GetCPASettings().AITypes;
			var nodeType = aiTypes.GetNodeType(Type);
			string translatedValue = nodeType switch {
				AI_InterpretType.KeyWord => aiTypes.GetKeyword(IdOrValue)?.ToString(),
				AI_InterpretType.Procedure => aiTypes.GetProcedure(IdOrValue)?.ToString(),
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
