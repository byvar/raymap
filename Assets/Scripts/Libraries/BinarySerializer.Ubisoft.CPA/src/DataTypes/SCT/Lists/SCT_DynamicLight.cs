namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_DynamicLight : BinarySerializable, ILST2_DynamicEntry<SCT_DynamicLight> {
		public Pointer<GLI_Light> Light { get; set; }
		public LST2_DynamicListElement<SCT_DynamicLight> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<SCT_DynamicLight>> LST2_Parent => ((ILST2_DynamicEntry<SCT_DynamicLight>)ListElement).LST2_Parent;
		public Pointer<SCT_DynamicLight> LST2_Next => ((ILST2_Entry<SCT_DynamicLight>)ListElement).LST2_Next;
		public Pointer<SCT_DynamicLight> LST2_Previous => ((ILST2_Entry<SCT_DynamicLight>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Light = s.SerializePointer<GLI_Light>(Light, name: nameof(Light))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_DynamicListElement<SCT_DynamicLight>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
