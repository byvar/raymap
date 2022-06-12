using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_SubLevel : U64_Struct {
		public U64_ArrayReference<LST_ReferenceElement<GAM_LevelEntry>> LevelEntryList { get; set; }
		public U64_Reference<HIE_SuperObject> SuperObjectRoot { get; set; }
		public U64_ArrayReference<POS_CompletePosition> StartMatrixList { get; set; }
		public ushort LevelEntryListCount { get; set; }
		public ushort StartMatrixListCount { get; set; }
		public U64_Index<U64_Placeholder> SubmapSoundEvent { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LevelEntryList = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<GAM_LevelEntry>>>(LevelEntryList, name: nameof(LevelEntryList));
			SuperObjectRoot = s.SerializeObject<U64_Reference<HIE_SuperObject>>(SuperObjectRoot, name: nameof(SuperObjectRoot))?.Resolve(s);
			StartMatrixList = s.SerializeObject<U64_ArrayReference<POS_CompletePosition>>(StartMatrixList, name: nameof(StartMatrixList));
			LevelEntryListCount = s.Serialize<ushort>(LevelEntryListCount, name: nameof(LevelEntryListCount));
			StartMatrixListCount = s.Serialize<ushort>(StartMatrixListCount, name: nameof(StartMatrixListCount));

			SubmapSoundEvent = s.SerializeObject<U64_Index<U64_Placeholder>>(SubmapSoundEvent, name: nameof(SubmapSoundEvent));

			LevelEntryList?.Resolve(s, LevelEntryListCount);
			StartMatrixList?.Resolve(s, StartMatrixListCount);
		}

	}
}
