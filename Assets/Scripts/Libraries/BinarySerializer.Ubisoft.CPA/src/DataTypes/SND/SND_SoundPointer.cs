namespace BinarySerializer.Ubisoft.CPA {
	public class SND_SoundPointer : BinarySerializable {
		public uint SoundEventId { get; set; }
		public Pointer SoundEventPointer { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SoundEventId = s.Serialize<uint>(SoundEventId, name: nameof(SoundEventId));
			SoundEventPointer = s.SerializePointer(SoundEventPointer, name: nameof(SoundEventPointer));

			// TODO: At SoundEventPointer, it writes a new pointer to the loaded sound event with this ID.
		}

		public static uint StructSize => 8;
	}
}
