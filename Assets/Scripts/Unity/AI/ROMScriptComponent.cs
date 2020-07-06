using UnityEngine;
using System.Collections;
using OpenSpace.ROM;

public class ROMScriptComponent : BaseScriptComponent {
    private Script script;
    private Perso perso;

    private TranslatedROMScript translation = null;
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
            translation = new TranslatedROMScript(script, perso);
            translation.printAddresses = showOffset;
            translation.expandMacros = expandMacros;
            //translatedScript = translation.ToString();
            Offset = script.Offset;
            offsetString = script.Offset.ToString();
        }
    }
}
