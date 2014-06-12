using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericCard : MonoBehaviour {

    public enum CardStates
    {
        IN_DECK, IN_HAND, IN_PLAY
    }

    public CardStates State;

    protected bool Zoomed;


    public void Start()
    {
        Zoomed = false;
    }

    public void OnMouseEnter()
    {
        if (State == CardStates.IN_DECK) return;

        if (!Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 3, "y", 3, "time", 0.5f));
            iTween.MoveTo(gameObject, iTween.Hash("y", -95f, "time", 0.5f));
            Zoomed = true;
        }
    }

    public void OnMouseExit()
    {
        if (State == CardStates.IN_DECK) return;

        if (Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 1, "y", 1, "time", 0.5f));
            iTween.MoveTo(gameObject, iTween.Hash("y", -160f, "time", 0.5f));
            Zoomed = false;
        }
    }

    public void SwitchState(CardStates newState)
    {
        State = newState;
    }
}
