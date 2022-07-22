﻿namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_Family : BinarySerializable, ILST2_DynamicEntry<GAM_Family> {
		public LST2_DynamicListElement<GAM_Family> ListElement { get; set; }
		public int ObjectFamilyType { get; set; }
		public LST2_StaticList<GAM_State> States { get; set; }

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

			throw new BinarySerializableException(this, $"{GetType()}: Not yet implemented");
		}
	}
}