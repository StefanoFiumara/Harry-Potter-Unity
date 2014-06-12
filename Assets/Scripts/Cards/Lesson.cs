using UnityEngine;
using System.Collections;

public class Lesson : GenericCard {

    public enum LessonTypes
    {
        CREATURES, CHARMS, TRANSFIGURATION, POTIONS, QUIDDITCH
    }

    public LessonTypes Type;

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (GLOBAL.MainPlayer.ActionsAvailable > 0)
        {
            if (!GLOBAL.MainPlayer.LessonTypesInPlay.Contains(Type))
            {
                GLOBAL.MainPlayer.LessonTypesInPlay.Add(Type);
            }

            GLOBAL.MainPlayer.LessonsInPlay++;
            GLOBAL.MainPlayer.ActionsAvailable--;

            State = CardStates.IN_PLAY;
        }

        //iTween Animations here
        // x: -160, y = 6, z = 15 + number of cards this column?
    }
}
