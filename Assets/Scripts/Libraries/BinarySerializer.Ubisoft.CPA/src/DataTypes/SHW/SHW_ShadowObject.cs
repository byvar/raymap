namespace BinarySerializer.Ubisoft.CPA {
	public class SHW_ShadowObject : BinarySerializable {
		public uint ShadowElement { get; set; }
		public Pointer<GEO_GeometricObject> ShadowObject { get; set; }
		
		public override void SerializeImpl(SerializerObject s) {
			ShadowElement = s.Serialize<uint>(ShadowElement, name: nameof(ShadowElement));
			ShadowObject = s.SerializePointer<GEO_GeometricObject>(ShadowObject, name: nameof(ShadowObject))?.ResolveObject(s);
		}
	}
}
