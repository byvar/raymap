using Newtonsoft.Json;
using OpenSpace.AI;
using System.Collections.Generic;

namespace OpenSpace.Object {

    // Used to indicate an object that can be referenced in scripts/dsgvars
    public interface IReferenceable {
        ReferenceFields References { get; set; }
    }

    public class ReferenceFields {
        public List<ScriptNode> referencedByNodes = new List<ScriptNode>();
        public List<DsgMem> referencedByDsgMems = new List<DsgMem>();
    }
}