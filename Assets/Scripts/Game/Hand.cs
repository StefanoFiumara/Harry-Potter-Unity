using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CardStates = GenericCard.CardStates;

public class Hand : MonoBehaviour {

    public const int SPACING = 25;

    public Player _Player;

    public float DrawCardTweenTime;

    public List<Transform> Cards;

	// Use this for initialization
	void Start () {
        Cards = new List<Transform>();
	}

    public void Add(Transform card)
    {
        Cards.Add(card);
        AnimateCardToHand(card);

    }

    private void AnimateCardToHand(Transform card)
    {
        Vector3 point1 = new Vector3(-77f, 4f, -220f);
        Vector3 point2 = new Vector3(-200f + Cards.Count * Hand.SPACING, -160f, 16f - Cards.Count);

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", point1,
                                                   "easetype", iTween.EaseType.easeOutExpo,
                                                   "islocal", true
                                                   ));

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", point2,
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
