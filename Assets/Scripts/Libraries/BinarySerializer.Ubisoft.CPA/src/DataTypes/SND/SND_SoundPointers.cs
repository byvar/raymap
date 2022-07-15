namespace BinarySerializer.Ubisoft.CPA {
	public class SND_SoundPointers : BinarySerializable {
		public SND_SoundPointer[] Pointers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Pointers = s.SerializeObjectArrayUntil<SND_SoundPointer>(Pointers, _ => s.CurrentFileOffset >= s.CurrentLength, name: nameof(Pointers));
		}
	}
}
