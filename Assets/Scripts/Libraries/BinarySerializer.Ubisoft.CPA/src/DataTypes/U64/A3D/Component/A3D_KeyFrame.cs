using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_KeyFrame : BinarySerializable
    {
        public ushort FrameIndex { get; set; }
        public ushort Mask { get; set; }

        public ushort QuaternionOrientationIndex { get; set; }
        public ushort QuaternionScaleIndex { get; set; }
        public ushort VectorScaleIndex { get; set; }
        public ushort VectorPositionIndex { get; set; }

        public short InterpolationParameter { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            FrameIndex = s.Serialize<ushort>(FrameIndex, name: nameof(FrameIndex));
            Mask = s.Serialize<ushort>(Mask, name: nameof(Mask));

            QuaternionOrientationIndex = s.Serialize<ushort>(QuaternionOrientationIndex, name: nameof(QuaternionOrientationIndex));
            QuaternionScaleIndex = s.Serialize<ushort>(QuaternionScaleIndex, name: nameof(QuaternionScaleIndex));
            VectorScaleIndex = s.Serialize<ushort>(VectorScaleIndex, name: nameof(VectorScaleIndex));
            VectorPositionIndex = s.Serialize<ushort>(VectorPositionIndex, name: nameof(VectorPositionIndex));

            InterpolationParameter = s.Serialize<short>(InterpolationParameter, name: nameof(InterpolationParameter));
        }
    }
}