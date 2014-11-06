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

        if (_Player.UseAction())
        {
            _Player._Hand.Remove(transform);
            _Player._InPlay.Add(transform, CardType);            
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
        State = CardStates.DISCARDED;
    }

    //Lesson Cards don't implement these methods
    public void OnInPlayBeforeTurnAction() { }
    public void OnInPlayAfterTurnAction() { }
    public void OnSelectedAction() { }
}
