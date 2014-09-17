using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {
    public Player _Player;

    public float DrawCardTweenTime;

    public List<Transform> Cards;

    public static readonly Vector3 HAND_PREVIEW_POSITION = new Vector3(-80f, -13f, -336f);
    public static readonly Vector3 HAND_CARDS_OFFSET = new Vector3(-240f, -200f, 0f);
    public static readonly float SPACING = 55f;

	// Use this for initialization
	public void Start () {
        Cards = new List<Transform>();
	}

    public void Add(Transform card)
    {
        AnimateCardToHand(card);
        AdjustHandSpacing();
        Cards.Add(card);
    }

    public void Remove(Transform card)
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

            iTween.MoveTo(Cards[i].gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                           "position", cardPosition,
                                                           "easetype", iTween.EaseType.easeInOutSine,
                                                           "islocal", true,
                                                           "oncomplete", "SwitchState",
                                                           "oncompletetarget", Cards[i].gameObject,
                                                           "oncompleteparams", CardStates.IN_HAND
                                                           ));
        }
    }

    private void AnimateCardToHand(Transform card)
    {
        Vector3 cardPosition = HAND_CARDS_OFFSET;

        float shrinkFactor = Cards.Count >= 12 ? 0.5f : 1f;

        cardPosition.x += Cards.Count * Hand.SPACING * shrinkFactor;
        cardPosition.z -= Cards.Count;

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", HAND_PREVIEW_POSITION,
                                                   "easetype", iTween.EaseType.easeOutExpo,
                                                   "islocal", true
                                                   ));

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", cardPosition,
                                                   "delay", DrawCardTweenTime + 0.25f,
                                                   "easetype", iTween.EaseType.easeInOutSine,
                                                   "islocal", true,
                                                   "oncomplete", "SwitchState",
                                                   "oncompletetarget", card.gameObject,
                                                   "oncompleteparams", CardStates.IN_HAND
                                                   ));

        iTween.RotateTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                     "y", 0f,
                                                     "easetype", iTween.EaseType.easeInOutSine
                                                     ));
    }

    public int NumberOfCards()
    {
        return Cards.Count;
    }
}
