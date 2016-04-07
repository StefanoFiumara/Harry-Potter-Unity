using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Locations
{
    public class WandShop : BaseLocation
    {

        private IEnumerable<BaseLesson> AllLessons
        {
            get
            {
                return Player.InPlay.Lessons.Cast<BaseLesson>()
                    .Concat(Player.OppositePlayer.InPlay.Lessons.Cast<BaseLesson>())
                    .Where(c => c.LessonType == LessonTypes.Charms);
            }
        }
        public override void OnEnterInPlayAction()
        {
            foreach (var lesson in AllLessons)
            {
                lesson.AmountLessonsProvided = 2;
            }

            Player.OnCardPlayedEvent += DoubleLessonsProvided;
            Player.OppositePlayer.OnCardPlayedEvent += DoubleLessonsProvided;
        }
        
        public override void OnExitInPlayAction()
        {
            foreach (var lesson in AllLessons)
            {
                lesson.AmountLessonsProvided = 1;
            }

            Player.OnCardPlayedEvent -= DoubleLessonsProvided;
            Player.OppositePlayer.OnCardPlayedEvent -= DoubleLessonsProvided;
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