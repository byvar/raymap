namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ViewportManagement : BinarySerializable {
		public Pointer<HIE_SuperObject> CameraSuperObject { get; set; }
		public Pointer<HIE_SuperObject> TempCameraSuperObject { get; set; }
		public Pointer<GLI_Camera> Camera { get; set; }
		public bool IsValid { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CameraSuperObject = s.SerializePointer<HIE_SuperObject>(CameraSuperObject, name: nameof(CameraSuperObject))?.ResolveObject(s);
			TempCameraSuperObject = s.SerializePointer<HIE_SuperObject>(TempCameraSuperObject, name: nameof(TempCameraSuperObject))?.ResolveObject(s);
			Camera = s.SerializePointer<GLI_Camera>(Camera, name: nameof(Camera))?.ResolveObject(s);
			IsValid = s.Serialize<bool>(IsValid, name: nameof(IsValid));
			s.Align(4, Offset);
		}
	}
}
