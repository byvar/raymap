namespace BinarySerializer.Ubisoft.CPA {
	public class GLI_SpecificAttributesFor3D : BinarySerializable {
		public Pointer<GLI_Camera> Camera { get; set; }
		public float Near { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Camera = s.SerializePointer<GLI_Camera>(Camera, name: nameof(Camera))?.ResolveObject(s);
			Near = s.Serialize<float>(Near, name: nameof(Near));
		}
	}
}
