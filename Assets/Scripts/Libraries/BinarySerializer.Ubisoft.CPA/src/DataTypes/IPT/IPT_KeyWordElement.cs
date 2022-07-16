namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_KeyWordElement : BinarySerializable, ILST2_DynamicEntry<IPT_KeyWordElement> {
		public LST2_DynamicListElement<IPT_KeyWordElement> LST2_Element { get; set; }

		public IPT_KeyWordElementUnion ElementUnion { get; set; }
		public IPT_KeyWordElementUnion ElementUnionDefault { get; set; }
		public byte Result { get; set; }
		public byte MinCounterInput { get; set; }
		public byte MaxCounterInput { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<IPT_KeyWordElement>> LST2_Parent => LST2_Element?.LST2_Parent;
		public Pointer<IPT_KeyWordElement> LST2_Next => LST2_Element?.LST2_Next;
		public Pointer<IPT_KeyWordElement> LST2_Previous => LST2_Element?.LST2_Previous;


		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().HasExtraInputData) {
				LST2_Element = s.SerializeObject<LST2_DynamicListElement<IPT_KeyWordElement>>(LST2_Element, name: nameof(LST2_Element))?.Resolve(s);
			}
			ElementUnion = s.SerializeObject<IPT_KeyWordElementUnion>(ElementUnion, name: nameof(ElementUnion));
			if(s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
				ElementUnionDefault = s.SerializeObject<IPT_KeyWordElementUnion>(ElementUnionDefault, name: nameof(ElementUnionDefault));

			if (s.GetCPASettings().EngineVersion != EngineVersion.Rayman2Revolution) {
				Result = s.Serialize<byte>(Result, name: nameof(Result));
				MinCounterInput = s.Serialize<byte>(MinCounterInput, name: nameof(MinCounterInput));
				MaxCounterInput = s.Serialize<byte>(MaxCounterInput, name: nameof(MaxCounterInput));
				s.SerializePadding(1, logIfNotNull: true);
			}
		}
	}
}
