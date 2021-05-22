using OpenSpace;
using OpenSpace.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSpace.Exporter {
    abstract class AIModelExporter {

        public static string AIModelToCSharp_R2(string nameSpace, AIModel ai)
        {

            string dsgVars = "";//  TODO

            if (ai.dsgVar != null) {
                for(int i = 0; i < ai.dsgVar.dsgVarInfos.Length; i++) {
                    dsgVars += "public " + DsgVarUtil.DsgVarEntryToCSharpAssignment(ai.dsgVar, i) + Environment.NewLine;
                }
            }

            List<string> ruleStatesInitializer = new List<string>();
            List<string> reflexStatesInitializer = new List<string>();

            TranslatedScript.TranslationSettings translationSettings = new TranslatedScript.TranslationSettings()
            {
                expandEntryActions = false,
                expandStrings = false,
                useStateIndex = true,
                exportMode = true,
                useHashIdentifiers = true
            };

            string behaviorsNormal = "";

            if (ai.behaviors_normal != null) {
                for (int i = 0; i < ai.behaviors_normal.Length; i++) {
                    if (ai.behaviors_normal[i].scripts != null) {
                        string combinedScript = "public async Task Rule_" + i + "_" + ai.behaviors_normal[i].name + "() {" + Environment.NewLine;
                        ruleStatesInitializer.Add("Rule_" + i + "_" + ai.behaviors_normal[i].name);
                        for (int j = 0; j < ai.behaviors_normal[i].scripts.Length; j++) {
                            TranslatedScript ts = new TranslatedScript(ai.behaviors_normal[i].scripts[j], null);
                            ts.settings = translationSettings;
                            combinedScript += "// Script " + j + Environment.NewLine + ts.ToCSharpString_R2() + Environment.NewLine;
                        }
                        combinedScript += "}";
                        behaviorsNormal += combinedScript + Environment.NewLine;
                    }
                }
            }

            string behaviorsReflex = "";

            if (ai.behaviors_reflex != null) {
                for (int i = 0; i < ai.behaviors_reflex.Length; i++) {
                    if (ai.behaviors_reflex[i].scripts != null) {
                        string combinedScript = "public async Task Reflex_" + i + "_" + ai.behaviors_reflex[i].name + "() {" + Environment.NewLine;
                        for (int j = 0; j < ai.behaviors_reflex[i].scripts.Length; j++) {
                            reflexStatesInitializer.Add("Reflex_" + i + "_" + ai.behaviors_reflex[i].name);
                            TranslatedScript ts = new TranslatedScript(ai.behaviors_reflex[i].scripts[j], null);
                            ts.settings = translationSettings;
                            combinedScript += "// Script " + j + Environment.NewLine + ts.ToCSharpString_R2() + Environment.NewLine;
                        }
                        combinedScript += "}";
                        behaviorsReflex += combinedScript + Environment.NewLine;
                    }
                }
            }

            string macros = "";

            if (ai.macros != null) {
                for (int i = 0; i < ai.macros.Length; i++) {
                    if (ai.macros[i].script != null) {
                        string macroName = ai.macros[i].name != null ? "_"+ai.macros[i].name : "";
                        string combinedScript = "private async Task Macro_" + i + macroName + "() {" + Environment.NewLine;
                        TranslatedScript ts = new TranslatedScript(ai.macros[i].script, null);
                        ts.settings = translationSettings;
                        combinedScript += ts.ToCSharpString_R2() + Environment.NewLine + "}";
                        macros += combinedScript + Environment.NewLine;
                    }
                }
            }

            // DONE TODO: replace evalMacro calls by replacing regex "evalMacro\([a-zA-Z0-9_]*\.Macro\[([0-9]+)\]\)" to "yield return Macro_$1()"
            // TODO: replace Proc_ChangeMyComport\([a-zA-Z0-9_]+\.Rule\[[0-9]+\]\[\"([^"]+)\"\]\)     with     sm.ChangeActiveRuleState("$1")
            // TODO: replace Cond_IsValidObject\(([^\)]+)\)    with $1 != null

            string startString = "protected override void Start() {" + Environment.NewLine + "base.Start();" + Environment.NewLine + Environment.NewLine;

            if (ruleStatesInitializer.Count > 0)
                startString += "smRule.SetState(" + ruleStatesInitializer[0] + ");" + Environment.NewLine;

            if (reflexStatesInitializer.Count > 0)
                startString += "smReflex.SetState(" + reflexStatesInitializer[0] + ");" + Environment.NewLine;

            startString += "}";

            string[] usingItems = new string[]{
                "UnityEngine",
                "System.Threading.Tasks",
                "System.Collections.Generic",
                "OpenSpaceImplementation",
                "OpenSpaceImplementation.AI",
                "OpenSpaceImplementation.Materials",
                "OpenSpaceImplementation.Input",
                "OpenSpaceImplementation.Sound",
                "OpenSpaceImplementation.Strings",
                "OpenSpaceImplementation.Waypoints",
                "OpenSpaceImplementation.Animation"
            };

            string usingBlock = string.Join(Environment.NewLine, usingItems.Select(i => "using " + i + ";"));

            string[] resultItems = new string[] {
                usingBlock,
                "namespace "+nameSpace+" {",
                "public partial class " + ai.name + " : Perso {",
                dsgVars,
                startString,
                behaviorsNormal,
                behaviorsReflex,
                macros,
                "}",
                "}"
            };

            return string.Join(Environment.NewLine, resultItems);
        }


    }
}
