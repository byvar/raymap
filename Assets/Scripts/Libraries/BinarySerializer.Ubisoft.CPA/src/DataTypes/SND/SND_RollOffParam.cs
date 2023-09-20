namespace BinarySerializer.Ubisoft.CPA {
	public class SND_RollOffParam : BinarySerializable {
		public SND_Real SaturationDistance { get; set; }
		public SND_Real BackgroundDistance { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SaturationDistance = s.SerializeObject<SND_Real>(SaturationDistance, name: nameof(SaturationDistance));
			BackgroundDistance = s.SerializeObject<SND_Real>(BackgroundDistance, name: nameof(BackgroundDistance));
		}
	}
}
