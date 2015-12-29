using HarryPotterUnity.Cards.BasicBehavior;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Charms.Items
{
    [UsedImplicitly]
    public class BrokenWand : ItemLessonProvider
    {
        public override void OnSelectedAction()
        {
            var card = Player.Deck.TakeTopCard();

            if (card is BaseLesson)
            {
                Player.InPlay.Add(card);
            }
            else
            {
                Player.Discard.Add(card);
            }

            Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions();
        }
    }
}
