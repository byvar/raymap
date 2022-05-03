using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_ZoneSet : U64_Struct { // State - Activation zone pair
		public U64_Reference<GAM_ActivationZone> ActivationZone { get; set; }
		public U64_Reference<GAM_State> State { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ActivationZone = s.SerializeObject<U64_Reference<GAM_ActivationZone>>(ActivationZone, name: nameof(ActivationZone))?.Resolve(s);
			State = s.SerializeObject<U64_Reference<GAM_State>>(State, name: nameof(State))?.Resolve(s);
		}
	}
}
