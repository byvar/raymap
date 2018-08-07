using UnityEngine;
using System.Collections;
using OpenSpace.EngineObject;
using OpenSpace.AI;
using System.Collections.Generic;

namespace OpenSpace
{
    public class DsgVarComponent : MonoBehaviour
    {
        public Perso perso;
        public DsgMem dsgMem;
        public DsgVar dsgVar;
        public DsgVarInfoEntry[] dsgVarEntries;

        public void SetPerso(Perso perso)
        {
            this.perso = perso;
            if (perso != null && perso.brain != null && perso.brain.mind != null && perso.brain.mind.dsgMem != null && perso.brain.mind.dsgMem.dsgVar != null) {
                this.dsgMem = perso.brain.mind.dsgMem;
                this.dsgVar = perso.brain.mind.dsgMem.dsgVar;
                this.dsgVarEntries = this.dsgVar.dsgVarInfos;

            }
        }
    }
}