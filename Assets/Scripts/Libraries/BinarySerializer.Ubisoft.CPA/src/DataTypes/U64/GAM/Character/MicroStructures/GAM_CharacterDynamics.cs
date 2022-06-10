using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterDynamics : U64_Struct {
		public MTH3D_Vector SlideFactor { get; set; }
		public DynamFlags Flags { get; set; } // Saved, but not actually loaded by the DS engine!
		public StructSize DynamSize { get; set; }
		public bool Collision { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SlideFactor = s.SerializeObject<MTH3D_Vector>(SlideFactor, name: nameof(SlideFactor));
			Flags = s.Serialize<DynamFlags>(Flags, name: nameof(Flags));
			DynamSize = s.Serialize<StructSize>(DynamSize, name: nameof(DynamSize));
			Collision = s.Serialize<bool>(Collision, name: nameof(Collision));
		}

		public enum StructSize : byte {
			Base = 0,
			Advanced = 1,
			Complex = 2
		}

		[Flags]
		public enum DynamFlags : ushort {
			Unknown                  = 0,
			Solid                    = 1 << 0,
			Liquid                   = 1 << 1,
			Gas                      = 1 << 2,
			Plasma                   = 1 << 3,
			IsMobile                 = 1 << 4,
			CanHangSmthgOn           = 1 << 5,
			TakeCareOfTheEnvironment = 1 << 6,
		}
	}
}
