namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_ObjectsTable : BinarySerializable, ILST2_StaticEntry<GAM_ObjectsTable> {
		public LST2_StaticListElement<GAM_ObjectsTable> ListElement { get; set; }
		public Pointer<GAM_ObjectsTableElement[]> ObjectsTable { get; set; }
		public Pointer<GAM_ObjectsTableElement[]> InitObjectsTable { get; set; }
		public ushort ElementsCount { get; set; }
		public GAM_ObjectsTableZDx ZDxUsed { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_ObjectsTable>> LST2_Parent => ((ILST2_StaticEntry<GAM_ObjectsTable>)ListElement).LST2_Parent;
		public Pointer<GAM_ObjectsTable> LST2_Next => ((ILST2_Entry<GAM_ObjectsTable>)ListElement).LST2_Next;
		public Pointer<GAM_ObjectsTable> LST2_Previous => ((ILST2_Entry<GAM_ObjectsTable>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_StaticListElement<GAM_ObjectsTable>>(ListElement, name: nameof(ListElement));
			ObjectsTable = s.SerializePointer<GAM_ObjectsTableElement[]>(ObjectsTable, name: nameof(ObjectsTable));
			InitObjectsTable = s.SerializePointer<GAM_ObjectsTableElement[]>(InitObjectsTable, name: nameof(InitObjectsTable));
			ElementsCount = s.Serialize<ushort>(ElementsCount, name: nameof(ElementsCount));
			ZDxUsed = s.Serialize<GAM_ObjectsTableZDx>(ZDxUsed, name: nameof(ZDxUsed));

			ObjectsTable?.ResolveObjectArray(s, ElementsCount);
			InitObjectsTable?.ResolveObjectArray(s, ElementsCount);
		}
	}
}
