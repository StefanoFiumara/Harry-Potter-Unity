using UnityEngine;
using System.Collections;
using LessonTypes = Lesson.LessonTypes;

public abstract class GenericSpell : GenericCard {

    public LessonTypes CostType;
    public int CostAmount;

    public new void Start()
    {
        base.Start();
    }

    public void OnMouseUp()
    {
        if (State != CardStates.IN_HAND) return;

        if (_Player.UseAction())
        {
            if (_Player.nLessonsInPlay >= CostAmount && _Player.LessonTypesInPlay.Contains(CostType))
            {
                _Player._Hand.Remove(transform);
                //tween here
                //TODO: need a special tween for spell cards.
                OnPlayAction();
            }
        }
    }

    public abstract void OnPlayAction();
}
