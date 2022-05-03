using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterCollSet : U64_Struct {
		public U64_Reference<GAM_ZdxArray>[] ZdxTables { get; set; }
		public U64_Reference<GAM_ZoneSetArray>[] ActivationTables { get; set; }
		public short CharacterPriority { get; set; }
		public byte CollisionFlag { get; set; }
		public byte CollisionComputeFrequency { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ZdxTables = s.SerializeObjectArray<U64_Reference<GAM_ZdxArray>>(ZdxTables, 4, name: nameof(ZdxTables));
			foreach(var zdx in ZdxTables) zdx?.Resolve(s);
			ActivationTables = s.SerializeObjectArray<U64_Reference<GAM_ZoneSetArray>>(ActivationTables, 4, name: nameof(ActivationTables));
			foreach(var zdx in ActivationTables) zdx?.Resolve(s);

			CharacterPriority = s.Serialize<short>(CharacterPriority, name: nameof(CharacterPriority));
			CollisionFlag = s.Serialize<byte>(CollisionFlag, name: nameof(CollisionFlag));
			CollisionComputeFrequency = s.Serialize<byte>(CollisionComputeFrequency, name: nameof(CollisionComputeFrequency));
		}

		public enum ZoneType : byte {
			Zdd = 0,
			Zdm = 1,
			Zde = 2,
			Zdr = 3
		}
	}
}
