using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class AI_Intelligence : U64_Struct {
		public U64_ArrayReference<LST_ReferenceElement<AI_Comport>> ComportList { get; set; }
		public U64_Reference<AI_Comport> ComportInit { get; set; }
		public ushort ComportCount { get; set; }
		public ushort ActionTableEntriesCount { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ComportList = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<AI_Comport>>>(ComportList, name: nameof(ComportList));
			ComportInit = s.SerializeObject<U64_Reference<AI_Comport>>(ComportInit, name: nameof(ComportInit))?.Resolve(s);
			ComportCount = s.Serialize<ushort>(ComportCount, name: nameof(ComportCount));
			ActionTableEntriesCount = s.Serialize<ushort>(ActionTableEntriesCount, name: nameof(ActionTableEntriesCount));

			ComportList?.Resolve(s, ComportCount);
		}
	}
}
