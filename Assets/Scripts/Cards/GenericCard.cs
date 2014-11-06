using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GenericCard : MonoBehaviour {

    public enum CardStates
    {
        IN_DECK, IN_HAND, IN_PLAY, DISCARDED
    }

    public enum CardTypes
    {
        LESSON, CREATURE, SPELL, ITEM, LOCATION, MATCH, ADVENTURE, CHARACTER
    }
    public enum CostTypes
    {
        CARE_OF_MAGICAL_CREATURES, CHARMS, TRANSFIGURATION, POTIONS, QUIDDITCH
    }

    public CardStates State;
    public CardTypes CardType;

    public Player _Player;

    //Not sure if we'll ever need to access this in the subclasses, private for now.
    private readonly Vector2 ColliderSize = new Vector2(50f, 70f);

    protected bool Zoomed;
    protected readonly float ZoomTweenTime = 0.1f;
    protected readonly float ZoomScale_Hand = 2.5f;
    protected readonly float ZoomScale_InPlay = 2.25f;

    public void Start()
    {
        //Add the collider through code instead of through unity so that if it ever changes, we won't need to edit every prefab.
        if(gameObject.collider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(ColliderSize.x, ColliderSize.y, 0.2f);
        }
            
        Zoomed = false;
    }
    
    private void ZoomInPlay()
    {
        if (!Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", ZoomScale_InPlay, "y", ZoomScale_InPlay, "time", ZoomTweenTime));
            Zoomed = true;
        }
    }

    private void ZoomInHand()
    {
        if (!Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", ZoomScale_Hand, "y", ZoomScale_Hand, "time", ZoomTweenTime));
            iTween.MoveTo(gameObject, iTween.Hash("y", Hand.HAND_CARDS_OFFSET.y + 60f, "time", ZoomTweenTime, "islocal", true));
            Zoomed = true;
        }
    }

    //TODO: Implement separate zoom
    /*
    public void OnMouseEnter()
    {
        switch (State)
        {
            case CardStates.IN_HAND:
                ZoomInHand();
                break;
            case CardStates.IN_PLAY:
                ZoomInPlay();
                break;
        }
        
    }
    */

    /*
    public void OnMouseExit()
    {
        if (Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 1, "y", 1, "time", ZoomTweenTime));

            if (State == CardStates.IN_HAND)
            {
                iTween.MoveTo(gameObject, iTween.Hash("y", Hand.HAND_CARDS_OFFSET.y, "time", ZoomTweenTime, "islocal", true));
            }

            Zoomed = false;
        }
    }
    */
    public void SwitchState(CardStates newState)
    {
        State = newState;
    }

    public abstract void BeforeTurnAction();
    public abstract void AfterTurnAction();
}
