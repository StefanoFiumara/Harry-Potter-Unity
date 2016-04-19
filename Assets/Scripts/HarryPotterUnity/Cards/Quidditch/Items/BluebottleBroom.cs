using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class BluebottleBroom : ItemLessonProvider
    {
        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var card = Player.Discard.GetHealableCards().FirstOrDefault();

            if (card != null)
            {
                Player.Deck.Add(card);
            }

            Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return Player.CanUseActions() && Player.IsLocalPlayer;
        }
    }
}
