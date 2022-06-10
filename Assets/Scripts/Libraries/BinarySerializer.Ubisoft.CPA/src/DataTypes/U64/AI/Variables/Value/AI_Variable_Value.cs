using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Variable_Value : U64_Struct {
		public bool IsArrayEntry { get; set; }
		public ushort Type { get; set; }

		// Value types
		public U64_Reference<AI_Declaration_ArrayEntry> ValueArrayEntry { get; set; }
		public bool ValueBoolean { get; set; }
		public sbyte ValueSByte { get; set; }
		public byte ValueUByte { get; set; }
		public short ValueShort { get; set; }
		public ushort ValueUShort { get; set; }
		public byte ValueListMaxSize { get; set; }
		public U64_Reference<AI_Declaration_Long> ValueInt { get; set; }
		public U64_Reference<AI_Declaration_UnsignedLong> ValueUInt { get; set; }
		public U64_Reference<AI_Declaration_Float> ValueFloat { get; set; }
		public U64_Reference<AI_Declaration_Vector3D> ValueVector { get; set; }
		public U64_Reference<WAY_WayPoint> ValueWayPoint { get; set; }
		public U64_Reference<GAM_Character> ValuePerso { get; set; }
		public U64_Index<U64_Placeholder> ValueText { get; set; }
		public U64_Reference<WAY_Graph> ValueGraph { get; set; }
		public U64_Reference<AI_Comport> ValueComport { get; set; }
		public U64_Reference<GAM_State> ValueAction { get; set; }
		public U64_Reference<AI_Declaration_UnsignedLong> ValueCaps { get; set; }
		public ushort ValueArraySize { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoBits<ushort>(b => {
				Type = b.SerializeBits<ushort>(Type, 15, name: nameof(Type));
				IsArrayEntry = b.SerializeBits<bool>(IsArrayEntry, 1, name: nameof(IsArrayEntry));
			});
			SerializeValue(s);
		}

		private void SerializeValue(SerializerObject s) {
			// Serialize differently depending on type
			var aiTypes = Context.GetCPASettings().AITypes;
			var nodeType = aiTypes.GetDsgVarType(Type);
			if (IsArrayEntry) {
				ValueArrayEntry = s.SerializeObject<U64_Reference<AI_Declaration_ArrayEntry>>(ValueArrayEntry, name: nameof(ValueArrayEntry))
					?.Resolve(s, onPreSerialize: (_,a) => a.Pre_Type = Type);
			} else {
				switch (nodeType) {
					case AI_DsgVarType.Boolean:
						s.DoBits<ushort>(b => {
							ValueBoolean = b.SerializeBits<bool>(ValueBoolean, 1, name: nameof(ValueBoolean));
							b.SerializePadding(15, logIfNotNull: true);
						});
						break;
					case AI_DsgVarType.SByte:
						s.DoBits<ushort>(b => {
							ValueSByte = b.SerializeBits<sbyte>(ValueSByte, 8, SignedNumberRepresentation.TwosComplement, name: nameof(ValueSByte));
							b.SerializePadding(8, logIfNotNull: false); // Don't log. Can be 0xFF because the value was expanded to a short
						});
						break;
					case AI_DsgVarType.UByte:
						s.DoBits<ushort>(b => {
							ValueUByte = b.SerializeBits<byte>(ValueUByte, 8, name: nameof(ValueUByte));
							b.SerializePadding(8, logIfNotNull: true);
						});
						break;
					case AI_DsgVarType.Short:
						ValueShort = s.Serialize<short>(ValueShort, name: nameof(ValueShort));
						break;
					case AI_DsgVarType.UShort:
						ValueUShort = s.Serialize<ushort>(ValueUShort, name: nameof(ValueUShort));
						break;
					case AI_DsgVarType.Int:
						ValueInt = s.SerializeObject<U64_Reference<AI_Declaration_Long>>(ValueInt, name: nameof(ValueInt))?.Resolve(s);
						break;
					case AI_DsgVarType.UInt:
						ValueUInt = s.SerializeObject<U64_Reference<AI_Declaration_UnsignedLong>>(ValueUInt, name: nameof(ValueUInt))?.Resolve(s);
						break;
					case AI_DsgVarType.Float:
						ValueFloat = s.SerializeObject<U64_Reference<AI_Declaration_Float>>(ValueFloat, name: nameof(ValueFloat))?.Resolve(s);
						break;
					case AI_DsgVarType.Vector:
						ValueVector = s.SerializeObject<U64_Reference<AI_Declaration_Vector3D>>(ValueVector, name: nameof(ValueVector))?.Resolve(s);
						break;
					case AI_DsgVarType.Perso:
						ValuePerso = s.SerializeObject<U64_Reference<GAM_Character>>(ValuePerso, name: nameof(ValuePerso))?.Resolve(s);
						break;
					case AI_DsgVarType.WayPoint:
						ValueWayPoint = s.SerializeObject<U64_Reference<WAY_WayPoint>>(ValueWayPoint, name: nameof(ValueWayPoint))?.Resolve(s);
						break;
					case AI_DsgVarType.Text:
						ValueText = s.SerializeObject<U64_Index<U64_Placeholder>>(ValueText, name: nameof(ValueText));
						break;
					case AI_DsgVarType.Comport:
						ValueComport = s.SerializeObject<U64_Reference<AI_Comport>>(ValueComport, name: nameof(ValueComport))?.Resolve(s);
						break;
					case AI_DsgVarType.Graph:
						ValueGraph = s.SerializeObject<U64_Reference<WAY_Graph>>(ValueGraph, name: nameof(ValueGraph))?.Resolve(s);
						break;
					case AI_DsgVarType.Action:
						ValueAction = s.SerializeObject<U64_Reference<GAM_State>>(ValueAction, name: nameof(ValueAction))?.Resolve(s);
						break;
					case AI_DsgVarType.Caps:
						ValueCaps = s.SerializeObject<U64_Reference<AI_Declaration_UnsignedLong>>(ValueCaps, name: nameof(ValueCaps))?.Resolve(s);
						break;
					case AI_DsgVarType.List:
						s.DoBits<ushort>(b => {
							ValueListMaxSize = b.SerializeBits<byte>(ValueListMaxSize, 8, name: nameof(ValueListMaxSize));
							b.SerializePadding(8, logIfNotNull: true);
						});
						break;
					case AI_DsgVarType.IntegerArray:
					case AI_DsgVarType.FloatArray:
					case AI_DsgVarType.PersoArray:
					case AI_DsgVarType.WayPointArray:
					case AI_DsgVarType.VectorArray:
					case AI_DsgVarType.TextArray:
						ValueArraySize = s.Serialize<ushort>(ValueArraySize, name: nameof(ValueArraySize));
						break;
					default:
						throw new BinarySerializableException(this, $"Unimplemented dsgvar type {nodeType} ({Type})");
				}
			}
		}
	}
}
