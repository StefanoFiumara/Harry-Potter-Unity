using UnityEngine;
using System.Collections;

public class Lesson : GenericCard {

    public enum LessonTypes
    {
        CREATURES, CHARMS, TRANSFIGURATION, POTIONS, QUIDDITCH
    }

    public LessonTypes LessonType;

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.ActionsAvailable > 0)
        {
            if (!_Player.LessonTypesInPlay.Contains(LessonType))
            {
                _Player.LessonTypesInPlay.Add(LessonType);
            }

            _Player._Hand.Remove(transform);
            _Player._InPlay.Add(transform, CardType);

            _Player.LessonsInPlay++;
            _Player.ActionsAvailable--;

            State = CardStates.IN_PLAY;
        }
    }
}
