using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.EngineObject;

public class ScriptComponent : MonoBehaviour {
    public Script script;
    public string offset;
    [TextArea(30,80)]
    public string translatedScript;

    public void SetScript(Script script, Perso perso) {
        this.script = script;
        TranslatedScript translation = new TranslatedScript(script, perso);
        translatedScript = translation.ToString();
        offset = script.offset.ToString();
    }
}
