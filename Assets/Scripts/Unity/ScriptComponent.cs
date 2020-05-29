using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections.Generic;

public class ScriptComponent : MonoBehaviour {
    public Script script;
    private Perso perso;
    public string offset;
    public string comportOffset;
    public bool showOffset = false;
    public bool expandMacros = false;

    private TranslatedScript translation = null;
    private string translatedScript = null;
    public string TranslatedScript {
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
            offset = script.offset.ToString();
            if (script.behaviorOrMacro != null) {
                comportOffset = script.behaviorOrMacro.Offset.ToString();
            }
        }
    }

    private bool _showOffset = false;
    private bool _expandMacros = false;
    private bool forceUpdateScript = false;
    public void Update()
    {
        if (showOffset!=_showOffset)
        {
            _showOffset = showOffset;
            forceUpdateScript = true;
            //SetScript(this.script, this.perso);
        }

        if (expandMacros!=_expandMacros) {

            _expandMacros = expandMacros;
            forceUpdateScript = true;
        }
    }
}
