using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class U64_ShortQuaternion : BinarySerializable
    {
        public short X { get; set; } // Divide by Int16.Max to get float
        public short Y { get; set; }
        public short Z { get; set; }
        public short W { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<short>(X, name: nameof(X));
            Y = s.Serialize<short>(Y, name: nameof(Y));
            Z = s.Serialize<short>(Z, name: nameof(Z));
            W = s.Serialize<short>(W, name: nameof(W));
        }
        public override bool UseShortLog => true;
        public override string ToString() => $"ShortQuaternion({X}, {Y}, {Z}, {W})";
    }
}