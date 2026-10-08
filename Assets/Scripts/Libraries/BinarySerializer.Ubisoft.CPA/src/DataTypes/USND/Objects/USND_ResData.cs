namespace BinarySerializer.Ubisoft.CPA {
	public abstract class USND_ResData : USND_IdObj {
		public uint Version { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			Version = s.Serialize<uint>(Version, name: nameof(Version));
		}
	}
}