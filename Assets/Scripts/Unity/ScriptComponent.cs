using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;

public class ScriptComponent : MonoBehaviour {
    private Script script;
    private Perso perso;
    public string offset;
    public bool showOffset = false;

    [TextArea(30,80)]
    public string translatedScript;

    public void SetScript(Script script, Perso perso) {
        if (script != null) {
            this.script = script;
            this.perso = perso;
            TranslatedScript translation = new TranslatedScript(script, perso);
            translation.printAddresses = showOffset;
            translatedScript = translation.ToString();
            offset = script.offset.ToString();
        }
    }

    private bool _showOffset = false;
    public void Update()
    {
        if (showOffset!=_showOffset)
        {
            _showOffset = showOffset;
            SetScript(this.script, this.perso);
        }
    }
}
