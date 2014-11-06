using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;
using CardTypes = GenericCard.CardTypes;

public class Player : MonoBehaviour {

    public Hand _Hand;
    public Deck _Deck;
    public InPlay _InPlay;
    public Player _OppositePlayer;
    public Discard _Discard;

    public Transform StartingCharacter; //Set by main menu? GameObject?

    public int nLessonsInPlay = 0;
    public List<LessonTypes> LessonTypesInPlay;

    public int ActionsAvailable = 2;

    public int nCreaturesInPlay;

	public void Start () {
        LessonTypesInPlay = new List<LessonTypes>(5);
	}

    public bool UseAction()
    {
        //TODO: also check for next turn here?
        //TODO: Clamp ActionsAvailable to 0-99
        return ActionsAvailable-- > 0;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        
        for (int i = 0; i < amount; i++)
        {
            Transform card = _Deck.TakeTopCard();

            
            iTween.MoveBy(card.gameObject, iTween.Hash("time", 0.5f,
                                                        "z", 20f,
                                                        "easetype", iTween.EaseType.easeInOutSine,
                                                        "delay", i * 0.3f
                                                        ));

            iTween.RotateTo(card.gameObject, iTween.Hash("time", 0.2f,
                                                        "y", 0f,
                                                        "easetype", iTween.EaseType.easeInOutSine,
                                                        "delay", i * 0.3f + 0.4f
                                                        ));
            _Discard.Add(card, 0.3f + i * 0.3f);
        }


    }

}
