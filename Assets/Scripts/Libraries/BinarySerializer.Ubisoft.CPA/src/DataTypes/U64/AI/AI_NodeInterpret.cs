using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	// Full NodeInterpret struct on DS/N64. Used only for NODFile struct type
	public class AI_NodeInterpret : U64_Struct {
		public uint Param { get; set; }
		public ushort NodesToSkip { get; set; }
		public byte Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.LogWarning($"{GetType()} is being serialized! Check if serialized correctly");
			Param = s.Serialize<uint>(Param, name: nameof(Param));
			NodesToSkip = s.Serialize<ushort>(NodesToSkip, name: nameof(NodesToSkip));
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			s.SerializePadding(1, logIfNotNull: true);
		}

		public static uint StructSize => 8;
	}
}
