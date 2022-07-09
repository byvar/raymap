namespace BinarySerializer.Ubisoft.CPA {
	public class GMT_DemoGMTList : BinarySerializable, LST2_IEntry<GMT_DemoGMTList> {
		public LST2_StaticListElement<GMT_DemoGMTList> ListElement { get; set; }
		public Pointer<GMT_GameMaterial> GameMaterial { get; set; }

		public Pointer<GMT_DemoGMTList> LST2_Next => ((LST2_IEntry<GMT_DemoGMTList>)ListElement).LST2_Next;
		public Pointer<GMT_DemoGMTList> LST2_Previous => ((LST2_IEntry<GMT_DemoGMTList>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GMT_DemoGMTList>>(ListElement, name: nameof(ListElement));
			GameMaterial = s.SerializePointer<GMT_GameMaterial>(GameMaterial, name: nameof(GameMaterial))?.ResolveObject(s);
		}
	}
}
