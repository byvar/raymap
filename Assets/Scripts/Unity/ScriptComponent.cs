using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;

public class ScriptComponent : MonoBehaviour {
    private Script script;
    private Perso perso;
    public string offset;
    public string comportOffset;
    public bool showOffset = false;

    private TranslatedScript translation = null;
    private string translatedScript = null;
    public string TranslatedScript {
        get {
            if ((forceUpdateScript || translatedScript == null) && translation != null) {
				translation.printAddresses = showOffset;
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
            //translatedScript = translation.ToString();
            offset = script.offset.ToString();
            if (script.behaviorOrMacro != null) {
                comportOffset = script.behaviorOrMacro.offset.ToString();
            }
        }
    }

    private bool _showOffset = false;
    private bool forceUpdateScript = false;
    public void Update()
    {
        if (showOffset!=_showOffset)
        {
            _showOffset = showOffset;
            forceUpdateScript = true;
            //SetScript(this.script, this.perso);
        }
    }
}
