using System;

namespace BinarySerializer.Ubisoft.CPA {
    [Flags]
    public enum EngineFlags {
        None = 0,
        U64 = 1 << 0,
    }
}