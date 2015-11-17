using HarryPotterUnity.Cards.BasicBehavior;
using JetBrains.Annotations;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    [UsedImplicitly]
    public class BluebottleBroom : ItemLessonProvider
    {
        public override void OnSelectedAction()
        {
            var card = Player.Discard.GetHealableCards(1);

            Player.Discard.RemoveAll(card);
            Player.Deck.AddAll(card);

            Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions();
        }
    }
}
