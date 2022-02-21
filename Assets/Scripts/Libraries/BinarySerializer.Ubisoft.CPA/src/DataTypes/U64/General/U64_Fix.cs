using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class U64_Fix : U64_Struct {
		public U64_Reference<U64_Placeholder> LevelsNameList { get; set; }
		public U64_Reference<U64_Placeholder> DscInput { get; set; }
		public U64_Reference<U64_Placeholder> InputLink { get; set; }
		public U64_Reference<U64_Placeholder> Vector3D { get; set; }
		public U64_Reference<U64_Placeholder> TripledIndex { get; set; }
		public U64_Reference<U64_Placeholder> Memory { get; set; }
		public U64_Reference<U64_Placeholder> ShadowTexture { get; set; }
		public U64_Reference<U64_Placeholder> LightTexture { get; set; }
		public U64_Reference<U64_Placeholder> VisualMaterialController { get; set; }
		public U64_Reference<U64_Placeholder> NoCtrlList { get; set; }

		public ushort Vector3DCount { get; set; }
		public ushort TripledIndexCount { get; set; }
		public ushort LevelsCount { get; set; }

		public U64_Reference<U64_Placeholder> GlobalVector3D { get; set; }
		public U64_Reference<U64_Placeholder> GlobalTripledIndex { get; set; }
		public ushort GlobalVector3DCount { get; set; }
		public ushort GlobalTripledIndexCount { get; set; }

		public U64_Reference<U64_Placeholder> PakFont { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LevelsNameList = s.SerializeObject<U64_Reference<U64_Placeholder>>(LevelsNameList, name: nameof(LevelsNameList));
			DscInput = s.SerializeObject<U64_Reference<U64_Placeholder>>(DscInput, name: nameof(DscInput));
			InputLink = s.SerializeObject<U64_Reference<U64_Placeholder>>(InputLink, name: nameof(InputLink));
			Vector3D = s.SerializeObject<U64_Reference<U64_Placeholder>>(Vector3D, name: nameof(Vector3D));
			TripledIndex = s.SerializeObject<U64_Reference<U64_Placeholder>>(TripledIndex, name: nameof(TripledIndex));
			Memory = s.SerializeObject<U64_Reference<U64_Placeholder>>(Memory, name: nameof(Memory));
			ShadowTexture = s.SerializeObject<U64_Reference<U64_Placeholder>>(ShadowTexture, name: nameof(ShadowTexture));
			LightTexture = s.SerializeObject<U64_Reference<U64_Placeholder>>(LightTexture, name: nameof(LightTexture));
			VisualMaterialController = s.SerializeObject<U64_Reference<U64_Placeholder>>(VisualMaterialController, name: nameof(VisualMaterialController));
			NoCtrlList = s.SerializeObject<U64_Reference<U64_Placeholder>>(NoCtrlList, name: nameof(NoCtrlList));

			Vector3DCount = s.Serialize<ushort>(Vector3DCount, name: nameof(Vector3DCount));
			TripledIndexCount = s.Serialize<ushort>(TripledIndexCount, name: nameof(TripledIndexCount));
			LevelsCount = s.Serialize<ushort>(LevelsCount, name: nameof(LevelsCount));

			GlobalVector3D = s.SerializeObject<U64_Reference<U64_Placeholder>>(GlobalVector3D, name: nameof(GlobalVector3D));
			GlobalTripledIndex = s.SerializeObject<U64_Reference<U64_Placeholder>>(GlobalTripledIndex, name: nameof(GlobalTripledIndex));
			GlobalVector3DCount = s.Serialize<ushort>(GlobalVector3DCount, name: nameof(GlobalVector3DCount));
			GlobalTripledIndexCount = s.Serialize<ushort>(GlobalTripledIndexCount, name: nameof(GlobalTripledIndexCount));

			PakFont = s.SerializeObject<U64_Reference<U64_Placeholder>>(PakFont, name: nameof(PakFont));


		}
	}
}
