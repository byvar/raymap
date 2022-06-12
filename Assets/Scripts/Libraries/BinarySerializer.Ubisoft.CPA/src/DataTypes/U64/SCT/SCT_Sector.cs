using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class SCT_Sector : U64_Struct {
		// Neighbors
		public U64_ArrayReference<SCT_SectorGraphic> Graphic { get; set; }
		public U64_ArrayReference<SCT_SectorGraphicParam> GraphicParam { get; set; }
		public U64_ArrayReference<SCT_SectorActivity> Activity { get; set; }
		public U64_ArrayReference<SCT_SectorCollision> Collision { get; set; }
		public U64_ArrayReference<LST_ReferenceElement<GLI_Light>> StaticLights { get; set; }
		public U64_ArrayReference<SCT_SectorSound> Sound { get; set; }
		public U64_ArrayReference<SCT_SectorSoundParam> SoundParam { get; set; }
		public U64_ArrayReference<SCT_SectorSoundEvent> SoundEvent { get; set; }
		public U64_ArrayReference<SCT_SectorSoundEventParam> SoundEventParam { get; set; }

		public U64_BoundingVolumeBox BorderBox { get; set; }
		public SCT_Sky Sky { get; set; }

		public ushort GraphicCount { get; set; }
		public ushort ActivityCount { get; set; }
		public ushort CollisionCount { get; set; }
		public ushort StaticLightsCount { get; set; }
		public ushort SoundCount { get; set; }
		public ushort SoundEventCount { get; set; }

		public bool Virtual { get; set; }
		public byte CameraType { get; set; }
		public short ZFar { get; set; } // * MapScale
		public byte Priority { get; set; }

		public byte Pad1 { get; set; } // Padding, but filled with garbage data
		public short Pad2 { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			Graphic = s.SerializeObject<U64_ArrayReference<SCT_SectorGraphic>>(Graphic, name: nameof(Graphic));
			GraphicParam = s.SerializeObject<U64_ArrayReference<SCT_SectorGraphicParam>>(GraphicParam, name: nameof(GraphicParam));
			Activity = s.SerializeObject<U64_ArrayReference<SCT_SectorActivity>>(Activity, name: nameof(Activity));
			Collision = s.SerializeObject<U64_ArrayReference<SCT_SectorCollision>>(Collision, name: nameof(Collision));
			StaticLights = s.SerializeObject<U64_ArrayReference<LST_ReferenceElement<GLI_Light>>>(StaticLights, name: nameof(StaticLights));
			Sound = s.SerializeObject<U64_ArrayReference<SCT_SectorSound>>(Sound, name: nameof(Sound));
			SoundParam = s.SerializeObject<U64_ArrayReference<SCT_SectorSoundParam>>(SoundParam, name: nameof(SoundParam));
			SoundEvent = s.SerializeObject<U64_ArrayReference<SCT_SectorSoundEvent>>(SoundEvent, name: nameof(SoundEvent));
			SoundEventParam = s.SerializeObject<U64_ArrayReference<SCT_SectorSoundEventParam>>(SoundEventParam, name: nameof(SoundEventParam));

			BorderBox = s.SerializeObject<U64_BoundingVolumeBox>(BorderBox, name: nameof(BorderBox));
			Sky = s.SerializeObject<SCT_Sky>(Sky, name: nameof(Sky));

			GraphicCount = s.Serialize<ushort>(GraphicCount, name: nameof(GraphicCount));
			ActivityCount = s.Serialize<ushort>(ActivityCount, name: nameof(ActivityCount));
			CollisionCount = s.Serialize<ushort>(CollisionCount, name: nameof(CollisionCount));
			StaticLightsCount = s.Serialize<ushort>(StaticLightsCount, name: nameof(StaticLightsCount));
			SoundCount = s.Serialize<ushort>(SoundCount, name: nameof(SoundCount));
			SoundEventCount = s.Serialize<ushort>(SoundEventCount, name: nameof(SoundEventCount));

			Virtual = s.Serialize<bool>(Virtual, name: nameof(Virtual));
			CameraType = s.Serialize<byte>(CameraType, name: nameof(CameraType));
			ZFar = s.Serialize<short>(ZFar, name: nameof(ZFar));
			Priority = s.Serialize<byte>(Priority, name: nameof(Priority));

			Pad1 = s.Serialize<byte>(Pad1, name: nameof(Pad1));
			Pad2 = s.Serialize<short>(Pad2, name: nameof(Pad2));

			// Resolve
			Graphic?.Resolve(s, GraphicCount);
			GraphicParam?.Resolve(s, GraphicCount);
			Activity?.Resolve(s, ActivityCount);
			Collision?.Resolve(s, CollisionCount);
			StaticLights?.Resolve(s, StaticLightsCount);
			Sound?.Resolve(s, SoundCount);
			SoundParam?.Resolve(s, SoundCount);
			SoundEvent?.Resolve(s, SoundEventCount);
			SoundEventParam?.Resolve(s,SoundEventCount);
		}

	}
}
