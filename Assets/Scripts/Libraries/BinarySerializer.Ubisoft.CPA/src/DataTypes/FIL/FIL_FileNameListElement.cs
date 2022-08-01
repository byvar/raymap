namespace BinarySerializer.Ubisoft.CPA {
	public class FIL_FileNameListElement : BinarySerializable, ILST2_DynamicEntry<FIL_FileNameListElement> {
		public LST2_DynamicListElement<FIL_FileNameListElement> ListElement { get; set; }
		public Pointer<string> FileName { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<FIL_FileNameListElement>> LST2_Parent => ((ILST2_DynamicEntry<FIL_FileNameListElement>)ListElement).LST2_Parent;
		public Pointer<FIL_FileNameListElement> LST2_Next => ((ILST2_Entry<FIL_FileNameListElement>)ListElement).LST2_Next;
		public Pointer<FIL_FileNameListElement> LST2_Previous => ((ILST2_Entry<FIL_FileNameListElement>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<FIL_FileNameListElement>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			FileName = s.SerializePointer<string>(FileName, name: nameof(FileName))?.ResolveString(s);
		}
	}
}
