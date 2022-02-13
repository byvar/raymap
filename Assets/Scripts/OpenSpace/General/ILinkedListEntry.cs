using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    interface ILinkedListEntry {
        LegacyPointer NextEntry { get; }
        LegacyPointer PreviousEntry { get; }
    }
}
