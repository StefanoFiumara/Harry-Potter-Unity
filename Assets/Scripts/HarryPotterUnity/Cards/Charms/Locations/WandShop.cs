using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;
using HarryPotterUnity.Utils;

namespace HarryPotterUnity.Cards.Charms.Locations
{
    public class WandShop : BaseLocation
    {

        private IEnumerable<BaseLesson> AllLessons
        {
            get
            {
                return Player.InPlay.LessonsOfType(LessonTypes.Charms)
                    .Concat(Player.OppositePlayer.InPlay.LessonsOfType(LessonTypes.Charms))
                    .Cast<BaseLesson>();
            }
        }
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            foreach (var lesson in AllLessons)
            {
                lesson.AmountLessonsProvided = 2;
            }

            Player.OnCardPlayed += DoubleLessonsProvided;
            Player.OppositePlayer.OnCardPlayed += DoubleLessonsProvided;
        }
        
        public override void OnExitInPlayAction()
        {
            foreach (var lesson in AllLessons)
            {
                lesson.AmountLessonsProvided = 1;
            }

            Player.OnCardPlayed -= DoubleLessonsProvided;
            Player.OppositePlayer.OnCardPlayed -= DoubleLessonsProvided;
        }

        private void DoubleLessonsProvided(BaseCard card, List<BaseCard> targets)
        {
            var lesson = card as BaseLesson;

            if (lesson != null && lesson.LessonType == LessonTypes.Charms)
            {
                lesson.AmountLessonsProvided = 2;
            }
        }
    }
}