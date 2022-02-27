using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarySerializer.Ubisoft.CPA {
    public enum PathCapitalizationType {
        All,
        LevelFolder, LevelFile, LevelRelocation,
        Fix, FixLvl, FixRelocation,
        LangFix, LangLevelFolder, LangLevelFile, LangLevelRelocation,
        DSB, LMFile, TextureFile
    };
}
