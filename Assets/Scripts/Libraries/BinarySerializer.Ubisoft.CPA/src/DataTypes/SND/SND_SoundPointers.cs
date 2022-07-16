namespace BinarySerializer.Ubisoft.CPA {
	public class SND_SoundPointers : BinarySerializable {
		public SND_SoundPointer[] Pointers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Pointers = s.SerializeObjectArray<SND_SoundPointer>(Pointers, s.CurrentLength / SND_SoundPointer.StructSize, name: nameof(Pointers));
		}
	}
}
