namespace BinarySerializer.Ubisoft.CPA {
	public class IPT_Input : BinarySerializable {
		public IPT_OnePadActivate OnePadActivate { get; set; }
		public IPT_OnePadActivate[] ValidAndActiveDevices { get; set; }
		public Pointer<IPT_Device>[] DevicesPointers { get; set; } // R2
		public IPT_Device[] Devices { get; set; } // R3
		public byte[] KeyboardCounter { get; set; }
		public IPT_DevicePadAndJoyCounter[] PadAndJoyCounter { get; set; }

		public SCR_LinkTable EntryLink { get; set; }
		public SCR_LinkTable CommandLink { get; set; }

		public IPT_KeyboardType KeyboardType { get; set; }
		public byte[] MouseButtonsCounter { get; set; }
		public byte[] UnknownBytes0 { get; set; }
		public uint EntryElementsCount { get; set; }
		public Pointer<IPT_EntryElement[]> EntryElements { get; set; }
		public LST2_DynamicList<IPT_EntryElement> EntryElementList { get; set; } // Unused?
		public short EventHistorySize { get; set; }
		public Pointer<IPT_EventHistoryElement[]> EventHistory { get; set; } // TODO



		public uint DevicesCount {
			get {
				if (Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)
					|| Context.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)
					|| Context.GetCPASettings().Platform == Platform.iOS) {
					return 18;
				} else {
					return 19;
				}
			}
		}

		public override void SerializeImpl(SerializerObject s) {
			OnePadActivate = s.Serialize<IPT_OnePadActivate>(OnePadActivate, name: nameof(OnePadActivate));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				ValidAndActiveDevices = s.SerializeArray<IPT_OnePadActivate>(ValidAndActiveDevices, DevicesCount, name: nameof(ValidAndActiveDevices));

				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)
					&& s.GetCPASettings().Platform != Platform.iOS)
					DevicesPointers = s.SerializePointerArray<IPT_Device>(DevicesPointers, DevicesCount, name: nameof(DevicesPointers));

			} else {
				s.Align(4, Offset);
				Devices = s.SerializeObjectArray<IPT_Device>(Devices, DevicesCount, name: nameof(Devices));
			}

			KeyboardCounter = s.SerializeArray<byte>(KeyboardCounter, 256, name: nameof(KeyboardCounter));

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				PadAndJoyCounter = s.SerializeObjectArray<IPT_DevicePadAndJoyCounter>(PadAndJoyCounter, DevicesCount, name: nameof(PadAndJoyCounter));
			}
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
				s.Align(4, Offset);
				EntryLink = s.SerializeObject<SCR_LinkTable>(EntryLink, name: nameof(EntryLink));
				CommandLink = s.SerializeObject<SCR_LinkTable>(CommandLink, name: nameof(CommandLink));
			}
			KeyboardType = s.Serialize<IPT_KeyboardType>(KeyboardType, name: nameof(KeyboardType));
			if (s.GetCPASettings().Platform != Platform.iOS && !s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
				MouseButtonsCounter = s.SerializeArray<byte>(MouseButtonsCounter, 9, name: nameof(MouseButtonsCounter));

				if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
					UnknownBytes0 = s.SerializeArray<byte>(UnknownBytes0, 24, name: nameof(UnknownBytes0));
				}
			}
			s.Align(4, Offset);
			EntryElementsCount = s.Serialize<uint>(EntryElementsCount, name: nameof(EntryElementsCount));
			EntryElements = s.SerializePointer<IPT_EntryElement[]>(EntryElements, name: nameof(EntryElements))?.ResolveObjectArray(s, EntryElementsCount);
			EntryElementList = s.SerializeObject<LST2_DynamicList<IPT_EntryElement>>(EntryElementList, name: nameof(EntryElementList));
			EventHistorySize = s.Serialize<short>(EventHistorySize, name: nameof(EventHistorySize));
			s.Align(4, Offset);
			EventHistory = s.SerializePointer<IPT_EventHistoryElement[]>(EventHistory, name: nameof(EventHistory))?.ResolveObjectArray(s, EventHistorySize);
		}
	}
}
