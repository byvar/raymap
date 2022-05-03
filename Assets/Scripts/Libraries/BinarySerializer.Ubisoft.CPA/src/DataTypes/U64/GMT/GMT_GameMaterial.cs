using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GMT_GameMaterial : U64_Struct {
		public U64_Reference<GLI_VisualMaterial> VisualMaterial { get; set; }
		public U64_Reference<U64_Placeholder> MechanicalMaterial { get; set; }
		public U64_Reference<GMT_CollideMaterial> CollideMaterial { get; set; }
		public U64_Reference<U64_Placeholder> SoundMaterial { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			VisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(VisualMaterial, name: nameof(VisualMaterial))?.Resolve(s);
			MechanicalMaterial = s.SerializeObject<U64_Reference<U64_Placeholder>>(MechanicalMaterial, name: nameof(MechanicalMaterial))?.Resolve(s);
			CollideMaterial = s.SerializeObject<U64_Reference<GMT_CollideMaterial>>(CollideMaterial, name: nameof(CollideMaterial))?.Resolve(s);
			SoundMaterial = s.SerializeObject<U64_Reference<U64_Placeholder>>(SoundMaterial, name: nameof(SoundMaterial))?.Resolve(s);
		}

	}

}
