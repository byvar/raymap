using System;

namespace BinarySerializer.Ubisoft.CPA {
    /// <summary>
    /// Settings for serializing OpenSpace game formats
    /// </summary>
    public class OpenSpaceSettings
    {
        public OpenSpaceSettings(EngineVersion engineVersion, Platform platform)
        {
            EngineVersion = engineVersion;
            Platform = platform;

            switch (EngineVersion)
            {
                case EngineVersion.TonicTrouble:
                case EngineVersion.TonicTroubleSpecialEdition:
                    MajorEngineVersion = MajorEngineVersion.TonicTrouble;
                    break;

                case EngineVersion.PlaymobilHype:
                case EngineVersion.PlaymobilAlex:
                case EngineVersion.PlaymobilLaura:
                    MajorEngineVersion = MajorEngineVersion.Montreal;
                    break;

                case EngineVersion.Rayman2:
                case EngineVersion.Rayman2Demo:
                case EngineVersion.Rayman2Revolution:
                case EngineVersion.RaymanRush:
                case EngineVersion.RaymanRavingRabbids:
                case EngineVersion.DonaldDuckQuackAttack:
                    MajorEngineVersion = MajorEngineVersion.Rayman2;
                    break;

                case EngineVersion.RaymanM:
                case EngineVersion.RaymanArena:
                case EngineVersion.Rayman3:
                case EngineVersion.DonaldDuckPK:
                case EngineVersion.Dinosaur:
                case EngineVersion.LargoWinch:
                    MajorEngineVersion = MajorEngineVersion.Rayman3;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(EngineVersion), EngineVersion, null);
            }
        }

        public MajorEngineVersion MajorEngineVersion { get; }
        public EngineVersion EngineVersion { get; }
        public Platform Platform { get; }

        public Endian GetEndian => Platform switch
        {
            Platform.NintendoGameCube => Endian.Big,
            Platform.Nintendo64 => Endian.Big,
            _ => Endian.Little
        };
    }
}