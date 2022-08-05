namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_Family : BinarySerializable, ILST2_DynamicEntry<GAM_Family> {
		public LST2_DynamicListElement<GAM_Family> ListElement { get; set; }
		public int ObjectFamilyType { get; set; }
		public LST2_StaticList<GAM_State> States { get; set; }
		public LST2_StaticList<GAM_SubAnimation> SubAnimations { get; set; }
		public Pointer<GAM_ObjectsTable> DefaultObjectsTable { get; set; }
		public LST2_StaticList<GAM_ObjectsTable> ObjectsTables { get; set; }
		public Pointer<COL_BoundingSphere> BoundingSphere { get; set; }

		// LipsSynch
		public Pointer<LS_LipsSynchValue[]> LipsSynchValues { get; set; }
		public uint LipsSynchValuesCount { get; set; }
		public uint LipsChannel { get; set; }

		public uint ChannelsCount { get; set; }
		public GAM_StaticBlocks Priority { get; set; }
		public bool Optimized { get; set; }
		public byte StorageBank { get; set; } // Where animations are stored
		public byte Properties { get; set; }

		// LST2 Implementation
		public Pointer<LST2_DynamicList<GAM_Family>> LST2_Parent => ((ILST2_DynamicEntry<GAM_Family>)ListElement).LST2_Parent;
		public Pointer<GAM_Family> LST2_Next => ((ILST2_Entry<GAM_Family>)ListElement).LST2_Next;
		public Pointer<GAM_Family> LST2_Previous => ((ILST2_Entry<GAM_Family>)ListElement).LST2_Previous;

		public override void SerializeImpl(SerializerObject s) {
			ListElement = s.SerializeObject<LST2_DynamicListElement<GAM_Family>>(ListElement, name: nameof(ListElement));
			if (!s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.Rayman2Revolution)) {
				ObjectFamilyType = s.Serialize<int>(ObjectFamilyType, name: nameof(ObjectFamilyType));
			}
			States = s.SerializeObject<LST2_StaticList<GAM_State>>(States, name: nameof(States))
				?.Resolve(s, name: nameof(States));

			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				SubAnimations = s.SerializeObject<LST2_StaticList<GAM_SubAnimation>>(SubAnimations, name: nameof(SubAnimations))
					?.Resolve(s, name: nameof(SubAnimations));
			}
			DefaultObjectsTable = s.SerializePointer<GAM_ObjectsTable>(DefaultObjectsTable, name: nameof(DefaultObjectsTable))?.ResolveObject(s);
			ObjectsTables = s.SerializeObject<LST2_StaticList<GAM_ObjectsTable>>(ObjectsTables, name: nameof(ObjectsTables))
				?.Resolve(s, name: nameof(ObjectsTables));

			// TODO: Correct ObjectsTables count
			/*
			 *  if ((Legacy_Settings.s.mode == Legacy_Settings.Mode.Rayman3PS2DevBuild_2002_09_06
                || f.objectLists.off_head == f.objectLists.off_tail)
                && f.objectLists.Count > 1) f.objectLists.Count = 1; // Correction for Rayman 2
			 * */

			BoundingSphere = s.SerializePointer<COL_BoundingSphere>(BoundingSphere, name: nameof(BoundingSphere))?.ResolveObject(s);

			if (s.GetCPASettings().EngineVersion == EngineVersion.Rayman3) {
				// LipsSynch
				LipsSynchValues = s.SerializePointer<LS_LipsSynchValue[]>(LipsSynchValues, name: nameof(LipsSynchValues));
				LipsSynchValuesCount = s.Serialize<uint>(LipsSynchValuesCount, name: nameof(LipsSynchValuesCount));
				LipsChannel = s.Serialize<uint>(LipsChannel, name: nameof(LipsChannel));
				LipsSynchValues?.ResolveObjectArray(s, LipsSynchValuesCount);
			}

			ChannelsCount = s.Serialize<uint>(ChannelsCount, name: nameof(ChannelsCount));
			Priority = s.Serialize<GAM_StaticBlocks>(Priority, name: nameof(Priority));
			Optimized = s.Serialize<bool>(Optimized, name: nameof(Optimized));
			if (s.GetCPASettings().EngineVersionTree.HasParent(EngineVersion.CPA_3)) {
				StorageBank = s.Serialize<byte>(StorageBank, name: nameof(StorageBank));
				Properties = s.Serialize<byte>(Properties, name: nameof(Properties));
			}
			s.Align(4, Offset);
		}
	}
}
