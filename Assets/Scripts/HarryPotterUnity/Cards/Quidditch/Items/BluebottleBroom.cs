using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class BluebottleBroom : ItemLessonProvider
    {
        public override void OnSelectedAction()
        {
            var card = Player.Discard.GetHealableCards(1).Single();
            
            Player.Deck.Add(card);

            Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.IsLocalPlayer;
        }
    }
}
