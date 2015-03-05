using System.Linq;

namespace HarryPotterUnity.Cards.Creatures
{
    public class CreatureRequiresDiscard : GenericCreature, IPersistentCard {

        public int LessonsToDiscard;
        protected override bool MeetsAdditionalCreatureRequirements()
        {
            return Player.InPlay.GetLessonsInPlay()
                .FindAll(card => ((Lesson) card).LessonType == Lesson.LessonTypes.Creatures)
                .Count >= LessonsToDiscard;
        }

        public new void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            for (var i = 0; i < LessonsToDiscard; i++)
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
