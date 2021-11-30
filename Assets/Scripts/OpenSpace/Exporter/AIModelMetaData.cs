using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenSpace.AI;

namespace OpenSpace.Exporter {
    public readonly struct AIModelMetaData
    {
        public static AIModelMetaData FromAIModel(AIModel m, string name, string[] initialRule, string[] initialReflex)
        {
            Dictionary<PointerInMap, string> offsetToBehaviourNameMap = new Dictionary<PointerInMap, string>();
            Dictionary<PointerInMap, PersoNames> offsetToPersoModelNameMap = new Dictionary<PointerInMap, PersoNames>();

            string map = MapLoader.Loader.lvlName;

            foreach (var p in MapLoader.Loader.persos) {
                offsetToPersoModelNameMap.Add(new PointerInMap(map,p.offset.FileOffset), new PersoNames(p.namePerso, p.nameModel+"__"+p.nameFamily, p.nameFamily));
            }

            if (m.behaviors_normal != null) {

                foreach (var rule in m.behaviors_normal) {
                    offsetToBehaviourNameMap.Add(new PointerInMap(map,rule.Offset.offset), rule.NameSubstring);
                }
            }

            if (m.behaviors_reflex != null) {

                foreach (var reflex in m.behaviors_reflex) {
                    offsetToBehaviourNameMap.Add(new PointerInMap(map,reflex.Offset.offset), reflex.NameSubstring);
                }
            }

            if (m.macros != null) {
                foreach (var macro in m.macros) {
                    offsetToBehaviourNameMap.Add(new PointerInMap(map,macro.Offset.offset), macro.NameSubstring);
                }
            }

            List<DsgVar> dsgVars = new List<DsgVar>();

            int iter = 0;
            if (m.dsgVar?.dsgVarInfos != null) {
                foreach (var d in m.dsgVar.dsgVarInfos) {
                    dsgVars.Add(new DsgVar(d.type, m.dsgVar.defaultValues[iter].GetUntypedValue()));
                    iter++;
                }
            }

            return new AIModelMetaData(m.Offset.offset, MapLoader.Loader.lvlName, name, initialRule, initialReflex,  dsgVars, offsetToBehaviourNameMap, offsetToPersoModelNameMap);
        }

        public readonly uint Offset;
        public readonly string Map;
        public readonly string Name;
        public readonly string[] InitialRules;
        public readonly string[] InitialReflexes;
        public readonly List<DsgVar> DsgVars;
        public readonly Dictionary<PointerInMap, string> OffsetToBehaviourNameMap;
        public readonly Dictionary<PointerInMap, PersoNames> OffsetToPersoNamesMap;

        public AIModelMetaData(uint offset, string map, string name, string[] initialRules, string[] initialReflexes, List<DsgVar> dsgVars, Dictionary<PointerInMap, string> offsetToBehaviourNameMap, Dictionary<PointerInMap, PersoNames> offsetToPersoNamesMap)
        {
            this.Offset = offset;
            this.Map = map;
            this.Name = name;
            this.InitialRules = initialRules;
            this.InitialReflexes = initialReflexes;
            this.DsgVars = dsgVars;
            this.OffsetToBehaviourNameMap = offsetToBehaviourNameMap;
            this.OffsetToPersoNamesMap = offsetToPersoNamesMap;
        }

        public readonly struct PersoNames {
            public readonly string InstanceName;
            public readonly string ModelName;
            public readonly string FamilyName;

            public PersoNames(string instanceName, string modelName, string familyName)
            {
                InstanceName = instanceName;
                ModelName = modelName;
                FamilyName = familyName;
            }
        }

        public readonly struct DsgVar
        {
            public readonly DsgVarType Type;
            public readonly object ModelValue;

            public DsgVar(DsgVarType type, object modelValue)
            {
                this.Type = type;
                this.ModelValue = modelValue;
            }
        }

        public static AIModelMetaData Merge(AIModelMetaData oldData, AIModelMetaData newData)
        {
            var oldBehaviourNameMap = oldData.OffsetToBehaviourNameMap;
            var newBehaviourNameMap = newData.OffsetToBehaviourNameMap;

            foreach (var kv in oldBehaviourNameMap) {
                if (!newBehaviourNameMap.ContainsKey(kv.Key)) {
                    newBehaviourNameMap.Add(kv.Key, kv.Value);
                }
            }

            var oldPersoNameMap = oldData.OffsetToPersoNamesMap;
            var newPersoNameMap = newData.OffsetToPersoNamesMap;

            foreach (var kv in oldPersoNameMap) {
                if (!newPersoNameMap.ContainsKey(kv.Key)) {
                    newPersoNameMap.Add(kv.Key, kv.Value);
                }
            }

            var initialRules = new List<string>();
            var initialReflexes = new List<string>();

            initialRules.AddRange(oldData.InitialRules);
            initialRules.AddRange(newData.InitialRules);

            initialReflexes.AddRange(oldData.InitialReflexes);
            initialReflexes.AddRange(newData.InitialReflexes);

            return new AIModelMetaData(newData.Offset, newData.Map, newData.Name, initialRules.Distinct().ToArray(), initialReflexes.Distinct().ToArray(), newData.DsgVars, newBehaviourNameMap,
                newPersoNameMap);

        }
    }
}
