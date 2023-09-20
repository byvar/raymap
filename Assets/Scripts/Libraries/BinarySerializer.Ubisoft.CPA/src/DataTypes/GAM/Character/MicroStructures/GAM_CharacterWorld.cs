namespace BinarySerializer.Ubisoft.CPA {
	public class GAM_CharacterWorld : BinarySerializable {
		public int SnowForce { get; set; }
		public int SnowForceInit { get; set; }
		public int RainForce { get; set; }
		public int RainForceInit { get; set; }
		public MTH3D_Vector WindVector { get; set; }
		public MTH3D_Vector WindVectorInit { get; set; }
		public MTH3D_Vector AirFogAttributes { get; set; }
		public MTH3D_Vector WaterFogAttributes { get; set; }
		public GLI_FloatColor_RGBA AirFogColor { get; set; }
		public GLI_FloatColor_RGBA WaterFogColor { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			SnowForce = s.Serialize<int>(SnowForce, name: nameof(SnowForce));
			SnowForceInit = s.Serialize<int>(SnowForceInit, name: nameof(SnowForceInit));
			RainForce = s.Serialize<int>(RainForce, name: nameof(RainForce));
			RainForceInit = s.Serialize<int>(RainForceInit, name: nameof(RainForceInit));
			WindVector = s.SerializeObject<MTH3D_Vector>(WindVector, name: nameof(WindVector));
			WindVectorInit = s.SerializeObject<MTH3D_Vector>(WindVectorInit, name: nameof(WindVectorInit));
			AirFogAttributes = s.SerializeObject<MTH3D_Vector>(AirFogAttributes, name: nameof(AirFogAttributes));
			WaterFogAttributes = s.SerializeObject<MTH3D_Vector>(WaterFogAttributes, name: nameof(WaterFogAttributes));
			AirFogColor = s.SerializeObject<GLI_FloatColor_RGBA>(AirFogColor, name: nameof(AirFogColor));
			WaterFogAttributes = s.SerializeObject<GLI_FloatColor_RGBA>(WaterFogAttributes, name: nameof(WaterFogAttributes));
		}
	}
}
