using System;
using System.Collections.Generic;
using LinkedListType = BinarySerializer.Ubisoft.CPA.LST2_ListType;

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
			COLTypes = COL_Types.GetCOLTypes(this);
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

        // Legacy. TODO: Remove those that become useless
        public CPA_GameMode Mode { get; set; } = CPA_GameMode.Rayman3PC;
        public LST2_ListType StaticListType { get; set; } = LST2_ListType.DoubleLinked;
        public bool HasObjectTypes { get; set; } = true;
        public bool HasNames { get; set; } = false;
        public bool HasDeformations { get; set; } = false;
        public int EntryActionsCount { get; set; } = 0;
        public bool HasExtraInputData { get; set; } = false;
        public bool HasMemorySupport { get; set; } = false;
        public Dictionary<string, uint> MemoryAddresses { get; set; } = null;
        public bool LoadFromMemory { get; set; } = false;
        public Encryption Encryption { get; set; } = Encryption.None;
        public bool EncryptPointerFiles { get; set; } = false;
        public bool HasLinkedListHeaderPointers { get; set; } = false;
        public bool SNA_Compression { get; set; } = false;
        public AITypes AITypes { get; set; }
		public COL_Types COLTypes { get; set; }
        public float TextureAnimationSpeedModifier { get; set; } = 1f;
        public float Luminosity { get; set; } = 0.5f;
        public bool Saturate { get; set; } = true;
        public Dictionary<PathCapitalizationType, PathCapitalization> PathCapitalization { get; set; } = new Dictionary<PathCapitalizationType, PathCapitalization>();
        public LevelTranslation LevelTranslation { get; set; } = null;
        public bool LinkUncategorizedObjectsToScriptFamily { get; set; } = false;
    }
}