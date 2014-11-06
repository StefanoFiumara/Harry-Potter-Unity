using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Discard : MonoBehaviour {

    private List<GenericCard> Cards;
    public Player _Player; //not sure if needed just yet

    private readonly Vector2 DISCARD_POSITION_OFFSET = new Vector2(-355f, -30f);

	public void Start () {
	    Cards = new List<GenericCard>();

        if (gameObject.collider == null)
        {
            var col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(50f, 70f, 1f);
            col.center = new Vector3(DISCARD_POSITION_OFFSET.x, DISCARD_POSITION_OFFSET.y, 0f);
        }
	}

    public void Add(GenericCard card, float tweenDelay = 0f) 
    {
        Cards.Add(card);
        card.transform.parent = transform;

        Vector3 cardPos = new Vector3(DISCARD_POSITION_OFFSET.x, DISCARD_POSITION_OFFSET.y, 16f);
        cardPos.z -=  Cards.Count * 0.2f;

        Helper.TweenCardToPosition(card.transform, cardPos, GenericCard.CardStates.DISCARDED, tweenDelay);

        //Check if a card is being discarded from the board and call the appropriate Action method.
        var cardInfo = card.GetComponent<GenericCard>() as PersistentCard;
        if (cardInfo != null)
        {
            if (card.State == GenericCard.CardStates.IN_PLAY)
            {
                cardInfo.OnExitInPlayAction();
                //TODO: Rotate back to vertical position
            }
        }
    }

    //TODO: OnMouseUp: View cards in pile
}
