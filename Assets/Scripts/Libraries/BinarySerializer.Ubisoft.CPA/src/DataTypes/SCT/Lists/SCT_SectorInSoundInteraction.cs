namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_SectorInSoundInteraction : BinarySerializable, ILST2_StaticEntry<SCT_SectorInSoundInteraction> {
		public Pointer<HIE_SuperObject> Sector { get; set; }
		public int Volume { get; set; }
		public LST2_StaticListElement<SCT_SectorInSoundInteraction> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<SCT_SectorInSoundInteraction>> LST2_Parent => ((ILST2_StaticEntry<SCT_SectorInSoundInteraction>)ListElement).LST2_Parent;
		public Pointer<SCT_SectorInSoundInteraction> LST2_Next => ((ILST2_Entry<SCT_SectorInSoundInteraction>)ListElement).LST2_Next;
		public Pointer<SCT_SectorInSoundInteraction> LST2_Previous => ((ILST2_Entry<SCT_SectorInSoundInteraction>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Sector = s.SerializePointer<HIE_SuperObject>(Sector, name: nameof(Sector))?.ResolveObject(s);
			Volume = s.Serialize<int>(Volume, name: nameof(Volume));
			ListElement = s.SerializeObject<LST2_StaticListElement<SCT_SectorInSoundInteraction>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
