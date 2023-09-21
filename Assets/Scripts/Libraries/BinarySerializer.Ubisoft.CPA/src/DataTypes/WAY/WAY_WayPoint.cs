namespace BinarySerializer.Ubisoft.CPA {
	public class WAY_WayPoint : BinarySerializable {
		public MTH3D_Vector Vertex { get; set; }
		public float Radius { get; set; }
		public Pointer<HIE_SuperObject> SuperObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Vertex = s.SerializeObject<MTH3D_Vector>(Vertex, name: nameof(Vertex));
			Radius = s.Serialize<float>(Radius, name: nameof(Radius));
			SuperObject = s.SerializePointer<HIE_SuperObject>(SuperObject, name: nameof(SuperObject))?.ResolveObject(s);
		}
	}
}
