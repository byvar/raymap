using System;

namespace BinarySerializer.Ubisoft.CPA {
    public class CPA_Vector3D : BinarySerializable {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));
        }
        public override bool UseShortLog => true;
        public override string ToString() => $"Vector({X}, {Y}, {Z})";
    }
}
