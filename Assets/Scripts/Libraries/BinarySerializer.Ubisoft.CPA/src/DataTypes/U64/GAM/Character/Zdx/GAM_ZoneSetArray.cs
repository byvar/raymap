using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ZoneSetArray : U64_Struct { // Contains state - activation zone pairs
		public LST_List<GAM_ZoneSet> ZoneSets { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ZoneSets = s.SerializeObject<LST_List<GAM_ZoneSet>>(ZoneSets, name: nameof(ZoneSets))?.Resolve(s);
		}
	}
}
