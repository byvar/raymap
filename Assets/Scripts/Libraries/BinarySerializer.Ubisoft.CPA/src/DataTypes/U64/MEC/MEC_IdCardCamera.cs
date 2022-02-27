using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class MEC_IdCardCamera : U64_Struct {
        public float AngularSpeed { get; set; }
        public float LinearSpeed { get; set; }
        public float SpeedingUp { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            AngularSpeed = s.Serialize<float>(AngularSpeed, name: nameof(AngularSpeed));
            LinearSpeed = s.Serialize<float>(LinearSpeed, name: nameof(LinearSpeed));
            SpeedingUp = s.Serialize<float>(SpeedingUp, name: nameof(SpeedingUp));
        }
    }
}
