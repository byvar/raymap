using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Declaration_ArrayEntry : U64_Struct {
		// Set in onPreSerialize
		public ushort Pre_Type { get; set; }

		// Struct members
		public int ValueInt { get; set; }
		public uint ValueUInt { get; set; }
		public float ValueFloat { get; set; }
		public U64_Reference<WAY_WayPoint> ValueWayPoint { get; set; }
		public U64_Reference<GAM_Character> ValuePerso { get; set; }
		public U64_Reference<AI_Declaration_Vector3D> ValueVector { get; set; }
		public U64_Index<U64_Placeholder> ValueText { get; set; }

		public ushort ArrayIndex { get; set; } // Index of variable (which is the array)
		public ushort EntryIndex { get; set; } // Index in array

		public override void SerializeImpl(SerializerObject s) {
			SerializeValue(s);
			ArrayIndex = s.Serialize<ushort>(ArrayIndex, name: nameof(ArrayIndex));
			EntryIndex = s.Serialize<ushort>(EntryIndex, name: nameof(EntryIndex));
		}

		private void SerializeValue(SerializerObject s) {
			void SerializeShort(Action a) {
				// Serialize padding for different endians, since this is actually an int.
				if (s.GetCPASettings().GetEndian == Endian.Big) s.SerializePadding(2, logIfNotNull: true);
				a();
				if (s.GetCPASettings().GetEndian == Endian.Little) s.SerializePadding(2, logIfNotNull: true);
			}


			// Serialize differently depending on type
			var aiTypes = Context.GetCPASettings().AITypes;
			var nodeType = aiTypes.GetDsgVarType(Pre_Type);
			switch (nodeType) {
				case AI_DsgVarType.Float:
					ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
					break;
				case AI_DsgVarType.Int:
					ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
					break;
				case AI_DsgVarType.UInt:
					ValueUInt = s.Serialize<uint>(ValueUInt, name: nameof(ValueUInt));
					break;
				case AI_DsgVarType.Perso:
					SerializeShort(() => {
						ValuePerso = s.SerializeObject<U64_Reference<GAM_Character>>(ValuePerso, name: nameof(ValuePerso))?.Resolve(s);
					});
					break;
				case AI_DsgVarType.WayPoint:
					SerializeShort(() => {
						ValueWayPoint = s.SerializeObject<U64_Reference<WAY_WayPoint>>(ValueWayPoint, name: nameof(ValueWayPoint))?.Resolve(s);
					});
					break;
				case AI_DsgVarType.Vector:
					SerializeShort(() => {
						ValueVector = s.SerializeObject<U64_Reference<AI_Declaration_Vector3D>>(ValueVector, name: nameof(ValueVector))?.Resolve(s);
					});
					break;
				case AI_DsgVarType.Text:
					SerializeShort(() => {
						ValueText = s.SerializeObject<U64_Index<U64_Placeholder>>(ValueText, name: nameof(ValueText));
					});
					break;

				default:
					throw new BinarySerializableException(this, $"Untreated DeclarationType {nodeType}:{Pre_Type} for ArrayEntry");
			}
		}
	}
}
