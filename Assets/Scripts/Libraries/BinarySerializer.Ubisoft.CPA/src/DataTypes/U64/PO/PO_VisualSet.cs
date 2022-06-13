using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	// VisualSets aren't used in R2 N64
	public class PO_VisualSet : U64_Struct {
		public LST_List<PO_LevelOfDetail> LODList { get; set; }
		public U64_Reference<U64_Placeholder> RLI { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LODList = s.SerializeObject<LST_List<PO_LevelOfDetail>>(LODList, name: nameof(LODList))?.Resolve(s);
			RLI = s.SerializeObject<U64_Reference<U64_Placeholder>>(RLI, name: nameof(RLI))?.Resolve(s);
		}
	}
}
