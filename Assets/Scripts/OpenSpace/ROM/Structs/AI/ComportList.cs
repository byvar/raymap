using OpenSpace.Loader;
using System.Linq;
using UnityEngine;

namespace OpenSpace.ROM {
	public class ComportList : ROMStruct {
		// Size: 8
		public Reference<ComportArray> comports;
		public Reference<Comport> currentComport;
		public ushort num_comports;
		public ushort word6;

		protected override void ReadInternal(Reader reader) {
			comports = new Reference<ComportArray>(reader);
			currentComport = new Reference<Comport>(reader, true);
			num_comports = reader.ReadUInt16();
			word6 = reader.ReadUInt16();
			comports.Resolve(reader, ca => ca.length = num_comports);
		}

        public void CreateGameObjects(string ruleOrReflex, GameObject persoGao, Perso perso)
        {
            if (this.comports.Value == null) {
                return;
            }

            GameObject intelParent = new GameObject(ruleOrReflex + " behaviours");
            intelParent.transform.parent = persoGao.transform;

            Reference<Comport>[] behaviors = this.comports.Value.comports;
            int iter = 0;
            foreach (var behaviorRef in behaviors) {
                Comport behavior = behaviorRef.Value;

                if (behavior == null) {
                    continue;
                }

                GameObject behaviorGao = new GameObject(ruleOrReflex+" #"+iter);
                behaviorGao.transform.parent = intelParent.transform;
                if (behavior.scripts?.Value?.scripts != null) {
                    foreach (Reference<OpenSpace.ROM.Script> scriptRef in behavior.scripts?.Value?.scripts) {
                        OpenSpace.ROM.Script script = scriptRef.Value;
                        GameObject scriptGao = new GameObject("Script");
                        scriptGao.transform.parent = behaviorGao.transform;
                        ROMScriptComponent scriptComponent = scriptGao.AddComponent<ROMScriptComponent>();
                        scriptComponent.SetScript(script, perso);
                    }
                }
                if (behavior.firstScript.Value != null) {
                    ROMScriptComponent scriptComponent = behaviorGao.AddComponent<ROMScriptComponent>();
                    scriptComponent.SetScript(behavior.firstScript.Value, perso);
                }
                if (iter == 0) {
                    behaviorGao.name += " (Init)";
                }
                if ((behavior.scripts?.Value == null || behavior.scripts?.Value.length == 0) && behavior.firstScript?.Value == null) {
                    behaviorGao.name += " (Empty)";
                }
                iter++;
            }
        }
	}
}