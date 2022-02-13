using OpenSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BrainComponent : MonoBehaviour {
    public BasePersoBehaviour perso;
    public DsgVarComponent dsgVars;
    public MindComponent mind;


    public List<Comport> Intelligence { get; set; } = new List<Comport>();
    public List<Comport> Reflex { get; set; } = new List<Comport>();
    public List<Macro> Macros { get; set; } = new List<Macro>();
    public class Comport {
        public LegacyPointer Offset { get; set; }
        public string GaoName { get; set; }
        public string Name { get; set; }
        public BaseScriptComponent FirstScript { get; set; }
        public List<BaseScriptComponent> Scripts { get; set; } = new List<BaseScriptComponent>();
    }
    public class Macro {
        public LegacyPointer Offset { get; set; }
        public string GaoName { get; set; }
        public string Name { get; set; }
        public BaseScriptComponent Script { get; set; }
    }
}
