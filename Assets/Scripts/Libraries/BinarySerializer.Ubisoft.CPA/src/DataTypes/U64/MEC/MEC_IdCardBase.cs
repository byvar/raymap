using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class MEC_IdCardBase : U64_Struct {
        public float Gravity { get; set; }
        public float Rebound { get; set; }
        public float Slide { get; set; }
        public float SlopeLimit { get; set; }
        public float TiltIntensity { get; set; }
        public float TiltInertia { get; set; }
        public float TiltOrigin { get; set; }
        public U64_Index<U64_Vector3D> Inertia { get; set; } // Index in AllVector3D table
        public U64_Index<U64_Vector3D> MaxSpeed { get; set; } // Index in AllVector3D table
        public Flags MiscFlags { get; set; }
        public ushort Swim { get; set; } // Just 1 flag that didn't fit in MiscFlags, it seems

        public override void SerializeImpl(SerializerObject s) {
            Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
            Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));
            Slide = s.Serialize<float>(Slide, name: nameof(Slide));
            SlopeLimit = s.Serialize<float>(SlopeLimit, name: nameof(SlopeLimit));
            TiltIntensity = s.Serialize<float>(TiltIntensity, name: nameof(TiltIntensity));
            TiltInertia = s.Serialize<float>(TiltInertia, name: nameof(TiltInertia));
            TiltOrigin = s.Serialize<float>(TiltOrigin, name: nameof(TiltOrigin));

            Inertia = s.SerializeObject<U64_Index<U64_Vector3D>>(Inertia, name: nameof(Inertia))?.SetAction(GAM_Fix.GetGlobalVector3DIndex);
            MaxSpeed = s.SerializeObject<U64_Index<U64_Vector3D>>(MaxSpeed, name: nameof(MaxSpeed))?.SetAction(GAM_Fix.GetGlobalVector3DIndex);

            MiscFlags = s.Serialize<Flags>(MiscFlags, name: nameof(MiscFlags));
            Swim = s.Serialize<ushort>(Swim, name: nameof(Swim));
        }

        [Flags]
        public enum Flags : ushort {
            None             = 0,
            Animation        = 0x0001,
            Collide          = 0x0002,
            Gravity          = 0x0004,
            Tilt             = 0x0008,
            Gi               = 0x0010,
            Climb            = 0x0020,
            OnGround         = 0x0040,
            CollisionControl = 0x0080,
            KeepSpeedZ       = 0x0100,
            SpeedLimit       = 0x0200,
            Inertia          = 0x0400,
            Stream           = 0x0800,
            StickOnPlatform  = 0x1000,
            Spider           = 0x2000,
            Shoot            = 0x4000,
            Scale            = 0x8000
        }
    }
}
