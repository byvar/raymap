namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_StaticLight : BinarySerializable, ILST2_StaticEntry<SCT_StaticLight> {
		public Pointer<GLI_Light> Light { get; set; }
		public LST2_StaticListElement<SCT_StaticLight> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<SCT_StaticLight>> LST2_Parent => ((ILST2_StaticEntry<SCT_StaticLight>)ListElement).LST2_Parent;
		public Pointer<SCT_StaticLight> LST2_Next => ((ILST2_Entry<SCT_StaticLight>)ListElement).LST2_Next;
		public Pointer<SCT_StaticLight> LST2_Previous => ((ILST2_Entry<SCT_StaticLight>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Light = s.SerializePointer<GLI_Light>(Light, name: nameof(Light))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_StaticListElement<SCT_StaticLight>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
