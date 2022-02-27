using System;

namespace BinarySerializer.Ubisoft.CPA.U64
{
    public class A3D_AnimationsFile : BinarySerializable
    {
        public uint AnimationsCount { get; set; }
        public uint IntAnimationsCount { get; set; }
        public uint MaxRamSizeOfAnim { get; set; }
        public uint MaxRomSizeOfAnim { get; set; }
        public A3D_AnimationOffset[] AnimationOffsets { get; set; }
        public Pointer EndOfFile { get; set; }

        // Parsed
        public A3D_Animation[] Animations { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));
            IntAnimationsCount = s.Serialize<uint>(IntAnimationsCount, name: nameof(IntAnimationsCount));
            MaxRamSizeOfAnim = s.Serialize<uint>(MaxRamSizeOfAnim, name: nameof(MaxRamSizeOfAnim));
            MaxRomSizeOfAnim = s.Serialize<uint>(MaxRomSizeOfAnim, name: nameof(MaxRomSizeOfAnim));
            AnimationOffsets = s.SerializeObjectArray<A3D_AnimationOffset>(AnimationOffsets, AnimationsCount, name: nameof(AnimationOffsets));
            EndOfFile = s.SerializePointer(EndOfFile, anchor: Offset, name: nameof(EndOfFile));
        }

        public A3D_Animation LoadAnimation(SerializerObject s, int index) {
            var offset = AnimationOffsets[index];
            var ptr = Offset + offset.AnimationOffset;

            if(Animations == null) Animations = new A3D_Animation[AnimationsCount];
            if (Animations[index] == null) {
                // Calculate size
                long size;
                if (index + 1 < AnimationsCount) {
                    size = AnimationOffsets[index + 1].AnimationOffset - AnimationOffsets[index].AnimationOffset;
                } else {
                    size = EndOfFile - ptr;
                }
                if (size == 0) return null;

                s.DoAt(ptr, () => {
                    if (offset.IsCompressed) {
                        s.DoEncoded(new YAY0Encoder(), () => {
                            Animations[index] = s.SerializeObject<A3D_Animation>(Animations[index], onPreSerialize: a => a.Pre_DataSize = s.CurrentLength, name: $"{nameof(Animations)}[{index}]");
                        });
                    } else {
                        Animations[index] = s.SerializeObject<A3D_Animation>(Animations[index], onPreSerialize: a => a.Pre_DataSize = size, name: $"{nameof(Animations)}[{index}]");
                    }
                });
            }
            return Animations[index];
        }
    }
}