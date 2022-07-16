namespace BinarySerializer.Ubisoft.CPA {
	public class GMT_DemoGMTList : BinarySerializable, ILST2_StaticEntry<GMT_DemoGMTList> {
		public LST2_StaticListElement<GMT_DemoGMTList> LST2_Element { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GMT_DemoGMTList>> LST2_Parent => LST2_Element?.LST2_Parent;
		public Pointer<GMT_DemoGMTList> LST2_Next => LST2_Element?.LST2_Next;
		public Pointer<GMT_DemoGMTList> LST2_Previous => LST2_Element?.LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			LST2_Element = s.SerializeObject<LST2_StaticListElement<GMT_DemoGMTList>>(LST2_Element, name: nameof(LST2_Element))?.Resolve(s);
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
		}
	}
}
