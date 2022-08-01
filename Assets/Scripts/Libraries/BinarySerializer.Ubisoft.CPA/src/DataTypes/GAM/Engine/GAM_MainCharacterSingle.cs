namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_MainCharacterSingle : BinarySerializable {
		public Pointer<HIE_SuperObject> Character { get; set; }
		public Pointer<HIE_SuperObject> NewCharacterForTheNextFrame { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Character = s.SerializePointer<HIE_SuperObject>(Character, name: nameof(Character))?.ResolveObject(s);
			NewCharacterForTheNextFrame = s.SerializePointer<HIE_SuperObject>(NewCharacterForTheNextFrame, name: nameof(NewCharacterForTheNextFrame))?.ResolveObject(s);
		}
	}
}
