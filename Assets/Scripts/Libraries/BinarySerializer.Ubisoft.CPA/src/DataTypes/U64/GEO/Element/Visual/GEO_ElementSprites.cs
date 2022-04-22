using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GEO_ElementSprites : U64_Struct {
		public MTH2D_Vector SizeOfSprite { get; set; }
		public U64_Reference<GLI_VisualMaterial> VisualMaterial { get; set; }
		public ushort CenterPointIndex { get; set; }
		public Type TypeOfSprite { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SizeOfSprite = s.SerializeObject<MTH2D_Vector>(SizeOfSprite, name: nameof(SizeOfSprite));
			VisualMaterial = s.SerializeObject<U64_Reference<GLI_VisualMaterial>>(VisualMaterial, name: nameof(VisualMaterial))?.Resolve(s);
			CenterPointIndex = s.Serialize<ushort>(CenterPointIndex, name: nameof(CenterPointIndex));
			TypeOfSprite = s.Serialize<Type>(TypeOfSprite, name: nameof(TypeOfSprite));
		}

		[Flags]
		public enum Type : ushort {
			None = 0,
			Scaled2D = 1,
			SemiLookAt = 2,
			UseMatrix = 8
		}
	}

}
