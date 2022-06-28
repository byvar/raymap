namespace BinarySerializer.Ubisoft.CPA {
	public class SNA_Description_FrameSynchro : SNA_Description_Data {
		public SNA_Description_String OnOff { get; set; }
		public SNA_Description_String FramesCount { get; set; }
		public SNA_Description_String LowLimitPercent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			OnOff = s.SerializeObject<SNA_Description_String>(OnOff, name: nameof(OnOff));
			FramesCount = s.SerializeObject<SNA_Description_String>(FramesCount, name: nameof(FramesCount));
			LowLimitPercent = s.SerializeObject<SNA_Description_String>(LowLimitPercent, name: nameof(LowLimitPercent));
		}
	}
}
