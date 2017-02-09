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
                return this.Player.InPlay.LessonsOfType(LessonTypes.Charms)
                    .Concat(this.Player.OppositePlayer.InPlay.LessonsOfType(LessonTypes.Charms))
                    .Cast<BaseLesson>();
            }
        }
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            foreach (var lesson in this.AllLessons)
            {
                lesson.AmountLessonsProvided = 2;
            }

            this.Player.OnCardPlayedEvent += this.DoubleLessonsProvided;
            this.Player.OppositePlayer.OnCardPlayedEvent += this.DoubleLessonsProvided;
        }
        
        public override void OnExitInPlayAction()
        {
            foreach (var lesson in this.AllLessons)
            {
                lesson.AmountLessonsProvided = 1;
            }

            this.Player.OnCardPlayedEvent -= this.DoubleLessonsProvided;
            this.Player.OppositePlayer.OnCardPlayedEvent -= this.DoubleLessonsProvided;
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