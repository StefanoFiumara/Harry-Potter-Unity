using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GenericCard : MonoBehaviour {

    public enum CardStates
    {
        IN_DECK, IN_HAND, IN_PLAY
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
    private readonly Vector2 ColliderSize = new Vector2(35f, 70f);

    protected bool Zoomed;
    protected readonly float ZoomTweenTime = 0.1f;
    protected readonly float ZoomScaleValue = 3f;

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

    public void OnMouseEnter()
    {
        // TODO: Implement separate zoom when card is IN_PLAY
        if (State != CardStates.IN_HAND) return;

        if (!Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", ZoomScaleValue, "y", ZoomScaleValue, "time", ZoomTweenTime));
            iTween.MoveTo(gameObject, iTween.Hash("y", Hand.HAND_CARDS_OFFSET.y + 60f, "time", ZoomTweenTime, "islocal", true));
            Zoomed = true;
        }
    }

    public void OnMouseExit()
    {
        if (State != CardStates.IN_HAND) return;

        if (Zoomed)
        {
            iTween.ScaleTo(gameObject, iTween.Hash("x", 1, "y", 1, "time", ZoomTweenTime));
            iTween.MoveTo(gameObject, iTween.Hash("y", Hand.HAND_CARDS_OFFSET.y, "time", ZoomTweenTime, "islocal", true));
            Zoomed = false;
        }
    }

    public void SwitchState(CardStates newState)
    {
        State = newState;
    }

    public abstract void BeforeTurnAction();
    public abstract void AfterTurnAction();
}
