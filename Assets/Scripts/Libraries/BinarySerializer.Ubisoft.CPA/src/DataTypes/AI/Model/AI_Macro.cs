namespace BinarySerializer.Ubisoft.CPA {
	public class AI_Macro : BinarySerializable {
		public string Name { get; set; }
		public Pointer<AI_TreeInterpret> InitTree { get; set; }
		public Pointer<AI_TreeInterpret> CurrentTree { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().Defines.HasFlag(CPA_EngineDefines.DebugAI)
				 && s.GetCPASettings().Platform != Platform.PS2
				 && s.GetCPASettings().Platform != Platform.PS3
				 && s.GetCPASettings().Platform != Platform.Xbox360) {
				Name = s.SerializeString(Name, length: 0x100, name: nameof(Name));
			}
			InitTree = s.SerializePointer<AI_TreeInterpret>(InitTree, name: nameof(InitTree))?.ResolveObject(s);
			CurrentTree = s.SerializePointer(CurrentTree, name: nameof(CurrentTree))?.ResolveObject(s);
		}
	}
}
