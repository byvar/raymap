using BinarySerializer.Audio.RIFF;

namespace BinarySerializer.Ubisoft.CPA {
	// Custom label chunk
	public class RIFF_Chunk_LabX : RIFF_ChunkData {
		public override string ChunkIdentifier => "labx";

		public uint LabelsCount { get; set; }
		public Label[] Labels { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LabelsCount = s.Serialize<uint>(LabelsCount, name: nameof(LabelsCount));
			Labels = s.SerializeObjectArray<Label>(Labels, LabelsCount, name: nameof(Labels));
		}

		public class Label : BinarySerializable {
			public string Name { get; set; }
			public uint SampleOffset { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Name = s.SerializeLengthPrefixedString<uint>(Name, name: nameof(Name));
				SampleOffset = s.Serialize<uint>(SampleOffset, name: nameof(SampleOffset));
			}
		}
	}
}