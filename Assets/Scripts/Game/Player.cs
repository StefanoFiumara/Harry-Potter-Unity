using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LessonTypes = Lesson.LessonTypes;
using CardStates = GenericCard.CardStates;

public class Player : MonoBehaviour {

    public Hand _Hand;
    public Deck _Deck;
    public InPlay _InPlay;

    public Transform StartingCharacter; //Set by main menu? GameObject?

    public int LessonsInPlay = 0;
    public List<LessonTypes> LessonTypesInPlay;

    public int ActionsAvailable = 2;

	public void Start () {
        LessonTypesInPlay = new List<LessonTypes>(5);

        //Register with the GLOBAL class
        GLOBAL.MainPlayer = this;
	}
	
    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 300, 150), "Shuffle"))
        {
            _Deck.Shuffle();
        }
    }
}
