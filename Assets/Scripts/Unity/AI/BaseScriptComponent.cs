using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;
using System.Collections.Generic;
using OpenSpace;

public abstract class BaseScriptComponent : MonoBehaviour {
    public OpenSpace.Pointer Offset;
    public string offsetString;
    public string comportOffset;
    public bool showOffset = false;
    public bool expandMacros = false;

    protected string translatedScript = null;
    public abstract string TranslatedScript { get; }

    protected bool _showOffset = false;
    protected bool _expandMacros = false;
    protected bool forceUpdateScript = false;

    public void Awake()
    {
        MapLoader.Loader.scriptComponents.Add(this);
    }

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
