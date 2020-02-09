using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.Object;
using OpenSpace;
using Assets.Scripts.OpenSpace.AI;

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
            Pointer.DoAt(ref writer, this.mind.intelligenceNormal.offset + 0x8, () =>
            {
                if (this.mind.intelligenceNormal.comport != null)
                    Pointer.Write(writer, this.mind.intelligenceNormal.comport.Offset);
                else
                    Pointer.Write(writer, null);
            });
            this.writeNormalComport = false;
        }

        if (this.writeReflexComport) {
            Pointer.DoAt(ref writer, this.mind.intelligenceReflex.offset + 0x8, () =>
            {
                if (this.mind.intelligenceReflex.comport!=null)
                    Pointer.Write(writer, this.mind.intelligenceReflex.comport.Offset);
                else
                    Pointer.Write(writer, null);
            });
            this.writeReflexComport = false;
        }
    }
}