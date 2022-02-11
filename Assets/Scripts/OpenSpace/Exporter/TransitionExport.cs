using OpenSpace;
using OpenSpace.AI;
using OpenSpace.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenSpace.Exporter {
    public class TransitionExport {

        [Serializable]
        public class State {
            public string name;
            public int index;
            public string offset;
            public int[] transitionToNormal;
            public int[] transitionToReflex;
        }

        [Serializable]
        public class StateDiagram {
            public List<State> normalStates;
            public List<State> reflexStates;
        }

        List<State> normalStates = new List<State>();
        List<State> reflexStates = new List<State>();
        StateDiagram stateDiagram;
        string json;

        public TransitionExport(Perso perso)
        {
            AIModel model = perso.brain.mind.AI_model;
            if (model==null) {
                return;
            }
            Behavior[] normalBehaviors = model.behaviors_normal; // normal/intelligence/rul behaviors
            Behavior[] reflexBehaviors = model.behaviors_reflex; // reflex behaviors

            this.normalStates = new List<State>();
            this.reflexStates = new List<State>();

            if (normalBehaviors != null) {
                foreach (Behavior b in normalBehaviors) {
                    normalStates.Add(CreateStateFromBehavior(b));
                }
            }

            if (reflexBehaviors != null) {
                foreach (Behavior b in reflexBehaviors) {
                    reflexStates.Add(CreateStateFromBehavior(b));
                }
            }

            stateDiagram = new StateDiagram();
            stateDiagram.normalStates = normalStates;
            stateDiagram.reflexStates = reflexStates;
            this.json = GetJson();
        }

        public override String ToString()
        {
            return this.json;
        }

        public string GetJson()
        {
            return JsonUtility.ToJson(stateDiagram);
        }

        public State CreateStateFromBehavior(Behavior behavior)
        {
 
            State state = new State();
            state.index = behavior.index;
            state.offset = behavior.Offset.ToString();
            state.name = behavior.ExportName;

            List<int> transitionToIndicesNormal = new List<int>();
            List<int> transitionToIndicesReflex = new List<int>();

            // Apparently hype and TT uses this
            if (behavior.scheduleScript != null) {
                transitionToIndicesNormal.AddRange(GetTransitionToIndicesNormalBehavior(behavior.scheduleScript));
                transitionToIndicesReflex.AddRange(GetTransitionToIndicesReflexBehavior(behavior.scheduleScript));
            }

            foreach (Script script in behavior.scripts) {

                transitionToIndicesNormal.AddRange(GetTransitionToIndicesNormalBehavior(script));
                transitionToIndicesReflex.AddRange(GetTransitionToIndicesReflexBehavior(script));
            }

            state.transitionToNormal = transitionToIndicesNormal.ToArray();
            state.transitionToReflex = transitionToIndicesReflex.ToArray();
            return state;
        }

        public List<int> GetTransitionToIndicesNormalBehavior(Script script)
        {
            List<int> transitionToIndices = new List<int>();

            for (int i = 0; i < script.scriptNodes.Count; i++) {

                ScriptNode node = script.scriptNodes[i];
                ScriptNode nextNode = null;
                if (i < script.scriptNodes.Count - 1) {
                    nextNode = script.scriptNodes[i + 1];
                }

                if (node.nodeType == ScriptNode.NodeType.Procedure) {
                    if (node.param>=CPA_Settings.s.aiTypes.procedureTable.Length) {
                        //Debug.LogError("node.param is out of range of procedure table: "+node.param);
                        continue;
                    }
                    string procedureType = CPA_Settings.s.aiTypes.procedureTable[node.param];
                    if (procedureType == "Proc_ChangeMyComport" || procedureType == "Proc_ChangeMyComportAndMyReflex" ||
                        procedureType == "_fn_p_stChangeMyComportIntelligenceProcedure" || procedureType == "_fn_p_stChangeMyComportIntelligenceAndReflexProcedure" || 
                        procedureType == "ChangeMyComportIntelligenceProcedure" ||  procedureType == "ChangeMyComportIntelligenceAndReflexProcedure" ) {

                        Behavior transitionBehavior = MapLoader.Loader.FromOffset<Behavior>(nextNode.param_ptr);
                        if (transitionBehavior!=null && !transitionToIndices.Contains(transitionBehavior.index)) {
                            transitionToIndices.Add(transitionBehavior.index);
                        }

                    }
                }

                if (node.nodeType == ScriptNode.NodeType.SubRoutine) {
                    Macro macro = MapLoader.Loader.FromOffset<Macro>(node.param_ptr);
                    if (macro != null) {
                        transitionToIndices.AddRange(GetTransitionToIndicesNormalBehavior(macro.script));
                    } else {
                        Debug.LogWarning("No macro found after subroutine call");
                    }
                }
            }



            return transitionToIndices;
        }

        public List<int> GetTransitionToIndicesReflexBehavior(Script script)
        {
            List<int> transitionToIndices = new List<int>();

            for (int i = 0; i < script.scriptNodes.Count; i++) {

                ScriptNode node = script.scriptNodes[i];
                ScriptNode nextNode = null;
                ScriptNode nextNextNode = null;
                if (i < script.scriptNodes.Count - 1) {
                    nextNode = script.scriptNodes[i + 1];
                }
                if (i < script.scriptNodes.Count - 2) {
                    nextNextNode = script.scriptNodes[i + 2];
                }

                if (node.nodeType == ScriptNode.NodeType.Procedure) {
                    if (node.param >= CPA_Settings.s.aiTypes.procedureTable.Length) {
                        //Debug.LogError("node.param is out of range of procedure table: "+node.param);
                        continue;
                    }
                    string procedureType = CPA_Settings.s.aiTypes.procedureTable[node.param];
                    if (procedureType == "Proc_ChangeMyComportReflex" || procedureType == "_fn_p_stChangeMyComportReflexProcedure" || procedureType == "ChangeMyComportReflexProcedure") {
                        Behavior transitionBehavior = MapLoader.Loader.FromOffset<Behavior>(nextNode.param_ptr);
                        if (transitionBehavior!=null && !transitionToIndices.Contains(transitionBehavior.index)) {
                            transitionToIndices.Add(transitionBehavior.index);
                        }
                    }

                    if (nextNextNode != null && (procedureType == "Proc_ChangeMyComportAndMyReflex" || procedureType == "_fn_p_stChangeMyComportIntelligenceAndReflexProcedure" || procedureType == "ChangeMyComportIntelligenceAndReflexProcedure" )) {
                        Behavior transitionBehavior = MapLoader.Loader.FromOffset<Behavior>(nextNextNode.param_ptr);
                        if (transitionBehavior != null && !transitionToIndices.Contains(transitionBehavior.index)) {
                            transitionToIndices.Add(transitionBehavior.index);
                        }
                    }
                }

                if (node.nodeType == ScriptNode.NodeType.SubRoutine) {
                    Macro macro = MapLoader.Loader.FromOffset<Macro>(node.param_ptr);
                    if (macro != null) {
                        transitionToIndices.AddRange(GetTransitionToIndicesReflexBehavior(macro.script));
                    } else {
                        Debug.LogWarning("No macro found after subroutine call");
                    }
                }
            }

            return transitionToIndices;
        }

    }
}
