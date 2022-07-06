namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_EntryElement : BinarySerializable, LST2_IEntry<IPT_EntryElement> {
		public LST2_DynamicListElement<IPT_EntryElement> LST2_Element { get; set; }
		public LST2_DynamicList<IPT_KeyWordElement> KeyWordsList { get; set; }
		public uint KeyWordElementsCount { get; set; }
		public ArrayPointer<IPT_KeyWordElement> KeyWordElements { get; set; }

		public int State { get; set; }
		public float AnalogicValue { get; set; }
		public sbyte AnalogicValueSByte { get; set; }
		public bool IsActive { get; set; }

		public Pointer UnknownPointer { get; set; }
		public Pointer ActionName { get; set; }
		public Pointer EntryName { get; set; } // For options

		// LST2_IEntry implementation
		public Pointer<IPT_EntryElement> LST2_Next => ((LST2_IEntry<IPT_EntryElement>)LST2_Element).LST2_Next;
		public Pointer<IPT_EntryElement> LST2_Previous => ((LST2_IEntry<IPT_EntryElement>)LST2_Element).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetCPASettings().HasExtraInputData) {
				LST2_Element = s.SerializeObject<LST2_DynamicListElement<IPT_EntryElement>>(LST2_Element, name: nameof(LST2_Element));
				KeyWordsList = s.SerializeObject<LST2_DynamicList<IPT_KeyWordElement>>(KeyWordsList, name: nameof(KeyWordsList));
			}

			if(s.GetCPASettings().Platform == Platform.PS2)
				throw new BinarySerializableException(this, "TODO: Implement in EntryElement");

			if (s.GetCPASettings().EngineVersion != EngineVersion.TonicTroubleSE) {
				KeyWordElementsCount = s.Serialize<uint>(KeyWordElementsCount, name: nameof(KeyWordElementsCount));
				KeyWordElements = s.SerializeArrayPointer<IPT_KeyWordElement>(KeyWordElements, name: nameof(KeyWordElements))?.Resolve(s, KeyWordElementsCount);
			}

			if(!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_2) &&
				s.GetCPASettings().EngineVersion != EngineVersion.TonicTroubleSE)
				UnknownPointer = s.SerializePointer(UnknownPointer, name: nameof(UnknownPointer));

			ActionName = s.SerializePointer(ActionName, name: nameof(ActionName));
			if(s.GetCPASettings().HasExtraInputData
				|| s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
				|| s.GetCPASettings().Platform == Platform.DC)
				EntryName = s.SerializePointer(EntryName, name: nameof(EntryName));

			State = s.Serialize<int>(State, name: nameof(State));
			if (s.GetCPASettings().EngineVersion == EngineVersion.TonicTroubleSE) {
				AnalogicValueSByte = s.Serialize<sbyte>(AnalogicValueSByte, name: nameof(AnalogicValueSByte));
			} else {
				AnalogicValue = s.Serialize<float>(AnalogicValue, name: nameof(AnalogicValue));
			}
			IsActive = s.Serialize<bool>(IsActive, name: nameof(IsActive));
			s.Align(baseOffset: Offset);
		}
	}
}
