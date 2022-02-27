using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_AnimationOffset : BinarySerializable
    {
        public uint AnimationOffset { get; set; }
        public bool IsCompressed { get; set; }

        public Pointer AnimationPointer(Pointer anchor) => anchor + AnimationOffset;

        public override void SerializeImpl(SerializerObject s)
        {
            s.DoBits<uint>(b => {
                AnimationOffset = b.SerializeBits<uint>(AnimationOffset, 31, name: nameof(AnimationOffset));
                IsCompressed = b.SerializeBits<bool>(IsCompressed, 1, name: nameof(IsCompressed));
            });
        }
    }
}