namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CameraNode : BinarySerializable, ILST2_DynamicEntry<GAM_CameraNode> {
		public Pointer<HIE_SuperObject> CameraSuperObject { get; set; } 
		public LST2_DynamicListElement<GAM_CameraNode> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_CameraNode>> LST2_Parent => ((ILST2_DynamicEntry<GAM_CameraNode>)ListElement).LST2_Parent;
		public Pointer<GAM_CameraNode> LST2_Next => ((ILST2_Entry<GAM_CameraNode>)ListElement).LST2_Next;
		public Pointer<GAM_CameraNode> LST2_Previous => ((ILST2_Entry<GAM_CameraNode>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			CameraSuperObject = s.SerializePointer<HIE_SuperObject>(CameraSuperObject, name: nameof(CameraSuperObject))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_CameraNode>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
