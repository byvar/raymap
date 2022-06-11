namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_FixMemory : GAM_LevelMemory {
		public uint FON { get; set; }
		public uint IPT { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			FON = s.Serialize<uint>(FON, name: nameof(FON));
			IPT = s.Serialize<uint>(IPT, name: nameof(IPT));
		}
	}
}
