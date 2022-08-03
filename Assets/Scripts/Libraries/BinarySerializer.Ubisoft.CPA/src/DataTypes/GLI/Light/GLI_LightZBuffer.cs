namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_LightZBuffer : BinarySerializable {
		public uint SizeX { get; set; }
		public uint SizeY { get; set; }
		public float CoefX { get; set; }
		public float CoefY { get; set; }
		public Pointer<float[]> ZBufferMap { get; set; }
		public Pointer<float[]> MiddleZBufferMap { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SizeX = s.Serialize<uint>(SizeX, name: nameof(SizeX));
			SizeY = s.Serialize<uint>(SizeY, name: nameof(SizeY));
			CoefX = s.Serialize<float>(CoefX, name: nameof(CoefX));
			CoefY = s.Serialize<float>(CoefY, name: nameof(CoefY));
			ZBufferMap = s.SerializePointer<float[]>(ZBufferMap, name: nameof(ZBufferMap));
			MiddleZBufferMap = s.SerializePointer<float[]>(MiddleZBufferMap, name: nameof(MiddleZBufferMap));
		}
	}
}
