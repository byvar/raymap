namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_SectorSoundEvent : BinarySerializable, ILST2_StaticEntry<SCT_SectorSoundEvent> {
		public Pointer<HIE_SuperObject> Sector { get; set; }
		public uint SoundEvent { get; set; }
		public LST2_StaticListElement<SCT_SectorSoundEvent> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<SCT_SectorSoundEvent>> LST2_Parent => ((ILST2_StaticEntry<SCT_SectorSoundEvent>)ListElement).LST2_Parent;
		public Pointer<SCT_SectorSoundEvent> LST2_Next => ((ILST2_Entry<SCT_SectorSoundEvent>)ListElement).LST2_Next;
		public Pointer<SCT_SectorSoundEvent> LST2_Previous => ((ILST2_Entry<SCT_SectorSoundEvent>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Sector = s.SerializePointer<HIE_SuperObject>(Sector, name: nameof(Sector))?.ResolveObject(s);
			SoundEvent = s.Serialize<uint>(SoundEvent, name: nameof(SoundEvent));
			ListElement = s.SerializeObject<LST2_StaticListElement<SCT_SectorSoundEvent>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
