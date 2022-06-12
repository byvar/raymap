using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class MEC_MechanicalMaterial : U64_Struct {
		/*public float Adhesion { get; set; }
		public float Absorption { get; set; }
		public float Friction { get; set; }
		public float Slide { get; set; }
		public float Progression { get; set; }
		public float PenetrationSpeed { get; set; }
		public float PenetrationMax { get; set; }*/

		public float BaseSlide { get; set; }
		public float BaseRebound { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			/*Adhesion = s.Serialize<float>(Adhesion, name: nameof(Adhesion));
			Absorption = s.Serialize<float>(Absorption, name: nameof(Absorption));
			Friction = s.Serialize<float>(Friction, name: nameof(Friction));
			Slide = s.Serialize<float>(Slide, name: nameof(Slide));
			Progression = s.Serialize<float>(Progression, name: nameof(Progression));
			PenetrationSpeed = s.Serialize<float>(PenetrationSpeed, name: nameof(PenetrationSpeed));
			PenetrationMax = s.Serialize<float>(PenetrationMax, name: nameof(PenetrationMax));*/

			BaseSlide = s.Serialize<float>(BaseSlide, name: nameof(BaseSlide));
			BaseRebound = s.Serialize<float>(BaseRebound, name: nameof(BaseRebound));
		}
	}
}
