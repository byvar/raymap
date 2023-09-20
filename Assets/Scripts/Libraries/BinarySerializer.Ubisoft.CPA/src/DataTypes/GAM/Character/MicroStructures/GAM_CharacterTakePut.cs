namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterTakePut : BinarySerializable {
		public Pointer<HIE_SuperObject> TakenOrPutSuperObject { get; set; }
		public int TargetModuleId { get; set; }
		public uint Flags { get; set; }
		public bool DestroyTakenObjectAtReinit { get; set; }
		public Pointer<MEC_MechanicsIdCard> PreviousMechanicsIdCard { get; set; }
		public Pointer<HIE_SuperObject> CurrentSuperObjectInventory { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			TakenOrPutSuperObject = s.SerializePointer<HIE_SuperObject>(TakenOrPutSuperObject, name: nameof(TakenOrPutSuperObject))?.ResolveObject(s);
			TargetModuleId = s.Serialize<int>(TargetModuleId, name: nameof(TargetModuleId));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			DestroyTakenObjectAtReinit = s.Serialize<bool>(DestroyTakenObjectAtReinit, name: nameof(DestroyTakenObjectAtReinit));
			s.Align(4, Offset);
			PreviousMechanicsIdCard = s.SerializePointer<MEC_MechanicsIdCard>(PreviousMechanicsIdCard, name: nameof(PreviousMechanicsIdCard))?.ResolveObject(s);
			CurrentSuperObjectInventory = s.SerializePointer<HIE_SuperObject>(CurrentSuperObjectInventory, name: nameof(CurrentSuperObjectInventory))?.ResolveObject(s);
		}
	}
}
