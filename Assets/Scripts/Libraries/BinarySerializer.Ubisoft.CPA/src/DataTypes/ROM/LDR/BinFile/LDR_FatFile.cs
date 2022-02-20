using System;

namespace BinarySerializer.Ubisoft.CPA.ROM
{
    public class LDR_FatFile : BinarySerializable
    {
        public Pointer Pre_DataPointer { get; set; }

        public uint LevelsCount { get; set; }
        public LDR_Fat FixFix { get; set; }
        public LDR_Fat FixLevels { get; set; }
        public LDR_Fat[] Levels { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Context.StoreObject<LDR_FatFile>(ContextKey, this);

			LevelsCount = s.Serialize<uint>(LevelsCount, name: nameof(LevelsCount));
			FixFix = s.SerializeObject<LDR_Fat>(FixFix, name: nameof(FixFix));
			FixLevels = s.SerializeObject<LDR_Fat>(FixLevels, name: nameof(FixLevels));
			Levels = s.SerializeObjectArray<LDR_Fat>(Levels, LevelsCount, name: nameof(Levels));

            FixFix.SerializeFat(s);
            FixLevels.SerializeFat(s);
            // TODO: Serialize level Fat

		}

        public static string ContextKey => nameof(LDR_FatFile);
    }
}