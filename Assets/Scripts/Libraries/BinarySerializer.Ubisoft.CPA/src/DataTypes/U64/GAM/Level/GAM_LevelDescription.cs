using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_LevelDescription : U64_Struct {
		public U64_Reference<GAM_DscLevel> LevelDescription { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LevelDescription = s.SerializeObject<U64_Reference<GAM_DscLevel>>(LevelDescription, name: nameof(LevelDescription))?.Resolve(s);
		}
	}
}
