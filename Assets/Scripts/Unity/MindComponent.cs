using UnityEngine;
using System.Collections;
using OpenSpace.AI;
using OpenSpace.EngineObject;
using OpenSpace;

public class MindComponent : MonoBehaviour {

    public Perso perso;
    public Mind mind;

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
                    Pointer.Write(writer, this.mind.intelligenceNormal.comport.offset);
                else
                    Pointer.Write(writer, null);
            });
            this.writeNormalComport = false;
        }

        if (this.writeReflexComport) {
            Pointer.DoAt(ref writer, this.mind.intelligenceReflex.offset + 0x8, () =>
            {
                if (this.mind.intelligenceReflex.comport!=null)
                    Pointer.Write(writer, this.mind.intelligenceReflex.comport.offset);
                else
                    Pointer.Write(writer, null);
            });
            this.writeReflexComport = false;
        }
    }
}