using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterDynamics : U64_Struct {
		public MTH3D_Vector SlideFactor { get; set; }
		public ushort Flags { get; set; }
		public StructSize DynamSize { get; set; }
		public bool Collision { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SlideFactor = s.SerializeObject<MTH3D_Vector>(SlideFactor, name: nameof(SlideFactor));
			Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
			DynamSize = s.Serialize<StructSize>(DynamSize, name: nameof(DynamSize));
			Collision = s.Serialize<bool>(Collision, name: nameof(Collision));
		}

		public enum StructSize : byte {
			Base = 0,
			Advanced = 1,
			Complex = 2
		}
	}
}
