namespace BinarySerializer.Ubisoft.CPA.U64 {
	// "ZeFonte"
	public class GLI_CPakFont : U64_Struct {
		public const int LettersCount = 117;
		public Letter[] Letters { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Letters = s.SerializeObjectArray<Letter>(Letters, LettersCount, name: nameof(Letters));
		}

		public class Letter : BinarySerializable {
			public const int BytesPerLineCount = 3;
			public const int LinesCount = 18;

			public byte[] Bitmap { get; set; }
			public byte BitmapSize { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Bitmap = s.SerializeArray<byte>(Bitmap, LinesCount * BytesPerLineCount, name: nameof(Bitmap));
				BitmapSize = s.Serialize<byte>(BitmapSize, name: nameof(BitmapSize));
			}
		}
	}

}
