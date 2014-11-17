using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {
    public Player _Player;

    public float DrawCardTweenTime;

    public List<GenericCard> Cards;

    public static readonly Vector3 HAND_PREVIEW_POSITION = new Vector3(-80f, -13f, -336f);
    public static readonly Vector3 HAND_CARDS_OFFSET = new Vector3(-240f, -200f, 0f);
    public static readonly float SPACING = 55f;

	public void Start () {
      //  Cards = new List<GenericCard>();
	}

    public void Add(GenericCard card, bool flip = true, bool preview = true, float animDelay = 0f)
    {
        card.transform.parent = transform;
        AnimateCardToHand(card.transform, flip, preview, animDelay);
        //AdjustHandSpacing();
        Cards.Add(card);
    }

    public void Remove(GenericCard card)
    {
        Cards.Remove(card);
        AdjustHandSpacing();
    }

    private void AdjustHandSpacing()
    {
        Vector3 cardPosition;
        
        float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;
        
        for (int i = 0; i < Cards.Count; i++)
        {
            cardPosition = HAND_CARDS_OFFSET;

            cardPosition.x += i * Hand.SPACING * shrinkFactor;
            cardPosition.z -= i;

            Helper.TweenCardToPosition(Cards[i].transform, cardPosition, CardStates.IN_HAND);
        }
    }

    private void AnimateCardToHand(Transform card, bool flip = true, bool preview = true, float animDelay = 0f)
    {
        Vector3 cardPosition = HAND_CARDS_OFFSET;

        float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

        cardPosition.x += Cards.Count * Hand.SPACING * shrinkFactor;
        cardPosition.z -= Cards.Count;

        if (preview)
        {
            Helper.TweenCardToPosition(card, HAND_PREVIEW_POSITION, card.GetComponent<GenericCard>().State, animDelay, iTween.EaseType.easeOutExpo);
        }

       Helper.TweenCardToPosition(card, cardPosition, CardStates.IN_HAND, animDelay + DrawCardTweenTime + 0.25f);

        if (flip)
        {
            iTween.RotateTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                         "y", 0f,
                                                         "easetype", iTween.EaseType.easeInOutSine,
                                                         "delay", animDelay
                                                         ));
        }
    }
}
