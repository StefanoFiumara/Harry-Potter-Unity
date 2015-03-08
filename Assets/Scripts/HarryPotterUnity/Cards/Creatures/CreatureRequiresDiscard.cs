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
            return Player.InPlay.GetAmountOfLessonsOfType(Lesson.LessonTypes.Creatures) >= _lessonsToDiscard;
        }

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            for (var i = 0; i < _lessonsToDiscard; i++)
            {
                var lesson = Player.InPlay.GetLessonOfType(Lesson.LessonTypes.Creatures);

                Player.InPlay.Remove(lesson);
                Player.Discard.Add(lesson);
            }
        }
    }
}
