using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericCard : MonoBehaviour {

    public enum CardStates
    {
        IN_DECK, IN_HAND, IN_PLAY
    }

    public enum CardTypes
    {
        LESSON, CREATURE, SPELL, ITEM, LOCATION, MATCH, ADVENTURE, CHARACTER
    }

    public CardStates State;
    public CardTypes CardType;

    public Player _Player;

    protected bool Zoomed;

    public void Start()
    {
        Zoomed = false;
    }

    public void OnMouseEnter()
    {
        // TODO: Implement separate zoom when card is IN_PLAY
        if (State != CardStates.IN_HAND) return;

        if (!Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 3, "y", 3, "time", 0.5f));
            iTween.MoveTo(gameObject, iTween.Hash("y", -95f, "time", 0.5f, "islocal", true));
            Zoomed = true;
        }
    }

    public void OnMouseExit()
    {
        if (State != CardStates.IN_HAND) return;

        if (Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 1, "y", 1, "time", 0.5f));
            iTween.MoveTo(gameObject, iTween.Hash("y", -160f, "time", 0.5f, "islocal", true));
            Zoomed = false;
        }
    }

    public void SwitchState(CardStates newState)
    {
        State = newState;
    }
}
