﻿namespace BinarySerializer.Ubisoft.CPA {
	public class SCT_SectorInActivityInteraction : BinarySerializable, ILST2_StaticEntry<SCT_SectorInActivityInteraction> {
		public Pointer<HIE_SuperObject> Sector { get; set; }
		public LST2_StaticListElement<SCT_SectorInActivityInteraction> ListElement { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<SCT_SectorInActivityInteraction>> LST2_Parent => ((ILST2_StaticEntry<SCT_SectorInActivityInteraction>)ListElement).LST2_Parent;
		public Pointer<SCT_SectorInActivityInteraction> LST2_Next => ((ILST2_Entry<SCT_SectorInActivityInteraction>)ListElement).LST2_Next;
		public Pointer<SCT_SectorInActivityInteraction> LST2_Previous => ((ILST2_Entry<SCT_SectorInActivityInteraction>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			Sector = s.SerializePointer<HIE_SuperObject>(Sector, name: nameof(Sector))?.ResolveObject(s);
			ListElement = s.SerializeObject<LST2_StaticListElement<SCT_SectorInActivityInteraction>>(ListElement, name: nameof(ListElement))?.Resolve(s);
		}
	}
}
