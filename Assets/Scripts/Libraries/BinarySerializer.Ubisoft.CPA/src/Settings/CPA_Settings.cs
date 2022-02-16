using System;

namespace BinarySerializer.Ubisoft.CPA {
    /// <summary>
    /// Settings for serializing OpenSpace game formats
    /// </summary>
    public class CPA_Settings
    {
        public CPA_Settings(EngineVersion engineVersion, Platform platform)
        {
            EngineVersion = engineVersion;
            Platform = platform;
            EngineVersionTree = EngineVersionTree.Create(this);
        }

        public EngineVersion EngineVersion { get; }
        public Platform Platform { get; }

        public Endian GetEndian => Platform switch
        {
            Platform.GC      => Endian.Big,
            Platform.N64     => Endian.Big,
            Platform.MacOS   => Endian.Big,
            Platform.Xbox360 => Endian.Big,
            Platform.PS3     => Endian.Big,
            _                => Endian.Little
        };

        /// <summary>
        /// Engine version tree. CPA has a complex history and evolved with many branches
        /// </summary>
        public EngineVersionTree EngineVersionTree { get; }
    }
}