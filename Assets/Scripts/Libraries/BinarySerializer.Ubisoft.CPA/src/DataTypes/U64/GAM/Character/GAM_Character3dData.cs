using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
	public class GAM_Character3dData : U64_Struct {
		public DrawMaskFlags DrawMask { get; set; }
		public U64_Vector2D ShadowScale { get; set; }
		public U64_Reference<GAM_State> InitialState { get; set; }
		public U64_Reference<GAM_ObjectsTable> InitialObjectsTable { get; set; }
		public byte BrainComputationFrequency { get; set; }
		public bool IsAlphabet { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			DrawMask = s.Serialize<DrawMaskFlags>(DrawMask, name: nameof(DrawMask));
			ShadowScale = s.SerializeObject<U64_Vector2D>(ShadowScale, name: nameof(ShadowScale));
			InitialState = s.SerializeObject<U64_Reference<GAM_State>>(InitialState, name: nameof(InitialState))?.Resolve(s);
			InitialObjectsTable = s.SerializeObject<U64_Reference<GAM_ObjectsTable>>(InitialObjectsTable, name: nameof(InitialObjectsTable))?.Resolve(s);
			BrainComputationFrequency = s.Serialize<byte>(BrainComputationFrequency, name: nameof(BrainComputationFrequency));
			IsAlphabet = s.Serialize<bool>(IsAlphabet, name: nameof(IsAlphabet));
			s.SerializePadding(2, logIfNotNull: true);
		}

		[Flags]
		public enum DrawMaskFlags : uint {
			None                                  = 0,
			IsTextured                            = 1,
			IsGouraud                             = 2, // else is flat
			IsNotWired                            = 4, // so is solid
			IsNotGrided                           = 8,
			IsNotDoted                            = 16, // means draw only the points
			IsNotOutlined                         = 32, // means draw only the points
			IsNotDrawCollideInformation           = 64, // means draw collision spheres & box
			IsNotDrawCollideInformationLight      = 128, // Draw only periferical
			IsNotForceDefaultMaterial             = 256, // Draw only periferical
			IsNotForceColorMaterial               = 512, // draw triangles with a single color set in the material
			IsTestingBackface                     = 1024,
			IsNotDrawingSuperObjectBoundingVolume = 2048,
			IsUseRLI                              = 4096,
			IsNotComputeSpecular                  = 8192,
			HasNotPriority                        = 16384, // 14th bit : Means this object has no priority
			IsUseStaticLights                     = 32768,
			IsUseShadow                           = 65536,
			CameraIsUnderWater                    = 131072,
			NotForceZSorting                      = 262144,
			NotInvertBackfaces                    = 524288,
			NotHideWhatIsUnderWater               = 0x100000,
			DrawNothing                           = 0x200000,
			IsNotChromed                          = 0x400000,
			IsNotVisibleInRealWorld               = 0x800000,
			IsNotVisibleInSymetricWorld           = 0x1000000,
			IsNotDrawingInMirror                  = 0x2000000,
			IsNotLightAlphaSensitive              = 0x4000000,
			IsWriteZBuffer                        = 0x8000000,
			HasNoMirror                           = 0x10000000,
			IsNotSinusEffectOnRLI                 = 0x20000000,
			IsEnableZSorting                      = 0x40000000,
			HasNotSinusEffect                     = 0x80000000,
			EnableAll                             = 0xFFFFFFFF
		}
	}
}
