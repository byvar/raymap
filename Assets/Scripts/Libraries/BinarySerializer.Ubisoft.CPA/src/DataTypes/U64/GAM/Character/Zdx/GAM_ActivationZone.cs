using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ActivationZone : U64_Struct { // List of indices of Zdx to be activated when this zone is enabled
		public LST_List<GAM_ZdxIndex> ZdxIndices { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ZdxIndices = s.SerializeObject<LST_List<GAM_ZdxIndex>>(ZdxIndices, name: nameof(ZdxIndices))?.Resolve(s);
		}
	}
}
