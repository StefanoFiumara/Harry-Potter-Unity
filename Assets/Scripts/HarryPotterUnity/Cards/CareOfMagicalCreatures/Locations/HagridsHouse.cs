using HarryPotterUnity.Enums;

namespace HarryPotterUnity.Cards.CareOfMagicalCreatures.Locations
{
    public class HagridsHouse : BaseLocation
    {
        public override void OnEnterInPlayAction()
        {
            base.OnEnterInPlayAction();

            Player.InPlay.OnCardExitedPlay += ReturnCreatureLessonToHand;
            Player.OppositePlayer.InPlay.OnCardExitedPlay += ReturnCreatureLessonToHand;
        }

        public override void OnExitInPlayAction()
        {
            base.OnExitInPlayAction();

            Player.InPlay.OnCardExitedPlay -= ReturnCreatureLessonToHand;
            Player.OppositePlayer.InPlay.OnCardExitedPlay -= ReturnCreatureLessonToHand;
        }

        private void ReturnCreatureLessonToHand(BaseCard card)
        {
            var lesson = card as BaseLesson;
            if (lesson != null && lesson.LessonType == LessonTypes.Creatures)
            {
                card.Player.Hand.Add(card);
            }
        }
    }
}