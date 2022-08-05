namespace BinarySerializer.Ubisoft.CPA {
	public class IPO_InstanciatedPhysicalObject : BinarySerializable, IHIE_LinkedObject {
		public Pointer<PO_PhysicalObject> PhysicalObject { get; set; }
		public Pointer<ISI_Radiosity> CurrentRadiosity { get; set; }

		public Pointer<Pointer<ISI_Radiosity>[]> Radiosity { get; set; }
		public Pointer<HIE_SuperObject> PortalCamera { get; set; }
		public uint LastTransitionID { get; set; }
		public float LastRatioUsed { get; set; }

		public uint Unknown0 { get; set; }
		public string Name { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			PhysicalObject = s.SerializePointer<PO_PhysicalObject>(PhysicalObject, name: nameof(PhysicalObject))?.ResolveObject(s);
			CurrentRadiosity = s.SerializePointer<ISI_Radiosity>(CurrentRadiosity, name: nameof(CurrentRadiosity))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				Radiosity = s.SerializePointer<Pointer<ISI_Radiosity>[]>(Radiosity, name: nameof(Radiosity)); // number of RLI tables is specified in LoadLevel
				PortalCamera = s.SerializePointer<HIE_SuperObject>(PortalCamera, name: nameof(PortalCamera))?.ResolveObject(s);
				LastTransitionID = s.Serialize<uint>(LastTransitionID, name: nameof(LastTransitionID));
				LastRatioUsed = s.Serialize<float>(LastRatioUsed, name: nameof(LastRatioUsed));
				if (s.GetCPASettings().HasNames) {
					Unknown0 = s.Serialize<uint>(Unknown0, name: nameof(Unknown0));
					Name = s.SerializeString(Name, length: 50, name: nameof(Name));
				}
			}
		}
	}
}
