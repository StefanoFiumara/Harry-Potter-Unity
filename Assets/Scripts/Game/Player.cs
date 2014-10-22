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
        return ActionsAvailable-- > 0;
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 75), "Shuffle"))
        {
            _Deck.Shuffle();
        }
    }


}
