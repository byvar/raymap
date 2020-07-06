using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections.Generic;

public class ScriptComponent : BaseScriptComponent {
    public Script script;
    private Perso perso;

    private TranslatedScript translation = null;
    public override string TranslatedScript {
        get {
            if ((forceUpdateScript || translatedScript == null) && translation != null) {
				translation.printAddresses = showOffset;
                translation.expandMacros = expandMacros;
                translatedScript = translation.ToString();
                forceUpdateScript = false;
                //translation = null;
            }
            return translatedScript;
        }
    }

    public void SetScript(Script script, Perso perso) {
        if (script != null) {
            this.script = script;
            this.perso = perso;
            translation = new TranslatedScript(script, perso);
            translation.printAddresses = showOffset;
            translation.expandMacros = expandMacros;
            //translatedScript = translation.ToString();
            Offset = script.offset;
            offsetString = script.offset.ToString();
            if (script.behaviorOrMacro != null) {
                comportOffset = script.behaviorOrMacro.Offset.ToString();
            }
        }
    }
}
