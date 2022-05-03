using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_StateRef : U64_Struct {
		public LST_ReferenceList<GAM_State> StateRefList { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StateRefList = s.SerializeObject<LST_ReferenceList<GAM_State>>(StateRefList, name: nameof(StateRefList))?.Resolve(s);
		}
	}
}
