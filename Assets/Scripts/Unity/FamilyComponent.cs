using UnityEngine;
using System.Collections;
using OpenSpace;
using System.Collections.Generic;
using System;

public class FamilyComponent : MonoBehaviour
{
    public Family family;
    public List<MechanicsIDCard> idCards;
    public int cardCount;
    public bool dirty = false;

    // Use this for initialization
    public void Init(Family family)
    {
        this.family = family;
        this.idCards = new List<MechanicsIDCard>();
        foreach(State state in this.family.states) {

            if (state.mechanicsIDCard==null) {
                continue;
            }

            if (!idCards.Contains(state.mechanicsIDCard)) {
                this.idCards.Add(state.mechanicsIDCard);
            }
        }

        this.cardCount = this.idCards.Count;
    }

    public void SaveChanges(Writer writer)
    {
        if (dirty) {

            foreach (State state in this.family.states) {
                state.mechanicsIDCard.Write(writer);
            }

            dirty = false;
        }
    }
}
