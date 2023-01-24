namespace BinarySerializer.Ubisoft.CPA {
	public class A3D_Runtime_Channel : BinarySerializable {
		public Pointer<HIE_SuperObject> SuperObject { get; set; }
		public Pointer<HIE_SuperObject> ZoomSuperObject { get; set; }
		public Pointer<A3D_Runtime_Channel> NextActiveChannel { get; set; }
		public Pointer<A3D_Runtime_Channel> ParentChannel { get; set; }

		public bool IsControlled { get; set; }
		public bool IsActive { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SuperObject = s.SerializePointer<HIE_SuperObject>(SuperObject, name: nameof(SuperObject))?.ResolveObject(s);
			ZoomSuperObject = s.SerializePointer<HIE_SuperObject>(ZoomSuperObject, name: nameof(ZoomSuperObject))?.ResolveObject(s);
			NextActiveChannel = s.SerializePointer<A3D_Runtime_Channel>(NextActiveChannel, name: nameof(NextActiveChannel))?.ResolveObject(s);
			ParentChannel = s.SerializePointer<A3D_Runtime_Channel>(ParentChannel, name: nameof(ParentChannel))?.ResolveObject(s);
			IsControlled = s.Serialize<bool>(IsControlled, name: nameof(IsControlled));
			IsActive = s.Serialize<bool>(IsActive, name: nameof(IsActive));
			s.Align(4, Offset);
		}
	}
}