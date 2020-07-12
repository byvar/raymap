using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;
using OpenSpace;
using OpenSpace.Exporter;

public class MindComponent : MonoBehaviour {

    public Perso perso;
    public Mind mind;
    
    // Behaviour/State transition overview
	private string transitionExport = null;
	public string TransitionExport {
		get {
			if (transitionExport == null) {
				transitionExport = new TransitionExport(perso).ToString();
			}
			return transitionExport;
		}
	}

    public bool writeReflexComport;
    public bool writeNormalComport;

    public void Init(Perso perso, Mind mind)
    {
        this.perso = perso;
        this.mind = mind;
    }

    public void Write(Writer writer)
    {
        if (this.writeNormalComport) {
            mind.intelligenceNormal.off_comport = mind.intelligenceNormal.comport?.Offset;
            mind.intelligenceNormal.Write(writer);
            this.writeNormalComport = false;
        }
        if (this.writeReflexComport) {
            mind.intelligenceReflex.off_comport = mind.intelligenceReflex.comport?.Offset;
            mind.intelligenceReflex.Write(writer);
            this.writeReflexComport = false;
        }
    }
}