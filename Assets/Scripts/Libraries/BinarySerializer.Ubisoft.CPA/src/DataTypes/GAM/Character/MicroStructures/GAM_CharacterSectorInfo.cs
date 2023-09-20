namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterSectorInfo : BinarySerializable {
		public Pointer<HIE_SuperObject> PreviousSector { get; set; }
		public Pointer<HIE_SuperObject> CurrentSector { get; set; }
		public Pointer<SCT_Character> NodeInSector { get; set; }
		public MTH3D_Vector PreviousPosition { get; set; }
		public Pointer<HIE_SuperObject> SectorSynchroCharacter { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2)) {
				PreviousSector = s.SerializePointer<HIE_SuperObject>(PreviousSector, name: nameof(PreviousSector))?.ResolveObject(s);
			}
			CurrentSector = s.SerializePointer<HIE_SuperObject>(CurrentSector, name: nameof(CurrentSector))?.ResolveObject(s);
			NodeInSector = s.SerializePointer<SCT_Character>(NodeInSector, name: nameof(NodeInSector))?.ResolveObject(s);
			PreviousPosition = s.SerializeObject<MTH3D_Vector>(PreviousPosition, name: nameof(PreviousPosition));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				SectorSynchroCharacter = s.SerializePointer<HIE_SuperObject>(SectorSynchroCharacter, name: nameof(SectorSynchroCharacter))?.ResolveObject(s);
			}
		}
	}
}
