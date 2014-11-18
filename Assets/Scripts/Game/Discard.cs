using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Discard : MonoBehaviour {

    private List<GenericCard> Cards;
    public Player _Player; //not sure if needed just yet

    public static readonly Vector2 DISCARD_POSITION_OFFSET = new Vector2(-355f, -30f);

    public static readonly Vector3 PREVIEW_OFFSET = new Vector3(-300f, -30f, -6f);

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

    public void Add(GenericCard card) 
    {
        Cards.Add(card);
        card.transform.parent = transform;

        Vector3 cardPos = new Vector3(DISCARD_POSITION_OFFSET.x, DISCARD_POSITION_OFFSET.y, 16f);
        cardPos.z -=  Cards.Count * 0.2f;

        Vector3 cardPreviewPos = cardPos;
        cardPreviewPos.z -= 20f;

        Helper.AddTweenToQueue(card, cardPreviewPos, 0.35f, 0f, CardStates.DISCARDED, card.State == CardStates.IN_DECK, card.State == CardStates.IN_PLAY);
        Helper.AddTweenToQueue(card, cardPos, 0.25f, 0f, CardStates.DISCARDED, false, false);
    }

    //TODO: OnMouseUp: View cards in discard pile
}
