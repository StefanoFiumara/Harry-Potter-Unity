using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace HarryPotterUnity.Cards.Creatures
{
    public class CreatureRequiresDiscard : GenericCreature, IPersistentCard {
        
        [UsedImplicitly, SerializeField]
        private int _lessonsToDiscard;

        protected override bool MeetsAdditionalCreatureRequirements()
        {
            return Player.InPlay.GetLessonsInPlay()
                .FindAll(card => ((Lesson) card).LessonType == Lesson.LessonTypes.Creatures)
                .Count >= _lessonsToDiscard;
        }

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            for (var i = 0; i < _lessonsToDiscard; i++)
            {
                var lesson =
                    Player.InPlay.GetLessonsInPlay()
                                 .First(x => ((Lesson) x).LessonType == Lesson.LessonTypes.Creatures);

                Player.InPlay.Remove(lesson);
                Player.Discard.Add(lesson);
            }
        }
    }
}
