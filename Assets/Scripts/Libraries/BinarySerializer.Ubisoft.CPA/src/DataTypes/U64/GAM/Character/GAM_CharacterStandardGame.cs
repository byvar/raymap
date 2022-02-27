using System;

namespace BinarySerializer.Ubisoft.CPA.U64 {
    public class GAM_CharacterStandardGame : U64_Struct {
        public uint CustomBitsInit { get; set; }
        public uint Capabilities { get; set; }
        public U64_Reference<GAM_Family> Family { get; set; }
        public short HitPointsInit { get; set; }
        public short HitPointsMaxInit { get; set; }
        public short HitPointsMaxMax { get; set; }
        public StdInitFlag Flags { get; set; }
        public ushort SpecialPositionFlags { get; set; } // Unused in Rayman 2
        public Platform PlatformType { get; set; }
        public byte TransparencyZoneMin { get; set; }
        public byte TransparencyZoneMax { get; set; }
        public byte TooFarLimit { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            CustomBitsInit = s.Serialize<uint>(CustomBitsInit, name: nameof(CustomBitsInit));
            Capabilities = s.Serialize<uint>(Capabilities, name: nameof(Capabilities));
            Family = s.SerializeObject<U64_Reference<GAM_Family>>(Family, name: nameof(Family))?.Resolve(s);
            HitPointsInit = s.Serialize<short>(HitPointsInit, name: nameof(HitPointsInit));
            HitPointsMaxInit = s.Serialize<short>(HitPointsMaxInit, name: nameof(HitPointsMaxInit));
            HitPointsMaxMax = s.Serialize<short>(HitPointsMaxMax, name: nameof(HitPointsMaxMax));
            Flags = s.Serialize<StdInitFlag>(Flags, name: nameof(Flags));
            SpecialPositionFlags = s.Serialize<ushort>(SpecialPositionFlags, name: nameof(SpecialPositionFlags));
            PlatformType = s.Serialize<Platform>(PlatformType, name: nameof(PlatformType));
            TransparencyZoneMin = s.Serialize<byte>(TransparencyZoneMin, name: nameof(TransparencyZoneMin));
            TransparencyZoneMax = s.Serialize<byte>(TransparencyZoneMax, name: nameof(TransparencyZoneMax));
            TooFarLimit = s.Serialize<byte>(TooFarLimit, name: nameof(TooFarLimit));
        }

        [Flags]
        public enum StdInitFlag : ushort {
            None = 0,
            WhenOutOfZoneNeverBack = 1 << 0,
            WhenOutOfZoneGoOutOfZone = 1 << 1,
            WhenDeadOrTakenNeverBack = 1 << 2,
            WhenDeadOrTakenGoOutOfZone = 1 << 3,
            WhenDeadOrTakenAlways = 1 << 4,
            WhenDeadOrTakenPlayerDead = 1 << 5,
            WhenDeadOrTakenMapLoaded = 1 << 6,
            WhenDeadOrTakenSavedGameLoaded = 1 << 7,
        }

        public enum Platform : byte {
            None = 0,
            StandardPlatform = 1
        }
    }
}
