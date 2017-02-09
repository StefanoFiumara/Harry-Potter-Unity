using System.Collections.Generic;
using System.Linq;
using HarryPotterUnity.Cards.BasicBehavior;

namespace HarryPotterUnity.Cards.Quidditch.Items
{
    public class BluebottleBroom : ItemLessonProvider
    {
        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var card = this.Player.Discard.NonHealingCards.FirstOrDefault();

            if (card != null)
            {
                this.Player.Deck.Add(card);
            }

            this.Player.UseActions();
        }

        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() && this.Player.IsLocalPlayer;
        }
    }
}
