using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_CharacterMicro : U64_Struct {
		public MTH3D_Matrix Rotation { get; set; }
		public U64_Index<MTH3D_Vector> Translation { get; set; }
		public MicroFlags Flags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Rotation = s.SerializeObject<MTH3D_Matrix>(Rotation, name: nameof(Rotation));
			Translation = s.SerializeObject<U64_Index<MTH3D_Vector>>(Translation, name: nameof(Translation))?.SetAction(GAM_Fix.GetVector3DIndex);
			Flags = s.Serialize<MicroFlags>(Flags, name: nameof(Flags));
		}

		[Flags]
		public enum MicroFlags : ushort {
			None                                  = 0,
			IsActive                              = (1 << 0),
		}
	}
}
