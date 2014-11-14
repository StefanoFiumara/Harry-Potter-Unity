using UnityEngine;
using System.Collections;

public class Lesson : GenericCard, PersistentCard {

    public enum LessonTypes
    {
        CREATURES, CHARMS, TRANSFIGURATION, POTIONS, QUIDDITCH
    }

    public LessonTypes LessonType;

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        //TODO: Only use action if player meets all other requirements!
        if (_Player.UseAction())
        {
            _Player._Hand.Remove(this);
            _Player._InPlay.Add(this);            
        }
    }

    public void OnEnterInPlayAction()
    {
        if (!_Player.LessonTypesInPlay.Contains(LessonType))
        {
            _Player.LessonTypesInPlay.Add(LessonType);
        }
        
        _Player.nLessonsInPlay++;

        State = CardStates.IN_PLAY;
    }

    public void OnExitInPlayAction()
    {
        _Player.nLessonsInPlay--;
        _Player.UpdateLessonTypesInPlay();
    }

    //Lesson Cards don't implement these methods
    public void OnInPlayBeforeTurnAction() { }
    public void OnInPlayAfterTurnAction() { }
    public void OnSelectedAction() { }
}
