using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {
    public Player _Player;

    public float DrawCardTweenTime;

    public List<Transform> Cards;

    public static readonly Vector3 HAND_PREVIEW_POSITION = new Vector3(-77f, 4f, -220f);
    public static readonly Vector3 HAND_CARDS_OFFSET = new Vector3(-200f, -160f, 16f);
    public static readonly float SPACING = 25f;

	// Use this for initialization
	public void Start () {
        Cards = new List<Transform>();
	}

    public void Add(Transform card)
    {
        Cards.Add(card);
        AnimateCardToHand(card);

    }

    public void Remove(Transform card)
    {
        Cards.Remove(card);
        //TODO: Adjust Hand after card removal
        //AdjustHandSpacing();
    }
    private void AnimateCardToHand(Transform card)
    {
        Vector3 cardPosition = HAND_CARDS_OFFSET;

        cardPosition.x += Cards.Count * Hand.SPACING;
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
