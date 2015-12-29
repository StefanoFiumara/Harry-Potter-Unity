using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.Charms.Locations
{
    public class WandShop : BaseLocation
    {
        public override void OnEnterInPlayAction()
        {
            var allLessons =
                Player.InPlay.GetLessonsOfType(LessonTypes.Charms)
                    .Concat(Player.OppositePlayer.InPlay.GetLessonsOfType(LessonTypes.Charms)).Cast<BaseLesson>();

            foreach (var lesson in allLessons)
            {
                lesson.AmountLessonsProvided = 2;
            }

            Player.OnCardPlayedEvent += DoubleLessonsProvided;
            Player.OppositePlayer.OnCardPlayedEvent += DoubleLessonsProvided;
        }
        
        public override void OnExitInPlayAction()
        {
            var allLessons =
                Player.InPlay.GetLessonsOfType(LessonTypes.Charms)
                    .Concat( Player.OppositePlayer.InPlay.GetLessonsOfType(LessonTypes.Charms) )
                    .Cast<BaseLesson>();

            foreach (var lesson in allLessons)
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