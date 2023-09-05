namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_Obstacle : BinarySerializable {
		public float Rate { get; set; }
		public MTH3D_Vector Norm { get; set; }
		public MTH3D_Vector Contact { get; set; }
		public Pointer<GMT_GameMaterial> MyMaterial { get; set; }
		public Pointer<GMT_GameMaterial> CollidedMaterial { get; set; }
		public Pointer<HIE_SuperObject> SuperObject { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Rate = s.Serialize<float>(Rate, name: nameof(Rate));
			Norm = s.SerializeObject<MTH3D_Vector>(Norm, name: nameof(Norm));
			Contact = s.SerializeObject<MTH3D_Vector>(Contact, name: nameof(Contact));
			MyMaterial = s.SerializePointer<GMT_GameMaterial>(MyMaterial, name: nameof(MyMaterial))?.ResolveObject(s);
			CollidedMaterial = s.SerializePointer<GMT_GameMaterial>(CollidedMaterial, name: nameof(CollidedMaterial))?.ResolveObject(s);
			SuperObject = s.SerializePointer<HIE_SuperObject>(SuperObject, name: nameof(SuperObject))?.ResolveObject(s);
		}
	}
}
