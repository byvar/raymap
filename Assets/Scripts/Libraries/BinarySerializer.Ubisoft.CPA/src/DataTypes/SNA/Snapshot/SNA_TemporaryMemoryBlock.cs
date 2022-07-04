namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_TemporaryMemoryBlock : SNA_MemoryBlock {
		public override byte Module => (byte)Context.GetCPASettings().SNATypes?.GetModuleInt(ModuleTranslation.Value);
		public override SNA_Module? ModuleTranslation => SNA_Module.TMP;
		public override string BlockName => $"{ModuleTranslation?.ToString() ?? Module.ToString()}_{Block}";

		public override void SerializeImpl(SerializerObject s) {
			Data = s.SerializeArray<byte>(Data, s.CurrentLength - s.CurrentFileOffset, name: nameof(Data));
		}
	}
}
