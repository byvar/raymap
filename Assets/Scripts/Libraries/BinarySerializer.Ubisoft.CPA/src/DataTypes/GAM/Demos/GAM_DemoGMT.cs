namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_DemoGMT : BinarySerializable, ILST2_StaticEntry<GAM_DemoGMT> {
		public LST2_StaticListElement<GAM_DemoGMT> ListElement { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_DemoGMT>> LST2_Parent => ((ILST2_StaticEntry<GAM_DemoGMT>)ListElement).LST2_Parent;
		public Pointer<GAM_DemoGMT> LST2_Next => ((ILST2_Entry<GAM_DemoGMT>)ListElement).LST2_Next;
		public Pointer<GAM_DemoGMT> LST2_Previous => ((ILST2_Entry<GAM_DemoGMT>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_DemoGMT>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
		}
	}
}
