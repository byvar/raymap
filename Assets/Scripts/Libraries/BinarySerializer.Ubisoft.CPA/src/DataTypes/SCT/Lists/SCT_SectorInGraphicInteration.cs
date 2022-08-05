namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_SectorInGraphicInteraction : BinarySerializable, ILST2_StaticEntry<SCT_SectorInGraphicInteraction> {
		public short LevelOfDetail { get; set; }
		public SCT_DisplayMode DisplayMode { get; set; }

		public Pointer<HIE_SuperObject> Sector { get; set; }
		public LST2_StaticListElement<SCT_SectorInGraphicInteraction> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<SCT_SectorInGraphicInteraction>> LST2_Parent => ((ILST2_StaticEntry<SCT_SectorInGraphicInteraction>)ListElement).LST2_Parent;
		public Pointer<SCT_SectorInGraphicInteraction> LST2_Next => ((ILST2_Entry<SCT_SectorInGraphicInteraction>)ListElement).LST2_Next;
		public Pointer<SCT_SectorInGraphicInteraction> LST2_Previous => ((ILST2_Entry<SCT_SectorInGraphicInteraction>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			LevelOfDetail = s.Serialize<short>(LevelOfDetail, name: nameof(LevelOfDetail));
			DisplayMode = s.Serialize<SCT_DisplayMode>(DisplayMode, name: nameof(DisplayMode));
			s.Align(4, Offset);
			Sector = s.SerializePointer<HIE_SuperObject>(Sector, name: nameof(Sector))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_StaticListElement<SCT_SectorInGraphicInteraction>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
