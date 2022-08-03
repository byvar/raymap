namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_DemoSO : BinarySerializable, ILST2_StaticEntry<GAM_DemoSO> {
		public LST2_StaticListElement<GAM_DemoSO> ListElement { get; set; }
		public Pointer<HIE_SuperObject> CharacterSuperObject { get; set; }
		public byte Reinit { get; set; }
		public byte DeltaCounter { get; set; }
		public MTH3D_Vector Translation { get; set; }
		public GAM_DemoCompressedTransform CompressedTransform { get; set; }

		public ushort StateIndex { get; set; }
		public ushort CurrentFrame { get; set; }
		public byte Activate { get; set; }
		public byte Transparency { get; set; }

		// LST2 Implementation
		public Pointer<LST2_StaticList<GAM_DemoSO>> LST2_Parent => ((ILST2_StaticEntry<GAM_DemoSO>)ListElement).LST2_Parent;
		public Pointer<GAM_DemoSO> LST2_Next => ((ILST2_Entry<GAM_DemoSO>)ListElement).LST2_Next;
		public Pointer<GAM_DemoSO> LST2_Previous => ((ILST2_Entry<GAM_DemoSO>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				ListElement = s.SerializeObject<LST2_StaticListElement<GAM_DemoSO>>(ListElement, name: nameof(ListElement))?.Resolve(s);
			}
			CharacterSuperObject = s.SerializePointer<HIE_SuperObject>(CharacterSuperObject, name: nameof(CharacterSuperObject))?.ResolveObject(s);
			Reinit = s.Serialize<byte>(Reinit, name: nameof(Reinit));
			DeltaCounter = s.Serialize<byte>(DeltaCounter, name: nameof(DeltaCounter));
			s.Align(4, Offset);
			Translation = s.SerializeObject<MTH3D_Vector>(Translation, name: nameof(Translation));
			CompressedTransform = s.SerializeObject<GAM_DemoCompressedTransform>(CompressedTransform, name: nameof(CompressedTransform));

			StateIndex = s.Serialize<ushort>(StateIndex, name: nameof(StateIndex));
			CurrentFrame = s.Serialize<ushort>(CurrentFrame, name: nameof(CurrentFrame));
			Activate = s.Serialize<byte>(Activate, name: nameof(Activate));
			Transparency = s.Serialize<byte>(Transparency, name: nameof(Transparency));
		}
	}
}
