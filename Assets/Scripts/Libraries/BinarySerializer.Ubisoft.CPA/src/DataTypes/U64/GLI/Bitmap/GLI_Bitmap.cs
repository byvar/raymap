namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GLI_Bitmap : U64_Struct {
		public int Pre_WidthLog { get; set; }
		public int Pre_HeightLog { get; set; }
		public int Width => 1 << Pre_WidthLog;
		public int Height => 1 << Pre_HeightLog;
		public int Length => Width * Height;

		public override void SerializeImpl(SerializerObject s) {
			throw new BinarySerializableException(this, $"Base {GetType()} SerializeImpl called!");
		}
	}

}
