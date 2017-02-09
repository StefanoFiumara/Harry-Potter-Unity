using System.Collections.Generic;
using System.Linq;

namespace HarryPotterUnity.Cards.Transfiguration.Items
{
    public class ColourChangingInk : BaseItem
    { 
        public override bool CanPerformInPlayAction()
        {
            return this.Player.CanUseActions() 
                && this.Player.Hand.Cards.Count > 0 
                && this.Player.IsLocalPlayer;
        }

        public override void OnInPlayAction(List<BaseCard> targets = null)
        {
            var cardsInHand = this.Player.Hand.Cards.ToList();
            int cardCount = cardsInHand.Count;

            this.Player.Deck.AddAll( cardsInHand );

            while (cardCount-- > 0)
            {
                this.Player.Deck.DrawCard();
            }

            this.Player.UseActions();
        }
    }
}