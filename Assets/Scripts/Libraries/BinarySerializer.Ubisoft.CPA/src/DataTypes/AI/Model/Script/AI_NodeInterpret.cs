namespace BinarySerializer.Ubisoft.CPA {
	public class AI_NodeInterpret : BinarySerializable {
		public uint Param { get; set; }
		public ushort NodesToSkip { get; set; }
		public byte Depth { get; set; }
		public byte Type { get; set; }

		public uint R2DCUnknown { get; set; }

		bool HasDebugAIFlag => Context.GetCPASettings().Defines.HasFlag(CPA_EngineDefines.DebugAI) && Context.GetCPASettings().Mode == CPA_GameMode.Rayman3GC;

		public AI_InterpretType? LinkedType => Context.GetCPASettings().AITypes?.GetNodeType(Type);

		public override void SerializeImpl(SerializerObject s) {
			Param = s.Serialize<uint>(Param, name: nameof(Param));
			if (s.GetCPASettings().Platform == Platform.DC) {
				R2DCUnknown = s.Serialize<uint>(R2DCUnknown, name: nameof(R2DCUnknown));
			}
			if (HasDebugAIFlag) {
				Type = (byte)s.Serialize<uint>((uint)Type, name: nameof(Type));
			}
			NodesToSkip = s.Serialize<ushort>(NodesToSkip, name: nameof(NodesToSkip));
			Depth = s.Serialize<byte>(Depth, name: nameof(Depth));
			if (!HasDebugAIFlag) {
				Type = s.Serialize<byte>(Type, name: nameof(Type));
			} else {
				s.SerializePadding(1, logIfNotNull: true);
			}

			s?.Log($"Linked Type: {LinkedType}");
		}
	}
}
