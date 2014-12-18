using UnityEngine;
using System.Collections;

public class CreatureRequiresDiscard : GenericCreature, IPersistentCard {

    public int LessonsToDiscard;
    protected override bool MeetsAdditionalRequirements()
    {
        return _Player._InPlay.GetLessonsInPlay()
            .FindAll(card => (card as Lesson).LessonType == Lesson.LessonTypes.Creatures)
            .Count >= LessonsToDiscard;
    }

    public void OnEnterInPlayAction()
    {
        base.OnEnterInPlayAction();

        //remove lessons here
    }
}
