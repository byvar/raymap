namespace BinarySerializer.Ubisoft.CPA {
	public class AI_DsgVarValueArray : BinarySerializable {
		public AI_DsgVarType Pre_LinkedType { get; set; }

		public uint UnknownRevolution { get; set; }
		public uint ElementType { get; set; }
		public byte MaxElementsCount { get; set; }

		public AI_DsgVarValue[] Values { get; set; }

		public AI_DsgVarType LinkedElementType => Context.GetCPASettings().AITypes.GetDsgVarType(ElementType);

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman2Revolution) {
				UnknownRevolution = s.Serialize<uint>(UnknownRevolution, name: nameof(UnknownRevolution));
				ElementType = s.Serialize<byte>((byte)ElementType, name: nameof(ElementType));
				MaxElementsCount = s.Serialize<byte>(MaxElementsCount, name: nameof(MaxElementsCount));
			} else {
				ElementType = s.Serialize<uint>(ElementType, name: nameof(ElementType));
				MaxElementsCount = s.Serialize<byte>(MaxElementsCount, name: nameof(MaxElementsCount));
			}
			s.Align(4, Offset);
			
			CheckLinkedElementType(s);

			Values = s.SerializeObjectArray<AI_DsgVarValue>(Values, MaxElementsCount, onPreSerialize: (e,i) => {
				e.Pre_IsArrayEntry = true;
				e.Pre_LinkedType = LinkedElementType;
			}, name: nameof(Values));
		}

		public void CheckLinkedElementType(SerializerObject s) {
			AI_DsgVarType expectedLinkedElementType = Pre_LinkedType switch {
				AI_DsgVarType.PersoArray => AI_DsgVarType.Perso,
				AI_DsgVarType.VectorArray => AI_DsgVarType.Vector,
				AI_DsgVarType.FloatArray => AI_DsgVarType.Float,
				AI_DsgVarType.IntegerArray => AI_DsgVarType.Int,
				AI_DsgVarType.WayPointArray => AI_DsgVarType.WayPoint,
				AI_DsgVarType.TextArray => AI_DsgVarType.Text,
				AI_DsgVarType.GraphArray => AI_DsgVarType.Graph,
				AI_DsgVarType.SuperObjectArray => AI_DsgVarType.SuperObject,
				AI_DsgVarType.SOLinksArray => AI_DsgVarType.SOLinks,
				AI_DsgVarType.SoundEventArray => AI_DsgVarType.SoundEvent,
				AI_DsgVarType.VisualMatArray => AI_DsgVarType.VisualMaterial,
				//AI_DsgVarType.TextRefArray => AI_DsgVarType.Text,
				AI_DsgVarType.ActionArray => AI_DsgVarType.Action,
				_ => AI_DsgVarType.None
			};
			if (expectedLinkedElementType == AI_DsgVarType.None || expectedLinkedElementType != LinkedElementType) {
				s?.SystemLogger?.LogWarning($"{Offset}: Unexpected linked element type for DsgVar array with type {Pre_LinkedType}: {LinkedElementType}");
			}
		}
	}
}
