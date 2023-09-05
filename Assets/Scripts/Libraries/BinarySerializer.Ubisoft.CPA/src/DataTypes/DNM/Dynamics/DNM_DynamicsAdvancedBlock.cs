namespace BinarySerializer.Ubisoft.CPA {
	public class DNM_DynamicsAdvancedBlock : BinarySerializable {
		public float InertiaX { get; set; }
		public float InertiaY { get; set; }
		public float InertiaZ { get; set; }

		public float StreamPriority { get; set; }
		public float StreamFactor { get; set; }

		public float SlideFactorX { get; set; }
		public float SlideFactorY { get; set; }
		public float SlideFactorZ { get; set; }
		public float PreviousSlide { get; set; }

		public MTH3D_Vector MaxSpeed { get; set; }
		public MTH3D_Vector StreamSpeed { get; set; }
		public MTH3D_Vector AddSpeed { get; set; }
		
		public MTH3D_Vector Limit { get; set; }

		public MTH3D_Vector CollisionTranslation { get; set; }
		public MTH3D_Vector InertiaTranslation { get; set; }

		public MTH3D_Vector GroundNormal { get; set; }
		public MTH3D_Vector WallNormal { get; set; }

		public sbyte CollideCounter { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			InertiaX = s.Serialize<float>(InertiaX, name: nameof(InertiaX));
			InertiaY = s.Serialize<float>(InertiaY, name: nameof(InertiaY));
			InertiaZ = s.Serialize<float>(InertiaZ, name: nameof(InertiaZ));

			StreamPriority = s.Serialize<float>(StreamPriority, name: nameof(StreamPriority));
			StreamFactor = s.Serialize<float>(StreamFactor, name: nameof(StreamFactor));

			SlideFactorX = s.Serialize<float>(SlideFactorX, name: nameof(SlideFactorX));
			SlideFactorY = s.Serialize<float>(SlideFactorY, name: nameof(SlideFactorY));
			SlideFactorZ = s.Serialize<float>(SlideFactorZ, name: nameof(SlideFactorZ));
			PreviousSlide = s.Serialize<float>(PreviousSlide, name: nameof(PreviousSlide));

			MaxSpeed = s.SerializeObject<MTH3D_Vector>(MaxSpeed, name: nameof(MaxSpeed));
			StreamSpeed = s.SerializeObject<MTH3D_Vector>(StreamSpeed, name: nameof(StreamSpeed));
			AddSpeed = s.SerializeObject<MTH3D_Vector>(AddSpeed, name: nameof(AddSpeed));

			Limit = s.SerializeObject<MTH3D_Vector>(Limit, name: nameof(Limit));

			CollisionTranslation = s.SerializeObject<MTH3D_Vector>(CollisionTranslation, name: nameof(CollisionTranslation));
			InertiaTranslation = s.SerializeObject<MTH3D_Vector>(InertiaTranslation, name: nameof(InertiaTranslation));

			GroundNormal = s.SerializeObject<MTH3D_Vector>(GroundNormal, name: nameof(GroundNormal));
			WallNormal = s.SerializeObject<MTH3D_Vector>(WallNormal, name: nameof(WallNormal));

			CollideCounter = s.Serialize<sbyte>(CollideCounter, name: nameof(CollideCounter));
			s.Align(4, Offset);
		}
	}
}
