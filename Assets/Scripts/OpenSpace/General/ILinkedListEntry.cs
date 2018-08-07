using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSpace {
    interface ILinkedListEntry {
        Pointer NextEntry { get; }
        Pointer PreviousEntry { get; }
    }
}
