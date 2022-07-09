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
		public LST2_DynamicList<IPT_EntryElement> ScriptEntryElementList { get; set; } // Only used for scripts
		public LST2_DynamicList<IPT_CommandElement> ScriptCommandElementList { get; set; } // Only used for scripts
		public uint EntryElementsCount { get; set; }
		public Pointer<IPT_EntryElement[]> EntryElements { get; set; }
		public uint CommandElementsCount { get; set; }
		public Pointer<IPT_CommandElement[]> CommandElements { get; set; }
		public uint IPTMemorySize { get; set; }
		public short EventHistorySize { get; set; }
		public Pointer<IPT_EventHistoryElement[]> EventHistory { get; set; } // TODO

		public bool AtLeastOneActionIsValidated { get; set; }
		public string LineCommand { get; set; }
		public string InternLineCommand { get; set; }
		public string SearchedLineCommand { get; set; }
		public string[] LastLineCommand { get; set; }
		public uint IndexInHistoryCommand { get; set; }
		public Pointer<IPT_CommandElement> SearchedCommand { get; set; }
		public Pointer<IPT_EntryElement> SwapCommandMode { get; set; }
		public Pointer<IPT_EntryElement> ClearCommandMode { get; set; }


		public const int StringLength = 78;

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
					ScriptEntryElementList = s.SerializeObject<LST2_DynamicList<IPT_EntryElement>>(ScriptEntryElementList, name: nameof(ScriptEntryElementList));
					ScriptCommandElementList = s.SerializeObject<LST2_DynamicList<IPT_CommandElement>>(ScriptCommandElementList, name: nameof(ScriptCommandElementList));
				}
			}
			s.Align(4, Offset);
			EntryElementsCount = s.Serialize<uint>(EntryElementsCount, name: nameof(EntryElementsCount));
			EntryElements = s.SerializePointer<IPT_EntryElement[]>(EntryElements, name: nameof(EntryElements))?.ResolveObjectArray(s, EntryElementsCount);
			CommandElementsCount = s.Serialize<uint>(CommandElementsCount, name: nameof(CommandElementsCount));
			CommandElements = s.SerializePointer<IPT_CommandElement[]>(CommandElements, name: nameof(CommandElements))?.ResolveObjectArray(s, CommandElementsCount);
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				IPTMemorySize = s.Serialize<uint>(IPTMemorySize, name: nameof(IPTMemorySize));
			}
			EventHistorySize = s.Serialize<short>(EventHistorySize, name: nameof(EventHistorySize));
			s.Align(4, Offset);
			EventHistory = s.SerializePointer<IPT_EventHistoryElement[]>(EventHistory, name: nameof(EventHistory))?.ResolveObjectArray(s, EventHistorySize);

			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.RedPlanet)) {
				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					AtLeastOneActionIsValidated = s.Serialize<bool>(AtLeastOneActionIsValidated, name: nameof(AtLeastOneActionIsValidated));

				LineCommand = s.SerializeString(LineCommand, length: StringLength, name: nameof(LineCommand));
				InternLineCommand = s.SerializeString(InternLineCommand, length: StringLength, name: nameof(InternLineCommand));
				SearchedLineCommand = s.SerializeString(SearchedLineCommand, length: StringLength, name: nameof(SearchedLineCommand));
				LastLineCommand = s.SerializeStringArray(LastLineCommand, 10, length: StringLength, name: nameof(LastLineCommand));
				s.Align(4, Offset);
				IndexInHistoryCommand = s.Serialize<uint>(IndexInHistoryCommand, name: nameof(IndexInHistoryCommand));
				SearchedCommand = s.SerializePointer<IPT_CommandElement>(SearchedCommand, name: nameof(SearchedCommand))?.ResolveObject(s);
				SwapCommandMode = s.SerializePointer<IPT_EntryElement>(SwapCommandMode, name: nameof(SwapCommandMode))?.ResolveObject(s);

				if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3))
					ClearCommandMode = s.SerializePointer<IPT_EntryElement>(ClearCommandMode, name: nameof(ClearCommandMode))?.ResolveObject(s);
			}
		}
	}
}
