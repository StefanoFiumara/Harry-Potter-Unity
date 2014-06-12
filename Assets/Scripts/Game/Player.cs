using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;

public class Player : MonoBehaviour {

    public Hand _Hand;

    public Deck _Deck;

    public Transform StartingCharacter; //Set by main menu? GameObject?

    public int LessonsInPlay = 0;
    public List<LessonTypes> LessonTypesInPlay;

    public int ActionsAvailable = 2;

    public float DrawCardTweenTime;
	
	public void Start () {
        LessonTypesInPlay = new List<LessonTypes>(5);

        //Register with the GLOBAL class
        GLOBAL.MainPlayer = this;
	}
	
    public void DrawCard(bool needsAction = false)
    {
        if (needsAction && ActionsAvailable <= 0) return;
        Transform card = _Deck.TakeTopCard();
        _Hand.Add(card);
        card.parent = _Hand.transform;

        if (needsAction) ActionsAvailable--;

        AnimateCardToHand(card); //also updates card state after animation
    }

    private void AnimateCardToHand(Transform card)
    {
        Vector3 point1 = new Vector3(-77f, 4f, -220f);
        Vector3 point2 = new Vector3(-200f + _Hand.NumberOfCards() * Hand.SPACING, -160f, 16f - _Hand.NumberOfCards());

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", point1,
                                                   "easetype", iTween.EaseType.easeOutExpo
                                                   ));

        iTween.MoveTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                   "position", point2,
                                                   "delay", DrawCardTweenTime + 0.25f,
                                                   "easetype", iTween.EaseType.easeInOutSine
                                                   ));

        iTween.RotateTo(card.gameObject, iTween.Hash("time", DrawCardTweenTime,
                                                     "y", 0f,
                                                     "easetype", iTween.EaseType.easeInOutSine,
                                                     "oncomplete", "SwitchState",
                                                     "oncompletetarget", card.gameObject,
                                                     "oncompleteparams", CardStates.IN_HAND
                                                     ));
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 300, 150), "Shuffle"))
        {
            _Deck.Shuffle();
        }
    }
}
