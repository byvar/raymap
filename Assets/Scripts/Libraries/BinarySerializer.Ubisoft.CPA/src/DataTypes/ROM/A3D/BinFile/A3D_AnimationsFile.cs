using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class A3D_AnimationsFile : BinarySerializable
    {
        public uint AnimationsCount { get; set; }
        public uint IntAnimationsCount { get; set; }
        public uint MaxRamSizeOfAnim { get; set; }
        public uint MaxRomSizeOfAnim { get; set; }
        public A3D_AnimationOffset[] AnimationOffsets { get; set; }
        public Pointer EndOfFile { get; set; }


        public override void SerializeImpl(SerializerObject s)
        {
			AnimationsCount = s.Serialize<uint>(AnimationsCount, name: nameof(AnimationsCount));
			IntAnimationsCount = s.Serialize<uint>(IntAnimationsCount, name: nameof(IntAnimationsCount));
			MaxRamSizeOfAnim = s.Serialize<uint>(MaxRamSizeOfAnim, name: nameof(MaxRamSizeOfAnim));
			MaxRomSizeOfAnim = s.Serialize<uint>(MaxRomSizeOfAnim, name: nameof(MaxRomSizeOfAnim));
			AnimationOffsets = s.SerializeObjectArray<A3D_AnimationOffset>(AnimationOffsets, AnimationsCount, name: nameof(AnimationOffsets));
            EndOfFile = s.SerializePointer(EndOfFile, anchor: Offset, name: nameof(EndOfFile));
		}

        public void LoadAnimation(SerializerObject s, int index) {
            var offset = AnimationOffsets[index];
            var ptr = Offset + offset.AnimationOffset;

            // Calculate size
            long size;
            if (index + 1 < AnimationsCount) {
                size = AnimationOffsets[index+1].AnimationOffset - AnimationOffsets[index].AnimationOffset;
            } else {
                size = EndOfFile - ptr;
            }


        }
    }
}